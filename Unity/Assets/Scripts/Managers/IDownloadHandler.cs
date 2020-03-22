public interface IDownloadHandler
{
    void OnDone(DownloadItem resource, string message);
    void OnProgress(DownloadItem resource, float progressValue);
}
