using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class VideoSourceModel 
{
    public string path = "";
    public string id = "";
    public string name = "";
    public List<CamPoseModel> points;
}
