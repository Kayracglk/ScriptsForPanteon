using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarFix : MonoBehaviour
{
    private Camera camera;

    [SerializeField] private Vector3 rotation;
    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (camera != null)
        {
            transform.LookAt(camera.transform.position);
        }
    }
}
