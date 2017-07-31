using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteShower : MonoBehaviour {

    public GameObject meteor;
	public bool ShouldSpawn = false;

    public float spawn_rate = 3f;
    private float last_time = 0.0f;

    public Vector2 currentDir = new Vector2(0,-1);
    // private float spawn_dist = 55;
    public float MaxVelocity = 150.0f;
	public float MinVelocity = 50.0f;

	public Vector2 StationCenter;
	public float XDeviation;
	public float YDeviation;

	private cRandom Random;

    void Start () {
		Random = new cRandom ( new System.Random ( ).Next ( ) );
	}
	
	void Update () {
		if ( ShouldSpawn ) {
			TimeCheck ( );
		}
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
			y = Random.RandomInRange ( 0.0f, 200.0f );
			if ( Random.RandomFloat ( ) < 0.5f ) { // Left
				x = 0.0f;
			} else { // Right
				x = 200.0f;
			}
		} else { // Top or bottom
			x = Random.RandomInRange ( 0.0f, 200.0f );
			if ( Random.RandomFloat ( ) < 0.5f ) { // Bottom
				y = 0.0f;
			} else { // Top
				y = 200.0f;
			}
		}

		Vector2 hit = GetPointToHit ( );
		Vector2 vel = ( hit - new Vector2 ( x, y ) ).normalized * Random.RandomInRange ( MinVelocity, MaxVelocity );

		GameObject m = ( GameObject ) Instantiate ( meteor, new Vector3 ( x, y ), Quaternion.identity );
		m.GetComponent<Rigidbody2D> ( ).velocity = vel;
		Destroy ( m, 30.0f );

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
		return new Vector2 ( Random.RandomInRange ( StationCenter.x - XDeviation, StationCenter.x + XDeviation ), Random.RandomInRange ( StationCenter.y - YDeviation, StationCenter.y + YDeviation ) );
	}
}
