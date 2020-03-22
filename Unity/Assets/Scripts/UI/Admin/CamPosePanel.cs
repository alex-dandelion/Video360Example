using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CamPosePanel : MonoBehaviour
{
    [SerializeField] private Button okButton;
    [SerializeField] private Transform pointContainer;
    [SerializeField] private GameObject pointRendererPrefab;
    [SerializeField] private GameObject camPosPanel;
    private VideoSourceModel videoSourceModel;

    public void Init( VideoSourceModel videoSourceModel, int camCount) 
    {
        this.gameObject.SetActive(true);
        this.videoSourceModel = videoSourceModel;

        if(videoSourceModel.points == null) 
        {
            videoSourceModel.points = new List<CamPoseModel>();
        }

        while ( videoSourceModel.points.Count < camCount ) 
        {
            float pos = -9 + videoSourceModel.points.Count * 6;
            videoSourceModel.points.Add( new CamPoseModel() { id = videoSourceModel.points.Count.ToString(), position = new Vector3(pos, -3.0f, 20.6f) } );
        }

        foreach(Transform tr in pointContainer) 
        {
            Destroy(tr.gameObject);
        }

        int i = 0;
        foreach (CamPoseModel camPoseModel in videoSourceModel.points) 
        {
            GameObject go = Instantiate(pointRendererPrefab);
            go.transform.SetParent(pointContainer,false);
            PosRendererElement posRenderer = go.GetComponent<PosRendererElement>();
            posRenderer.Init(camPoseModel);
            i++;
        }
    }

    public void OnEnable()
    {
        okButton.onClick.AddListener(OnButtonOkClick);
    }

    public void OnDisable()
    {
        okButton.onClick.RemoveAllListeners();
    }

    private void OnButtonOkClick()
    {
        foreach ( Transform item in pointContainer) 
        {
            PosRendererElement posRenderer = item.GetComponent<PosRendererElement>();
            CamPoseModel camPose = GetCamPoseModel(videoSourceModel.points, posRenderer.GetId());
            if (camPose == null)
                continue;

            camPose.position = posRenderer.GetPosition();
            camPose.rotation = posRenderer.GetRotation();
        }

        camPosPanel.SetActive(false);
    }

    private CamPoseModel GetCamPoseModel(List<CamPoseModel> camsPoseModel, string id) 
    {
        foreach(CamPoseModel camPose in camsPoseModel) 
        {
            if(camPose.id == id)
            {
                return camPose;
            }
        }
        return null;
    }
}
