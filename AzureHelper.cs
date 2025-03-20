using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;

namespace AzureUploader
{
    public class AzureHelper
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;

        public AzureHelper(string connectionString, string containerName = null)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            if (!string.IsNullOrEmpty(containerName))
            {
                _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
        }

        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (BlobContainerItem container in _blobServiceClient.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        public async Task<string> UploadFileAsync(string fileName, string baseDirectory)
        {
            try
            {
                string filePath = Path.Combine(baseDirectory, fileName);
                if (!File.Exists(filePath))
                    return "Error: File does not exist in the specified directory!";

                string blobName = Path.GetFileName(filePath);
                BlobClient blobClient = _containerClient.GetBlobClient(blobName);

                using FileStream fileStream = File.OpenRead(filePath);
                await blobClient.UploadAsync(fileStream, overwrite: true);

                return $"File '{blobName}' uploaded successfully to container '{_containerClient.Name}'!";
            }
            catch (Exception ex)
            {
                return $"Error uploading file: {ex.Message}";
            }
        }

        public async Task<string> RetrieveFileAsync(string blobName, string downloadPath)
        {
            try
            {
                BlobClient blobClient = _containerClient.GetBlobClient(blobName);
                if (!await blobClient.ExistsAsync())
                    return $"Error: File '{blobName}' not found in container '{_containerClient.Name}'.";

                BlobDownloadInfo download = await blobClient.DownloadAsync();
                string localFilePath = Path.Combine(downloadPath, blobName);

                using FileStream fileStream = File.Create(localFilePath);
                await download.Content.CopyToAsync(fileStream);

                return $"File '{blobName}' downloaded successfully from container '{_containerClient.Name}' to '{localFilePath}'!";
            }
            catch (Exception ex)
            {
                return $"Error downloading file: {ex.Message}";
            }
        }
    }
}
