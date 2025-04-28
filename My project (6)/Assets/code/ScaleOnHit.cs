using UnityEngine;

public class ScaleOnHit : MonoBehaviour
{
    private bool hasScaled = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasScaled)
        {
            transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
            hasScaled = true;
        }
    }
}
