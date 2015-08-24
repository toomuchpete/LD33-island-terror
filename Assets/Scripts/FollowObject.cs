using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
	public GameObject Target;

	// Update is called once per frame
	void Update () {
		if (Target) {
			transform.position = Target.transform.position;
		}
	}
}
