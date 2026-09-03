using System.Collections.Generic;
using UnityEngine;

public class TechUpgradeTreeManager : MonoBehaviour
{
    private static TechUpgradeTreeManager instance;
    public static TechUpgradeTreeManager Instance => instance;

    [Header("처음부터 열려있을 시작 노드 데이터들")]
    [SerializeField] private List<UpgradeBranchSO> startingNodes;
    [Header("선 긋기 세팅")]
    [SerializeField] private Transform lineFolder;
    [SerializeField] private GameObject linePrefab;

    // 씬에 배치된 모든 노드 UI를 담아둘 딕셔너리 (SO를 키값으로 사용)
    private Dictionary<UpgradeBranchSO, TechUpgradeUI> nodeDictionary = new Dictionary<UpgradeBranchSO, TechUpgradeUI>();

    // 싱글턴 destroy 를 this로 하면 컴포넌트가 삭제되는거지 gameobject가 사라지는거 아니라서 주의 필요
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
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
}