using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PosRendererElement : MonoBehaviour
{
    [SerializeField] private InputField idField;
    [SerializeField] private InputField posXField;
    [SerializeField] private InputField posYField;
    [SerializeField] private InputField posZField;
    [SerializeField] private InputField rotationField;

    private CamPoseModel camPosModel = null;

    public void Init(CamPoseModel camPosModel) 
    {
        this.camPosModel = camPosModel;
        idField.text = camPosModel.id;
        posXField.text = camPosModel.position.x.ToString();
        posYField.text = camPosModel.position.y.ToString();
        posZField.text = camPosModel.position.z.ToString();
        rotationField.text = camPosModel.rotation.z.ToString();
    }

    public Vector3 GetPosition() 
    {
        return new Vector3( (float)Convert.ToDouble(posXField.text), (float)Convert.ToDouble(posYField.text), (float)Convert.ToDouble(posZField.text));
    }

    public Vector3 GetRotation() 
    {
        return new Vector3(0, 0, (float)Convert.ToDouble(rotationField.text) );
    }

    public string GetId() 
    {
        return camPosModel.id;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
