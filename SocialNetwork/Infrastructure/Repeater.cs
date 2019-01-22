using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SocialNetwork.Infrastructure
{
    public static class Repeater
    {
        private static readonly Queue<(Func<bool>, int)> jobs = new Queue<(Func<bool>, int)>();
        private static readonly object jobsLock = new object();

        static Repeater()
        {
            new Thread(() =>
            {
                while (true)
                {
                    List<(Func<bool>, int)> currentJobs;
                    lock (jobsLock)
                        currentJobs = jobs.ToList();
                    for (int i = 0; i < currentJobs.Count; i++)
                    {
                        var (job, attemptsCount) = currentJobs[i];
                        try
                        {
                            if (!job())
                                EnqueueJob(job, attemptsCount - 1);
                        }
                        catch
                        {
                            EnqueueJob(job, attemptsCount - 1);
                        }
                    }
                    Thread.Sleep(250);
                }
            }).Start();
        }

        public static void EnqueueJob(Func<bool> job, int attemptsCount)
        {
            if (attemptsCount > 0)
                lock (jobsLock)
                    jobs.Enqueue((job, attemptsCount));
        }
    }
}
