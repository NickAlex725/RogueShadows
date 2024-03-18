using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [SerializeField] private Transform _barrel;
    [SerializeField] private ParticleSystem _gunVFX;

    public override void Attack(Player target)
    {
        _audioSource.PlayOneShot(_attackSFX);
        _currentAttackCD = _attackCD;
        _canAttack = false;
        Instantiate(_gunVFX, _barrel.position, Quaternion.identity);
        target.TakeDamage(_damage);
    }

    public override void Update()
    {
        base.Update();

        if(_nav.remainingDistance <= _nav.stoppingDistance && _canAttack)
        {
            Attack(_target);
        }else
        {
            _nav.isStopped = false;
        }
    }
}
