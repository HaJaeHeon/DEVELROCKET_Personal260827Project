using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UILineRenderer 사용을 권장
using DG.Tweening;
using UnityEngine.UI.Extensions;

public class TechUpgradeTreeManager : MonoBehaviour
{
    public static TechUpgradeTreeManager Instance;
    //public SkillNodeUI nodeUI; 

    [Header("처음부터 열려있을 시작 노드 데이터들")]
    public List<UpgradeBranchSO> startingNodes;
    [Header("UILineRenderer 선 긋기 세팅")]
    public float lineWidth = 5f;        // 선의 두께
    public float drawDuration = 1.0f;   // 선이 차오르는 시간
    public Color lineColor = Color.yellow; // 선 색상

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
            // 다음 노드 활성화
            nextNodeUI.UnlockNode();
            nextNodeUI.nodeButton.interactable = true;

            // 선 긋기 연출
            DrawLineExtension(masteredNode.GetComponent<RectTransform>(), nextNodeUI.GetComponent<RectTransform>());
        }
    }


    private void DrawLine(RectTransform startPos, RectTransform endPos)
    {
        // 유니티 UI Extensions의 UILineRenderer를 사용하거나, 
        // 얇고 긴 Image(RectTransform)를 생성하여 start와 end 사이의 각도와 길이를 계산해 배치합니다.
        // 여기에 DOVirtual.Float을 쓰면 선이 차오르는 애니메이션을 만들 수 있습니다.
    }

    // 노드가 해금되었을 때 이 함수를 호출해주세요!
    private void DrawLineExtension(RectTransform startPos, RectTransform endPos)
    {
        // 1. 선 역할을 할 빈 UI 오브젝트 생성
        GameObject lineObj = new GameObject($"Line_{startPos.name}_to_{endPos.name}");
        lineObj.transform.SetParent(this.transform, false);

        // UI 노드들보다 뒤쪽에 선이 깔리도록 순서를 맨 위로 올림
        lineObj.transform.SetAsFirstSibling();

        // 2. UILineRenderer 컴포넌트 추가 및 세팅
        UILineRenderer uiLine = lineObj.AddComponent<UILineRenderer>();
        uiLine.color = lineColor;
        uiLine.LineThickness = lineWidth; // 이 패키지에서는 두께를 이걸로 조절합니다.

        // (참고) 만약 선이 안 보이거나 보라색으로 깨지면 아래 주석을 풀어주세요.
        // uiLine.material = Canvas.GetDefaultCanvasMaterial();

        // 3. 좌표 변환 (매우 중요 ⭐)
        // 두 노드의 월드 좌표를, 방금 만든 선 오브젝트 기준의 로컬 좌표로 변환합니다.
        Vector2 startPoint = lineObj.transform.InverseTransformPoint(startPos.position);
        Vector2 endPoint = lineObj.transform.InverseTransformPoint(endPos.position);

        // 4. 선의 꼭짓점(Points) 초기 세팅
        uiLine.Points = new Vector2[2];
        uiLine.Points[0] = startPoint; // 출발점
        uiLine.Points[1] = startPoint; // 도착점 (시작할 때는 길이가 0이어야 하므로 출발점과 똑같이 둡니다)

        // 5. DOTween으로 선이 뻗어나가는 연출
        // 0부터 1까지(0% ~ 100%) drawDuration 동안 숫자를 올립니다.
        DOVirtual.Float(0f, 1f, drawDuration, (t) =>
        {
            // 현재 퍼센트(t)에 맞춰서 출발점과 도착점 사이의 현재 위치를 계산합니다.
            Vector2 currentPoint = Vector2.Lerp(startPoint, endPoint, t);

            // 끝점(인덱스 1)의 위치를 갱신합니다.
            uiLine.Points[1] = currentPoint;

            //  핵심: UILineRenderer는 배열 값이 바뀌면 이 함수를 꼭 불러줘야 화면에 다시 그립니다!
            uiLine.SetVerticesDirty();

        }).SetEase(Ease.OutQuad); // 부드럽게 감속하는 애니메이션 곡선
    }
}