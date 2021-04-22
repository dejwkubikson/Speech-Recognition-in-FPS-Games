using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public bool speechRecognition = true;
    public int timeToLose = 4;
    public bool gameLost = false;
    public bool gameWon = false;
    public bool exitGame = false;
    public string gameStatusInfo = "";

    public int currentMission = 0;

    public bool forceMission4 = false;
    public bool checkPointReached = false;

    AllyScript allyScript;
    EnemyGroupBehaviour enemyGroupBehaviour;

    public GameObject playerObject;
    public GameObject enemyGroup;
    public GameObject speechRecognitionObject;

    VoiceControl voiceControl;

    public bool resourcesLoaded = false;

    Scene scene;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void LoadResources()
    {
        allyScript = GameObject.FindGameObjectWithTag("Ally").GetComponent<AllyScript>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        enemyGroup = GameObject.FindGameObjectWithTag("ScriptedGroup");
        enemyGroupBehaviour = enemyGroup.GetComponent<EnemyGroupBehaviour>();

        voiceControl = GameObject.FindGameObjectWithTag("VoiceControl").GetComponent<VoiceControl>();

        speechRecognitionObject = GameObject.FindGameObjectWithTag("SpeechRecognition");
        speechRecognitionObject.SetActive(speechRecognition);
    }

    // Update is called once per frame
    void Update()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "Menu")
        {
            resourcesLoaded = false;
            if (exitGame)
            {
                ResetSettings();
                exitGame = false;
            }
            return;
        }

        if (!resourcesLoaded)
        {
            LoadResources();
            resourcesLoaded = true;
        }

        if (currentMission == 1)
        {
            if (enemyGroup.transform.position.x + 1 < playerObject.transform.position.x)
                StartMission2();
        }

        if (!speechRecognition)
        {
            if (currentMission == 2)
            {
                if(allyScript.missionOneEnemies[0].GetComponent<EnemyBehaviour>().dead == false || allyScript.missionOneEnemies[1].GetComponent<EnemyBehaviour>().dead == false)
                {
                    if (allyScript.missionOneEnemies[0].GetComponent<EnemyBehaviour>().dead)
                    {
                        allyScript.EnemyToKill(allyScript.missionOneEnemies[1]);
                        allyScript.KillEnemyOrder();
                    }
                    else if (allyScript.missionOneEnemies[1].GetComponent<EnemyBehaviour>().dead)
                    {
                        allyScript.EnemyToKill(allyScript.missionOneEnemies[0]);
                        allyScript.KillEnemyOrder();
                    }
                }
            }

            if(currentMission == 3)
            {
                if (allyScript.missionTwoEnemies[0].GetComponent<EnemyBehaviour>().dead == false || allyScript.missionTwoEnemies[1].GetComponent<EnemyBehaviour>().dead == false)
                {
                    if (allyScript.missionTwoEnemies[0].GetComponent<EnemyBehaviour>().dead)
                    {
                        allyScript.EnemyToKill(allyScript.missionTwoEnemies[1]);
                        allyScript.KillEnemyOrder();
                    }
                    else if (allyScript.missionTwoEnemies[1].GetComponent<EnemyBehaviour>().dead)
                    {
                        allyScript.EnemyToKill(allyScript.missionTwoEnemies[0]);
                        allyScript.KillEnemyOrder();
                    }
                }
            }
        }
        else
        {

            if (currentMission == 3)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Soldier");

                bool alive = false;
                // Check if Soldier 5 and 6 are dead
                for(int i = 0; i < enemies.Length; i++)
                {
                    if(enemies[i].name == "Soldier 6" || enemies[i].name == "Soldier 5")
                    {
                        if(enemies[i].GetComponent<EnemyBehaviour>().dead == false)
                        {
                            alive = true;
                        }
                    }
                }

                if (alive == false || forceMission4)
                {
                    StartMission4();
                }
            }
        }
    }

    public void ResetSettings()
    {
        // Reset all the data
        speechRecognition = true;
        timeToLose = 4;
        gameLost = false;
        gameWon = false;
        gameStatusInfo = "";

        currentMission = 0;

        forceMission4 = false;
        checkPointReached = false;
    }

    public void GameLost(string reason)
    {
        //Debug.Log("GameLost(" + reason + ")");
        gameLost = true;
        gameStatusInfo = reason;

        SceneManager.LoadScene("Menu");
    }

    public void GameWon()
    {
        //Debug.Log("GameWon()");
        gameWon = true;
        gameStatusInfo = "You have successfully killed the boss without raising the alarm.";
        SceneManager.LoadScene("Menu");
    }

    public void StartMission1()
    {
        if (currentMission >= 1)
            return;

        currentMission = 1;

        allyScript.RadioCall("Alpha, there's a group of enemies in front of you. Do not engage! I repeat do not engage, we don't stand a chance without alerting everybody.");
        enemyGroupBehaviour.moveToPoint = true;

        if(speechRecognition)
            voiceControl.DisplayPossibleCommands(new string[]{"Where are the enemies?"});
    }

    private void StartMission2()
    {
        if (currentMission >= 2)
            return;

        currentMission = 2;

        allyScript.RadioCall("Alpha, the enemy group passed you. I can see two more tangos near the campfire. " +
            "We need to kill them both at once. Your call.");

        if (speechRecognition)
        {
            voiceControl.DisplayPossibleCommands(new string[] { "Take down the enemy on the left", "Right target is yours", "Right tango is mine", "Kill the one further to me", "Closer enemy is yours" });
            voiceControl.waitForTargetSpecification = true;
        }
    }

    public void StartMission3()
    {
        if (currentMission >= 3)
            return;

        currentMission = 3;

        if (speechRecognition)
        {
            allyScript.RadioCall("You're nearly in the village, multiple tangos spotted. Two near campfire.\nGet rid of the " +
            "moving enemies and we'll deal with them at the end. Mahmed is in the main building.");
            voiceControl.DisplayPossibleCommands(new string[] { "Where are the enemies", "Deal with enemies near campfire." });
        }
        else
        {
            allyScript.RadioCall("You're nearly in the village, multiple tangos spotted. Two near campfire, I'll help you with those two. " +
                "Mahmed is in the main building on your North.");
        }
    }

    public void StartMission4()
    {
        if (currentMission >= 4)
            return;

        currentMission = 4;

        if (speechRecognition)
        {
            voiceControl.waitForTargetSpecification = true;

            if (forceMission4 == true)
            {
                allyScript.RadioCall("Roger, we'll deal with the enemies at the campfire now. Your call which one to take down.");

                if (speechRecognition)
                    voiceControl.DisplayPossibleCommands(new string[] { "I will take down the one closer to me", "Kill the one further", "Closer enemy is yours" });
            }
            else
            {
                allyScript.RadioCall("I can only see the tangos near the campfire. Your call which one to take down.");

                if (speechRecognition)
                    voiceControl.DisplayPossibleCommands(new string[] { "I will take down the one further to me", "Kill the one closer to me", "Closer enemy is yours", "Take down the enemy on the left", "Right target is yours", });
            }
        }
    }

    public int GetCurrentMission()
    {
        return currentMission;
    }
}
