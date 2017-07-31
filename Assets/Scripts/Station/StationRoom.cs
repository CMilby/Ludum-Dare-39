using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum eRoomType {
	ROOM_HALLWAY,
	ROOM_ROOM,
	ROOM_INTERSECTION
}

public class StationRoom {

	public List<eOpenSides> OpenSides;
	public eRoomType RoomType;

	public int Width;
	public int Height;
	public int X;
	public int Y;

	private StationTile[,] Tiles;
	public List<Point2D> Doorways;

	public StationTile GetTileAt ( int p_x, int p_y ) { 
		return Tiles [ p_x, p_y ]; // There's math I could do to do this better
								   // But I don't feel like figuring it out rn
	}

	public bool RoomsOverlap ( StationRoom p_room, int p_padding ) {
		return ( X - p_padding < p_room.X + p_room.Width && X + Width + p_padding > p_room.X && Y - p_padding < p_room.Y + p_room.Height && Y + Height + p_padding > p_room.Y );
	}

	public bool ContainsPoint ( int p_x, int p_y ) {
		return ( p_x >= X && p_x <= X + Width && p_y >= Y && p_y <= Y + Height ); 
	}

	public int Area() {
		return Width * Height;
	}

	public static StationRoom GenerateRoom ( StationTileManager p_tileManager, int p_width, int p_height, params eOpenSides[] p_sides ) {
		StationRoom room = new StationRoom ( );
		room.Width = p_width;
		room.Height = p_height;
		room.OpenSides = p_sides.ToList<eOpenSides> ( );
		room.RoomType = eRoomType.ROOM_ROOM;

		room.Tiles = new StationTile[ p_width, p_height ];
		for ( int x = 0; x < p_width; x++ ) {
			for ( int y = 0; y < p_height; y++ ) {
				// Handle corners
				if ( x == 0 && y == 0 ) {
					room.Tiles [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_OUTER_CORNER, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];
					continue;
				} else if ( x == p_width - 1 && y == 0 ) {
					room.Tiles [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_OUTER_CORNER, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];
					continue;
				} else if ( x == 0 && y == p_height - 1 ) {
					room.Tiles [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_OUTER_CORNER, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
					continue;
				} else if ( x == p_width - 1 && y == p_height - 1 ) {
					room.Tiles [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_OUTER_CORNER, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];
					continue;
				}

				// Temporary
				room.Tiles [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];

				// Handle edges
				if ( x == 0 ) {
					continue;
				} else if ( y == 0 ) {
					continue;
				} else if ( x == p_width - 1 ) {
					continue;
				} else if ( y == p_height - 1 ) {
					continue;
				}

				// Handle fill
				// Do this later
			}
		}
		return room;
	}

	public static StationRoom GenerateHallwaysAndIntersections ( StationTileManager p_tileManager, eRoomType p_roomType, params eOpenSides[] p_sides ) {
		if ( p_sides.Contains ( eOpenSides.SIDE_NORTH ) && p_sides.Contains ( eOpenSides.SIDE_EAST ) && p_sides.Contains ( eOpenSides.SIDE_SOUTH ) && p_sides.Contains ( eOpenSides.SIDE_WEST ) ) { // All sides open
			if ( p_roomType == eRoomType.ROOM_INTERSECTION ) {
				return Generate4WayIntersection ( p_tileManager );
			}
		} else if ( p_sides.Contains ( eOpenSides.SIDE_NORTH ) && p_sides.Contains ( eOpenSides.SIDE_EAST ) && p_sides.Contains ( eOpenSides.SIDE_SOUTH ) ) { 
			if ( p_roomType == eRoomType.ROOM_INTERSECTION ) {
				return GenerateNESIntersection ( p_tileManager );
			}
		} else if ( p_sides.Contains ( eOpenSides.SIDE_EAST ) && p_sides.Contains ( eOpenSides.SIDE_SOUTH ) && p_sides.Contains ( eOpenSides.SIDE_WEST ) ) { 
			if ( p_roomType == eRoomType.ROOM_INTERSECTION ) {
				return GenerateESWIntersection ( p_tileManager );
			}
		} else if ( p_sides.Contains ( eOpenSides.SIDE_SOUTH ) && p_sides.Contains ( eOpenSides.SIDE_WEST ) && p_sides.Contains ( eOpenSides.SIDE_NORTH ) ) { 
			if ( p_roomType == eRoomType.ROOM_INTERSECTION ) {
				return GenerateSWNIntersection ( p_tileManager );
			}
		} else if ( p_sides.Contains ( eOpenSides.SIDE_WEST ) && p_sides.Contains ( eOpenSides.SIDE_NORTH ) && p_sides.Contains ( eOpenSides.SIDE_EAST ) ) { 
			if ( p_roomType == eRoomType.ROOM_INTERSECTION ) {
				return GenerateWNEIntersection ( p_tileManager );
			}
		} else if ( p_sides.Contains ( eOpenSides.SIDE_NORTH ) && p_sides.Contains ( eOpenSides.SIDE_SOUTH ) ) {
			if ( p_roomType == eRoomType.ROOM_HALLWAY ) {
				return GenerateNSHallway ( p_tileManager );
			}
		} else if ( p_sides.Contains ( eOpenSides.SIDE_EAST ) && p_sides.Contains ( eOpenSides.SIDE_WEST ) ) {
			if ( p_roomType == eRoomType.ROOM_HALLWAY ) {
				return GenerateEWHallway ( p_tileManager );
			}
		}

		return null;
	}

	private static StationRoom GenerateNESIntersection ( StationTileManager p_tileManager ) {
		StationRoom room = new StationRoom ( );
		room.Width = 2;
		room.Height = 2;
		room.OpenSides = new List<eOpenSides> ( ) { eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST };
		room.RoomType = eRoomType.ROOM_INTERSECTION;

		room.Tiles = new StationTile[ 2, 2 ];
		room.Tiles [ 0, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
		room.Tiles [ 0, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
		room.Tiles [ 1, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
		room.Tiles [ 1, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];

		return room;
	}

	private static StationRoom GenerateSWNIntersection ( StationTileManager p_tileManager ) {
		StationRoom room = new StationRoom ( );
		room.Width = 2;
		room.Height = 2;
		room.OpenSides = new List<eOpenSides> ( ) { eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH };
		room.RoomType = eRoomType.ROOM_INTERSECTION;

		room.Tiles = new StationTile[ 2, 2 ];
		room.Tiles [ 0, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];
		room.Tiles [ 0, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];
		room.Tiles [ 1, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];
		room.Tiles [ 1, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];

		return room;
	}

	private static StationRoom GenerateWNEIntersection ( StationTileManager p_tileManager ) {
		StationRoom room = new StationRoom ( );
		room.Width = 2;
		room.Height = 2;
		room.OpenSides = new List<eOpenSides> ( ) { eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST };
		room.RoomType = eRoomType.ROOM_INTERSECTION;

		room.Tiles = new StationTile[ 2, 2 ];
		room.Tiles [ 0, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];
		room.Tiles [ 1, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL,  eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];
		room.Tiles [ 0, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];
		room.Tiles [ 1, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];

		return room;
	}

	private static  StationRoom GenerateESWIntersection ( StationTileManager p_tileManager ) {
		StationRoom room = new StationRoom ( );
		room.Width = 2;
		room.Height = 2;
		room.OpenSides = new List<eOpenSides> ( ) { eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST };
		room.RoomType = eRoomType.ROOM_INTERSECTION;

		room.Tiles = new StationTile[ 2, 2 ];
		room.Tiles [ 0, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];
		room.Tiles [ 1, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
		room.Tiles [ 0, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];
		room.Tiles [ 1, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];

		return room;
	}

	private static StationRoom GenerateNSHallway ( StationTileManager p_tileManager ) {
		StationRoom room = new StationRoom ( );
		room.Width = 2;
		room.Height = 2;
		room.OpenSides = new List<eOpenSides> ( ) { eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_NORTH };
		room.RoomType = eRoomType.ROOM_HALLWAY;

		room.Tiles = new StationTile[ 2, 2 ];
		room.Tiles [ 0, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
		room.Tiles [ 0, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
		room.Tiles [ 1, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];
		room.Tiles [ 1, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];

		return room;
	}

	private static StationRoom GenerateEWHallway ( StationTileManager p_tileManager ) {
		StationRoom room = new StationRoom ( );
		room.Width = 2;
		room.Height = 2;
		room.OpenSides = new List<eOpenSides> ( ) {eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST };
		room.RoomType = eRoomType.ROOM_HALLWAY;

		room.Tiles = new StationTile[ 2, 2 ];
		room.Tiles [ 0, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];
		room.Tiles [ 1, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];
		room.Tiles [ 0, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];
		room.Tiles [ 1, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];

		return room;
	}

	private static StationRoom Generate4WayIntersection( StationTileManager p_tileManager ) {
		StationRoom room = new StationRoom ( );
		room.Width = 2;
		room.Height = 2;
		room.OpenSides = new List<eOpenSides> ( ) { eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST };
		room.RoomType = eRoomType.ROOM_INTERSECTION;

		room.Tiles = new StationTile[ 2, 2 ];
		room.Tiles [ 0, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];
		room.Tiles [ 1, 0 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
		room.Tiles [ 0, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];
		room.Tiles [ 1, 1 ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];

		return room;
	}
}

