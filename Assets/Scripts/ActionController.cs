using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;    // 습득 가능한 최대 거리
    
    private bool pickupActivated = false;   // 습득 가능할 시 true

    private RaycastHit hitInfo; // 충돌체 정보 저장

    // 아이템 레이어에만 반응하도록 레이어마스크를 설정
    [SerializeField]
    private LayerMask layerMask;

    // 필요한 컴포넌트
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;

    void Update()
    {
        CheckItem();    // 아이템 여부 확인
        TryAction();    // 아이템 줍기 시도
    }

    // 아이템 줍기 시도
    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            CheckItem();    // 아이템 여부 확인 (확실하게 확인하고 줍기 위해 한번 더 호출)
            CanPickUp();    // 아이템 줍기
        }
    }

    // 아이템 줍기 동작
    private void CanPickUp()
    {
        // 줍기가 가능할 때
        if (pickupActivated)
        {
            // 아이템이 존재할 때
            if (hitInfo.transform != null){
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itmeName + " 흭득 ");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }

    // 아이템 확인
    private void CheckItem()
    {
        // 카메라 위치에서 동작 (스크립트를 카메라에 넣어줌)
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask)){
            if (hitInfo.transform.CompareTag("Item")){
                ItemInfoAppear();
            }
        }
        else
            InfoDisappear();
    }

    // 아이템이 확인되었을 때
    private void ItemInfoAppear()
    {
        // 아이템 줍기 가능
        pickupActivated = true;

        // UI 활성화
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itmeName + " 흭득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    // 아이템을 주웠을 때
    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
