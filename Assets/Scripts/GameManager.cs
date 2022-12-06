using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; //플레이어의 움직임 젱
    public static bool isOpenInventory = false; // 인벤토리 활성화
    public static bool isOpenCraftManual = false; // 건축 메뉴창 활성화

    public static bool isNight = false; 
    public static bool isWater = false;

    public static bool isPause = false; // 메뉴가 호출되면 true

    private WeaponManager theWM;
    private bool flag = false;

    void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // <- CursorLockMode.Locked에 들어있음.
        theWM = FindObjectOfType<WeaponManager>();
    }

    void Update()
    {
        if (isOpenInventory || isOpenCraftManual || isPause){
            canPlayerMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else{
            canPlayerMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (isWater){
            if (flag == false){
                StopAllCoroutines();
                StartCoroutine(theWM.WeaponInCoroutine());
                flag = true;
            }
        }
        else{
            if (flag == true){
                theWM.WeaponOut();
                flag = false;
            }
        }
    }
}
