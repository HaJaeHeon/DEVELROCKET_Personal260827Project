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
        BoardManager.Instance.boardObject = mapObject;
        GameManager.Instance.earnEffect = earnEffectUI;
        BoardManager.Instance.playerMove = playerMove;
        GameManager.Instance.hfsmManager = hfsmManager;
        BoardManager.Instance.InitNode();
    }
}
