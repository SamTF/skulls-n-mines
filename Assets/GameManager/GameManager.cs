using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player player = null;
    [SerializeField]
    private float explosionRadius = 1f;
    [SerializeField]
    private float holeRadius = 1f;
    [SerializeField]
    private Transform holesParent = null;
    
    /// Events
    public event Action onPlayerDeath;
    public event Action onVictory;

    /// Player Stats
    private bool isPlayerAlive = true;
    private bool gameStarted = false;

    // Level stuff
    private string levelName = "";
    private int lastLevel = 4;
    private int enemiesTotal;
    private int enemiesRemaining;

    /// Singleton thing
    private static GameManager _instance = null;
    public static GameManager instance
    {
        get {return _instance;}
    }

    private void Awake()
    {
        // Singleton Thing
        if (instance == null)   { _instance = this; }
        else                    { Destroy(gameObject); }
    }

    private void Start() {
        Time.timeScale = 1f;
        levelName = SceneManager.GetActiveScene().name;
        UIManager.instance.SetLevelName(levelName);
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.Period))
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            if(currentLevel == lastLevel)   return;
            SceneManager.LoadScene(currentLevel += 1);
        }
        if (Input.GetKey(KeyCode.Comma))
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            if(currentLevel == 0)   return;
            SceneManager.LoadScene(currentLevel -= 1);
        }
        if (Input.GetKey(KeyCode.R))
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentLevel);
        }
        if (Input.GetKey(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioManager.instance.ToggleMute();
        }
    }

    /// Events
    // When the Player Dies
    public void OnPlayerDeath()
    {
        if(!isPlayerAlive)      return;

        isPlayerAlive = false;
        onPlayerDeath.Invoke();
        StartCoroutine(RestartLevel());
    }

    /// Level loading stuff
    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3f);

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel);
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if(currentLevel == lastLevel)   yield break;

        SceneManager.LoadScene(currentLevel += 1);
    }


    public void SetEnemyTotal(int total)
    {
        enemiesTotal = total;
        enemiesRemaining = enemiesTotal;

        UIManager.instance.UpdateEnemyCounter(enemiesRemaining);
    }

    public void OnSwarmerDeath()
    {
        enemiesRemaining--;
        UIManager.instance.UpdateEnemyCounter(enemiesRemaining);

        if(enemiesRemaining <= 0)
        {
            OnVictory();
        }
    }

    private void OnVictory()
    {
        onVictory.Invoke();
        StartCoroutine(LoadNextLevel());
    }

    /// Public Getters
    //  Player
    public Vector3   PlayerPosition     => player ? player.transform.position  : Vector3.zero;
    public Transform PlayerTransform    => player ? player.transform           : transform;
    public bool      IsPlayerAlive      => isPlayerAlive;
    // Explosions & Holes
    public float     RadiusExplosion    => explosionRadius;
    public float     RadiusHole         => holeRadius;
    public Transform HolesParent        => holesParent;

    public float GetAngleToPlayer(Vector2 postion)
    {
        if (player == null) return 0f;  // To prevent errors when the player has been destroyed

        Vector2 playerPos = player.transform.position;
        float angleBetween = (Mathf.Atan2(playerPos.y - postion.y, playerPos.x - postion.x) * 180 / Mathf.PI);

        return angleBetween;
    }
}
