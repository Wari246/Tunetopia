using UnityEngine;

public class ScaleOnTrigger : MonoBehaviour
{
    private bool hasScaled = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasScaled)
        {
            transform.localScale *= 1.5f;
            hasScaled = true;
        }
    }
}
