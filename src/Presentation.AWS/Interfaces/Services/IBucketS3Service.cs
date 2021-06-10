using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAdministradora.Infra.CrossCutting.Aws.Interfaces.Services
{
    public interface IBucketS3Service
    {
        Task<string> UploadObjectAsync(Stream fileStream, string filePath);
        Task<Stream> DownloadObjectAsync(string filePath);
        Task<bool> DeleteSingleObject(string filePath);
    }
}
