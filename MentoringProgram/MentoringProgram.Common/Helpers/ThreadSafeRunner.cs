using System;
using System.Threading;
using System.Threading.Tasks;

namespace MentoringProgram.Common.Helpers
{
    public static class ThreadSafeRunner
    {
        public static async Task Run(Func<Task> action, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                await action?.Invoke();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
