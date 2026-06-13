using Obstor;
using Obstor.Model;

// Ensure that Obstor is running:
//   docker run --rm -p 9000:9000 ghcr.io/obstor/obstor:latest server /data

// Create Obstor client
var obstorClient = new ObstorClientBuilder("http://localhost:9000")
    .WithStaticCredentials("obstoradmin", "obstoradmin")
    .Build();

// Create the test-bucket (if it doesn't exist)
const string testBucket = "testbucket";
var hasBucket = await obstorClient.BucketExistsAsync(testBucket).ConfigureAwait(false);
if (!hasBucket)
    await obstorClient.CreateBucketAsync(testBucket).ConfigureAwait(false);

Console.WriteLine($"Bucket '{testBucket}' is ready.");
