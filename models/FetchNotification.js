"use strict";
const appRoot = require('app-root-path');
const Room = require(appRoot + '/models/Room');
const Constants = require(appRoot + '/config/constants');
var fs = require('fs');
var handlebars = require('handlebars');
const User = require(appRoot + '/models/User');
const Notifications = require(appRoot + '/models/Notification');
let mongoose = require('mongoose');


/**
 * @desc fetch game history
 *
 * @param {String} username
 */
const fetchNotification = userId => {

    return new Promise((resolve, reject) => {

     //fetch notifications
        Notifications.notifications1({received_by_user:userId,read_unread:0,created_at:{
        $gte: new Date((new Date().getTime() - (35 * 24 * 60 * 60 * 1000)))
    }
}).then(function (responseParams) {
          //Notifications.notifications1({received_by_user:userId}).then(function (responseParams) {

            resolve(responseParams)

        });
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
//user leaderboard
const fetchDetails = userEmail => {

    return new Promise((resolve, reject) => {

        User.findLeaderboard().then(function (responseParams) {
            //let usrId = responseParams._id+"";
            resolve(responseParams)

        }).catch(function (fetchHistoryErr) {
            reject({status:Constants.API_ERROR,message:fetchHistoryErr});
        });
    });
};
//changeNotificationStatus
const changeNotificationStatus = notificationId => {
    console.log("ok");
    console.log(mongoose.Types.ObjectId(notificationId));

    return new Promise((resolve, reject) => {
       console.log("mongoose.Types.ObjectId(notificationId"+mongoose.Types.ObjectId(notificationId));
     //fetch notifications
        Notifications.updateNotification({_id:mongoose.Types.ObjectId(notificationId)},{read_unread:1}).then(function (responseParams) {
            resolve(responseParams)

        });
    });
};
module.exports = { fetchNotification,userValidChk,fetchDetails,changeNotificationStatus };