using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class DownloadManager : MonoBehaviour
{
    private Stack<DownloadItem> resorcesQuery = new Stack<DownloadItem>();
    private List<IDownloadHandler> dowloadHandlers = new List<IDownloadHandler>();
    private bool cancelLoading;
    private UnityWebRequest downloadWebRequest;

    public void Subscribe( IDownloadHandler dowloadHanler )
    {
        if(!dowloadHandlers.Contains(dowloadHanler))
        {
            dowloadHandlers.Add(dowloadHanler);
        }
    }

    public void UnSubscribe( IDownloadHandler dowloadHanler )
    {
        dowloadHandlers.Remove(dowloadHanler);
    }
   
    public void Download( DownloadItem resource  )
    {
        string fileName = string.Empty;
        if (resource.id != "-1")
        {
            fileName = "video_" + resource.id + ".mp4";
        }
        else
        {
            fileName = "audio.mp3";
        }

        string videoPath = Application.persistentDataPath + "/Documents/" + fileName;
        if (File.Exists(videoPath))
        {
            File.Delete(videoPath);
        }
        else
        {
            resorcesQuery.Push(resource);
        }

        if(downloadWebRequest == null)
        {
            StartCoroutine(DownloadNext());
        }
    }

    private IEnumerator DownloadNext(bool pauseOnStart = false)
    {
        cancelLoading = false;
        if (resorcesQuery.Count == 0)
        {
            yield break;
        }

        DownloadItem resourceInWork = resorcesQuery.Pop();
        downloadWebRequest = new UnityWebRequest(resourceInWork.path);

        Debug.Log($"start load form path {resourceInWork.path} ");
        if (pauseOnStart)
        {
            yield return new WaitForSeconds(2); 
        }

        string directoryName = Application.persistentDataPath + "/Documents/";
        if (!Directory.Exists(Application.persistentDataPath + "/Documents/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Documents/");
        }
        string fileName = "";
        if(resourceInWork.id != "-1") 
        {
            fileName = "video_" + resourceInWork.id + ".mp4";
        }else
        {
            fileName = "audio.mp3";
        }

        FileDownloadHandler downloadHandler = new FileDownloadHandler(new byte[4 * 1024], directoryName, fileName );
        downloadWebRequest.downloadHandler = downloadHandler;
        downloadWebRequest.SendWebRequest();

        while (downloadWebRequest != null && !downloadWebRequest.isDone) 
        {
            yield return new WaitForSeconds(0.5f);
            if (cancelLoading)
            {
                yield break;
            }

            List<IDownloadHandler> safeList = new List<IDownloadHandler>(dowloadHandlers); //aviod list content changes while iterate

            foreach ( IDownloadHandler item in safeList )
            {
                item.OnProgress( resourceInWork, downloadHandler.Progress );
            }
        }

        bool mustPauseOnStart = true;
        if ( !String.IsNullOrEmpty(downloadWebRequest.error) || downloadWebRequest.isNetworkError || downloadWebRequest.isHttpError )
        {
            Debug.LogError($"Download error: {resourceInWork.id}, Error: {downloadWebRequest.error}");
            string filePath = directoryName + "video_" + resourceInWork.id + ".mp4";
            if ( File.Exists(filePath) ) 
            {
                File.Delete(filePath);
            }

            List<IDownloadHandler> safeList = new List<IDownloadHandler>(dowloadHandlers);
            foreach (IDownloadHandler item in safeList)
            {
                item.OnDone(resourceInWork, $"error: {downloadWebRequest.error}");
            }
        }
        else if ( downloadWebRequest.responseCode == 410 )
        {
            Debug.LogError($"File {resourceInWork.id} is gone, must send another request");
        }
        else
        { 
            List<IDownloadHandler> safeList = new List<IDownloadHandler>(dowloadHandlers);
            foreach (IDownloadHandler item in safeList)
            {
                item.OnDone(resourceInWork, "done");
            }
            mustPauseOnStart = false;
        }

        resourceInWork = null;
        downloadWebRequest = null;

        if(resorcesQuery.Count >0 )
        {
           StartCoroutine(DownloadNext(mustPauseOnStart));
        }
    } 

    public void Cancel()
    {
        resorcesQuery.Clear();
        cancelLoading = true;

        if (downloadWebRequest == null)
            return;

        (downloadWebRequest.downloadHandler as FileDownloadHandler).Cancel();
        downloadWebRequest = null;
    }
} 
