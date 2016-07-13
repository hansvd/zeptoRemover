using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZeptoRemove
{
    class ZeptoHelper
    {

        public static bool IsInfected(string destDir, string[] files)
        {
            return files.Any(f => Path.GetExtension(f)?.Equals(".zepto") ?? false);
        }

        public static int  RemoveInfected(string[] files)
        {
            int nrZepto = 0;
            foreach (var f in files)
            {
                var ext = Path.GetExtension(f);
                if (ext == null || (ext != ".zepto" && ext != ".html"))
                    continue;
                if (ext == ".html")
                {
                    var fName = Path.GetFileName(f);
                    if (fName == null || !Regex.IsMatch(fName, "_\\d+_HELP_instructions.html"))
                        continue;
                }
                if (ext == ".zepto") nrZepto ++;
                File.Delete(f);
            }
            return nrZepto;
        }

        public static int CopyMissing(string destDir, string backupDir)
        {
            if (backupDir == null) return 0;
            var backupFiles = Directory.GetFiles(backupDir);
            if (backupFiles.Length == 0) return 0;
            var destFiles = Directory.GetFiles(destDir);
            var nrCopied = 0;
            foreach (var f in backupFiles)
            {
                var fName = Path.GetFileName(f);
                if (fName == null) continue;
                if (destFiles.Any(d => fName.Equals(Path.GetFileName(d))))
                    continue;

                File.Copy(f,Path.Combine(destDir,fName));
                nrCopied++;
            }
            return nrCopied;
        }
    }
}
