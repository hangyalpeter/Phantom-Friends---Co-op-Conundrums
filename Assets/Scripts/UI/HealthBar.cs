using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    private IHealthProvider healthComponent;

    [SerializeField]
    private GameObject healthBarPrefab;
    [SerializeField]
    private GameObject canvas;
    private GameObject healthBarInstance;
    [SerializeField]
    private Vector3 healthBarPosition = new Vector3(380, -9, 0);

    private NetworkVariable<float> networkHealth = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkHealth.OnValueChanged += OnNetworkHealthChanged;
    }

    private void Start()
    {
        healthComponent = GetComponent<IHealthProvider>();

        if (IsServer)
        {
            if (healthComponent != null)
            {
                networkHealth.Value = healthComponent.MaxHealth;
                healthComponent.OnHealthChanged += SetHealthOnServer;
            }
        }
    }

    private void Update()
    {
        if (IsClient)
        {
            SetupHealthBar();
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null && IsServer)
        {
            healthComponent.OnHealthChanged -= SetHealthOnServer;
        }

        if (networkHealth != null)
        {
            networkHealth.OnValueChanged -= OnNetworkHealthChanged;
        }
    }

    private void SetHealthOnServer(float health)
    {
        networkHealth.Value = health;
    }

    private void OnNetworkHealthChanged(float oldHealth, float newHealth)
    {
        if (healthSlider != null)
        {
            if (newHealth <= 0)
            {
                Destroy(healthBarInstance);
            }

            SetHealth(newHealth);
        }
    }

    private void SetupHealthBar()
    {
        if (healthSlider == null)
        {
            canvas = GameObject.Find("Canvas");
            healthBarInstance = Instantiate(healthBarPrefab, healthBarPosition, Quaternion.identity);
            healthBarInstance.transform.SetParent(canvas.transform, false);
            healthSlider = healthBarInstance.GetComponent<Slider>();

            SetMaxHealth(healthComponent.MaxHealth);
        }
    }

    private void SetMaxHealth(float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    private void SetHealth(float health)
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }
}

