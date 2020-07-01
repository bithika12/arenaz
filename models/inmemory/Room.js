"use strict";
const appRoot = require('app-root-path');
//INCLUDE CONSTANT
var constants = require(appRoot + "/config/constants");
//INCLUDE UTILS PACKAGE
var room = require(appRoot + '/utils/MemoryDatabaseManger').room;
var RoomDb = require(appRoot + '/schema/Schema').roomModel;
//INCLUDE MODEL
//const  deck            = require('../gameplay/Deck');
//INCLUDE GAME  CLASS MODULE
//const Bridge    = require('../gameplay/Bridge');
//const  Bid            = require('../gameplay/Bid');
var _ = require("underscore");
const Math = require('math');
const logger = require(appRoot + '/utils/LoggerClass');
var io = require(appRoot + '/utils/SocketManager').io;
const response = require(appRoot + '/utils/ResponseManeger');
const moment=require("moment");
let gameUserFirstArr=[];
let gameUserSecondArr=[];
let gameUserFirstObj={};
let gameUserSecondObj={};
let gameUserFirstArr1=[];


/**
 * @desc This function is used for calculate score
 * @param {Object} reqObj
 */


 room.throwDartDetailsV5 = function (reqObj) {
    return new Promise((resolve, reject) => {


        let score;
        let multiplier;
        let calculatedScore;
        let boardScore;
        let userArr = [];
        let newArr = [];
        let newArr1 = [];
        let newArr3 = [];
        let userTurn;
        let dartPnt;
        let remainingScore=0;
        let isWin;
        let userTurnOppnt;
        let userTurnGame;
        boardScore = reqObj.score;
        let playStatus = 0;
        let cupNumber;
        let cupNumberOppo;
        let availableCoin;
        //let findIndexOpponentMod;
        let cupOpponent;

        room.findOne({roomName: reqObj.roomName}
            , function (err, result) {
                if (result) {
                   /* if(result.game_time!='undefined'){
                        reject({message: "Game is over"});
                    }*/
                    userArr = result.users;
                    let currentTime=new Date().getTime();
                    const diff = currentTime - result.gametime;
                    const gameSeconds = Math.floor(diff / 1000 % 60);

                    let findIndexOppo = userArr.findIndex(elemt => (elemt.turn >= 3 && elemt.userId != reqObj.userId/*roomDetails.dealStartDirection*/));
                    logger.print("opponent turn set 0"+findIndexOppo);
                    if (findIndexOppo != -1)
                        userArr[findIndexOppo].turn = 0;
                    userArr.findIndex(function (elemt) {

                        if (elemt.userId == reqObj.userId) {
                            remainingScore = elemt.total;
                            userTurn = elemt.turn + 1;
                            logger.print("current user turn"+userTurn);
                            //userTurn = (elemt.turn) >= 3 ? 0 : elemt.turn + 1;
                            //userTurn = (elemt.turn) >= 3 ? 0 : elemt.turn + 1;
                            //userTurnOppnt = (elemt.turn) = 3 ? 0 : elemt.turn;
                            dartPnt = reqObj.dartPoint;
                            isWin = elemt.isWin;


                            calculatedScore = remainingScore - reqObj.score;
                            /*
                              * If user score is 10 but user hits 21
                              * or some other number bigger
                              * It is a bust and turn changes
                             */
                             //hitScore,
                             //scoreMultiplier

                            if (/*reqObj.score == 1 || reqObj.score < 0 ||*/ 
                                calculatedScore < 0) {
                                //reject({message:"It is bust"});
                                logger.print("It is a bust");
                                logger.print("set trun to opponent as it is a bust");
                                userTurn = 3;
                                playStatus = 1;
                                calculatedScore=remainingScore;
                            }
                            if (calculatedScore == 0) {
                                logger.print("win the match");
                                isWin = 1;
                                cupNumber = 70;
                                availableCoin=elemt.roomCoin;

                                let findIndexOpponent = userArr.findIndex(elemt => elemt.userId != reqObj.userId);
                                cupNumberOppo = Math.round(((userArr[findIndexOpponent].total / 333) * 100), 0);
                                cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                                userArr[findIndexOpponent].cupNumber = cupNumberOppo;
                            } /*else {
                                cupNumber = Math.round(((reqObj.score / remainingScore) * 100), 0);
                                cupNumber = Math.round(((cupNumber * 70) / 100), 0);
                            }*/
                        }

                    });
                    /*calculatedScore = remainingScore - reqObj.score;

                     if (reqObj.score == 1 || reqObj.score < 0 || calculatedScore < 0) {
                         userTurn = 3;
                         playStatus=1;

                    }
                     if(calculatedScore==0){
                         isWin=1;
                         cupNumber=70;
                     }*/

                    let findIndex = userArr.findIndex(elemt => elemt.userId === reqObj.userId);
                    userArr[findIndex].score = reqObj.score;
                    userArr[findIndex].total = calculatedScore;
                    userArr[findIndex].turn = userTurn;
                    userArr[findIndex].dartPoint = dartPnt;
                    userArr[findIndex].isWin = isWin;
                    userArr[findIndex].userId = reqObj.userId;
                    userArr[findIndex].status = "active";
                    userArr[findIndex].cupNumber = cupNumber;
                    //userArr[findIndex].userTurn=userTurnGame;
                    let findIndexOpponentMod = userArr.findIndex(elemt => elemt.userId != reqObj.userId);
                    if(findIndexOpponentMod!=-1)
                       cupOpponent=userArr[findIndexOpponentMod].cupNumber;
                    resolve({
                        roomName: reqObj.roomName,
                        users: reqObj.userId,
                        remainingScore: calculatedScore,
                        finalArr: userArr,
                        userTurn: userTurn,
                        dartPoint: dartPnt,
                        playStatus: playStatus,
                        isWin: isWin,
                        playerScore: reqObj.score,
                        cupNumber: cupNumber,
                        gameTotalTime:gameSeconds,
                        availableCoin:availableCoin,
                        //availableCoin:availableCoin,
                        cupOpponent:cupOpponent,
                        //new add
                        userCoin:availableCoin,
                        opponentCup:cupOpponent,
                        opponentUserId:userArr[findIndexOpponentMod].userId
                    });
                } else {
                    console.log("Unable to find room"+reqObj.roomName);
                    reject({message: "No room found"});
                }
            });
    });
}

