using UnityEngine;

// This script is used to control the player with keyboard / controller mechanics. This differs from
// the Player_ControllerMove script in that this script also allows the player to jump
public class Player_FullControl : MonoBehaviour
{
    // The speed of the player
    [SerializeField]
    float movementSpeed = 5.0f;

    // The turn speed of the player
    [SerializeField]
    float turnSpeed = 1000f;

    // The force that the player jumps with
    [SerializeField]
    float jumpForce = 6f;

    // A layer mask defining what layers constitute the ground
    [SerializeField]
    LayerMask whatIsGround;

    // The player's animator component
    Animator animator;

    // The player's rigidbody component
    new Rigidbody rigidbody;

    // The player's x, y, and z controller input
    Vector3 playerInput;

    // Is the player currently on the ground?
    bool grounded = true;

    void Start()
    {
        // Get references to the player's rigidbody and animator components
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Did the player press the "Jump" button AND are they on the ground? NOTE: we check
        // for GetButtonDown() in the regular Update() instad of FixedUpdate() since FixedUpdate() 
        // runs slower and thus can miss our input. 
        if (Input.GetButtonDown("Jump") && grounded)
        {
            // Add a Y-axis force to the character
            rigidbody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            // Tell the animator to play the jumping animation
            animator.SetTrigger("Jump");
            // We are no longer on the ground
            grounded = false;
        }

        // Update the animator by telling it whether or not we are currently on the ground
        animator.SetBool("Grounded", grounded);
    }

    void FixedUpdate()
    {
        // Generate an imaginary sphere at the character's feet and see if it collides with anything on the ground layer. 
        // If it does, then we are on the ground. Otherwise we are not on the ground
        if (Physics.CheckSphere(transform.position, .1f, whatIsGround))
            grounded = true;
        else
            grounded = false;

        // If the GameManager exists AND it tells us that the game is over, leave
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            // Go back to our Idle animation by dropping the speed to 0
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
