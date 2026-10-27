using System;
using System.IO;
using System.Xml;

class TestTutorials
{
    static void Main()
    {
        string baseDir = @"D:\SteamLibrary\steamapps\common\Zero Falls\Tutorials";
        string[] dir1 = Directory.GetDirectories(baseDir);

        foreach (string d1 in dir1)
        {
            Console.WriteLine("DIR 1: " + d1);
            string[] dir2 = Directory.GetDirectories(d1);
            foreach (string d2 in dir2)
            {
                Console.WriteLine("  DIR 2: " + d2);
                string[] files = Directory.GetFiles(d2, "*.xml*");
                Console.WriteLine("    Files: " + files.Length);
                foreach (var f in files)
                {
                    Console.WriteLine("      File: " + f);
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(f);
                        XmlNodeList names = doc.GetElementsByTagName("Name");
                        if (names == null || names.Count == 0)
                        {
                            Console.WriteLine("        [CRASH] No <Name> tag found in " + f);
                        }
                        else
                        {
                            Console.WriteLine("        Name: " + names[0].InnerText);
                        }

                        XmlNodeList pages = doc.GetElementsByTagName("Page");
                        Console.WriteLine("        Pages: " + pages.Count);
                        for (int p = 0; p < pages.Count; p++)
                        {
                            var page = pages[p];
                            var img = page.SelectSingleNode("img");
                            if (img != null)
                            {
                                var src = img.Attributes["src"];
                                Console.WriteLine("          Page " + p + " img src: " + (src != null ? src.Value : "NULL SRC ATTR!"));
                                if (src == null)
                                {
                                    Console.WriteLine("          [CRASH] <img> tag has NO src attribute!");
                                }
                            }
                            var body = page.SelectSingleNode("body");
                            if (body == null)
                            {
                                Console.WriteLine("          [CRASH] <Page> tag has NO <body>!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("        [EXCEPTION] " + ex.ToString());
                    }
                }
            }
        }
    }
}
