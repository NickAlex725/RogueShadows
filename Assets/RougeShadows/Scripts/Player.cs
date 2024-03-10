using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _dashStrength;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _gravityMultiplier = 3.0f;

    [Header("References")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private GameObject _art;
    [SerializeField] private Animator _anim;
    [SerializeField] private Collider _col;
    [SerializeField] private Transform _VFXTransform;
    [SerializeField] private ParticleSystem _dashVFX;
    private TrailRenderer _trail;

    // vars for character controller
    private CharacterController _characterController;
    private Vector2 _move;
    private float _currentMoveSpeed;
    private Vector3 _direction;
    private float _currnetVelocity;
    private float _velocity;
    private float _gravity = -9.81f;

    //vars for player status
    private bool _isAlive = true;
    private Health _health;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _trail = GetComponent<TrailRenderer>();
        _health = GetComponent<Health>();
        _col = GetComponent<Collider>();
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
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        _col.enabled = false;
        _anim.SetBool("Dashing", true);
        _trail.emitting = true;
        Instantiate(_dashVFX, _VFXTransform.position, Quaternion.identity);
        for (float i = 0; i < _dashDuration; i++)
        {
            _characterController.Move(_direction * _dashStrength);
            yield return new WaitForSeconds(0.01f);
        }
        Instantiate(_dashVFX, _VFXTransform.position, Quaternion.identity);
        _trail.emitting = false;
        _anim.SetBool("Dashing", false);
        _col.enabled = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
        _direction = new Vector3(_move.x, 0.0f, _move.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        /* disabling jumping since we don't plan on using it for now
        if (!context.started) return;
        if (!_characterController.isGrounded) return;

        _velocity += _jumpPower;
        */
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
        if(_health.TakeDamage(damage) <= 0)
        {
            //player death
            _anim.SetBool("isAlive", false);
            _isAlive = false;
            _input.DeactivateInput();
        }
    }
}
