using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    float startPos, targetPos, currPos;
    public bool left;

    private void Awake()
    {
        startPos = currPos = targetPos = GetComponent<RectTransform>().anchoredPosition.x;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (left)
            targetPos = 0f;
        else
            targetPos = -500f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (left)
            targetPos = -350f;
        else
            targetPos = -150f;
    }

    private void Update()
    {
        currPos = Mathf.Lerp(currPos, targetPos, Time.deltaTime * 10f);
        Vector3 pos = GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchoredPosition = new Vector3(currPos, pos.y, pos.z);
    }

    public void ResetButton()
    {
        if (left)
            targetPos = -350f;
        else
            targetPos = -150f;
    }
}