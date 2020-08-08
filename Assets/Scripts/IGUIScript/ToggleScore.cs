using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class manages the scoreboard
 * The scoreboard is updated on a function call
 * It lists all the information of players
 * Sorted by kills then by assists in descending order
 * Player has to press Tab to bring the scoreboard up
*/

public class ToggleScore : MonoBehaviour
{
    public GameObject scoreboard;
    public GameObject theStatBar;
    public GameObject emptyStatBar;
    private HexGameManager manager;
    private bool updated;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<HexGameManager>();
        updated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if (!updated)
            {
                UpdateLeaderboard(manager.GetPlayers());
                updated = true;
            }
            scoreboard.SetActive(true);
        }
        else
        {
            updated = false;
            scoreboard.SetActive(false);
        }
    }

    void ClearLeaderboard()
    {
        Transform content = scoreboard.transform.Find("Scroll View/Viewport/Content");
        for (int x = 0; x < content.childCount; ++x)
        {
            Destroy(content.GetChild(x).gameObject);
        }
    }

    void UpdateLeaderboard(List<PlayerInfo> playerInfos)
    {
        ClearLeaderboard();
        List<PlayerInfo> blue;
        List<PlayerInfo> red;

        blue = new List<PlayerInfo>();
        red = new List<PlayerInfo>();

        for (int x = 0; x < playerInfos.Count; ++x)
        {
            if (playerInfos[x].team == "Blue")
                blue.Add(playerInfos[x]);
            else
                red.Add(playerInfos[x]);
        }

        blue = blue.OrderByDescending(o => o.kills).ThenByDescending(o => o.assists).ToList();
        red = red.OrderByDescending(o => o.kills).ThenByDescending(o => o.assists).ToList();

        Transform content = scoreboard.transform.Find("Scroll View/Viewport/Content");

        foreach (PlayerInfo pB in blue)
        {
            GameObject pStat = Instantiate(theStatBar, content) as GameObject;
            pStat.GetComponent<Image>().color = new Color(0f, 1f, 1f, 130f / 255f);

            pStat.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = pB.name;
            pStat.transform.Find("Kills").GetComponent<TextMeshProUGUI>().text = (pB.kills).ToString();
            pStat.transform.Find("Deaths").GetComponent<TextMeshProUGUI>().text = (pB.deaths).ToString();
            pStat.transform.Find("Assists").GetComponent<TextMeshProUGUI>().text = (pB.assists).ToString();
        }

        if (blue.Count < 3)
        {
            for (int x = 0; x < 3 - blue.Count; ++x)
            {
                GameObject pStat = Instantiate(emptyStatBar, content) as GameObject;
            }
        }

        foreach (PlayerInfo pR in red)
        {
            GameObject pStat = Instantiate(theStatBar, content) as GameObject;
            pStat.GetComponent<Image>().color = new Color(1f, 0f, 0f, 130f / 255f);

            pStat.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = pR.name;
            pStat.transform.Find("Kills").GetComponent<TextMeshProUGUI>().text = (pR.kills).ToString();
            pStat.transform.Find("Deaths").GetComponent<TextMeshProUGUI>().text = (pR.deaths).ToString();
            pStat.transform.Find("Assists").GetComponent<TextMeshProUGUI>().text = (pR.assists).ToString();
        }
    }
}
