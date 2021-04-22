using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public GameObject mainCamera;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        direction = new Vector3(45, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = mainCamera.transform.eulerAngles.y;
        transform.localEulerAngles = direction;
    }
}
