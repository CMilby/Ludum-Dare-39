using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {

    public GameObject meteor_hit_explosion;

	void Start () {
		
	}

	void Update () {
		
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 norm = collision.contacts[0].normal;
        GameObject MHE = null;

        if (meteor_hit_explosion)
            MHE = (GameObject)Instantiate(meteor_hit_explosion, collision.contacts[0].point, Quaternion.FromToRotation(collision.contacts[0].point, norm));


        float dist = Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        float intensity = Mathf.Lerp(1.0f, 0.0f, dist / 60.0f);

        GameObject.FindGameObjectWithTag("camera").GetComponent<CameraController>().shake(0.3f * intensity, 0.9f);


        if (collision.collider.gameObject.tag == "solar")
            print("hit solar panel");


        Destroy(gameObject);
        Destroy(MHE, 4);


    }

}
