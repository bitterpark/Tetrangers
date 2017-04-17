using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class TabButtonsView : MonoBehaviour
{
	public static event UnityAction EWeaponsTabPressedGlobal;
	public event UnityAction EWeaponsTabPressed;
	public static event UnityAction EEquipmentTabPressedGlobal;
	public event UnityAction EEquipmentTabPressed;
	public static event UnityAction ESkillsTabPressedGlobal;
	public event UnityAction ESkillsTabPressed;

	[SerializeField]
	Button weaponsTabButton;
	[SerializeField]
	Button equipmentTabButton;
	[SerializeField]
	Button skillsTabButton;

	void Awake()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		weaponsTabButton.onClick.AddListener(() => 
		{
			if (EWeaponsTabPressedGlobal != null) EWeaponsTabPressedGlobal();
			if (EWeaponsTabPressed != null) EWeaponsTabPressed();
		});
		equipmentTabButton.onClick.AddListener(() => 
		{
			if (EEquipmentTabPressedGlobal != null) EEquipmentTabPressedGlobal();
			if (EEquipmentTabPressed != null) EEquipmentTabPressed();
		});
		skillsTabButton.onClick.AddListener(() => 
		{
			if (ESkillsTabPressedGlobal != null) ESkillsTabPressedGlobal();
			if (ESkillsTabPressed != null) ESkillsTabPressed();
		});
	}

	public void ClearView()
	{
		EWeaponsTabPressed = null;
		EEquipmentTabPressed = null;
		ESkillsTabPressed = null;
	}
}
