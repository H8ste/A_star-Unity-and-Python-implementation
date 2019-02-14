using System.Threading;

/// <summary>
///     The superclass that you should derive from. It provides Start() and Stop() method and Running property.
///     It will start the thread to run Run() when you call Start().
/// </summary>
public abstract class RunAbleThread
{
    private readonly Thread _runnerThread;

    protected RunAbleThread()
    {
        // we need to create a thread instead of calling Run() directly because it would block unity
        // from doing other tasks like drawing game scenes
        _runnerThread = new Thread(Run);
    }

    public bool Running { get; set; }
    public bool Send { get; set; }

    /// <summary>
    /// This method will get called when you call Start(). This method make sure that
    /// this method terminates in a finite time. You can use Running property (which will be set to false when Stop() is
    /// called) to determine when you should stop the method.
    /// </summary>
    protected abstract void Run();

    public void Start()
    {
        Running = true;
        _runnerThread.Start();

    }

    public void Continue()
    {
        Send = true;
        //_runnerThread.Resume();

    }

    public void Pause()
    {
        Send = false;
        //_runnerThread.Suspend();
    }

    public void Stop()
    {
        Running = false;
        // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
        // _runnerThread will end before main thread end
        _runnerThread.Join();
    }
}