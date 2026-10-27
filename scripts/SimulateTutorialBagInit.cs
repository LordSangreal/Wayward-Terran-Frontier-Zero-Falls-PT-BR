using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

class SimulateTutorialBagInit
{
    static void Main()
    {
        string currentDir = @"D:\SteamLibrary\steamapps\common\Zero Falls";
        string tutDir = currentDir + @"\Tutorials";
        Console.WriteLine("Tutorial Dir: " + tutDir);

        string[] dir1 = Directory.GetDirectories(tutDir);
        for (int i = 0; i < dir1.Length; i++)
        {
            DirectoryInfo di1 = new DirectoryInfo(dir1[i]);
            string catName = di1.Name;
            Console.WriteLine("[" + i + "] Cat: " + catName + " (" + dir1[i] + ")");

            string[] dir2 = Directory.GetDirectories(dir1[i]);
            for (int j = 0; j < dir2.Length; j++)
            {
                DirectoryInfo di2 = new DirectoryInfo(dir2[j]);
                string subName = di2.Name;
                Console.WriteLine("    [" + j + "] Sub: " + subName + " (" + dir2[j] + ")");

                string[] files = Directory.GetFiles(dir2[j], "*.xml*");
                Console.WriteLine("        Files: " + (files != null ? files.Length : 0));

                XmlDocument doc = new XmlDocument();
                if (files != null && files.Length > 0)
                {
                    doc.Load(files[0]);
                }
                else
                {
                    Console.WriteLine("        [WARNING] NO FILES IN " + dir2[j]);
                }

                try
                {
                    TestTemplateCtor(doc, dir2[j]);
                    Console.WriteLine("        -> Template loaded successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("        -> [CRASH IN TEMPLATE CTOR]: " + ex.ToString());
                }
            }
        }
    }

    static void TestTemplateCtor(XmlDocument doc, string dir)
    {
        XmlNodeList names = doc.GetElementsByTagName("Name");
        string name = names[0].InnerText;

        XmlNodeList pages = doc.GetElementsByTagName("Page");
        for (int p = 0; p < pages.Count; p++)
        {
            var page = pages[p];
            var img = page.SelectSingleNode("img");
            if (img != null)
            {
                string src = img.Attributes["src"].Value;
                string fullImg = dir + "\\" + src;
            }
            var body = page.SelectSingleNode("body");
            if (body != null)
            {
                string xml = body.InnerXml;
            }
        }
    }
}
