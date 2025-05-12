using UnityEngine;

public class SnapSocket : MonoBehaviour
{
    public string socketID;  // ? ?????
    public string acceptablePlugID;
    public Transform insertionEndPoint; // ???????Transform?Unity??????????

    public float allowedAngleDifference = 10f;
    private bool isFullySnapped = false;

    public bool IsFullySnapped
    {
        get { return isFullySnapped; }
    }

    public void SetConnectionStatus(bool status)
    {
        if (isFullySnapped != status)
        {
            isFullySnapped = status;
            if (isFullySnapped)
            {
                Debug.Log(gameObject.name + " is fully snapped.");
            }
            else
            {
                Debug.Log(gameObject.name + " is not fully snapped.");
            }
        }
    }

    public void SetOccupied(bool value)
    {
        isFullySnapped = value;
    }
}
