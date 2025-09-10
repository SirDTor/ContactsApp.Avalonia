using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.IO;

public static class AvatarGenerator
{
    private static readonly Random random = new Random();

    public static (Bitmap bitmap, byte[] bytes)? GenerateRandomAvatar(int width = 64, int height = 64)
    {
        try
        {
            // создаем RenderTargetBitmap
            var rtb = new RenderTargetBitmap(new PixelSize(width, height), new Vector(96, 96));

            using (var ctx = rtb.CreateDrawingContext(true))
            {
                // рисуем каждый пиксель как маленький прямоугольник
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte r = (byte)random.Next(256);
                        byte g = (byte)random.Next(256);
                        byte b = (byte)random.Next(256);

                        var brush = new SolidColorBrush(Color.FromArgb(255, r, g, b));
                        ctx.FillRectangle(brush, new Rect(x, y, 1, 1));
                    }
                }
            }

            // сохраняем в MemoryStream
            using var ms = new MemoryStream();
            rtb.Save(ms);
            ms.Position = 0;

            var bitmap = new Bitmap(ms);
            return (bitmap, ms.ToArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}