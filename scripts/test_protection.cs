using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class TestProtection
{
    static void Main()
    {
        string backupExe = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF_backup.exe";
        var asm = AssemblyDefinition.ReadAssembly(backupExe);

        int totalReplaced = 0;
        int skippedReadWrite = 0;

        foreach (var type in asm.MainModule.Types)
        {
            if (type.Name == "TutorialTemplate" || type.Name == "TUTORIAL_BAG" || type.Name == "WorldDatabase" || type.Name == "AttackPlayerWithNonFactionStage")
                continue;
            if (type.Name == "LoaderProc" || type.Name == "MenuBackground" || type.Name == "MenuBackgroundV2" || type.Name == "MONSTERBAG")
                continue;
            if (type.BaseType != null && type.BaseType.Name == "ParticleEffect")
                continue;

            // Serialization protections
            if (type.Name == "TriggerEvent" || type.Name == "AGENTBAG" || type.Name == "AgentTracker" || type.Name == "GameFile")
            {
                skippedReadWrite++;
                continue;
            }

            foreach (var m in type.Methods)
            {
                if (!m.HasBody) continue;
                string mNameLower = m.Name.ToLowerInvariant();
                if (mNameLower.StartsWith("read") || mNameLower.StartsWith("write") ||
                    mNameLower.StartsWith("save") || mNameLower.StartsWith("loadgame") ||
                    m.Name == "setPhase" || m.Name == "cleanUpQuests" || m.Name == "adoptAgent")
                {
                    skippedReadWrite++;
                    continue;
                }

                if (m.Name == "drawBumpyStuff" || m.Name == "combinePasses" || m.Name == "applyToScreen" || m.Name == "drawGraphs")
                    continue;
                if (m.Name == "LoadContent" || m.Name == "loadContent" || m.Name == "LoadParticleEffect")
                    continue;
                if (type.Name == "Game1" && (m.Name == ".ctor" || m.Name == "Initialize"))
                    continue;
                if (type.Name == "LoaderProc" && m.Name == "loadSound")
                    continue;
                if (type.Name == "ShipNavigationRev3" && (m.Name == "startSounds" || m.Name == "spawnFire"))
                    continue;
                if (type.Name == "Ship" && m.Name == "playTravelCharge")
                    continue;
                if (type.Name == "HackingScreen" && m.Name == "Update")
                    continue;
                if ((type.Name == "ShipDesignRev4" || type.Name == "ShipDesignRev3") && (m.Name == "InitCategories" || m.Name == "Init"))
                    continue;
                if (type.BaseType != null && type.BaseType.Name == "TriggerEvent" && m.Name == ".ctor")
                    continue;

                foreach (var i in m.Body.Instructions)
                {
                    if (i.OpCode == OpCodes.Ldstr)
                    {
                        totalReplaced++;
                    }
                }
            }
        }

        Console.WriteLine("Protected test run:");
        Console.WriteLine("Skipped serialization methods/classes: " + skippedReadWrite);
        Console.WriteLine("Total ldstr candidates still available for translation: " + totalReplaced);
    }
}
