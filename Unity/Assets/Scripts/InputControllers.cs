using UnityEngine;
using System;

public class InputControllers : MonoBehaviour
{
    private IRotate[] rotates;

    private void Start()
    {
        rotates = GetComponentsInChildren<IRotate>();

        foreach (var item in rotates)
        {
            item.Init(this);
        }
    }

    public void ResetRotation()
    {
        foreach(var item in rotates) 
        {
            item.ResetRotation();
        }
    }

    public Action<CameraRotateMode> CameraRotateModeChanged;

    private CameraRotateMode cameraRotateMode = CameraRotateMode.Touch;

    public CameraRotateMode CameraRotateMode
    {
        get
        {
            return cameraRotateMode;
        }
        set
        {
            cameraRotateMode = value;
            if (CameraRotateModeChanged != null)
                return;
        }
    }
}
