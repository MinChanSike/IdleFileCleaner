using NLog;
using Quartz;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace IdleFileCleaner {
    public class FileCleaningJob : IJob {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context) {
            Console.WriteLine("File Cleaning Job Execute at {0}", DateTime.Now.ToLongTimeString());
            Task.Run(() => CheckIdleFiles());            
        }

        private async Task CheckIdleFiles() {
            try {
                foreach (var item in AppSettings.GetSourcePaths()) {
                    foreach (var file in Directory.EnumerateFiles(item, AppSettings.FileExtension)) {
                        await CleanIdleFile(file);
                    }
                }
            } catch (Exception ex) {
                Logger.Error(ex, "CheckIdleFiles Exception");
            }
        }

        private async Task CleanIdleFile(string path) {
            FileInfo file = new FileInfo(path);
            if (file.LastWriteTime < DateTime.Now.AddDays(-AppSettings.IdleLimit)) {
                if (AppSettings.Action.ToLower() == "delete")
                    await DeleteFileTask(file);
                else if (AppSettings.Action.ToLower() == "move") {
                    await MoveFileTask(file);
                    await DeleteFileTask(file);
                } else {
                    Logger.Info("No action found to do.");
                }
            }
        }

        private async Task DeleteFileTask(FileInfo fInfo) {
            Console.WriteLine("DeleteFile: {0}", fInfo.Name);
            await AutoRetryTaskHelper.Run(async () => {
                await Task.Run(() => File.Delete(fInfo.FullName));
                Logger.Info("Have been deleted file: {0}", fInfo.Name);
            },
            ex => {
                Logger.Error($"Error occur when delete file task:{ex}");
                return true;
            }, TimeSpan.FromSeconds(AppSettings.RetryInterval));
        }

        private async Task MoveFileTask(FileInfo fInfo) {          
            await AutoRetryTaskHelper.Run(async () =>
                    await CopyFile(fInfo.FullName, AppSettings.DistinationPath),
                    ex => {
                        Logger.Error($"Error occur when move file task:{ex}");
                        return true;
                    }, TimeSpan.FromSeconds(AppSettings.RetryInterval), 3);
        }

        private async Task CopyFile(string sourcePath, string distinationPath) {
            using (FileStream SourceStream = File.Open(sourcePath, FileMode.Open)) {
                using (FileStream DestinationStream = File.Create(distinationPath + sourcePath.Substring(sourcePath.LastIndexOf('\\')))) {
                    await SourceStream.CopyToAsync(DestinationStream);
                    Logger.Info("Have been copy to distination: {0}", distinationPath);
                }
            }
        }


    }
}
