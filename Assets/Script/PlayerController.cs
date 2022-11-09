using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private PlayerControls playerInput;
    private MoveState moveState;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject leftRaycast;
    [SerializeField] private GameObject rightRaycast;
    [SerializeField] private GameModulator gameModulator;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerControls();
        playerInput.Player.Move.performed += ctx => MovePlayer(playerInput.Player.Move.ReadValue<Vector2>());
        playerInput.Player.Move.canceled += ctx => MovePlayer(playerInput.Player.Move.ReadValue<Vector2>());
        moveState = MoveState.Swimming;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void MovePlayer(Vector2 joystickOrientation)
    {
        if (moveState == MoveState.Swimming)
        {
            // Make the player face the direction pointed by the joystick
            if (joystickOrientation != Vector2.zero)
            {
                float angle = Mathf.Atan2(-joystickOrientation.x, joystickOrientation.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
        else
        {
            //Make the player look at the opposite of the wall he is fixed to
        }
    }

    private void Update()
    {
        // Move the player
        if (moveState == MoveState.Swimming)
        {
            if (playerInput.Player.Move.ReadValue<Vector2>() != Vector2.zero && gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < gameModulator.swimSpeedBeforeNextMove)
            {
                gameObject.GetComponent<Rigidbody2D>().AddForce(playerInput.Player.Move.ReadValue<Vector2>() * gameModulator.swimPropulsionPower, ForceMode2D.Impulse);
            }
        }
        else
        {
            // Throwing 2 rays to check if the player is aligned with a wall inclinasion
            RaycastHit2D leftRay = Physics2D.Raycast(leftRaycast.transform.position, -transform.up, 100f, layerMask);
            RaycastHit2D rightRay = Physics2D.Raycast(rightRaycast.transform.position, -transform.up, 100f, layerMask);
            Debug.DrawRay(leftRaycast.transform.position, -transform.up, Color.red);
            Debug.DrawRay(rightRaycast.transform.position, -transform.up, Color.magenta);

            //Debug.Log("Left: " + Vector2.Distance(leftRaycast.transform.position, leftRay.point) + " | Right: " + Vector2.Distance(rightRaycast.transform.position, rightRay.point));
            Vector3 dir = (leftRay.point - rightRay.point).normalized;
            Vector3 playerAlignement = (leftRaycast.transform.position - rightRaycast.transform.position).normalized;
            Debug.Log("dir: " + dir + " || alignement: " + playerAlignement);
            Debug.DrawRay(rightRay.point, dir, Color.green);

            // Rotate the player if he is not aligned with the wall
            if (Math.Round(dir.x, 1) != Math.Round(playerAlignement.x, 1) || Math.Round(dir.y, 1) != Math.Round(playerAlignement.y, 1))
            {
                /*                transform.rotation = Quaternion.LookRotation(dir) * Quaternion.FromToRotation(Vector3.right, Vector3.forward);
                                Vector3 eulerRotation = transform.rotation.eulerAngles;
                                transform.rotation = Quaternion.Euler(0, 0, eulerRotation.z > 0 ? -eulerRotation.z : eulerRotation.z);
                */
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10);
            }

            // Move the player closer to the wall if he is to far
            if (Vector2.Distance(leftRaycast.transform.position, leftRay.point) > gameModulator.grabDistanceWhileMove + .1)
            {
                transform.Translate(-transform.up * 5f * Time.deltaTime, Space.World);
                Debug.Log("getting closer to the wall");
                Debug.DrawRay(transform.position, -transform.up, Color.red);
            }
            // Move the player farther to the wall if he is to close
            else if (Vector2.Distance(leftRaycast.transform.position, leftRay.point) < gameModulator.grabDistanceWhileMove) {
                transform.Translate(transform.up * 5f * Time.deltaTime, Space.World);
                Debug.Log("getting farther to the wall");
                Debug.DrawRay(transform.position, transform.up, Color.red);
            }
            transform.Translate(new Vector3(playerInput.Player.Move.ReadValue<Vector2>().x, 0, 0) * gameModulator.grabMoveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            //SetMoveState(MoveState.Grabbing);
        }
    }

    public void ToggleMoveState()
    {
        if (this.moveState == MoveState.Swimming)
            this.moveState = MoveState.Grabbing;
        else
            this.moveState = MoveState.Swimming;
    }

    public enum MoveState
    {
        Swimming,
        Grabbing
    }
}