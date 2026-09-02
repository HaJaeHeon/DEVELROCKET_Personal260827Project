using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuilds : MonoBehaviour
{
    [field: SerializeField] public bool isProcess { get; private set; }

    public void BuildProcess()
    {
        isProcess = true;
        StartCoroutine(ProcessingBuild());
    }

    public IEnumerator ProcessingBuild()
    {
        Debug.Log($"isProcess : {isProcess}");
        IncomeBuilding();
        yield return StartCoroutine(Build(GameManager.Instance.tile));

        isProcess = false;
        Debug.Log($"isProcess : {isProcess}");
    }

    public void IncomeBuilding()
    {
        TileNode tile = GameManager.Instance.tile;

        // 한 줄의 묶음을 9로 판단하여 나눌 때 9로 나눔
        // 현재 예외는 따로 생기지 않았으나 추후 문제 생길 수 있음
        CurrencyType type = (CurrencyType)(int)(tile.tileIndex / 9);

        Dictionary<CurrencyType, int> receipt = new Dictionary<CurrencyType, int>();
        receipt.Add(type, tile.reward);

        GameManager.Instance.UpdateAccount(receipt);
    }

    public IEnumerator Build(TileNode tile)
    {
        if (tile.buildingCount < GameManager.Instance.maxBuildingCount)
        {
            yield return StartCoroutine(tile.BuildBuiling());
        }
        else
        {
            Debug.LogWarning($"{tile.buildingCount} is same or over the {GameManager.Instance.maxBuildingCount}");
        }
    }

}
