﻿using System;
using System.IO;
using ZeptoRemove;

namespace Restore
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine($"usage: ZeptoRemove.exe backupDir destDir [go]");
                return -1;
            }

            var backupDir = args[0];
            var destDir = args[1];
            var go = args.Length > 2 && args[2] == "go";

            if (!Directory.Exists(backupDir))
            {
                Console.WriteLine($"Can find backup dir {backupDir}");
                return -1;
            }
            if (!Directory.Exists(destDir))
            {
                Console.WriteLine($"Can find dir {destDir}");
                return -1;
            }
            new ZeptoRemover(backupDir,destDir, !go).Invoke();
            return 0;
        }
    }
}
