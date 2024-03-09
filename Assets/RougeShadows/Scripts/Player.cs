using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [SerializeField] private GameObject _art;
    [SerializeField] private int _dashStrength;

    private Rigidbody _rb;
    private bool _canDash = true; //set to true for testing purposes
    private bool _isDashing;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnShadowDash(InputValue value)
    {
        Debug.Log("Dashing");
        if(_canDash)
        {
            _isDashing = true;
            _rb.AddForce(Vector3.forward * _dashStrength, ForceMode.Force);
        }
    }

    public void OnMove(InputValue value)
    {
        Debug.Log("moving");
    }

    public void OnLook(InputValue value)
    {
        Debug.Log("Looking");
    }

    public void OnJump(InputValue value)
    {
        Debug.Log("Jumping");
    }

    public void OnSprint(InputValue value)
    {
        Debug.Log("Sprinting");
    }
}
