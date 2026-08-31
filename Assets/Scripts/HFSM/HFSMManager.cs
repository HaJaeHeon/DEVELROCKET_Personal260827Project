using UnityEngine;
using UnityHFSM;

public class HFSMManager : MonoBehaviour
{
    private StateMachine fsm;

    private void Start()
    {
        fsm = new StateMachine();

        fsm.AddState("Idle", new IdleState());
        fsm.AddState("Move", new MoveState());

        StateMachine diceFsm = new StateMachine();
        diceFsm.AddState("DicePhysics", new DicePhysicsState());
        diceFsm.AddState("DiceAnimated", new DiceAnimatedState());
        diceFsm.AddState("DiceAuto", new DiceAutoState());
        fsm.AddState("DiceRoll", new DIceRollState());

        StateMachine arrivedFsm = new StateMachine();
        arrivedFsm.AddState("EventTile", new EventTileState());
        arrivedFsm.AddState("BuildTile", new BuildTileState());
        fsm.AddState("ArrivedTile", new ArrivedTileState());

        //fsm.AddTransition("Idle", "DiceRoll", t => /* 상태 전환 조건 */);
        //fsm.AddTransition("DiceRoll", "Move", t => /* 상태 전환 조건 */);
        //fsm.AddTransition("Move", "ArrivedTile", t => /* 상태 전환 조건 */);
        //fsm.AddTransition("ArrivedTile", "Idle", t => /* 상태 전환 조건 */);

        fsm.SetStartState("Idle");
        fsm.Init();
    }

    private void Update()
    {
        fsm.OnLogic();
    }
}
