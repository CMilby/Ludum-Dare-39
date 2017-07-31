using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlash : MonoBehaviour {

	private Light Light;

	public float WaitTime = 0.25f;
	public float MaxIntensity = 2.25f;

	private int LightState = 2;

	void Start () {
		Light = GetComponent<Light> ( );
	}

	void Update () {
		if ( LightState == 1 ) {
			StartCoroutine ( WaitForOn ( WaitTime, 0.0f, MaxIntensity ) );
		} else if ( LightState == 2 ) {
			StartCoroutine ( WaitForOff ( WaitTime, MaxIntensity, 0.0f ) );
		}
	}

	IEnumerator WaitForOn( float p_time, float p_start, float p_end ) {
		LightState = 3;
		float t = 0.0f;
		while ( t < p_time ) {
			t += Time.deltaTime;
			Light.intensity = Mathf.Lerp ( p_start, p_end, t / p_time );
			yield return new WaitForSeconds ( 0.01f );
		}
		LightState = 2;
	}

	IEnumerator WaitForOff( float p_time, float p_start, float p_end ) {
		LightState = 3;
		float t = 0.0f;
		while ( t < p_time ) {
			t += Time.deltaTime;
			Light.intensity = Mathf.Lerp ( p_start, p_end, t / p_time );
			yield return new WaitForSeconds ( Time.deltaTime );
		}
		LightState = 1;
	}
}
