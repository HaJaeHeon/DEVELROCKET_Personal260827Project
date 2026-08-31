using UnityEngine;
using UnityHFSM;

public class ArrivedTileState : StateBase
{
    public ArrivedTileState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
}
