using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public Vector2 rotationSpeed = new Vector2(0.1f, 0.2f);
    public bool reverse;

    private Camera mainCamera;
    private Vector2 lastMousePosition;
    private GameObject selectedObject; // 右クリックで選択したオブジェクト

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 右クリックしたオブジェクトを特定
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // タグが "Rotatable" のオブジェクトのみ回転可能
                if (hit.collider.gameObject.CompareTag("Rotatable"))
                {
                    selectedObject = hit.collider.gameObject;
                    lastMousePosition = Input.mousePosition;
                }
                else
                {
                    selectedObject = null; // 回転不可のオブジェクトなら選択しない
                }
            }
            else
            {
                selectedObject = null; // 何もヒットしなかったらターゲットなし
            }
        }

        // 右クリックしている間、選択されたオブジェクトのみ回転
        if (Input.GetMouseButton(1) && selectedObject != null)
        {
            var x = reverse ? (lastMousePosition.y - Input.mousePosition.y) : (Input.mousePosition.y - lastMousePosition.y);
            var y = reverse ? (Input.mousePosition.x - lastMousePosition.x) : (lastMousePosition.x - Input.mousePosition.x);

            var newAngle = Vector3.zero;
            newAngle.x = x * rotationSpeed.x;
            newAngle.y = y * rotationSpeed.y;

            selectedObject.transform.Rotate(newAngle);

            lastMousePosition = Input.mousePosition;
        }
    }
}
