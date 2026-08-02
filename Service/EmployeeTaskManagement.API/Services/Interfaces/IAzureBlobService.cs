using System.Runtime.InteropServices;

namespace EmployeeTaskManagement.API.Services.Interfaces
{
    
    public interface IAzureBlobService
    {
        Task<Stream> DownloadAsync(string blobName);
    }
}
