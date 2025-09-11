using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateBase<T> where T : Player<T>
{
    protected T state;
    public StateBase(T next)
    {
        state = next;
    }

    // ó‘Ô‚É“ü‚é‚Æ‚«‚Ìˆ—
    public virtual void OnEnterState() { }
    // ó‘Ô‚ÌXVˆ—
    public virtual void OnUpdate() { }
    // ó‘Ô‚©‚ço‚é‚Æ‚«‚Ìˆ—
    public virtual void OnExitState() { }
}