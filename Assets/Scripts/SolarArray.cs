using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarArray : MonoBehaviour {

    public SpriteRenderer sr;

    public Sprite brokenSprite;
    public Sprite fixedSprite;

    public Fixable fx;

    public bool isBroken = false;

    public float current_power_production_eff;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        fx = GetComponent<Fixable>();
    }

    void Update() {

    }

    public void fix()
    {

        isBroken = false;
        sr.sprite = fixedSprite;

    }


    public void destroy()
    {
        isBroken = true;
        sr.sprite = brokenSprite;
        fx.health = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "meteor")
        {

            destroy();
            Destroy(collision.gameObject);

        }
    }

}
