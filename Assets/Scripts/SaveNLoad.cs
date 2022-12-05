using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;   // 플레이어 위치 저장
    public Vector3 playerRot;   // 플레이어 방향 저장

    public List<int> invenArrayNumber = new List<int>();        // 아이템의 인벤토리 위치 저장
    public List<string> invenItemName = new List<string>();     // 아이템의 이름 저장
    public List<int> invenItemNumber = new List<int>();         // 아이템의 개수 저장
}

public class SaveNLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;             // 저장 경로
    private string SAVE_FILENAME = "/SaveFile.txt"; // 저장 파일 이름

    private PlayerController thePlayer;
    private Inventory theInven;

    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        // 폴더 존재 확인
        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
    }
    
    // 저장 메소드
    public void SaveData()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        theInven = FindObjectOfType<Inventory>();

        // 플레이어 위치 저장
        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        // 인벤토리 안에 있는 슬롯 배열 전부 가져온 후 저장
        Slot[] slots = theInven.GetSlots();
        for (int i = 0; i < slots.Length; i++){
            if (slots[i].item != null){
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(slots[i].item.itmeName);
                saveData.invenItemNumber.Add(slots[i].itemCount);
            }
        }

        // json을 사용하여 세이브
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    // 로드 메소드
    public void LoadData()
    {
        // 파일이 존재할 경우
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME)){
            // json 파일을 그대로 읽으면 안되므로 SaveData 클래스에 맞게 변경한다.
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            thePlayer = FindObjectOfType<PlayerController>();
            theInven = FindObjectOfType<Inventory>();

            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;

            for (int i = 0; i < saveData.invenItemName.Count; i++){
                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            }

            Debug.Log("로드 완료");
        }
        else
            Debug.Log("세이브 파일이 없습니다.");
        
    }
}
