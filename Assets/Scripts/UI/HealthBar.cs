using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    //public Slider healthSlider;
    private Slider healthSlider;

    private IHealthProvider healthComponent;

    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private Vector3 healthBarPosition = new Vector3(380, -9, 0);

    void Start()
    {
        if (healthSlider == null)
        {
            canvas = GameObject.Find("Canvas");
            GameObject h = Instantiate(healthBar, healthBarPosition,  Quaternion.identity);
            h.transform.SetParent(canvas.transform, false);
            healthSlider = h.GetComponent<Slider>();
        }

        healthComponent = GetComponent<IHealthProvider>();
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged += SetHealth;
            SetMaxHealth(healthComponent.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged -= SetHealth;
        }

    }

    private void SetMaxHealth(float health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;

    }

    private void SetHealth(float health)
    {
        healthSlider.value = health;
    }


   
}
