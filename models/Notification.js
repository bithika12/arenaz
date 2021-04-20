const appRoot = require('app-root-path');
let Notification = require(appRoot +'/schema/Schema').notificationModel;
let User = require(appRoot +'/schema/Schema').User;

let Mongoose = require('mongoose');
//let mongoose = require(appRoot +'/config/mongoose');
const moment=require("moment");
let  Notifications ={};
let messageSchema = require(appRoot +'/schema/Schema').messageModel;
//mongoose.set('debug', true);

Notifications.notifications1 =function(reqObj){
    //console.log(reqObj);return false;
    return new Promise((resolve,reject) => {
        Notification.find(reqObj,{subject:1,message:1,created_at:1,read_unread:1,_id:1}).then(responses => {
            //console.log("notis",responses);
            //let new_time = moment(responses.created_at).format('YYYY-MM-DD HH:mm:ss');

            let chart = [];
            responses.map(function(entry) {
                let new_time = moment(entry.created_at).format('YYYY-MM-DD HH:mm:ss');
            chart.push({
                name: "",
                message: entry.subject,
                cretaedTime:new_time,
                type: "message",
                read_status:entry.read_unread,
                notification_id:entry._id,
                description:entry.message 

                //description:"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum."

            });
        });
            resolve(chart);
        })
    });
};

Notifications.updateNotification =function(conditionObj,updateObj){
    return new Promise((resolve,reject) => {
        Notification.update(conditionObj, {$set: updateObj}).then(responses => {
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

//createMessage
Notifications.createMessage =function(reqObj){
    //console.log(reqObj);return false;
    return new Promise((resolve,reject) => {
        messageSchema.create(reqObj).then(responses => {
            resolve(responses);
        })
    });
};
//acknowledgeMessage
Notifications.acknowledgeMessage =function(reqObj){
    //console.log(reqObj);return false;
    return new Promise((resolve,reject) => {
        messageSchema.update(reqObj, {$set: {seen_status:1}}).then(responses => {
            resolve(responses);
        })
    });
};
//viewMessage
Notifications.viewMessage =function(reqObj){
    //console.log(reqObj);return false;
    return new Promise((resolve,reject) => {
       messageSchema.find(reqObj,{from_user_name:1,message:1,created_at:1,to_user_name:1,room_name:1,seen_status:1,_id:1}).then(responses => {

        //messageSchema.update(reqObj, {$set: {seen_status:1}}).then(responses => {
            resolve(responses);
        })
    });
};
module.exports =Notifications;