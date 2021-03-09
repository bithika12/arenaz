
public enum Page
{
    UIPanel,
    GameplayPanel,
    CharacterSelectionPanel,
    AccountAccessDetailsPanel,
    AccountAccesOverlay,
    RegistrationOverlay,
    LogINOverlay,
    ForgotPasswordOverlay,
    LevelSelectionPanel,
    SettingsPanel,
    PlayerColorChooser,
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
    ComingSoonOverlay,
    PopUpTextAccountAccess,
    PopUpTextSettings,
    SelfHitScore,
    OpponentHitScore,
    HitScore,
    GameLoadingPanel,
    PopupPanel,
    RegionPopup,
    InternetConnectionLostPanel,
    GameplayUIPanel,
    TrainingUIPanel,
    DeleteAccountAlertOverlay,
    TrainingPlayerWinPanel,
    NotEnoughCoinOverlay,
    NewUserCongratulationPanel,
    SurrenderedPopupPanel,
    ReconnectCountdownPanel,
    VersionCheckPanel,
    WalletDepositRequestPanel,
    WalletDepositConfirmPanel,
    WalletWithdrawPanel,
    TrainingPopupPanel,
    CheckInternetConnectionPanel
}

public enum ERace
{
    Canines,
    Kepler,
    Cyborg,
    CyborgSecond,
    Human,
    Ebot,
    KeplerSecond,
    HumanFemale,
    HumanFemaleSecond,
    HumanFemaleThird,
    EbotSecond,
    EbotThird,
    CaninesSecond,
    CaninesThird
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
    Black,
    DarkBlue,
    DarkGreen,
    Grey,
    LightBlue,
    LightGreen,
    Orange,
    Red,
    Teal,
    White,
    Yellow,
    None,
    HandImage,
}

public enum EColor
{
    None = 0,
    DarkBlue,
    DarkGreen,
    Grey,
    LightBlue,
    LightGreen,
    Orange,
    Red,
    Teal,
    White,
    Yellow,
}

public enum PlayerPrefsValue
{
    LoginID,
    Password,
    Logout,
    SelectedRace,
    SelectedColor,
    SelectedCharacter,
}

public enum Hide
{
    previous,
    none
}

public enum Show
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
    Medium,
    rounded
}

public enum GameobjectTag
{
    Player,
    DartBoard
}

public enum SceneType
{
    Loading,
    LevelSelection,
    Gameplay
}

public enum EWinningStatus
{
    Defeat = 0,
    Victory = 1,
    Draw = 2
}
public enum EAudioClip
{
    GameplayBg,
    AudienceCheering,
    DartHit,
    DartMiss,
    Timeout,
    Intro,
    GameLose,
    NumberDisplay,
    WindowChange,
    GameWin,
    Bust,
    ButtonClick,
    MainMenu,
    PlayerSkinSelection,
    Countdown,
    MoneyWin,
}

public enum EGameOverStatus
{
    Lose,
    Win, 
    Draw,
}

public static class DownloadUrl
{
    public static string url;
}

public static class GameResources
{
    public const string characterImageFolderPath = "Characters";
    public const string dartImageFolderPath = "Darts";
    public const string dartPrefabFolderPath = "Prefabs/Darts";
}

public static class User
{
    public static string UserName;
    public static string UserId;
    public static string UserEmailId;
    public static string UserAccessToken;
    public static string UserRace;
    public static string UserCountry;
    public static string UserLanguage;
    public static string DartName;
    public static string UserColor;
    public static string RoomName;
    public static int UserCoin;
    public static int UserCup;
    public static string UserSelectedGame;
}

public static class SettingData
{
    public static float BGMVolume = 1.0f;
    public static float SFXVolume = 1.0f;
}

public static class Opponent
{
    public static string opponentName;
    public static string opponentId;
    public static string opponentRace;
    public static string opponentColor;
    public static string dartName;
}

public static class ScoreData
{
    public static int requiredScore;
}

public static class ConstantInteger
{
    public const int totalDartPointsForProjectileMove = 10;
    public const int inputPosNo = 5;
    public const int fewPosNo = 10;
    public const int shootingAngle = 45;
    public const int totalGameScore = 100;
    public const int timerValue = 10;
    public const int autoLoginWait = 1;
}

public static class ConstantStrings
{
    public static readonly string USER_SAVE_FILE_KEY = "AD_USER_SAVE.sv";
    public static readonly string ROOM_VALUE = "Room_Value";
    public static readonly string ROOM_NAME = "Room_Name";

    public const string successFullyRegisterd = "You Have Successfully Registered. You Will Be Logged In.";
    public const string successFullyLoggedOut = "User Loggedout SuccessFully!";
    public const string emailAlreadyExists = "Email address entered already exists. Please use forgot password to login.";
    public const string noInternet = "Please Check Your Internet Connection.";
    public const string successFullyLoggedIn = "User login successfully.";
    public const string doesNotHaveNumber = "Your password must contain at least one number.";
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
    public const string userNameSpecialCharacter = "Your Username can't have Special Characters.";
    public const string userNameLimitOverflow = "Username maximum limit is 13 characters";
    public const string wrongConfPassword = "Your password and confirmation of password must match.";
    public const string wrongEmailAndUsername = "The credentials you entered is not correct. Please try again or contact support.";
    public const string rightEmailAndUserName = "Your login credentials have been emailed to you.";
    public const string login = "Log In";
    public const string logout = "Log Out";
    public const string defaultUserName = "UserName";
    public const string showTrigger = "Straight";
    public const string hideTrigger = "Reverse";
    public const string dart = "Dart";
    public const string turnCancelled = "TurnCancelled";

    public const string emptyFieldWarning = "This field can't be empty!";
    public const string newAccountWelcomeFirst = "You have successfully created a new account. As a welcome gift we have deposited";
    public const string newAccountWelcomeSecond = "free coins into your account so you can start playing.";
}