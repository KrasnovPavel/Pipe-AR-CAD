using UnityEngine;
using Vuforia;

public class MarkTrackableEventHandler : DefaultTrackableEventHandler
{

   public int Id;

   protected override void OnTrackingFound()
   {
      base.OnTrackingFound();
      // table add mark
      GameObject TableObject = GameObject.Find("Table");
      MarksHandler TableObjectMarksHandler =  TableObject.GetComponent<MarksHandler>();
      TableObjectMarksHandler.ActiveMarks.Add(gameObject);
   }
   protected override void OnTrackingLost()
   {
       base.OnTrackingLost();
       // table remove mark
      
       GameObject TableObject = GameObject.Find("Table");
       MarksHandler TableObjectMarksHandler =  TableObject.GetComponent<MarksHandler>();
       TableObjectMarksHandler.ActiveMarks.Remove(gameObject);
   }

}
