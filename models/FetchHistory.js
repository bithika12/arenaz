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
const fetchHistory = userId => {

    return new Promise((resolve, reject) => {

        Room.findHistory(userId).then(function (responseParams) {
            if(responseParams.length >0){
                let chart = [];
                let gameStatus;

                responseParams.map(function(entry) {
                    //console.log(entry.users);
                    let upDate=entry.updated_at;
                    let updatedTime=upDate.getTime();//in seconds
                    let currentTime=new Date().getTime();
                    const diff = currentTime - updatedTime;
                    let timeWithCurrent = Math.floor(diff / 1000 % 60);

                    let entusers=entry.users;
                    chart.push({
                        game_time: entry.game_time,
                        updated_at: entry.updated_at,
                        last_time:timeWithCurrent,
                        values: entusers.map(function(entry1) {
                            if(entry1.userId==userId){
                                if(entry1.isWin==1)
                                    gameStatus='VICTORY';
                                else if(entry1.isWin==2)
                                    gameStatus='DRAW';
                                else if(entry1.isWin==0)
                                    gameStatus='DEFEAT';
                            }
                            return {
                                gameResult:gameStatus,
                                userId: entry1.userId,
                                userName: entry1.userName,
                                userScore:entry1.total,
                                cupNumber:entry1.cupNumber};
                        })
                    });
                });

                console.log(chart);
                resolve(chart);
            }
            else{
                resolve({status:Constants.SUCCESS_STATUS,message:"No Data Found"});
            }


        }).catch(function (fetchHistoryErr) {
            reject({status:Constants.API_ERROR,message:fetchHistoryErr});
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
module.exports = { fetchHistory,userValidChk };