using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Door : MonoBehaviour
{
    public MMFeedbacks closeDoorFeedback;
    public MMFeedbacks openDoorFeedback;

    public void CloseDoor()
    {
        closeDoorFeedback.PlayFeedbacks();
    }
    public void OpenDoor()
    {
        openDoorFeedback.PlayFeedbacks();
    }
}
