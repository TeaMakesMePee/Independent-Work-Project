using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class DamagedIndicator : MonoBehaviour
{
    Vector3 enemyPos;
    float currAlpha;
    public void Init(Vector3 pos)
    {
        enemyPos = pos;
        currAlpha = 150f;
    }

    void Update()
    {
        GameObject thePlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        Vector3 look = thePlayer.transform.forward;
        look.y = 0f;
        look.Normalize();
        Vector3 dir = enemyPos - thePlayer.transform.position;
        dir.y = 0f;
        dir.Normalize();
        float angle = Vector3.Angle(look, dir);
        if (Vector3.Cross(look, dir).y > 0f)
            angle *= -1f;
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
        currAlpha = Mathf.Lerp(currAlpha, 0f, Time.deltaTime * 2f);
        GetComponent<Image>().color = new Color(1f, 1f, 1f, currAlpha / 255f);
        if (currAlpha < 0.01f)
            Destroy(gameObject);
    }
}
