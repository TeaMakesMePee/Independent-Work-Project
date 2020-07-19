using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillfeedFade : MonoBehaviour
{
    // Start is called before the first frame update
    private float cooldown = 5f;

    // Update is called once per frame
    void Update()
    {
        if (cooldown <= 0f)
        {
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(GetComponent<CanvasGroup>().alpha, 0f, Time.deltaTime * 4f);
            if (GetComponent<CanvasGroup>().alpha <= 0.05f)
                Destroy(gameObject);
        }

        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
    }
}
