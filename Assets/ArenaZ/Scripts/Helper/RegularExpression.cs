using System.Text.RegularExpressions;

public class RegularExpression
{
    public Regex hasNumber = new Regex(@"[0-9]+");
    public Regex hasUpperChar = new Regex(@"[A-Z]+");
    public Regex hasLowerChar = new Regex(@"[a-z]+");
    public Regex hasCapAndSmall = new Regex(@"[a-zA-Z]+");
    public Regex hasSpace = new Regex(@"\s");
    // public Regex emailFormat = new Regex(@"^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+\.)+[a-z]{2,5}$");
    public Regex emailFormat = new Regex(@"^[a-z0-9._]+@([-a-z0-9]+\.)+[a-z]{2,5}$");
    public Regex hasMinimum8Chars = new Regex(@".{8,}");
    public Regex hasAtTheRate = new Regex(@"\@");
    public Regex hasspecialCharacter = new Regex(@"([<>\?\*\\\""/\|@&'#!])+");
}
