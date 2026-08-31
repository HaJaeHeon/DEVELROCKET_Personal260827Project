using UnityEngine;
using UnityHFSM;

public class HFSMManager : MonoBehaviour
{
    public enum DiceMode
    {
        Physics,
        Animated,
        Auto
    };

    public DiceMode currentMode;
    [SerializeField] private DIceRoll roll;
    
    private StateMachine fsm;

    private void Start()
    {
        fsm = new StateMachine();

        fsm.AddState("Idle", new IdleState());
        fsm.AddState("Move", new MoveState());

        // 주사위 관련 FSM 등록
        StateMachine diceFsm = new StateMachine();
        diceFsm.AddState("DicePhysics", new DicePhysicsState(roll));
        diceFsm.AddState("DiceAnimated", new DiceAnimatedState());
        diceFsm.AddState("DiceAuto", new DiceAutoState());
        
        diceFsm.AddState("Entry", new StateBase(false));
        diceFsm.SetStartState("Entry");
        
        fsm.AddState("diceFsm", diceFsm);
        
        // 주사위 관련 Transition
        diceFsm.AddTransition("Entry", "DicePhysics", t => currentMode == DiceMode.Physics);
        diceFsm.AddTransition("Entry", "DiceAnimated", t => currentMode == DiceMode.Animated);
        diceFsm.AddTransition("Entry", "DiceAuto", t => currentMode == DiceMode.Auto);

        // arrivedTile 관련 FSM 등록
        StateMachine arrivedFsm = new StateMachine();
        arrivedFsm.AddState("EventTile", new EventTileState());
        arrivedFsm.AddState("BuildTile", new BuildTileState());
        
        arrivedFsm.AddState("Entry", new StateBase(false));
        arrivedFsm.SetStartState("Entry");
        
        fsm.AddState("arrivedFsm", arrivedFsm);
        
        // arrivedTile 관련 Transition
        //arrivedFsm.AddTransition("Entry", "EventTile",  => );
        //arrivedFsm.AddTransition("Entry", "BuildTile",  => );
        
        //fsm.AddTransition("Idle", "DiceRoll", t => /* 상태 전환 조건 */);
        //fsm.AddTransition("DiceRoll", "Move", t => /* 상태 전환 조건 */);
        //fsm.AddTransition("Move", "ArrivedTile", t => /* 상태 전환 조건 */);
        //fsm.AddTransition("ArrivedTile", "Idle", t => /* 상태 전환 조건 */);
        
        fsm.AddTriggerTransition("Button_DiceRoll", "Idle", "diceFsm");

        fsm.SetStartState("Idle");
        fsm.Init();
    }

    private void Update()
    {
        fsm.OnLogic();
    }

    public void OnClickDiceRoll()
    {
        fsm.Trigger("Button_DiceRoll");
    }
}
