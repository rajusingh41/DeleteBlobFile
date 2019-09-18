using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteBlobFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var containerName = "yourContainerName";
            var dirRef = "dgn5/qrcode/6";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            foreach (IListBlobItem blob in container.GetDirectoryReference(dirRef).ListBlobs(true))
            {

                if (blob.GetType() == typeof(CloudBlob) || blob.GetType().BaseType == typeof(CloudBlob))
                {
                    var blockBlob = container.GetBlockBlobReference(((CloudBlob)blob).Name);
                    blockBlob.FetchAttributes();
                    var timemodified = blockBlob.Properties.LastModified;
                    if (blockBlob.Properties.LastModified < DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)))
                    {
                        ((CloudBlob)blob).DeleteIfExists();
                    }
                }
            }
        }
    }
}
