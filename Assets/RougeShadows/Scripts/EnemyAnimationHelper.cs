using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHelper : MonoBehaviour
{
    public BaseEnemy _baseEnemy;

    public void FinishUnsheath()
    {
        _baseEnemy._animator.SetBool("Unsheathed", true);
        _baseEnemy._animator.SetBool("isWalking", true);
    }

    public void Damage()
    {
        _baseEnemy._target.TakeDamage(_baseEnemy._damage);
        _baseEnemy._audioSource.PlayOneShot(_baseEnemy._attackSFX);
        //_baseEnemy._canAttack = true;
    }

}
