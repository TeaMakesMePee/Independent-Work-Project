using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    float startScale, targetScale, currScale;

    private void Awake()
    {
        startScale = currScale = targetScale = GetComponent<RectTransform>().localScale.x;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = 2.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = 2f;
    }

    private void Update()
    {
        currScale = Mathf.Lerp(currScale, targetScale, Time.deltaTime * 10f);
        GetComponent<RectTransform>().localScale = new Vector3(currScale, currScale, currScale);
    }
}