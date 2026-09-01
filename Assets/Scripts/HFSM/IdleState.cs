using UnityEngine;
using UnityHFSM;

public class IdleState : StateBase
{
    public IdleState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }

    // 이 상태에 진입시 실행
    public override void OnEnter()
    {
        Debug.Log("IdleState / OnEnter");
    }

    // Update 역할
    public override void OnLogic()
    {
        
    }

    // 이 상태를 빠져나갈 때 실행
    public override void OnExit()
    {
        Debug.Log("IdleState / OnEnter");
    }
}
