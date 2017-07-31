using UnityEngine;
using System.Collections;

public class PlayerInventory : MonoBehaviour {

	public Tool WelderTool;
	public Tool WrenchTool;

	public bool HasWelder;
	public bool HasWrench;

	void Awake() {
		HasWelder = HasWelder = false;
	}

	void Update ( ) {
		if ( WelderTool != null && WelderTool.UsesLeft <= 0 ) {
			WelderTool = null;
			HasWelder = false;
		}

		if ( WrenchTool != null && WrenchTool.UsesLeft <= 0 ) {
			WrenchTool = null;
			HasWrench = false;
		}
	}

	public void AddTool ( Tool p_tool ) {
		if ( p_tool.Type == eToolType.TOOL_WELDER ) {
			HasWelder = true;
			WelderTool = p_tool;
			WelderTool.UsesLeft = 8;
		} else if ( p_tool.Type == eToolType.TOOL_WRENCH ) {
			HasWrench = true;
			WrenchTool = p_tool;
			WrenchTool.UsesLeft = 8;
		}
	}

	public void Fixed ( eToolType p_type ) {
		if ( p_type == eToolType.TOOL_WELDER && HasWelder ) {
			WelderTool.UsesLeft--;
			if ( WelderTool.UsesLeft <= 0 ) {
				HasWelder = false;
				WelderTool = null;
			}
		} else if ( p_type == eToolType.TOOL_WRENCH && HasWrench ) {
			WrenchTool.UsesLeft--;
			if ( WrenchTool.UsesLeft <= 0 ) {
				HasWrench = false;
				WrenchTool = null;
			}
		}
	}
}

