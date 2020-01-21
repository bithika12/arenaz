"use strict";
const appRoot = require('app-root-path');
const Room = require(appRoot + '/models/Room');
var Rooms = require('../schema/Schema').roomModel;
var Coins = require('../schema/Schema').coinModel;


const Constants = require(appRoot + '/config/constants');
var fs = require('fs');
var handlebars = require('handlebars');
const User = require(appRoot + '/models/User');
const Role = require(appRoot + '/models/Role');
const Coin = require(appRoot + '/models/Coin');
//const Room = require(appRoot + '/models/Room');
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

/**
 * @desc chk user role for admin
 *
 * @param {String} username
 */


const userValidChkAdmin = userEmail => {

    return new Promise((resolve, reject) => {
        Role.findOne({ slug: "admin"},{_id: 1,name:1,slug:1}).then(roledetails=> {
            let roleId = roledetails._id+"";
        User.findDetailsByEmail({email: userEmail}).then(function (responseParams) {
            let usrId = responseParams._id+"";
            let usrroleId = responseParams.roleId+"";
            if(usrroleId== roleId)
              resolve(usrId)
            else
                reject({status:Constants.API_ERROR,message:"You are not allowed"});

        }).catch(function (fetchHistoryErr) {
            reject({status:Constants.API_ERROR,message:fetchHistoryErr});
        });

       });
    });
};
 /**
 * @desc fetch game history for admin panel
 *
 * @param {String} username
 */
const fetchHistoryAdmin12 = userId => {

    return new Promise((resolve, reject) => {

        Room.findHistoryAdmin(userId).then(function (responseParams) {
            if(responseParams.length >0){
                let chart = [];
                let gameStatus;
                let winnerUsername;
                let winnerScore;
                let winnerCup;
                let loserUsername;
                let loserScore;
                let loserCup;
                let firstUser;
                let secondUser;
                let firstScore;
                let secondScore;

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
                        game_name:entry.name,
                        values: entusers.map(function(entry1) {
                            //if(entry1.userId==userId){
                                if(entry1.isWin==1) {
                                    gameStatus = 'VICTORY';
                                    winnerUsername = entry1.userName;
                                    winnerScore=entry1.total;
                                    winnerCup=entry1.cupNumber;

                                }

                                else if(entry1.isWin==2)
                                    gameStatus='DRAW';
                                else if(entry1.isWin==0)
                                    gameStatus='DEFEAT';
                                    loserUsername=entry1.userName;
                                    loserScore=entry1.total;
                                    loserCup =entry1.cupNumber;
                            //}

                            return {
                                gameResult:gameStatus,
                                userId: entry1.userId,
                                userName: entry1.userName,
                                userScore:entry1.total,
                                cupNumber:entry1.cupNumber,
                                loserUsername:loserUsername,
                                loserScore:loserScore,
                                loserCup:loserCup,
                                winnerUsername:winnerUsername,
                                winnerScore:winnerScore,
                                winnerCup:winnerCup
                                 };
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

const fetchHistoryAdmin = userId => {

    return new Promise((resolve, reject) => {

        Room.findHistoryAdmin(userId).then(function (responseParams) {
            if(responseParams.length >0){
                let chart = [];
                let gameStatus;
                let winnerUsername;
                let winnerScore;
                let winnerCup;
                let loserUsername;
                let loserScore;
                let loserCup;
                let firstUser;
                let secondUser;
                let firstScore;
                let secondScore;

                responseParams.map(function(entry) {
                    //console.log(entry.users);
                    let upDate=entry.updated_at;
                    let updatedTime=upDate.getTime();//in seconds
                    let currentTime=new Date().getTime();
                    const diff = currentTime - updatedTime;
                    let timeWithCurrent = Math.floor(diff / 1000 % 60);
                    let winnerUserId=(entry.users[0]['isWin']==1 ? entry.users[0]['userName'] : entry.users[1]['userName']);
                    //let entusers=entry.users;
                    chart.push({
                        //game_time: entry.game_time,
                        //updated_at: entry.updated_at,
                        //last_time:timeWithCurrent,
                        game_name:entry.name,
                        first_user:entry.users[0]['userName'],
                        second_user:entry.users[1]['userName'],
                        first_user_score:entry.users[0]['total'],
                        second_user_score:entry.users[1]['total'],
                        winner_user:winnerUserId
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
const updateProfileAdmin =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        if(updateObj.password)
            updateObj.password  =  password.hashPassword(updateObj.password);
        User.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}

const modifyProfileDetails =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        User.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}
const fetchRoleName =(condObj) =>{
    return  new Promise((resolve,reject) => {
        Role.find(condObj,{_id: 1,name:1,slug:1}).then(response=> {
            resolve(response[0]._id+"")
        }).catch(err=>{
            reject(err);
        })

    });
}
//fetch coin list
//fetchCoin
const fetchCoin =(condObj) =>{
    return  new Promise((resolve,reject) => {
        Coin.find({status:"active"},{_id: 1,number:1}).then(response=> {
            resolve(response)
        }).catch(err=>{
            reject(err);
        })

    });
}
//add coin
const addCoin =(updateObj) =>{
    return  new Promise((resolve,reject) => {

        Coin.find({number:updateObj.number},{_id: 1,number:1}).then(res=> {
            if(res.length){
                reject({message:"Already added"});
            }
            else{
                Coin.create(updateObj).then(response => {
                    return resolve(response);
                }).catch(err => {
                    return reject(err);
                });
            }
        }).catch(err=>{
            reject(err);
        })

    });
}
//updatRoomAdmin
//fetchRoleName
//modifyProfileDetails
const updateRoomAdmin =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        Rooms.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}
//updateCoinAdmin
const updateCoinAdmin =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        Coins.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}
module.exports = { updateCoinAdmin,updateRoomAdmin,addCoin,fetchHistory,userValidChk,userValidChkAdmin,fetchHistoryAdmin,updateProfileAdmin,modifyProfileDetails,fetchRoleName,fetchCoin };