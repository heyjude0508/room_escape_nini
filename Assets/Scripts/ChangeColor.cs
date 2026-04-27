using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest: MonoBehaviour, IInteractable
{
    public MeshRenderer cubeRenderer;
    public GameEvent gameEventAimStart;
    public GameEvent gameEventAimEnd;
    public GameEvent gameEventInteract;

    public DOTweenAnimation dtAnim;
    void Start()
    {
        //dtAnim.DOPlay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventAimStart();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventAimEnd();
        }
    }

    public void Interact()
    {
        cubeRenderer.material.color = new Color(Random.value, Random.value, Random.value);
        gameEventInteract.Raise();
    }

    public string GetDescription()
    {
        return "Change the cube color";
    }

    public void EventAimStart()
    {
        gameEventAimStart.Raise();
    }

    public void EventAimEnd()
    {
        gameEventAimEnd.Raise();
    }
}
