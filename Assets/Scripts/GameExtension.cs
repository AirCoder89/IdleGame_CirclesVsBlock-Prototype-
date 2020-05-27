
public static class GameExtension
{
    private static string _thousands = "K";
    private static string _millions = "M";
    private static string _billions = "B";
    
    public static string ConvertGold(ulong amount)
    {
        var longAmount = amount.ToString().Length;
        if (longAmount < 5) return GenerateZero(amount);
        var div = 0;
        var abv = "";

        if (longAmount >= 5 && longAmount < 7)
        {
            div = 1000;
            abv = _thousands;
        }
        else if (longAmount >= 7 && longAmount < 10)
        {
            div = 1000000;
            abv = _millions;
        }
        else if (longAmount >= 10)
        {
            div = 1000000000;
            abv = _billions;
        }
        
        var num = ((ulong)amount / (ulong)div);
        var rest = (ulong)amount % (ulong)div;
        if (rest == 0 || rest.ToString().Substring(0, 1) == "0") return num + abv;
        return num + "." + rest.ToString().Substring(0, 1) + abv;
    }

    private static string GenerateZero(ulong amount)
    {
        if (amount.ToString().Length >= 3) return amount.ToString();
        return "0" + amount.ToString();
    }
}
