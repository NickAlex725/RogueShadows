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
    [SerializeField] protected int _detectionRadius = 3;

    [Header("References")]
    [SerializeField] GameObject _shadowTriggerBox;
    [SerializeField] Transform _shadowDropPosition;
    [SerializeField] protected ParticleSystem _deathVFX;
    [SerializeField] protected AudioClip _deathSFX;
    [SerializeField] protected AudioClip _attackSFX;
    protected AudioSource _audioSource;
    protected Player _target;
    protected NavMeshAgent _nav;
    protected Health _health;

    //enemy info
    protected bool _canAttack = false;
    protected float _currentAttackCD;

    protected WaveSpawner _waveSpawner;

    private void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        _health = GetComponent<Health>();
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _waveSpawner = FindAnyObjectByType<WaveSpawner>();
        _audioSource = GetComponent<AudioSource>();
    }

    public virtual void Update()
    {
        trackPlayer();
    }

    public abstract void Attack(Player target);

    public virtual void TakeDamage(int damage)
    {
        if (_health.TakeDamage(damage) <= 0)
        {
            Instantiate(_deathVFX, transform.position, Quaternion.identity);
            Instantiate(_shadowTriggerBox, _shadowDropPosition.position, Quaternion.identity);
            Destroy(gameObject);
            if(_waveSpawner != null)
            {
                _waveSpawner.RemoveEnemy();
            }
            _audioSource.PlayOneShot(_deathSFX);
            //Play death anim
            //Destroy gameobject in a animation event
        }
    }

    private void trackPlayer()
    {
        if (Vector3.Distance(this.gameObject.transform.position, _target.transform.position) <= _detectionRadius)
        {
            _nav.isStopped = false;
            _nav.SetDestination(_target.transform.position);
            transform.LookAt(_target.transform);
            if (!_canAttack)
            {
                _currentAttackCD -= Time.deltaTime;
                if (_currentAttackCD <= 0)
                {
                    _canAttack = true;
                }
            }
        }
        else
        {
            _nav.isStopped = true;
        }

    }
}
