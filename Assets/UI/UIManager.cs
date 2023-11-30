using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #pragma warning disable 0414

    /// Children
    [Header("LevelName")]
    [SerializeField]
    private Text levelName = null;

    [Header("Enemy Counter")]
    [SerializeField]
    private Text enemyCounter = null;

    /// Singleton thing
    private static UIManager _instance;
    public  static UIManager instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)  _instance = this;
        else                    Destroy(gameObject);
    }

    private void Start() {
        GameManager.instance.onVictory += OnVictory;
    }


    public void UpdateEnemyCounter(int count)
    {
        enemyCounter.text = count.ToString();
    }

    private void OnVictory()
    {
        enemyCounter.text = "Victory!";
    }

    public void SetLevelName(string l)
    {
        levelName.text = l;
    }


}
