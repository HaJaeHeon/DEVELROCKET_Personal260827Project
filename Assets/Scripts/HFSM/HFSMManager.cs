using UnityEngine;
using UnityHFSM;

public class HFSMManager : MonoBehaviour
{
    public DiceMode currentMode;
    public TileMode currentTileType;


    [SerializeField] private DIceRoll roll;
    [SerializeField] private PlayerMove move;
    [SerializeField] private TileEvents tileEvent;
    [SerializeField] private TileBuilds tileBuild;
    
    private StateMachine fsm;

    private void Start()
    {
        fsm = new StateMachine();


        // Idle 관련 FSM
        fsm.AddState("Idle", new IdleState());
        fsm.AddTransition("Idle", "diceFsm", t => roll.isAutoRoll == true);

        // Move 관련 FSM
        fsm.AddState("Move", new MoveState(move));

        fsm.AddTransition("Move", "arrivedFsm", t => move.isRunning == false);

        // 주사위 관련 FSM 등록
        StateMachine diceFsm = new StateMachine();
        diceFsm.AddState("DicePhysics", new DicePhysicsState(roll));
        diceFsm.AddState("DiceAnimated", new DiceAnimatedState(roll));
        diceFsm.AddState("DiceAuto", new DiceAutoState(roll));
        
        diceFsm.AddState("Entry", new StateBase(false, true));
        diceFsm.SetStartState("Entry");
        
        fsm.AddState("diceFsm", diceFsm);

        // 주사위 관련 Transition
        
        diceFsm.AddTransition("Entry", "DicePhysics", t => currentMode == DiceMode.Physics);
        diceFsm.AddTransition("Entry", "DiceAnimated", t => currentMode == DiceMode.Animated);
        diceFsm.AddTransition("Entry", "DiceAuto", t => currentMode == DiceMode.Auto);

        fsm.AddTransition("diceFsm", "Move", t => roll.isRolling == false);



        // arrivedTile 관련 FSM 등록
        StateMachine arrivedFsm = new StateMachine();
        arrivedFsm.AddState("EventTile", new EventTileState(tileEvent));
        arrivedFsm.AddState("BuildTile", new BuildTileState(tileBuild));
        
        arrivedFsm.AddState("Entry", new StateBase(false, true));
        arrivedFsm.SetStartState("Entry");
        
        fsm.AddState("arrivedFsm", arrivedFsm);

        arrivedFsm.AddTransition("Entry", "EventTile", t => GameManager.Instance.CalcTileType() == TileMode.Event);
        arrivedFsm.AddTransition("Entry", "BuildTile", t => GameManager.Instance.CalcTileType() == TileMode.Build);
        
        fsm.AddTransition("arrivedFsm", "Idle", t => tileEvent.isProcess == false && tileBuild.isProcess == false);
        
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
