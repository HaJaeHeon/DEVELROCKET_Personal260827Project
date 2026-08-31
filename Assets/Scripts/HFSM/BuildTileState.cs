using UnityEngine;
using UnityHFSM;

public class BuildTileState : StateBase
{
    public BuildTileState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
}
