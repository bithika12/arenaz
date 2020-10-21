"use strict";
const appRoot = require('app-root-path');
const Room = require(appRoot + '/models/Room');
var Rooms = require('../schema/Schema').roomModel;
var Coins = require('../schema/Schema').coinModel;
//Versions
var Versions = require('../schema/Schema').versionModel;

let Notification = require(appRoot +'/schema/Schema').notificationModel;
var underscore  = require('underscore');
//Transaction
let Transaction = require(appRoot +'/schema/Schema').userTransactionModel;


const Constants = require(appRoot + '/config/constants');
var fs = require('fs');
var handlebars = require('handlebars');
const User = require(appRoot + '/models/User');
const Role = require(appRoot + '/models/Role');
const Coin = require(appRoot + '/models/Coin');
const Match = require(appRoot + '/models/Game');
var User1 = require('../schema/Schema').userModel; 
const moment=require("moment");
//const Room = require(appRoot + '/models/Room');
/**
 * @desc fetch game history
 *
 * @param {String} username
 */ 

 const fetchHistory12 = userId => {

    return new Promise((resolve, reject) => {

        Room.findHistory(userId).then(function (responseParams) {
            let a=JSON.parse(responseParams);
            console.log("90"+(a.length));
            //resolve(a);

            if(a.length >0){
                let chart = [];
                let gameStatus;

                a.map(function(entry,key) {
                    console.log("dateDifference"+entry);

                    let upDate=entry.updated_at;
                    let updatedTime=upDate.getTime();//in seconds
                    console.log("updated time in sec"+updatedTime);
                    let currentTime=new Date().getTime();
                    console.log("currentTime sec"+currentTime);

                    const diff = currentTime - updatedTime;
                    let timeWithCurrent = Math.floor(diff / 1000 % 60);
                    let gameCoin;

                    let entusers=entry.users;
                    
                });

                //let res={details:chart};
                //let obj = _.extend({}, chart);
               // console.log(obj);

                console.log(chart);
                //resolve(chart);
            }
            else{
                resolve({status:Constants.SUCCESS_STATUS,message:"No Data Found"});
            }




        }).catch(function (fetchHistoryErr) {
            reject({status:Constants.API_ERROR,message:fetchHistoryErr});
        });
    });
};

