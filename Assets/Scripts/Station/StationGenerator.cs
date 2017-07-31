using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class StationGenerator : MonoBehaviour {

	private enum eDirection {
		DIR_N,
		DIR_S,
		DIR_E,
		DIR_W
	}

	private cRandom Random;
	private long Seed = 1234;
	private int MaxDepth = 7;

	private List<StationRoom> Rooms;

	public  void Init ( long p_seed, int p_maxDepth ) {
		Seed = p_seed;
		MaxDepth = p_maxDepth;
	}

	public List<StationRoom> Generate ( int p_width, int p_height, eStationSize p_size, StationTileManager p_tileManager, StationRoomManager p_roomManager, StationDecoratorManager p_decorationManager, ref StationTile[,] p_station, ref List<GameObject> p_triggers, ref List<DecWrapper> p_decorations ) {
		p_station = new StationTile[ p_width, p_height ];
		p_triggers = new List<GameObject> ( );
		p_decorations = new List<DecWrapper> ( );

		Random = new cRandom ( Seed );
		Rooms = new List<StationRoom> ( );

		do {
			for ( int i = 0; i < p_width; i++ ) {
				for ( int j = 0; j < p_height; j++ ) {
					p_station[ i, j ] = null;
				}
			}

			Rooms.Clear();
			Random.SetSeed ( Seed );
			Seed++;

			// Start end hallway in center of room
			// p_roomManager.AddRoom ( 100, p_height / 2, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST ), ref p_station );
			// p_roomManager.AddRoom ( 102, p_height / 2, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST ), ref p_station );
			// p_roomManager.AddRoom ( 104, p_height / 2, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST ), ref p_station );
			p_roomManager.AddRoom ( 106, p_height / 2 - 1, StationRoom.GenerateRoom ( p_tileManager, 6, 4 ), ref p_station );
			// p_roomManager.AddRoom ( 112, p_height / 2, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST ), ref p_station );
			// p_roomManager.AddRoom ( 114, p_height / 2, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST ), ref p_station );
			// p_roomManager.AddRoom ( 116, p_height / 2, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_WEST, eOpenSides.SIDE_EAST ), ref p_station );

			// Now we need to generate some random stuff...
			GenerateRandomRoom ( eDirection.DIR_N, 108, p_height / 2 + 3, 1, eDirection.DIR_N, p_tileManager, p_roomManager, ref p_station, ref p_triggers );
			GenerateRandomRoom ( eDirection.DIR_S, 108, p_height / 2 - 1, 1, eDirection.DIR_S, p_tileManager, p_roomManager, ref p_station, ref p_triggers );
			EntranceColliders ( p_height, ref p_triggers );
			TryConnectRooms ( 108, p_tileManager, p_roomManager, ref p_station, ref p_triggers );
			BuildWalls ( p_width, p_height, p_tileManager, ref p_station );

			RoomFinder ( p_width, p_height, p_tileManager, ref p_station );
			OutsideDecorator( 108, p_size, p_tileManager, ref p_station );
			InteriorDecorator( p_tileManager, p_decorationManager, ref p_decorations );
			FindHallways ( p_width, p_height, p_tileManager, ref p_station );
		} while ( Rooms.Count < 5 );
		return Rooms;
	}

	private void FindHallways( int p_width, int p_height, StationTileManager p_tileManager, ref StationTile[,] p_station ) {
		float doorChance = 0.1f;
		for ( int x = 0; x < p_width; x++ ) {
			for ( int y = 0; y < p_height; y++ ) {
				StationTile tile = p_station [ x, y ];
				if ( tile == null ) {
					continue;
				}

				if ( tile.Type != eTileType.TILE_BASIC_FLOOR_WALL ) {
					continue;
				}

				bool isRoom = false;
				for ( int i = 0; i < Rooms.Count; i++ ) {
					if ( Rooms [ i ].ContainsPoint ( x, y ) ) {
						isRoom = true;
						break;
					}
				}

				if ( isRoom ) {
					continue;
				}

				if ( Random.RandomFloat() < doorChance ) {
					if ( tile.OpenSides.Contains ( eOpenSides.SIDE_NORTH ) && tile.OpenSides.Contains ( eOpenSides.SIDE_EAST ) && tile.OpenSides.Contains ( eOpenSides.SIDE_SOUTH ) ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_DOOR, eOpenSides.SIDE_WEST )[ 0 ];
					} else if ( tile.OpenSides.Contains ( eOpenSides.SIDE_EAST ) && tile.OpenSides.Contains ( eOpenSides.SIDE_SOUTH ) && tile.OpenSides.Contains ( eOpenSides.SIDE_WEST ) ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_DOOR, eOpenSides.SIDE_NORTH )[ 0 ];
					} else if ( tile.OpenSides.Contains ( eOpenSides.SIDE_SOUTH ) && tile.OpenSides.Contains ( eOpenSides.SIDE_WEST ) && tile.OpenSides.Contains ( eOpenSides.SIDE_NORTH ) ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_DOOR, eOpenSides.SIDE_EAST )[ 0 ];
					} else if ( tile.OpenSides.Contains ( eOpenSides.SIDE_WEST ) && tile.OpenSides.Contains ( eOpenSides.SIDE_NORTH ) && tile.OpenSides.Contains ( eOpenSides.SIDE_EAST ) ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_DOOR, eOpenSides.SIDE_SOUTH )[ 0 ];
					}
				}
			}
		}
	}

	private void InteriorDecorator ( StationTileManager p_tileManager, StationDecoratorManager p_decorationManager, ref List<DecWrapper> p_decorations ) {
		float batterySpawnChance = 0.35f;
		float objectSpawnChance = 0.25f;
		foreach ( StationRoom room in Rooms ) {
			for ( int x = room.X; x <= room.X + room.Width; x++ ) {
				for ( int y = room.Y; y <= room.Y + room.Height; y++ ) {
					bool isDoorway = false;
					for ( int i = 0; i < room.Doorways.Count; i++ ) {
						if ( room.Doorways [ i ].Equals ( x, y ) ) {
							isDoorway = true;
							break;
						}
					}

					if ( isDoorway ) {
						continue;
					}

					// Not doorways so let's add some stuff
					if ( Random.RandomFloat ( ) < batterySpawnChance && IsLeftWall ( room, x ) ) {
						p_decorations.Add ( new DecWrapper ( ) {
							DecPrefab = p_decorationManager.GetDecorationOfType ( eDecorationType.DECORATION_BATTERY_W ),
							X = x,
							Y = y
						} );
						continue;
					} 

					if ( Random.RandomFloat() < batterySpawnChance && IsRightWall ( room, x ) ) {
						p_decorations.Add ( new DecWrapper ( ) {
							DecPrefab = p_decorationManager.GetDecorationOfType ( eDecorationType.DECORATION_BATTERY_E ),
							X = x,
							Y = y
						} );
						continue;
					}

					if ( Random.RandomFloat ( ) < objectSpawnChance ) {
						float randChance = Random.RandomFloat ( );
						if ( randChance < 0.4f ) {
							p_decorations.Add ( new DecWrapper ( ) {
								DecPrefab = p_decorationManager.GetDecorationOfType ( eDecorationType.DECORATION_SHELF ),
								X = x,
								Y = y
							} );
						} else if ( randChance < 0.5f ) {
							p_decorations.Add ( new DecWrapper ( ) {
								DecPrefab = p_decorationManager.GetDecorationOfType ( eDecorationType.DECORATION_BED ),
								X = x,
								Y = y
							} );
						} else {
							p_decorations.Add ( new DecWrapper ( ) {
								DecPrefab = p_decorationManager.GetDecorationOfType ( eDecorationType.DECORATION_TABLE ),
								X = x,
								Y = y
							} );
						}
					}
				}
			}

			p_decorations.Add ( new DecWrapper ( ) {
				DecPrefab = p_decorationManager.GetDecorationOfType ( eDecorationType.DECORATION_LIGHT ),
				X = room.X + ( room.Width / 2 ),
				Y = room.Y + ( room.Height / 2 )
			} );
		}
	}

	private bool IsWall ( StationRoom p_room, int p_x, int p_y ) {
		return ( p_x == p_room.X || p_y == p_room.Y || p_x == p_room.X + p_room.Width || p_y == p_room.Y + p_room.Height );
	}

	private bool IsLeftRightWall ( StationRoom p_room, int p_x, int p_y ) {
		return ( p_x == p_room.X || p_x == p_room.X + p_room.Width );
	}

	private bool IsLeftWall ( StationRoom p_room, int p_x ) {
		return ( p_x == p_room.X );
	}

	private bool IsRightWall ( StationRoom p_room, int p_x ) {
		return ( p_x == p_room.X + p_room.Width );
	}

	private bool IsTopWall ( StationRoom p_room, int p_y ) {
		return p_y == p_room.Y + p_room.Height;
	}

	private void RoomFinder ( int p_width, int p_height, StationTileManager p_tileManager, ref StationTile[,] p_station ) {
		Rooms.Clear ( );
		for ( int x = 0; x < p_width; x++ ) {
			for ( int y = 0; y < p_height; y++ ) {
				StationTile tile = p_station [ x, y ];
				if ( tile == null ) {
					continue;
				}

				bool foundRoomBLCorner = false;
				if ( tile.Type == eTileType.TILE_BASIC_FLOOR_OUTER_CORNER ) {
					if ( tile.OpenSides.Contains ( eOpenSides.SIDE_NORTH ) && tile.OpenSides.Contains ( eOpenSides.SIDE_EAST ) ) {
						// p_station [ x, y ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_HALLWAY );
						foundRoomBLCorner = true;
					}
				} else if ( tile.Type == eTileType.TILE_BASIC_FLOOR_WALL &&
				            p_station [ x, y + 1 ] != null &&
				            p_station [ x, y + 1 ].Type == eTileType.TILE_BASIC_FLOOR_INNER_CORNER &&
				            p_station [ x, y + 2 ] != null &&
				            ( p_station [ x, y + 2 ].Type == eTileType.TILE_BASIC_FLOOR_WALL ||
				            p_station [ x, y + 2 ].Type == eTileType.TILE_BASIC_FLOOR_INNER_CORNER ) ) {
					if ( tile.OpenSides.Contains ( eOpenSides.SIDE_NORTH ) && tile.OpenSides.Contains ( eOpenSides.SIDE_EAST ) && tile.OpenSides.Contains ( eOpenSides.SIDE_SOUTH ) ) {
						// p_station [ x, y ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_HALLWAY );
						foundRoomBLCorner = true;
					}
				} else if ( tile.Type == eTileType.TILE_BASIC_FLOOR_WALL &&
				            p_station [ x + 1, y ] != null &&
				            p_station [ x + 1, y ].Type == eTileType.TILE_BASIC_FLOOR_INNER_CORNER ) {
					if ( tile.OpenSides.Contains ( eOpenSides.SIDE_NORTH ) && tile.OpenSides.Contains ( eOpenSides.SIDE_EAST ) && tile.OpenSides.Contains ( eOpenSides.SIDE_SOUTH ) ) {
						// p_station [ x, y ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_HALLWAY );
						foundRoomBLCorner = true;
					}
				}

				if ( !foundRoomBLCorner ) {
					continue;
				}

				int myURX = -1;
				int myURY = -1;
				for ( int i = 0; i < 10; i++ ) {
					for ( int j = 0; j < 8; j++ ) {
						StationTile myTile = p_station [ x + i, y + j ];
						if ( myTile == null ) {
							continue;
						}

						if ( myTile.Type == eTileType.TILE_BASIC_FLOOR_OUTER_CORNER ) {
							if ( myTile.OpenSides.Contains ( eOpenSides.SIDE_SOUTH ) && myTile.OpenSides.Contains ( eOpenSides.SIDE_WEST ) ) {
								myURX = x + i;
								myURY = y + j;
								// p_station [ myURX, myURY ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_HALLWAY );
							}
						}
					}
				}

				if ( myURX == -1 || myURY == -1 ) {
					continue;
				}

				// p_station [ x, y ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_HALLWAY );
				// p_station [ myURX, myURY ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_HALLWAY );

				StationRoom room = new StationRoom ( );
				room.X = x;
				room.Y = y;
				room.Width = ( myURX - x );
				room.Height = ( myURY - y );
				room.RoomType = eRoomType.ROOM_ROOM;
				room.Doorways = new List<Point2D> ( );

				// Bottom and top row
				for ( int i = 0; i <= room.Width; i++ ) {
					if ( p_station [ x + i, y ].Type == eTileType.TILE_BASIC_FLOOR_INNER_CORNER ) {
						room.Doorways.Add ( new Point2D ( x + i, y ) );
					}

					if ( p_station [ x + i, y ].Type == eTileType.TILE_BASIC_FLOOR_WALL && p_station [ x + i, y - 1 ] != null && p_station [ x + i, y - 1 ].Type == eTileType.TILE_BASIC_FLOOR_WALL ) {
						room.Doorways.Add ( new Point2D ( x + i, y ) );
					}

					if ( p_station [ x + i, y + room.Height ].Type == eTileType.TILE_BASIC_FLOOR_INNER_CORNER ) {
						room.Doorways.Add ( new Point2D ( x + i, y + room.Height) );
					}

					if ( p_station [ x + i, y + room.Height ].Type == eTileType.TILE_BASIC_FLOOR_WALL && p_station [ x + i, y + room.Height + 1 ] != null && p_station [ x + i, y + room.Height + 1 ].Type == eTileType.TILE_BASIC_FLOOR_WALL ) {
						room.Doorways.Add ( new Point2D ( x + i, y + room.Height ) );
					}
				}

				// Left and right column
				for ( int i = 0; i <= room.Height; i++ ) {
					if ( p_station [ x, y + i ].Type == eTileType.TILE_BASIC_FLOOR_INNER_CORNER ) {
						room.Doorways.Add ( new Point2D ( x, y + i ) );
					}

					if ( p_station [ x, y + i ].Type == eTileType.TILE_BASIC_FLOOR_WALL && p_station [ x - 1, y + i ] != null && p_station[ x - 1 , y + i ].Type == eTileType.TILE_BASIC_FLOOR_WALL ) {
						room.Doorways.Add ( new Point2D ( x, y + i ) );
					}

					if ( p_station [ x + room.Width, y + i ].Type == eTileType.TILE_BASIC_FLOOR_INNER_CORNER ) {
						room.Doorways.Add ( new Point2D ( x + room.Width, y + i ) );
					}

					if ( p_station [ x + room.Width, y + i ].Type == eTileType.TILE_BASIC_FLOOR_WALL && p_station [ x + room.Width - 1, y + i ] != null && p_station[ x + room.Width - 1 , y + i ].Type == eTileType.TILE_BASIC_FLOOR_WALL ) {
						room.Doorways.Add ( new Point2D ( x + room.Width, y + i ) );
					}
				}

				/*for ( int i = 0; i < room.Doorways.Count; i++ ) {
					p_station [ room.Doorways [ i ].X, room.Doorways [ i ].Y ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_HALLWAY );
				}*/

				Rooms.Add ( room );
			}
		}

		// Remove overlapping rooms
		for ( int i = 0; i < Rooms.Count; i++ ) {
			for ( int j = i + 1; j < Rooms.Count; j++ ) {
				if ( Rooms [ i ].RoomsOverlap ( Rooms [ j ], 0 ) ) {
					int a = Rooms [ i ].Area ( );
					int b = Rooms [ j ].Area ( );

					if ( a < b ) {
						Rooms.RemoveAt ( i );
					} else {
						Rooms.RemoveAt ( j );
					}
				}
			}
		}
	}

	private void OutsideDecorator( int p_middle, eStationSize p_size, StationTileManager p_tileManager, ref StationTile[,] p_station ) {
		foreach ( StationRoom room in Rooms ) {
			if ( room.Width % 2 == 1 && room.Height % 2 == 1 ) {
				int x = room.X + ( room.Width / 2 );
				int y = room.Y + ( room.Height / 2 );

				p_station [ x, y ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_WINDOW );
				p_station [ x + 1, y ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_WINDOW );
				p_station [ x, y + 1 ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_WINDOW );
				p_station [ x + 1, y + 1 ] = p_tileManager.GetStationTileOfType ( eTileType.TILE_BASIC_FLOOR_WINDOW );
			}
				
			int panelsAdded = 0;

			int maxPanelChance = 6;
			if ( p_size == eStationSize.STATION_SMALL ) {
				maxPanelChance = 10;
			} else if ( p_size == eStationSize.STATION_MEDIUM ) {
				maxPanelChance = 8;
			} else if ( p_size == eStationSize.STATION_LARGE ) {
				maxPanelChance = 6;
			}

			int numPanels = Random.RandomInRange ( 0, maxPanelChance );
			for ( int i = 0; i < numPanels || panelsAdded < 1; i++ ) {
				float side = Random.RandomFloat ( );
	
				if ( side < 0.25f ) {
					int x = room.X - 1;
					int y = Random.RandomInRange ( room.Y, room.Y + room.Height );

					if ( p_station [ x, y ] != null ) {
						continue;
					}

					if ( p_station [ x + 1, y ] != null ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_SOLAR_PANEL, eOpenSides.SIDE_WEST )[ 0 ];
						panelsAdded++;
					}
				} else if ( side < 0.5f ) {
					int x = room.X + room.Width;
					int y = Random.RandomInRange ( room.Y, room.Y + room.Height );

					if ( p_station [ x, y ] != null ) {
						continue;
					}

					if ( p_station [ x - 1, y ] != null ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_SOLAR_PANEL, eOpenSides.SIDE_EAST )[ 0 ];
						panelsAdded++;
					}
				} else if ( side < 0.75f ) {
					int x = Random.RandomInRange ( room.X, room.X + room.Width );
					int y = room.Y - 1;

					if ( p_station [ x, y ] != null ) {
						continue;
					}

					if ( p_station [ x, y + 1 ] != null ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_SOLAR_PANEL, eOpenSides.SIDE_SOUTH )[ 0 ];
						panelsAdded++;
					}
				} else {
					int x = Random.RandomInRange ( room.X, room.X + room.Width );
					int y = room.Y + room.Height;

					if ( p_station [ x, y ] != null ) {
						continue;
					}

					if ( p_station [ x, y - 1 ] != null ) {
						p_station [ x, y ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_SOLAR_PANEL, eOpenSides.SIDE_NORTH )[ 0 ];
						panelsAdded++;
					}
				}
			}
		}
	}

	private void EntranceColliders ( int p_height, ref List<GameObject> p_colliders ) {
		/*GameObject go = new GameObject ( "Trigger_" + 100 + "_" + p_height / 2 );
		BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
		collider.gameObject.layer = 9;
		collider.isTrigger = true;
		collider.offset = new Vector2 ( 103 * 1.28f - ( 1.28f / 2.0f ), ( p_height / 2.0f + 1 ) * 1.28f - ( 1.28f / 2.0f ) );
		collider.size = new Vector2 ( 6 * 1.28f, 2 * 1.28f );
		p_colliders.Add ( go );*/

		GameObject go = new GameObject ( "Trigger_" + 106 + "_" + p_height / 2 );
		BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
		collider.gameObject.layer = 9;
		collider.isTrigger = true;
		collider.offset = new Vector2 ( 109 * 1.28f - ( 1.28f / 2.0f ), ( p_height / 2.0f + 1 ) * 1.28f - ( 1.28f / 2.0f ) );
		collider.size = new Vector2 ( 6 * 1.28f, 4 * 1.28f );
		p_colliders.Add ( go );

		/*go = new GameObject ( "Trigger_" + 112 + "_" + p_height / 2 );
		collider = go.AddComponent<BoxCollider2D> ( );
		collider.gameObject.layer = 9;
		collider.isTrigger = true;
		collider.offset = new Vector2 ( 115 * 1.28f - ( 1.28f / 2.0f ), ( p_height / 2.0f + 1 ) * 1.28f - ( 1.28f / 2.0f ) );
		collider.size = new Vector2 ( 6 * 1.28f, 2 * 1.28f );
		p_colliders.Add ( go );*/
	}

	private void BuildWalls( int p_width, int p_height, StationTileManager p_tileManager, ref StationTile[,] p_station ) {
		for ( int i = 1; i < p_width - 1; i++ ) {
			for ( int j = 1; j < p_height - 1; j++ ) {
				if ( p_station [ i, j ] == null ) {
					continue;
				}
				      // EAST							// NORTH							// WEST							// SOUTH
				if ( p_station [ i + 1, j ] != null && p_station [ i, j + 1 ] != null && p_station [ i - 1, j ] != null && p_station [ i, j - 1 ] != null ) {
					// EITHER MIDDLE TILE OR INSIDE CORNER
					if ( p_station [ i + 1, j + 1 ] == null ) {
						p_station [ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST ) [ 0 ];
					} else if ( p_station [ i + 1, j - 1 ] == null ) {
						p_station [ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_EAST ) [ 0 ];
					} else if ( p_station [ i - 1, j + 1 ] == null ) {
						p_station [ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_WEST ) [ 0 ];
					} else if ( p_station [ i - 1, j - 1 ] == null ) {
						p_station [ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_INNER_CORNER, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST ) [ 0 ];
					} else {
						p_station [ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST ) [ 0 ];
					}
				} else if ( p_station [ i + 1, j ] != null && p_station [ i, j + 1 ] != null && p_station [ i - 1, j ] != null ) { // SOUTH NULL
					p_station[ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST )[ 0 ];
				} else if ( p_station [ i + 1, j ] != null && p_station [ i, j + 1 ] != null && p_station [ i, j - 1 ] != null ) { // WEST NULL
					p_station[ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH )[ 0 ];
				} else if ( p_station [ i + 1, j ] != null && p_station [ i - 1, j ] != null && p_station [ i, j - 1 ] != null ) { // NORTH NULL
					p_station[ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_EAST, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST )[ 0 ];
				} else if ( p_station [ i, j + 1 ] != null && p_station [ i - 1, j ] != null && p_station [ i, j - 1 ] != null ) { // EAST NULL
					p_station[ i, j ] = p_tileManager.GetTileOfTypeWithOpenings ( eTileType.TILE_BASIC_FLOOR_WALL, eOpenSides.SIDE_SOUTH, eOpenSides.SIDE_WEST, eOpenSides.SIDE_NORTH )[ 0 ];
				}
			}
		}
	}

	private void TryConnectRooms ( int p_middle, StationTileManager p_tileManager, StationRoomManager p_roomManager, ref StationTile[,] p_station, ref List<GameObject> p_colliders ) {
		List<StationRoom> left = Rooms.Where ( x => x.X < p_middle && x.RoomType == eRoomType.ROOM_ROOM ).ToList<StationRoom> ( );
		List<StationRoom> right = Rooms.Where ( x => x.X >= p_middle && x.RoomType == eRoomType.ROOM_ROOM ).ToList<StationRoom> ( );

		left.Sort ( ( r1, r2 ) => r1.Y.CompareTo ( r2.Y ) );
		right.Sort ( ( r1, r2 ) => r1.Y.CompareTo ( r2.Y ) );

		TryConnectLeftSide ( left, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
		TryConnectRightSide ( right, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
	}

	private void TryConnectRightSide ( List<StationRoom> p_rooms, StationTileManager p_tileManager, StationRoomManager p_roomManager, ref StationTile[,] p_station, ref List<GameObject> p_colliders ) {
		for ( int i = 1; i < p_rooms.Count; i++ ) {
			StationRoom a = p_rooms [ i - 1 ];
			StationRoom b = p_rooms [ i ];

			// 1 is for padding
			if ( a.X - 1 < b.X + b.Width && a.X + a.Width + 1 > b.X ) {
				int dist = Math.Abs ( a.Y - b.Y );
				if ( dist > 15 || Random.RandomFloat() < 0.5f ) {
					continue;
				}

				GenerateNSHallway ( Math.Max ( a.X, b.X ), Math.Min ( a.Y + a.Height, b.Y + b.Height ), Math.Max ( a.Y, b.Y ) , p_tileManager, p_roomManager, ref p_station );
				Rooms.Add ( new StationRoom() { X = Math.Max ( a.X, b.Y ), Y = Math.Min ( a.Y + a.Height, b.Y + b.Height ), Width = 2, Height = Math.Max ( a.Y, b.Y ) - Math.Min ( a.Y + a.Height, b.Y + b.Height ), RoomType  = eRoomType.ROOM_HALLWAY, OpenSides = new List<eOpenSides>() { eOpenSides.SIDE_NORTH, eOpenSides.SIDE_SOUTH } } );

				GameObject go = new GameObject ( "Trigger_" + Math.Max ( a.X, b.X ) + "_" + Math.Min ( a.Y + a.Height, b.Y + b.Height ) );
				BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
				collider.gameObject.layer = 9;
				collider.isTrigger = true;
				collider.offset = new Vector2 ( ( Math.Max ( a.X, b.X ) + 1 ) * 1.28f - ( 1.28f / 2.0f ), ( Math.Min ( a.Y + a.Height, b.Y + b.Height ) + ( ( Math.Max ( a.Y, b.Y ) - Math.Min ( a.Y + a.Height, b.Y + b.Height ) ) / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) );
				collider.size = new Vector2 ( 2 * 1.28f, ( Math.Max ( a.Y, b.Y ) - Math.Min ( a.Y + a.Height, b.Y + b.Height ) ) * 1.28f );
				p_colliders.Add ( go );

				if ( a.X < b.X ) {
					p_rooms [ i - 1 ].OpenSides.Add ( eOpenSides.SIDE_NORTH );
					p_rooms [ i ].OpenSides.Add ( eOpenSides.SIDE_SOUTH );
				} else {
					p_rooms [ i - 1 ].OpenSides.Add ( eOpenSides.SIDE_SOUTH );
					p_rooms [ i ].OpenSides.Add ( eOpenSides.SIDE_NORTH );
				}
			}
		}
	}

	private void TryConnectLeftSide ( List<StationRoom> p_rooms, StationTileManager p_tileManager, StationRoomManager p_roomManager, ref StationTile[,] p_station, ref List<GameObject> p_colliders ) {
		for ( int i = 1; i < p_rooms.Count; i++ ) {
			StationRoom a = p_rooms [ i - 1 ];
			StationRoom b = p_rooms [ i ];

			// 1 is for padding
			if ( a.X - 1 < b.X + b.Width && a.X + a.Width + 1 > b.X ) {
				int dist = Math.Abs ( a.Y - b.Y );
				if ( dist > 15 || Random.RandomFloat() < 0.5f ) {
					continue;
				}

				GenerateNSHallway ( Math.Max ( a.X, b.X ), Math.Min ( a.Y + a.Height, b.Y + b.Height ), Math.Max ( a.Y, b.Y ) , p_tileManager, p_roomManager, ref p_station );
				Rooms.Add ( new StationRoom() { X = Math.Max ( a.X, b.Y ), Y = Math.Min ( a.Y + a.Height, b.Y + b.Height ), Width = 2, Height = Math.Max ( a.Y, b.Y ) - Math.Min ( a.Y + a.Height, b.Y + b.Height ), RoomType  = eRoomType.ROOM_HALLWAY, OpenSides = new List<eOpenSides>() { eOpenSides.SIDE_NORTH, eOpenSides.SIDE_SOUTH } } );

				GameObject go = new GameObject ( "Trigger_" + Math.Max ( a.X, b.X ) + "_" + Math.Min ( a.Y + a.Height, b.Y + b.Height ) );
				BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
				collider.gameObject.layer = 9;
				collider.isTrigger = true;
				collider.offset = new Vector2 ( ( Math.Max ( a.X, b.X ) + 1 ) * 1.28f - ( 1.28f / 2.0f ), ( Math.Min ( a.Y + a.Height, b.Y + b.Height ) + ( ( Math.Max ( a.Y, b.Y ) - Math.Min ( a.Y + a.Height, b.Y + b.Height ) ) / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) );
				collider.size = new Vector2 ( 2 * 1.28f, ( Math.Max ( a.Y, b.Y ) - Math.Min ( a.Y + a.Height, b.Y + b.Height ) ) * 1.28f );
				p_colliders.Add ( go );

				if ( a.X < b.X ) {
					p_rooms [ i - 1 ].OpenSides.Add ( eOpenSides.SIDE_NORTH );
					p_rooms [ i ].OpenSides.Add ( eOpenSides.SIDE_SOUTH );
				} else {
					p_rooms [ i - 1 ].OpenSides.Add ( eOpenSides.SIDE_SOUTH );
					p_rooms [ i ].OpenSides.Add ( eOpenSides.SIDE_NORTH );
				}
			}
		}
	}

	private void GenerateRandomRoom ( eDirection p_initial, int p_lastX, int p_lastY, int p_depth, eDirection p_comeFromDir, StationTileManager p_tileManager, StationRoomManager p_roomManager, ref StationTile[,] p_station, ref List<GameObject> p_colliders ) {
		// float randomRoomType = Random.RandomFloat ( );

		int minOffset = 6;
		int maxOffset = 6;
		int roomMin = 4;
		int roomMax = 8;

		int width = ForceEven ( Random.RandomInRange ( roomMin, roomMax ) );
		int height = ForceEven ( Random.RandomInRange ( roomMin, roomMax ) );

		if ( p_comeFromDir == eDirection.DIR_E ) {
			/*int y;
			if ( p_initial == eDirection.DIR_N ) {
				y = p_lastY - ( height / 4 ) - 1;
			} else {
				y = ForceEven( p_lastY - ( height / 4 ) ) - 1;
			}*/

			int y = p_lastY - ( height / 4 );
			if ( y % 2 == 1 ) {
				y--;
			}

			// int y = ForceEven( p_lastY - ( height / 4 ) );
			// int y = p_lastY - ( height / 4 );
			int x = ForceEven( p_lastX + Random.RandomInRange ( minOffset, maxOffset ) );

			GenerateEWHallway ( p_lastX, x, p_lastY, p_tileManager, p_roomManager, ref p_station );
			p_roomManager.AddRoom ( x, y, StationRoom.GenerateRoom ( p_tileManager, width, height ), ref p_station );
			Rooms.Add ( new StationRoom ( ) { RoomType = eRoomType.ROOM_ROOM, X = x, Y = y, Width = width, Height = height, OpenSides = new List<eOpenSides>() { eOpenSides.SIDE_WEST } } );
			Rooms.Add ( new StationRoom ( ) { RoomType = eRoomType.ROOM_HALLWAY, X = x, Y = p_lastY, Width = ( x - p_lastX ), Height = 2, OpenSides = new List<eOpenSides>() { eOpenSides.SIDE_EAST, eOpenSides.SIDE_WEST } } );
		
			GameObject go = new GameObject ( "Trigger_" + x + "_" + y );
			BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( x + ( width / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ), ( y + ( height / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( width * 1.28f, height * 1.28f );
			p_colliders.Add ( go );

			go = new GameObject ( "Trigger_" + x + "_" + y );
			collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( p_lastX + ( ( x - p_lastX ) / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ), ( p_lastY + 1 ) * 1.28f - ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( ( x - p_lastX ) * 1.28f, 2 * 1.28f );
			p_colliders.Add ( go );
		} else if ( p_comeFromDir == eDirection.DIR_W ) {
			int y = ForceEven ( p_lastY - ( height / 4 ) );
			int xOff = ForceEven( Random.RandomInRange ( minOffset, maxOffset ) );
			int x = ForceEven( p_lastX - xOff );
			width += 2;

			GenerateEWHallway ( x, p_lastX, p_lastY, p_tileManager, p_roomManager, ref p_station );
			p_roomManager.AddRoom ( x - xOff, y, StationRoom.GenerateRoom ( p_tileManager, width, height ), ref p_station );
			Rooms.Add ( new StationRoom ( ) { RoomType = eRoomType.ROOM_ROOM, X = x - xOff, Y = y, Width = width, Height = height, OpenSides = new List<eOpenSides>() { eOpenSides.SIDE_EAST } } );
			Rooms.Add ( new StationRoom ( ) { RoomType = eRoomType.ROOM_HALLWAY, X = x, Y = p_lastY, Width = ( p_lastX - x ), Height = 2, OpenSides = new List<eOpenSides>() { eOpenSides.SIDE_EAST, eOpenSides.SIDE_WEST } } );
	
			GameObject go = new GameObject ( "Trigger_" + x + "_" + y );
			BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( x + ( width / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) - ( xOff * 1.28f ), ( y + ( height / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( width * 1.28f, height * 1.28f );
			p_colliders.Add ( go );

			go = new GameObject ( "Trigger_" + x + "_" + p_lastY );
			collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( x + ( width / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ), ( p_lastY + 1 ) * 1.28f - ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( ( p_lastX - x ) * 1.28f, 2 * 1.28f );
			p_colliders.Add ( go );
		} else if ( p_comeFromDir == eDirection.DIR_N ) {
			int y = ForceEven( p_lastY + Random.RandomInRange ( minOffset, maxOffset ) );
			int x = ForceEven( p_lastX - ( width / 4 ) );

			GenerateNSHallway ( p_lastX, p_lastY, y, p_tileManager, p_roomManager, ref p_station );
			p_roomManager.AddRoom ( x, y, StationRoom.GenerateRoom ( p_tileManager, width, height ), ref p_station );
			// Rooms.Add ( new StationRoom ( ) { RoomType = eRoomType.ROOM_ROOM, X = x, Y = y, Width = width, Height = height } );

			GameObject go = new GameObject ( "Trigger_" + x + "_" + y );
			BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( x + ( width / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ), ( y + ( height / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( width * 1.28f, height * 1.28f );
			p_colliders.Add ( go );

			go = new GameObject ( "Trigger_" + p_lastX + "_" + p_lastY );
			collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( p_lastX + 1 ) * 1.28f - ( 1.28f / 2.0f ), ( p_lastY + ( ( y - p_lastY ) / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( 2 * 1.28f, ( y - p_lastY ) * 1.28f );
			p_colliders.Add ( go );

			if ( p_depth < MaxDepth ) {
				int off;
				if ( p_depth > MaxDepth / 2 ) {
					off = Random.RandomInRange ( 0, 2 );
				} else {
					off = Random.RandomInRange ( 1, 3 );
				}

				List<eOpenSides> sides = new List<eOpenSides> ( ) {
					eOpenSides.SIDE_WEST,
					eOpenSides.SIDE_EAST,
					eOpenSides.SIDE_NORTH
				};
				sides = sides.OrderBy ( (item) => Random.RandomFloat ( ) ).ToList<eOpenSides> ( ); // Shuffle the sides

				for ( int i = 0; i < off; i++ ) {
					if ( sides [ i ] == eOpenSides.SIDE_NORTH ) {
						GenerateRandomRoom ( p_initial, x + ( width / 4 ), y + height, p_depth + 1, p_comeFromDir, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
					} else if ( sides [ i ] == eOpenSides.SIDE_WEST ) {
						GenerateRandomRoom ( p_initial, x, y + ( height / 4 ), p_depth + 1, eDirection.DIR_W, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
					} else if ( sides [ i ] == eOpenSides.SIDE_EAST ) {
						GenerateRandomRoom ( p_initial, x + width, y + ( height / 4 ), p_depth + 1, eDirection.DIR_E, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
					}
				}
			}
		} else if ( p_comeFromDir == eDirection.DIR_S ) {
			int y = ForceEven ( p_lastY - Random.RandomInRange ( minOffset, maxOffset ) ) - height;
			int x = ForceEven ( p_lastX - ( width / 4 ) );

			GenerateNSHallway ( p_lastX, y, p_lastY, p_tileManager, p_roomManager, ref p_station );
			p_roomManager.AddRoom ( x, y, StationRoom.GenerateRoom ( p_tileManager, width, height ), ref p_station );
			// Rooms.Add ( new StationRoom ( ) { RoomType = eRoomType.ROOM_ROOM, X = x, Y = y, Width = width, Height = height } );

			GameObject go = new GameObject ( "Trigger_" + x + "_" + y );
			BoxCollider2D collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( x + ( width / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ), ( y + ( height / 2.0f ) ) * 1.28f - ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( width * 1.28f, height * 1.28f );
			p_colliders.Add ( go );

			go = new GameObject ( "Trigger_" + p_lastX + "_" + y );
			collider = go.AddComponent<BoxCollider2D> ( );
			collider.gameObject.layer = 9;
			collider.isTrigger = true;
			collider.offset = new Vector2 ( ( p_lastX + 1 ) * 1.28f - ( 1.28f / 2.0f ), ( y + ( ( p_lastY - y + ( height / 2.0f ) ) / 2.0f ) ) * 1.28f + ( 1.28f / 2.0f ) );
			collider.size = new Vector2 ( 2 * 1.28f, ( p_lastY - y - height ) * 1.28f );
			p_colliders.Add ( go );

			if ( p_depth < MaxDepth ) {
				int off;
				if ( p_depth > MaxDepth / 2 ) {
					off = Random.RandomInRange ( 0, 2 );
				} else {
					off = Random.RandomInRange ( 1, 3 );
				}

				List<eOpenSides> sides = new List<eOpenSides> ( ) {
					eOpenSides.SIDE_WEST,
					eOpenSides.SIDE_EAST,
					eOpenSides.SIDE_SOUTH
				};
				sides = sides.OrderBy ( (item) => Random.RandomFloat ( ) ).ToList<eOpenSides> ( ); // Shuffle the sides

				for ( int i = 0; i < off; i++ ) {
					if ( sides [ i ] == eOpenSides.SIDE_SOUTH ) {
						GenerateRandomRoom ( p_initial, x + ( width / 4 ), y, p_depth + 1, p_comeFromDir, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
					} else if ( sides [ i ] == eOpenSides.SIDE_WEST ) {
						GenerateRandomRoom ( p_initial,x, y + ( height / 4 ), p_depth + 1, eDirection.DIR_W, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
					} else if ( sides [ i ] == eOpenSides.SIDE_EAST ) {
						GenerateRandomRoom ( p_initial, x + width, y + ( height / 4 ), p_depth + 1, eDirection.DIR_E, p_tileManager, p_roomManager, ref p_station, ref p_colliders );
					}
				}
			}
		}
	}

	private void GenerateNSHallway ( int p_x, int p_yMin, int p_yMax, StationTileManager p_tileManager, StationRoomManager p_roomManager, ref StationTile[,] p_station ) {
		for ( int i = p_yMin; i < p_yMax; i += 2 ) {
			p_roomManager.AddRoom ( p_x, i, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_NORTH, eOpenSides.SIDE_SOUTH ), ref p_station );
		}
	}

	private void GenerateEWHallway ( int p_xMin, int p_xMax, int p_y, StationTileManager p_tileManager, StationRoomManager p_roomManager, ref StationTile[,] p_station ) {
		for ( int i = p_xMin; i < p_xMax; i += 2 ) {
			p_roomManager.AddRoom ( i, p_y, StationRoom.GenerateHallwaysAndIntersections ( p_tileManager, eRoomType.ROOM_HALLWAY, eOpenSides.SIDE_EAST, eOpenSides.SIDE_WEST ), ref p_station );
		}
	}

	private int ForceEven ( int p_value ) {
		if ( p_value % 2 == 1 ) {
			return p_value - 1;
		}
		return p_value;
	}
}

