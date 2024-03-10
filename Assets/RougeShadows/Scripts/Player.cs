using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _dashStrength;
    [SerializeField] private int _dashDamage;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _gravityMultiplier = 3.0f;

    [Header("References")]
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _VFXTransform;
    [SerializeField] private ParticleSystem _dashVFX;
    private PlayerInput _input;
    private TrailRenderer _trail;

    //character controller
    private CharacterController _characterController;
    private Vector2 _move;
    private float _currentMoveSpeed;
    private Vector3 _direction;
    private float _currnetVelocity;
    private float _velocity;
    private float _gravity = -9.81f;
    private Vector2 _mouseDirection;

    //player info
    private Health _health;
    private bool _canDoDamage = false;
    private bool _canBeDamaged = true;

    private void OnTriggerEnter(Collider other)
    {
        if(_canDoDamage)
        {
            var target = other.GetComponent<MeleeEnemy>();
            if (target != null)
            {
                target.TakeDamage(_dashDamage);
            }
        }
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _trail = GetComponent<TrailRenderer>();
        _health = GetComponent<Health>();
        _currentMoveSpeed = _moveSpeed;
    }

    private void Update()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    public void ShadowDash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        _mouseDirection.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        _mouseDirection.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).z;
        _mouseDirection.x -= transform.position.x;
        _mouseDirection.y -= transform.position.z;
        _mouseDirection.Normalize();
        Debug.Log(_mouseDirection);
        //StartCoroutine(Dash(_mouseDirection));
    }

    private IEnumerator Dash(Vector3 dashDir)
    {
        _canBeDamaged = false;
        _canDoDamage = true;
        _anim.SetBool("Dashing", true);
        _trail.emitting = true;
        Instantiate(_dashVFX, _VFXTransform.position, Quaternion.identity);
        for (float i = 0; i < _dashDuration; i++)
        {
            _characterController.Move(dashDir * _dashStrength);
            yield return new WaitForSeconds(0.01f);
        }
        _canBeDamaged = true;
        _canDoDamage = false;
        _anim.SetBool("Dashing", false);
        _trail.emitting = false;
        Instantiate(_dashVFX, _VFXTransform.position, Quaternion.identity);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
        _direction = new Vector3(_move.x, 0.0f, _move.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_characterController.isGrounded) return;

        _velocity += _jumpPower;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _currentMoveSpeed = _sprintSpeed;
        }
        else if(context.canceled)
        {
            _currentMoveSpeed = _moveSpeed;
        }
    }

    private void ApplyRotation()
    {
        if (_move.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currnetVelocity, 0.05f);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    private void ApplyMovement()
    {
        _characterController.Move(_direction * _currentMoveSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if(_characterController.isGrounded && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * _gravityMultiplier * Time.deltaTime;
        }
        _direction.y = _velocity;
    }

    public void TakeDamage(int damage)
    {
        if(_canBeDamaged)
        {
            if (_health.TakeDamage(damage) <= 0)
            {
                //player death
                _anim.SetBool("isAlive", false);
                _input.DeactivateInput();
            }
            Debug.Log(damage + " Damage Taken!");
        }
    }
}