room.throwDartDetails = function (reqObj) {
    return new Promise((resolve, reject) => {


        let score;
        let multiplier;
        let calculatedScore;
        let boardScore;
        let userArr = [];
        let newArr = [];
        let newArr1 = [];
        let newArr3 = [];
        let userTurn;
        let dartPnt;
        let remainingScore=0;
        let isWin;
        let userTurnOppnt;
        let userTurnGame;
        boardScore = reqObj.score;
        let playStatus = 0;
        let cupNumber;
        let cupNumberOppo;
        let availableCoin;
        let userScore;
        //let findIndexOpponentMod;
        let cupOpponent;
        let userTotalScore=0;
        let userRemainScore=0;
        let roundScore=0;
        let totalGameScores;
        let gameScoreOpponent;

        room.findOne({roomName: reqObj.roomName}
            , function (err, result) {
                if (result) {
                   /* if(result.game_time!='undefined'){
                        reject({message: "Game is over"});
                    }*/

                    let dt1=moment().format('MM/DD/YYYY HH:mm:ss');
                    let config = "MM/DD/YYYY HH:mm:ss";
                    let ms = moment(dt1, config).diff(moment(result.createtime,config));
                    let d = (moment.duration(ms))/1000;
                    let gameSeconds = Math.floor(d);

                    if(gameSeconds >360){
                        gameSeconds=360;
                    }


                    userArr = result.users;
                    let currentTime=new Date().getTime();
                    //createtime
                    const diff = currentTime - result.createtime;
                    //const diff = currentTime - result.gametime;
                    //const gameSeconds = Math.floor(diff / 1000 % 60);

                    let findIndexOppo = userArr.findIndex(elemt => (elemt.turn >= 3 && elemt.userId != reqObj.userId/*roomDetails.dealStartDirection*/));
                    logger.print("opponent turn set 0"+findIndexOppo);
                    if (findIndexOppo != -1)
                        userArr[findIndexOppo].turn = 0;
                    userArr.findIndex(function (elemt) {

                        if (elemt.userId == reqObj.userId) {
                            remainingScore = elemt.total;
                            userTurn = elemt.turn + 1;
                            logger.print("current user turn"+userTurn);
                            //userTurn = (elemt.turn) >= 3 ? 0 : elemt.turn + 1;
                            //userTurn = (elemt.turn) >= 3 ? 0 : elemt.turn + 1;
                            //userTurnOppnt = (elemt.turn) = 3 ? 0 : elemt.turn;
                            dartPnt = reqObj.dartPoint;
                            isWin = elemt.isWin;
                            
                            totalGameScores=parseInt(reqObj.score);   
                            roundScore=parseInt(reqObj.score);
                            
                            calculatedScore = remainingScore - reqObj.score;

                            /*
                              * If user score is 10 but user hits 21
                              * or some other number bigger
                              * It is a bust and turn changes
                             */
                             //hitScore,
                             //scoreMultiplier

                            if (calculatedScore < 0) {
                                //reject({message:"It is bust"});
                                logger.print("It is a bust");
                                logger.print("set trun to opponent as it is a bust");
                                userTurn = 3;
                                playStatus = 1;
                                calculatedScore=remainingScore;
                                //userRemainScore=remainingScore; 
                            }
                            

                            if (calculatedScore == 0) {
                                logger.print("win the match");
                                isWin = 1;
                                cupNumber = 70;
                                availableCoin=elemt.roomCoin;

                                ///////////////////////
                                  
                                ///////////////////////

                                let findIndexOpponent = userArr.findIndex(elemt => elemt.userId != reqObj.userId);
                                /*if(userArr[findIndexOpponent].total <99){
                                    cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)+25),0);
                                }
                                else{
                                    cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)-25),0);
                                }*/                                
                                

                                ///09-06////

                                if(userArr[findIndexOpponent].total <99){
                                     cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)+25),0);
                                    }
                                else{
                                   cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)-25),0);
                                } 

                               cupNumberOppo=Math.round(((cupNumberOppo)*70/199),0);


                                //cupNumberOppo=Math.round(((199-userArr[findIndexOpponent].total)*70/199),0);
                                ///////09-06////////////////
                                //cupNumberOppo = Math.round(((userArr[findIndexOpponent].total / 333) * 100), 0);
                                //cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                                userArr[findIndexOpponent].cupNumber = cupNumberOppo;
                            } /*else {
                                cupNumber = Math.round(((reqObj.score / remainingScore) * 100), 0);
                                cupNumber = Math.round(((cupNumber * 70) / 100), 0);
                            }*/
                        }

                    });
                    /*calculatedScore = remainingScore - reqObj.score;

                     if (reqObj.score == 1 || reqObj.score < 0 || calculatedScore < 0) {
                         userTurn = 3;
                         playStatus=1;

                    }
                     if(calculatedScore==0){
                         isWin=1;
                         cupNumber=70;
                     }*/

                    let findIndex = userArr.findIndex(elemt => elemt.userId === reqObj.userId);
                    //userArr[findIndex].roundscore = roundScore;
                   // userArr[findIndex].score = userScore;
                    userArr[findIndex].score = reqObj.score;
                    //userArr[findIndex].total = userRemainScore;
                    userArr[findIndex].total = calculatedScore;
                    userArr[findIndex].turn = userTurn;
                    userArr[findIndex].dartPoint = dartPnt;
                    userArr[findIndex].isWin = isWin;
                    userArr[findIndex].userId = reqObj.userId;
                    userArr[findIndex].status = "active";
                    userArr[findIndex].cupNumber = cupNumber;
                    userArr[findIndex].totalGameScore = totalGameScores;
                    //totalGameScore
                    //userArr[findIndex].userTurn=userTurnGame;
                    let findIndexOpponentMod = userArr.findIndex(elemt => elemt.userId != reqObj.userId);
                    if(findIndexOpponentMod!=-1){
                       cupOpponent=userArr[findIndexOpponentMod].cupNumber;
                       gameScoreOpponent=userArr[findIndexOpponentMod].totalGameScore;
                    }

                   console.log("user life score"+totalGameScores);

                   userArr[findIndex].hitScore = reqObj.hitScore;
                   userArr[findIndex].scoreMultiplier = reqObj.scoreMultiplier;

                   //gameUserFirstArr=[];
                   //gameUserSecondArr=[];
                   //gameUserFirstObj={};
                   //gameUserSecondObj={};
                   // let gameUserFirstArr1=[];
                   userArr.findIndex(function (reselemt) {                      
                         //if(reselemt.total !=199 && reselemt.userId =reqObj.userId){
                         reselemt.roomName=reqObj.roomName;                       
                           
                           gameUserFirstArr1.push(reselemt);
                      // }
                        
                        

                      


                   });

                   console.log("gameUserFirstArr"+gameUserFirstArr1);
                   console.log("gameUserSecondArr"+gameUserSecondArr);



                    resolve({
                        roomName: reqObj.roomName,
                        users: reqObj.userId,
                        //remainingScore: userRemainScore,
                        remainingScore: calculatedScore,
                        finalArr: userArr,
                        userTurn: userTurn,
                        dartPoint: dartPnt,
                        playStatus: playStatus,
                        isWin: isWin,
                        //playerScore: userScore,
                        playerScore: reqObj.score,
                        cupNumber: cupNumber,
                        gameTotalTime:gameSeconds,
                        availableCoin:availableCoin,
                        //availableCoin:availableCoin,
                        cupOpponent:cupOpponent,
                        //new add
                        userCoin:availableCoin,
                        opponentCup:cupOpponent,
                        opponentUserId:userArr[findIndexOpponentMod].userId,
                        roundScore:roundScore,
                        totalGameScores:totalGameScores,
                        gameScoreOpponent:gameScoreOpponent,

                        /////
                        hitScore:reqObj.hitScore,
                        scoreMultiplier:reqObj.scoreMultiplier,
                         
                        gameUserFirstArr1:gameUserFirstArr1,
                        gameUserSecondArr:gameUserSecondArr


                    });
                } else {
                    console.log("Unable to find room"+reqObj.roomName);
                    reject({message: "No room found"});
                }
            });
    });
}

