using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndgameUI : MonoBehaviour
{
    private Image theBackground, redBar, blueBar;
    private GameObject barParent;
    private float bgAlpha, t_bgAlpha;
    private float redFill, t_redFill;
    private float blueFill, t_blueFill;
    private bool startEnd;

    private float bar_cTime, bar_mTime, bar_cY, bar_tY;
    private float bg_cTime, bg_mTime;
    private float fill_cTime, fill_mTime;
    private bool bgDone, barDone;
    private int blueScore, redScore;
    private TextMeshProUGUI bScoreText, rScoreText;

    private void Awake()
    {
        theBackground = transform.Find("Background").GetComponent<Image>();
        redBar = transform.Find("Bar").transform.Find("Red").GetComponent<Image>();
        blueBar = transform.Find("Bar").transform.Find("Blue").GetComponent<Image>();
        bScoreText = transform.Find("Bar").transform.Find("BlueText").GetComponent<TextMeshProUGUI>();
        rScoreText = transform.Find("Bar").transform.Find("RedText").GetComponent<TextMeshProUGUI>();
        barParent = transform.Find("Bar").gameObject;
        startEnd = false;
        bar_cY = barParent.GetComponent<RectTransform>().localPosition.y;
        bgAlpha = redFill = blueFill = bar_tY = 0f;
        bar_cTime = bg_cTime = fill_cTime = 0f;
        bar_mTime = bg_mTime = 2f;
        fill_mTime = 8f;
        t_blueFill = t_redFill = 0f;
        bgDone = barDone = false;
        blueScore = redScore = 0;
    }

    private void Update()
    {
        if (!startEnd)
            return;

        if (bg_cTime <= bg_mTime)
        {
            bg_cTime += Time.deltaTime;
            bgAlpha = Mathf.Lerp(bgAlpha, t_bgAlpha, bg_cTime / bg_mTime);
            theBackground.color = new Color(0f, 0f, 0f, bgAlpha / 255f);
            if (bg_cTime >= 1f)
            {
                bgDone = true;
            }
        }
        else
        {
            bg_cTime = 0f;
            bgAlpha = t_bgAlpha;
            theBackground.color = new Color(0f, 0f, 0f, bgAlpha / 255f);
        }

        if (bgDone)
        {
            if (bar_cTime <= bar_mTime)
            {
                bar_cTime += Time.deltaTime;
                bar_cY = Mathf.Lerp(bar_cY, bar_tY, bar_cTime / bar_mTime);
                barParent.GetComponent<RectTransform>().localPosition = new Vector3(0f, bar_cY, 0f);
                if (bar_cTime >= 1f)
                {
                    barDone = true;
                }
            }
            else
            {
                barDone = true;
                bar_cTime = 0f;
                bar_cY = 0f;
                barParent.GetComponent<RectTransform>().localPosition = new Vector3(0f, bar_cY, 0f);
            }
        }

        if (barDone)
        {
            if (fill_cTime <= fill_mTime)
            {
                fill_cTime += Time.deltaTime;

                redFill = Mathf.Lerp(redFill, t_redFill, fill_cTime / fill_mTime);
                redBar.fillAmount = redFill;

                blueFill = Mathf.Lerp(blueFill, t_blueFill, fill_cTime / fill_mTime);
                blueBar.fillAmount = blueFill;

                if (redFill + blueFill >= 0.98f)
                {
                    if (redFill > blueFill)
                    {
                        redScore = (int)(redFill * 100f);
                        blueScore = 100 - redScore;
                    }
                    else
                    {
                        blueScore = (int)(blueFill * 100f);
                        redScore = 100 - blueScore;
                    }

                    rScoreText.text = redScore.ToString();
                    bScoreText.text = blueScore.ToString();
                }
                else
                {
                    redScore = (int)(redFill * 100f);
                    rScoreText.text = redScore.ToString();

                    blueScore = (int)(blueFill * 100f);
                    bScoreText.text = blueScore.ToString();
                }
            }
            else
            {
                bgDone = true;
                fill_cTime = 0f;

                redFill = t_redFill;
                redBar.fillAmount = redFill;

                redScore = (int)(redFill * 100f);
                blueScore = 100 - redScore;

                rScoreText.text = redScore.ToString();
                bScoreText.text = blueScore.ToString();

                blueFill = t_blueFill;
                blueBar.fillAmount = blueFill;
            }
        }
    }

    public void StartAnim(float red, float blue)
    {
        startEnd = true;
        t_bgAlpha = 80f;
        t_redFill = red / (red + blue);
        t_blueFill = blue / (red + blue);
    }
}
