using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum eOpenSides {
	SIDE_NORTH,
	SIDE_SOUTH,
	SIDE_EAST,
	SIDE_WEST
}

public enum eTileType {
	TILE_NONE,
	TILE_BASIC_FLOOR,
	TILE_BASIC_FLOOR_WALL,
	TILE_BASIC_FLOOR_HALLWAY,
	TILE_BASIC_FLOOR_DOOR,
	TILE_BASIC_FLOOR_INNER_CORNER,
	TILE_BASIC_FLOOR_OUTER_CORNER,
	TILE_SOLAR_PANEL,
	TILE_BASIC_FLOOR_WINDOW
}

public class StationTile : MonoBehaviour, IComparable {

	public List<eOpenSides> OpenSides;
	public eTileType Type;

	public int X;
	public int Y;
	public float Rotation;

	public StationTile ( ) {
		OpenSides = new List<eOpenSides> ( );
		X = Y = 0;
	}

	public int CompareTo ( object p_obj ) {
		if ( p_obj == null ) {
			return 1;
		}

		StationTile tile = p_obj as StationTile;
		if ( tile == null ) {
			throw new ArgumentException ( "Error: Object not StationTile object" );
		}

		/*int compX = X.CompareTo ( tile.X );
		if ( compX != 0 ) {
			return compX;
		}

		return Y.CompareTo ( Y );*/

		Vector3 pos = transform.position;
		Vector3 otherPos = tile.transform.position;
		int compX = pos.x.CompareTo ( otherPos.x );
		if ( compX != 0 ) {
			return compX;
		}

		return pos.y.CompareTo ( otherPos.y );
	}
}
