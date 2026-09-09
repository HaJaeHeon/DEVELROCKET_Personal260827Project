using UnityEngine;
using UnityHFSM;

public class BuildTileState : StateBase
{
    private TileBuilds builds;
    public BuildTileState(TileBuilds builds, bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        this.builds = builds;
    }
    // 이 상태에 진입시 실행
    public override void OnEnter()
    {
        //Debug.Log("BuildTileState / OnEnter");
        builds.BuildProcess();
    }

    // Update 역할
    public override void OnLogic()
    {
        
    }

    // 이 상태를 빠져나갈 때 실행
    public override void OnExit()
    {
        //Debug.Log("BuildTileState / OnExit");
    }
}
