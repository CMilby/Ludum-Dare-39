using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixable : MonoBehaviour {

    public float health;
    private float max_health = 1.0f;

    public float fix_rate = 0.004f;
    public float fix_distance;

    public bool beingFixed;

    public bool isSolar;
    public bool isBattery;

    public GameObject player;
    public GameObject needs_fixed_icon;
    public GameObject health_bar;

    public GameObject fixing_effect;
    public GameObject broken_effect;

    void Start () {
        health = max_health;
    }
	
	void Update () {

        health_bar.SetActive(false);
        needs_fixed_icon.SetActive(false);


        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if (beingFixed)
        {
            if (isSolar)
            {
                health += fix_rate * player.GetComponent<PlayerController>().solar_fix_rate;
            }
            if (isBattery)
            {
                health += fix_rate * player.GetComponent<PlayerController>().battery_fix_rate;
            }

            if (health >= max_health)
            {
                gameObject.SendMessage("fix", SendMessageOptions.DontRequireReceiver);
                health = max_health;

                if (isSolar)
                {
					player.GetComponent<PlayerInventory> ( ).Fixed ( eToolType.TOOL_WELDER );
                }

                if (isBattery)
                {
					player.GetComponent<PlayerInventory> ( ).Fixed ( eToolType.TOOL_WRENCH );
                }

            }
            else if(health < max_health)
            {
                needs_fixed_icon.SetActive(false);
                showHealth();
            }

            fixing_effect.SetActive(true);
            broken_effect.SetActive(false);

        }
        else if(health < max_health && health > 0)
        {
            health -= 0.0025f / 3.0f;
            needs_fixed_icon.SetActive(false);
            broken_effect.SetActive(true);
            fixing_effect.SetActive(false);
            showHealth();
        }
        else if(health <= 0)
        {
            needs_fixed_icon.SetActive(true);
            fixing_effect.SetActive(false);
            broken_effect.SetActive(true);

        }
        else
        {
            fixing_effect.SetActive(false);
            broken_effect.SetActive(false);

        }

    }

    void showHealth()
    {
        health_bar.SetActive(true);
        health_bar.GetComponent<GeneralHealthBar>().setHealth(health / max_health);
    }

    void takeDamage(int amount)
    {
        health -= amount;
    }

    private void OnMouseOver()
    {

        beingFixed = false;

        if (player)
        {
            player.GetComponent<PlayerController>().isInteracting = false;
            float dist = Vector2.Distance(player.transform.position, transform.position);

            if (dist < fix_distance)
            {
                if (Input.GetMouseButton(0))
                {
                    if(health < max_health)
                    {

                        if (player.GetComponent<PlayerController>().onStation && isBattery)
                        {
                            beingFixed = true;
                        }
                        else if (!player.GetComponent<PlayerController>().onStation && isSolar)
                        {
                            beingFixed = true;
                        }

                        if (beingFixed)
                        {
                            player.GetComponent<PlayerController>().isInteracting = true;
                            player.GetComponent<PlayerController>().interactor_object = gameObject;

                            Vector3 temp_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            temp_pos.z = -2;
                            fixing_effect.transform.position = temp_pos;

                        }
                        
                    }

                }

            }

        }

    }

    private void OnMouseExit()
    {
        beingFixed = false;
        player.GetComponent<PlayerController>().isInteracting = false;

    }



}
