# DEVELROCKET_Personal260827Project

- 노션 페이지 : https://app.notion.com/p/3c9e11440b95802581e6f60afb231db7?source=copy_link
- 깃허브 주소 : https://github.com/HaJaeHeon/DEVELROCKET_Personal260827Project.git
# I_Marble 프로젝트 명세서

## 1. 프로젝트 요약

| 항목 | 내용 |
| :--- | :--- |
| **이름** | 하재헌 |
| **프로젝트명 / 게임명** | I_Marble |
| **게임 한 줄 정의** | 주사위를 굴려 보드에 정해진 자원들을 수집해 스킬노드를 찍어 플레이 감각 개선 및 자동화 하여 엔딩 스킬노드를 찍어 게임을 클리어 합니다. |
| **적용한 심화 기술** | Singleton, SO, HFSM, Json, CSV, BigInteger |
| **GitHub 저장소 링크** | [GitHub 바로가기](https://github.com/HaJaeHeon/DEVELROCKET_Personal260827Project.git) |
| **조작 방법** | 마우스로 조작하여 플레이 |
| **실행 환경** | Windows 10/11, 1920 * 1080 권장, 300MB 이상의 여유 공간 필요, 로컬 싱글 플레이 |

---

## 2. 프로젝트 개요
* **게임명** : I_Marble
* **장르** : 모노폴리 류 아이들러 게임
* **한 줄 정의** : 주사위를 굴려 보드에 정해진 자원들을 수집하고, 스킬 노드를 개방하여 플레이 감각을 개선 및 자동화하며 최종적으로 엔딩 스킬 노드를 찍어 클리어하는 게임입니다.

---

## 3. 실행 방법
* **빌드 실행** : 빌드된 `.exe` 파일을 실행하여 플레이
* **Unity 버전** : 6000.3.61f
* **조작 키** : 마우스 전용 조작

---

## 4. 핵심 플레이 흐름

### 씬(Scene) 흐름
> 타이틀 씬 ➔ 로딩 씬 ➔ 게임 플레이 씬 ➔ 로딩 씬 ➔ 타이틀 씬 (순환 구조)

### 플레이 사이클
```text
-1 대기 상태
-2 Dice 로직 실행
   ├─ 물리 기반 주사위 Roll
   ├─ Animation 기반 주사위 Roll
   └─ 자동화 주사위 Roll
-3 플레이어 이동
-4 타일 도착
   ├─ 이벤트 타일
   │  ├─ 시작 타일
   │  ├─ 무인도 타일
   │  ├─ 축제 타일
   │  └─ 세계여행 타일
   └─ 건설 타일
      ├─ 식량 타일
      ├─ 나무 타일
      ├─ 석재 타일
      └─ 산업 타일
-5 다시 대기 상태로 복귀
```

## 5. 주요 기능 목록
GameManager : 전반적인 게임 데이터 관리
TechUpgradeTreeManager : 스킬 노드 시스템 관리
UpgradeBranchSO : 스킬 노드의 ScriptableObject 데이터
TechUpgradeUI : 스킬 노드 관련 UI 중심 로직
BoardManager : 보드 타일 생성 및 관리
TileDataSO : 각 타일의 ScriptableObject 데이터
TileNode : 개별 타일의 중심 로직
HFSMManager : 계층형 유한 상태 기계(HFSM) 관리
DiceRoll : 주사위 굴림 및 결과 판정 중심 로직
DataManager : Save Data 및 InitData 생성 관리

## 6. 프로젝트 구조
```text
Assets/
├── Datas/       # ScriptableObject(SO) 데이터 파일 목록
├── Import/      # Import한 외부 에셋 및 라이브러리
├── Prefabs/     # Prefab으로 사용할 Object 목록
├── Resources/   # Damage Number 에셋 Prefab, DOTweenSettings 등
├── Scenes/      # 게임 씬 목록
└── Scripts/     # 프로젝트 주요 스크립트 목록
```

## 7. 핵심 로직 설명
주사위가 회전 후 멈추었을 때, 위를 향하는 면의 값을 계산하는 로직입니다.
```C#
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
```
코드 설명 : 주사위 오브젝트의 각 6개 면 방향 벡터와 월드의 윗 방향(Vector3.up)을 내적(Dot Product)합니다. 내적 값이 가장 큰 면이 현재 하늘을 바라보는 면이 되며, 인스펙터에서 맵핑해둔 diceFace 배열을 참조하여 최종 주사위 결과값을 반환합니다.

## 8. 적용한 심화 기술과 선택 이유
- HFSM (Hierarchical Finite State Machine)
  기획 단계에서 게임 로직이 많은 상태를 가지고 있고, 순환하는 그래프 구조를 띄고 있었습니다. FSM이나 BT(Behavior Tree)보다 상태의 계층화 및 관리가 용이한 HFSM이 구조적으로 가장 적합하다고 판단하여 도입했습니다.

- BigInteger
  아이들러(방치형) 게임의 장르적 특성상 후반부에 재화량이 일반적인 자료형의 한계를 넘어 폭발적으로 증가하는 구간이 발생합니다. 이를 처리하기 위해 C# 내부 라이브러리인 BigInteger를 채택했습니다.

- Json
  플레이어 데이터 저장 방식을 고민할 때, 데이터 내에 클래스와 리스트가 중첩되는 계층 구조가 존재했습니다. 평면적인 데이터 처리에 유리한 CSV보다는 복잡한 계층 구조를 직렬화하기 좋은 Json 포맷이 더 유리하다는 결론을 내렸습니다.

## 9. 성능 개선 내역
문제 : 아이들러 게임 특성상 짧은 시간 내에 재화 획득과 관련된 UI 요소가 화면에 다수 생성 및 파괴되는 현상 발생.

  해결 : 잦은 Instantiate/Destroy로 인한 가비지 컬렉션(GC) 스파이크를 방지하기 위해, 외부 에셋인 'Damage Number' 내에 포함된 오브젝트 풀링(Pool) 기능을 적극 활용하여 성능 병목을 해소했습니다.

## 10. 개발 중 문제와 해결 과정
- 1 - 스킬 노드 UI 상태별 색상 변경 이슈
문제 : 스킬 노드의 상태(MAX, 활성화, 재화 부족, 비활성화)에 따라 외곽선(Outline)의 색상으로 구분감을 주려 했습니다. 하지만 Outline 컴포넌트의 색상을 변경할 때 Button 컴포넌트의 색상도 함께 변질되는 현상이 발생했습니다.

  해결 : Button 컴포넌트의 Transition 설정이 Color Tint로 설정되어 있어 Outline 색상 변경 시 간섭이 일어나는 것을 확인하고, Transition 방식을 수정하여 간섭 문제를 해결했습니다.

- 2 - 빈 객체를 활용한 타일 복제 시 좌표 중첩 문제
문제 : Create Empty로 최상위 객체를 만들고, 그 자식으로 타일 객체를 여러 개 복제하여 배치했을 때, 타일들의 Local Position과 World Position이 동일하게 출력되는 버그가 발생했습니다.

  해결 : 에디터 상에서 하드코딩으로 배치하는 대신, 타일들의 위치 값을 정리하여 CSV 파일로 저장해두고 게임 실행 시 해당 데이터를 로드하여 동적으로 맵 좌표를 구성하고 맵을 생성하도록 구조를 개편할 예정입니다.

## 11. 향후 개선 방향
초기 기획과 달라진 부분이 있지만, 이벤트 타일에 도착했을 때 단순히 정해진 이벤트가 출력되는 것을 넘어 추가적인 미니 게임 요소를 도입하고 싶습니다.
 예를 들어 룰렛, 자판기, 돌림판 등의 시각적이고 직관적인 미니게임을 통해 재화를 얻는 수단을 다양화하고, 추후 스킬 노드를 통해 이러한 미니게임 역시 자동화할 수 있도록 구현한다면 플레이어에게 더 큰 재미를 줄 수 있을 것이라 기대합니다.
