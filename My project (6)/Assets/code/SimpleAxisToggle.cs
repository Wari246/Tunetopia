using UnityEngine;
using UnityEngine.UI;

public class SimpleAxisToggle : MonoBehaviour
{
    public Transform targetObject;
    public Button toggleButton;
    public Text buttonText;

    private bool movementEnabled = true;
    private Plane dragPlane;
    private Vector3 dragOffset;
    private bool isDragging = false;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleMovement);
        UpdateButtonLabel();
    }

    void ToggleMovement()
    {
        movementEnabled = !movementEnabled;
        UpdateButtonLabel();
    }

    void UpdateButtonLabel()
    {
        buttonText.text = movementEnabled ? "Move: ON" : "Move: OFF";
    }

    void Update()
    {
        if (!movementEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragPlane = new Plane(Camera.main.transform.forward * -1, targetObject.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                dragOffset = targetObject.position - ray.GetPoint(enter);
                isDragging = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                targetObject.position = ray.GetPoint(enter) + dragOffset;
            }
        }

        if (Input.GetMouseButtonUp(0)) isDragging = false;
    }
}
