using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FiniteResourceModel : ResourceModel
{
	public FiniteResourceModel(int resourceMax) : base(resourceMax)
	{
	}

	public override void ResetToStartingStats()
	{
		resourceCurrent = resourceMax;
	}
}

