using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {
	public float BaseMovementSpeed;
	public float BaseRotationSpeed;
	public GameObject Container;

	private Rigidbody2D RB2D;

	private List<GameObject> Targets = new List<GameObject>();
	private float NomCooldown = 0;
	private float MovementSpeed;

	void Start() {
		RB2D = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate () {
		RB2D.velocity = Vector2.zero;

		if (NomCooldown > 0) {
			return;
		}

		UpdateSpeeds();
		Rotate();
		Move();
	}

	void UpdateSpeeds() {
		MovementSpeed = BaseMovementSpeed + GameManager.Instance.PlayerLevel() * 6f;
	}

	void Update() {
		if (NomCooldown > 0) {
			NomCooldown -= Time.deltaTime;
		}
	}

	void Rotate() {
		transform.Rotate(-transform.forward * BaseRotationSpeed * Input.GetAxis("Rotate") * Time.deltaTime);
	}

	void Move() {
		RB2D.velocity = transform.right * MovementSpeed * Input.GetAxis("Movement") * Time.deltaTime;
		GameManager.Instance.BlobVelocity = RB2D.velocity.sqrMagnitude;
		RB2D.angularVelocity = 0.0f;
		NomNomNom ();
	}

	void NomNomNom() {
		Targets.RemoveAll(item => item == null);
		if (Targets.Count == 0) {
			return;
		}

		NomCooldown = 0.50f;
		GameManager.Instance.CitizenEaten();
		Destroy (Targets[0]);
	}

	void OnTriggerEnter2D(Collider2D other) {
		GameObject CollidedObject = other.gameObject;
		if (!CollidedObject.CompareTag("Citizen")) {
			return;
		}

		Targets.Add(CollidedObject);
	}

	void OnTriggerExit2D(Collider2D other) {
		GameObject CollidedObject = other.gameObject;
		Targets.Remove(CollidedObject);
	}
}
