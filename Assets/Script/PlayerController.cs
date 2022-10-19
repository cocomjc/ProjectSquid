using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    private PlayerControls playerInput;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerControls();
        playerInput.Player.Move.performed += ctx => RotatePlayer(playerInput.Player.Move.ReadValue<Vector2>());
        playerInput.Player.Move.canceled += ctx => RotatePlayer(playerInput.Player.Move.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void RotatePlayer(Vector2 joystickOrientation)
    {
        // Make the player face the direction pointed by the joystick
        if (joystickOrientation != Vector2.zero)
        {
            float angle = Mathf.Atan2(-joystickOrientation.x, joystickOrientation.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void Update()
    {
        // Move the player
        if (playerInput.Player.Move.ReadValue<Vector2>() != Vector2.zero && gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < .1f)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(playerInput.Player.Move.ReadValue<Vector2>() * 40, ForceMode2D.Impulse);
        }
    }

    public Vector2 GetDirection()
    {
        return playerInput.Player.Move.ReadValue<Vector2>();
    }
}