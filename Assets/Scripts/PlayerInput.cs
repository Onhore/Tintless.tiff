using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool jumpHeld;
    [HideInInspector] public bool jumpPressed;

    private bool readyToClear;

    private void Update()
    {
        ClearInputs();

        PrecessInputs();

        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
    }

    private void FixedUpdate()
    {
        readyToClear = true;
    }

    private void ClearInputs()
    {
        if (!readyToClear)
            return;

        horizontal = 0f;
        jumpHeld = false;
        jumpPressed = false;

        readyToClear = false;
    }

    private void PrecessInputs()
    {
        horizontal += Input.GetAxis("Horizontal");

        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        jumpHeld = jumpHeld || Input.GetButton("Jump");
    }
}
