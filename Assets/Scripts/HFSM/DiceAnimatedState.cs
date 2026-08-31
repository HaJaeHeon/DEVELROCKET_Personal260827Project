using UnityEngine;
using UnityHFSM;

public class DiceAnimatedState : StateBase
{
    public DiceAnimatedState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
}
