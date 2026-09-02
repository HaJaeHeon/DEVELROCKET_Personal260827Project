using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TileEvents : MonoBehaviour
{
    [field:SerializeField] public bool isProcess {  get; private set; }
    

    // 이벤트칸 도착 시 이벤트 판단하여 발생
    // 추가 작업 필요
    public IEnumerator ExcuteEvents(TileNode tile)
    {
        yield return null;
    }
    public void EventProcess()
    {
        isProcess = true;
        StartCoroutine(ProcessingEvent());
    }

    public IEnumerator ProcessingEvent()
    {
        Debug.Log($"isProcess : {isProcess}");

        yield return StartCoroutine(ExcuteEvents(GameManager.Instance.tile));

        isProcess = false;
        Debug.Log($"isProcess : {isProcess}");
    }
}
