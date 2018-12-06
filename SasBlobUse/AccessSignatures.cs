using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SasBlobUse
{
    public class AccessSignatures
    {
        public AccessSignatures(string containerSas, string blobSas, string containerSasWithAccessPolicy, string blobSasWithAccessPolicy)
        {
            ContainerSas = containerSas;
            BlobSas = blobSas;
            ContainerSasWithAccessPolicy = containerSasWithAccessPolicy;
            BlobSasWithAccessPolicy = blobSasWithAccessPolicy;
        }
        public string ContainerSas { get; }
        public string BlobSas { get; }
        public string ContainerSasWithAccessPolicy { get; }
        public string BlobSasWithAccessPolicy { get; }

    }
}
