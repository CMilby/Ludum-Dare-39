using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour {

	public float PanX = 1.0f;
	public float PanY = 1.0f;
	public float PanSpeed = 2.0f;

	void Start () {
		
	}
	
	void Update () {
		float moveSpeed = Time.deltaTime * PanSpeed;
		transform.position += ( new Vector3 ( PanX, PanY, 0.0f ).normalized * moveSpeed );
	}
}
