using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private Slider timelineSlider = null;
    [SerializeField] private Text currentTime = null;
    [SerializeField] private Text totalTime = null;
    [SerializeField] private AudioManager audioManager = null;

    private VideoPlayersController videoPlayerController = null;
    private bool pauseUpdate = false;

    public void Init (VideoPlayersController videoPlayerController)
    {
        this.videoPlayerController = videoPlayerController;
    }

    public void Show() 
    {
        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        volumeSlider.onValueChanged.AddListener( OnVolumeValueChanged );
        timelineSlider.onValueChanged.AddListener(OnTimelineValueChanged);
        videoPlayerController.videoChanged += OnVideoChanged;
        OnVideoChanged(0);
    }

    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveAllListeners();
        timelineSlider.onValueChanged.RemoveAllListeners();
        videoPlayerController.videoChanged -= OnVideoChanged;
    }

    private void OnVolumeValueChanged( float value ) 
    {
        AudioListener.volume = value;
    }

    private void OnTimelineValueChanged(float value) 
    {
        if (!pauseUpdate)
            return;
            
        videoPlayerController.GetCurrentVideo().Time = value;
        audioManager.SeekTo(value);

        currentTime.text = ((int)value).ToString();
    }

    private void OnVideoChanged(int videoId) 
    {
        pauseUpdate = true;
        StartCoroutine(WaitForVideoInitialized());
    }

    private IEnumerator WaitForVideoInitialized() 
    {
        while (videoPlayerController.GetCurrentVideo().Length == 0)
            yield return new WaitForFixedUpdate();
        
        pauseUpdate = false;
        timelineSlider.maxValue = videoPlayerController.GetCurrentVideo().Length;
        totalTime.text = ((int)timelineSlider.maxValue).ToString();
    }

    private void Update()
    {
        if (pauseUpdate)
            return;

        if (videoPlayerController.GetCurrentVideo().Time == 0) //avoid zero time on then videos switching
            return;

        timelineSlider.value = (float)videoPlayerController.GetCurrentVideo().Time;
        currentTime.text = ((int)timelineSlider.value).ToString();
    }

    public void OnTimelineDown() 
    {
        pauseUpdate = true;
    }

    public void OnTimelineUp()
    {
        pauseUpdate = false;
    }
}
