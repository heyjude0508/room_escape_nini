using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 返回关键道具的描述
    string GetDescription();

    // 和道具进行交互
    void Interact();

}
