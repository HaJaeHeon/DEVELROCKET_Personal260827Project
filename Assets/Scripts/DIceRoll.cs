using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum RollType
{
    Classic,
    Animate,
    Auto
}

[RequireComponent(typeof(Rigidbody))]
public class DiceRoll : MonoBehaviour
{
    private Rigidbody rb;
    [field:SerializeField] public bool isRolling { get; private set;  }

    [SerializeField] private float rollPower;
    [SerializeField] private float torquePower;
    [SerializeField] private Transform initDiceTransform;
    [field:SerializeField] public bool isAutoRoll {  get; private set; }
    [field: SerializeField] public int finalDiceNum;

    // GameManager 에서 관리
    private float rollDuration;
    
    private Coroutine currentRoutine;

    /// <summary>
    /// 주사위 모델 위치에 맞게 조절
    /// 위, 아래, 오른쪽 , 왼쪽 , 앞, 뒤 순서
    /// </summary>
    public int[] diceFace = { 1, 2, 3, 4, 5, 6 };

    private Coroutine autoRollRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isRolling = false;
        isAutoRoll = false;

        rollDuration = GameManager.Instance.rollDuration;
    }


    //============Classic Roll====================================================================

    /// <summary>
    /// 위 방향으로 쏘아 올리는거는 Force로 주사위가 올라갔다가 내려오는 느낌 나게 만들고
    /// 회전은 Vector3.one을 해 나름 잘 섞이게끔 만듦
    /// </summary>
    [ContextMenu("주사위 굴리기")]
    public void ClassicRoll()
    {
        if (isRolling)
            return;

        rb.useGravity = true;
        isRolling = true;

        //주사위 초기화
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initDiceTransform.position;

        Vector3 randomSpin = new Vector3(
           Random.Range(-3f, 3f),
           Random.Range(-3f, 3f),
           Random.Range(-3f, 3f)
           );

        rb.AddForce(Vector3.forward * rollPower, ForceMode.Impulse);
        rb.AddTorque(randomSpin * torquePower, ForceMode.Impulse);

        StartCoroutine(DiceResult());
    }

    private IEnumerator DiceResult()
    {
        // 주사위가 돌자미자 멈췄다고 판단하는것을 방지
        yield return new WaitForSeconds(0.5f);

        // 주사위 속도와 회전 속도가 0에 근접할때까지 대기
        yield return new WaitUntil(() => rb.linearVelocity.sqrMagnitude < 0.1f && rb.angularVelocity.sqrMagnitude < 0.1f);

        finalDiceNum = CalcDiceFace();
        Debug.Log($"주사위 결과값 : {finalDiceNum}");

        // GameManager에 보내기
        GameManager.Instance.diceNum = finalDiceNum;

        isRolling = false;
    }

    private int CalcDiceFace()
    {
        Vector3[] directions = new Vector3[]
        {
         transform.up,
         -transform.up,
         transform.right,
         -transform.right,
         transform.forward,
         -transform.forward
        };

        float maxDotProduct = float.MinValue;
        int diceNumber = int.MinValue;

        for (int i = 0; i < directions.Length; i++)
        {
            float dotProduct = Vector3.Dot(directions[i], Vector3.up);

            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                diceNumber = diceFace[i];
            }
        }
        return diceNumber;
    }

    //=====================Animate Roll================================================

    [ContextMenu("Animate Roll")]
    public void AnimateRoll()
    {
        if (isRolling)
            return;

        if (currentRoutine == null)
        {
            currentRoutine = StartCoroutine(RollCoroutine());
        }
    }

    private IEnumerator RollCoroutine()
    {
        if (isRolling)
            yield break;

        isRolling = true;
        rb.useGravity = false;
        rb.isKinematic = true;
        transform.DOKill();

        transform.position = initDiceTransform.position;

        finalDiceNum = Random.Range(1, 7);
        //Debug.Log($"current randomResult = {finalDiceNum}");

        Vector3 spinVector = Vector3.one * 1080f;

        yield return transform.DORotate(spinVector, rollDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(() =>
        {
            DiceFacing(finalDiceNum);
        }).WaitForCompletion();

        GameManager.Instance.diceNum = finalDiceNum;

        currentRoutine = null;
        autoRollRoutine = null;

        isRolling = false;
    }

    //=======================Auto Roll======================================================

    [ContextMenu("Auto Roll")]
    public void AutoRoll()
    {
        if (isRolling)
            return;

        if(autoRollRoutine == null)
        {
            autoRollRoutine = StartCoroutine(AutoRollCoroutine());
        }
    }

    private IEnumerator AutoRollCoroutine()
    {
        yield return StartCoroutine(RollCoroutine());
    }

    // 추후 오토가 필요 없을 때 사용하도록
    public void ChangeAuto()
    {
        isAutoRoll = !isAutoRoll;

        if (!isAutoRoll && autoRollRoutine != null)
        {
            StopCoroutine(autoRollRoutine);
            autoRollRoutine = null;
        }
    }

    //==================================== Facing =========================================
    public void DiceFacing(int faceNum)
    {
        Vector3[] directions = new Vector3[]
        {
         transform.up,
         -transform.up,
         transform.right,
         -transform.right,
         transform.forward,
         -transform.forward
        };

        for (int i = 0; i < directions.Length; i++)
        {
            if (diceFace[i] == faceNum)
            {
                transform.rotation = Quaternion.FromToRotation(directions[i], -Camera.main.transform.forward);

                //Debug.Log($"i = {i} / faceNum = {faceNum}");
                return;
            }
        }
    }
}
