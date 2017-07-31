using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour {

    public float paralax_amount;

    private Vector2 main_origin;
    private Vector2 my_origin;

    private Vector2 offset;

    void Start()
    {
        main_origin = Camera.main.transform.position;
        my_origin = transform.position;
    }

    void Update()
    {

        offset = main_origin - (Vector2)Camera.main.transform.position;

        transform.position = my_origin + (Vector2)(offset / paralax_amount);


    }
}
