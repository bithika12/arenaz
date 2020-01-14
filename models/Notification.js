const appRoot = require('app-root-path');
let Notification = require(appRoot +'/schema/Schema').notificationModel;
let User = require(appRoot +'/schema/Schema').User;

let Mongoose = require('mongoose');
//let mongoose = require(appRoot +'/config/mongoose');

let  Notifications ={};
//mongoose.set('debug', true);

Notifications.notifications =function(reqObj){
    //console.log(reqObj);return false;
    return new Promise((resolve,reject) => {
        Notification.find(reqObj).then(responses => {
            //console.log("notis",responses);
            resolve(responses);
        })
    });
};

Notifications.updateNotification =function(conditionObj,updateObj){
    return new Promise((resolve,reject) => {
        Notification.updateMany(conditionObj, {$set: updateObj}).then(responses => {
            resolve(responses);
        })
    });
};
Notifications.createNotification =function(reqObj){
    //console.log(reqObj);return false;
    return new Promise((resolve,reject) => {
        Notification.create(reqObj).then(responses => {
            resolve(responses);
        })
    });
};

module.exports =Notifications;