using UnityEngine;

public class CameraRotateGyro : MonoBehaviour, IRotate
{
    [SerializeField] private Transform cameraTransform;

    private InputControllers inputControllers;

    public void Init( InputControllers inputController )
    {
        this.inputControllers = inputController;
    }

    private void Update ()
	{            
        if (inputControllers.CameraRotateMode != CameraRotateMode.Gyroscope) 
            return; 

        if (!SystemInfo.supportsGyroscope)
            return;

        cameraTransform.localRotation = DeviceRotation.Get();
    }

    public void ResetRotation() 
    {
        cameraTransform.rotation = Quaternion.identity;
    }
}
