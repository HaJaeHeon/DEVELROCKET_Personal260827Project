using System.Collections;
using UnityEngine;
using DG.Tweening;
public class PlayerMove : MonoBehaviour
{
    [Header("이동 관련")]
    //[SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;
    //점프 시간을 줄여서 animation속도 짧게하기
    [SerializeField] private float jumpDuration;
    //말 이동 대기 시간 짧게
    [SerializeField] private float waitForNextNode;
    //말 위치를 타일 면과 닿게
    [SerializeField] private float heightOffset;

    [Space]
    [SerializeField] private int currentNodeIndex;
    [field:SerializeField] public bool isRunning {  get; private set; }

    // gameManager나 여기서 init 으로 플레어이 위치 처음 타일로 초기화 필요
    private void Start()
    {
        Vector3 initPosition = BoardManager.Instance.GetTile(0).transform.position;
        transform.position = initPosition + Vector3.up * heightOffset;
    }

    public void StartMove(int diceNum)
    {
        if (isRunning)
            return;

        StartCoroutine(PieceMove(diceNum));
    }

    [ContextMenu("플레어이 말 이동")]
    public IEnumerator PieceMove(int diceNum)
    {
        isRunning = true;
        for (int i = 0; i < diceNum; i++)
        {
            currentNodeIndex++;

            TileNode targetNode = BoardManager.Instance.GetTile(currentNodeIndex);
            Vector3 endPosition = targetNode.transform.position + Vector3.up * heightOffset;
            //Debug.Log(endPosition);

            yield return transform.DOJump(endPosition, jumpHeight, 1, jumpDuration).WaitForCompletion();

            if(waitForNextNode > 0)
            {
                yield return new WaitForSeconds(waitForNextNode);
            }
        }

        TileNode finalTile = BoardManager.Instance.GetTile(currentNodeIndex);
        GameManager.Instance.tile = finalTile;
        //Debug.Log($"마지막 타일 {finalTile.tileIndex}");
        finalTile.SetReward();
        isRunning = false;
    }
}
