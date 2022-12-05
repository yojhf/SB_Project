using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private SaveNLoad theSaveNLoad;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            if (!GameManager.isPause){
                CallMenu();
            }
            else{
                CloseMenu();
            }
        }
    }

    private void CallMenu()
    {
        GameManager.isPause = true;
        go_BaseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    private void CloseMenu()
    {
        GameManager.isPause = false;
        go_BaseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ClickSave()
    {
        Debug.Log("Save");
        theSaveNLoad.SaveData();
    }

    public void ClickLoad()
    {
        Debug.Log("Load");
        theSaveNLoad.LoadData();
    }

    public void ClickExit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
