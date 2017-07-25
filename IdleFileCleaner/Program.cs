using System;
using Topshelf;

namespace IdleFileCleaner {
    class Program {
        static void Main(string[] args) {
            HostFactory.Run(x => {
                x.Service <FileCleanerScheduler> (s => {
                    s.ConstructUsing(name => new FileCleanerScheduler());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription("Idle File Cleaner Service");
                x.SetDisplayName("Idle File Cleaner Service");
                x.SetServiceName("IdleFileCleanerService");
            });
        }
    }
}
