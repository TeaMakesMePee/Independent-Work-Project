using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    float startPos, targetPos, currPos;

    private void Awake()
    {
        startPos = currPos = targetPos = GetComponent<RectTransform>().anchoredPosition.x;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPos = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPos = -350f;
    }

    private void Update()
    {
        currPos = Mathf.Lerp(currPos, targetPos, Time.deltaTime * 10f);
        Vector3 pos = GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchoredPosition = new Vector3(currPos, pos.y, pos.z);
    }
}