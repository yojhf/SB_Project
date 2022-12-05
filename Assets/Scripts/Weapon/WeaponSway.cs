using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // 기존 위치
    private Vector3 originPos;

    // 현재 위치
    private Vector3 currentPos;

    // Sway 한계 (무기 흔들림 max)
    [SerializeField]
    private Vector3 limitPos;

    // 정조준 Sway 한계
    [SerializeField]
    private Vector3 fineSightlimitPos;

    // 부드러운 움직임 정도
    [SerializeField]
    private Vector3 smoothSway;

    // 필요한 컴포넌트
    [SerializeField]
    private GunController theGunController;

    void Start()
    {
        originPos = this.transform.localPosition;
    }

    void Update()
    {
        if (GameManager.canPlayerMove && !Inventory.inventoryActivated)
            TrySway();
    }

    private void TrySway()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0){
            Swaying();
        }
        else
            BackToOriginPos();
    }

    private void Swaying()
    {
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if (!theGunController.GetFineSightMode()){
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -limitPos.y, limitPos.y),
                           originPos.z);
        }
        else{
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -fineSightlimitPos.x, fineSightlimitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightlimitPos.y, fineSightlimitPos.y),
                           originPos.z);
        }

        transform.localPosition = currentPos;
    }

    private void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
