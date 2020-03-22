using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadItem 
{
    public string id;
    public string path;

    public DownloadItem(string id, string path) 
    {
        this.id = id;
        this.path = path;
    }
}
