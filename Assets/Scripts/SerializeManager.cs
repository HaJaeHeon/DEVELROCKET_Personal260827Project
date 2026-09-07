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
        if (BoardManager.Instance.boardObject == null)
        {
            BoardManager.Instance.boardObject = mapObject;
        }
        if (GameManager.Instance.earnEffect == null)
        { 
            GameManager.Instance.earnEffect = earnEffectUI;
        }
        if (BoardManager.Instance.playerMove == null)
        { 
            BoardManager.Instance.playerMove = playerMove;
        }
        if(GameManager.Instance.hfsmManager == null)
        {
            GameManager.Instance.hfsmManager = hfsmManager;
        }
    }
}
