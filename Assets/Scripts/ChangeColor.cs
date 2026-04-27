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

    public string showInfo;
    void Start()
    {
        //dtAnim.DOPlay();
    }

    void Update()
    {
        
    }

    public void Interact()
    {
        cubeRenderer.material.color = new Color(Random.value, Random.value, Random.value);
        //gameEventInteract.Raise();
        this.gameObject.SetActive(false);
    }

    public string GetDescription()
    {
        return showInfo;
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
