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
        GameData.pStat.Init();

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

        GetStats();
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

    public void SetStats(int kills, int deaths, int assists, int wins, int losses, int draws, int hits, int misses, int damage, int captured, int playtime)
    {
        int exp = kills * 100 + assists * 50 + wins * 500 + draws * 250 + damage + captured * 10 + playtime;
        Debug.LogError("Exp: " + exp);
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Kills", Value = GameData.pStat.totalKills + kills},
                new StatisticUpdate { StatisticName = "Deaths", Value = GameData.pStat.totalDeaths + deaths},
                new StatisticUpdate { StatisticName = "Assists", Value = GameData.pStat.totalAssists + assists},
                new StatisticUpdate { StatisticName = "MostKills", Value = (kills >= GameData.pStat.mostKills ? kills : GameData.pStat.mostKills)},
                new StatisticUpdate { StatisticName = "Wins", Value = GameData.pStat.totalWins + wins},
                new StatisticUpdate { StatisticName = "Losses", Value = GameData.pStat.totalLosses + losses},
                new StatisticUpdate { StatisticName = "Draws", Value = GameData.pStat.totalDraws + draws},
                new StatisticUpdate { StatisticName = "Hits", Value = GameData.pStat.totalHits + hits},
                new StatisticUpdate { StatisticName = "Misses", Value = GameData.pStat.totalMisses +misses},
                new StatisticUpdate { StatisticName = "Damage", Value = GameData.pStat.totalDamage + damage},
                new StatisticUpdate { StatisticName = "MostDamage", Value = (damage >= GameData.pStat.mostDamage ? damage : GameData.pStat.mostDamage)},
                new StatisticUpdate { StatisticName = "Captures", Value = GameData.pStat.totalCaptures + captured},
                new StatisticUpdate { StatisticName = "MostCaptures", Value = (captured >= GameData.pStat.mostCaptures ? captured : GameData.pStat.mostCaptures)},
                new StatisticUpdate { StatisticName = "Playtime", Value = GameData.pStat.playtime + playtime}, //in seconds
                new StatisticUpdate { StatisticName = "Exp", Value = exp} //calculate the experience required
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
                    GameData.pStat.totalKills = eachStat.Value;
                    break;
                case "Deaths":
                    //Set the score
                    GameData.pStat.totalDeaths = eachStat.Value;
                    break;
                case "Assists":
                    //Set the score
                    GameData.pStat.totalAssists = eachStat.Value;
                    break;
                case "MostKills":
                    //Set the score
                    GameData.pStat.mostKills = eachStat.Value;
                    break;
                case "Wins":
                    //Set the score
                    GameData.pStat.totalWins = eachStat.Value;
                    break;
                case "Losses":
                    //Set the score
                    GameData.pStat.totalLosses = eachStat.Value;
                    break;
                case "Draws":
                    //Set the score
                    GameData.pStat.totalLosses = eachStat.Value;
                    break;
                case "Hits":
                    //Set the score
                    GameData.pStat.totalHits = eachStat.Value;
                    break;
                case "Misses":
                    //Set the score
                    GameData.pStat.totalMisses = eachStat.Value;
                    break;
                case "Damage":
                    //Set the score
                    GameData.pStat.totalDamage = eachStat.Value;
                    break;
                case "MostDamage":
                    //Set the score
                    GameData.pStat.mostDamage = eachStat.Value;
                    break;
                case "Captures":
                    //Set the score
                    GameData.pStat.totalCaptures = eachStat.Value;
                    break;
                case "MostCaptures":
                    //Set the score
                    GameData.pStat.mostCaptures = eachStat.Value;
                    break;
                case "Playtime":
                    //Set the score
                    GameData.pStat.playtime = eachStat.Value;
                    break;
                case "Exp":
                    //Set the score
                    GameData.pStat.experience = eachStat.Value;
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
