using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras; // 4つのカメラをInspectorで設定
    private int currentIndex = 0;

    void Start()
    {
        UpdateCamera();
    }

    public void SwitchCamera()
    {
        currentIndex = (currentIndex + 1) % cameras.Length;
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == currentIndex);
        }
    }
}
