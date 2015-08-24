using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public Text ScoreText;
	public static GameManager Instance;

	private int Score   = 0;
	private float Level = 0.0f;

	// Hunger Stuff
	public float BlobVelocity;
	public float BaseMetabolism;
	public float StomachVolume   = 100f;
	public float StomachContents = 100f;

	public float CurrentMetabolism;

	void Start () {
		Instance = this;
	}

	void Update() {
		ScoreText.text = Score.ToString();
		Digest();
	}

	void Digest() {
		StomachContents -= GetMetabolism() * Time.deltaTime;
		if (StomachContents < 0) {
			GameOver();
		}
	}

	void GameOver() {
		Time.timeScale = 0;
	}

	float GetMetabolism() {
		CurrentMetabolism = BaseMetabolism + (BlobVelocity * Level * 0.2f); 
		return CurrentMetabolism;
	}

	public void CitizenEaten() {
		Score += 1;
		Level = Mathf.Log(Score, 2) + (Score / 5.0f);
		StomachContents = Mathf.Min(StomachVolume, StomachContents + 20f); 
	}

	public float PlayerLevel() {
		return Level;
	}

	public float StomachFullPercent() {
		return StomachContents / StomachVolume;
	}
}
