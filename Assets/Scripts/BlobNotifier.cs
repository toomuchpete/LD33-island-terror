using UnityEngine;
using System.Collections;

public class BlobNotifier : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			gameObject.SendMessageUpwards("BlobSeen", other.gameObject, SendMessageOptions.DontRequireReceiver);
		}

		if (other.gameObject.CompareTag("Citizen")) {
			gameObject.SendMessageUpwards("CitizenSeen", other.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			gameObject.SendMessageUpwards("BlobLeft", other.gameObject, SendMessageOptions.DontRequireReceiver);
		}		
	}
}