room.throwDartDetailsRunning = function (reqObj) {
    return new Promise((resolve, reject) => {


        let score;
        let multiplier;
        let calculatedScore;
        let boardScore;
        let userArr = [];
        let newArr = [];
        let newArr1 = [];
        let newArr3 = [];
        let userTurn;
        let dartPnt;
        let remainingScore=0;
        let isWin;
        let userTurnOppnt;
        let userTurnGame;
        boardScore = reqObj.score;
        let playStatus = 0;
        let cupNumber;
        let cupNumberOppo;
        let availableCoin;
        let userScore;
        //let findIndexOpponentMod;
        let cupOpponent;
        let userTotalScore=0;
        let userRemainScore=0;
        let roundScore=0;
        let totalGameScores;
        let gameScoreOpponent;

        room.findOne({roomName: reqObj.roomName}
            , function (err, result) {
                if (result) {
                   /* if(result.game_time!='undefined'){
                        reject({message: "Game is over"});
                    }*/

                    let dt1=moment().format('MM/DD/YYYY HH:mm:ss');
                    let config = "MM/DD/YYYY HH:mm:ss";
                    let ms = moment(dt1, config).diff(moment(result.createtime,config));
                    let d = (moment.duration(ms))/1000;
                    let gameSeconds = Math.floor(d);

                    if(gameSeconds >360){
                        gameSeconds=360;
                    }


                    userArr = result.users;
                    let currentTime=new Date().getTime();
                    //createtime
                    const diff = currentTime - result.createtime;
                    //const diff = currentTime - result.gametime;
                    //const gameSeconds = Math.floor(diff / 1000 % 60);

                    let findIndexOppo = userArr.findIndex(elemt => (elemt.turn >= 3 && elemt.userId != reqObj.userId/*roomDetails.dealStartDirection*/));
                    logger.print("opponent turn set 0"+findIndexOppo);
                    if (findIndexOppo != -1)
                        userArr[findIndexOppo].turn = 0;
                    userArr.findIndex(function (elemt) {

                        if (elemt.userId == reqObj.userId) {
                            remainingScore = elemt.total;
                            userTurn = elemt.turn + 1;
                            logger.print("current user turn"+userTurn);
                            //userTurn = (elemt.turn) >= 3 ? 0 : elemt.turn + 1;
                            //userTurn = (elemt.turn) >= 3 ? 0 : elemt.turn + 1;
                            //userTurnOppnt = (elemt.turn) = 3 ? 0 : elemt.turn;
                            dartPnt = reqObj.dartPoint;
                            isWin = elemt.isWin;

                            //new code on 30 th march 2020//
                            console.log("user1 acore"+reqObj.score);
                            console.log("elemt.score"+elemt.score);
                            //userScore1=Math.sum(reqObj.score,elemt.score);
                            //console.log("userScore1"+userScore1);
                            if(userTurn ==1){                             
                             roundScore=0; 
                             elemt.score=0;                            
                            }
                            else{
                                roundScore=elemt.roundscore;
                            }                            
                            totalGameScores=parseInt(reqObj.score) + parseInt(elemt.totalGameScore);   
                            roundScore=parseInt(reqObj.score) + parseInt(roundScore);
                            
                            userScore=parseInt(reqObj.score) + parseInt(elemt.score);
                            console.log("total user score" +userScore);
                            
                            console.log("remaining score"+remainingScore);
                            userTotalScore=remainingScore-userScore;
                            console.log("userTotalScore"+userTotalScore);
                            if(userTurn ==3){

                             userRemainScore=remainingScore-userScore;
                             console.log("userRemainScore"+userRemainScore);
                            }
                            else{
                               userRemainScore=remainingScore; 
                               console.log("userRemainScore"+userRemainScore);
                            }
                            //new code on 30 th march 2020//

                            //calculatedScore = remainingScore - reqObj.score;

                            /*
                              * If user score is 10 but user hits 21
                              * or some other number bigger
                              * It is a bust and turn changes
                             */
                             //hitScore,
                             //scoreMultiplier

                            if (userTotalScore < 0) {
                                //reject({message:"It is bust"});
                                logger.print("It is a bust");
                                logger.print("set trun to opponent as it is a bust");
                                userTurn = 3;
                                playStatus = 1;
                                //calculatedScore=remainingScore;
                                userRemainScore=remainingScore; 
                            }
                            calculatedScore = remainingScore - reqObj.score;

                            if (userTotalScore == 0) {
                                logger.print("win the match");
                                isWin = 1;
                                cupNumber = 70;
                                availableCoin=elemt.roomCoin;

                                ///////////////////////
                                  
                                ///////////////////////

                                let findIndexOpponent = userArr.findIndex(elemt => elemt.userId != reqObj.userId);
                                /*if(userArr[findIndexOpponent].total <99){
                                    cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)+25),0);
                                }
                                else{
                                    cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)-25),0);
                                }*/                                
                                

                                ///09-06////

                                if(userArr[findIndexOpponent].total <99){
                                     cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)+25),0);
                                    }
                                else{
                                   cupNumberOppo=Math.round((parseInt(userArr[findIndexOpponent].total)-25),0);
                                } 

                               cupNumberOppo=Math.round(((cupNumberOppo)*70/199),0);


                                //cupNumberOppo=Math.round(((199-userArr[findIndexOpponent].total)*70/199),0);
                                ///////09-06////////////////
                                //cupNumberOppo = Math.round(((userArr[findIndexOpponent].total / 333) * 100), 0);
                                //cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                                userArr[findIndexOpponent].cupNumber = cupNumberOppo;
                            } /*else {
                                cupNumber = Math.round(((reqObj.score / remainingScore) * 100), 0);
                                cupNumber = Math.round(((cupNumber * 70) / 100), 0);
                            }*/
                        }

                    });
                    /*calculatedScore = remainingScore - reqObj.score;

                     if (reqObj.score == 1 || reqObj.score < 0 || calculatedScore < 0) {
                         userTurn = 3;
                         playStatus=1;

                    }
                     if(calculatedScore==0){
                         isWin=1;
                         cupNumber=70;
                     }*/

                    let findIndex = userArr.findIndex(elemt => elemt.userId === reqObj.userId);
                    userArr[findIndex].roundscore = roundScore;
                    userArr[findIndex].score = userScore;
                    //userArr[findIndex].score = reqObj.score;
                    userArr[findIndex].total = userRemainScore;
                    //userArr[findIndex].total = calculatedScore;
                    userArr[findIndex].turn = userTurn;
                    userArr[findIndex].dartPoint = dartPnt;
                    userArr[findIndex].isWin = isWin;
                    userArr[findIndex].userId = reqObj.userId;
                    userArr[findIndex].status = "active";
                    userArr[findIndex].cupNumber = cupNumber;
                    userArr[findIndex].totalGameScore = totalGameScores;
                    //totalGameScore
                    //userArr[findIndex].userTurn=userTurnGame;
                    let findIndexOpponentMod = userArr.findIndex(elemt => elemt.userId != reqObj.userId);
                    if(findIndexOpponentMod!=-1){
                       cupOpponent=userArr[findIndexOpponentMod].cupNumber;
                       gameScoreOpponent=userArr[findIndexOpponentMod].totalGameScore;
                    }

                   console.log("user life score"+totalGameScores);

                   userArr[findIndex].hitScore = reqObj.hitScore;
                   userArr[findIndex].scoreMultiplier = reqObj.scoreMultiplier;

                   //gameUserFirstArr=[];
                   //gameUserSecondArr=[];
                   //gameUserFirstObj={};
                   //gameUserSecondObj={};
                   // let gameUserFirstArr1=[];
                   userArr.findIndex(function (reselemt) {                      
                         //if(reselemt.total !=199 && reselemt.userId =reqObj.userId){
                         reselemt.roomName=reqObj.roomName;                       
                           
                           gameUserFirstArr1.push(reselemt);
                      // }
                        
                        

                      


                   });

                   console.log("gameUserFirstArr"+gameUserFirstArr1);
                   console.log("gameUserSecondArr"+gameUserSecondArr);



                    resolve({
                        roomName: reqObj.roomName,
                        users: reqObj.userId,
                        remainingScore: userRemainScore,
                        //remainingScore: calculatedScore,
                        finalArr: userArr,
                        userTurn: userTurn,
                        dartPoint: dartPnt,
                        playStatus: playStatus,
                        isWin: isWin,
                        playerScore: userScore,
                        //playerScore: reqObj.score,
                        cupNumber: cupNumber,
                        gameTotalTime:gameSeconds,
                        availableCoin:availableCoin,
                        //availableCoin:availableCoin,
                        cupOpponent:cupOpponent,
                        //new add
                        userCoin:availableCoin,
                        opponentCup:cupOpponent,
                        opponentUserId:userArr[findIndexOpponentMod].userId,
                        roundScore:roundScore,
                        totalGameScores:totalGameScores,
                        gameScoreOpponent:gameScoreOpponent,

                        /////
                        hitScore:reqObj.hitScore,
                        scoreMultiplier:reqObj.scoreMultiplier,
                         
                        gameUserFirstArr1:gameUserFirstArr1,
                        gameUserSecondArr:gameUserSecondArr


                    });
                } else {
                    console.log("Unable to find room"+reqObj.roomName);
                    reject({message: "No room found"});
                }
            });
    });
}


room.throwDartDetailsDraw = function (reqObj) {
    return new Promise((resolve, reject) => {


        let score;
        let multiplier;
        let calculatedScore;
        let boardScore;
        let userArr = [];
        let newArr = [];
        let newArr1 = [];
        let newArr3 = [];
        let userTurn;
        let dartPnt;
        let remainingScore;
        let isWin;
        let userTurnOppnt;
        let userTurnGame;
        boardScore = reqObj.score;
        let playStatus = 0;
        let cupNumber;
        let cupNumberOppo;
        room.findOne({roomName: reqObj.roomName}
            , function (err, result) {
                if (result) {



                    let findIndex = userArr.findIndex(elemt => elemt.userId === reqObj.userId);
                    cupNumber=70;
                    userArr[findIndex].isWin = 2;
                    userArr[findIndex].status = "inactive";
                    userArr[findIndex].cupNumber = cupNumber;

                    let findIndexOpp = userArr.findIndex(elemt => elemt.userId != reqObj.userId);
                    userArr[findIndexOpp].isWin = 2;
                    userArr[findIndexOpp].status = "inactive";
                    userArr[findIndexOpp].cupNumber = cupNumber;

                    resolve({
                        roomName: reqObj.roomName,
                        finalArr: userArr

                    });
                } else {
                    reject({message: "No room found"});
                }
            });
    });
}


room.throwDartDetailsOld = function (reqObj) {
    return new Promise((resolve, reject) => {


        let score;
        let multiplier;
        let calculatedScore;
        let boardScore;
        let userArr = [];
        let newArr = [];
        let newArr1 = [];
        let newArr3 = [];
        let userTurn;
        boardScore = reqObj.score;
        room.findOne({roomName: reqObj.roomName}
            , function (err, result) {
                if (result) {
                    userArr = result.users;
                    if (reqObj.isDouble) {
                        multiplier = 2;
                    }
                    if (reqObj.isTriple) {
                        multiplier = 3;
                    } else {
                        multiplier = 1;
                    }

                    userArr.findIndex(function (elemt) {
                        if (elemt.userId == reqObj.userId) {
                            score = elemt.total
                            userTurn = (elemt.turn) >= 3 ? 0 : elemt.turn + 1;
                        }
                    });

                    /*
                      * If it hits the centre it should deduct 25*2
                     */
                    if (reqObj.isCentre) {
                        calculatedScore = 25 * 2;
                        boardScore = calculatedScore;
                        calculatedScore = score - (boardScore * multiplier);
                    }
                    /*
                      * If it hits the orange other rim of the centre.Score is 25.
                     */
                    if (reqObj.isBull) {
                        calculatedScore = 25;
                        boardScore = calculatedScore;
                        calculatedScore = parseInt(score - (boardScore * multiplier));
                    } else {
                        calculatedScore = parseInt(score - (boardScore * multiplier));
                    }

                    if (calculatedScore == 0) {
                        //GAME FINISHED
                        //modify the array

                        userArr.findIndex(function (elemt) {
                            if (elemt.userId == reqObj.userId)
                                newArr.push({
                                    score: score,
                                    total: calculatedScore,
                                    userId: elemt.userId,
                                    status: elemt.status,
                                    isWin: 1,
                                    turn: userTurn
                                });
                            else
                                newArr1.push({
                                    score: elemt.score,
                                    total: elemt.total,
                                    userId: elemt.userId,
                                    status: elemt.status,
                                    isWin: 1,
                                    turn: elemt.turn
                                });

                        });
                        newArr3 = newArr.concat(newArr1);

                        for (let i in result) {
                            result.users = newArr3
                        }
                        let finalArr = result;
                        resolve({users: reqObj.userId, remainingScore: calculatedScore, finalArr: finalArr});

                    }
                    /*
                      * If user score is 10 but user hits 21
                      * or some other number bigger
                      * It is a bust and turn changes
                     */
                    else if (calculatedScore == 1 || calculatedScore < 0 || boardScore > score) {
                        console.log("bust");
                    } else {
                        userArr.findIndex(function (elemt) {
                            if (elemt.userId == reqObj.userId)
                                newArr.push({
                                    score: score,
                                    total: calculatedScore,
                                    userId: elemt.userId,
                                    status: elemt.status,
                                    turn: userTurn
                                });
                            else
                                newArr1.push({
                                    score: elemt.score,
                                    total: elemt.total,
                                    userId: elemt.userId,
                                    status: elemt.status,
                                    turn: elemt.turn
                                });

                        });
                        newArr3 = newArr.concat(newArr1);

                        for (let i in result) {
                            result.users = newArr3
                        }
                        let finalArr = result;
                        resolve({
                            users: reqObj.userId,
                            remainingScore: calculatedScore,
                            finalArr: finalArr,
                            userTurn: userTurn
                        });

                    }
                } else {
                    reject(false);
                }
            });
    });
}


