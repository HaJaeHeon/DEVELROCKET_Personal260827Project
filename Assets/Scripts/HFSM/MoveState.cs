using UnityEngine;
using UnityHFSM;

public class MoveState : StateBase
{
    public MoveState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
}
