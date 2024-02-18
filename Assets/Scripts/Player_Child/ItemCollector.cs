using TMPro;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{

    public static ItemCollector Instance { get; private set; }

    private int appplesCount = 0;
    [SerializeField] private TextMeshProUGUI applesCountText;
    [SerializeField] private AudioSource appleSound;

    private void OnEnable()
    {
        CollectibleApple.OnCollected += CollectApple;
    }

    private void OnDisable()
    {
        CollectibleApple.OnCollected -= CollectApple;
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


    
}

