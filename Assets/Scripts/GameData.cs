public static class GameData
{
    public enum Division
    {
        P_Tank,
        P_Damage,
        P_Flank
    }

    private static Division theDivision = Division.P_Tank;

    public static void SetDivision(Division theDiv)
    {
        theDivision = theDiv;
    }

    public static Division GetDivision()
    {
        return theDivision;
    }
}
