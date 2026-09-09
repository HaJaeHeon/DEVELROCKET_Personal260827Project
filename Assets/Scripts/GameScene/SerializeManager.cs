using UnityEngine;

public class SerializeManager : MonoBehaviour
{
    [SerializeField] private GameObject mapObject;
    [SerializeField] private EarnEffectUI earnEffectUI;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private HFSMManager hfsmManager;




    private void Start()
    {
        InitGameManager();
        InitBoardManager();
    }

    private void InitGameManager()
    {
        GameManager.Instance.earnEffect = earnEffectUI;
        GameManager.Instance.hfsmManager = hfsmManager;
        GameManager.Instance.gameClearPanel = clearPanel;
        //GameManager.Instance.Init();
    }

    private void InitBoardManager()
    {
        BoardManager.Instance.boardObject = mapObject;
        BoardManager.Instance.playerMove = playerMove;
        BoardManager.Instance.InitNode();
    }
}