room.inmCreateRoom = function (userObj) {
    return new Promise((resolve, reject) => {
        room.findOne({$or: [{users: {$size: 1}}]}, function (err, roomresult) {
            var userArr = [];
            if (roomresult == null || roomresult.length == 0) {
                //userDirection = findAvailabePosition(userArr);
                //userObj.direction = userDirection;
                //serArr.push(userObj);
                room.insert({
                    users: userArr,
                    noOfRound: 1,
                    totalUser: 1,
                    //dealerId          : '',
                    //dealStartDirection : userObj.direction,
                    //cardOpenUser      : '',
                    //currentBidDirection       : '',
                    //bid               : [],
                    createtime: new Date().getTime()
                }, function (err, insertroomresult) {
                    if (err)
                        reject(err)
                    else
                        resolve({
                            users: insertroomresult.users,
                            direction: userDirection,
                            roomName: insertroomresult._id,
                            noOfRound: 1
                        })
                })
            } else {
                roomName = roomresult._id;
                var userArr = roomresult.users;
                const index = userArr.findIndex((e) => e.userId === userObj.userId);
                //userDirection = findAvailabePosition(userArr);
                if (index === -1) {
                    //userObj.direction   =  userDirection;
                    userObj.userType = "user";
                    userArr.push(userObj);
                }
                room.update({_id: roomName}, {$set: {users: userArr}}, function (err, updateroomresult) {
                    if (err)
                        reject({})
                    else {
                        resolve({users: userArr, roomName: roomName, noOfRound: roomresult.noOfRound});
                    }

                });
            }

        })

    })
}
room.roomJoineeCreation = function (conditionObj, updateObj) {

    return new Promise((resolve, reject) => {
        let dt1=moment().format('MM/DD/YYYY HH:mm:ss');
        room.findOne({roomId: conditionObj.roomId}, function (err, roomresult) {
            var userArr = [];
            if (roomresult == null || roomresult.length == 0) {
                userArr.push(updateObj.userObj)
                room.insert({
                    roomId: conditionObj.roomId,
                    roomName: conditionObj.roomName,
                    users: userArr,
                    totalUser: 1,
                    createtime: dt1,
                    //createtime: new Date().getTime(),
                    gametime:""
                }, function (err, insertroomresult) {
                    if (err)
                        reject(err)
                    else
                        resolve({users: insertroomresult.users})
                })
            } else {

                userArr.push(updateObj.userObj);
                if (userArr[0]['roomCoin'] != conditionObj.roomCoin) {

                    //add in a new room as coin not match
                    room.insert({
                        roomId: conditionObj.roomId,
                        roomName: conditionObj.roomName,
                        users: userArr,
                        totalUser: 1,
                        createtime: dt1,
                        //createtime: new Date().getTime(),
                        gametime:""
                    }, function (err, insertroomresult) {
                        if (err)
                            reject(err)
                        else
                            resolve({users: insertroomresult.users})
                    })
                }
                else {
                var roomUser = roomresult.users;
                console.log("roomUser ", roomUser);
                console.log("updateObj ", updateObj);

                const index = roomUser.findIndex((e) => e.userId === updateObj.userObj.userId);
                if (index === -1) {
                    roomUser.push(userObj)
                }
                console.log(" index : ", index);
                room.update({roomId: conditionObj.roomId},
                    {
                        $set: {users: roomUser, gametime: new Date().getTime()}/*$pull:  {  users  : { userId:updateObj.userObj.userId}},
						$push:  {  users  : updateObj.userObj }*/
                    },

                    function (err, updateroomresult) {
                        if (err)
                            reject({})
                        else {
                            resolve({users: roomUser})
                        }

                    });
               }
            }

        });
    });
}

room.getRoomDetails = function (condObj, obj) {

    return new Promise((resolve, reject) => {
        room.findOne({roomName: condObj.roomName}, function (err, roomresult) {
            if (err)
                reject(err)
            else
                resolve(roomresult)
        })
    });
}

room.roomUserCount = function (condObj) {
    return new Promise((resolve, reject) => {
        room.findOne(condObj, function (err, roomresult) {
            if (err)
                reject()
            else
                var totalplayer = (roomresult == null) ? 0 : roomresult.users.length;
            resolve(totalplayer)
        })
    });
}
room.updateRoomDetails = function (reqObj, condObj) {
    return new Promise((resolve, reject) => {
        room.update(condObj, {$set: reqObj}, {multi: true}, function (err, numReplaced) {
            if (err)
                reject({})
            else
                resolve(numReplaced)
        });
    });
}
room.removeRoom = function (condObj) {
    return new Promise((resolve, reject) => {
        room.remove(condObj, {}, function (err, numRemoved) {
            resolve(numRemoved)
        });
    });
}

room.userLeaveMod = function (condObj, updateObj) {
    console.log("***** leaveJoinee >> ", condObj);
    return new Promise((resolve, reject) => {
        room.update({roomName: condObj.roomName},
            {
                $pull: {
                    users: {userId: condObj.userId},
                }
            },
            {multi: true},
            function (err, updateRoom) {
                if (err) {
                    reject({})
                } else {
                    resolve(updateRoom)
                }
            });
    });
}
room.userLeave09_06_20 = function (condObj, updateObj) {
    return new Promise((resolve, reject) => {
        room.findOne({roomName: condObj.roomName}
            , function (err, result) {
                let score;
                let multiplier;
                let calculatedScore;
                let boardScore;
                let userArr = [];
                let newArr = [];
                let newArr1 = [];
                let newArr3 = [];
                let userTurn;
                let dartPnt;
                let remainingScore;
                let isWin;
                let userTurnOppnt;
                let userTurnGame;
                boardScore = condObj.score;
                let playStatus = 0;
                let cupNumber;
                let playerScore;
                userArr = result.users;
                let cupNumberOppo;
                let currentTime=new Date().getTime();
                const diff = currentTime - result.createtime;
                //const diff = currentTime - result.gametime;
                //const gameSeconds = Math.floor(diff / 1000 % 60);

                let userOpponentUserId;
                let totalGameScores;
                let gameScoreOpponent;

                 let dt1=moment().format('MM/DD/YYYY HH:mm:ss');
                  let config = "MM/DD/YYYY HH:mm:ss";
                   let ms = moment(dt1, config).diff(moment(result.createtime,config));
                  let d = (moment.duration(ms))/1000;
                  let gameSeconds = Math.floor(d);

                if(gameSeconds >360){
                        gameSeconds=360;
                    }
                   



                let findIndex = userArr.findIndex(elemt => elemt.userId === condObj.userId);

                let findIndexOppo = userArr.findIndex(elemt => elemt.userId != condObj.userId);
                if(findIndexOppo!=-1)
                  userOpponentUserId=userArr[findIndexOppo].userId;

                if(userArr.length ==1){
                    //userArr[findIndex].isWin=2;
                    //userArr[findIndexOppo].isWin=2;
                    userArr[findIndex].status = "inactive";

                    //playStatus=2;
                    //isWin=2;
                    //userArr[findIndexOppo].cupNumber=70;
                    //userArr[findIndex].cupNumber=70;

                }

                if(userArr.length >1 && gameSeconds <=8){
                    userArr[findIndex].isWin=2;
                    userArr[findIndexOppo].isWin=2;
                    userArr[findIndex].status = "inactive";

                    //playStatus=2;
                    isWin=2;
                    userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].cupNumber=70;

                    cupNumberOppo=userArr[findIndexOppo].cupNumber;

                    gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                }
                if(findIndexOppo !=-1 &&  gameSeconds >8){
                    if(userArr[findIndexOppo].total != userArr[findIndex].total){
                    userArr[findIndexOppo].isWin = 1;
                    userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].status = "inactive";
                    playStatus=userArr[findIndex].playStatus;
                    isWin=1;

                    userArr[findIndexOppo].cupNumber=Math.round(((199-userArr[findIndexOppo].total)*70/199),0);


                    if(userArr[findIndex].total <99){
                         cupNumberOppo=Math.round((parseInt(userArr[findIndex].total)+25),0);
                            }
                      else{
                         cupNumberOppo=Math.round((parseInt(userArr[findIndex].total)-25),0);
                        } 

                    //cupNumberOppo = Math.round(((userArr[findIndexOppo].total / 333) * 100), 0);
                    //cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                    userArr[findIndex].cupNumber = cupNumberOppo;

                    //console.log("opponent"+userArr[findIndexOppo].userId);
                   cupNumberOppo=userArr[findIndexOppo].cupNumber;
                   gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                  }
                  else{
                    userArr[findIndex].isWin=2;
                    userArr[findIndexOppo].isWin=2;
                    userArr[findIndex].status = "inactive";                    
                    isWin=2;                    

                    cupNumberOppo=userArr[findIndexOppo].cupNumber;

                    gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                  }
                }
                //userArr[findIndex].status = "inactive";
                calculatedScore= userArr[findIndex].total;
                userTurn=userArr[findIndex].turn;
                dartPnt=userArr[findIndex].dartPoint;
                playStatus=userArr[findIndex].playStatus;
                //isWin=userArr[findIndex].isWin;
                playerScore=userArr[findIndex].score;
                cupNumber=userArr[findIndex].cupNumber;
                totalGameScores=userArr[findIndex].totalGameScore;
                //userArr[findIndexOppo].isWin = 1;
                resolve({
                    roomName: condObj.roomName,
                    users: condObj.userId,
                    remainingScore: calculatedScore,
                    finalArr: userArr,
                    userTurn: userTurn,
                    dartPoint: dartPnt,
                    playStatus: playStatus,
                    isWin: isWin,
                    playerScore: playerScore,
                    
                    cupNumber: cupNumber,
                    gameTotalTime:gameSeconds,
                    opponentUserId:userOpponentUserId,
                    ////////////////////////////////////
                    opponentCup:cupNumber,
                    cupNumber:cupNumberOppo,
                    availableCoin:userArr[findIndex].roomCoin,
                    opponentCoin:userArr[findIndex].roomCoin,
                    
                    totalGameScores:gameScoreOpponent,
                    gameScoreOpponent:totalGameScores
                });

    });
});
}

