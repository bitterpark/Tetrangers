using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipView : MonoBehaviour {

	[SerializeField]
	Text shipName;
	[SerializeField]
	Image shipImage;
	[SerializeField]
	Text healthText;
	[SerializeField]
	Text energyText;
	[SerializeField]
	Transform weaponsArea;

	public void SetNameAndSprite(string name, Sprite sprite)
	{
		shipName.text = name;
		shipImage.sprite = sprite;
	}

	public void SetHealth(int newHealth, int maxHealth)
	{
		healthText.text = newHealth.ToString() + "/" + maxHealth.ToString();
	}

	public void SetEnergy(int energy)
	{
		energyText.text = energy.ToString();
	}

}
