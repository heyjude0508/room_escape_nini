using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    public bool isPickedUp = false;

    public Coroutine xiecheng;
    public GameObject light;
    public IEnumerator PorologueStart()
    {
        yield return new WaitForSeconds(3);

        Debug.Log("What is happening");

        yield return new WaitForSeconds(1);

        Debug.Log("Light on");
        light.SetActive(true);




        yield return null;
    }


    // Start is called before the first frame update
    void Start()
    {
        xiecheng = StartCoroutine(PorologueStart());
    }

    private void OnDisable()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
