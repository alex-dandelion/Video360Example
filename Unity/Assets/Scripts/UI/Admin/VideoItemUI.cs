using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace UI 
{
    public class VideoItemUI : MonoBehaviour, IDownloadHandler
    {
        [SerializeField] private InputField id;
        [SerializeField] private InputField linkField;
        [SerializeField] private Button posButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private Text statusText;

        private VideoSourcesPanel videoSourcesPanel;
        private VideoSourceModel videoSourceModel;

        public void Init(VideoSourceModel videoSourceModel, VideoSourcesPanel videoSourcesPanel ) 
        {
            id.text = videoSourceModel.id;
            linkField.text = videoSourceModel.path;

            loadButton.onClick.AddListener(OnLoadButtonClick);
            removeButton.onClick.AddListener(OnRemoveButtonClick);
            posButton.onClick.AddListener(OnPosButtonClick);
            linkField.onEndEdit.AddListener(OnEndEdit);
            this.videoSourcesPanel = videoSourcesPanel;
            this.videoSourceModel = videoSourceModel;

            string videoPath = Application.persistentDataPath + "/Documents/video_" + videoSourceModel.id + ".mp4";
            if (File.Exists(videoPath))
            {
                statusText.text = "on disk";
            }
            else 
            {
                statusText.text = "empty";
            }
        }

        public void OnProgress(DownloadItem resource, float progressValue)
        {
            if(resource.id != videoSourceModel.id) 
            {
                return;
            }

            statusText.text = "progress: " + progressValue + "%";
        }

        public void OnDone(DownloadItem resource, string message)
        {
            if (resource.id != videoSourceModel.id)
            {
                return;
            }

            statusText.text = message;
        }

        private void OnEndEdit(string value) 
        {
            videoSourceModel.path = value;
            videoSourcesPanel.SaveAll();
        }

        private void OnLoadButtonClick()
        {
            videoSourcesPanel.Dowload(videoSourceModel, this);
        }

        private void OnRemoveButtonClick() 
        {
            videoSourcesPanel.RmoveItem(videoSourceModel);
            Destroy(this.gameObject);
        }

        private void OnPosButtonClick() 
        {
            videoSourcesPanel.OpenPosWindow(videoSourceModel);
        }
    }
}