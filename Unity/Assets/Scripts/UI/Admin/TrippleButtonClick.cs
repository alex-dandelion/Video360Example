using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TrippleButtonClick : MonoBehaviour
{
    [SerializeField] private float timeoutBetweenClick = 0.5f;
    [SerializeField] private GameObject goToEnable;

    private float lastClickTime = -1;
    private int clickTimes = 0;

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClcik);
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    private void OnButtonClcik() 
    {
        if (Time.time > lastClickTime + timeoutBetweenClick)
            clickTimes = 0;
        else
            clickTimes++;

        lastClickTime = Time.time;

        if (clickTimes > 2) 
        {
            goToEnable.SetActive(true);
            clickTimes = 0;
            gameObject.SetActive(false);
       }
    }
}
