using UnityEngine;
using UnityHFSM;

public class DicePhysicsState : StateBase
{
    public DicePhysicsState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
}
