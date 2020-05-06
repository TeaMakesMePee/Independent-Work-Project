using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float intensity, smooth;
    private Quaternion initRot;

    private void Start()
    {
        initRot = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        Sway();
    }

    private void Sway()
    {
        float mouseInputX = Input.GetAxis("Mouse X");
        float mouseInputY = Input.GetAxis("Mouse Y");

        //calculate target rotation
        Quaternion rotDeltaX = Quaternion.AngleAxis(-intensity * mouseInputX, Vector3.up);
        Quaternion rotDeltaY = Quaternion.AngleAxis(intensity * mouseInputY, Vector3.right);
        Quaternion nextRot = initRot * rotDeltaX * rotDeltaY;

        //rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, nextRot, Time.deltaTime * smooth);
    }
}
