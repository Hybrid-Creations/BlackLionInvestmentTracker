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

    CancellationTokenSource cancelSource;
    CancellationTokenSource currentDelaySource;

    public void Start(bool startNow)
    {
        cancelSource = new CancellationTokenSource();

        bool firstRun = true;

        Task.Run(async () =>
        {
            do
            {
                currentDelaySource?.Dispose();
                currentDelaySource = new CancellationTokenSource();
                if (cancelSource.IsCancellationRequested)
                    break;

                if (firstRun && startNow) { } // Don't delay if we want it to Invoke on first run
                else
                {
                    try
                    {
                        await Task.Delay(Interval - offset, currentDelaySource.Token);
                    }
                    catch (System.Exception e) 
                    { 
                        if(e is not TaskCanceledException)
                        {
                            cancelSource.Cancel(); // Safely fail out the timer
                            GD.PushError(e); // But show the error
                        }
                    } 
                }

                if (cancelSource.IsCancellationRequested)
                    break;

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    GD.Print("Timer has elapsed.");
                    await Elapsed?.Invoke();
                }
                catch (Exception e)
                {
                    GD.PushError(e);
                }

                offset = stopwatch.Elapsed; // This makes it so the tiemr always runs every X interval, as it takes away the time it took to run the code
                stopwatch.Stop();

                firstRun = false;
            }
            while (Repeat && cancelSource.IsCancellationRequested == false);

            currentDelaySource?.Dispose();
        }, cancelSource.Token);
    }

    public void Stop()
    {
        cancelSource.Cancel();
    }

    public void InvokeASAP()
    {
        currentDelaySource.Cancel();
    }
}
