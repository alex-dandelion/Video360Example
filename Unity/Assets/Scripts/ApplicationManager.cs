using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public bool blockTouch = false;

    [HideInInspector]
    public float lastClickTime = 0;
}