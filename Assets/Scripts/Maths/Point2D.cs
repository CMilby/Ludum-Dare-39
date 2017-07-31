using UnityEngine;
using System.Collections;

public struct Point2D {

	public int X { get; set; }

	public int Y { get; set; }

	public Point2D (int p_x, int p_y) : this()
	{
		X = p_x;
		Y = p_y;
	}

	public static Point2D Add ( Point2D p_p1, Point2D p_p2 ) {
		return new Point2D ( p_p1.X + p_p2.X, p_p1.Y + p_p2.Y );
	}

	public static Point2D Subtract ( Point2D p_1, Point2D p_2 ) {
		return new Point2D ( p_2.X - p_1.X, p_2.Y - p_2.Y );
	}

	public static int Dot ( Point2D p_1, Point2D p_2 ) {
		return p_1.X * p_2.X + p_1.Y + p_2.Y;
	}

	public static int LengthSq ( Point2D p_point ) {
		return Dot ( p_point, p_point );
	}

	public static Point2D Abs ( Point2D p_point ) {
		return new Point2D ( Mathf.Abs ( p_point.X ), Mathf.Abs ( p_point.Y ) );
	}

	public bool Equals ( Point2D p_point ) {
		return ( X == p_point.X && Y == p_point.Y );
	}

	public bool Equals ( int p_x, int p_y ) {
		return ( X == p_x && Y == p_y );
	}
}

