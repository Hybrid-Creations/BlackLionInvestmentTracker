using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace BLIT.Timers;

public class BetterTimer
{
    public TimeSpan Interval { get; set; }
    public Action Elapsed { get; set; }
    public bool Repeat { get; set; } = false;

    TimeSpan offset = TimeSpan.Zero;

    CancellationTokenSource source;
    CancellationTokenSource currentDelaySource;

    public void Start(bool startNow)
    {
        source = new CancellationTokenSource();

        bool firstRun = true;
        Task.Run(async () =>
        {
            do
            {
                currentDelaySource?.Dispose();
                currentDelaySource = new CancellationTokenSource();
                if (source.IsCancellationRequested)
                    break;

                if (firstRun && startNow) { } // Don't delay if we want it to Invoke on first run
                else
                {
                    try
                    {
                        await Task.Delay(Interval - offset, currentDelaySource.Token);
                    }
                    catch (System.Exception) { }
                }

                if (source.IsCancellationRequested)
                    break;

                var stopwatch = Stopwatch.StartNew();

                Elapsed?.Invoke();

                offset = stopwatch.Elapsed;
                stopwatch.Stop();

                firstRun = false;
            }
            while (Repeat && source.IsCancellationRequested == false);

            currentDelaySource?.Dispose();
        }, source.Token);
    }

    public void Stop()
    {
        source.Cancel();
    }

    public void InvokeASAP()
    {
        currentDelaySource.Cancel();
    }
}
