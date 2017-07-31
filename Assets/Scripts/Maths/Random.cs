using UnityEngine;
using System.Collections;
using System;

public class cRandom {

	private MersenneTwister Twister;

	public cRandom ( ) {
		Twister = new MersenneTwister ( 1234 );
	}

	public cRandom ( long p_seed ) {
		Twister = new MersenneTwister ( p_seed );
	}

	public void SetSeed ( long p_seed ) {
		Twister.SetSeed ( p_seed );
	}

	public double RandomDouble ( ) {
		double value = ( ( double ) Twister.Random ( ) / ( double ) long.MaxValue ) * 10000000000.0;
		return value - Math.Truncate ( value );
	}

	public float RandomFloat ( ) {
		float value = ( ( float ) Twister.Random ( ) / ( float ) long.MaxValue ) * 10000000000.0f;
		return value - ( float ) Math.Truncate ( value );
	}

	public int RandomInRange ( int p_min, int p_max ) {
		if ( p_max < p_min ) {
			int temp = p_max;
			p_max = p_min;
			p_min = temp;
		}

		return p_min + ( int ) ( RandomDouble ( ) * ( ( p_max - p_min ) + 1 ) );
	}

	public float RandomInRange ( float p_min, float p_max ) {
		return p_min + ( p_max - p_min ) * RandomFloat ( );
	}
}

