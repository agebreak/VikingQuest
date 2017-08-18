﻿using UnityEngine;

// This script is used to control the player with keyboard / controller mechanics. It is mostly meant 
// for playing the game on Standalone or Console
public class Player_ControllerMove : MonoBehaviour
{
    // The speed of the player
    [SerializeField]
    float movementSpeed = 5.0f;

    // The turn speed of the player
    [SerializeField]
    float turnSpeed = 1000f;

    // The player's animator component
    Animator animator;

    // The player's rigidbody component
    new Rigidbody rigidbody;

    // The player's x, y, and z controller input
    Vector3 playerInput;

    void Start()
    {
        // Get references to the player's rigidbody and animator components
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // If the GameManager exists AND it tells us that the game is over, leave
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            //Go back to our Idle animation by dropping the speed to 0
            animator.SetFloat("Speed", 0f);
            return;
        }

        // Get the horizontal and vertical input (up/down/left/right arrows, WASD keys, controller analog stick, etc),
        // and store that input in our playerInput variable (there won't be any "y" input)
        playerInput.Set(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Tell the animator the "speed" of our movement based on the magnitude 
        // of the vector (the numerical "value" of the vector)
        animator.SetFloat("Speed", playerInput.sqrMagnitude);

        // If there is no input from the player, we're done here and can leave
        if (playerInput == Vector3.zero)
            return;

        // We take our input, multiply it by our speed, and then multiply Time.deltaTime. We then
        // add this amount to our current position to get the new desired position. NOTE: We "normalize"
        // our input so that the player won't move faster going diagonolly. NOTE: multiplying the value
        // with Time.deltaTime ensures that everyone has the same gameplay regardless of the speed of their
        // computers or the physics settings of the game
        Vector3 newPosition = transform.position + playerInput.normalized * movementSpeed * Time.deltaTime;

        // Use the rigidbody to move to the new position. This is better than Transform.Translate since it means
        // the player will move with physics and force instead of just "teleporting" to the new spot
        rigidbody.MovePosition(newPosition);

        // Use the "Quaternion" class to determine the rotation we need to face the direction we want to go
        Quaternion newRotation = Quaternion.LookRotation(playerInput);

        // If we need to turn and face a new direction, use the RotateTowards() method to turn quickly, but not
        // instantly (which looks better)
        if (rigidbody.rotation != newRotation)
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, newRotation, turnSpeed * Time.deltaTime);
    }
}
