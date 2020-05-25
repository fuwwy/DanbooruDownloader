﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E621Downloader.Utilities
{
    public static class TaskUtility
    {
        public static async Task RunWithRetry(Func<Task> job, Func<Exception, bool> retryCondition, int retryCount, int retryDelay)
        {
            do
            {
                try
                {
                    await job.Invoke();
                }
                catch (Exception e)
                {
                    if (!retryCondition(e))
                    {
                        throw;
                    }
                    retryCount--;

                    await Task.Delay(retryDelay);
                    continue;
                }

                break;
            } while (retryCount >= 0);
        }
    }
}
