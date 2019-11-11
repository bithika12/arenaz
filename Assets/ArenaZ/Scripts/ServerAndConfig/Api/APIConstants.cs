using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ
{
    public static class APIConstants
    {
        #region API_METHOD_NAMES

        public const string API_REGISTER = "register";

        public const string API_SESSIONS = "sessions";
        public const string API_FILES = "files";
        public const string API_FAVOURITE_FILES = "favorites";
        public const string API_QUIZZES = "quizzes";
        public const string API_PARTICIPANTS = "participants";

        public const string API_ASSETSTORE_LOGIN = "assetstore/login";
        public const string API_ASSETSTORE_PRODUCTS = "assetstore/products";
        public const string API_ASSETSTORE_CATEGORIES = "assetstore/categories";
        public const string API_ASSETSTORE_TAGS = "assetstore/tags";
        public const string API_ASSETSTORE_SEARCH = "assetstore/search";
        public const string API_ASSETSTORE_ASSETS = "assetstore/assets";
        public const string API_ASSETSTORE_AVILABLE_TIERS = "assetstore/available-tiers";
        #endregion


        #region API_ATTRIBUTES

        public const string NAME = "name";
        public const string FIRST_NAME = "first_name";
        public const string LAST_NAME = "last_name";
        public const string USER_NAME = "username";
        public const string EMAIL_ID = "email";
        public const string PASSWORD = "password";
        public const string CONFIRM_PASSWORD = "c_password";
        public const string LOCATION = "location";
        

        public const string STATUS = "status";
        #endregion


        #region RESPONSE_CODES

        public const int successCode = 200;
        public const int successCreate = 201;
        public const int badRequest = 400;
        public const int resourceNotFound = 404;

        #endregion


        #region PARAMS

        private const string baseUrl = "http://skylect.local/";
        private const string headAccept = "accept";
        private const string headAuthorization = "Authorization";
        private const string headContent = "Content-Type";
        private const string headAcceptValue = "";
        private const string headAuthorizationValue = "";
        private const string headContentValue = "";

        #endregion

        /// <summary>
        /// Base API URL.
        /// </summary>
        /*public static string BaseURL
        {
            get { return baseUrl; }
        }*/

        /// <summary>
        /// header type accept.
        /// </summary>
        public static string HeadAccept
        {
            get { return headAccept; }
        }

        /// <summary>
        /// header type authorization.
        /// </summary>
        public static string HeadAuthorization
        {
            get { return headAuthorization; }
        }

        /// <summary>
        /// header type content.
        /// </summary>
        public static string HeadContent
        {
            get { return headContent; }
        }

        /// <summary>
        /// header type accept value.
        /// </summary>
        public static string HeadAcceptValue
        {
            get { return headAcceptValue; }
        }

        /// <summary>
        /// header type authorization value.
        /// </summary>
        public static string HeadAuthorizationValue
        {
            get { return headAuthorizationValue; }
        }

        /// <summary>
        /// header type content value.
        /// </summary>
        public static string HeadContentValue
        {
            get { return headContentValue; }
        }

        /// <summary>
        /// success response code.
        /// </summary>
        public static int Success
        {
            get { return successCode; }
        }

        /// <summary>
        /// Gets the create success.
        /// </summary>
        /// <value>The create success.</value>
        public static int CreateSuccess
        {
            get { return successCreate; }
        }
    }
}
