using UnityEngine;

// This script detects when the player reaches the goal zone and then tells
// the Game Manager about it
public class GoalZone : MonoBehaviour
{
    // Is the goal currently active?
    bool isActive = true;

    void OnTriggerEnter(Collider other)
    {
        // If the goal isn't currently active OR the object entering 
        // the goal isn't the player, leave
        if (!isActive || !other.CompareTag("Player"))
            return;

        // Since the player entered the goal, it is no longer active. This
        // prevents us from trying to win multiple times
        isActive = false;

        // If the GameManager exists, tell it that the player entered the goal
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerEnteredGoalZone();
    }
}
