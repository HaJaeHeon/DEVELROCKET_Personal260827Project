using UnityEngine;
using UnityHFSM;

public class BuildTileState : StateBase
{
    public BuildTileState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }
    // 이 상태에 진입시 실행
    public override void OnEnter()
    {
        Debug.Log("BuildTileState / OnEnter");
    }

    // Update 역할
    public override void OnLogic()
    {
        
    }

    // 이 상태를 빠져나갈 때 실행
    public override void OnExit()
    {
        Debug.Log("BuildTileState / OnExit");
    }
}
