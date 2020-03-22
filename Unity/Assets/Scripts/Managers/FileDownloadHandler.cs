using System.IO;
using UnityEngine.Networking;


public class FileDownloadHandler : DownloadHandlerScript
{
    private int expected = -1;
    private int received = 0;
    private string filepath;
    private FileStream fileStream;
    private bool canceled = false;

    public FileDownloadHandler(byte[] buffer, string directoryName, string fileName)
      : base(buffer)
    {
        this.filepath = directoryName + fileName;
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
    }

    protected override byte[] GetData() { return null; }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || data.Length < 1)
        {
            return false;
        }
        received += dataLength;
        if (!canceled) 
            fileStream.Write(data, 0, dataLength);

        if(received == expected) 
        {
            fileStream.Close();
        }
        return true;
    }

    protected override float GetProgress()
    {
        if (expected < 0) return 0;
        return (float)received / expected;
    }

    public float Progress
    {
        get
        {
            return (float)received / expected;
        }
    }

    protected override void CompleteContent()
    {
        fileStream.Close();
    }

    protected override void ReceiveContentLength(int contentLength)
    {
        UnityEngine.Debug.Log("ReceiveContentLength " + contentLength );
        expected = contentLength;
    }

    public void Cancel()
    {
        canceled = true;
        fileStream.Close();
        File.Delete(filepath);
    }
}
