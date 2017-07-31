using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class StationDecoratorManager : MonoBehaviour {

	public List<StationDecoration> Decorations;

	public StationDecoration GetDecorationOfType ( eDecorationType p_type ) {
		return Decorations.FirstOrDefault ( x => x.Type == p_type );
	}
}

