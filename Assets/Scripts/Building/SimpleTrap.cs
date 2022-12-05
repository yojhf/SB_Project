using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTrap : MonoBehaviour
{
    private Rigidbody[] rigid;
    [SerializeField] private GameObject go_Meat;

    [SerializeField] private int damage;

    private bool isActivated = false;

    private AudioSource theAudio;
    [SerializeField] private AudioClip sound_Activate;
    
    private StatusController theStatus;

    void Start()
    {
        rigid = GetComponentsInChildren<Rigidbody>();
        theAudio = GetComponent<AudioSource>();
        theStatus = FindObjectOfType<StatusController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (!isActivated){
            if (!other.transform.CompareTag("Untagged")){
                isActivated = true;
                theAudio.clip = sound_Activate;
                theAudio.Play();

                Destroy(go_Meat);   // 고기 제거

                for (int i = 0; i < rigid.Length; i++){
                    rigid[i].useGravity = true;
                    rigid[i].isKinematic = false;
                }

                if (other.transform.tag == "Player"){
                    theStatus.DecreaseHP(damage);
                }
            }
        }
    }
}
