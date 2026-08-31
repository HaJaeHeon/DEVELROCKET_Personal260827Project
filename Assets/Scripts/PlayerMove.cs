using System.Collections;
using UnityEngine;
using DG.Tweening;
public class PlayerMove : MonoBehaviour
{
    [Header("이동 관련")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float waitForNextNode;
    [SerializeField] private float heightOffset;

    [Space]
    [SerializeField] private int currentNodeIndex;
    [SerializeField] private bool isRunning;

    // gameManager나 여기서 init 으로 플레어이 위치 처음 타일로 초기화 필요

    public void MoveInInspector(int diceNum)
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
            Debug.Log(endPosition);

            yield return transform.DOJump(endPosition, jumpHeight, 1, jumpDuration).WaitForCompletion();

            if(waitForNextNode > 0)
            {
                yield return new WaitForSeconds(waitForNextNode);
            }
        }

        TileNode finalTile = BoardManager.Instance.GetTile(currentNodeIndex);
        Debug.Log($"마지막 타일 {finalTile.tileIndex}");
        isRunning = false;
    }
}
