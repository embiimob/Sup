using System;
using System.IO;

class Program
{
    static bool IsUnsupportedImageFormat(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return false;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length < 12) return false;

                byte[] header = new byte[12];
                fs.Read(header, 0, 12);

                // Check for RIFF WEBP
                if (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 && // RIFF
                    header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)   // WEBP
                {
                    return true;
                }

                // Check for ftypavif (AVIF)
                if (header[4] == 0x66 && header[5] == 0x74 && header[6] == 0x79 && header[7] == 0x70 && // ftyp
                    header[8] == 0x61 && header[9] == 0x76 && header[10] == 0x69 && header[11] == 0x66)   // avif
                {
                    return true;
                }
            }
        }
        catch { }

        return false;
    }

    static void Main()
    {
        Console.WriteLine("test3.jpeg (avif): " + IsUnsupportedImageFormat("test3.jpeg"));
        Console.WriteLine("test5.svg (svg): " + IsUnsupportedImageFormat("test5.svg"));
    }
}
