using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Obstor;
using Obstor.Model;

// Ensure that Obstor is running:
//   docker run --rm -p 9000:9000 ghcr.io/obstor/obstor:latest server /data

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging
    .AddSimpleConsole(/*opt => opt.SingleLine = true*/)
    .SetMinimumLevel(LogLevel.Debug);

// Add Obstor
builder.Services
    .AddObstor("http://localhost:9000")
    .WithStaticCredentials("obstoradmin", "obstoradmin");

// Obtain a host
using var host = builder.Build();

// Obtain a Obstor client
var obstorClient = host.Services.GetRequiredService<IObstorClient>();

// Create the test-bucket (if it doesn't exist)
const string testBucket = "testbucket";
var hasBucket = await obstorClient.BucketExistsAsync(testBucket).ConfigureAwait(false);
if (!hasBucket)
    await obstorClient.CreateBucketAsync(testBucket).ConfigureAwait(false);

// Listen for all bucket events
var observable = await obstorClient
    .ListenBucketNotificationsAsync(testBucket, [
        EventType.ObjectCreatedAll,
        EventType.ObjectAccessedAll,
        EventType.ObjectRemovedAll
    ])
    .ConfigureAwait(false);

using var subscription = observable.Subscribe(e => Console.WriteLine($"{e.S3.Bucket.Name}:{e.S3.Object.Key} - {e.EventName}"));

// Write out 100 objects in parallel
var buffer = new byte[256];
for (var i = 0; i < buffer.Length; ++i)
    buffer[i] = (byte)i;

await Task.WhenAll(Enumerable.Range(0, 100).Select(i => $"test-{i:D04}").Select(async key =>
{
    var ms = new MemoryStream(buffer, false);
    await using (ms.ConfigureAwait(false))
    {
        await obstorClient.PutObjectAsync(testBucket, key, ms).ConfigureAwait(false);
    }
})).ConfigureAwait(false);

// Read an object file
await using var stream = await obstorClient.GetObjectAsync(testBucket, "test-0000").ConfigureAwait(false);
await using (stream.ConfigureAwait(false))
{
    // TODO: Do something with the stream
}

// List all objects starting with "test-" in the test-bucket
// (max 20 objects at a time)
await foreach (var objItem in obstorClient.ListObjectsAsync(testBucket, prefix: "test-", delimiter: "/", pageSize: 20).ConfigureAwait(false))
    Console.WriteLine($"{objItem.Key,-40} {objItem.Size,10} bytes, etag: {objItem.ETag}");
