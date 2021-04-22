using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ScriptedGroup")
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            GameControl gameControl = gameController.GetComponent<GameControl>();
            gameControl.GameLost("The enemy group spotted you.");
        }
        if(other.name == "Mission-1")
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            GameControl gameControl = gameController.GetComponent<GameControl>();
            gameControl.StartMission1();
        }

        if(other.name == "Mission-3")
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            GameControl gameControl = gameController.GetComponent<GameControl>();
            gameControl.StartMission3();
        }

        if(other.name == "Checkpoint-1")
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            GameControl gameControl = gameController.GetComponent<GameControl>();
            gameControl.checkPointReached = true;
        }
    }
}
