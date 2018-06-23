using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPersonButton : MonoBehaviour {
	public GameController gameController;

	public RawImage image;
	public GameObject frame;

	// Use this for initialization
	void Start () {
		Button but = GetComponent<Button>();
		but.onClick.AddListener(HitPerson);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void HitPerson() {
		// get available markers
		List<TargetRecognizer> possibleTargets = gameController.getTargets();

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
	}
}
