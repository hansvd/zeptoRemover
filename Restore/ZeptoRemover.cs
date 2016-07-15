using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace ZeptoRemove
{
    public class ZeptoRemover
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _backupDir;
        private readonly string _destDir;
        private readonly bool _dryRun;

        public ZeptoRemover(string backupDir, string destDir, bool dryRun)
        {
            _backupDir = backupDir;
            _destDir = destDir;
            _dryRun = dryRun;
        }

        public void Invoke()
        {           
            CleanUpDir(_backupDir,_destDir);
        }

        void CleanUpDir(string backupDir, string destDir)
        {
            try
            {
                CleanUpDirBody(backupDir, destDir);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{destDir} <= {backupDir}");
            }
        }

        private void CleanUpDirBody(string backupDir, string destDir)
        {
// check if contains zepto files
            var destFiles = Directory.GetFiles(destDir);
            bool isInfected = ZeptoHelper.IsInfected(destFiles);

            if (isInfected)
                Logger.Info($"{destDir} <= {backupDir}");
            // check if src contains available
            if (isInfected && backupDir == null)
            {
                Logger.Warn($"Cannot restore dir {destDir}");
                if (!_dryRun)
                    File.WriteAllText(Path.Combine(destDir, "ZiptoRemovalWarning.txt"), $"No backups found");
            }

            // cleanup
            Cleanup(backupDir, destDir, destFiles);
            CleanupChilds(backupDir, destDir);
        }

        private void Cleanup(string backupDir, string destDir, string[] destFiles)
        {
            try
            {
                var helper = new ZeptoHelper(_dryRun);
                var nrRemoved = helper.RemoveInfected(destFiles);
                if (nrRemoved == 0) return;
                var nrCopied = helper.CopyMissing(destDir, backupDir);

                if (nrRemoved > nrCopied)
                {
                    Logger.Warn($"Not all files restored for {destDir}, {nrRemoved} removed, {nrCopied} restored");
                    if (!_dryRun)
                        File.WriteAllText(Path.Combine(destDir,"ZiptoRemovalWarning.txt"),$"Not al files restored in this dir: {nrCopied}/{nrRemoved}");
                }
                else
                {
                    Logger.Info($"Cleanup {nrRemoved} for {destDir}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e,$"Cannot cleanup dir {destDir}");
            }
        }

        private void CleanupChilds(string backupDir, string destDir)
        {
           
            var backupChilds = backupDir == null ? new List<string>() : Directory.GetDirectories(backupDir).ToList();
            var destChildDirs = Directory.GetDirectories(destDir);
            foreach (var destChild in destChildDirs)
            {
                var childName = Path.GetFileName(destChild);
                if (childName == null) continue;
                if (childName == "$RECYCLE.BIN") continue;
                if (childName == "System Volume Information") continue;
                if (childName.StartsWith("GEINFECTEERD")) continue;

                var backupChild =
                    backupChilds.FirstOrDefault(
                        d => childName.Equals(Path.GetFileName(d), StringComparison.InvariantCultureIgnoreCase));
                CleanUpDir(backupChild, destChild);
                backupChilds.Remove(backupChild);
            }

            // Log all the unused backup dirs
            foreach (var backupChild in backupChilds)
            {
                Logger.Warn($"Backup dir {backupChild} not used!");
            }
        }
    }
}
