using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayfabHandler : MonoBehaviour
{
    public static PlayfabHandler pf;

    //Auth
    public TMP_InputField username, password;
    private string user, pass;
    public GameObject authTab, menuTab;

    private void OnEnable()
    {
        if (pf == null)
        {
            pf = this;
        }
        else
        {
            if (pf != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "88AA3";
        }

        if (PlayerPrefs.HasKey("username"))
        {
            AutoLogin();
        }
    }

    #region Auth

    public void Logout()
    {
        PlayerPrefs.SetString("username", "");
        PlayerPrefs.SetString("password", "");
    }

    private void AutoLogin()
    {
        var request = new LoginWithPlayFabRequest { Username = PlayerPrefs.GetString("username"), Password = PlayerPrefs.GetString("password"), TitleId = PlayFabSettings.TitleId };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnFailure);
    }

    public void OnClickLogin()
    {
        var request = new LoginWithPlayFabRequest { Username = username.text, Password = password.text, TitleId = PlayFabSettings.TitleId };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnFailure);

        user = username.text;
        pass = password.text;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        if (!PlayerPrefs.HasKey("username"))
        {
            PlayerPrefs.SetString("username", user);
            PlayerPrefs.SetString("password", pass);
        }
        CloseTabs();
        GameData.SetAuth(true);
    }

    public void CloseTabs()
    {
        authTab.SetActive(false);
        menuTab.SetActive(true);
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void OnSignUpClick()
    {
        RegisterPlayFabUserRequest registRequest = new RegisterPlayFabUserRequest
        {
            Username = username.text,
            Password = password.text,
            TitleId = PlayFabSettings.TitleId,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(registRequest, OnRegistSucees, OnFailure);
    }

    void OnRegistSucees(RegisterPlayFabUserResult result)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = username.text }, OnDisplayName, OnDisplayNameFail);
        OnClickLogin();
    }

    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {

    }

    void OnDisplayNameFail(PlayFabError result)
    {
        Debug.LogError(result.GenerateErrorReport());
    }

    #endregion

    #region Stats

    public void SetStats(int kills, int deaths, int assists, int mostkills, int wins, int losses, int hits, int misses, int damage, int mostdamage, int captured, int mostcaptured, int playtime, int exp)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Kills", Value = kills},
                new StatisticUpdate { StatisticName = "Deaths", Value = deaths},
                new StatisticUpdate { StatisticName = "Assists", Value = assists},
                new StatisticUpdate { StatisticName = "MostKills", Value = mostkills},
                new StatisticUpdate { StatisticName = "Wins", Value = wins},
                new StatisticUpdate { StatisticName = "Losses", Value = losses},
                new StatisticUpdate { StatisticName = "Hits", Value = hits},
                new StatisticUpdate { StatisticName = "Misses", Value = misses},
                new StatisticUpdate { StatisticName = "Damage", Value = damage},
                new StatisticUpdate { StatisticName = "MostDamage", Value = mostdamage},
                new StatisticUpdate { StatisticName = "Captures", Value = captured},
                new StatisticUpdate { StatisticName = "MostCaptures", Value = mostcaptured},
                new StatisticUpdate { StatisticName = "Playtime", Value = playtime}, //in seconds
                new StatisticUpdate { StatisticName = "Exp", Value = exp}
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    private void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    void OnGetStats(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            switch (eachStat.StatisticName)
            {
                case "Kills":
                    //Set the score
                    break;
                case "Deaths":
                    //Set the score
                    break;
                case "Assists":
                    //Set the score
                    break;
                case "MostKills":
                    //Set the score
                    break;
                case "Wins":
                    //Set the score
                    break;
                case "Losses":
                    //Set the score
                    break;
                case "Hits":
                    //Set the score
                    break;
                case "Misses":
                    //Set the score
                    break;
                case "Damage":
                    //Set the score
                    break;
                case "MostDamage":
                    //Set the score
                    break;
                case "Captures":
                    //Set the score
                    break;
                case "MostCaptures":
                    //Set the score
                    break;
                case "Playtime":
                    //Set the score
                    break;
                case "Exp":
                    //Set the score
                    break;
            }
        }
        //if (result.Statistics.Count == 0)
            //cookieGame.SetScore(0);
        //EnterGame();
    }

    #endregion

    #region Leaderboard

    struct Leaderboard
    {
        public Leaderboard(string u, int s)
        {
            username = u;
            theStat = s;
        }
        public string username;
        public int theStat;
    }

    private List<Leaderboard> leaderboard;

    public void GetWinLeaderboard()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "Wins", MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetWinLeaderboard, OnErrorLeaderboard);
    }

    void OnGetWinLeaderboard(GetLeaderboardResult result)
    {
        leaderboard = new List<Leaderboard>();
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            leaderboard.Add(new Leaderboard(player.Profile.DisplayName, player.StatValue));
        }
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    #endregion
}
