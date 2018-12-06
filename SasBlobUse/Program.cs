using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;


namespace SasBlobUse
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("DefaultStorage");
            Console.WriteLine(connectionString);

            //Parse the connection string and return a reference to the storage account.
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            //Create the blob client object.
            var blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            var container = blobClient.GetContainerReference(Constants.Container);

            var sasBuilder = new AccessSignatureBuilder(container);

            var sasTokens = await sasBuilder.GenerateSignaturesAsync();

            var tester = new SasTester(sasTokens);

            var defaultColor = Console.ForegroundColor;
            foreach (var test in tester)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Test: {test.TestName} {(test.Passed ? "Passed!" : "Failed :(")}");
                Console.ForegroundColor = defaultColor;
            }
            
            Console.ReadLine();
        }

        
    }
}
