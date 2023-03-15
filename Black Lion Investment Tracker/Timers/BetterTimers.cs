using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace BLIT.Timers;

public class ThreadedTimer
{
    public TimeSpan Interval { get; set; }
    public Func<Task> Elapsed { get; set; }
    public bool Repeat { get; set; } = false;

    TimeSpan offset = TimeSpan.Zero;

    CancellationTokenSource source;
    CancellationTokenSource currentDelaySource;

    public void Start(bool startNow)
    {
        source = new CancellationTokenSource();

        bool firstRun = true;

        // Thread thread = new(() =>
        // {
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

                try
                {
                    await Elapsed?.Invoke();
                }
                catch (Exception e)
                {
                    GD.PushError(e);
                }

                offset = stopwatch.Elapsed;
                stopwatch.Stop();

                firstRun = false;
            }
            while (Repeat && source.IsCancellationRequested == false);

            currentDelaySource?.Dispose();
        }, source.Token);
        // });
        // thread.Start();
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

