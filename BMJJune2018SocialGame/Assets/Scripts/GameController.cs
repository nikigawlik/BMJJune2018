using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	private List<TargetRecognizer> possibleTargets;

	void Awake () {
		possibleTargets = new List<TargetRecognizer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddTarget(TargetRecognizer t) {
		possibleTargets.Add(t);
	}

	public List<TargetRecognizer> getTargets() {
		return possibleTargets;
	}
}
