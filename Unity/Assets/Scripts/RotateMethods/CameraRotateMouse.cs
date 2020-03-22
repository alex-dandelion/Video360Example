using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraRotateMouse : MonoBehaviour, IRotate
{
    private const int RMB_ID = 0;
    private Vector2? rmbPrevPos;
    private float x;
    private float y;

    [SerializeField] private GraphicRaycaster[] raycasters;
    [SerializeField] private EventSystem eventSystem;
    [Range(1, 100)]
    [SerializeField] private float rotationSpeed = 4;
    [SerializeField] private Transform cameraTransform;

    private InputControllers inputControllers;

    public void Init(InputControllers inputController)
    {
        this.inputControllers = inputController;
    }

    void Update()
    {
        if (inputControllers.CameraRotateMode != CameraRotateMode.Mouse)
            return;

        if (IsCanvasClicked())
            return;

        if (rmbPrevPos.HasValue)
        {
            if (Input.GetMouseButton(RMB_ID))
            {
                if (((int)rmbPrevPos.Value.x != (int)Input.mousePosition.x)
                    || ((int)rmbPrevPos.Value.y != (int)Input.mousePosition.y))
                {
                    x += (rmbPrevPos.Value.y - Input.mousePosition.y) * Time.deltaTime * rotationSpeed;
                    y -= (rmbPrevPos.Value.x - Input.mousePosition.x) * Time.deltaTime * rotationSpeed;
                    cameraTransform.rotation = Quaternion.Euler(x, y, 0);
                    rmbPrevPos = Input.mousePosition;
                }
            }
            else
            {
                rmbPrevPos = null;
            }
        }
        else if (Input.GetMouseButtonDown(RMB_ID))
        {
            rmbPrevPos = Input.mousePosition;
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
