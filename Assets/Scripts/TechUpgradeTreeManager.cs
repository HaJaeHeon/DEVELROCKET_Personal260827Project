using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UILineRenderer 사용을 권장
using DG.Tweening;

public class TechUpgradeTreeManager : MonoBehaviour
{
    public static TechUpgradeTreeManager Instance;
    //public SkillNodeUI nodeUI; 

    [Header("처음부터 열려있을 시작 노드 데이터들")]
    public List<UpgradeBranchSO> startingNodes;
    [Header("UILineRenderer 선 긋기 세팅")]
    public float drawDuration = 0.5f;   // 선이 차오르는 시간

    // 씬에 배치된 모든 노드 UI를 담아둘 딕셔너리 (SO를 키값으로 사용)
    private Dictionary<UpgradeBranchSO, TechUpgradeUI> nodeDictionary = new Dictionary<UpgradeBranchSO, TechUpgradeUI>();

    void Awake()
    {
        Instance = this;

        // Content 아래에 있는 모든 SkillNodeUI를 찾아서 딕셔너리에 등록
        //SkillNodeUI[] allNodes = GetComponentsInChildren<SkillNodeUI>();
        //foreach (var nodeUI in allNodes)
        //{
        //    nodeDictionary.Add(nodeUI.nodeData, nodeUI);
        //}
        //Debug.Log($"{gameObject.name}'s Dictionary Count : {nodeDictionary.Count}");
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
        // 딕셔너리에서 찾을 필요 없이, 방금 마스터한 노드가 자식들을 다 알고 있습니다!
        foreach (TechUpgradeUI nextNodeUI in masteredNode.nextNodes)
        {
            // 선 긋기 연출
            if (masteredNode.branchImage != null)
            {
                // 추후 branch가 여러개일때 생각하기
                DOVirtual.Float(0f, 1f, drawDuration, (t) =>
                {
                    masteredNode.branchImage.fillAmount = t;
                }).OnComplete(() => UnlockNode(nextNodeUI));
            }
            else
            {
                Debug.Log("branch Image is null");
            }
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
}