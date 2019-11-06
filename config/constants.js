/* NODE MAILER SETUP*/
var nodemailer = require('nodemailer');
var smtpConfig = {
                pool: true,
                host: 'smtp.gmail.com',
                port: 587,
                secure: false,
                auth: {
                  user: 'runa.redappletech@gmail.com',
                  pass: 'fwasjaiayhhsmcem'       
                },
                tls: {
                  rejectUnauthorized: false
                }
    };

 var transporter = nodemailer.createTransport(smtpConfig);

    //  transporter.verify(function(error, success) {
    //    if (error) {
    //      console.log(error);
    //    } else {
    //      console.log("Server is ready to take our messages");
    //    }
    // }); 

/** constants declaration **/
module.exports = {
    /* Status code*/

    ERROR_STATUS                : "0",
    SUCCESS_STATUS              : "1",
    WARINING_STATUS             : "3",
    UNIQUIE_EMAIL               : "2",
    PARAMMISSING_STATUS         : "0",
    EMAIL_NOT_EXISTS            : "4",
    INVALID_OTP                 : "5",
    INVALID_AUTHERIZATION       : "5",
    INVALID_TOKEN               : "10",
    /* Node mailer*/    
    TRANSPORTER                 : transporter,
    /* Link */
    BASEURL                     : "http://13.232.233.114:3000/",
    FILEURL                     : "http://13.232.233.114:3000/images/",
    FACE_BOOK_LINK              : "www.facebook.com",
    TWITTER_LINK                : "www.twitter.com",
    API_URL_PATH                : "",

    /*FILE PATH*/

    GAME_IMAGE_PATH             : "images/game_image/",
    PROFILE_IMAGE_PATH          : "images/user_images/",
    PROFILE_THUMB_IMAGE_PATH    : "images/user_images/thumb_images/",
    /*PAGGIGNATION SET UP*/

    LIMIT                       : 20,
    OFFSET                      : 0,
    /*DEFAULT VALUE SET*/
    
    DEFAULT_INTEGER             : 0,
    DEFAULT_STRING              : "",
    DEFAULT_STRING_NO           : "",
    DEFAULT_MONEY               : 500,
    /* NOtification options*/


};