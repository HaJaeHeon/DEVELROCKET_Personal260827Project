using UnityEngine;
using UnityHFSM;

public class DIceRollState : StateBase
{
    public DIceRollState(bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
    }

    // 이 상태에 '진입'할 때 딱 한 번 실행됨
    public override void OnEnter()
    {
        Debug.Log("주사위 애니메이션 굴림 시작!");
        // 여기서 아까 만드셨던 주사위 DOTween 코루틴을 실행합니다.
    }

    // 이 상태가 돌아가는 '동안' 매 프레임 실행됨 (Update 역할)
    public override void OnLogic()
    {
        // 주사위가 굴러가는 도중에 할 일이 있다면 여기에 작성
    }

    // 이 상태를 '빠져나갈' 때 딱 한 번 실행됨
    public override void OnExit()
    {
        Debug.Log("주사위 굴림 끝! 이동할 준비 완료.");
    }
}
