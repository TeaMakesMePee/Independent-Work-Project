using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisionInfoTab : MonoBehaviour
{
    [SerializeField]
    public GameObject tank, damage, flank;

    public List<GameObject> toDisable;
    private bool disabled;

    private GameObject currTab;
    private float activePos, inactivePos, currPos, targetPos;
    private void Awake()
    {
        switch (GameData.GetDivision())
        {
            case GameData.Division.P_Damage:
                currTab = damage;
                break;
            case GameData.Division.P_Flank:
                currTab = flank;
                break;
            case GameData.Division.P_Tank:
                currTab = tank;
                break;
        }
        currTab.transform.parent.gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        activePos = 0f;
        inactivePos = currPos = - 270f;
        disabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            targetPos = activePos;
            if (!disabled)
                DisableUIUnderneath();
        }
        else
        {
            targetPos = inactivePos;
            if (disabled)
                EnableUIUnderneath();
        }

        currPos = Mathf.Lerp(currPos, targetPos, Time.deltaTime * 15f);
        currTab.transform.position = new Vector3(currTab.transform.position.x, currPos, currTab.transform.position.z);
    }

    void DisableUIUnderneath()
    {
        for (int x = 0; x < toDisable.Count; ++x)
        {
            toDisable[x].SetActive(false);
        }
        disabled = true;
    }

    void EnableUIUnderneath()
    {
        for (int x = 0; x < toDisable.Count; ++x)
        {
            toDisable[x].SetActive(true);
        }
        disabled = false;
    }
}
