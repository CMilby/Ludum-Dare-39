using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class StationRoomManager : MonoBehaviour {

	public List<StationRoom> Rooms;

	public List<StationRoom> GetRoomFromTypeAndOrientation ( eRoomType p_type, params eOpenSides[] p_sides ) {
		List<StationRoom> myRooms = Rooms.Where ( x => x.RoomType == p_type ).ToList<StationRoom>();
		return myRooms.Where ( x => !x.OpenSides.Except ( p_sides.ToList<eOpenSides>() ).Any() ).ToList<StationRoom>();
	}

	public void AddRoom ( int p_x, int p_y, StationRoom p_room, ref StationTile[,] p_tiles ) {
		int width = p_room.Width;
		int height = p_room.Height;

		for ( int i = p_x; i < p_x + width; ++i ) {
			for ( int j = p_y; j < p_y + height; ++j ) {
				p_tiles [ i, j ] = p_room.GetTileAt( i - p_x, j - p_y );
			}
		}
	}
}

