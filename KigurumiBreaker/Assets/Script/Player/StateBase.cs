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

    // ��Ԃɓ���Ƃ��̏���
    public virtual void OnEnterState() { }
    // ��Ԃ̍X�V����
    public virtual void OnUpdate() { }
    // ��Ԃ���o��Ƃ��̏���
    public virtual void OnExitState() { }
}