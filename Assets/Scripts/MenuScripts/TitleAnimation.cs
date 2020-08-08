using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * This scripts handles the lighting applied on the title in the main menu
 * It gives a cool effect
*/

public class TitleAnimation : MonoBehaviour
{
    private TextMeshProUGUI gameTitle;
    private float startLighting, currLighting, targetLighting;
    private float lightCooldown, timetoAnim, curranimTime;
    private bool up, down;

    private void Awake()
    {
        gameTitle = GetComponent<TextMeshProUGUI>();
        startLighting = currLighting = targetLighting = 0f;
        lightCooldown = 2f;
        curranimTime = 0f;
        timetoAnim = 10f;
        up = down = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (lightCooldown <= 0f && !up && !down)
        {
            up = true;
            targetLighting = 6f;
            curranimTime = 0f;
        }

        if (up && !down)
        {
            if (curranimTime <= timetoAnim)
            {
                curranimTime += Time.deltaTime;
                currLighting = Mathf.Lerp(currLighting, targetLighting, curranimTime / timetoAnim);
                gameTitle.fontSharedMaterial.SetFloat(ShaderUtilities.ID_LightAngle, currLighting);
                gameTitle.UpdateMeshPadding();
            }
            else
            {
                curranimTime = 0f;
                up = false;
                down = true;
                targetLighting = startLighting;
            }
        }

        if (down && !up)
        {
            if (curranimTime <= timetoAnim)
            {
                curranimTime += Time.deltaTime;
                currLighting = Mathf.Lerp(currLighting, targetLighting, curranimTime / timetoAnim);
                gameTitle.fontSharedMaterial.SetFloat(ShaderUtilities.ID_LightAngle, currLighting);
                gameTitle.UpdateMeshPadding();
            }
            else
            {
                curranimTime = 0f;
                down = false;
            }
        }

        lightCooldown -= Time.deltaTime;
    }

    public void OnApplicationQuit()
    {
        gameTitle.fontSharedMaterial.SetFloat(ShaderUtilities.ID_LightAngle, 0f);
    }
}
