using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Player.GetInstance().transform.position = new Vector3(transform.position.x, transform.position.y, Player.GetInstance().transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
