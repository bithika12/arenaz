"use strict";
const appRoot = require('app-root-path');
//INCLUDE CONSTANT
var constants = require(appRoot + "/config/constants");
//INCLUDE UTILS PACKAGE
var room = require(appRoot + '/utils/MemoryDatabaseManger').room;
//INCLUDE MODEL
//const  deck            = require('../gameplay/Deck');
//INCLUDE GAME  CLASS MODULE
//const Bridge    = require('../gameplay/Bridge');
//const  Bid            = require('../gameplay/Bid');
var _ = require("underscore");
const Math = require('math');
const logger = require(appRoot + '/utils/LoggerClass');
/**
 * @desc This function is used for calculate score
 * @param {Object} reqObj
 */
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
        let remainingScore;
        let isWin;
        let userTurnOppnt;
        let userTurnGame;
        boardScore = reqObj.score;
        let playStatus = 0;
        let cupNumber;
        let cupNumberOppo;
        let availableCoin;

        room.findOne({roomName: reqObj.roomName}
            , function (err, result) {
                if (result) {
                    userArr = result.users;
                    let currentTime=new Date().getTime();
                    const diff = currentTime - result.gametime;
                    const gameSeconds = Math.floor(diff / 1000 % 60);

                    let findIndexOppo = userArr.findIndex(elemt => (elemt.turn >= 3 && elemt.userId != reqObj.userId/*roomDetails.dealStartDirection*/));
                    if (findIndexOppo != -1)
                        userArr[findIndexOppo].turn = 0;
                    userArr.findIndex(function (elemt) {

                        if (elemt.userId == reqObj.userId) {
                            remainingScore = elemt.total;
                            userTurn = elemt.turn + 1;
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
                            if (reqObj.score == 1 || reqObj.score < 0 || calculatedScore < 0) {
                                //reject({message:"It is bust"});
                                console.log("It is a bust");
                                userTurn = 3;
                                playStatus = 1;
                                calculatedScore=remainingScore;
                            }
                            if (calculatedScore == 0) {
                                console.log("win the match");
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
                        availableCoin:availableCoin
                    });
                } else {
                    console.log("Unable to find room");
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
        room.findOne({roomId: conditionObj.roomId}, function (err, roomresult) {
            var userArr = [];
            if (roomresult == null || roomresult.length == 0) {
                userArr.push(updateObj.userObj)
                room.insert({
                    roomId: conditionObj.roomId,
                    roomName: conditionObj.roomName,
                    users: userArr,
                    totalUser: 1,
                    createtime: new Date().getTime(),
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
                        createtime: new Date().getTime(),
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
                const diff = currentTime - result.gametime;
                const gameSeconds = Math.floor(diff / 1000 % 60);

                let findIndex = userArr.findIndex(elemt => elemt.userId === condObj.userId);

                let findIndexOppo = userArr.findIndex(elemt => elemt.userId != condObj.userId);

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

                }
                if(findIndexOppo !=-1 &&  gameSeconds >8){
                    userArr[findIndexOppo].isWin = 1;
                    userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].status = "inactive";
                    playStatus=userArr[findIndex].playStatus;
                    isWin=1;

                    cupNumberOppo = Math.round(((userArr[findIndexOppo].total / 333) * 100), 0);
                    cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                    userArr[findIndex].cupNumber = cupNumberOppo;
                }
                //userArr[findIndex].status = "inactive";
                calculatedScore= userArr[findIndex].total;
                userTurn=userArr[findIndex].turn;
                dartPnt=userArr[findIndex].dartPoint;
                playStatus=userArr[findIndex].playStatus;
                //isWin=userArr[findIndex].isWin;
                playerScore=userArr[findIndex].score;
                cupNumber=userArr[findIndex].cupNumber;

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
                    gameTotalTime:gameSeconds
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
                const diff = currentTime - result.gametime;
                const gameSeconds = Math.floor(diff / 1000 % 60);
                let cupNumberOppo;

                let findIndex = userArr.findIndex(elemt => elemt.userId === condObj.userId);

                let findIndexOppo = userArr.findIndex(elemt => elemt.userId != condObj.userId);


                if(findIndexOppo !=-1){
                    userArr[findIndexOppo].isWin = 1;
                    userArr[findIndexOppo].cupNumber=70;
                    userArr[findIndex].status = "inactive";
                    playStatus=userArr[findIndex].playStatus;
                    isWin=1;

                    cupNumberOppo = Math.round(((userArr[findIndexOppo].total / 333) * 100), 0);
                    cupNumberOppo = Math.round(((cupNumberOppo * 70) / 100), 0);
                    userArr[findIndex].cupNumber = cupNumberOppo;
                }
                userArr[findIndex].status = "inactive";
                calculatedScore= userArr[findIndex].total;
                userTurn=userArr[findIndex].turn;
                dartPnt=userArr[findIndex].dartPoint;
                playStatus=userArr[findIndex].playStatus;
                //isWin=userArr[findIndex].isWin;
                playerScore=userArr[findIndex].score;
                cupNumber=userArr[findIndex].cupNumber;

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
                    gameTotalTime:gameSeconds
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
        room.findOne({roomName: condObj.roomName}, function (err, roomDetails) {
            if (roomDetails) {
                let users = roomDetails.users;
                let findIndex = users.findIndex(elemt => (elemt.turn > 0 && elemt.turn < 3)/*||  elemt.turn < 1 *//*roomDetails.dealStartDirection*/);
                  logger.print("***Next turn index "+findIndex);
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

room.updateInmemoryRoomMod = function (updateArr) {
    return new Promise((resolve, reject) => {

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
                            availableCoin:updateArr.availableCoin
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


module.exports = room;
