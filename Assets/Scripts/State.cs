using UnityEngine;

public abstract class State
{
    protected PlayerContext PlayerContext;

    public State(PlayerContext playerContext)
    {
        PlayerContext = playerContext;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
