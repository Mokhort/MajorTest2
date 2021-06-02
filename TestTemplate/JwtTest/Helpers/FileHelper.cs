using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.Helpers
{
    public static class FileHelper
    {
        public static async Task WriteToFile(this IFormFile file, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(fs);
        }
    }
}
