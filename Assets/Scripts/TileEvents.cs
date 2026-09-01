using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TileEvents : MonoBehaviour
{
    public enum Events
    {
        Start,
        Island,
        Festival,
        Travel
    }

    [field:SerializeField] public bool isProcess {  get; private set; }
    public void EventProcess()
    {
        isProcess = true;
        StartCoroutine(ProcessingEvent());
    }

    public IEnumerator ProcessingEvent()
    {
        Debug.Log($"isProcess : {isProcess}");
        yield return new WaitForSeconds(1f);

        yield return transform.DOComplete();

        isProcess = false;
        Debug.Log($"isProcess : {isProcess}");
    }
}
