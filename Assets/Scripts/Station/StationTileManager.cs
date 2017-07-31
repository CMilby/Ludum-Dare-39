using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StationTileManager : MonoBehaviour {

	public List<StationTile> Tiles;

	public StationTile GetStationTileOfType ( eTileType p_type ) {
		return Tiles.FirstOrDefault ( x => x.Type == p_type );
	}
 
	public List<StationTile> GetTileOfTypeWithOpenings ( eTileType p_type, params eOpenSides[] p_sides ) {
		List<StationTile> myTiles = Tiles.Where ( x => x.Type == p_type ).ToList<StationTile>();
		return myTiles.Where ( x => !x.OpenSides.Except ( p_sides.ToList<eOpenSides>() ).Any() ).ToList<StationTile>();
	}
}