room.userLeave = function (condObj, updateObj) {
    return new Promise((resolve, reject) => {
        room.findOne({roomName: condObj.roomName}
            , function (err, result) {
                let score;
                let multiplier;
                let calculatedScore;
                let boardScore;
                let userArr = [];
                let newArr = [];
                let newArr1 = [];
                let newArr3 = [];
                let userTurn;
                let dartPnt;
                let remainingScore;
                let isWin;
                let userTurnOppnt;
                let userTurnGame;
                boardScore = condObj.score;
                let playStatus = 0;
                let cupNumber;
                let playerScore;
                userArr = result.users;
                let cupNumberOppo;
                let currentTime=new Date().getTime();
                const diff = currentTime - result.createtime;
                //const diff = currentTime - result.gametime;
                //const gameSeconds = Math.floor(diff / 1000 % 60);

                let userOpponentUserId;
                let totalGameScores;
                let gameScoreOpponent;

                 let dt1=moment().format('MM/DD/YYYY HH:mm:ss');
                  let config = "MM/DD/YYYY HH:mm:ss";
                   let ms = moment(dt1, config).diff(moment(result.createtime,config));
                  let d = (moment.duration(ms))/1000;
                  let gameSeconds = Math.floor(d);

                if(gameSeconds >360){
                        gameSeconds=360;
                    }
                   



                let findIndex = userArr.findIndex(elemt => elemt.userId === condObj.userId);

                let findIndexOppo = userArr.findIndex(elemt => elemt.userId != condObj.userId);
                if(findIndexOppo!=-1)
                  userOpponentUserId=userArr[findIndexOppo].userId;

                if(userArr.length ==1){
                    //userArr[findIndex].isWin=2;
                    //userArr[findIndexOppo].isWin=2;
                    userArr[findIndex].status = "inactive";

                    //playStatus=2;
                    //isWin=2;
                    //userArr[findIndexOppo].cupNumber=70;
                    //userArr[findIndex].cupNumber=70;

                }

                if(userArr.length >1 && gameSeconds <=8){
                    userArr[findIndex].isWin=2;
                    userArr[findIndexOppo].isWin=2;
                    userArr[findIndex].status = "inactive";

                    //playStatus=2;
                    isWin=2;

                    userArr[findIndex].cupNumber=
                    Math.round(((199-userArr[findIndex].total)*70/199),0);

                    userArr[findIndexOppo].cupNumber=
                    Math.round(((199-userArr[findIndexOppo].total)*70/199),0);

                    /*userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].cupNumber=70;*/

                    cupNumberOppo=userArr[findIndexOppo].cupNumber;

                    gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                }
                if(findIndexOppo !=-1 &&  gameSeconds >8){
                    //if(userArr[findIndexOppo].total != userArr[findIndex].total){
                    userArr[findIndexOppo].isWin = 1;
                    //userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].status = "inactive";
                    playStatus=userArr[findIndex].playStatus;
                    isWin=1;

                    userArr[findIndexOppo].cupNumber=Math.round(((199-userArr[findIndexOppo].total)*70/199),0);


                    if(userArr[findIndex].total <99){
                         cupNumberOppo=Math.round((parseInt(userArr[findIndex].total)+25),0);
                            }
                      else{
                         cupNumberOppo=Math.round((parseInt(userArr[findIndex].total)-25),0);
                        }

                    //cupNumberOppo=Math.round(((199-cupNumberOppo)*70/199),0);
                    cupNumberOppo=Math.round(((cupNumberOppo)*70/199),0);

                    //cupNumberOppo = Math.round(((userArr[findIndexOppo].total / 333) * 100), 0);
                    //cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                    userArr[findIndex].cupNumber = cupNumberOppo;

                    //console.log("opponent"+userArr[findIndexOppo].userId);
                   cupNumberOppo=userArr[findIndexOppo].cupNumber;
                   gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                  //}
                 /* else{
                    userArr[findIndex].isWin=2;
                    userArr[findIndexOppo].isWin=2;
                    userArr[findIndex].status = "inactive";                    
                    isWin=2;                    

                    cupNumberOppo=userArr[findIndexOppo].cupNumber;

                    gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                  }*/
                }
                //userArr[findIndex].status = "inactive";
                calculatedScore= userArr[findIndex].total;
                userTurn=userArr[findIndex].turn;
                dartPnt=userArr[findIndex].dartPoint;
                playStatus=userArr[findIndex].playStatus;
                //isWin=userArr[findIndex].isWin;
                playerScore=userArr[findIndex].score;
                cupNumber=userArr[findIndex].cupNumber;
                totalGameScores=userArr[findIndex].totalGameScore;
                //userArr[findIndexOppo].isWin = 1;
                resolve({
                    roomName: condObj.roomName,
                    users: condObj.userId,
                    remainingScore: calculatedScore,
                    finalArr: userArr,
                    userTurn: userTurn,
                    dartPoint: dartPnt,
                    playStatus: playStatus,
                    isWin: isWin,
                    playerScore: playerScore,
                    
                    cupNumber: cupNumber,
                    gameTotalTime:gameSeconds,
                    opponentUserId:userOpponentUserId,
                    ////////////////////////////////////
                    opponentCup:cupNumber,
                    cupNumber:cupNumberOppo,
                    availableCoin:userArr[findIndex].roomCoin,
                    opponentCoin:userArr[findIndex].roomCoin,
                    
                    totalGameScores:gameScoreOpponent,
                    gameScoreOpponent:totalGameScores
                });

    });
});
}

