using NLog;
using Quartz;
using Quartz.Impl;
using System;

namespace IdleFileCleaner {
    public class FileCleanerScheduler : IDisposable {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        IScheduler _scheduler;

        public void Start() {
            try {
                Logger.Info("Service is starting...");
                ISchedulerFactory schedFact = new StdSchedulerFactory();                
                _scheduler = schedFact.GetScheduler();
                _scheduler.Start();

                IJobDetail job = JobBuilder.Create<FileCleaningJob>()
                                           .WithIdentity("IdleFileCleaner", "Cleaner")
                                           .Build();
                ITrigger trigger = TriggerBuilder.Create()
                                                 .WithCalendarIntervalSchedule
                                                  (s => s.WithIntervalInDays(1)).Build();
                _scheduler.ScheduleJob(job, trigger);
                Logger.Info("Service is started.");
            } catch (ArgumentException e) {
                Console.WriteLine("File cleaner scheduler exception", e);
            }
        }

        public void Stop() {
            Logger.Info("Service is stop.");
        }

        public void Dispose() {
            _scheduler.Clear();
        }
    }
}
