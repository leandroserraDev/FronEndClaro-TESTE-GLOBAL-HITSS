using Presentation.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Helpers
{
    public static class ConvertToBase64 
    {
        public static async Task<string> ConvertImageToBase64(string path)
        {

            using (Image image = Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return await Task.FromResult(base64String);
                }
            }
        }

    }
}
