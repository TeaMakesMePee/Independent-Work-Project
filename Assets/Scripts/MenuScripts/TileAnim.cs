using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class TileAnim : MonoBehaviour
{
    private float nextAngle,currAngle;
    private float coolDown, flipcoolDown;
    private Vector3 camtohex;
    private GameObject cam;
    private bool ifChanged;
    public int row;
    private float hue;
    public float interval;

    private void Start()
    {
        nextAngle = 0f;
        currAngle = 0f;
        cam = GameObject.Find("Main Camera");
        camtohex = (transform.position - cam.transform.position).normalized;
        hue = 0.05f;
        ifChanged = false;
    }

    private void Update()
    {
        //transform.Rotate(Vector3.forward * Time.deltaTime * 36f);
        if (coolDown < 0f)
        {
            camtohex.y = 0;
            currAngle = Mathf.Lerp(currAngle, nextAngle, Time.deltaTime * 8f);
            transform.eulerAngles = new Vector3(-90f, 0f, currAngle);

            if (flipcoolDown < 0f)
            {
                nextAngle += 180f;
                flipcoolDown = 5f;
            }

            if (Vector3.Angle(transform.up, camtohex) > 80f && Vector3.Angle(transform.up, camtohex) < 100f)
            {
                if (!ifChanged)
                {
                    GameObject mesh = transform.GetChild(0).gameObject;
                    mesh.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(hue, Random.Range(0f, 1f), Random.Range(0f, 1f));
                    ifChanged = true;
                    if (hue < 1f)
                    {
                        hue += 0.05f;
                    }
                    else if (hue == 1f)
                    {
                        hue = 0.05f;
                    }
                }
            }
            else
            {
                ifChanged = false;
            }
            flipcoolDown -= Time.deltaTime;
        }

        coolDown -= Time.deltaTime;
    }

    public void Init(int _row, float _interval)
    {
        row = _row;
        interval = _interval;
        coolDown = row * interval + 1f;
    }
}
