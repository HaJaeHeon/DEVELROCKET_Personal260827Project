using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public string saveFileName = "SaveData_Slot_1.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 넘어가도 매니저 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 중복을 피해 비어있는 파일명을 알아서 찾아 저장합니다.
    public void CreateNewSaveSafe(GameDatas targetData)
    {
        int slotNumber = 1;
        string savePath = "";

        // 💡 [핵심 로직] 파일이 존재하지 않는 번호를 찾을 때까지 번호를 1씩 증가시킴!
        while (true)
        {
            string fileName = $"SaveData_Slot_{slotNumber}.json";
            savePath = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(savePath))
            {
                // 빈 번호를 찾았다면 while문을 빠져나감
                break;
            }

            slotNumber++; // 파일이 있으면 다음 번호로 넘어감
        }

        // 빈 경로(savePath)를 찾았으므로 그곳에 안전하게 저장
        string jsonText = JsonUtility.ToJson(targetData, true);
        File.WriteAllText(savePath, jsonText);
        Debug.Log($"중복을 피해 [ {slotNumber}번 ] 슬롯에 새 게임을 안전하게 생성했습니다!");
    }

    /// <summary>
    /// 파일에서 데이터를 불러옵니다. (Load)
    /// </summary>
    public GameDatas LoadGameData()
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        // 1. 파일이 존재하는지 검사
        if (File.Exists(savePath))
        {
            // 2. 파일 안의 JSON 텍스트를 읽어옴
            string jsonText = File.ReadAllText(savePath);

            // 3. 텍스트를 다시 GameDatas 객체로 복원해서 반환!
            GameDatas loadedData = JsonUtility.FromJson<GameDatas>(jsonText);

            Debug.Log("[Load] 세이브 파일 불러오기 성공!");
            return loadedData;
        }
        else
        {
            // 파일이 없다면 (처음 게임을 켰거나, 세이브 파일을 지웠을 때)
            Debug.Log("[Load] 세이브 파일이 없습니다. 새로운 데이터를 생성합니다.");

            // 완전 새 도시락통(기본값)을 만들어서 던져줌
            return new GameDatas();
        }
    }
}