using System;

public interface IHealthProvider
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
    event Action<float> OnHealthChanged;
    static event Action OnEnemyDied;
    event Action OnDied;

}


