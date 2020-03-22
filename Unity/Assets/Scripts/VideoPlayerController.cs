using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject camPoseObject;
    [SerializeField] private GameObject content;

    private VideoSourceModel videoSource;

    public string GetId() 
    {
        return videoSource.id;
    }

    public void Init(VideoSourceModel videoSource, SourceManager delayTime)
    {
        foreach (Transform item in content.transform)
        {
            Destroy(item.gameObject);
        }

        foreach (CamPoseModel model in videoSource.points)
        {
            GameObject go = Instantiate(camPoseObject);
            VideoButton videoButton = go.GetComponent<VideoButton>();
            go.transform.SetParent(content.transform, false);
            videoButton.Init(model);
        }
        content.SetActive(false);
        string fileName = Application.persistentDataPath + "/Documents/video_" + videoSource.id + ".mp4";
        if (!File.Exists(fileName))
            return;

        videoPlayer.url = fileName;

        this.transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    public void SetClip(VideoClip videoClip) 
    {
        videoPlayer.clip = videoClip;
    }

    public double Time 
    {
        set 
        {
            videoPlayer.time = value;
        }
        get 
        {
            return videoPlayer.time;
        }
    }   

    public void Pause() 
    {
        if (videoPlayer == null)
            return;

        videoPlayer.Pause();
    }

    public void Stop() 
    {
        if (videoPlayer == null)
            return;

        videoPlayer.Stop();

    }

    public void Play()
    {
        if (videoPlayer == null)
            return;
            
        videoPlayer.Play();
    }

    public void EnableContent(bool isEnable) 
    {
        content.SetActive(isEnable);
    }

    public float Length
    {
        get 
        {
            if (videoPlayer == null)
                return 0;

            return (float)videoPlayer.length;
        }
    }   

    public bool IsPlayingNow 
    {
        get 
        {
            return videoPlayer.isPlaying;
        }
    }
}