room.userLeaveNew = function (condObj, updateObj) {
    return new Promise((resolve, reject) => {
        room.findOne({roomName: condObj.roomName}
            , function (err, result) {
                let score;
                let multiplier;
                let calculatedScore;
                let boardScore;
                let userArr = [];
                let newArr = [];
                let newArr1 = [];
                let newArr3 = [];
                let userTurn;
                let dartPnt;
                let remainingScore;
                let isWin=0;
                let userTurnOppnt;
                let userTurnGame;
                boardScore = condObj.score;
                let playStatus = 0;
                let cupNumber;
                let playerScore;
                userArr = result.users;
                let currentTime=new Date().getTime();
                const diff = currentTime - result.createtime;
                //const diff = currentTime - result.gametime;
                //const gameSeconds = Math.floor(diff / 1000 % 60);
                let cupNumberOppo;
                let opponentCoin;
                let userCoin;
                let totalGameScores;
                let gameScoreOpponent;

                let dt1=moment().format('MM/DD/YYYY HH:mm:ss');
                    let config = "MM/DD/YYYY HH:mm:ss";
                    let ms = moment(dt1, config).diff(moment(result.createtime,config));
                    let d = (moment.duration(ms))/1000;
                    let gameSeconds = Math.floor(d);

                    if(gameSeconds >360){
                        gameSeconds=360;
                    }






                let findIndex = userArr.findIndex(elemt => elemt.userId === condObj.userId);
                //here opponent is winner
                let findIndexOppo = userArr.findIndex(elemt => elemt.userId != condObj.userId);
                //winner user id
                let findIndexOppoUserId=userArr[findIndexOppo].userId;
                 totalGameScores=userArr[findIndex].totalGameScore;

                /////09_06//////////
                if(findIndexOppo !=-1 && gameSeconds <=8){
                    userArr[findIndex].isWin=2;
                    userArr[findIndexOppo].isWin=2;
                    userArr[findIndex].status = "leave";
                    playStatus=userArr[findIndex].playStatus;

                    //playStatus=2;
                    isWin=2;
                    //userArr[findIndexOppo].cupNumber=70;
                    //userArr[findIndex].cupNumber=70;

                    userArr[findIndex].cupNumber=
                    Math.round(((199-userArr[findIndex].total)*70/199),0);

                    userArr[findIndexOppo].cupNumber=
                    Math.round(((199-userArr[findIndexOppo].total)*70/199),0);


                    cupNumberOppo=userArr[findIndexOppo].cupNumber;

                    gameScoreOpponent=userArr[findIndexOppo].totalGameScore;
                    opponentCoin=userArr[findIndexOppo].roomCoin;

                }
                ///09_o6//////////////

                 
                if(findIndexOppo !=-1 &&  gameSeconds >8){
                    //if(userArr[findIndexOppo].total != userArr[findIndex].total){
                    userArr[findIndexOppo].isWin = 1;
                    //userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].status = "leave";
                    playStatus=userArr[findIndex].playStatus;
                    isWin=1;

                    userArr[findIndexOppo].cupNumber=Math.round(((199-userArr[findIndexOppo].total)*70/199),0);


                    if(userArr[findIndex].total <99){
                         cupNumberOppo=Math.round((parseInt(userArr[findIndex].total)+25),0);
                            }
                      else{
                         cupNumberOppo=Math.round((parseInt(userArr[findIndex].total)-25),0);
                        } 

                     cupNumberOppo=Math.round(((cupNumberOppo)*70/199),0);
                     //cupNumberOppo=Math.round(((199-cupNumberOppo)*70/199),0);    

                    //cupNumberOppo = Math.round(((userArr[findIndexOppo].total / 333) * 100), 0);
                    //cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                    userArr[findIndex].cupNumber = cupNumberOppo;

                    ///////coin set///////
                    opponentCoin=userArr[findIndexOppo].roomCoin;
                    gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                 // }

                  /*else{

                    userArr[findIndexOppo].isWin = 2;
                    userArr[findIndex].isWin = 2;
                    //userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].status = "leave";
                    playStatus=userArr[findIndex].playStatus;
                    isWin=2;                
                    ///////coin set///////
                    opponentCoin=userArr[findIndexOppo].roomCoin;
                    gameScoreOpponent=userArr[findIndexOppo].totalGameScore;

                  }*/
                }
                userArr[findIndex].status = "leave";
                calculatedScore= userArr[findIndex].total;
                userTurn=userArr[findIndex].turn;
                dartPnt=userArr[findIndex].dartPoint;
                playStatus=userArr[findIndex].playStatus;
                //isWin=userArr[findIndex].isWin;
                playerScore=userArr[findIndex].score;
                cupNumber=userArr[findIndex].cupNumber;
                userCoin=userArr[findIndex].roomCoin;
                //userArr[findIndexOppo].isWin = 1;
                resolve({
                    roomName: condObj.roomName,
                    users: condObj.userId,
                    remainingScore: calculatedScore,
                    finalArr: userArr,
                    userTurn: userTurn,
                    dartPoint: dartPnt,
                    playStatus: playStatus,
                    isWin: isWin,
                    playerScore: playerScore,
                    cupNumber: cupNumber,
                    gameTotalTime:gameSeconds,
                    opponentUserId:findIndexOppoUserId,
                    opponentCup:userArr[findIndexOppo].cupNumber,
                    //opponentCup:cupNumberOppo,
                    opponentCoin:opponentCoin,
                    userCoin:userCoin,

                    totalGameScores:totalGameScores,
                    gameScoreOpponent:gameScoreOpponent
                });

            });
    });
}

room.findNextUser = function (condObj) {
    return new Promise((resolve, reject) => {
        room.findOne({roomName: condObj.roomName}, function (err, roomDetails) {
            if (roomDetails) {
                // let  bid             = roomDetails.bid;
                let users = roomDetails.users;
                //let  currentBidDirection     = roomDetails.currentBidDirection;
                //const allUsers=namesArray.result.users;
                let findIndex = users.findIndex(elemt => elemt.status === 'active' /*roomDetails.dealStartDirection*/);
                resolve({userId: users[findIndex].userId});
            } else {
                reject({});
            }
        })
    })
}


room.findNextUserDart = function (condObj) {
    return new Promise((resolve, reject) => {
        console.log("next turn console"+condObj.roomName);
        room.findOne({roomName: condObj.roomName}, function (err, roomDetails) {
            if (roomDetails) {
                let users = roomDetails.users;
                let findIndex = users.findIndex(elemt => (elemt.turn > 0 && elemt.turn < 3)/*||  elemt.turn < 1 *//*roomDetails.dealStartDirection*/);
                  logger.print("***Next turn index "+findIndex);
                  //logger.print("current user turn"+users[findIndex].turn);
                //resolve({ userId  : users[findIndex].userId});
                if (findIndex == -1) {

                    let findIndex1 = users.findIndex(elemt => elemt.turn < 1 /*roomDetails.dealStartDirection*/);
                    logger.print("***Next turn index "+findIndex1);
                    if (findIndex1 != -1)
                        resolve({userId: users[findIndex1].userId});
                    else
                        logger.print("***Next user not found");
                        reject({message: "User not found"});
                } else {
                    resolve({userId: users[findIndex].userId});
                }
            } else {
                logger.print("***Room not found while fetch next turn ");
                reject({});
            }
        })
    })
}


room.findNextUserDartMod = function (condObj) {
    return new Promise((resolve, reject) => {
        room.findOne({roomName: condObj.roomName}, function (err, roomDetails) {
            if (roomDetails) {
                let users=[];
                users = roomDetails.users;
                let findIndex = users.findIndex(elemt => (elemt.userId!=condObj.userId)/*||  elemt.turn < 1 *//*roomDetails.dealStartDirection*/);
                users[findIndex].turn = 4;
                //resolve({userId: users[findIndex].userId});

                room.update({roomName: condObj.roomName}, {$set: {users: users}},

                    function (err, updateroomresult) {
                        if (err)
                            reject({message: "Error:Database connection error"})
                        else {
                            if (updateroomresult > 0)
                                resolve({userId: users[findIndex].userId});
                            else
                                reject({message: "Unable to update memory room"});
                        }

                    });
            } else {
                reject({});
            }
        })
    })
}


