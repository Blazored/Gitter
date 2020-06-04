using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core
{
    public class DebounceHelper
    {
        private CancellationTokenSource debounceToken = null;

        public async Task DebounceAsync(Func<CancellationToken, Task> func, int delayMilliseconds = 1000)
        {
            // https://stackoverflow.com/a/62196612/

            // Cancel previous task
            if (debounceToken != null) { debounceToken.Cancel(); }

            // Assign new token
            debounceToken = new CancellationTokenSource();

            await Task.Delay(delayMilliseconds, debounceToken.Token);

            if (debounceToken.IsCancellationRequested)
                return;

            await func(debounceToken.Token);
        }
    }
}
