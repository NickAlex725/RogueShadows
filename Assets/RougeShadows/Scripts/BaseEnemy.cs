using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class BaseEnemy : MonoBehaviour
{
    [Header("Enmey Stats")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _attackCD;

    [Header("References")]
    protected GameObject _target;
    protected NavMeshAgent _nav;
    protected Health _health;

    //enemy info
    protected bool _canAttack = true;
    protected float _currentAttackCD;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if(player != null && _canAttack)
        {
            Attack(player);
        }
    }

    private void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        _health = GetComponent<Health>();
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void Update()
    {
        _nav.SetDestination(_target.transform.position);
        if(!_canAttack)
        {
            _currentAttackCD -= Time.deltaTime;
            if(_currentAttackCD <= 0)
            {
                _canAttack = true;
            }
        }
    }

    public virtual void Attack(Player target)
    {
        target.TakeDamage(_damage);
        _currentAttackCD = _attackCD;
        _canAttack = false;
    }

    public virtual void TakeDamage(int damage)
    {
        if (_health.TakeDamage(damage) <= 0)
        {
            Destroy(gameObject);
            //Play death anim
            //Destroy gameobject in a animation event
        }
    }
}
