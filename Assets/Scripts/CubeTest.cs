using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest: MonoBehaviour, IInteractable
{
    public MeshRenderer cubeRenderer;

    void Start()
    {
    }

    void Update()
    { 
    }

    public void Interact()
    {
        cubeRenderer.material.color = new Color(Random.value, Random.value, Random.value);
    }

    public string GetDescription()
    {
        return "Change the cube color";
    }
}
