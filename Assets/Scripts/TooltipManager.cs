using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipManager : Singleton<TooltipManager> 
{
	//public static event UnityEngine.Events.UnityAction ETooltipsDestroyed;
	Tooltip activeTooltip = null;

	[SerializeField]
	Tooltip tooltipPrefab;
	

	public void CreateTooltip(string tooltipText, Transform tooltipParent)
	{
		DestroyAllTooltips();

		activeTooltip=Instantiate(tooltipPrefab);
		activeTooltip.DisplayTextOnly(tooltipText, tooltipParent);
	}	
	
	public void DestroyAllTooltips() 
	{
		if (activeTooltip!=null) 
			GameObject.Destroy(activeTooltip.gameObject);
		activeTooltip = null;
	}
}
