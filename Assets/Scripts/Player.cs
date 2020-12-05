using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bool drawDebugRaycasts = true;

    [Header("Movement Properties")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float coyoteDuration = 0.05f;
    [SerializeField] private float maxFallSpeed = -25f;

    [Header("Jump Properties")]
    [SerializeField] private float jumpForce = 6.3f;
    [SerializeField] private float jumpHoldForce = 1.9f; // Добавляется в обычной высоте от прыжка
    [SerializeField] private float jumpHoldDuration = 0.1f;

    [Header("Environment Check Propeties")]
    [SerializeField] private Vector2 footOffset = new Vector2(0.4f, 1f);
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Status Flags")]
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isJumping;

    private float jumpTime;
    private float coyoteTime;

    private PlayerInput input;
    private Rigidbody2D rigidBody;

    private float originalXScale;
    private int direction = 1;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();

        originalXScale = transform.localScale.x;
    }

    private void FixedUpdate()
    {
        PhysicsCheck();

        GroundMovement();
        MidAirMovement();
    }

    #region StatuHandle

    private void PhysicsCheck()
    {
        isOnGround = false;

        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset.x, footOffset.y), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset.x, footOffset.y), Vector2.down, groundDistance);

        if (leftCheck || rightCheck)
            isOnGround = true;
    }
    #endregion

    #region Movement
    private void GroundMovement()
    {
        float xVelocity = speed * input.horizontal;

        if (xVelocity * direction < 0f)
            FlipDirection();

        rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);

        if (isOnGround)
            coyoteTime = Time.time + coyoteDuration;
    }

    private void MidAirMovement()
    {
        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time))
        {
            isOnGround = false;
            isJumping = true;

            jumpTime = Time.time + jumpHoldDuration;

            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        else if (isJumping)
        {
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            if (jumpTime <= Time.time)
                isJumping = false;
        }


        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
    }
    #endregion

    private void FlipDirection()
    {
        direction *= -1;

        Vector3 scale = transform.localScale;

        scale.x = originalXScale * direction;

        transform.localScale = scale;
    }

    #region Raycasting
    private void SetJumpVelocityZero() => rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);

    private RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length) => Raycast(offset, rayDirection, length, groundLayer);
    private RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        if (drawDebugRaycasts)
        {
            Color color = hit ? Color.red : Color.green;

            Debug.DrawRay(pos + offset, rayDirection * length, color);
        }

        return hit;
    }
    #endregion
}
