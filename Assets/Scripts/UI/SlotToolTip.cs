using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private Text txt_ItemName;
    [SerializeField]
    private Text txt_ItemDesc;
    [SerializeField]
    private Text txt_ItemHowtoUsed;

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);

        // ToolTip 위치를 적당한 곳에 배치 (_pos = Slot의 position)
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.65f, -go_Base.GetComponent<RectTransform>().rect.height * 0.75f, 0f);
        go_Base.transform.position = _pos;

        txt_ItemName.text = _item.itmeName;
        txt_ItemDesc.text = _item.itemDesc;

        if (Item.ItemType.Equipment == _item.itemType){
            txt_ItemHowtoUsed.text = "우클릭 - 장착";
        }
        else if (Item.ItemType.Used == _item.itemType){
            txt_ItemHowtoUsed.text = "우클릭 - 먹기";
        }
        else
            txt_ItemHowtoUsed.text = "";
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
