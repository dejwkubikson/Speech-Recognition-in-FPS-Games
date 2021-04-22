using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupBehaviour : MonoBehaviour
{
    public Vector3[] moveToPoints = { new Vector3(-4, 0, 6), new Vector3(-8, 0, 6), new Vector3(-10, 0, 10) };
    public bool moveToPoint = false;
    private int currPoint = 1;

    public float speed = 0.2f;

    // Update is called once per frame
    void Update()
    {
        MoveToPoint(speed * Time.deltaTime);
        RotateTowardsPoint(speed * Time.deltaTime);
    }

    void MoveToPoint(float step)
    {
        if (moveToPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToPoints[currPoint - 1], step);
            if (Vector3.Distance(transform.position, moveToPoints[currPoint - 1]) < 1)
            {
                if (currPoint < moveToPoints.Length)
                {
                    currPoint++;
                }
                else
                {
                    moveToPoint = false;
                    Destroy(this.gameObject);
                }
            }
        }
    }

    void RotateTowardsPoint(float step)
    {
        Vector3 targetDirection = moveToPoints[currPoint - 1] - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
