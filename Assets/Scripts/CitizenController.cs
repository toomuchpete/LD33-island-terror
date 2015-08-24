using UnityEngine;
using System.Collections;

public class CitizenController : MonoBehaviour {
	public CityController HomeCity;
	[SerializeField] private CityController DestinationCity;
	public float MoveSpeed = 0.25f;

	public bool Traveling = false;

	private Vector3 TravelDestination;
	private bool Panicked = false;
	private float PlayerLevel = 0;

	public TravelerKnowlege Knowledge = new TravelerKnowlege();

	void Start() {
		UpdatePlayerLevel();
		MoveSpeed = 0.25f + (PlayerLevel * 0.05f);
	}

	void Update() {
		UpdatePlayerLevel();
		MoveTowardDestination();
	}

	void UpdatePlayerLevel() {
		PlayerLevel = GameManager.Instance.PlayerLevel();
	}

	public void SetDestination(CityController NewDestination) {
		DestinationCity   = NewDestination;

		if (DestinationCity == null) {
			Traveling = false;
			return;
		}

		if (Knowledge.DestinationCity == "") {
			Knowledge.DestinationCity = NewDestination.Name;
		}

		TravelDestination         = NewDestination.transform.position;

		Traveling = true;
	}

	public void SetHome(CityController NewHome) {
		HomeCity = NewHome;

		Knowledge.OriginCity = NewHome.Name;
	}

	void ArriveAtDestination() {
		Knowledge.TravelerPanicked = Panicked;

		DestinationCity.ProcessArrival(Knowledge);

		SetDestination(null);
		Destroy(gameObject, 0.25f);
	}

	void Panic() {
		if (Panicked) {
			return;
		}

		SetDestination(HomeCity);
		MoveSpeed *= 1.25f + (PlayerLevel * 0.15f);
		Panicked   = true;

		Knowledge.TravelerPanicked = true;
	}

	void BlobSeen() {
		Panic();
	}

	void CitizenSeen(GameObject Friend) {
		CitizenController citizen = Friend.GetComponent<CitizenController>();

		if (citizen == null) { return; }

		// Check to see if the citizen is fleeing!
		if (Knowledge.DestinationCity != citizen.Knowledge.DestinationCity) {
			return;
		}

		if (citizen.Knowledge.TravelerPanicked) {
			Panic();
		}
	}
	
	void MoveTowardDestination() {
		if (Traveling == false) {
			return;
		}

		Vector3 Destination    = GetTravelDestination();
		float MoveDistance     = Time.deltaTime * MoveSpeed;
		Vector3 MovementVector = Destination - transform.position;
	
		transform.rotation = Quaternion.LookRotation(Vector3.forward, MovementVector);

		if (MovementVector.magnitude < MoveDistance) {
			ArriveAtDestination();
			return;
		}

		transform.position += MovementVector.normalized * MoveDistance;
	}

	Vector3 GetTravelDestination() {
		if (DestinationCity == null) {
			Traveling = false;
			TravelDestination = transform.position;
		}

		return TravelDestination;
	}
}
