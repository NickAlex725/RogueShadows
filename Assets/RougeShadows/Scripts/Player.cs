using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private int _moveSpeed;
    [SerializeField] private int _sprintSpeed;
    [SerializeField] private int _jumpPower;
    [SerializeField] private int _dashStrength;
    [SerializeField] private float _gravityMultiplier = 3.0f;

    [Header("References")]
    [SerializeField] private GameObject _art;

    private CharacterController _characterController;
    private Vector2 _move;
    private int _currentMoveSpeed;
    private Vector3 _direction;
    private float _currnetVelocity;
    private float _velocity;
    private float _gravity = -9.81f;
    private bool _canDash = true; //set to true for testing purposes
    private bool _isDashing;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _currentMoveSpeed = _moveSpeed;
    }

    private void Update()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    public void OnShadowDash(InputAction.CallbackContext context)
    {
        Debug.Log("Dashing");
        if(_canDash)
        {
            _isDashing = true;
        }
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
}
