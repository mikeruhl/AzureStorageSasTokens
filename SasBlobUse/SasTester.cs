using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SasBlobUse
{
    public class SasTester :IEnumerable<SasTest>
    {
        private readonly AccessSignatures _signatures;

        public SasTester(AccessSignatures signatures)
        {
            _signatures = signatures;
        }

        public async Task<bool> RunContainerSasTest()
        {
            return await ContainerTest(_signatures.ContainerSas);
        }

        public async Task<bool> RunContainerSasWithAccessPolicyTest()
        {
            return await ContainerTest(_signatures.ContainerSasWithAccessPolicy);
        }

        public async Task<bool> RunBlobSasTest()
        {
            return await BlobTest(_signatures.BlobSas);
        }

        public async Task<bool> RunBlobSasWithAccessPolicyTest()
        {
            return await BlobTest(_signatures.BlobSasWithAccessPolicy);
        }

        private async Task<bool> ContainerTest(string sas)
        {
            var passed = true;
            var container = new CloudBlobContainer(new Uri(sas));
            var blobList = new List<ICloudBlob>();
            try
            {
                var blob = container.GetBlockBlobReference("blobCreatedViaSAS.txt");
                var blobContent = "This blob was created with a shared access signature granting write permissions to the container. ";
                await blob.UploadTextAsync(blobContent);
                Console.WriteLine("Write operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {

                Console.WriteLine("Write operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
                passed = false;
            }

            //List operation: List the blobs in the container.
            try
            {
                foreach (ICloudBlob blob in container.ListBlobs())
                {
                    blobList.Add(blob);
                }
                Console.WriteLine("List operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("List operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
                passed = false;
            }

            //Read operation: Get a reference to one of the blobs in the container and read it.
            try
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(blobList[0].Name);
                MemoryStream msRead = new MemoryStream();
                msRead.Position = 0;
                using (msRead)
                {
                    blob.DownloadToStream(msRead);
                    Console.WriteLine(msRead.Length);
                }
                Console.WriteLine("Read operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Read operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
                passed = false;
            }
            Console.WriteLine();

            //Delete operation: Delete a blob in the container.
            try
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(blobList[0].Name);
                blob.Delete();
                Console.WriteLine("Delete operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Delete operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
                passed = false;
            }

            return passed;
        }

        private async Task<bool> BlobTest(string sas)
        {
            var passed = true;
            var blob = new CloudBlockBlob(new Uri(sas));
            try
            {
                var blobContent = "This blob was created with a shared access signature granting write permissions to the blob. ";
                var msWrite = new MemoryStream(Encoding.UTF8.GetBytes(blobContent))
                {
                    Position = 0
                };
                using (msWrite)
                {
                    await blob.UploadFromStreamAsync(msWrite);
                }
                Console.WriteLine("Write operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Write operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
                passed = false;
            }

            //Read operation: Read the contents of the blob.
            try
            {
                var msRead = new MemoryStream();
                using (msRead)
                {
                    await blob.DownloadToStreamAsync(msRead);
                    msRead.Position = 0;
                    using (var reader = new StreamReader(msRead, true))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
                Console.WriteLine("Read operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Read operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
                passed = false;
            }

            //Delete operation: Delete the blob.
            try
            {
                await blob.DeleteAsync();
                Console.WriteLine("Delete operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Delete operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
                passed = false;
            }

            return passed;
        }

        public IEnumerator<SasTest> GetEnumerator()
        {
            yield return new SasTest(){TestName = nameof(RunContainerSasTest), Passed = RunContainerSasTest().GetAwaiter().GetResult()};
            yield return new SasTest() { TestName = nameof(RunContainerSasWithAccessPolicyTest), Passed = RunContainerSasWithAccessPolicyTest().GetAwaiter().GetResult() };
            yield return new SasTest() { TestName = nameof(RunBlobSasTest), Passed = RunBlobSasTest().GetAwaiter().GetResult() };
            yield return new SasTest() { TestName = nameof(RunBlobSasWithAccessPolicyTest), Passed = RunBlobSasWithAccessPolicyTest().GetAwaiter().GetResult() };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
