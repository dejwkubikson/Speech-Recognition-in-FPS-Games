using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyScript : MonoBehaviour
{
    GameObject playerObject;
    public GameObject enemyToKill = null;
    public Text allyText;
    public Text playerText;

    VoiceControl voiceControl;
    GameControl gameControl;

    public GameObject[] missionOneEnemies;
    public GameObject[] missionTwoEnemies;
    public GameObject[] enemies;

    private AudioSource audioSource;

    private bool answered = false;
    private float answerTimer = 3.0f;
    private float answerCurrTimer = 0.0f;

    public bool notUnderstood = false;
    public bool rogerThat = false;
    public string rogerThatText = "";
    public bool tellAboutEnemies = false;
    public bool tellIfEnemyDown = false;
    public bool tellWhereToGoWhatToDo = false;
    public bool determineEnemyToKill = false;
    public string determineEnemyToKillPos = "";
    public bool killEnemyOrder = false;
    public bool confirmTarget = false;
    public bool forceMissionFour = false;

    private string targetPos = "";

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        voiceControl = GameObject.FindGameObjectWithTag("VoiceControl").GetComponent<VoiceControl>();
        gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
        audioSource = GetComponent<AudioSource>();

        enemies = GameObject.FindGameObjectsWithTag("Soldier");
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Soldier");

        if (answered)
        {
            answerCurrTimer += Time.deltaTime;
            if(answerCurrTimer > answerTimer)
            {
                answered = false;
                answerCurrTimer = 0.0f;
                allyText.text = "";
                playerText.text = "";
            }
        }

        if(rogerThat)
        {
            Roger();
            rogerThat = false;
            rogerThatText = "";
        }

        if(notUnderstood)
        {
            NotUnderstood();
            notUnderstood = false;
        }

        if(tellAboutEnemies)
        {
            tellAboutEnemies = false;
            TellAboutEnemies();
        }

        if(tellIfEnemyDown)
        {
            tellIfEnemyDown = false;
            TellIfEnemyDown();
        }

        if(tellWhereToGoWhatToDo)
        {
            tellWhereToGoWhatToDo = false;
            TellWhereToGoWhatToDo();
        }

        if(determineEnemyToKill)
        {
            if (DetermineEnemyToKill(determineEnemyToKillPos))
            {
                voiceControl.waitForTargetSpecification = false;
                voiceControl.waitForKillConfirmation = true;
            }
            determineEnemyToKill = false;
            determineEnemyToKillPos = "";
        }

        if(killEnemyOrder)
        {
            killEnemyOrder = false;
            KillEnemyOrder();
        }

        if(confirmTarget)
        {
            confirmTarget = false;
            ConfirmTarget();
        }

        if(forceMissionFour)
        {
            forceMissionFour = false;
            ForceMissionFour();
        }

        if(!gameControl.speechRecognition)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("TellAboutEnemies()");
                TellAboutEnemies();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("TellWhereToGoWhatToDo()");
                TellWhereToGoWhatToDo();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("TellIfEnemyDown()");
                TellIfEnemyDown();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("Roger");
                Roger();
            }
        }
    }

    public void NotUnderstood()
    {
        int random = Random.Range(1, 4);
        if (random == 1)
            RadioCall("I didn't get that Alpha.");
        else if (random == 2)
            RadioCall("I don't understand you Alpha.");
        else if (random == 3)
            RadioCall("Negative, be clear with your commands Alpha.");
        else
            RadioCall("Alpha, what are you talking about?");
    }

    public void RadioCall(string text)
    {
        Debug.Log(text);
        answered = false;
        allyText.text = "Charlie: " + text;
    }

    public void Roger()
    {
        if (allyText.text == "" || answered)
            return;

        if(gameControl.speechRecognition)
        {
            //allyText.text = "Alpha: " + rogerThatText;
        }
        else
        {
            int random = Random.Range(1, 3);
            if (random == 1)
                playerText.text = "Alpha: Roger";
            else if (random == 2)
                playerText.text = "Alpha: Understood";
            else
                playerText.text = "Alpha: Got it";
        }

        answered = true; 
    }

    public void TellWhereToGoWhatToDo()
    {
        int currentMission = gameControl.GetCurrentMission();

        switch (currentMission)
        {
            case 0:

                RadioCall("Alpha you need to get to the small village. " +
                    "There's a bridge that will get you through the river.");
                break;
            case 1:

                RadioCall("Alpha we need to wait for the group of enemies to pass.");
                break;
            case 2:

                if (missionOneEnemies[0].GetComponent<EnemyBehaviour>().dead && missionOneEnemies[1].GetComponent<EnemyBehaviour>().dead)
                {
                    if (gameControl.checkPointReached == false)
                    {
                        RadioCall("Alpha you need to get over the bridge and get to the main village where Mohamed is.");
                    }
                    else
                    {
                        RadioCall("Alpha you need to get to the village.");
                    }
                }
                else
                {
                    if(enemyToKill == null)
                    {
                        RadioCall("Alpha we need to kill the two tangos near the campfire. Tell me which one is yours.");
                        if (gameControl.speechRecognition)
                            voiceControl.DisplayPossibleCommands(new string[] { "I'll take down the one on the left", "Right one is yours", "Right one is mine" });
                    }
                    else
                    {
                        RadioCall("Alpha waiting for your command to shoot.");
                        if (gameControl.speechRecognition)
                            voiceControl.DisplayPossibleCommands(new string[] { "Shoot", "Kill", "Confirm your target" });
                    }
                }
                break;
            case 3:

                RadioCall("Alpha you need to eliminate or get past the multiple tangos to the main building.");
                break;
            case 4:

                if (missionTwoEnemies[0].GetComponent<EnemyBehaviour>().dead && missionTwoEnemies[1].GetComponent<EnemyBehaviour>().dead)
                {
                    RadioCall("Alpha get into the main building and eliminate Mohamed.");
                }
                else
                {
                    if (enemyToKill == null)
                    {
                        RadioCall("Alpha we need to kill the two tangos near the campfire. Tell me which one is yours.");
                        if (gameControl.speechRecognition)
                            voiceControl.DisplayPossibleCommands(new string[] { "I will take down the one closer to me", "Kill the one further to me", "Closer enemy to me is yours" });
                    }
                    else
                    {
                        RadioCall("Alpha, waiting for your command to shoot.");
                        if (gameControl.speechRecognition)
                            voiceControl.DisplayPossibleCommands(new string[] { "Shoot", "Kill", "Confirm your target" });
                    }
                }
                break;
            default:
                break;
        }
    }

    public void TellIfEnemyDown()
    {
        int currentMission = gameControl.GetCurrentMission();
        if(currentMission == 2)
        {
            if (missionOneEnemies[0].GetComponent<EnemyBehaviour>().dead && missionOneEnemies[1].GetComponent<EnemyBehaviour>().dead)
            {
                RadioCall("Alpha both targets down. Nice shot.");
            }
            else
            {
                if (gameControl.speechRecognition)
                {
                    if (voiceControl.waitForKillConfirmation)
                    {
                        RadioCall("Alpha tangos alive, waiting for your call");
                        voiceControl.DisplayPossibleCommands(new string[] { "Shoot", "Kill", "Confirm your target" });
                    }
                    else
                    {
                        RadioCall("Alpha tangos alive, waiting for your target confirmation.");
                        voiceControl.DisplayPossibleCommands(new string[] { "I'll take down the one on the left", "Right one is yours", "Right one is mine" });
                    }
                }
            }
        }
        else if(currentMission == 4)
        {
            if (missionTwoEnemies[0].GetComponent<EnemyBehaviour>().dead && missionTwoEnemies[1].GetComponent<EnemyBehaviour>().dead)
            {
                RadioCall("Both tangos down. Good shot Alpha.");
            }
            else
            {
                if (gameControl.speechRecognition) 
                {
                    if (voiceControl.waitForKillConfirmation)
                    {
                        RadioCall("Alpha, tangos alive, waiting for your call");
                        voiceControl.DisplayPossibleCommands(new string[] { "Shoot", "Kill", "Confirm your target" });
                    }
                    else
                    {
                        RadioCall("Alpha, tangos alive, waiting for your target confirmation.");
                        voiceControl.DisplayPossibleCommands(new string[] { "I'll take down the one near me", "Kill the one near the bulding", "My one is near the building" });
                    }
                }
            }
        }
        else
        {
            RadioCall("Alpha, nothing to confirm.");
        }
    }

    public void TellAboutEnemies()
    {
        int[] nsewEnemyCount = { 0, 0, 0, 0 };
        int totalEnemies = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject enemy = enemies[i];

            if (enemy.GetComponent<EnemyBehaviour>() != null)
            {
                if (enemy.GetComponent<EnemyBehaviour>().dead)
                    continue;
            }

            playerObject = GameObject.FindGameObjectWithTag("Player");

            float distance = Vector3.Distance(enemy.transform.position, playerObject.transform.position);

            // Find enemies in radious
            if (distance <= 4)
            {
                // Forward vector will always be the same. Prevents from giving 'directional' direction of enemies.
                Vector3 fwd = new Vector3(0, 0, 1); //playerObject.transform.forward;
                Vector3 targetDir = enemy.transform.position - playerObject.transform.position;

                // Getting angle to object
                float angleToEnemy = Vector3.Angle(targetDir, fwd);

                // Checking if the object is on the right or left of the object - used to get the correct Compass location
                Vector3 perp = Vector3.Cross(fwd, targetDir);
                float dir = Vector3.Dot(perp, playerObject.transform.up);

                // N
                if(angleToEnemy <= 45)
                {
                    //Debug.Log($"{enemy.name}: Enemy at North, angle {angleToEnemy}, dir {dir}");
                    nsewEnemyCount[0] += 1;
                }
                // E
                else if(angleToEnemy > 45 && angleToEnemy <= 135 && dir > 0)
                {
                    //Debug.Log($"{enemy.name}: Enemy at East, angle {angleToEnemy}, dir {dir}");
                    nsewEnemyCount[2] += 1;
                }
                // W
                else if(angleToEnemy > 45 && angleToEnemy <= 135 && dir < 0)
                {
                    //Debug.Log($"{enemy.name}: Enemy to West, angle {angleToEnemy}, dir {dir}");
                    nsewEnemyCount[3] += 1;
                }
                // S
                else
                {
                    //Debug.Log($"{enemy.name}: Enemy at South, angle {angleToEnemy}, dir {dir}");
                    nsewEnemyCount[1] += 1;
                }
                
                totalEnemies++;
            }
        }

        // List enemies to the player
        string radioText = "";
        if (nsewEnemyCount[0] > 0)
        {
            if (nsewEnemyCount[0] > 1)
            {
                radioText += $"{nsewEnemyCount[0]} enemies at your North.";
            }
            else
            {
                radioText += "One enemy at your North. ";
            }
        }

        if (nsewEnemyCount[2] > 0)
        {
            if (nsewEnemyCount[2] > 1)
            {
                radioText += $"{nsewEnemyCount[2]} enemies at your East. ";
            }
            else
            {
                radioText += "One enemy at your East. ";
            }
        }

        if (nsewEnemyCount[3] > 0)
        {
            if (nsewEnemyCount[3] > 1)
            {
                radioText += $"{nsewEnemyCount[3]} enemies at your West. ";
            }
            else
            {
                radioText += "One enemy at your West. ";
            }
        }

        if (nsewEnemyCount[1] > 0)
        {
            if (nsewEnemyCount[1] > 1)
            {
                radioText += $"{nsewEnemyCount[1]} enemies at your South. ";
            }
            else
            {
                radioText += "One enemy at your South. ";
            }
        }

        if(totalEnemies == 0)
        {
            radioText = "I don't see anyone nearby.";
        }

        RadioCall("Alpha, " + radioText);
    }

    private static Vector3 getRelativePosition(Transform origin, Vector3 position)
    {
        Vector3 distance = position - origin.position;
        Vector3 relativePosition = Vector3.zero;

        relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
        relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
        relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);

        return relativePosition;
    }

    public void Test(string posName)
    {
        DetermineEnemyToKill(posName);
    }

    public bool DetermineEnemyToKill(string enemyPosName)
    {
        Debug.Log($"DetermineEnemyToKill({enemyPosName})");
        bool found = false;
        int currentMission = gameControl.GetCurrentMission();

        Vector3 enemy1Pos = Vector3.zero;
        Vector3 enemy2Pos = Vector3.zero;
        GameObject enemy1 = null;
        GameObject enemy2 = null;

        // Getting relative position from player to enemy
        if (currentMission == 2)
        {
            enemy1 = missionOneEnemies[0];
            enemy2 = missionOneEnemies[1];
            Debug.Log($"Relative 1: {enemy1Pos}, 2: {enemy2Pos}");
        }
        else if(currentMission == 4)
        {
            enemy1 = missionTwoEnemies[0];
            enemy2 = missionTwoEnemies[1];
        }

        switch(enemyPosName)
        {
            case "left":
                enemy1Pos = getRelativePosition(playerObject.transform, enemy1.transform.position);
                enemy2Pos = getRelativePosition(playerObject.transform, enemy2.transform.position);
                if (enemy1Pos.x <= enemy2Pos.x)
                    EnemyToKill(enemy1);
                else
                    EnemyToKill(enemy2);
                found = true;
                break;
            case "right":
                enemy1Pos = getRelativePosition(playerObject.transform, enemy1.transform.position);
                enemy2Pos = getRelativePosition(playerObject.transform, enemy2.transform.position);
                if (enemy1Pos.x >= enemy2Pos.x)
                    EnemyToKill(enemy1);
                else
                    EnemyToKill(enemy2);
                found = true;
                break;
            case "closer":
                enemy1Pos = enemy1.transform.position;
                enemy2Pos = enemy2.transform.position;
                if (Vector3.Distance(playerObject.transform.position, enemy1Pos) <= Vector3.Distance(playerObject.transform.position, enemy2Pos))
                    EnemyToKill(enemy1);
                else
                    EnemyToKill(enemy2);
                found = true;
                break;
            case "near":
                enemy1Pos = enemy1.transform.position;
                enemy2Pos = enemy2.transform.position;
                if (Vector3.Distance(playerObject.transform.position, enemy1Pos) <= Vector3.Distance(playerObject.transform.position, enemy2Pos))
                    EnemyToKill(enemy1);
                else
                    EnemyToKill(enemy2);
                found = true;
                break;
            case "further":
                enemy1Pos = enemy1.transform.position;
                enemy2Pos = enemy2.transform.position;
                if (Vector3.Distance(playerObject.transform.position, enemy1Pos) >= Vector3.Distance(playerObject.transform.position, enemy2Pos))
                    EnemyToKill(enemy1);
                else
                    EnemyToKill(enemy2);
                found = true;
                break;
            default:
                found = false;
                break;
        }

        if(found == false)
        {
            int random = Random.Range(1, 3);
            if(random == 1)
                RadioCall("Alpha, I didn't understand that, I repeat, I didn't understand that.");
            else if(random == 2)
                RadioCall("Alpha, I don't know which tango you want me to kill, I repeat, I don't know which tango you want me to kill.");
            else
                RadioCall("Alpha, be more specific with the target you want me to shoot, I repeat, be more specific with the target you want me to shoot.");
        }
        else
        {
            targetPos = enemyPosName;

            if (enemyPosName == "further" || enemyPosName == "closer")
                RadioCall("Roger that Alpha, I'll kill the target " + enemyPosName + " to you.");
            else if (enemyPosName == "near")
                RadioCall("Roger that Alpha, I'll kill the target near you");
            else
                RadioCall("Roger that Alpha, I'll kill the target on your " + enemyPosName + ".");
        }

        return found;
    }

    public void ConfirmTarget()
    {
        if(enemyToKill == null)
        {
            RadioCall("Negative, specify the target you want me to kill.");
        }

        if (targetPos == "further" || targetPos == "closer")
            RadioCall("Alpha, I'm aimed in the target " + targetPos + " to you.");
        else if (targetPos == "near")
            RadioCall("Alpha, I'm aimed in the target near you.");
        else
            RadioCall("Alpha, I'm aimed in the target on your " + targetPos + ".");
    }

    public void EnemyToKill(GameObject enemyObject)
    {
        enemyToKill = enemyObject;

        RadioCall("Alpha, target in sight. On your command.");

        if (gameControl.speechRecognition)
        {
            voiceControl.DisplayPossibleCommands(new string[] { "Shoot", "Kill", "Confirm your target" });
            voiceControl.waitForKillConfirmation = true;
        }
    }

    public void ForceMissionFour()
    {
        gameControl.forceMission4 = true;
    }

    public void KillEnemyOrder()
    {
        if (enemyToKill == null)
            return;

        EnemyBehaviour enemyScript = enemyToKill.GetComponent<EnemyBehaviour>();

        enemyScript.ReceiveDamage(100);

        audioSource.Play();

        int random = Random.Range(1, 4);
        if (random == 1)
            RadioCall("Alpha, enemy down");
        else if (random == 2)
            RadioCall("Alpha, tango down");
        else if (random == 3)
            RadioCall("Alpha, target dead");
        else
            RadioCall("Alpha, enemy killed");
    }
}
