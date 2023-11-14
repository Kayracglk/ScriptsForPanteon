using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public bool check = false;

    [SerializeField] private Transform karakter;

    RaycastHit hit;
    Ray ray;

    Vector3 mouseInput;

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked) return;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            mouseInput = new Vector3(hit.point.x, 0, hit.point.z);
        }

        Quaternion rotation = Quaternion.LookRotation(mouseInput - transform.position);

        transform.rotation = Quaternion.Lerp(rotation, transform.rotation, 55*Time.deltaTime);
    }
}
