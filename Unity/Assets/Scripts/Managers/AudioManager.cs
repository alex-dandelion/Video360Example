using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private bool isPlayed = false;

    public void Start() 
    {
        LoadAudio();
    }

    public void SeekTo(float position) 
    {
        if (position < 0)
            return;

        audioSource.time = position;
    }

    public void Play() 
    {
        if (isPlayed)
            return;

        isPlayed = true;
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
        isPlayed = false;
    }

    private void LoadAudio() 
    {
        string filePath = "file://" + Application.persistentDataPath + "/Documents/doublebass.wav";
        StartCoroutine(LoadAudioFromServer(filePath, AudioType.UNKNOWN, clip => {
            if (clip == null)
                return;
            audioSource.clip = clip;
        }));
    }

    private IEnumerator LoadAudioFromServer(string url, AudioType audioType, Action<AudioClip> response)
    {
        var request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);

        yield return request.SendWebRequest();

        if (!request.isHttpError && !request.isNetworkError)
        {
            response(DownloadHandlerAudioClip.GetContent(request));
        }
        else
        {
            Debug.Log($"error in audio request {url}, {request.error}");
            response(null);
        }

        request.Dispose();
    }
}
