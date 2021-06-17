"use strict";
const appRoot = require('app-root-path');
const User = require(appRoot + '/models/User');
const Constants = require(appRoot + '/config/constants');
var fs = require('fs');
var handlebars = require('handlebars');


/**
 * @desc Get email with username
 *
 * @param {String} username
 */
const callEmailChkByUserName = userName => {

    return new Promise((resolve, reject) => {

        User.findUser({userName: userName}).then(function (responseParams) {

                resolve(responseParams)


        }).catch(function (checkEmailExistsErr) {
            reject({status:Constants.API_ERROR,message:"The credentials you entered is not correct. Please try again or contact support."});
      });
    });
};

/**
 * @desc update password
 *
 * @param {String} email,password
 */
const callEmailUpdatePassword = (responseParams,hashPassword) => {

    return new Promise((resolve, reject) => {

        User.updateOne({ _id :responseParams.id},{$set :{password:hashPassword}}).then(function (res) {
            resolve(responseParams)

        }).catch(function (updateEmailErr) {
            reject({status:Constants.API_ERROR,message:"Password update not occured successfully."});
        });
    });
};

/**
 * @desc email send
 *
 * @param {String} id,email,password
 */

const callEmailSend = (res,password) => {

    return new Promise((resolve, reject) => {

        // mail send /////////////////////////////////////////////////////////
        let readHTMLFile = function(path, callback) {
            fs.readFile(path, {encoding: 'utf-8'}, function (err, html) {
                if (err) {
                    throw err;
                    callback(err);
                }
                else {
                    callback(null, html);
                }
            });
        };
        readHTMLFile(appRoot  + '/views/email/forgotpasswordemail.jade', function(err, html) {
            let template = handlebars.compile(html);
            let replacements = {
                Username: res.userName,
                password:password
            };
            let htmlToSend = template(replacements);
            let mailOptions = {
                from: 'bithikamahato88@gmail.com',
                to : res.email,
                //to : 'bithikamahato88@gmail.com',
                subject : 'Arena-Z Login Credentials',
                html : htmlToSend
            };
            Constants.TRANSPORTER.sendMail(mailOptions, function (error, response) {
                if (error) {
                    console.log(error);
                    callback(error);
                }
                else{
                    resolve(true);
                }
            });
        });

        ///////////////////mail send ///////////////////////////////////////////////////////////////
    });
};


/**
 * @desc Get email with username
 *
 * @param {String} email
 */
const callEmailChkByEmail = email => {

    return new Promise((resolve, reject) => {

        User.findUser({email: email}).then(function (responseParams) {

            resolve(responseParams)


        }).catch(function (checkEmailExistsErr) {
            reject({status:Constants.API_ERROR,message:"The credentials you entered is not correct. Please try again or contact support."});
        });
    });
};

const callEmailSendLogin = (email,verifyCode) => {

    console.log("userdetails "+(verifyCode))

    return new Promise((resolve, reject) => {

        // mail send /////////////////////////////////////////////////////////
        let readHTMLFile = function(path, callback) {
            fs.readFile(path, {encoding: 'utf-8'}, function (err, html) {
                if (err) {
                    throw err;
                    callback(err);
                }
                else {
                    callback(null, html);
                }
            });
        };
        readHTMLFile(appRoot  + '/views/email/verifyemail.jade', function(err, html) {
            let template = handlebars.compile(html);
            let code1= Math.floor((Math.random() * 90) + 1);
            let replacements = {
                code: verifyCode
               
            };
            let htmlToSend = template(replacements);
            let mailOptions = {
                from: email,
                to : email,
                //to : 'bithikamahato88@gmail.com',
                subject : 'Arena-Z Verify Email',
                html : htmlToSend
            };
            Constants.TRANSPORTER.sendMail(mailOptions, function (error, response) {
                if (error) {
                    console.log(error);
                    callback(error);
                }
                else{
                    resolve(true);
                }
            });
        });

        ///////////////////mail send ///////////////////////////////////////////////////////////////
    });
};


module.exports = { callEmailSendLogin,callEmailChkByUserName,callEmailUpdatePassword,callEmailSend,callEmailChkByEmail };