using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraRotateSwipe : MonoBehaviour, IRotate
{
    [SerializeField] private GraphicRaycaster[] raycasters;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Transform cameraTransform;

    [Range(0.01f, 1f)]
    [SerializeField] private float rotationSpeed;
    [Tooltip("Минимальная величина сдвига в одном кадре")]
    [Range(1f, 10f)]
    [SerializeField] private float sensitivity;

    private float x;
    private float y;

    private InputControllers inputControllers;

    public void Init(InputControllers inputController)
    {
        this.inputControllers = inputController;
    }

    private void Update()
    {
        if (inputControllers.CameraRotateMode != CameraRotateMode.Touch) 
            return;

        if ( Input.touchCount == 0 || IsCanvasClicked() )
            return;
            
        Touch touch = Input.touches[0];
        if(touch.phase == TouchPhase.Moved)
        {
            Vector3 rotation = touch.deltaPosition;
            if (Mathf.Abs(rotation.x) < sensitivity && Mathf.Abs(rotation.y) < sensitivity)
                return;

            x += rotation.x * rotationSpeed;
            y -= rotation.y * rotationSpeed;
            cameraTransform.rotation = Quaternion.Euler( y, x, 0 );
        }
    }

    private bool IsCanvasClicked()
    {
        foreach (GraphicRaycaster raycaster in raycasters)
        {
            PointerEventData m_PointerEventData = new PointerEventData(eventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetRotation() 
    {
        cameraTransform.rotation = Quaternion.identity;
        x = 0;
        y = 0;
    }
}
