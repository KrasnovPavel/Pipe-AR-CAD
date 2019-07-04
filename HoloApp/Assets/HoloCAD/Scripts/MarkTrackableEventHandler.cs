using UnityEngine;
using Vuforia;

public class MarkTrackableEventHandler : DefaultTrackableEventHandler
{

   public int Id;
   public bool IsActive;
   public override void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
   {
       base.OnTrackableStateChanged(previousStatus, newStatus);
       if (newStatus == TrackableBehaviour.Status.TRACKED) OnTrackingFoundMark();
       if (newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) OnTrackingLostMark();
       Debug.Log($"Status changed: {previousStatus} {newStatus}");
   }

   protected  void OnTrackingFoundMark()
   {
      base.OnTrackingFound();
      // table add mark
      
      Debug.Log($"Target {Id} found");
      GameObject TableObject = GameObject.Find("Table");
      MarksHandler TableObjectMarksHandler =  TableObject.GetComponent<MarksHandler>();
      IsActive = true;
   }
   protected  void OnTrackingLostMark()
   {
       base.OnTrackingLost();
       // table remove mark
       
       Debug.Log($"Target {Id} lost");
       GameObject TableObject = GameObject.Find("Table");
       MarksHandler TableObjectMarksHandler =  TableObject.GetComponent<MarksHandler>();
       IsActive = false;
   }

}
