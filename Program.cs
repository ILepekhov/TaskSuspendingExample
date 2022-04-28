using TaskSuspendingExample;

using CancellationTokenSource globalCts = new();
CancellationToken globalCancellationToken = globalCts.Token;
TaskSuspender suspender = new();

Task worker = Task.Run(async () =>
{
    try
    {
        Console.WriteLine("Starting Worker");
        
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), globalCancellationToken);
                
            await suspender.WaitIfSuspended();
            
            Console.WriteLine($"Worker: {DateTime.Now}");            
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Worker was completed");
    }
});

await Task.Delay(TimeSpan.FromSeconds(5));

Console.WriteLine("Suspending Worker");
suspender.SetSuspendSignal(globalCancellationToken);

await Task.Delay(TimeSpan.FromSeconds(10));

Console.WriteLine("Resuming Worker");
suspender.SetResumeSignal();

await Task.Delay(TimeSpan.FromSeconds(3));

Console.WriteLine("Suspending Worker");
suspender.SetSuspendSignal(globalCancellationToken);

await Task.Delay(TimeSpan.FromSeconds(10));

Console.WriteLine("Globally cancelling");
globalCts.Cancel();

await Task.Delay(TimeSpan.FromSeconds(3));