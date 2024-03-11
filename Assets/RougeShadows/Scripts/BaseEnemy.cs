using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Enmey Stats")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _attackCD;
    [SerializeField] protected ParticleSystem _deathVFX;

    [Header("References")]
    [SerializeField] GameObject _shadowTriggerBox;
    [SerializeField] Transform _shadowDropPosition;
    protected Player _target;
    protected NavMeshAgent _nav;
    protected Health _health;

    //enemy info
    protected bool _canAttack;
    protected float _currentAttackCD;

    private void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        _health = GetComponent<Health>();
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public virtual void Update()
    {
        _nav.SetDestination(_target.transform.position);
        transform.LookAt(_target.transform);
        if(!_canAttack)
        {
            _currentAttackCD -= Time.deltaTime;
            if(_currentAttackCD <= 0)
            {
                _canAttack = true;
            }
        }
    }

    public abstract void Attack(Player target);

    public virtual void TakeDamage(int damage)
    {
        if (_health.TakeDamage(damage) <= 0)
        {
            Instantiate(_deathVFX, transform.position, Quaternion.identity);
            Instantiate(_shadowTriggerBox, _shadowDropPosition.position, Quaternion.identity);
            Destroy(gameObject);
            //Play death anim
            //Destroy gameobject in a animation event
        }
    }
}
