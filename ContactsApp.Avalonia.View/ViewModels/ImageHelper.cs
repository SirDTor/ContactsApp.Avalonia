using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Avalonia.View.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <returns></returns>
        public static Bitmap LoadFromResource(Uri resourceUri)
        {
            return new Bitmap(AssetLoader.Open(resourceUri));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<Bitmap?> LoadFromWeb(Uri url)
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsByteArrayAsync();
                return new Bitmap(new MemoryStream(data));
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public static Bitmap ByteToImage(byte[] blob)
        {
            using (MemoryStream memoryStream = new())
            {
                memoryStream.Write(blob, 0, blob.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                Bitmap bitmap = new Bitmap(memoryStream);
                return bitmap;
            }
        }
    }
}
