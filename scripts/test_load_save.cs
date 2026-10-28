using System;
using System.IO;
using System.Reflection;
using System.Data.SQLite;

class TestSave
{
    static void Main()
    {
        string gameDir = @"D:\SteamLibrary\steamapps\common\Zero Falls";
        string exePath = Path.Combine(gameDir, "WTF.exe");

        Directory.SetCurrentDirectory(gameDir);

        try
        {
            var asm = Assembly.LoadFrom(exePath);
            var charDataType = asm.GetType("CoOpSpRpG.CHARACTER_DATA");
            var loadMethod = charDataType.GetMethod("loadWorldCharacter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            Console.WriteLine("Invoking loadWorldCharacter('Thiago', 'Thiago')...");
            loadMethod.Invoke(null, new object[] { "Thiago", "Thiago" });
            Console.WriteLine("SUCCESS: loadWorldCharacter executed cleanly with NO errors!");

            // Now test reading the world save blob
            string dbPath = Path.Combine(gameDir, @"worldsaves\Thiago.sqlite");
            byte[] worldData = null;
            using (var conn = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT data FROM world LIMIT 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            worldData = (byte[])reader["data"];
                        }
                    }
                }
            }

            if (worldData != null)
            {
                Console.WriteLine("World data blob length: " + worldData.Length + " bytes.");
                var gameFileType = asm.GetType("CoOpSpRpG.GameFile");
                var ctor = gameFileType.GetConstructor(new Type[] { typeof(byte[]) });
                if (ctor != null)
                {
                    Console.WriteLine("Constructing GameFile(blob)...");
                    var gf = ctor.Invoke(new object[] { worldData });
                    Console.WriteLine("SUCCESS: GameFile(blob) deserialized successfully with NO crashes!");
                }
            }
        }
        catch (TargetInvocationException tie)
        {
            Console.WriteLine("TARGET INVOCATION ERROR: " + tie.InnerException);
            Console.WriteLine(tie.InnerException.StackTrace);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex);
        }
    }
}
