using UnityEngine;
using Vuforia;

public class TargetRecognizer : MonoBehaviour, ITrackableEventHandler
{
    public TrackableBehaviour target;

    public GameController gameController;

    public bool isTracking = false;
    public string targetName;

    void Start()
    {
        target = GetComponent<TrackableBehaviour>();
        target.RegisterTrackableEventHandler(this);
    
        gameController.AddTarget(this);
    }
     
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            // Play audio when target is found
            // Debug.Log(target.TrackableName);
            isTracking = true;
            targetName = target.TrackableName;
        }
        else
        {
            isTracking = false;
        }
    }   
}