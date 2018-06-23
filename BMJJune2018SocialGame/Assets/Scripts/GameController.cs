using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public Button connectButton;
	public InputField serverAddressField;

	public Button hitPersonButton;
	public RawImage image;
	public GameObject frame;

	public GameObject splashScreen;
	public GameObject leaderboard;
	public GameObject scrollable;
	public GameObject attackScreen;

	private List<TargetRecognizer> possibleTargets;

	void Awake () {
		possibleTargets = new List<TargetRecognizer>();
	}

	private void Start() {
		connectButton.onClick.AddListener(TryConnectToServer);

		hitPersonButton.onClick.AddListener(HitPerson);
	}

	void TryConnectToServer() {
		string address = serverAddressField.text;
		// TODO connect to server
		splashScreen.SetActive(false);
		attackScreen.SetActive(true);
	}

	public void AddTarget(TargetRecognizer t) {
		possibleTargets.Add(t);
	}

	public List<TargetRecognizer> getTargets() {
		return possibleTargets;
	}


	void HitPerson() {
		// get available markers
		List<TargetRecognizer> possibleTargets = getTargets();

		foreach (TargetRecognizer t in possibleTargets)
		{
			if(t.isTracking) {
				// do stuff
				Debug.Log("HIT PERSON: " + t.targetName);
				frame.SetActive(false);
				StartCoroutine("TakeSnapshot");
			}
		}
	}

	WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

	public IEnumerator TakeSnapshot()
	{
		yield return frameEnd;


		float cutoff = 0.125f * Screen.height;
		int h = (int) (Screen.height - cutoff * 2);

		Texture2D tex = new Texture2D(Screen.width, h);

		tex.ReadPixels(new Rect(0, cutoff, Screen.width, h), 0, 0);
		tex.Apply();

		byte[] data = tex.EncodeToPNG();
		Debug.Log(data);

		image.texture = tex;

		frame.SetActive(true);

		// continue
		attackScreen.SetActive(false);
		scrollable.SetActive(true);
	}
}
