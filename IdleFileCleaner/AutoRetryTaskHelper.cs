using System;
using System.Threading.Tasks;

namespace IdleFileCleaner {
    public static class AutoRetryTaskHelper {
        public static async Task Run(Func<Task> action, Func<Exception, bool> errorHandler, TimeSpan retryDelay, int retryCount) {
            while (--retryCount > 0) {
                try {
                    await action();
                    return;
                } catch (Exception ex) {
                    if (!errorHandler(ex)) return;
                }
                await Task.Delay(retryDelay);
            }
        }

        public static async Task Run(Func<Task> action, Func<Exception, bool> errorHandler, TimeSpan retryDelay) {
            while (true) {
                try {
                    await action();
                    return;
                } catch (Exception ex) {
                    if (!errorHandler(ex)) {
                        return;
                    }
                }
                await Task.Delay(retryDelay);
            }
        }

        public static async Task<T> Run<T>(Func<Task<T>> func, Func<Exception, bool> errorHandler, TimeSpan retryDelay) {
            while (true) {
                try {
                    return await func();
                } catch (Exception ex) {
                    if (!errorHandler(ex)) {
                        return default(T);
                    }
                }
                await Task.Delay(retryDelay);
            }
        }
    }
}
