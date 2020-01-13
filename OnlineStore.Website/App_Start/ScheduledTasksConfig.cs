using OnlineStore.DataLayer;
using OnlineStore.Services;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.Website
{
    public static class ScheduledTasksConfig
    {
        public static int count = 0;
        public static void Configure()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(1)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }

    public class EmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            EmailServices.SendEmails(20);
        }
    }
}