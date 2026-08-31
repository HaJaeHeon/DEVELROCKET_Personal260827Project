using UnityEngine;
using UnityHFSM;

public class IdleState : StateBase
{
    public IdleState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
}
