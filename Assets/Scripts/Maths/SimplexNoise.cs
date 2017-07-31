using UnityEngine;
using System.Collections;

public class SimplexNoise {

	private readonly int[,] grad3 = new int[ 12, 3 ] {
		{ 1, 1, 0 }, { -1,  1, 0 }, { 1, -1,  0 }, { -1, -1,  0 },
		{ 1, 0, 1 }, { -1,  0, 1 }, { 1,  0, -1 }, { -1,  0, -1 },
		{ 0, 1, 1 }, { 0, -1, 1 }, { 0,  1, -1 }, { 0, -1, -1 }	
	};

	private int[] perm = new int[ 512 ] {
		151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142,
		8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117,
		35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71,
		134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41,
		55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
		18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
		250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182,
		189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43,
		172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97,
		228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239,
		107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
		138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,

		151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142,
		8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117,
		35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71,
		134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41,
		55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
		18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
		250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182,
		189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43,
		172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97,
		228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239,
		107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
		138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
	};

	private cRandom Random;

	public SimplexNoise ( ) {
		Random = new cRandom ( ( long ) Random.RandomInRange ( int.MinValue, int.MaxValue ) );
		SetSeed ( );
	}

	public SimplexNoise ( long p_seed ) {
		Random = new cRandom ( p_seed );
		SetSeed ( );
	}

	public float RawNoise2D ( float p_x, float p_y ) {
		float n0, n1, n2;

		float F2 = 0.5f * ( Mathf.Sqrt ( 3.0f ) - 1.0f );

		float s = ( p_x + p_y ) * F2;
		int i = FastFloor ( p_x + s );
		int j = FastFloor ( p_y + s );

		float G2 = ( 3.0f - Mathf.Sqrt ( 3.0f ) ) / 6.0f;
		float t = ( i + j ) * G2;

		float X0 = i - t;
		float Y0 = j - t;

		float x0 = p_x - X0;
		float y0 = p_y - Y0;

		int i1, j1;
		if ( x0 > y0 ) {
			i1 = 1;
			j1 = 0;
		} else {
			i1 = 0;
			j1 = 1;
		}

		float x1 = x0 - i1 + G2;
		float y1 = y0 - j1 + G2;
		float x2 = x0 - 1.0f + 2.0f * G2;
		float y2 = y0 - 1.0f + 2.0f * G2;

		int ii = i & 255;
		int jj = j & 255;
		int gi0 = perm [ ii + perm [ jj ] ] % 12;
		int gi1 = perm [ ii + i1 + perm [ jj + j1 ] ] % 12;
		int gi2 = perm [ ii + 1 + perm [ jj + 1 ] ] % 12;

		float t0 = 0.5f - x0 * x0 - y0 * y0;
		if ( t0 < 0 ) {
			n0 = 0.0f;
		} else {
			t0 *= t0;
			n0 = t0 * t0 * Dot ( grad3 [ gi0, 0 ], grad3 [ gi0, 1 ], x0, y0 );
		}

		float t1 = 0.5f - x1 * x1 - y1 * y1;
		if ( t1 < 0.0f ) {
			n1 = 0.0f;
		} else {
			t1 *= t1;
			n1 = t1 * t1 * Dot ( grad3 [ gi1, 0 ], grad3 [ gi1, 1 ], x1, y1 );
		}

		float t2 = 0.5f - x2 * x2 - y2 * y2;
		if ( t2 < 0.0f ) {
			n2 = 0.0f;
		} else {
			t2 *= t2;
			n2 = t2 * t2 * Dot ( grad3 [ gi2, 0 ], grad3 [ gi2, 1 ], x2, y2 );
		}

		return 70.0f * ( n0 + n1 + n2 );
	}

	public float OctaveNoise2D ( float p_octaves, float p_persistence, float p_scale, float p_x, float p_y ) {
		float total = 0;
		float frequency = p_scale;
		float amplitude = 1;
		float maxAmplitude = 0;
		for ( int i = 0; i < p_octaves; i++ ) {
			total += RawNoise2D ( p_x * frequency, p_y * frequency ) * amplitude;

			frequency *= 2;
			maxAmplitude += amplitude;
			amplitude *= p_persistence;
		}

		return total / maxAmplitude;
	}

	public float ScaledOctaveNoise2d ( float p_octaves, float p_persistence, float p_scale, float p_lowBound, float p_highBound, float p_x, float p_y ) {
		return OctaveNoise2D ( p_octaves, p_persistence, p_scale, p_x, p_y ) * ( p_highBound - p_lowBound ) / 2 + ( p_highBound + p_highBound ) / 2;
	}

	public void SetSeed ( long p_seed ) {
		Random.SetSeed ( p_seed );
		SetSeed ( );
	}

	private void SetSeed ( ) {
		for ( int i = 0; i < 256; i++ ) {
			perm [ i ] = i;
		}

		for ( int i = 0; i < 256; i++ ) {
			int k = perm [ i ];
			int j = Random.RandomInRange ( 0, 256 );

			perm [ i ] = perm [ j ];
			perm [ j ] = k;

			perm [ 256 + i ] = perm [ j ];
			perm [ 256 + j ] = k;
		}
	}

	private int FastFloor ( float p_value ) {
		return p_value > 0 ? ( int ) p_value : ( int ) p_value - 1;
	}

	private float Dot ( int p_a0, int p_a1, float p_x, float p_y ) {
		return p_a0 * p_x + p_a1 * p_y;
	}
}