function getNatural(num) {
    return parseFloat(num.toString().split(".")[0]);
}
const fetchHistory = userId => {

    return new Promise((resolve, reject) => {

        Room.findHistory(userId).then(function (responseParams) {
            console.log("responseParams"+typeof(responseParams));

            console.log("responseParams123"+JSON.stringify(responseParams));
            //let responseParams1=JSON.parse(responseParams);
            //console.log("responseParams1"+(responseParams1.length));
            //console.log("responseParams1"+responseParams);
            //let userarr=[];
            //userarr.push(responseParams);
            //console.log("userarr"+userarr.length);
            if(responseParams.length >0){
                let chart = [];
                let gameStatus;

                responseParams.map(function(entry) {
                    //console.log("dateDifference"+entry.dateDifference);
                    if(entry.updated_at){ 
                    let upDate=entry.updated_at;
                    let dt=moment(upDate).format('MM/DD/YYYY HH:mm:ss');
                    let dt1=moment().format('MM/DD/YYYY HH:mm:ss');

                    console.log("dt"+dt);
                    console.log("dt1"+dt1);

                     
                     var config = "MM/DD/YYYY HH:mm:ss";
                    var ms = moment(dt1, config).diff(moment(dt,config));

                    var dms=moment.duration(ms);
                    var d = (moment.duration(ms))/1000;
                    console.log("d"+d);
                    let timePeriod="";
                    let timePeriodType="";
                    let beforeRoundtimePeriod="";

                    if(d > 24*3600)
                    {

                        //no. of day
                        //timePeriod=Math.floor(dms.asDays());
                        beforeRoundtimePeriod=dms.asDays();
                        timePeriod=Math.floor(beforeRoundtimePeriod);
                        timePeriodType="Days";
                    }
                    else if(d > 3600 && d < 24*3600){

                        //hrs.
                        beforeRoundtimePeriod=dms.asHours();
                         timePeriod=Math.floor(beforeRoundtimePeriod);
                         timePeriodType="Hours";


                    }
                    else if(d < 3600){

                        beforeRoundtimePeriod=d/60;

                        timePeriod=Math.floor(beforeRoundtimePeriod);
                        timePeriodType="Minutes";


                        //minutes.


                    }

                    //var s = Math.floor(d.asHours()) + moment.utc(ms).format(":mm:ss");

                    console.log("timePeriod"+timePeriod);    
                    //let dt2=moment.startOf('day').fromNow();
                    //console.log("dt2"+days);
                    //console.log("upDate"+upDate);
                    let updatedTime=upDate.getTime();//in seconds
                    console.log("updated time in sec"+updatedTime);
                    let currentTime=new Date().getTime();
                    console.log("currentTime"+new Date());
                    console.log("currentTime sec"+currentTime);

                    const diff = currentTime - updatedTime;
                    console.log("diff"+diff);
                    let timeWithCurrent = Math.floor(diff / 1000 % 60);
                    let gameCoin;

                    let entusers=entry.users;
                    chart.push({
                        created_at:entry.created_at,
                        game_time: entry.game_time,
                        //updated_at: entry.updated_at,
                        last_time:Math.floor(beforeRoundtimePeriod),//timePeriod,
                        timePeriodType: timePeriodType,
                        //beforeRoundtimePeriod: beforeRoundtimePeriod,
                        //last_time:timeWithCurrent,
                        gameDetails: entusers.map(function(entry1) {
                            //console.log("key1"+key1);

                         //User1.findOne({userName:entry1.userName},{_id: 1,firstName:1,lastName:1}).then(userRes=> {
                             // console.log("ok1"+userRes);
                             let firstName=(!entry1.firstName)? '' : entry1.firstName;
                             let lastName=(!entry1.lastName)? '' : entry1.lastName;
                             let userNm=firstName+" "+lastName;
                             userNm=entry1.userName;
                              //let userNm=userRes.firstName+userRes.lastName;
                          
                            /*if(entry1.userId==userId){
                                if(entry1.isWin==1) {
                                    gameStatus = 'VICTORY';
                                    gameCoin = entry1.roomCoin * 2;
                                }
                                else if(entry1.isWin==2) {
                                    gameStatus = 'DRAW';
                                    gameCoin = entry1.roomCoin;
                                }
                                else if(entry1.isWin==0) {
                                    gameStatus = 'DEFEAT';
                                    gameCoin = entry1.roomCoin;
                                }
                            }*/
                           // User1.find({userName:entry1.userName}).then(userRes=> {
                              
                            return {
                                //gameResult:gameStatus,
                                userId: entry1.userId,
                                userName: userNm,
                                userScore:entry1.total,
                                cupNumber:entry1.cupNumber,
                                colorName:entry1.colorName,
                                raceName:entry1.raceName,
                                coinNumber:entry1.roomCoin,
                               // gameCoin:gameCoin,
                                gameResult:entry1.isWin
                            };

                        //});
                        })
                    });

                    ////

                }
                 else{
                    console.log("po0");
                 }
                });

                //let res={details:chart};
                //let obj = _.extend({}, chart);
               // console.log(obj);

                console.log("chart"+JSON.stringify(chart));
                resolve(chart);
            }
            else{
                resolve({status:Constants.SUCCESS_STATUS,message:"No Data Found"});
            }


        }).catch(function (fetchHistoryErr) {
            console.log("pop"+fetchHistoryErr);
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
                    if(entry.updated_at){ 
                    console.log("plo123"+JSON.stringify(entry.users));    
                    //console.log(entry.users);
                   // let upDate=entry.updated_at;
                    //let updatedTime=upDate.getTime();//in seconds
                    //let currentTime=new Date().getTime();
                    //const diff = currentTime - updatedTime;
                    //let timeWithCurrent = Math.floor(diff / 1000 % 60);
                    let winnerUserId;
                    if(entry.users[0]['isWin']==1){
                        winnerUserId=entry.users[0]['userName'];
                    }
                    else if(entry.users[0]['isWin']==0){
                         winnerUserId=entry.users[1]['userName'];
                    }
                    else if(entry.users[0]['isWin']==2){
                         winnerUserId=entry.users[1]['userName'] + "  and  "+entry.users[0]['userName'];
                    }
                    //let winnerUserId=(entry.users[0]['isWin']==1 ? entry.users[0]['userName'] : entry.users[1]['userName']);
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
                        winner_user:winnerUserId,
                        game_time:entry.game_time
                    });

                 }
                 else{
                    console.log("po0");
                 }
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
//updateVersionAdmin
const updateVersionAdmin =(condObj,updateObj) =>{
    console.log("ok");
    return  new Promise((resolve,reject) => {
        Versions.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}
//fetch active users
//fetchActiveUsers
const fetchUserList =(condObj) =>{
    return  new Promise((resolve,reject) => {
        let condObj={onlineStatus: 1};
        User.find(condObj,{_id: 1,userName:1,email:1,firstName:1,lastName:1}).then(roledetails=> {
            return resolve(roledetails);
        }).catch(err => {
            return reject(err);
        });

    });
}
//fetchMatches
const fetchMatches =(condObj) =>{
    return  new Promise((resolve,reject) => {
        Match.find({status:"active"},{_id: 1,name:1,score:1,details:1}).then(response=> {
            resolve(response)
        }).catch(err=>{
            reject(err);
        })

    });
}
//addMatch
const addMatch =(updateObj) =>{
    return  new Promise((resolve,reject) => {

        Match.find({name:updateObj.name},{_id: 1,name:1}).then(res=> {
            if(res.length){
                reject({message:"Already added"});
            }
            else{
                Match.create(updateObj).then(response => {
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
//updateGameAdmin
const updateGameAdmin =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        Match.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}
//fetchMail
const fetchMail =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        Notification.find({status:"active"},
            {message:1,created_at:1,read_unread:1,_id:1,received_by_user:1}).then(responses => {
            //console.log("notis",responses);
            //let new_time = moment(responses.created_at).format('YYYY-MM-DD HH:mm:ss');
              //fetch user details/////
              
              resolve(responses);
         

        //})
        })

    });
}
//fetch transaction
const fetchTransaction =(condObj) =>{
    return  new Promise((resolve,reject) => {
        Transaction.find(condObj,
            {status:1}).then(responses => {
            //console.log("notis",responses);
            //let new_time = moment(responses.created_at).format('YYYY-MM-DD HH:mm:ss');
              //fetch user details/////
              
              resolve(responses);
         

        //})
        })

    });
}
//addTransaction
const addTransaction =(updateObj) =>{
    console.log("plll");
    return  new Promise((resolve,reject) => {        
                Transaction.create(updateObj).then(response => {
                    console.log("plo9"+response._id)
                    //let a=response.insertedId;
                    //console.log("ppp"+a);

                       
                    return resolve(response._id);
                }).catch(err => {
                    return reject(err);
                });
            

    });
}
const addMail =(updateObj) =>{
    return  new Promise((resolve,reject) => {

        //Match.find({name:updateObj.name},{_id: 1,name:1}).then(res=> {
           /* if(res.length){
                reject({message:"Already added"});
            }*/
            //else{
                Notification.create(updateObj).then(response => {
                    return resolve(response);
                }).catch(err => {
                    return reject(err);
                });
            //}
        /*}).catch(err=>{
            reject(err);
        })*/

    });
}
//updateTransaction
const updateTransaction =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        Transaction.deleteOne({"_id":condObj._id}
        ).then(responses=> {                

        //User.updateOne({"deviceDetails.accessToken":condObj.accessToken},{ $set : {status:"inactive"} }).then(responses=> {                
          return resolve(responses);
      }).catch(err => {
          return reject(err);
      }); 

    });
}
const updateMail =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        Notification.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}

const fetchHistoryUser = userId => {

    return new Promise((resolve, reject) => {
        console.log("ok");

        Room.findHistory1(userId).then(function (responseParams) {
          console.log("responseParams"+responseParams);
            if(responseParams.length >0){
                let chart = [];
                let gameStatus;

                responseParams.map(function(entry) {
                    //console.log("dateDifference"+entry.dateDifference);

                    let upDate=entry.updated_at;
                    let updatedTime=upDate.getTime();//in seconds
                    console.log("updated time in sec"+updatedTime);
                    let currentTime=new Date().getTime();
                    console.log("currentTime sec"+currentTime);

                    const diff = currentTime - updatedTime;
                    let timeWithCurrent = Math.floor(diff / 1000 % 60);
                    let gameCoin;

                    let entusers=entry.users;
                    chart.push({
                        created_at:entry.created_at,
                        game_time: entry.game_time,
                        //updated_at: entry.updated_at,
                        last_time:timeWithCurrent,
                        gameDetails: entusers.map(function(entry1) {
                            //console.log("key1"+key1);

                         //User1.findOne({userName:entry1.userName},{_id: 1,firstName:1,lastName:1}).then(userRes=> {
                             // console.log("ok1"+userRes);
                             let firstName=(!entry1.firstName)? '' : entry1.firstName;
                             let lastName=(!entry1.lastName)? '' : entry1.lastName;
                             let userNm=firstName+" "+lastName;
                              //let userNm=userRes.firstName+userRes.lastName;
                          
                            if(entry1.userId==userId){
                                if(entry1.isWin==1) {
                                    gameStatus = 'VICTORY';
                                    gameCoin = entry1.roomCoin * 2;
                                }
                                else if(entry1.isWin==2) {
                                    gameStatus = 'DRAW';
                                    gameCoin = entry1.roomCoin;
                                }
                                else if(entry1.isWin==0) {
                                    gameStatus = 'DEFEAT';
                                    gameCoin = entry1.roomCoin;
                                }


                                 resolve({
                                
                                userId: entry1.userId,
                                userName: userNm,
                                userScore:entry1.total,
                                cupNumber:entry1.cupNumber,
                                colorName:entry1.colorName,
                                raceName:entry1.raceName,
                                coinNumber:entry1.roomCoin, 
                                totalCupWin:entry1.totalCupWin,                                
                                gameResult:entry1.isWin
                            });


                                
                            }
                           // User1.find({userName:entry1.userName}).then(userRes=> {
                              
                            /*return {
                                
                                userId: entry1.userId,
                                userName: userNm,
                                userScore:entry1.total,
                                cupNumber:entry1.cupNumber,
                                colorName:entry1.colorName,
                                raceName:entry1.raceName,
                                coinNumber:entry1.roomCoin,                               
                                gameResult:entry1.isWin
                            };*/

                        //});
                        })
                    });
                });

                //let res={details:chart};
                //let obj = _.extend({}, chart);
               // console.log(obj);

                //console.log(chart);
                //resolve(chart);
                //resolve(chart[0].gameDetails[0])
            }
            else{
                resolve({status:Constants.SUCCESS_STATUS,message:"No Data Found"});
            }


        }).catch(function (fetchHistoryErr) {
            reject({status:Constants.API_ERROR,message:fetchHistoryErr});
        });
    });
};

const updateTransactionConfirm =(condObj,updateObj) =>{
    return  new Promise((resolve,reject) => {
        Transaction.updateOne(condObj,{ $set : updateObj }).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });

    });
}

const chkValidTransaction =(condObj) =>{
    return  new Promise((resolve,reject) => {
        Transaction.findOne(condObj,{_id: 1,status:1,user_name:1,amount:1}).then(transactiondetails=> {
            return resolve(transactiondetails);
        }).catch(err => {
            return reject(err);
        });

    });
}

module.exports = { chkValidTransaction,updateTransactionConfirm,fetchTransaction,updateTransaction,addTransaction,updateVersionAdmin,fetchHistoryUser,
    updateMail,addMail,fetchMail,updateGameAdmin,
    addMatch,fetchMatches,fetchUserList,
    updateCoinAdmin,updateRoomAdmin,addCoin,
    fetchHistory,userValidChk,
    userValidChkAdmin,fetchHistoryAdmin,
    updateProfileAdmin,modifyProfileDetails,fetchRoleName,fetchCoin };