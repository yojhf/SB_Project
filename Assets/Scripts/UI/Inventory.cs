using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;  // 인벤토리 실행 시 마우스만 움직이도록 사용할 변수

    // 필요한 컴포넌트 
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotParent;
    [SerializeField]
    private ItemEffectDatabase theItemEffectDatabase;

    // 슬롯들
    private Slot[] slots;

    public Slot[] GetSlots() { return slots; }  // 현재 인벤토리의 슬롯들을 전체 반환 시켜줄 메소드

    [SerializeField] private Item[] items;      // 게임에 존재하는 아이템

    // 세이브 로드 시 사용하게될 메소드
    public void LoadToInven(int _arrayNum, string _itemName, int _itemNum)
    {
        for (int i = 0; i < items.Length; i++){
            if (items[i].itmeName == _itemName){
                slots[_arrayNum].AddItem(items[i], _itemNum);
            }
        }
    }

    void Start()
    {
        slots = go_SlotParent.GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I)){
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
    }

    private void OpenInventory()
    {
        GameManager.isOpenInventory = true;
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        GameManager.isOpenInventory = false;
        go_InventoryBase.SetActive(false);
        theItemEffectDatabase.HideToolTip();    // 아이템 툴팁 끄기
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        // 장비가 아닐 경우
        if (Item.ItemType.Equipment != _item.itemType){
            for (int i = 0; i < slots.Length; i++){
                if (slots[i].item != null){
                    if (slots[i].item.itmeName == _item.itmeName){
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++){
                if (slots[i].item == null){
                    slots[i].AddItem(_item, _count);
                    return;
                }
            }
    }
}
