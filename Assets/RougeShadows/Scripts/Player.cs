using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int _moveSpeed = 3;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _dashStrength = 0.05f;
    [SerializeField] private int _dashDamage;
    [SerializeField] private float _dashDuration = 0.05f;
    [SerializeField] private AudioClip _dashInSFX;
    [SerializeField] private AudioClip _dashOutSFX;
    [SerializeField] private float _insideShadowMoveSpeed = 10;
    [SerializeField] private float _gravityMultiplier = 3.0f;
    [SerializeField] private bool _enableSprint;
    [SerializeField] private bool _enableJump;

    [Header("References")]
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _VFXTransform;
    [SerializeField] private ParticleSystem _dashVFX;
    [SerializeField] private GameObject _deathUI;
    private AudioSource _audioSource;
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
    private Vector3 _mouseDirection;

    //player info
    private Health _health;
    public bool canDash = false;
    public bool shadowSpeedUp = false;
    private bool _canDoDamage = false;
    private bool _canBeDamaged = true;

    //Player UI
    public Image _healthBarImage;
    public float _healthBarAmount;
    [SerializeField] private float _healthBarMulti;

    //raycast
    [SerializeField] private LayerMask groundMask;
    private Camera mainCamera;

    private void OnTriggerEnter(Collider other)
    {
        if(_canDoDamage)
        {
            var Mtarget = other.GetComponent<MeleeEnemy>();
            var Rtarget = other.GetComponent<RangedEnemy>();
            if (Mtarget != null)
            {
                Mtarget.TakeDamage(_dashDamage);
            }
            if (Rtarget != null)
            {
                Rtarget.TakeDamage(_dashDamage);
            }
        }
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _trail = GetComponent<TrailRenderer>();
        _health = GetComponent<Health>();
        _audioSource = GetComponent<AudioSource>();
        _currentMoveSpeed = _moveSpeed;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
        ApplyGravity();
        Aim();
    }

    public void ShadowDash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if(canDash)
        {
            StartCoroutine(Dash());
        }
    }
    
    private IEnumerator Dash()
    {
        _audioSource.PlayOneShot(_dashInSFX);
        _input.DeactivateInput();
        _canBeDamaged = false;
        _canDoDamage = true;
        _anim.SetBool("Dashing", true);
        _trail.emitting = true;
        Instantiate(_dashVFX, _VFXTransform.position, Quaternion.identity);
        for (float i = 0; i < _dashDuration; i++)
        {
            transform.forward = _mouseDirection;
            _characterController.Move(_mouseDirection.normalized * _dashStrength);
            yield return new WaitForSeconds(0.01f);
        }
        _audioSource.PlayOneShot(_dashOutSFX);
        _input.ActivateInput();
        _canBeDamaged = true;
        _canDoDamage = false;
        _anim.SetBool("Dashing", false);
        _trail.emitting = false;
        Instantiate(_dashVFX, _VFXTransform.position, Quaternion.identity);
    }

    //Raycasting
    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }

    }

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            _mouseDirection = position - transform.position;

            //no janky rotations when hovering over player
            _mouseDirection.y = 0;

            //transform.forward = _mouseDirection;
        }
    }
    //end of Raycasting code

    public void Move(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
        _direction = new Vector3(_move.x, 0.0f, _move.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(_enableJump)
        {
            if (!context.started) return;
            if (!_characterController.isGrounded) return;

            _velocity += _jumpPower;
        }
    }
/*
    public void Sprint(InputAction.CallbackContext context)
    {
        if(_enableSprint)
        {
            if (context.started)
            {
                _currentMoveSpeed = _sprintSpeed;
            }
            else if (context.canceled)
            {
                _currentMoveSpeed = _moveSpeed;
            }
        }
    }
*/
    private void ApplyMovement()
    {
        if (shadowSpeedUp)
        {
            _currentMoveSpeed = _insideShadowMoveSpeed;
        }
        else
        {
            _currentMoveSpeed = _moveSpeed;
        }
        _characterController.Move(_direction * Time.deltaTime * _currentMoveSpeed);
    }

    private void ApplyRotation()
    {
        if (_move.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currnetVelocity, 0.05f);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
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
                _deathUI.gameObject.SetActive(true);

            }
            _healthBarAmount -= damage;
            _healthBarImage.fillAmount = _healthBarAmount / _healthBarMulti;
        }
    }


}
