using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float _moveSpeed = 1;
 //   [SerializeField] private float _sprintSpeed;
 //   [SerializeField] private float _jumpPower;
    [SerializeField] private float _dashStrength = 0.05f;
    [SerializeField] private int _dashDamage;
    [SerializeField] private float _dashDuration = 0.05f;
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

    //raycast
    [SerializeField] private LayerMask groundMask;
    private Camera mainCamera;

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
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ApplyMovement();
        Aim();
        ApplyGravity();

        //RMB Dash
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Dash());
        }
    }

    /*
    public void ShadowDash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        StartCoroutine(Dash());
    }
    */

    private IEnumerator Dash()
    {
        _canBeDamaged = false;
        _canDoDamage = true;
        _anim.SetBool("Dashing", true);
        _trail.emitting = true;
        Instantiate(_dashVFX, _VFXTransform.position, Quaternion.identity);
        for (float i = 0; i < _dashDuration; i++)
        {
            _characterController.Move(_direction * _dashStrength);
            yield return new WaitForSeconds(0.01f);
        }
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
            _direction = position - transform.position;

            //no janky rotations when hovering over player
            _direction.y = 0;

            transform.forward = _direction;
        }
    }
    //end of Raycasting code

    //LMB Move towards mouse/raycast distance
    private void ApplyMovement()
    {
        if (Input.GetMouseButton(0))
        {
           _characterController.Move(_direction * Time.deltaTime * _moveSpeed);
        }
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
