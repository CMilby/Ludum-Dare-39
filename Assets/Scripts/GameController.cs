using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public MeteoriteShower Shower;
	public AnimationCurve MeteorSpawnRateCurve;

	public int CurrentWave = 0;
	public float LifeSupportState = 1.0f;

	public float WaveDelay = 15.0f;
	public float CurrentWaveTotalTime = 30.0f;

	[HideInInspector]
	public float NextWaveStartTime;
	private bool WaveStarted = false;
	[HideInInspector]
	public bool WaveEnded = false;

	public bool GameOver = false;

    void Start () {
		NextWaveStartTime = Time.time + WaveDelay;
		WaveStarted = false;
	}

	void Update () {
		if ( Time.time >= NextWaveStartTime && !WaveStarted ) {
			StartCoroutine ( StartWave ( ) );
		}
	}

	IEnumerator StartWave ( ) {
		WaveStarted = true;
		CurrentWave++;
		Shower.spawn_rate = MeteorSpawnRateCurve.Evaluate ( CurrentWave / 100.0f ) * 3.0f;
		Shower.ShouldSpawn = true;
		WaveEnded = false;
		yield return new WaitForSeconds ( CurrentWaveTotalTime );
		CurrentWaveTotalTime += 2.0f;
		NextWaveStartTime = Time.time + WaveDelay;
		WaveStarted = false;
		Shower.ShouldSpawn = false;
		WaveEnded = true;
	}
}
