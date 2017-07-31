using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBattery : MonoBehaviour {

    public int charge_level;

    public GameObject[] cells;

    public SpriteRenderer sr;

    public Sprite brokenSprite;
    public Sprite fixedSprite;

    public Fixable fx;

    public bool isBroken = false;

    [SerializeField] float next_fail_time;





    //audio
    public AudioSource audio_src;

    public AudioClip explode;
    public AudioClip fixing;


    private void Start()
    {
        if(!sr)
            sr = GetComponent<SpriteRenderer>();
        if(!fx)
            fx = GetComponent<Fixable>();


        updateCellSprites();

        next_fail_time = Random.Range(100,400);
        Invoke("destroy", next_fail_time);

    }

    void Update () {

    }

    public void fix()
    {
        isBroken = false;
        sr.sprite = fixedSprite;
    }

    public void destroy()
    { 
        charge_level = 0;
        isBroken = true;
        sr.sprite = brokenSprite;
        fx.health = 0;
        updateCellSprites();

        //audio
        audio_src.clip = explode;
        audio_src.Play();


        //random battery failures
        next_fail_time = Random.Range(100, 400);
        Invoke("destroy", next_fail_time);

    }


    public void updateCellSprites()
    {

        for (int i = 0; i < cells.Length; i++)
        {

            if (i <= charge_level - 1)
                cells[i].SetActive(true);
            else
                cells[i].SetActive(false);

        }
    }
    

    public void charge(int amount)
    {

        charge_level += amount;

        if (charge_level >= cells.Length-1)
            charge_level = cells.Length-1;

        updateCellSprites();

    }

    public void setChargeLevel(int level)
    {

        charge_level = level;

        if (charge_level >= cells.Length)
            charge_level = cells.Length;
        if (charge_level <= 0)
            charge_level = 0;

        updateCellSprites();

    }

    public void drain(int amount)
    {

        charge_level -= amount;

        if (charge_level <= 0)
            charge_level = 0;

        updateCellSprites();


    }

}
