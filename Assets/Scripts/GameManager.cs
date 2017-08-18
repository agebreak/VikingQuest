using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script handles the logic and UI for our game. It controls how much time the player has,
// how many points they have scored, and it detects when the player has won or lost the game
public class GameManager : MonoBehaviour
{
    // This class contains a public static reference to itself. This means that it 
    // will be accessible to other classes globally, even if they don't have a 
    // reference or link to it. 
    static GameManager instance;

    [Header("Game Properties")]

    // Amount of points the player needs to lower the wall
    [SerializeField]
    int scoreToWin = 4;

    // How long the player has to reach the goal
    [SerializeField]
    float timeAmount = 60f;

    // A reference to the wall script that lowers it
    [SerializeField]
    WallMover wall;

    // A second camera that will be used to show the wall lowering to the player. This is optional
    [SerializeField]
    Camera wallCamera;

    [Header("UI Elements")]

    // The UI element that shows the amount of time
    [SerializeField]
    Text timeText;

    // The UI element that shows the player's score
    [SerializeField]
    Text collectText;

    // The panel that will pop up when the player wins	
    [SerializeField]
    GameObject winPanel;

    // The panel that will pop up when the player loses
    [SerializeField]
    GameObject lossPanel = null;

    // The player's current score
    int score;

    // Is the game over?
    bool gameover;

    public static GameManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // If there currently isn't a GameManager, make this the game manager. Otherwise,
        // destroy this object. We only want one GameManager
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        // Set our initial UI text for score and the amount of time
        collectText.text = score + " / " + scoreToWin;
        timeText.text = ((int)timeAmount).ToString();
    }

    void Update()
    {
        //Always look to see if the player is pressing the "Cancel" button (escape). If so,
        //quit the game
        if (Input.GetButtonDown("Cancel"))
            ExitGame();

        //If the game is already over, don't do anything (just leave)
        if (gameover)
            return;

        //Reduce the player's time
        timeAmount -= Time.deltaTime;

        //If the player's time is now at or below zero...
        if (timeAmount <= 0f)
        {
            //...set the time to zero...
            timeAmount = 0f;
            //...record that the game is now over...
            gameover = true;
            //...and show the Loss Panel
            lossPanel.SetActive(true);
        }

        //Update the UI to show the remaining time
        timeText.text = ((int)timeAmount).ToString();
    }

    //This method is called from the CollectableSpawner when the player picks up a shield
    public void PlayerScored()
    {
        //If the player already has enough points, leave. This prevents the score from
        //becoming larger than the needed score in the UI (for instance, 5 out of 4), and also
        //prevents us from trying to lower the wall multiple times
        if (score >= scoreToWin)
            return;

        //Increase the player's score and update the UI
        score++;
        collectText.text = score + " / " + scoreToWin;

        //If the player hasn't scored eough points yet, leave
        if (score < scoreToWin)
            return;

        //Lower the wall
        wall.LowerWall();

        //If we have a wall camera...
        if (wallCamera != null)
        {
            //...turn it on...
            wallCamera.enabled = true;
            //...and call a method the hide it again after 3 seconds
            Invoke("HideDoorCamera", 3f);
        }
    }

    //This method is called when the player enters the goal area
    public void PlayerEnteredGoalZone()
    {
        //The game is now over
        gameover = true;
        //Show the Win Panel
        winPanel.SetActive(true);
    }

    //This method is called from the Player's script. We only want the player to be
    //able to move if the game isn't over
    public bool IsGameOver()
    {
        //Return whether or not the game is over
        return gameover;
    }

    //This methid is called from the AdManager when the player watches a rewarded ad
    public void AddMoreGameTime(float amount)
    {
        //Add the given amount of time to the player's time
        timeAmount += amount;

        //If the game is already over, it is no longer over, so that the player
        //can keep trying
        if (gameover)
            gameover = false;
    }

    void HideDoorCamera()
    {
        //Hide the wall camera
        wallCamera.enabled = false;
    }

    //This method is called from the "Play Again" buttons in the UI
    public void ReloadScene()
    {
        //Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //This method allows the player to exit the game either by pressing the correct key or
    //selecting to exit the game from the UI
    public void ExitGame()
    {
        //Check to see if we are in the editor, and if we are simply stop playing
        //If we are not in the editor, then we are in a build and we need to tell the application
        //to Quit. NOTE: This does not work on all platforms. Some platforms only allow the OS to
        //terminate applications and in those cases Application.Quit() won't do anything
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
    }
}
