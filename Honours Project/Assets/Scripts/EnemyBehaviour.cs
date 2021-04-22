using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public Vector3[] moveToPoints = { new Vector3(-4, 0, 6), new Vector3(-8, 0, 6), new Vector3(-10, 0, 10) };
    public bool moveToPoint = false;
    public bool pointReached = false;
    private int currPoint = 1;

    public float speed = 0.2f;

    public float timeMin = 3.0f;
    public float timeMax = 10.0f;
    private float randomWaitTime = 0.0f;

    public float health = 100;
    public bool playerSeen = false;
    private float playerSeenTimer = 0.0f;

    private bool startDmgTimer = false;
    private float dmgTimer = 2.0f;

    public bool bossSoldier = false;

    private GameObject playerObject;
    public PlayerScript playerScript;

    public bool dead = false;

    GameControl gameControlScript;

    public bool hasFlashLight = false;
    public GameObject flashLight;

    public bool deadEnemySeen = false;
    public GameObject deadEnemy;

    public GameObject wrapper;

    public Animator animator;

    private void Start()
    {
        gameControlScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerObject.transform.GetChild(0).GetComponent<PlayerScript>();
        randomWaitTime = Random.Range(timeMin, timeMax);

        if (gameObject.name == "BossSoldier")
        {
            bossSoldier = true;
        }

        if (bossSoldier)
            return;

        flashLight.SetActive(hasFlashLight);
        animator = wrapper.GetComponent<Animator>();
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;
        

        if (bossSoldier)
            return;

        deadEnemySeen = CheckIfVisibleEnemyDead();

        if (CheckIfPlayerVisible())
        {
            playerSeen = true;

            RotateTowardsPoint(playerObject.transform.position);

            playerSeenTimer += Time.deltaTime;
            playerScript.inSight = true;
            playerScript.seenTimer = playerSeenTimer;

            if (playerSeenTimer > gameControlScript.timeToLose)
                gameControlScript.GameLost("The enemy spotted you.");
        }
        else
        {
            playerSeenTimer = 0;
            playerSeen = false;
        }

        if (deadEnemySeen && deadEnemy != null)
        {
            startDmgTimer = true;
            if(playerSeen == false)
                RotateTowardsPoint(deadEnemy.transform.position);
        }

        if (startDmgTimer)
        {
            dmgTimer -= Time.deltaTime;

            if (dmgTimer <= 0)
            {
                if(deadEnemySeen)
                    gameControlScript.GameLost("The enemy raised an alarm after noticing a dead soldier.");
                else
                    gameControlScript.GameLost("The enemy raised an alarm after being shot.");
            }
        }

        if(pointReached)
        {
            randomWaitTime = Random.Range(timeMin, timeMax);
            pointReached = false;

            if (currPoint < moveToPoints.Length)
            {
                currPoint++;
            }
            else
            {
                currPoint = 1;
            }
        }

        if (moveToPoints.Length > 0)
        {
            if (randomWaitTime <= 0)
            {
                moveToPoint = true;
                animator.enabled = true;
            }
            else if (moveToPoint == false)
            {
                randomWaitTime -= Time.deltaTime;

                animator.enabled = false;
            }
        }

        MoveToPoint(speed * Time.deltaTime);
    }

    void MoveToPoint(float step)
    {
        if(moveToPoint && playerSeen == false && deadEnemySeen == false && moveToPoints.Length > 0)
        {
            RotateTowardsPoint(moveToPoints[currPoint - 1]);
            transform.position = Vector3.MoveTowards(transform.position, moveToPoints[currPoint - 1], step);
            if(Vector3.Distance(transform.position, moveToPoints[currPoint - 1]) < 0.01f)
            {
                moveToPoint = false;
                pointReached = true;
                randomWaitTime = 0.0f;
            }
        }
    }

    void RotateTowardsPoint(Vector3 pointToLookAt)
    {
        Vector3 targetDirection = pointToLookAt - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void ReceiveDamage(float amount)
    {
        startDmgTimer = true;
        health -= amount;

        if (health <= 0)
            Die();
        else
            RotateTowardsPoint(playerObject.transform.position);
    }

    void Die()
    {
        playerSeen = false;

        if (bossSoldier)
            gameControlScript.GameWon();

        gameObject.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, transform.eulerAngles.z);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.GetComponent<BoxCollider>().enabled = true;
        animator.enabled = false;
        dead = true;
    }

    bool CheckIfVisibleEnemyDead()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Soldier");
        for(int i = 0; i < enemyObjects.Length; i++)
        {
            GameObject enemyObject = enemyObjects[i];
            Vector3 direction = enemyObject.transform.position - transform.position;
            float angleToEnemy = Vector3.Angle(direction, transform.forward);

            float distToEnemy = Vector3.Distance(enemyObject.transform.position, transform.position);

            //Debug.Log(enemyObjects[i].name + " angle " + angleToEnemy + " dist " + distToEnemy);

            if ((angleToEnemy >= -45 && angleToEnemy <= 45 && distToEnemy < 3f) || distToEnemy < 0.8f)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position + new Vector3(0, 0.01f, 0), direction, out hit, 4))
                {
                    Debug.DrawRay(transform.position + new Vector3(0, 0.01f, 0), direction * hit.distance, Color.yellow);

                    if(hit.transform.tag == "Soldier")
                    {
                        EnemyBehaviour enemyBehaviour = enemyObject.GetComponent<EnemyBehaviour>();
                        if (enemyBehaviour != null)
                        {
                            if (enemyBehaviour.dead)
                            {
                                deadEnemy = enemyObject;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        deadEnemy = null;
        return false;
    }

    bool CheckIfPlayerVisible()
    {
        Vector3 targetDir = playerObject.transform.position - transform.position;

        float angleToPlayer = Vector3.Angle(targetDir, transform.forward);

        float distToPlayer = Vector3.Distance(playerObject.transform.position, transform.position);

        if ((angleToPlayer >= -45 && angleToPlayer <= 45 && distToPlayer < 2.5f) || distToPlayer < 0.8f)
        {
            //Debug.Log("PLAYER IN ANGLE!");
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.07f, 0), targetDir, out hit, 4))
            {
                //Debug.Log(gameObject.name + " HIT OBJECT " + hit.transform.gameObject.name);
                Debug.DrawRay(transform.position + new Vector3(0, 0.07f, 0), targetDir * hit.distance, Color.yellow);
                
                if(hit.transform == playerObject.transform)
                {
                    //Debug.Log("HIT PLAYER!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, targetDir * 1000, Color.white);
                //Debug.Log(" Did not hit anything");

                return false;
            }
        }
        else
            return false;
    }
}
