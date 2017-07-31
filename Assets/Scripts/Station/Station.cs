using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public enum eStationSize {
	STATION_SMALL,
	STATION_MEDIUM,
	STATION_LARGE
}

public class Station : MonoBehaviour {

	public StationTileManager TileManager;
	public StationRoomManager RoomManager;
	public StationDecoratorManager DecorationManager;
	public ToolManager ToolManager;

	public StationGenerator StationGenerator;

	public eStationSize Size;

	public long Seed = 1234;
	public int MaxWidth = 100;
	public int MaxHeight = 100; 

	public const float TileSize = 1.28f; // Width && height of tiles

	private StationTile[,] StationTiles;
	private List<GameObject> StationTriggers;
	private List<DecWrapper> Decorations;
	private List<StationRoom> Rooms;

	public bool HasWrench = true;
	public bool HasWelder = true;

	void Awake ( ) {
		int maxDepth = 2;
		if ( Size == eStationSize.STATION_SMALL ) {
			maxDepth = 2;
		} else if ( Size == eStationSize.STATION_MEDIUM ) {
			maxDepth = 4;
		} else if ( Size == eStationSize.STATION_LARGE ) {
			maxDepth = 6;
		}

		Seed = new System.Random ( ).Next ( );
		StationGenerator.Init ( Seed, maxDepth );
		Rooms = StationGenerator.Generate ( MaxWidth, MaxHeight, Size, TileManager, RoomManager, DecorationManager, ref StationTiles, ref StationTriggers, ref Decorations );

		TilesToWorld ( );
		CreateBounds ( );
		CreateDecorations ( );

		SpawnNewTool ( eToolType.TOOL_WELDER );
		SpawnNewTool ( eToolType.TOOL_WRENCH );
	}

	void Update ( ) {
		if ( !HasWelder ) {
			StartCoroutine ( _SpawnNewTool ( eToolType.TOOL_WELDER ) );
			HasWelder = true;
		}

		if ( !HasWrench ) {
			StartCoroutine ( _SpawnNewTool ( eToolType.TOOL_WRENCH ) );
			HasWrench = true;
		}
	}

	IEnumerator _SpawnNewTool( eToolType p_type ) {
		yield return new WaitForSeconds ( 60.0f );
		SpawnNewTool ( p_type );
	}

	private void SpawnNewTool ( eToolType p_type ) {
		StationRoom room = Rooms[ Random.Range ( 0, Rooms.Count ) ];

		const int maxAttempts = 10;
		int attempts = 0;
		do {
			int x = Random.Range ( room.X, room.X + room.Width );
			int y = Random.Range ( room.Y, room.Y + room.Height );

			bool validLoc = true;
			for ( int i = 0; i < Decorations.Count; i++ ) {
				if ( Decorations[ i ].X == x && Decorations [ i ].Y == y ) {
					validLoc = false;
					break;
				}
			}

			if ( !validLoc ) {
				attempts++;
				continue;
			}

			Tool t = Instantiate ( ToolManager.GetToolOfType ( p_type ) );
			t.transform.position = new Vector3 ( x * TileSize, y * TileSize );
			break;
		} while ( attempts <= maxAttempts );
	}

	private void TilesToWorld ( ) {
		for ( int x = 0; x < MaxWidth; x++ ) {
			for ( int y = 0; y < MaxHeight; y++ ) {
				StationTile myTile = StationTiles [ x, y ];
				if ( myTile == null || myTile.Type == eTileType.TILE_NONE ) {
					continue;
				}

				StationTile go = Instantiate ( myTile, transform );
				go.transform.position = new Vector3 ( x * TileSize, y * TileSize );
				go.transform.rotation = Quaternion.Euler ( 0.0f, 0.0f, go.Rotation );
				if ( myTile.Type == eTileType.TILE_SOLAR_PANEL ) {
					go.transform.localScale = new Vector3 ( 2.0f, 2.0f, 2.0f );
					if ( myTile.Rotation == 0.0f ) {
						go.transform.position = new Vector3 ( go.transform.position.x, go.transform.position.y - ( TileSize / 2.0f ) );
					} else if ( myTile.Rotation == 180.0f ) {
						go.transform.position = new Vector3 ( go.transform.position.x, go.transform.position.y + ( TileSize / 2.0f ) );
					} else if ( myTile.Rotation == 270.0f ) {
						go.transform.position = new Vector3 ( go.transform.position.x - ( TileSize / 2.0f ), go.transform.position.y );
					} else if ( myTile.Rotation == 90.0f ) {
						go.transform.position = new Vector3 ( go.transform.position.x + ( TileSize / 2.0f ), go.transform.position.y );
					}
				}
			}
		}
	}

	private void CreateBounds() {
		for ( int i = 0; i < StationTriggers.Count; i++ ) {
			StationTriggers [ i ].transform.SetParent ( transform );
		}
	}

	private void CreateDecorations() {
		for ( int i = 0; i < Decorations.Count; i++ ) {
			StationDecoration dec = Instantiate ( Decorations [ i ].DecPrefab, transform );

			if ( dec.Type == eDecorationType.DECORATION_BATTERY_E ) {
				dec.transform.position = new Vector3 ( Decorations [ i ].X * TileSize - 0.065f, Decorations [ i ].Y * TileSize, -0.001f );
			} else if ( dec.Type == eDecorationType.DECORATION_BATTERY_W ) {
				dec.transform.position = new Vector3 ( Decorations [ i ].X * TileSize + 0.065f, Decorations [ i ].Y * TileSize, -0.001f );
			} else if ( dec.Type == eDecorationType.DECORATION_LIGHT ) {
				dec.transform.position = new Vector3 ( Decorations [ i ].X * TileSize + ( TileSize / 2.0f ), Decorations [ i ].Y * TileSize + ( TileSize / 2.0f ), -0.9f );
			} else {
				dec.transform.position = new Vector3 ( Decorations [ i ].X * TileSize, Decorations [ i ].Y * TileSize );
				dec.transform.rotation = Quaternion.Euler ( 0.0f, 0.0f, Decorations [ i ].Rotation );
			}
		}
	}
}

