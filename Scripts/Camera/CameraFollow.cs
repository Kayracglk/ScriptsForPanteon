using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset;
    
    [SerializeField] private float smoothSpeed = 8f;
    
    private void Awake()
    {
        instance = this;
    }
    private void LateUpdate()
    {
        if(target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
        }
    }

    public void ChangeCameraTarget(Transform m_transform)
    {
        target = m_transform;
    }
}
