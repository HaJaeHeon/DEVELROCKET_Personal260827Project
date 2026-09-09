using System.Text;
using System.IO;
using UnityEngine;

public class MapToCSV : MonoBehaviour
{
    [Header("추출/생성 설정")]
    [Tooltip("자식들을 가지고 있는 부모 오브젝트 (이 부모의 1단계 자식들만 추출합니다)")]
    public Transform targetParent;

    [Tooltip("CSV 파일 이름 (예: MapData.csv)")]
    public string fileName = "MapData.csv";

    [Tooltip("CSV에서 읽어와서 새로 생성할 때 쓸 타일 프리팹")]
    public GameObject tilePrefabToSpawn;

    // ==========================================================
    // 1. 자식 위치 추출해서 CSV로 저장하기
    // ==========================================================
    [ContextMenu("1. 추출: 직속 자식 위치 -> CSV 저장")]
    public void ExportDirectChildrenToCSV()
    {
        TileNode[] tiles = targetParent.GetComponentsInChildren<TileNode>();

        if (targetParent == null)
        {
            Debug.LogError("추출할 targetParent가 지정되지 않았습니다!");
            return;
        }

        string filePath = Path.Combine(Application.dataPath, fileName);
        Debug.Log($"[경로 확인] 현재 찾고 있는 파일 위치: {filePath}");

        if (File.Exists(filePath))
        {
            Debug.Log("[경로 확인] 파일이 존재함을 감지했습니다! 팝업을 띄웁니다."); // 팝업 직전 확인

#if UNITY_EDITOR
            bool overwrite = UnityEditor.EditorUtility.DisplayDialog(
                "덮어쓰기 경고",
                $"'{fileName}' 파일이 이미 존재합니다.\n덮어쓰시겠습니까?",
                "덮어쓰기", "취소");

            if (!overwrite) return;
#endif
        }
        else
        {
            Debug.Log("[경로 확인] 파일이 없습니다! 팝업 없이 바로 저장합니다.");
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Index,Name,PosX,PosY,PosZ"); // 헤더 행

        // ★ 핵심: GetComponentsInChildren 대신 부모의 childCount를 이용해 직속 자식만 순회
        for (int i = 0; i < tiles.Length; i++)
        {
            // GetChild(i)는 정확히 1단계 아래의 자식만 가져옵니다.
            Transform child = tiles[i].transform;
            Vector3 pos = child.position;

            sb.AppendLine($"{i},{child.name},{pos.x},{pos.y},{pos.z}");
        }

        File.WriteAllText(filePath, sb.ToString());

        Debug.Log($"✅ [추출 완료] 총 {tiles.Length}개의 위치가 저장되었습니다: {filePath}");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    // ==========================================================
    // 2. CSV 읽어와서 타일 생성하기
    // ==========================================================
    [ContextMenu("2. 생성: CSV 읽기 -> 타일 배치")]
    public void ImportCSVAndSpawnTiles()
    {
        if (tilePrefabToSpawn == null)
        {
            Debug.LogError("생성할 tilePrefabToSpawn 프리팹을 지정해주세요!");
            return;
        }

        string filePath = Path.Combine(Application.dataPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        // CSV 파일의 모든 줄을 읽어옵니다.
        string[] lines = File.ReadAllLines(filePath);

        // 맵의 부모가 될 오브젝트
        GameObject parentObject = new GameObject("CSV_TO_MAP");

        // 첫 번째 줄(index 0)은 헤더이므로 건너뛰고, 데이터(index 1)부터 시작합니다.
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            // 빈 줄 무시
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 쉼표(,)를 기준으로 데이터를 쪼갭니다.
            // 쪼개진 배열 [0]: Index, [1]: Name, [2]: PosX, [3]: PosY, [4]: PosZ
            string[] values = line.Split(',');

            if (values.Length < 5) continue; // 데이터가 부족하면 패스


            try
            {
                // 문자열(String)을 실수(Float)로 변환
                float x = float.Parse(values[2]);
                float y = float.Parse(values[3]);
                float z = float.Parse(values[4]);

                Vector3 spawnPos = new Vector3(x, y, z);
                string originalName = values[1];

                // 프리팹 생성! (내 자식으로 넣음)
                GameObject newTile = Instantiate(tilePrefabToSpawn, spawnPos, Quaternion.identity, parentObject.transform);

                // 원본 이름 복구 (원한다면 _Copy 같은 걸 붙일 수도 있음)
                newTile.name = originalName;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{i}번째 줄 파싱 에러: {e.Message}");
            }
        }

        // 1을 빼는 이유: 헤더 줄을 제외하기 위함
        Debug.Log($"✅ [생성 완료] {lines.Length - 1}개의 타일이 CSV 좌표에 맞춰 생성되었습니다!");
    }
}
