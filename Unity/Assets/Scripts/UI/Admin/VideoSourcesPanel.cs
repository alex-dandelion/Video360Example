using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace UI 
{
    public class VideoSourcesPanel : MonoBehaviour , IDownloadHandler
    {
        [SerializeField] private GameObject videoItemPrefab;
        [SerializeField] private Transform verticalListContainer;
        [SerializeField] private Button addItemButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject enableButtonGO;
        [SerializeField] private Toggle useFakeToggle;
        [SerializeField] private CamPosePanel camPosePanel;
        [SerializeField] private InputField delayTimeField;

        [Space]
        [SerializeField] private SourceManager sourceManager;
        [SerializeField] private DownloadManager downloadManager;
        [SerializeField] private VideoPlayersController videoPlayersController;

        [Space]
        [SerializeField] private InputField audioUrl;
        [SerializeField] private Button loadAudioButton;
        [SerializeField] private Button resetAudioButton;
        [SerializeField] private Text audioStatusText;

        private void OnEnable()
        {
            addItemButton.onClick.AddListener(OnAddButtonClick);
            closeButton.onClick.AddListener(OnCloseButtonClick);
            loadAudioButton.onClick.AddListener(OnLoadAudioButtonClick);
            resetAudioButton.onClick.AddListener(OnReseButtonClick);
            Refresh();
        }

        private void OnDisable()
        {
            addItemButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            loadAudioButton.onClick.RemoveAllListeners();
            resetAudioButton.onClick.RemoveAllListeners();
            downloadManager.UnSubscribe(this);
        }

        private void OnAddButtonClick()
        {
            int lastId = GetLastId();
            VideoSourceModel videoSourceModel = new VideoSourceModel()
            {
                id = lastId.ToString(),
            };

            bool added = sourceManager.AddItem(videoSourceModel);
            if (added) 
            {
                Refresh();
            }
        }

        private void OnLoadAudioButtonClick() 
        {
            downloadManager.Subscribe(this);
            downloadManager.Download(new DownloadItem("-1", audioUrl.text));
        }

        private void OnReseButtonClick() 
        {
            string fileName = Application.persistentDataPath + "/Documents/audio.mp3";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private void OnCloseButtonClick() 
        {
            sourceManager.useFake = useFakeToggle.isOn;
            enableButtonGO.SetActive(true);
            this.gameObject.SetActive(false);
            videoPlayersController.Refresh();
            SaveAll();
        }

        private void Refresh()
        {
            delayTimeField.text = sourceManager.delayTime.ToString();
            foreach (Transform item in verticalListContainer) 
            {
                Destroy(item.gameObject);
            }

            Dictionary<string, VideoSourceModel> sources = sourceManager.GetAllVideoModels();
            foreach (VideoSourceModel model in sources.Values)
            {
                GameObject go = Instantiate(videoItemPrefab);
                go.transform.SetParent(verticalListContainer, false);
                go.GetComponent<VideoItemUI>().Init(model, this);
            }

            audioUrl.text = sourceManager.GetAudioURL();
            string fileName = Application.persistentDataPath + "/Documents/audio.mp3";
            if (File.Exists(fileName))
            {
                audioStatusText.text = "on disk";
            }
        }

        public void SaveAll() 
        {
            sourceManager.delayTime = (float) Convert.ToDouble(delayTimeField.text);
            sourceManager.SaveData();
            sourceManager.SaveAudioURL(audioUrl.text);
        }

        public void RmoveItem(VideoSourceModel model) 
        {
            sourceManager.RemoveItem(model);
            string fileName = Application.persistentDataPath + "/Documents/video_" + model.id+".mp4";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        public void Dowload(VideoSourceModel model, IDownloadHandler downloadHandler) 
        {
            downloadManager.Subscribe(downloadHandler);
            downloadManager.Download(new DownloadItem( model.id, model.path) );
        }

        private int GetLastId() 
        {
            int lastId = 0;
            Dictionary<string, VideoSourceModel> sources = sourceManager.GetAllVideoModels();

            foreach (VideoSourceModel item in sources.Values) 
            {
                lastId = Mathf.Max(lastId, Convert.ToInt32(item.id));
            }

            return lastId+1;
        }

        public void OpenPosWindow(VideoSourceModel model) 
        {
            Dictionary<string, VideoSourceModel> sources = sourceManager.GetAllVideoModels();
            camPosePanel.Init(model, sources.Count);
        }

        public void OnDone(DownloadItem resource, string message)
        {
            if (resource.id != "-1")
            {
                return;
            }
            audioStatusText.text = message;
        }

        public void OnProgress(DownloadItem resource, float progressValue)
        {
            if(resource.id != "-1")
            {
                return;
            }
            audioStatusText.text = "progress: " + progressValue + "%";
        }
    }
}