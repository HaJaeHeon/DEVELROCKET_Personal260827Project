using UnityEngine;
using UnityHFSM;

public class MoveState : StateBase
{
    private PlayerMove move;
    public MoveState(PlayerMove move, bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        this.move = move;
    }
    // 이 상태에 진입시 실행
    public override void OnEnter()
    {
        Debug.Log("MoveState / OnEnter");
        move.StartMove(GameManager.Instance.diceNum);
    }

    // Update 역할
    public override void OnLogic()
    {
        
    }

    // 이 상태를 빠져나갈 때 실행
    public override void OnExit()
    {
        Debug.Log("MoveState / OnExit");
    }
}
