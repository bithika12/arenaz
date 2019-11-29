
public enum Page
{
    CharacterSelectionPanel,
    AccountAccessDetailsPanel,
    AcoountAccesOverlay,
    RegistrationOverlay,
    LogINOverlay,
    LevelSelectionPanel,
    SettingsPanel,
    LeaderBoardPanel,
    PlayerMatchHistoryPanel,
    MailboxPanel,
    MailPopUp,
    InfoAndRulesForPlayerPanel,
    TopAndBottomBarPanel,
    PlayerMatchPanel,
    DrawMatchPanel,
    PlayerLoosePanel,
    PlayerWinPanel,
    ShootingrangePanel,
    Canines,
    Kepler,
    Cyborg,
    CyborgSecond,
    Human,
    Ebot,
    KeplerSecond,
    ComingSoonOverlay,
    PopUpTextAccountAccess,
    PopUpTextSettings
}

public enum ButtonType
{
    CloseImage,
    GreenImageAccAccess,
    GreenImageCharacter,
    GreenImageMailBox,
    GreenImageSettings,
    GreenImageShootingRange,
    GreenImageYouWinLoose,
    YellowImagecharacter,
    YellowImageDraw,
    BackImage,
    HistoryImage,
    InfoAndRulesImage,
    SettingsImage,
    MailboxImage,
    PlusImage,
    RightArrow,
    LeftArrow,
    None
}

public enum Hide
{
    previous,
    none
}

public enum Store
{
    characterName,
    uiName,
    none
}

public enum GameType
{
    normal,
    training
}

public enum Checking
{
    Username,
    EmailID,
    Password
}

public enum ProfilePicType
{
    Small,
    Medium
}

public static class Constants
{
    public const string successFullyRegisterd = "User registered successfully!!";
    public const string successFullyLoggedOut = "User Loggedout SuccessFully!!";
    public const string emailAlreadyExists = "Email Already Exists";
    public const string noInternet = "Please Check Your Internet Connection!!";
    public const string successFullyLoggedIn = "User login successfully!!";
    public const string doesNotHaveNumber = "Must Contain One Number!!";
    public const string doesNotHaveChar = "Must Contain Characters";
    public const string mailIsNotValid = "Is Not In A Proper Format Ex: abcd@a.com";
    public const string doesNotContainAtTheRate = "Must Contain @ Symbol";
    public const string doesNotHaveSpecialChar = "Must Contain One Special Character!!";
    public const string doesNotHaveUpperCaseChar = "MustContain One Upper Case Character!!";
    public const string doesNotHaveLowerCaseChar = "Must Contain One Lower Case Character!!";
    public const string doesNotHaveMinEightChar = "Must contain 8 Characters!!";
    public const string containedSpace = "Doesn't Contain Any Space";
    public const string passwordIsNull = "Should Be Like - Ex : Abc@1234";
    public const string isNull = "Cannot Be Blank";
    public const string wrongConfPassword = "PassWord Doesn't Match!!";
    public const string login = "Log In";
    public const string logout = "Log Out";
    public const string defaultUserName = "UserName";
    public const string showTrigger = "Straight";
    public const string hideTrigger = "Reverse";

}