using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections.Generic;

public class VideoPlayersController : MonoBehaviour 
{
    [SerializeField] private List<VideoPlayerController> videos;
    [SerializeField] private SourceManager sourceManager;
    [SerializeField] private VideoClip fakeSource;
    [SerializeField] private GameObject videoPlayerPrefab;
    [SerializeField] private AudioManager audioManager;

    private int currrentId;
    private double lastVideoTime = 0;

    public Action<int> videoChanged = null;
    private bool isPlaying = false;

    private void Start()
    {
        Refresh();
    }

    public void Refresh() 
    {
        foreach(VideoPlayerController video in videos) 
        {
            Destroy(video.gameObject);
        }

        videos.Clear();

        Dictionary<string, VideoSourceModel> sources = sourceManager.GetAllVideoModels();
        foreach (VideoSourceModel item in sources.Values)
        {
            GameObject go = Instantiate(videoPlayerPrefab);
            VideoPlayerController videoController = go.GetComponent<VideoPlayerController>();
            videoController.transform.SetParent(transform, false);
            videoController.Init(item, sourceManager);

            if (sourceManager.useFake)
            {
                videoController.SetClip(fakeSource);
            }
            videos.Add(videoController);
        }

        if (videos.Count > 0)
        {
            videos[0].gameObject.SetActive(true);
            videos[0].EnableContent(true);
        }
    }

    private VideoPlayerController GetVideoById(string id) 
    {
        foreach(VideoPlayerController videoPlayer in videos)
        {
            if(videoPlayer.GetId() == id) 
            {
                return videoPlayer;
            }
        }

        return null;
    }

    public void PauseCurrent() 
    {
        GetCurrentVideo().Pause();
        audioManager.Pause();
        isPlaying = false;
    }

    public void PlayCurrent() 
    {
        GetCurrentVideo().Play();
        audioManager.Play();
        isPlaying = true;
    }

    public void Forward()
    {
        GetCurrentVideo().Time += 5;
        audioManager.SeekTo((float)GetCurrentVideo().Time );
    }

    public void Back() 
    {
        GetCurrentVideo().Time -= 5;
        audioManager.SeekTo((float)GetCurrentVideo().Time);
    }

    public void PlayVideo(int id) 
    {
        if (id > videos.Count - 1)
            return;

        if(videos[currrentId].Time != 0) 
        {
            lastVideoTime = videos[currrentId].Time;
        }

        foreach(VideoPlayerController video in videos) 
        {
            video.Stop();
            videos[currrentId].EnableContent(false);
        }

        currrentId = id;
        videos[currrentId].EnableContent(true);
        if (isPlaying) 
        {
            videos[currrentId].Play();
        }

        videos[currrentId].Time = lastVideoTime;
        if (videoChanged != null)
        {
            videoChanged.Invoke(currrentId);
        }
    }

    public int GetCurrentVideoId() 
    {
        return currrentId;
    }

    public VideoPlayerController GetCurrentVideo() 
    {
        return videos[currrentId];
    }
}
