using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPersonButton : MonoBehaviour {
	public GameController gameController;

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
			}
		}
	}
}
