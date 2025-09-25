using System;
using UnityEngine;

public class testttt : MonoBehaviour
{
    [SerializeField] private Camera camera; 
    
    private void Update()
    {
        Debug.Log(camera.WorldToScreenPoint(transform.position));
    }
}
