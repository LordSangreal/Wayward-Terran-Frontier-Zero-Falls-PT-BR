using System;
using System.IO;

public class ReadCrashLog {
    public static void Main() {
        string path = @"D:\SteamLibrary\steamapps\common\Zero Falls\logs\crashlog4.bin";
        if (!File.Exists(path)) {
            Console.WriteLine("File not found");
            return;
        }

        try {
            using (var fs = File.OpenRead(path))
            using (var reader = new BinaryReader(fs)) {
                // Read whatever is in the binary stream
                while (fs.Position < fs.Length) {
                    try {
                        string s = reader.ReadString();
                        Console.WriteLine("String: " + s);
                    } catch {
                        fs.Position = 0;
                        byte[] b = reader.ReadBytes((int)fs.Length);
                        Console.WriteLine("Raw ASCII/UTF8: \n" + System.Text.Encoding.UTF8.GetString(b));
                        break;
                    }
                }
            }
        } catch (Exception ex) {
            Console.WriteLine("Error reading crashlog: " + ex);
        }
    }
}
