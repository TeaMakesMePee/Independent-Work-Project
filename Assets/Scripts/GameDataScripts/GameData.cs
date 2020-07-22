public static class GameData
{
    public enum Division
    {
        P_Tank,
        P_Damage,
        P_Flank
    }

    private static Division theDivision = Division.P_Damage;

    public static void SetDivision(Division theDiv)
    {
        theDivision = theDiv;
    }

    public static Division GetDivision()
    {
        return theDivision;
    }

    private static bool isAuth = false;

    public static void SetAuth(bool auth)
    {
        isAuth = auth;
    }

    public static bool GetAuth()
    {
        return isAuth;
    }

    public struct Database
    {
        public int totalKills, totalDeaths, totalAssists, totalWins, totalLosses, totalDraws, totalHits, totalMisses
            , totalDamage, mostKills, mostDamage, totalCaptures, mostCaptures, playtime, experience;

        public void Init()
        {
            totalKills = totalDeaths = totalAssists = totalWins = totalLosses = totalDraws = totalHits = totalMisses = totalDamage = mostKills = mostDamage = totalCaptures = mostCaptures = playtime = experience = 0;
        }
    }

    public static Database pStat = new Database();
}
