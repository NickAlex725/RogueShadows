using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    public override void Attack(Player target)
    {
        _audioSource.PlayOneShot(_attackSFX);
        target.TakeDamage(_damage);
        _currentAttackCD = _attackCD;
        _canAttack = false;
    }

    public override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null && _canAttack)
        {
            Attack(player);
        }
    }
}
