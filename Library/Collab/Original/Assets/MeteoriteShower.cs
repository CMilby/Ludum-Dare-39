using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteShower : MonoBehaviour {

    public GameObject meteor;

    private float spawn_rate = 3f;
    private float last_time = 0.0f;

    public Vector2 currentDir = new Vector2(0,-1);
    private float spawn_dist = 55;
    public float meteor_vel = 100;

	public Vector2 StationCenter;
	public float XDeviation;
	public float YDeviation;

	private cRandom Random;

    void Start () {
		Random = new cRandom ( new System.Random ( ).Next ( ) );
	}
	
	void Update () {

        TimeCheck();

	}

    void TimeCheck()
    {

        if(Time.time > last_time + spawn_rate)
        {
            //spawn
            last_time = Time.time;

            SpawnMeteor();

        }

    }


    private void SpawnMeteor() {
		float x = 0.0f;
		float y = 0.0f;

		if ( Random.RandomFloat ( ) < 0.5f ) { // Left or right
			if ( Random.RandomFloat ( ) < 0.5f ) { // Left

			} else { // Right

			}
		} else { // Top or bottom

		}

		/*Vector2 dir = currentDir;
        dir.x += Random.Range(-0.2f,0.2f);
        dir.y += Random.Range(-0.2f,0.2f);

        Vector2 start_pos = spawn_dist * (-currentDir);
        Vector2 velocity = dir * meteor_vel * Random.Range(0.7f, 1.2f);

        GameObject m = (GameObject)Instantiate(meteor, start_pos, Quaternion.identity);

        m.GetComponent<Rigidbody2D>().velocity = velocity;

        Destroy(m, 5);*/
    }

	private Vector2 GetPointToHit() {
		return new Vector2 ( );
	}
}
