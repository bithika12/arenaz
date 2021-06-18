namespace RedApple.Api
{
    public static class Urls
    {
        public const string REGISTER = "registration";
        public const string USER_LOGIN = "login";
        public const string LOGOUT = "logout";
        public const string DELETE_ACCOUNT = "delete/account";
        public const string JSON = "json";
        public const string ACTIVATE_TOKEN = "activate";
        public const string CREATE_USER = "users";
        public const string USERGROUP_ASSIGN = "users/assign";
        public const string USERGROUP_UNASSIGN = "users/unassign";
        public const string USER_GROUPS = "usergroups";
        public const string USER_ACCESS_TOKEN = "login";
        public const string CLIENT_ACCESS_TOKEN = "login/client";
        public const string REFRESH_TOKEN = "login/client";
        public const string FORGOT_PASSWORD = "forgot/password";
        public const string TEAM = "teams";
        public const string MODIFY_TEAM = "/teams/{0}";
        public const string LIST_TEAMS = "/teams/joined/{0}/{1}/{2}/{3}";
        public const string INVITATION_TEAM = "/teams/{0}{1}{2}";
        public const string INVITATION_STATUS = "/invitations/{0}/{1}";
        public const string ACCEPT = "accept";
        public const string REJECT = "reject";
        public const string PROFILE = "profile";
        public const string USER = "user";
        public const string LIST_FAVOURITE_FILES = "/user/{0}/{1}/{2}/{3}/{4}/{5}";
        public const string USERS = "users";
        public const string PERMISSIONS = "permissions";
        public const string LIST_LICENSES = "/licenses/{0}/{1}/{2}/{3}";
        public const string MODIFY_LICENSE = "/licenses/{0}";

        public const string SESSIONS = "sessions";
        public const string MODIFY_SESSION = "/sessions/{0}";
        public const string LIST_SESSIONS = "/sessions/{0}/{1}/{2}/{3}";
        public const string ATTACH_DEATTACH_SESSION = "/sessions/{0}/{1}/{2}";
        public const string FILES = "files";
        public const string FORMAT_FILES = "/files/{0}";
        public const string DOWNLOAD_FILES = "/files/{0}/{1}";
        public const string LIST_FILES = "/files/{0}/{1}/{2}/{3}";
        public const string FAVOURITE_FILES = "favorites";
        public const string QUIZZES = "quizzes";
        public const string PARTICIPANTS = "participants";

        public const string ASSETSTORE_LOGIN = "assetstore/login";
        public const string ASSETSTORE_PRODUCTS = "assetstore/products";
        public const string ASSETSTORE_ADDTOCART = "/assetstore/products/{0}";
        public const string ASSETSTORE_CATEGORIES = "assetstore/categories";
        public const string ASSETSTORE_TAGS = "assetstore/tags";
        public const string ASSETSTORE_SEARCH = "assetstore/search";
        public const string ASSETSTORE_ASSETS = "assetstore/assets";
        public const string ASSETSTORE_AVILABLE_TIERS = "assetstore/available-tiers";

        public const string TEMP_PARTICIPATION_NOTIFICATION = "sessions/{0}/participants/{1}";

        public const string SAVE_SELECTION_DETAILS = "add/details";
        public const string GET_SELECTION_DETAILS = "get/details";

        public const string GET_GAME_HISTORY = "game/history";
        public const string GET_USER_GAME_HISTORY = "user/game/history";
        public const string FETCH_NOTIFICATIONS = "fetch/notifications";
        public const string CHANGE_NOTIFICATION_STATUS = "change/notification/status";
        public const string FETCH_LEADERBOARD = "user/board";

        public const string FETCH_UNREAD_MESSAGE = "fetch/unread/message";

        public const string APP_VERSION_CHECK = "get/app/version";
        public const string GET_MASTER_DATA = "user/get/master";
        public const string GAME_ALLGAMES = "game/allgames";

        public const string WALLET_USER_GET_COIN = "user/get/coin";
        public const string WALLET_CURRENCY_PRICE = "currency/price";
        public const string WALLET_REQUEST_DEPOSIT = "request/deposit";
        public const string WALLET_REQUEST_WITHDRAW = "request/withdraw";
        public const string WALLET_CANCEL_DEPOSIT = "cancel/deposit";
        public const string WALLET_CONFIRM_DEPOSIT = "confirm/deposit";
        public const string WALLET_CHECK_TRANSACTION_STATUS = "check/transaction/status";
        public const string WALLET_HISTORY = "transaction-list-user";
        public const string EMAIL_VERIFY = "verify/code";
        public const string RESEND_EMAIL = "resend/mail";
        public const string ADD_USER_COIN = "admin/add-user-coins";
    }
}
