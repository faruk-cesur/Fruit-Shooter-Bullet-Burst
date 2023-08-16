using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [field: SerializeField] public float StartingHealth { get; set; }
    public UnityAction OnDeath;

    public bool IsDead { get; set; }
    private float _currentHealth;

    public float CurrentHealth
    {
        get => _currentHealth;

        set
        {
            if (IsDead)
                return;

            _currentHealth = value;

            if (IsHealthLowerThanZero())
            {
                Kill();
            }
        }
    }

    private bool IsHealthLowerThanZero()
    {
        return _currentHealth <= 0;
    }

    private void Start()
    {
        SetStartingHealth();
    }

    private void SetStartingHealth()
    {
        CurrentHealth = StartingHealth;
    }

    public void Damage(float damageAmount)
    {
        if (IsDead)
            return;

        CurrentHealth -= damageAmount;
    }

    public void Heal(float healAmount)
    {
        if (IsDead)
            return;

        CurrentHealth += healAmount;
        if (CurrentHealth > StartingHealth)
        {
            CurrentHealth = StartingHealth;
        }
    }

    private void Kill()
    {
        if (IsDead)
            return;

        IsDead = true;
        _currentHealth = 0;
        OnDeath?.Invoke();
    }
}