using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class DIceRoll : MonoBehaviour
{
   private Rigidbody rb;
   [SerializeField] private bool isRolling;

   [SerializeField] private float rollPower;
   [SerializeField] private float torquePower;
   [SerializeField] private Transform initDicePosition;

   // 
   /// <summary>
   /// 주사위 모델 위치에 맞게 조절
   /// 위, 아래, 오른쪽 , 왼쪽 , 앞, 뒤 순서
   /// </summary>
   public int[] diceFace = { 1, 2, 3, 4, 5, 6 };

   private void Awake()
   {
      rb = GetComponent<Rigidbody>();
   }

   /// <summary>
   /// 위 방향으로 쏘아 올리는거는 Force로 주사위가 올라갔다가 내려오는 느낌 나게 만들고
   /// 회전은 Vector3.one을 해 나름 잘 섞이게끔 만듦
   /// </summary>
   [ContextMenu("주사위 굴리기")]
   public void Roll()
   {
      if (isRolling)
         return;
      
      isRolling = true;
      
      //주사위 초기화
      rb.linearVelocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      transform.position = initDicePosition.position;
      
      Vector3 randomSpin = new Vector3(
         Random.Range(-1f, 1f),
         Random.Range(-1f, 1f),
         Random.Range(-1f, 1f)
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

      int finalNumber = CalcDiceFace();
      Debug.Log($"주사위 결과값 : {finalNumber}");
      
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
}
