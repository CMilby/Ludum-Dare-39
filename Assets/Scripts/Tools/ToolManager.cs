using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

public enum eToolType {
	TOOL_WRENCH,
	TOOL_WELDER
}

public class ToolManager : MonoBehaviour {

	public List<Tool> Tools;

	public Tool GetToolOfType ( eToolType p_type ) {
		return Tools.FirstOrDefault ( x => x.Type == p_type );
	}
}

