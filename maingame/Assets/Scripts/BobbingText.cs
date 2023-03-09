using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingText : MonoBehaviour
{
    public float speed = 0.05f;
    public int goingUpTime = 100;
    int time = 0;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, speed, 0);

        time = (time+1)%goingUpTime;
        
        if (time == 0) {
            speed = speed * (-1);
        }
    }
}
