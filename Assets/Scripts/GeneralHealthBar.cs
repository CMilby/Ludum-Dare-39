using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralHealthBar : MonoBehaviour {

    public GameObject fill_bar;

    private float health;

    public Color full_health_color;
    public Color low_health_color;

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void setHealth(float h)
    {

        health = h;

        Vector2 temp_scl = fill_bar.transform.localScale;
        temp_scl.x = Mathf.Lerp(0,1,h);
        fill_bar.transform.localScale = temp_scl;

        //Vector2 temp_pos = fill_bar.transform.localPosition;
        //temp_pos.x = Mathf.Lerp(-0.2f, 0.0f, h);
       // fill_bar.transform.position = temp_pos;

        fill_bar.GetComponent<SpriteRenderer>().color = Color.Lerp(low_health_color, full_health_color, health);

    }
}
