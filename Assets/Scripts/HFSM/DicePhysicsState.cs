using UnityEngine;
using UnityHFSM;

public class DicePhysicsState : StateBase
{
    private DIceRoll roll;
    public DicePhysicsState(DIceRoll roll, bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        this.roll = roll;
    }
    // 이 상태에 진입시 실행
    public override void OnEnter()
    {
        roll.ClassicRoll();
    }

    // Update 역할
    public override void OnLogic()
    {
        
    }

    // 이 상태를 빠져나갈 때 실행
    public override void OnExit()
    {
        
    }
}
