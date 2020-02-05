const appRoot = require('app-root-path');
let Notification = require(appRoot +'/schema/Schema').notificationModel;
let User = require(appRoot +'/schema/Schema').User;

let Mongoose = require('mongoose');
//let mongoose = require(appRoot +'/config/mongoose');
const moment=require("moment");
let  Notifications ={};
//mongoose.set('debug', true);

Notifications.notifications1 =function(reqObj){
    //console.log(reqObj);return false;
    return new Promise((resolve,reject) => {
        Notification.find(reqObj,{message:1,created_at:1}).then(responses => {
            //console.log("notis",responses);
            //let new_time = moment(responses.created_at).format('YYYY-MM-DD HH:mm:ss');

            let chart = [];
            responses.map(function(entry) {
                let new_time = moment(entry.created_at).format('YYYY-MM-DD HH:mm:ss');
            chart.push({
                name: "",
                message: entry.message,
                cretaedTime:new_time,
                type: "message",
                description:"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum."

            });
        });
            resolve(chart);
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