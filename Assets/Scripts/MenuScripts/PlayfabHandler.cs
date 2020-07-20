using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayfabHandler : MonoBehaviour
{
    //Auth
    public TMP_InputField username, password;

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "88AA3";
        }
    }

    #region Login

    public void OnClickLogin()
    {
        var request = new LoginWithPlayFabRequest { Username = username.text, Password = password.text, TitleId = PlayFabSettings.TitleId };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("You logged in successfully!");
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
        OnSignUpLogin();
    }

    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {

    }

    void OnDisplayNameFail(PlayFabError result)
    {
        Debug.LogError(result.GenerateErrorReport());
    }

    private void OnSignUpLogin()
    {
        var request = new LoginWithPlayFabRequest { Username = username.text, Password = password.text, TitleId = PlayFabSettings.TitleId, };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnFailure);
    }

    #endregion

    #region Stats

    public void SetStats(int kills, int deaths, int assists, int wins, int losses, int hits, int misses, int exp)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Kills", Value = kills},
                new StatisticUpdate { StatisticName = "Deaths", Value = deaths},
                new StatisticUpdate { StatisticName = "Assists", Value = assists},
                new StatisticUpdate { StatisticName = "Wins", Value = wins},
                new StatisticUpdate { StatisticName = "Losses", Value = losses},
                new StatisticUpdate { StatisticName = "Hits", Value = hits},
                new StatisticUpdate { StatisticName = "Misses", Value = misses},
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
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
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
