using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemCollector : MonoBehaviour
{

    public static ItemCollector Instance { get; private set; }

    private int appplesCount = 0;
    [SerializeField] private TextMeshProUGUI applesCountText;
    [SerializeField] private AudioSource appleSound;

    private int totalApplesCount = 0;
    private void OnEnable()
    {
        CollectibleApple.OnCollected += CollectApple;
        GameEvents.LevelFinished += SaveAppleCountAndSetStars;
    }

    private void OnDisable()
    {
        CollectibleApple.OnCollected -= CollectApple;
        GameEvents.LevelFinished -= SaveAppleCountAndSetStars;
    }

    private void SaveAppleCountAndSetStars()
    {
        var currentLevelName = SceneManager.GetActiveScene().name;
        var stars = 0;
        var collectedApplesRatio = (float)appplesCount / (float)totalApplesCount;
        if (collectedApplesRatio >= 0.8)
        {
            stars = 3;
        }
        else if (collectedApplesRatio < 0.8 && collectedApplesRatio >= 0.6)
        {
            stars = 2;
        }
        else
        {
            stars = 1;
        }
        if (stars > PlayerPrefs.GetInt(currentLevelName) )
        {
            PlayerPrefs.SetInt(currentLevelName, stars);
            PlayerPrefs.Save();
            GameEvents.StarsChanged?.Invoke(currentLevelName, stars);
        }
    }

    private void CollectApple()
    {
        appplesCount++;
        appleSound.Play();
        applesCountText.text = "Apples: " + appplesCount.ToString();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        totalApplesCount = GameObject.FindGameObjectsWithTag("Apple").Count();
    }



}

