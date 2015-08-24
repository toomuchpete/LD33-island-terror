[System.Serializable]
public struct TravelerKnowlege {
	public string OriginCity;
	public float OriginThreat;
	public string DestinationCity;
	public int TravelersSent;
	public bool TravelerPanicked;

	override public string ToString() {
		string Output;

		Output = OriginCity + " => " + DestinationCity + " { Sent: " + TravelersSent + ", " + "Threat: " + OriginThreat + ", Panicked: " + TravelerPanicked + "}";

		return Output;
	}
}
