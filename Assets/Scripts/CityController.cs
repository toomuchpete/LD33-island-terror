using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CityController : MonoBehaviour {
	public GameObject CitizenPrefab;
	[SerializeField] private float TravelerCooldown  = 0.0f;

	public Scrollbar ThreatMeter;
	public Text TotalLostLabel;

	public CityController[] LinkedCities;

	private Vector3 CityPosition;
	public string Name;
	private float Threat = 0.0f;
	private bool BlobVisible = false;

	private Dictionary<string, int> TravelersSent  = new Dictionary<string, int>();
	private Dictionary<string, int> TravelersRecvd = new Dictionary<string, int>();
	private Dictionary<string, int> TravelersLost  = new Dictionary<string, int>();

	void Start() {
		CityPosition = transform.position;
		Name         = gameObject.name;
	}

	void Update () {
		if (TravelerCooldown <= 0.0f) {
			SpawnTraveler();
			ResetCitizenSpawnCooldown();
		}

		if (TravelerCooldown > 0f) {
			TravelerCooldown -= Time.deltaTime;
		}

		DecayThreat();
		UpdateThreatMeter();
	}


/* Defense */
	void DecayThreat() {
		if (!BlobVisible) {
			Threat -= Time.deltaTime * .10f;
		}

		Threat = Mathf.Clamp(Threat, BaselineThreat(), 1.0f);
	}

	float BaselineThreat() {
		float MinimumThreat = 0.0f;

		foreach (int LostTravelers in TravelersLost.Values) {
			if (LostTravelers > 2) {
				MinimumThreat += (LostTravelers-2) * 0.1f;
			}
		}

		return Mathf.Clamp01(MinimumThreat);
	}

	int TotalLostTravelers() {
		int Total = 0;
		foreach (int LostTravelers in TravelersLost.Values) {
			Total += LostTravelers;
		}

		return Total;
	}

	void UpdateThreatMeter() {
		if (ThreatMeter == null) {
			return;
		}

		ThreatMeter.size = Threat;
	}

	void UpdateTotalLost() {
		if (TotalLostLabel == null) {
			return;
		}

		TotalLostLabel.text = TotalLostTravelers().ToString();
	}

	void AddThreat(float Amount) {
		Threat = Mathf.Clamp01(Threat+Amount);
	}

	void BlobSeen() {
		AddThreat(1.0f);
		BlobVisible = true;
	}

	void BlobLeft() {
		BlobVisible = false;
	}

/* Immigration */
	void ResetCitizenSpawnCooldown() {
		TravelerCooldown  = 4f;                                                                              // Cooldown Minimum
		TravelerCooldown += Mathf.Abs(RandomFromDistribution.RandomFromStandardNormalDistribution()) * 2.0f; // Random contributon, typically close to 1.0f
		TravelerCooldown += Threat * 5.0f;                                                                   // Spawn less often when threat is high
	}

	void SpawnTraveler() {
		if (BlobVisible) {
			return;
		}

		Debug.Assert(LinkedCities.Length > 0, this.Name + " has no linked cities.");

		GameObject t = (GameObject)Instantiate(CitizenPrefab, CityPosition, Quaternion.identity);

		CitizenController c = t.GetComponent<CitizenController>();
		Debug.Assert(c, "CitizenController not found for " + this.Name + "'s citizen prefab.");



		CityController Destination = ChooseDestination();

		if (c) {
			c.SetHome(this);
			c.SetDestination(Destination);
		}

		CountTravelerOut(Destination);

		c.Knowledge.TravelersSent = TravelersSent[Destination.Name];
		c.Knowledge.OriginThreat  = Threat;
	}

	CityController ChooseDestination() {
		int DestinationCount = LinkedCities.Length;

		return LinkedCities[Random.Range(0,DestinationCount)];
	}

	void CountTravelerOut(CityController City) {
		CountTraveler(TravelersSent, City);
	}

	void CountTravelerIn(CityController City) {
		CountTraveler(TravelersRecvd, City);
	}

	void CountTraveler(Dictionary<string, int> Counter, CityController City) {
		if (!Counter.ContainsKey(City.Name)) {
			Counter[City.Name] = 0;
		}

		Counter[City.Name]++;
	}

	int GetTravelerCount(Dictionary<string, int> Counter, string CityName) {
		if (Counter.ContainsKey(CityName)) {
			return Counter[CityName];
		}
		
		return 0;
	}



	int GetTravelersReceivedFrom(string CityName) {
		return GetTravelerCount(TravelersRecvd, CityName);
	}

	public void ProcessArrival(TravelerKnowlege Knowledge) {
		if (Knowledge.TravelerPanicked) {
			AddThreat(0.2f);
		}

		if (Knowledge.DestinationCity == Name) {
			string OriginCityName = Knowledge.OriginCity;

			TravelersLost[OriginCityName] = Knowledge.TravelersSent - GetTravelersReceivedFrom(OriginCityName);
			UpdateTotalLost();
		}

		if (Knowledge.OriginCity == Name) {
			TravelersSent[Knowledge.DestinationCity] -= 1;
		}

		AddThreat(Knowledge.OriginThreat);
	}
}
