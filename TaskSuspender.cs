namespace TaskSuspendingExample;

public class TaskSuspender
{
    private TaskCompletionSource<object?>? _tcs;
    private readonly object _lock = new();

    private IDisposable? _tokenSubscription;

    public void SetSuspendSignal(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            DisposeTokenSubscription();
            
            if (_tcs is {Task.IsCompleted: false})
            {
                _tcs.SetResult(null);
            }
            
            _tcs = new TaskCompletionSource<object?>();

            _tokenSubscription = cancellationToken.Register(SetResumeSignal);
        }
    }

    public void SetResumeSignal()
    {
        lock (_lock)
        {
            DisposeTokenSubscription();
            
            if (_tcs is {Task.IsCompleted: false})
            {
                _tcs.SetResult(null);
            }
        }
    }

    public async Task WaitIfSuspended()
    {
        await GetTaskToAwait();
    }

    private Task GetTaskToAwait()
    {
        lock (_lock)
        {
            return _tcs?.Task ?? Task.CompletedTask;
        }
    }

    private void DisposeTokenSubscription()
    {
        _tokenSubscription?.Dispose();
        _tokenSubscription = null;
    }
}