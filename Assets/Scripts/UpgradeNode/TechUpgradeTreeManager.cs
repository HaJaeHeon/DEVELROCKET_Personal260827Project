using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UILineRenderer 사용을 권장
using DG.Tweening;

public class TechUpgradeTreeManager : MonoBehaviour
{
    private static TechUpgradeTreeManager instance;
    public static TechUpgradeTreeManager Instance => instance;

    [Header("처음부터 열려있을 시작 노드 데이터들")]
    [SerializeField] private List<UpgradeBranchSO> startingNodes;
    [Header("선 긋기 세팅")]
    [SerializeField] private float drawDuration = 0.5f;   // 선이 차오르는 시간
    [SerializeField] private Transform lineFolder;
    [SerializeField] private GameObject linePrefab;

    // 씬에 배치된 모든 노드 UI를 담아둘 딕셔너리 (SO를 키값으로 사용)
    private Dictionary<UpgradeBranchSO, TechUpgradeUI> nodeDictionary = new Dictionary<UpgradeBranchSO, TechUpgradeUI>();

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 씬에 있는 모든 노드를 찾아서, 루트 노드인지 확인 후 켜고 끕니다.
        TechUpgradeUI[] allNodes = GetComponentsInChildren<TechUpgradeUI>();

        foreach (TechUpgradeUI nodeUI in allNodes)
        {
            if (nodeUI.isRootNode)
            {
                nodeUI.SetupNode(true);
            }
            else
            {
                nodeUI.SetupNode(false);
            }
        }
    }

    public void OnNodeMastered(TechUpgradeUI masteredNode)
    {
        // 선 긋기 연출
        //if (masteredNode.branchImage != null)
        //{
        //    // branch 여러개여도 동시에 가능하도록 foreach 돌림
        //    DOVirtual.Float(0f, 1f, drawDuration, (t) =>
        //    {
        //        masteredNode.branchImage.ForEach((image)=> image.fillAmount = t);
        //    }).OnComplete(() => UnlockNode(nextNodeUI));
        //}
        for (int i = 0; i < masteredNode.nextNodes.Count; i++)
        {
            GameObject line = GameObject.Instantiate(linePrefab);
            line.transform.SetParent(lineFolder);
            LineUI ui = line.GetComponent<LineUI>();
            ui.nodeA = masteredNode.gameObject.GetComponent<RectTransform>();
            ui.nodeB = masteredNode.nextNodes[i].gameObject.GetComponent<RectTransform>();
            ui.DrawLine();

            UnlockNode(masteredNode.nextNodes[i]);
        }
    }

    // 다음 노드 활성화
    private void UnlockNode(TechUpgradeUI ui)
    {
        ui.UnlockNode();
        ui.nodeButton.interactable = true;
    }

    // 노드가 해금되었을 때 이 함수를 호출
    private void DrawLineExtension(Image branchImage)
    {
        //branchImage.DOFillAmount(1f, drawDuration).SetEase(Ease.OutQuad);

        DOVirtual.Float(0f, 1f, drawDuration, (t) =>
        {
            branchImage.fillAmount = t;
        }).SetEase(Ease.OutQuad);
    }

    private void DrawLine()
    {

    }
}