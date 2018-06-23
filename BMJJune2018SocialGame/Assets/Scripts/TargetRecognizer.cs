using UnityEngine;
using Vuforia;

public class TargetRecognizer : MonoBehaviour, ITrackableEventHandler
{
    public TrackableBehaviour target;

    void Start()
    {
        target = GetComponent<TrackableBehaviour>();
        target.RegisterTrackableEventHandler(this);
    
    
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
            Debug.Log(target.TrackableName);
            
        }
        else
        {

        }
    }   
}