using System;

public interface IHealthProvider
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
    public event Action<float> OnHealthChanged;
    static event Action OnEnemyDied;
    public event Action OnDied;
    public void TakeDamage(float damage);

}


