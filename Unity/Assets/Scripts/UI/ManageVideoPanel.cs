using UnityEngine;
using UnityEngine.UI;

public class ManageVideoPanel : MonoBehaviour
{
    [SerializeField] private Button playButton = null;
    [SerializeField] private Button pauseButton = null;
    [SerializeField] private Button prevButton = null;
    [SerializeField] private Button forvardButton = null;
    [SerializeField] private Button settingsButton = null;
    [SerializeField] private Button gyroButton = null;
    [SerializeField] private Button touchButton = null;
    [SerializeField] private Text   videoLabel = null;
    [SerializeField] private Transform cameraTransform = null;

    [Space]
    [SerializeField] private VideoPlayersController videoPlayersController;
    [SerializeField] private SettingPanel settingsPanel;

    private void Awake()
    {
        settingsPanel.Init(videoPlayersController);
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        pauseButton.onClick.AddListener(OnPauseButtonClick);
        prevButton.onClick.AddListener(OnPrevButtonClick);
        forvardButton.onClick.AddListener(OnForvardButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        gyroButton.onClick.AddListener(OnGyroButtonClick);
        touchButton.onClick.AddListener(OnTouchButtonClick);
        videoPlayersController.videoChanged += OnVideoChanged;
        videoLabel.text = "Video " + videoPlayersController.GetCurrentVideoId();
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
        prevButton.onClick.RemoveAllListeners();
        forvardButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        gyroButton.onClick.RemoveAllListeners();
        touchButton.onClick.RemoveAllListeners();
        videoPlayersController.videoChanged -= OnVideoChanged;
    }

    private void OnPlayButtonClick() 
    {
        videoPlayersController.PlayCurrent();
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    private void OnPauseButtonClick()
    {
        videoPlayersController.PauseCurrent();
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    private void OnPrevButtonClick()
    {
        videoPlayersController.Back();
    }

    private void OnForvardButtonClick()
    {
        videoPlayersController.Forward();
    }

    private void OnSettingsButtonClick()
    {
        if (settingsPanel.isActiveAndEnabled) 
        {
            videoLabel.gameObject.SetActive(true);
            settingsPanel.Hide();
        }
        else 
        {
            videoLabel.gameObject.SetActive(false);
            settingsPanel.Show();
        }
    }

    private void OnGyroButtonClick() 
    {
        cameraTransform.rotation = Quaternion.identity;
        gyroButton.gameObject.SetActive(false);
        touchButton.gameObject.SetActive(true);
        FindObjectOfType<InputControllers>().CameraRotateMode = CameraRotateMode.Gyroscope;
    }

    private void OnTouchButtonClick() 
    {
        cameraTransform.rotation = Quaternion.identity;
        gyroButton.gameObject.SetActive(true);
        touchButton.gameObject.SetActive(false);
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            FindObjectOfType<InputControllers>().CameraRotateMode = CameraRotateMode.Touch;
        else
            FindObjectOfType<InputControllers>().CameraRotateMode = CameraRotateMode.Mouse;
    }

    private void OnVideoChanged(int id) 
    {
        videoLabel.text = "Video " + id;
    }
}