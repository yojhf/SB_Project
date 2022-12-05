using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    [SerializeField] private float waterDrag;   // 물속 중력
    private float originDrag;

    [SerializeField] private Color waterColor;      // 물속 색깔
    [SerializeField] private float waterFogDensity; // 물 탁함 정도

    [SerializeField] private Color waterNightColor; // 밤 물속 색깔
    [SerializeField] private float waterNightFogDensity; // 밤 물 탁함 정도

    private Color originColor;      // 기본 색깔
    private float originFogDensity; // 기본 탁함 정도

    [SerializeField] private Color originNightColor;        // 기본 밤 색깔
    [SerializeField] private float originNightFogDensity;   // 기본 밤 탁함 정도

    [SerializeField] private string sound_WaterOut;
    [SerializeField] private string sound_WaterIn;
    [SerializeField] private string sound_Breathe;

    [SerializeField] private float breatheTime;
    private float currentBreatheTime;

    [SerializeField] private float totalOxygen;
    private float currentOxygen;
    private float temp;

    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private Text text_totalOxygen;
    [SerializeField] private Text text_currentOxygen;
    [SerializeField] private Image image_gauge;

    private StatusController thePlayerStat;
    
    void Start()
    {
        originColor = RenderSettings.fogColor;
        originFogDensity = RenderSettings.fogDensity;

        originDrag = 0;

        thePlayerStat = FindObjectOfType<StatusController>();
        currentOxygen = totalOxygen;
        text_totalOxygen.text = totalOxygen.ToString();
    }

    void Update() {
        if (GameManager.isWater){
            currentBreatheTime += Time.deltaTime;
            if (currentBreatheTime >= breatheTime){
                SoundManager.instance.PlaySE(sound_Breathe);
                currentBreatheTime = 0;
            }
        }

        DecreaseOxygen();
    }

    private void DecreaseOxygen()
    {
        if (GameManager.isWater){
            if (currentOxygen <= 0){
                temp += Time.deltaTime;
                if (temp >= 1){
                    thePlayerStat.DecreaseHP(1);
                    temp = 0;
                }
            }
            else{
                currentOxygen -= Time.deltaTime;
                text_currentOxygen.text = Mathf.RoundToInt(currentOxygen).ToString();
                image_gauge.fillAmount = currentOxygen / totalOxygen;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player")){
            GetWater(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.CompareTag("Player")){
            GetOutWater(other);
        }
    }

    private void GetWater(Collider _player)
    {
        SoundManager.instance.PlaySE(sound_WaterIn);

        go_BaseUI.SetActive(true);

        GameManager.isWater = true;
        _player.transform.GetComponent<Rigidbody>().drag = waterDrag;

        if(!GameManager.isNight){
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFogDensity;
        }
        else{
            RenderSettings.fogColor = waterNightColor;
            RenderSettings.fogDensity = waterNightFogDensity;
        }
    }

    private void GetOutWater(Collider _player)
    {
        if (GameManager.isWater){
            currentOxygen = totalOxygen;
            SoundManager.instance.PlaySE(sound_WaterOut);

            go_BaseUI.SetActive(false);
            
            GameManager.isWater = false;
            _player.transform.GetComponent<Rigidbody>().drag = originDrag;

            if(!GameManager.isNight){
                RenderSettings.fogColor = originColor;
                RenderSettings.fogDensity = originFogDensity;
            }
            else{
                RenderSettings.fogColor = originNightColor;
                RenderSettings.fogDensity = originNightFogDensity;
            }
        }
    }
}
