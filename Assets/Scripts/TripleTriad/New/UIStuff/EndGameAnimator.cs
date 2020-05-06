using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public class EndGameAnimator : MonoBehaviour
	{

		#region Configuration


		#endregion



		#region Functions

		public void ControlUiLoadingModifier(int shouldIBeLoading)
		{
			if (shouldIBeLoading == 0)
			{
				TripleTriadManager.instance.ttUi.isLoading = true;
			}
			else
			{
				TripleTriadManager.instance.ttUi.isLoading = false;
			}

		}

		#endregion
	}
}
