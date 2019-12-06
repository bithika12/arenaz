
public enum Page
{
    CharacterSelectionPanel,
    AccountAccessDetailsPanel,
    AcoountAccesOverlay,
    RegistrationOverlay,
    LogINOverlay,
    ForgotPasswordOverlay,
    LevelSelectionPanel,
    SettingsPanel,
    LoggedInText,
    LogOutAlertOverlay,
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

public enum AccountAccessType
{
    Registration,
    Login
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
    public const string successFullyRegisterd = "You Have Successfully Registered. You Will Be Logged In.";
    public const string successFullyLoggedOut = "User Loggedout SuccessFully!";
    public const string emailAlreadyExists = "Email address entered already exists. Please use forgot password to login.";
    public const string noInternet = "Please Check Your Internet Connection.";
    public const string successFullyLoggedIn = "User login successfully.";
    public const string doesNotHaveNumber = "Your password must contain at least number.";
    public const string doesNotHaveChar = "Must Contain Characters.";
    public const string mailIsNotValid = "Invalid Email address. Please try again.";
    public const string passwordIsNotValid = "Invalid Password. Please try again.";
    public const string doesNotContainAtTheRate = "Must Contain @ Symbol.";
    public const string doesNotHaveSpecialChar = "Your password must contain at least one special character ’#@$()*.";
    public const string doesNotHaveUpperCaseChar = "Your password must contain at least one uppercase character.";
    public const string doesNotHaveLowerCaseChar = "Your password must contain at least one lowercase character.";
    public const string doesNotHaveMinThreeChar = "UserName Should Be At Least 3 Characters.";
    public const string doesNotHaveMinEightChar = "Your password must be at least 8 characters.";
    public const string userNameContainedSpace = "UserName Doesn't Contain Any Space.";
    public const string passwordContainedSpace = "Password Doesn't Contain Any Space.";
    public const string loginEmailPasswordBlank = "The Email Address And Password You Entered Is Incorrect. Please Try Again.";
    public const string userNameBlank = "Your Username Is Incorrect. Please try Again.";
    public const string wrongConfPassword = "Your password and confirmation of password must match.";
    public const string wrongEmailAndUsername = "The credentials you entered is not correct. Please try again or contact support.";
    public const string rightEmailAndUserName = "Your login credentials have been emailed to you.";
    public const string login = "Log In";
    public const string logout = "Log Out";
    public const string defaultUserName = "UserName";
    public const string showTrigger = "Straight";
    public const string hideTrigger = "Reverse";
}