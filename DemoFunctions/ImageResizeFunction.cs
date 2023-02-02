using Microsoft.Azure.WebJobs;
using SixLabors.ImageSharp.Processing;
using System;
using System.Drawing;
using System.IO;
using sharp = SixLabors.ImageSharp;

namespace DemoFunctions
{
    [StorageAccount("BlobConnection")]
    public class ImageResizeFunction
    {
        private static readonly Size size = new Size(EnvAsInt("ImageResizeWidth"), EnvAsInt("ImageResizeHeight"));

        [FunctionName(nameof(ImageResizeFunction))]
        public static void ExecuteAsync([BlobTrigger("img/{name}")] Stream original, [Blob("imgprocessed/{name}", FileAccess.Write)] Stream resized)
        {
            using (var image = sharp.Image.Load(original))
            {
                image.Mutate(x => x
                    .Resize(size.Width, size.Height)
                );
                image.Save(resized, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
            }
        }

        private static int EnvAsInt(string name) => int.Parse(Env(name));
        private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}
