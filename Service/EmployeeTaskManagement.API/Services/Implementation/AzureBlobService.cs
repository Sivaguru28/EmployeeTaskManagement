using Azure.Identity;
using Azure.Storage.Blobs;
using EmployeeTaskManagement.API.Common.Attributes;
using EmployeeTaskManagement.API.Configurations;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace EmployeeTaskManagement.API.Services.Implementation
{
    [ScopedService]
    public class AzureBlobService : IAzureBlobService
    {
        private readonly BlobContainerClient _containerClient;


        public AzureBlobService(IOptions<AzureStorageConfig> options)
        {
            var settings = options.Value;

            var serviceClient = new BlobServiceClient(
                new Uri($"https://{settings.AccountName}.blob.core.windows.net"),
                new DefaultAzureCredential());

            _containerClient = serviceClient.GetBlobContainerClient(settings.ContainerName);
        }

        public async Task<Stream> DownloadAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"Blob '{blobName}' not found.");
            }

            var response = await blobClient.DownloadStreamingAsync();

            return response.Value.Content;
        }

    }
}


