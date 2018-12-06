using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SasBlobUse
{
    public class AccessSignatureBuilder
    {
        private readonly CloudBlobContainer _container;
        public AccessSignatureBuilder(CloudBlobContainer container)
        {
            _container = container;
        }

        private string GetContainerSasUri(CloudBlobContainer container)
        {
            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Write
                              |SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Delete
            };
            var sasContainerToken = container.GetSharedAccessSignature(sasConstraints);
            return container.Uri + sasContainerToken;
        }

        public async Task<AccessSignatures> GenerateSignaturesAsync()
        {
            var containerSas = GetContainerSasUri(_container);
            var blobSas = await GetBlobSasUri(_container).ConfigureAwait(false);
            var perms = await _container.GetPermissionsAsync().ConfigureAwait(false);
            perms.SharedAccessPolicies.Clear();
            await _container.SetPermissionsAsync(perms);
            await CreateSharedAccessPolicy(_container, Constants.AccessPolicy);
            var containerSasWithPolicy = GetContainerSasUriWithPolicy(_container, Constants.AccessPolicy);
            var blobSasWithPolicy = await GetBlobSasUriWithPolicy(_container, Constants.AccessPolicy).ConfigureAwait(false);

            return new AccessSignatures(containerSas, blobSas, containerSasWithPolicy, blobSasWithPolicy);
        }
        
        async Task<string> GetBlobSasUri(CloudBlobContainer container)
        {
            var blob = container.GetBlockBlobReference(Constants.SasBlob);
            var blobContent = "This blob will be accessible to clients via a shared access signature (SAS).";
            await blob.UploadTextAsync(blobContent);
            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
                              |SharedAccessBlobPermissions.Delete
            };
            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);
            return blob.Uri + sasBlobToken;
        }

        private async Task CreateSharedAccessPolicy(CloudBlobContainer container,
            string policyName)
        {
            var permissions = await container.GetPermissionsAsync();
            var sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read
                              |SharedAccessBlobPermissions.Delete
            };
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            await container.SetPermissionsAsync(permissions);
        }

        private string GetContainerSasUriWithPolicy(CloudBlobContainer container, string policyName)
        {
            var sasContainerToken = container.GetSharedAccessSignature(null, policyName);
            return container.Uri + sasContainerToken;
        }

        static async Task<string> GetBlobSasUriWithPolicy(CloudBlobContainer container, string policyName)
        {
            var blob = container.GetBlockBlobReference(Constants.SasBlobPolicy);
            var blobContent = "This blob will be accessible to clients via a shared access signature. " +
                                 "A stored access policy defines the constraints for the signature.";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(blobContent));
            ms.Position = 0;
            using (ms)
            {
                await blob.UploadFromStreamAsync(ms);
            }
            var sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            return blob.Uri + sasBlobToken;
        }
    }
}
