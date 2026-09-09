using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    //파일 경로 프로젝트 폴더 안으로 변경해야함
    public string initDataFile = "InitSaveFile.json";
    public string saveFileName = "SaveDataFile.json";
    public GameDatas currentDatas;

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
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        string jsonText = JsonUtility.ToJson(targetData, true);
        File.WriteAllText(savePath, jsonText);
        Debug.Log($"새로운 SaveFile 생성 / 생성 위치 : {savePath}");
    }

    /// <summary>
    /// 파일에서 데이터를 불러옵니다. (Load)
    /// </summary>
    public GameDatas LoadGameData()
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(savePath))
        {
            string jsonText = File.ReadAllText(savePath);

            GameDatas loadedData = JsonUtility.FromJson<GameDatas>(jsonText);

            Debug.Log("[Load] 세이브 파일 불러오기 성공!");
            return loadedData;
        }
        else
        {
            // 파일이 없다면 (처음 게임을 켰거나, 세이브 파일을 지웠을 때)
            Debug.Log("[Load] 세이브 파일이 없습니다. 초기 데이터를 가져옵니다.");

            string newPath = Path.Combine(Application.persistentDataPath, initDataFile);
            if(File.Exists(newPath))
            {
                string jsonText = File.ReadAllText(newPath);

                GameDatas loadedData = JsonUtility.FromJson<GameDatas>(jsonText);

                return loadedData;
            }
            else
            {
                GameDatas data = new GameDatas();
                Debug.Log("초기 파일도 없어서 초기 파일 생성");
                // 여기 진짜 초기 파일 생성하는거 만들어야함
                return data;
            }
        }
    }
    public GameDatas LoadInitData()
    {
        string dataPath = Path.Combine(Application.persistentDataPath, initDataFile);

        if(File.Exists(dataPath))
        {
            string jsonText = File.ReadAllText(dataPath);

            GameDatas loadedData = JsonUtility.FromJson<GameDatas>(jsonText);

            Debug.Log("[Load] 초기 데이터 파일 불러오기 성공");

            return loadedData;
        }
        else
        {
            Debug.Log("[Load] 초기 데이터 파일이 없습니다. 새로운 데이터를 생성합니다.");

            return new GameDatas();
        }
    }

    public void ClickLoadData()
    {
        currentDatas = LoadGameData();
    }

    public void ClickStartInitData()
    {
        currentDatas = LoadInitData();
    }
}