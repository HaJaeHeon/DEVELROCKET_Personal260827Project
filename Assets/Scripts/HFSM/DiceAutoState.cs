using UnityEngine;
using UnityHFSM;

public class DiceAutoState : StateBase
{
    public DiceAutoState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
}
