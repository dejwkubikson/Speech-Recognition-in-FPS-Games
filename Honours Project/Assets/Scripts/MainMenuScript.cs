using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject gameControlPrefab;
    GameControl gameControl;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameControlObject = GameObject.FindGameObjectWithTag("GameController");
        // This prevents GameControl from duplicating
        if (gameControlObject == null)
        {
            gameControlObject = Instantiate(gameControlPrefab);
            /*gameControlObject = new GameObject("GameController");
            gameControlObject.AddComponent<GameControl>();*/
        }

        // Making the mouse visible.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameControl = gameControlObject.GetComponent<GameControl>();

        GameObject.Find("Toggle").GetComponent<Toggle>().isOn = gameControl.speechRecognition;

        Text gameStatus = GameObject.Find("GameStatus").GetComponent<Text>();

        gameStatus.text = "";

        if(gameControl != null)
        {
            if (gameControl.gameLost)
            {
                gameStatus.text = "You lost!";
            }
            else if (gameControl.gameWon)
            {
                gameStatus.text = "You won!";
            }

            GameObject.Find("GameStatusInfo").GetComponent<Text>().text = gameControl.gameStatusInfo;
        }
    }

    private void Update()
    {
        //Cursor.visible = true;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SpeechRecognitionToggleChange(bool value)
    {
        if(gameControl != null)
            gameControl.speechRecognition = value;
    }
}
