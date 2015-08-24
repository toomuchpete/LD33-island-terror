using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HungerHUD : MonoBehaviour {
	public Scrollbar HungerBar;

	void Update () {
		HungerBar.size = GameManager.Instance.StomachFullPercent();
	}
}
