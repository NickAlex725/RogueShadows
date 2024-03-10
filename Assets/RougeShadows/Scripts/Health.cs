using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int _maxHealth;
    private int _minHealth = 0;

    public int _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void GainHealth(int healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public int TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, _minHealth, _maxHealth); //makes sure health doesnt go past the max and min
        return _currentHealth;
    }
}
