using HAdministradora.Infra.CrossCutting.Aws.Interfaces.Services;
using Presentation.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Helpers
{
    public class ConvertToBase64
    {
        IBucketS3Service _bucketS3Service;

        public async Task<string> ConvertImageToBase64(string path)
        {

            var imageBucketS3 = await _bucketS3Service.DownloadObjectAsync(path);
            if (imageBucketS3 != null)
            {

                byte[] imageBytes = ConverteStreamToByteArray(imageBucketS3);

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            return null;
        }

        public byte[] ConverteStreamToByteArray(Stream stream)
        {
            byte[] byteArray = new byte[16 * 1024];
            using (MemoryStream mStream = new MemoryStream())
            {
                int bit;
                while ((bit = stream.Read(byteArray, 0, byteArray.Length)) > 0)
                {
                    mStream.Write(byteArray, 0, bit);
                }
                return mStream.ToArray();
            }
        }
    }
}
