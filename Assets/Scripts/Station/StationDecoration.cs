using UnityEngine;
using System.Collections;

public enum eDecorationType {
	DECORATION_BATTERY_W,
	DECORATION_BATTERY_E,
	DECORATION_BED,
	DECORATION_SHELF,
	DECORATION_TABLE,
	DECORATION_LIGHT
}

public class DecWrapper {

	public StationDecoration DecPrefab;
	public int X;
	public int Y;

	public float Rotation;
}

public class StationDecoration : MonoBehaviour {

	public eDecorationType Type;
}

