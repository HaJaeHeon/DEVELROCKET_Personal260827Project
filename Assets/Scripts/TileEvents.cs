using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TileEvents : MonoBehaviour
{

    [field:SerializeField] public bool isProcess {  get; private set; }

    public void SwitchEvents(TileNode tile)
    {
        switch(tile.tileType)
        {
            case TileType.FoodLine:
            case TileType.WoodLine:
            case TileType.StoneLine:
            case TileType.IndustryLine:
                IncomeBuild();
                break;

            case TileType.StartZone:
            case TileType.Island:
            case TileType.Festival:
            case TileType.Travel:
            case TileType.Tax:
                Debug.Log("아직 안만듦");
                break;
        }
    }

    public void IncomeBuild()
    {
        TileNode tile = GameManager.Instance.tile;

        // 한 줄의 묶음을 9로 판단하여 나눌 때 9로 나눔
        // 현재 예외는 따로 생기지 않았으나 추후 문제 생길 수 있음
        CurrencyType type = (CurrencyType) (int)(tile.tileIndex / 9);
        
        Dictionary<CurrencyType, int> receipt = new Dictionary<CurrencyType, int>();
        receipt.Add(type, tile.reward);

        GameManager.Instance.UpdateAccount(receipt);
    }


    public void EventProcess()
    {
        isProcess = true;
        StartCoroutine(ProcessingEvent());
    }

    public IEnumerator ProcessingEvent()
    {
        Sequence seq = DOTween.Sequence();

        Debug.Log($"isProcess : {isProcess}");
        //yield return new WaitForSeconds(1f);

        seq.AppendCallback(() =>
        {
            SwitchEvents(GameManager.Instance.tile);
        }).WaitForCompletion();

        yield return transform.DOComplete();

        isProcess = false;
        Debug.Log($"isProcess : {isProcess}");
    }
}
