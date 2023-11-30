using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private Text timerText = null;
    [SerializeField]
    private bool isActive = false;
    
    private float timeCounter = 0f;
    
    
    /// Singleton thing
    private static Timer _instance;
    public static Timer instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)  _instance = this;
        else                    Destroy(gameObject);
    }

    private void Update()
    {
        if (isActive)   timeCounter += Time.deltaTime;

        timerText.text = timeCounter.ToString("F2");
    }

    public void SetActive(bool active) => isActive = active;

    public float TimeCounter => timeCounter;
}

