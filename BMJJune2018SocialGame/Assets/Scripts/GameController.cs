using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

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
	public GameObject loadingScreen;

	private List<TargetRecognizer> possibleTargets;

	private string serverAddress;

	void Awake () {
		possibleTargets = new List<TargetRecognizer>();
	}

	private void Start() {
		connectButton.onClick.AddListener(TryConnectToServer);

		hitPersonButton.onClick.AddListener(HitPerson);
	}

    public string Url(string path) {
        return "http://" + serverAddress + ":3000/" + path;
    }

	public delegate void Callback(string message);

	public IEnumerator Get(string path, Callback onSuccess, Callback onFail) {
		string url = Url(path);
		UnityWebRequest request = null;

		try {
			request = UnityWebRequest.Get(url);
		} 
		catch(System.UriFormatException e) {
			if(onFail != null) onFail(e.Message);
			yield break;
		}

		using (request) {
			yield return request.SendWebRequest();
			if (request.isNetworkError || request.isHttpError) {
				if(onFail != null) onFail(request.error);
				yield break;
			}

			byte[] results = request.downloadHandler.data;
			string str = Encoding.Default.GetString(results);
			if(onSuccess != null) onSuccess(str);
		}
    }

	void TryConnectToServer() {
		serverAddress = serverAddressField.text;
		// TODO show some loading circle or similar
		loadingScreen.SetActive(true);
		StartCoroutine(Get("", (string message) => {
			// success
			loadingScreen.SetActive(false);
			
			splashScreen.SetActive(false);
			attackScreen.SetActive(true);
		}, (string message) => {
			// fail
			// TODO show some warning
			Debug.Log("Error: " + message);
			loadingScreen.SetActive(false);
		}));
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
