using System;
using System.IO;

public class ReadCrashLog {
    public static void Main(string[] args) {
        string path = args.Length > 0 ? args[0] : @"D:\SteamLibrary\steamapps\common\Zero Falls\logs\crashlog1.bin";
        if (!File.Exists(path)) {
            Console.WriteLine("File not found: " + path);
            return;
        }

        try {
            using (var fs = File.OpenRead(path))
            using (var reader = new BinaryReader(fs)) {
                byte[] b = reader.ReadBytes((int)fs.Length);
                Console.WriteLine("=== CRASHLOG CONTENT ===");
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(b));
            }
        } catch (Exception ex) {
            Console.WriteLine("Error reading crashlog: " + ex);
        }
    }
}
