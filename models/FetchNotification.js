"use strict";
const appRoot = require('app-root-path');
const Room = require(appRoot + '/models/Room');
const Constants = require(appRoot + '/config/constants');
var fs = require('fs');
var handlebars = require('handlebars');
const User = require(appRoot + '/models/User');


/**
 * @desc fetch game history
 *
 * @param {String} username
 */
const fetchNotification = userId => {

    return new Promise((resolve, reject) => {

     //fetch notifications

    });
};
/**
 * @desc chk user role
 *
 * @param {String} username
 */


const userValidChk = userEmail => {

    return new Promise((resolve, reject) => {

        User.findDetailsByEmail({email: userEmail}).then(function (responseParams) {
            let usrId = responseParams._id+"";
            resolve(usrId)

        }).catch(function (fetchHistoryErr) {
            reject({status:Constants.API_ERROR,message:fetchHistoryErr});
        });
    });
};
module.exports = { fetchNotification,userValidChk };