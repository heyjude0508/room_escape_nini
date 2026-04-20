using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingRoomStart : MonoBehaviour
{
    public GameEvent gameEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            gameEvent.Raise();
        }
    }
}
