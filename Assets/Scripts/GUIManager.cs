using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUIManager : MonoBehaviour {

    public Slider Slider;
    public float batteryLife;
    public float gameLength;

    //private float endTime;
    private float currentBattery;
    private float burnRate;

	void Start () {
        Slider.value = batteryLife;
        Slider.maxValue = batteryLife;
        burnRate = batteryLife / gameLength;
        Slider.transform.position = new Vector3((Screen.width / 2) - (Slider.maxValue/2), 5);
	}
	
	void Update () {
        //currentTime = Time.time;
        currentBattery = Slider.value;    

        if(currentBattery > 0) {
            Slider.value -= burnRate; 
        }
        
	}

    void addPower (float p_time)
    {
        if(currentBattery + p_time > batteryLife)
        {
            Slider.value = batteryLife;
        }
        else
        {
            
        }
    }


}
