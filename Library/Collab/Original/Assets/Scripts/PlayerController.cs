using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float thrust;
    public float max_velocity;
    public bool onStation;

    private Vector3 direction;
    private Rigidbody2D rb;
	private Collider2D collide;

    public GameObject sprite;
    public GameObject jets;
    public Animator anim;

    public LayerMask what_is_station;

    public AudioSource audio_src;

    public GameObject interactor_object;
    public bool isInteracting = false;

	private PlayerInventory inventory;

    private float max_amb_vol;

	private float TimeOutJetpackEnable = 0.05f;
	private bool ShouldEnable = true;

    public float tether_dist;

	public bool HasWrench;
	public bool HasWelder;

	void Awake ( ) {
		inventory = GetComponent<PlayerInventory> ( );
	}

    // Use this for initialization
    void Start()
    {
        if(!rb)
            rb = GetComponent<Rigidbody2D>();

		if ( !collide ) {
			collide = GetComponent<Collider2D> ( );
		}

        max_amb_vol = audio_src.volume;

    }

    void Update()
    {

		HasWelder = inventory.HasWelder;
		HasWrench = inventory.HasWrench;

        if (onStation)
        {
            audio_src.volume = Mathf.Lerp(audio_src.volume, max_amb_vol, 0.07f);
            GetComponent<DistanceJoint2D>().enabled = false;
        }
        else
        {
            audio_src.volume = Mathf.Lerp(audio_src.volume, 0.0f, 0.07f);
            tether();
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        direction = new Vector3(moveX, moveY, 0.0f);

        float mult = 1;

        if (isInteracting)
        {
            mult = 0.1f;
        }
        else
        {
            mult = 1;
        }


        if (onStation)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, thrust * direction.normalized * mult, 0.3f);

            anim.SetFloat("Velocity", rb.velocity.magnitude);
            anim.SetBool("Floating", false);

            jets.SetActive(false);

        }
        else if(!onStation)
        {

            if (!isInteracting)
                rb.AddForce(direction.normalized * thrust * mult);
            else
                rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0,0), 0.02f);
    
            anim.SetFloat("Velocity", rb.velocity.magnitude);
            anim.SetBool("Floating", true);

            jets.SetActive(true);

        }




        Vector3 angles = sprite.transform.localEulerAngles;


        if (isInteracting)
        {
            print("asdfasdf");

            float x_d = interactor_object.transform.position.x - transform.position.x;
            float y_d = interactor_object.transform.position.y - transform.position.y;

            angles.z = (Mathf.Atan2(y_d, x_d) - (Mathf.PI / 2.0f)) * Mathf.Rad2Deg;
        }
        else
        {
            if (moveX != 0 || moveY != 0)
            {
                angles.z = ((Mathf.Atan2(direction.normalized.y, direction.normalized.x) - (Mathf.PI / 2.0f)) * Mathf.Rad2Deg);

            }
        }


       


        if (angles.z < 0)
            angles.z = 360 + angles.z;

        float new_angle = sprite.transform.localEulerAngles.z;
        new_angle = Mathf.Lerp(new_angle, angles.z, 0.2f);
       // angles.z = new_angle;


        sprite.transform.localEulerAngles = angles;// Vector3.Lerp(sprite.transform.localEulerAngles, angles, 0.5f);

    }

	private void OnTriggerEnter2D ( Collider2D p_collider ) {
		if ( p_collider.tag == "tool" ) {
			inventory.AddTool ( p_collider.gameObject.GetComponent<Tool> ( ) );
			Destroy ( p_collider.gameObject );
		}
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
			if ( collision.bounds.Intersects ( collide.bounds ) ) {
				onStation = true;
				audio_src.enabled = true;
				ShouldEnable = false;
			}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
           // onStation = false;
			ShouldEnable = true;
			StartCoroutine ( EnableJetpack() );
        }
    }

	IEnumerator EnableJetpack() {
		yield return new WaitForSeconds ( TimeOutJetpackEnable );

		if ( ShouldEnable ) {
			onStation = false;
		}
	}




    private void tether()
    {
        GameObject connected = findTileToTether();
        GetComponent<DistanceJoint2D>().enabled = true;

        //print("pos: " + connected.transform.position.ToString());

        Vector2 pos_0 = transform.position;
        Vector2 pos_1 = connected.transform.position;

        //GetComponent<DistanceJoint2D>().connectedAnchor = pos_1;
        GetComponent<DistanceJoint2D>().connectedAnchor = pos_1;
        GetComponent<DistanceJoint2D>().distance = tether_dist;


    }

    private GameObject findTileToTether()
    {
        float min_dist = 9999;

        GameObject closest = null;

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("tile"))
        {
            float dist = Vector2.Distance(transform.position, g.transform.position);
            if (dist < min_dist)
            {
                min_dist = dist;
                closest = g;
            }
        }

        return closest;

    }
    
}
