using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldVisuals : MonoBehaviour {

    public GameObject[] planets;
    public bool[] picked;

    public float planet_range;

    int num_planets;

    private void Start()
    {
        picked = new bool[planets.Length];

        num_planets = Random.Range(1,5);


        for(int i = 0; i < num_planets; i++)
        {

            //no planet repetition
            int index = Random.Range(0,planets.Length);
            while (picked[index])
            {
                index = Random.Range(0, planets.Length);
            }

            picked[index] = true;

            int pick = Random.Range(i+1, 5);
            int scale = 4 * pick;// Random.Range(2,20);

            Vector2 pos = Random.insideUnitCircle * planet_range;
            Vector2 scl = new Vector2(scale, scale);

            GameObject p = (GameObject) Instantiate(planets[index], pos, Quaternion.identity);

            p.transform.localScale = scl;
            p.GetComponent<Paralax>().paralax_amount = 30 - scale;
            p.GetComponent<SpriteRenderer>().sortingOrder = -(30 - scale);
        }

    }

}
