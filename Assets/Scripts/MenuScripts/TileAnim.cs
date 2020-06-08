using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class TileAnim : MonoBehaviour
{
    private float nextAngle,currAngle;
    private float coolDown;
    private Vector3 camtohex, hextocam;
    private GameObject cam;
    private bool ifAnim;
    private bool ifChanged = false;
    private float initAngle;

    private void Start()
    {
        nextAngle = 0f;
        currAngle = 0f;
        Debug.LogError(nextAngle);
        Debug.LogError(transform.rotation);
        coolDown = Random.Range(15f, 30f);
        cam = GameObject.Find("Main Camera");
        camtohex = (transform.position - cam.transform.position).normalized;
        float angle = Vector3.Angle(transform.up, camtohex);
        initAngle = angle;
        //Debug.LogError(angle);
    }

    private void Update()
    {
        ////transform.Rotate(Vector3.forward * Time.deltaTime * 36f);
        //camtohex.y = 0;
        //currAngle = Mathf.Lerp(currAngle, nextAngle, Time.deltaTime * 2f);
        //transform.eulerAngles = new Vector3(-90f, 0f, currAngle);

        //if (coolDown < 0f)
        //{
        //    Debug.LogError(true);
        //    nextAngle += 180f;
        //    coolDown = Random.Range(15f, 30f);
        //}
        ////transform.localRotation = nextAngle;

        //if (Mathf.Abs(Vector3.Cross(transform.up, camtohex).y) > 0.9f)
        //{
        //    if (!ifChanged)
        //    {
        //        GameObject mesh = transform.GetChild(0).gameObject;
        //        //mesh.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        //        mesh.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 0.7f), Random.Range(0f, 1f));
        //        //Debug.LogError(Vector3.Cross(transform.up, camtohex).y);
        //        ifChanged = true;
        //    }
        //}
        //else
        //{
        //    ifChanged = false;
        //}

        //coolDown -= Time.deltaTime;
    }
}
