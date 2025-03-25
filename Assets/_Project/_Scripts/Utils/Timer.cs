using System;

public abstract class Timer
{
    protected float CurrentTime;
    
    public Action OnTimerStart = delegate { };
    public Action<float> OnTimerTick = delegate { };
    public Action OnTimerStop = delegate { };
    
    public bool IsRunning { get; protected set; }
    
    public virtual void Start() {
        if (IsRunning) return;
        IsRunning = true;
        OnTimerStart.Invoke();
    }

    public virtual void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        OnTimerStop.Invoke();
    }
    
    public void Resume() => IsRunning = true;
    public void Pause() => IsRunning = false;
    
    public abstract void Tick(float deltaTime);
    public abstract float Process();
}

public class StopwatchTimer : Timer
{
    public override void Tick(float deltaTime)
    {
        if (!IsRunning) return;
        CurrentTime += deltaTime;
    }

    public override float Process()
    {
        return CurrentTime;
    }
}

public class CountdownTimer : Timer
{
    private float _duration;
    
    public CountdownTimer(float duration)
    {
        this._duration = duration;
    }

    public override void Start()
    {
        CurrentTime = _duration;
        base.Start();
    }
    
    public override void Tick(float deltaTime)
    {
        if (!IsRunning) return;
        
        CurrentTime -= deltaTime;
        
        OnTimerTick.Invoke(Process());

        if (CurrentTime > 0) return;
        
        CurrentTime = 0;
        Stop();
    }

    public override float Process()
    {
        return 1 - CurrentTime / _duration;
    }
    
    public bool IsFinished => CurrentTime <= 0;
}
