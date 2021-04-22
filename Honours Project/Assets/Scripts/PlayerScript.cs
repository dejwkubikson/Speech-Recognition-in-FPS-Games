using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public RawImage alertImage;
    public Image crossHair;
    public bool inSight = false;
    public float seenTimer = 0.0f;

    private float fireRate = 0.6f;
    private float fireRateTimer = 0.6f;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        alertImage.enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject gameControlObject = GameObject.FindGameObjectWithTag("GameController");
            if(gameControlObject != null)
            {
                GameControl gameControl = gameControlObject.GetComponent<GameControl>();
                if(gameControl != null)
                {
                    gameControl.exitGame = true;
                    gameControl.resourcesLoaded = false;
                    SceneManager.LoadScene("Menu");
                }
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject enemy1 = GameObject.Find("Soldier 1");
        GameObject enemy2 = GameObject.Find("Soldier 2");

        if(player != null && enemy1 != null && enemy2 != null)
            Debug.Log($"Distance {enemy1.name}: {Vector3.Distance(player.transform.position, enemy1.transform.position)}, {enemy2.name}: {Vector3.Distance(player.transform.position, enemy2.transform.position)}");

        Debug.DrawRay(transform.position, transform.forward * 1000, Color.white);

        inSight = false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Soldier");
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemyBehaviour enemyScript = enemies[i].GetComponent<EnemyBehaviour>();
            if(enemyScript != null)
            {
                if (enemyScript.playerSeen)
                {
                    inSight = true;
                }
            }
        }

        if (inSight)
        {
            seenTimer += Time.deltaTime;
            InSight();
        }
        else
        {
            alertImage.enabled = false;
            seenTimer = 0.0f;
        }

        if(fireRateTimer < fireRate)
        {
            fireRateTimer += Time.deltaTime;
        }

        if(Input.GetMouseButton(0) && fireRateTimer >= fireRate)
        {
            fireRateTimer = 0;
            audioSource.Play();
            RaycastHit hit;
            int layerMask = 1 << 2;
            layerMask = ~layerMask;

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("hit : " + hit.transform.name);

                if (hit.transform.name == "Head" || hit.transform.name == "Body")
                {
                    GameObject enemyObject = hit.transform.parent.transform.parent.transform.parent.gameObject;
                    if (enemyObject != null)
                    {
                        EnemyBehaviour enemyScript = enemyObject.GetComponent<EnemyBehaviour>();
                        if (hit.transform.name == "Head")
                            enemyScript.ReceiveDamage(100);
                        else
                            enemyScript.ReceiveDamage(50);
                    }
                }
                else if(hit.transform.tag == "ScriptedGroup")
                {
                    GameControl gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
                    gameControl.GameLost("The enemy group raised an alarm.");
                }
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //Debug.Log("Did Hit " + hit.transform.name);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    public void InSight()
    {
        alertImage.enabled = true;

        byte pulseVal = (byte)(255 * Mathf.Abs(Mathf.Sin(seenTimer * 3)));

        if (seenTimer <= 2)
        {
            // Flash Alert UI in #FF9600 colour
            alertImage.color = new Color32(255, 150, 0, pulseVal);
        }
        else
        {
            // Flash Alert UI in #FF0000 colour
            alertImage.color = new Color32(255, 0, 0, pulseVal);
        }
    }
}
