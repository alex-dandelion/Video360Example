using UnityEngine;
using UnityEngine.UI;
using System;

public class VideoButton : MonoBehaviour
{
    [SerializeField] private Text idText;
    [SerializeField] private Button button;
    [SerializeField] private Image spnnerImage;
        
    public static float lastClickTime;

    private SourceManager sourceManager;
    private Transform target;
    private CamPoseModel camPosModel;
    private float timeLeft = -1;
    private float spinPos = -1;
    private bool delayedPlay = false;

    private void Awake()
    {
        target = GameObject.FindWithTag("MainCamera").transform;
        sourceManager = FindObjectOfType<SourceManager>();
    }

    private void Update()
    {
        transform.LookAt(target);

        if(spinPos < 0) 
        {
            spnnerImage.gameObject.SetActive(false);
        }
        else if (spinPos > 0) 
        {
            float t = timeLeft / spinPos;
            spnnerImage.fillAmount = Mathf.Lerp(1, 0, spinPos / timeLeft );
            spinPos -= Time.deltaTime;
            if(spinPos <=0 && delayedPlay) 
            {
                delayedPlay = false;
                ReseRotation();
                FindObjectOfType<VideoPlayersController>().PlayVideo(Convert.ToInt32(camPosModel.id));
                lastClickTime = Time.time;
            }
        }
    }

    public void Init(CamPoseModel camPosModel) 
    {
        this.camPosModel = camPosModel;
        idText.text = camPosModel.id;
        transform.position = camPosModel.position;
        Vector3 pivot = button.transform.localRotation.eulerAngles;
        button.transform.localRotation = Quaternion.Euler( pivot.x, pivot.y, camPosModel.rotation.z );
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void OnButtonClick() 
    {
        if (delayedPlay)
            return;

        if (lastClickTime + sourceManager.delayTime > Time.time ) 
        {
            spnnerImage.gameObject.SetActive(true);
            timeLeft = lastClickTime + sourceManager.delayTime - Time.time;
            spinPos = timeLeft;
            delayedPlay = true;
            return;
        }

        lastClickTime = Time.time;
        ReseRotation();
        FindObjectOfType<VideoPlayersController>().PlayVideo( Convert.ToInt32( camPosModel.id ) );
    }

    private void ReseRotation() 
    {
        FindObjectOfType<CameraRotateSwipe>()?.ResetRotation();
        FindObjectOfType<CameraRotateGyro>()?.ResetRotation();
        FindObjectOfType<CameraRotateMouse>()?.ResetRotation();
    }
}