room.updateInmemoryRoom = function (userObj, updateArr) {
    return new Promise((resolve, reject) => {

        room.update({roomName: userObj.roomName}, {$set: {users: updateArr.finalArr}},

            //room.update({roomName : userObj.roomName},{ $set: { users: updateArr.finalArr.users }},
            function (err, updateroomresult) {
                if (err)
                    reject({message: "Error:Database connection error"})
                else {
                    if (updateroomresult > 0)
                        //resolve({roomName : userObj.roomName,userArr:updateArr})
                        resolve({
                            roomName: userObj.roomName,
                            userId: updateArr.users,
                            remainingScore: updateArr.remainingScore,
                            dartPoint: updateArr.dartPoint
                        })
                    //resolve({userId: updateArr.users,remainingScore:updateArr.remainingScore,userTurn:updateArr.userTurn,dartPoint:updateArr.dartPoint})
                    else
                        reject({message: "Unable to update memory room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });

    })
}

room.updateInmemoryRoomMod12 = function (updateArr) {
    return new Promise((resolve, reject) => {
        console.log("816 inmpre1"+updateArr.opponentCup);

        room.update({roomName: updateArr.roomName}, {$set: {gameTotalTime:updateArr.gameTotalTime,users: updateArr.finalArr}},

            //room.update({roomName : userObj.roomName},{ $set: { users: updateArr.finalArr.users }},
            function (err, updateroomresult) {
                if (err)
                    reject({message: "Error:Database connection error"})
                else {
                    console.log("update memory room successfully after dart thrown");
                    if (updateroomresult > 0)
                        //resolve({roomName : userObj.roomName,userArr:updateArr})
                        resolve({
                            roomName: updateArr.roomName,
                            userId: updateArr.users,
                            remainingScore: updateArr.remainingScore,
                            dartPoint: updateArr.dartPoint,
                            playStatus: updateArr.playStatus,
                            isWin: updateArr.isWin,
                            roomUsers: updateArr.finalArr,
                            playerScore: updateArr.playerScore,
                            cupNumber: updateArr.cupNumber,
                            gameTotalTime:updateArr.gameTotalTime,
                            availableCoin:updateArr.userCoin,
                            opponentCup:updateArr.opponentCup,
                            opponentUserId:updateArr.opponentUserId,
                            opponentCoin:(!updateArr.opponentCoin)? updateArr.userCoin : updateArr.opponentCoin,
                            roundScore:updateArr.roundScore,
                            userTurn:updateArr.userTurn,

                            totalGameScores:updateArr.totalGameScores,
                            gameScoreOpponent:updateArr.gameScoreOpponent,
                            ///////
                            hitScore:updateArr.hitScore,
                            scoreMultiplier:updateArr.scoreMultiplier


                        })
                    //resolve({userId: updateArr.users,remainingScore:updateArr.remainingScore,userTurn:updateArr.userTurn,dartPoint:updateArr.dartPoint})
                    else
                        reject({message: "Unable to update memory room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });

    })
}

room.updateInmemoryRoomMod = function (updateArr) {
    return new Promise((resolve, reject) => {
        console.log("816 inmpre1"+updateArr.opponentCup);

        room.update({roomName: updateArr.roomName}, {$set: {gameTotalTime:updateArr.gameTotalTime,users: updateArr.finalArr}},

            //room.update({roomName : userObj.roomName},{ $set: { users: updateArr.finalArr.users }},
            function (err, updateroomresult) {
                if (err)
                    reject({message: "Error:Database connection error"})
                else {
                    console.log("update memory room successfully after dart thrown");
                    if (updateroomresult > 0)
                        //resolve({roomName : userObj.roomName,userArr:updateArr})
                        resolve({
                            roomName: updateArr.roomName,
                            userId: updateArr.users,
                            remainingScore: updateArr.remainingScore,
                            dartPoint: updateArr.dartPoint,
                            playStatus: updateArr.playStatus,
                            isWin: updateArr.isWin,
                            roomUsers: updateArr.finalArr,
                            playerScore: updateArr.playerScore,
                            cupNumber: updateArr.cupNumber,
                            gameTotalTime:updateArr.gameTotalTime,
                            availableCoin:(!updateArr.userCoin)? updateArr.availableCoin :updateArr.userCoin,
                            opponentCup:updateArr.opponentCup,
                            opponentUserId:updateArr.opponentUserId,
                            opponentCoin:updateArr.opponentCoin,

                            totalGameScores:updateArr.totalGameScores,
                            gameScoreOpponent:updateArr.gameScoreOpponent
                            //roundScore:updateArr.roundScore
                        })
                    //resolve({userId: updateArr.users,remainingScore:updateArr.remainingScore,userTurn:updateArr.userTurn,dartPoint:updateArr.dartPoint})
                    else
                        reject({message: "Unable to update memory room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });

    })
}


room.updateInmemoryRoomLeave = function (userObj, updateArr) {
    return new Promise((resolve, reject) => {

        room.update({roomName: userObj.roomName}, {$set: {users: updateArr.userAr}},
            function (err, updateroomresult) {
                if (err)
                    reject({message: "Error:Database connection error"})
                else {
                    if (updateroomresult > 0)
                        resolve({usertotal: updateArr.userAr, total: updateArr.totalArr})
                    else
                        reject({message: "Unable to update memory room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });

    })
}

room.updateInmemoryRoomLeaveDisconnect = function (roomName, updateArr) {
    return new Promise((resolve, reject) => {

        room.update({roomName: roomName}, {$set: {users: updateArr.userAr}},
            function (err, updateroomresult) {
                if (err)
                    reject({message: "Error:Database connection error"})
                else {
                    if (updateroomresult > 0)
                        resolve({usertotal: updateArr.userAr, total: updateArr.totalArr})
                    else
                        reject({message: "Unable to update memory room"});

                }

            });

    })
}

room.userLeave1 = function (condObj, updateObj) {
    console.log("***** leaveJoinee >> ", condObj);
    return new Promise((resolve, reject) => {
        room.update({roomName: condObj.roomName},
            {
                $pull: {
                    users: {userId: condObj.userId},
                }
            },
            {multi: true},
            function (err, updateRoom) {
                if (err) {
                    reject({})
                } else {
                    resolve(updateRoom)
                }
            });
    });
}


room.userLeaveMod = function (condObj, updateObj) {
    console.log("***** leaveJoinee >> ", condObj);
    return new Promise((resolve, reject) => {
        room.update({roomName: condObj.roomName, users: {userId: condObj.userId}},
            {
                $set: {
                    users: {isWin: 1},
                }
            },
            function (err, updateRoom) {
                if (err) {
                    reject({})
                } else {
                    resolve(updateRoom)
                }
            });
    });
}

///game time ///////////////
room.gameTimerStart = function (reqObj) {
    return new Promise((resolve, reject) => {
        console.log("game timer room start");

        room.findOne({roomName: reqObj.roomName}
            , function (err, result) {
                if (result) {
                   console.log("result found");
                    var i = 6;
                 //logger.print("  ************  first turn loop start");
                 let timer = setTimeout(function gameStartTimmer1(gameStartObj1) {
                    i--;
                    if(i===6){
                      //brodcast////////////
                      console.log("game time start");
                     io.to(gameStartObj1.roomName).emit('gameTimer', response.generate(constants.SUCCESS_STATUS, {}, "Your game time start"));

                    }

                    if (i === 0) {
                        //game winner loser clculation///////////////
                        console.log("game time finish");
                        console.log("first turn i turn 0 "+roomDetails.userId+gameStartObj1.roomName);
                        clearTimeout(this.interval);
                        //logger.print("Next turn sent after game request"+roomDetails.userId);
                        //io.to(gameStartObj1.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
                    }
                    if(i===4){
                      //brodcast////////////
                      console.log("only 4 sec remaining");
                     io.to(gameStartObj1.roomName).emit('gameTimer', response.generate(constants.SUCCESS_STATUS, {}, "Only 10 seconds remaining"));

                    }                    
                     else {
                        gameStartObj1.i = i
                        timer = setTimeout(gameStartTimmer1, 1000, gameStartObj1);
                    }
                }, 1000, roomObj);
                  
                } else {
                    console.log("Unable to find room"+reqObj.roomName);
                    reject({message: "No room found"});
                }
            });
    });
}

///rejoin ///
room.findNextUserDartJoin = function (condObj) {
    return new Promise((resolve, reject) => {
        console.log("next turn console"+condObj.roomName);
        RoomDb.findOne({name: condObj.roomName}, function (err, roomDetails) {
            if (roomDetails) {
                let users = roomDetails.users;
                let findIndex = users.findIndex(elemt => (elemt.turn > 0 && elemt.turn < 3)/*||  elemt.turn < 1 *//*roomDetails.dealStartDirection*/);
                  logger.print("***Next turn index "+findIndex);
                  //logger.print("current user turn"+users[findIndex].turn);
                //resolve({ userId  : users[findIndex].userId});
                if (findIndex == -1) {

                    let findIndex1 = users.findIndex(elemt => elemt.turn < 1 /*roomDetails.dealStartDirection*/);
                    logger.print("***Next turn index "+findIndex1);
                    if (findIndex1 != -1)
                        resolve({userId: users[findIndex1].userId});
                    else
                        logger.print("***Next user not found");
                        reject({message: "User not found"});
                } else {
                    resolve({userId: users[findIndex].userId});
                }
            } else {
                logger.print("***Room not found while fetch next turn ");
                reject({});
            }
        })
    })
}

//winAfterTimerEnd

room.winAfterTimerEnd = function (reqObj) {
    return new Promise((resolve, reject) => {

        console.log("reqObj"+reqObj);
        let score;
        let multiplier;
        let calculatedScore;
        let boardScore;
        let userArr = [];
        let newArr = [];
        let newArr1 = [];
        let newArr3 = [];
        let userTurn;
        let dartPnt;
        let remainingScore=0;
        let isWin;
        let userTurnOppnt;
        let userTurnGame;
        boardScore = reqObj.score;
        let playStatus = 0;
        let cupNumber;
        let cupNumberOppo;
        let availableCoin;
        let userScore;
        //let findIndexOpponentMod;
        let cupOpponent;
        let userTotalScore=0;
        let userRemainScore=0;
        let roundScore=0;
        let playerScore;
        let gameres;
        let totalGameScores;
        let gameScoreOpponent;

        //const gameSeconds = Math.floor(diff / 1000 % 60);

        room.findOne({roomName: reqObj}
            , function (err, result) {
                if (result) {
                    console.log("result  found");
                   /* if(result.game_time!='undefined'){
                        reject({message: "Game is over"});
                    }*/

                    let dt1=moment().format('MM/DD/YYYY HH:mm:ss');
                    let config = "MM/DD/YYYY HH:mm:ss";
                    let ms = moment(dt1, config).diff(moment(result.createtime,config));
                    let d = (moment.duration(ms))/1000;
                    let gameSeconds = Math.floor(d);

                    if(gameSeconds >360){
                        gameSeconds=360;
                    }

                    userArr = result.users;  

                    console.log(userArr);

                    let currentTime=new Date().getTime();
                    const diff = currentTime - result.createtime;
                    //const diff = currentTime - result.gametime;
                    //const gameSeconds = Math.floor(diff / 1000 % 60);

                    //console.log(userArr[0].total); 

                    let min = userArr.reduce(function (prev, current) {
                      console.log("min prev.total"+prev.total);
                      console.log("min current.total"+current.total);  
                      return (prev.total > current.total) ? prev : current
                    });

                    console.log("min"+min);

                    let equalScore = userArr.reduce(function (prev, current) {
                      console.log("equal prev.total"+prev.total);
                      console.log("equal current.total"+current.total);
                      return (prev.total === current.total) ? prev : 0
                    });

                    console.log("equalScore"+equalScore.total);

                    /*let max = userArr.reduce(function (prev, current) {
                      console.log("max prev.total"+prev.total);
                      console.log("max current.total"+current.total);    
                      return (prev.total < current.total) ? prev : 0
                    });*/
                    
                    let max = userArr.reduce(function (prev, current) {
                      console.log("max prev.total"+prev.total);
                      console.log("max current.total"+current.total);    
                      return (prev.total < current.total) ? prev : current
                    });

                    console.log("max"+max.userId);

                    if(equalScore.total >0){
                        //draw//////

                     let findIndex = userArr.findIndex(elemt => elemt.userId === equalScore.userId);
                    console.log("equal userId"+findIndex);

                    let findIndexOppo = userArr.findIndex(elemt => elemt.userId != equalScore.userId);
                    console.log("equal userId"+findIndexOppo);

                    userArr[findIndex].isWin = 2;                   
                    userArr[findIndexOppo].isWin = 2;  

                    playStatus=userArr[findIndex].playStatus;
                    isWin=2;

                    userArr[findIndex].cupNumber=Math.round(((199-userArr[findIndex].total)*70/199),0);

                    userArr[findIndexOppo].cupNumber=Math.round(((199-userArr[findIndexOppo].total)*70/199),0);
                    cupNumberOppo=userArr[findIndexOppo].cupNumber;
                    
                    //userArr[findIndex].cupNumber = cupNumberOppo;

                calculatedScore= userArr[findIndex].total;
                userTurn=(!userArr[findIndex].turn) ? 0 : userArr[findIndex].turn;
                dartPnt=(!userArr[findIndex].dartPoint) ? 0 : userArr[findIndex].dartPoint;
                playStatus=(!userArr[findIndex].playStatus) ? 0 : userArr[findIndex].playStatus;
                //isWin=userArr[findIndex].isWin;
                playerScore=userArr[findIndex].score;
                cupNumber=userArr[findIndex].cupNumber;
                cupNumberOppo=(!userArr[findIndexOppo].cupNumber) ? 0 : userArr[findIndexOppo].cupNumber;
                //userArr[findIndexOppo].isWin = 1;
                //RoomDb.find({name:"RM1586354802229"},{game_time:1})
                //.then(gameresponses=> {

                  RoomDb.findOne({name:reqObj}, {_id: 1,game_time:1, name:1}).then(gameresponses=> {
                   console.log("length"+gameresponses.game_time);
                   if(gameresponses.game_time >0){
                    isWin=0;
                   }

                   resolve({
                    roomName: reqObj,
                    users: userArr[findIndex].userId,
                    remainingScore: calculatedScore,
                    finalArr: userArr,
                    userTurn: userTurn,
                    dartPoint: dartPnt,
                    playStatus: playStatus,
                    isWin: isWin,
                    playerScore: playerScore,
                    
                    cupNumber: cupNumber,
                    gameTotalTime:gameSeconds,
                    opponentUserId:userArr[findIndexOppo].userId,
                    ////////////////////////////////////
                    opponentCup:cupNumberOppo,                    
                    availableCoin:userArr[findIndex].roomCoin,
                    opponentCoin:userArr[findIndex].roomCoin,
                    userCoin:userArr[findIndex].roomCoin,
                    roundScore:roundScore,

                    totalGameScores:userArr[findIndex].totalGameScore,
                    gameScoreOpponent:userArr[findIndexOppo].totalGameScore
                    //gameres:gameresponses
                });

                }).catch(err => {
                   reject(err);
                });

                    }
                    else {
                    let findIndex = userArr.findIndex(elemt => elemt.userId === max.userId);
                    console.log("max userId"+findIndex);

                    let findIndexOppo = userArr.findIndex(elemt => elemt.userId != max.userId);
                    console.log("min userId"+findIndexOppo);

                    userArr[findIndex].isWin = 1;                   
                    
                    playStatus=userArr[findIndex].playStatus;
                    isWin=1;

                    userArr[findIndex].cupNumber=Math.round(((199-userArr[findIndex].total)*70/199),0);
                    console.log("userArr[findIndex].cupNumber"+userArr[findIndex].cupNumber);
                    

                    /*if(userArr[findIndexOppo].total <99){
                         cupNumberOppo=Math.round((parseInt(userArr[findIndexOppo].total)+25),0);
                            }
                      else{
                         cupNumberOppo=Math.round((parseInt(userArr[findIndexOppo].total)-25),0);
                        }*/


                    if(userArr[findIndexOppo].total <99){
                                     cupNumberOppo=Math.round((parseInt(userArr[findIndexOppo].total)+25),0);
                                    }
                                else{
                                   cupNumberOppo=Math.round((parseInt(userArr[findIndexOppo].total)-25),0);
                                } 

                    cupNumberOppo=Math.round(((cupNumberOppo)*70/199),0);    


                    //cupNumberOppo=Math.round(((199-userArr[findIndexOppo].total)*70/199),0);
                    

                    userArr[findIndexOppo].cupNumber = cupNumberOppo;
                     console.log("userArr[findIndex].total"+userArr[findIndex].total);
                     console.log("userArr[findIndex].cupNumber"+userArr[findIndex].cupNumber);
                     console.log("userArr[findIndexOppo].cupNumber"+userArr[findIndexOppo].cupNumber);
                     console.log("userArr[findIndexOppo].total"+userArr[findIndexOppo].total);
                calculatedScore= userArr[findIndex].total;
                userTurn=(!userArr[findIndex].turn) ? 0 : userArr[findIndex].turn;
                dartPnt=(!userArr[findIndex].dartPoint) ? 0 : userArr[findIndex].dartPoint;
                playStatus=(!userArr[findIndex].playStatus) ? 0 : userArr[findIndex].playStatus;
                //isWin=userArr[findIndex].isWin;
                playerScore=userArr[findIndex].score;
                cupNumber=userArr[findIndex].cupNumber;
                cupNumberOppo=(!userArr[findIndexOppo].cupNumber) ? 0 : userArr[findIndexOppo].cupNumber;
                //userArr[findIndexOppo].isWin = 1;
                //RoomDb.find({name:"RM1586354802229"},{game_time:1})
                //.then(gameresponses=> {

                  RoomDb.findOne({name:reqObj}, {_id: 1,game_time:1, name:1}).then(gameresponses=> {
                   console.log("length"+gameresponses.game_time);
                   if(gameresponses.game_time >0){
                    isWin=0;
                   }

                   resolve({
                    roomName: reqObj,
                    users: userArr[findIndex].userId,
                    remainingScore: calculatedScore,
                    finalArr: userArr,
                    userTurn: userTurn,
                    dartPoint: dartPnt,
                    playStatus: playStatus,
                    isWin: isWin,
                    playerScore: playerScore,
                    
                    cupNumber: cupNumber,
                    gameTotalTime:gameSeconds,
                    opponentUserId:userArr[findIndexOppo].userId,
                    ////////////////////////////////////
                    opponentCup:cupNumberOppo,                    
                    availableCoin:userArr[findIndex].roomCoin,
                    opponentCoin:userArr[findIndex].roomCoin,
                    userCoin:userArr[findIndex].roomCoin,
                    roundScore:roundScore,
                    //gameres:gameresponses
                    totalGameScores:userArr[findIndex].totalGameScore,
                    gameScoreOpponent:userArr[findIndexOppo].totalGameScore
                });

                }).catch(err => {
                   reject(err);
                });

            }

                /*resolve({
                    roomName: reqObj,
                    users: userArr[findIndex].userId,
                    remainingScore: calculatedScore,
                    finalArr: userArr,
                    userTurn: userTurn,
                    dartPoint: dartPnt,
                    playStatus: playStatus,
                    isWin: isWin,
                    playerScore: playerScore,
                    
                    cupNumber: cupNumber,
                    gameTotalTime:gameSeconds,
                    opponentUserId:userArr[findIndexOppo].userId,
                    ////////////////////////////////////
                    opponentCup:cupNumberOppo,                    
                    availableCoin:userArr[findIndex].roomCoin,
                    opponentCoin:userArr[findIndex].roomCoin,
                    userCoin:userArr[findIndex].roomCoin,
                    roundScore:roundScore
                });*/

                    //fetch game finish or not////



                    /*RoomDb.find({name:"RM1586354802229",game_time:{ '$gte': '0' }}).then(responses=> {
                        if(responses){                          
                          console.log("ok"+responses.length);
                          resolve(true);
                        }
                        else{                            
                            console.log("no responses");
                            resolve(true);
                        }
                    }).catch(err => {
                         reject(err);
                    });*/

                    //resolve(true);





                    /*userArr.map(function(key,entry) {
                        console.log(userArr[key]['total']);
                       
                 });*/
               }
               else{
                console.log("result not found");
               }
                    
            });
               
    });
}



room.findNextRejoinMod = function (condObj) {
    return new Promise((resolve, reject) => {
        console.log("next turn console"+condObj.roomName);
        room.findOne({roomName: condObj.roomName}, function (err, roomDetails) {
            if (roomDetails) {
                let users = roomDetails.users;
                let findIndex = users.findIndex(elemt => (elemt.turn > 0 && elemt.turn < 3)/*||  elemt.turn < 1 *//*roomDetails.dealStartDirection*/);
                  logger.print("***Next turn index "+findIndex);
                  //logger.print("current user turn"+users[findIndex].turn);
                //resolve({ userId  : users[findIndex].userId});
                if (findIndex == -1) {

                    let findIndex1 = users.findIndex(elemt => elemt.turn < 1 /*roomDetails.dealStartDirection*/);
                    logger.print("***Next turn index "+findIndex1);
                    if (findIndex1 != -1)
                        resolve({userId: users[findIndex1].userId,turnstat:0});
                    else
                        logger.print("***Next user not found");
                        reject({message: "User not found"});
                } else {
                    resolve({userId: users[findIndex].userId,turnstat:1});
                }
            } else {
                logger.print("***Room not found while fetch next turn ");
                reject({});
            }
        })
    })
}

module.exports = room;
