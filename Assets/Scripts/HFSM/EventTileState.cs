using UnityEngine;
using UnityHFSM;

public class EventTileState : StateBase
{
    public EventTileState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
}
