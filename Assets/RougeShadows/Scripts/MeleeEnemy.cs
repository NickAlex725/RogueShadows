using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    public override void Attack(Player target)
    {
        //_animator.SetTrigger("Attack");
    }

    public override void Update()
    {
        base.Update();
    }

    private void OnTriggerStay(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null && _canAttack)
        {
            _nav.isStopped = true;
            _animator.SetBool("isWalking", false);
            _animator.SetTrigger("Attack"); //damage function is in EnemyAnimationHelper.cs now
            _currentAttackCD = _attackCD;
            _canAttack = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            _animator.SetBool("isWalking", true);
            _nav.isStopped = false;
        }
    }
}
