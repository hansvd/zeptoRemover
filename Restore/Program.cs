using System;
using System.IO;

namespace Restore
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"usage: ZeptoRemove.exe backupDir destDir");
                return -1;
            }

            var backupDir = args[0];
            var destDir = args[1];

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
            new ZeptoRemover(backupDir,destDir).Invoke();
            return 0;
        }
    }
}
