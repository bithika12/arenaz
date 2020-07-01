/** Required Packeges include.*/
const appRoot = require('app-root-path');
var async = require('async');
/* Include Constants*/
var constants = require(appRoot + "/config/constants");
/** Including Models for db operations.*/
var user = require(appRoot + '/models/User');
var room = require(appRoot + '/models/Room');
var inmRoom = require(appRoot + '/models/inmemory/Room');
/*include utils packages */
var io = require(appRoot + '/utils/SocketManager').io;
const response = require(appRoot + '/utils/ResponseManeger');
const logger = require(appRoot + '/utils/LoggerClass');
const AI = require(appRoot + '/models/gameplay/AI');

var roomDatastore = require(appRoot + '/utils/MemoryDatabaseManger').room;
let allOnlineUsers = require(appRoot + '/utils/MemoryDatabaseManger').allOnlineUsers;

let gameRequestQueue = [];
let requestIsRunning = false;
let waitingDartInterval = [];
let count = 0;
let Notification  = require(appRoot +'/models/Notification')
let _ = require('underscore');
let dartArray=[];
let Timer_Started=true;
const moment=require("moment");
//let g;

/*room.createRoom({userId : "5de7ac25c9dba27a72be9023"}).then(function(result){
	console.log("success",result);
}).catch(err=>{
	console.log("error",err);
})*/
var roomMemory = require(appRoot + '/utils/MemoryDatabaseManger').room;
var RoomDb = require(appRoot + '/schema/Schema').roomModel;
var RoomlogDb = require(appRoot + '/schema/Schema').roomLogModel;

io.on('connection', function (socket) {



    function drawResult(reqobj){
        //clearInterval(waitingDartInterval[reqobj.roomName]);
        return function (callback) {
            inmRoom.throwDartDetailsDraw(reqobj).then(function(roomDetails){
                console.log("roomdetails",roomDetails)
                callback(null, roomDetails);
            }).catch(err=>{
                callback("err", null);
            })

        }
    }
   function dartProcess(reqobj){
       //clearInterval(waitingDartInterval[reqobj.roomName]);
		return function (callback) {
			inmRoom.throwDartDetails(reqobj).then(function(roomDetails){
	        console.log("roomdetails",roomDetails)
			callback(null,roomDetails);
		     }).catch(err=>{
	          callback("err", null);
             })

		}
	}


    function updateRoomModified(reqobj,callback){
        console.log("po");
        inmRoom.updateInmemoryRoomMod12(reqobj).then(function(roomDetails){
          console.log("roomdetails",roomDetails)
            callback(null, roomDetails);
        }).catch(err=>{
            callback("err", null);
        })

    }

    function updateRoom(reqobj,callback){
		inmRoom.updateInmemoryRoomMod(reqobj).then(function(roomDetails){
          console.log("roomdetails",roomDetails)
			callback(null, roomDetails);
		}).catch(err=>{
			callback("err", null);
		})

	}
    function intervalFunc() {
        count++;
        client.send(message, 3001, 'localhost', (err) => {
            //client.close();
        });
        if (count == '3') {
            clearInterval(this);
        }
    }
    //gameStatusUpdate

    function gameStatusUpdate(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin==1) {
            //if (reqobj.isWin) {
                //user update with coin
                 user.updateUserCoin({userId: reqobj.userId},
                  {startCoin: reqobj.availableCoin,
                    cupNo:reqobj.cupNumber,
                    userScore:reqobj.totalGameScores
                }).then(function (userStatusUpdate) {
                     callback(null, reqobj);
                });
            } else {
                callback(null, reqobj);
            }
        })
    }

    function gameStatusUpdateOpponent(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin==1) {
            //if (reqobj.isWin) {
                console.log("12"+reqobj.opponentCup);
                //user update with coin
                //reqobj.roomUsers
                let findIndex = reqobj.roomUsers.findIndex(elemt => (elemt.userId!=reqobj.userId));
                let userOppo=reqobj.roomUsers[findIndex].userId;
                console.log("opponent user"+userOppo);
                user.updateUserCoinOpponent({userId: userOppo}, {startCoin: reqobj.availableCoin,cupNo:reqobj.opponentCup,userScore:reqobj.gameScoreOpponent/*reqobj.cupOpponent*/}).then(function (userStatusUpdate) {
                    callback(null, reqobj);
                });
            } else {
                callback(null, reqobj);
            }
        })
    }

    function gameStatusUpdateLeaveMod(reqobj, callback) {
        return new Promise((resolve, reject) => {
                //user update with coin
            console.log("pre"+reqobj.opponentCup);
               if(reqobj.isWin==1){
                user.updateUserCoin({userId: reqobj.opponentUserId},
                    {startCoin: reqobj.opponentCoin,
                    cupNo:reqobj.opponentCup,
                    userScore:reqobj.gameScoreOpponent
                }).then(function (userStatusUpdate) {
                    callback(null, reqobj);
                });

            }
            else{
                callback(null, reqobj);
            }
        })
    }
    //gameStatusUpdateOpponentLeaveMod

    function gameStatusUpdateOpponentLeaveMod(reqobj, callback) {
        return new Promise((resolve, reject) => {
                //user update with coin
                //reqobj.roomUsers
                if(reqobj.isWin==1){
                let findIndex = reqobj.roomUsers.findIndex(elemt => (elemt.userId!=reqobj.userId));
                let userOppo=reqobj.roomUsers[findIndex].userId;
                console.log("opponent user"+userOppo);
                console.log("loser coin"+reqobj.availableCoin);
                user.updateUserCoinOpponent({userId: reqobj.userId},
                    {startCoin: reqobj.availableCoin,
                    cupNo:reqobj.cupNumber,
                    userScore:reqobj.totalGameScores

                }).then(function (userStatusUpdate) {
                    callback(null, reqobj);
                });

            }
            else{
                callback(null, reqobj);
            }

        })
    }

        function gameOverProcess(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin) {
                room.updateRoomGameOver({roomName: reqobj.roomName,gameTotalTime:reqobj.gameTotalTime}, {userObj: reqobj.roomUsers}).then(function (updateRoom) {

                    Notification.createNotification({
                        //sent_by_user     : req.user_id ,
                        received_by_user : reqobj.userId,
                        subject          : "You are winner",
                        message          : "You are winner",
                        read_unread      : 0
                    }).then(function(notificationdetails){

                        user.findDetailsGame({_id:reqobj.userId}).then((firstUserTotalCup)=>{
                        console.log("firstUserTotalCup"+firstUserTotalCup.cupNo);    
                         
                        user.findDetailsGame({_id:reqobj.opponentUserId}).then((secondUserTotalCup)=>{
                        
                        logger.print("***Game Notification added ");
                        logger.print("***Game over successfully ");
                        io.sockets.to(socket.id).emit('gameWin',response.generate( constants.SUCCESS_STATUS,{},"You won the match!"));
                        io.to(reqobj.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                           // userId: reqobj.userId,
                            firstUserId: reqobj.userId,
                            firstUserGameStatus: "Win",
                            secondUserId:reqobj.opponentUserId,
                            secondUserGameStatus: "Lose",
                            roomName: reqobj.roomName,

                            firstUserCupNumber:reqobj.cupNumber,
                            secondUserCupNumber:reqobj.opponentCup,

                            firstUserCoinNumber: reqobj.availableCoin,
                            secondUserCoinNumber: reqobj.opponentCoin,
                            completeStatus:1,

                            firstUserTotalCup: firstUserTotalCup.cupNo,
                            secondUserTotalCup: secondUserTotalCup.cupNo
                            //gameStatus:"Win"
                        }, "Game is over"));
                        callback(null, reqobj);

                        ////

                        }).catch(secondUserTotalCupErr=>{
                        console.log("secondUserTotalCupErr"+secondUserTotalCupErr);
                       })

                       }).catch(firstUserTotalCupErr=>{
                        console.log("firstUserTotalCupErr"+firstUserTotalCupErr);
                      })

                      //////
                    });

                }).catch(err => {
                    logger.print("***Room update error ", err);
                })
            } else {
                callback(null, reqobj);
            }
        })
    }
    function gameOverProcessv1(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin) {
                room.updateRoomGameOver({roomName: reqobj.roomName,gameTotalTime:reqobj.gameTotalTime}, {userObj: reqobj.roomUsers}).then(function (updateRoom) {

                    Notification.createNotification({
                        //sent_by_user     : req.user_id ,
                        received_by_user : reqobj.userId,
                        subject          : "You are winner",
                        message          : "You are winner",
                        read_unread      : 0
                    }).then(function(notificationdetails){

                        logger.print("***Game Notification added ");
                        logger.print("***Game over successfully ");
                        io.sockets.to(socket.id).emit('gameWin',response.generate( constants.SUCCESS_STATUS,{},"You won the match!"));
                        io.to(reqobj.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                           // userId: reqobj.userId,
                            firstUserId: reqobj.userId,
                            firstUserGameStatus: "Win",
                            secondUserId:reqobj.opponentUserId,
                            secondUserGameStatus: "Lose",
                            roomName: reqobj.roomName,

                            firstUserCupNumber:reqobj.cupNumber,
                            secondUserCupNumber:reqobj.opponentCup,

                            firstUserCoinNumber: reqobj.availableCoin,
                            secondUserCoinNumber: reqobj.opponentCoin,
                            completeStatus:1
                            //gameStatus:"Win"
                        }, "Game is over"));
                        callback(null, reqobj);
                    });

                }).catch(err => {
                    logger.print("***Room update error ", err);
                })
            } else {
                callback(null, reqobj);
            }
        })
    }

   ///insertRoomLog
   function insertRoomLog(reqobj, callback) {
        return new Promise((resolve, reject) => {
                console.log("room log cont");
                room.updateRoomLogGameOver({roomName: reqobj.roomName,
                    gameTotalTime:reqobj.gameTotalTime,userId:reqobj.userId}, 
                    {userObj: reqobj.roomUsers},{hitScore:reqobj.hitScore,
                        scoreMultiplier:reqobj.scoreMultiplier}).then(function (updateRoom) {

                    callback(null, reqobj);

                }).catch(err => {
                    logger.print("***Room update error ", err);
                })
            
        })
    }


    /**
     * @desc This function is used for throw dart
     * @param {String} accesstoken
     * @param {String} roomName
     * @param {String} score
     */
    socket.on('throwDart', function (req) {
        req.socketId = socket.id;
        dartArray.push(1);
        //clearTimeout(darttimer[req.roomName]);
        //chk valid dart o.r not
        room.findValidDart({roomName: req.roomName}).then(function (roomDetails) {
            console.log("dartArray length"+dartArray.length)

            async.waterfall([
                dartProcess(req),
                updateRoomModified,

                //updateRoom,                
                //NEWLY ADDED FOR COIN
                gameStatusUpdate,
                gameStatusUpdateOpponent,
                gameOverProcess,
                ////newly added log///
                insertRoomLog,
                ////////////////
                userNextStartDart,
                dartTimer

            ], function (err, result) {
                if (result) {

                    console.log("user life score"+req.roomName);
                    console.log("result.isWin"+result.isWin);
                    logger.print(" throw dart done", req);
                    if (result.playStatus == 1) {
                        logger.print("it is bust");
                    }
                    if(result.isWin==1) {
                        logger.print("winner cup number" + result.cupNumber);
                        logger.print("loser cup number" + result.cupOpponent);
                     }
                    logger.print("1111user remaining score"+ result.remainingScore);
                    logger.print("user score" +result.playerScore);

                    /*io.to(req.roomName).emit('gameThrow', response.generate(constants.SUCCESS_STATUS, {
                        userId: result.userId,
                        roomName: result.roomName,
                        remainingScore: parseInt(result.remainingScore),
                        dartPoint: result.dartPoint,
                        playStatus: result.playStatus,
                        playerScore: result.playerScore,
                        cupNumber: result.cupNumber,
                        hitScore:req.hitScore,
                        scoreMultiplier:req.scoreMultiplier,
                        roundScore:result.roundScore
                    }, "Dart thrown"));*/

                } else
                    logger.print("***May be Only one user in that room so opponent coin update failed ", err);
                io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {"err": err}, "User game is over but still on that room!"));
            });

        }).catch(err => {
            logger.print("game is over but try to throw dart"+req.roomName);
            io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {"err": err}, "game is over but try to throw dart"));
        });

    });


    /**
     * @desc This function is used for fetch turn after dart throw
     * @param {String} accesstoken
     * @param {String} roomName
     * @param {String} score
     */
    socket.on('throwDartComplete', function (req) {
        //req.socketId = socket.id;
        inmRoom.findNextUserDart({roomName: req.roomName}).then(function (roomDetails) {
            if (roomDetails) {
                io.to(req.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
                //clearTimeout(waitingDartInterval[reqobj.roomName]);
            }
        }).catch(err => {
            //reject(err);
        });

    });

    //GAME START

    function waitingForUser(reqobj){
        return function (callback) {
            var i =  constants.GAME_TIMMER;
            let timer = setTimeout(function gameStartTimmer(gameStartObj) {
                var clientsInRoom = io.nsps['/'].adapter.rooms[gameStartObj.roomName];
                var numClients = clientsInRoom === undefined ? 0 : Object.keys(clientsInRoom.sockets).length;
                i--;if (numClients > 0) {
                    if (i === 0 || numClients == 2) {
                        //req.numClients = numClients;
                        //clearTimeout(this.interval);
                        if(numClients==1 && i==0)
                        {
                            gameStartObj.status=1;
                            //update status room
                            room.updateRoomAfterWait({roomName : reqobj.roomName}).then(responses=> {
                                io.sockets.to(reqobj.roomName).emit('noUser', response.generate(constants.ERROR_STATUS, {message: "No opponent found"}));
                                clearTimeout(this.interval);
                                //callback(null, 1);
                                callback(null, gameStartObj);
                            }).catch(err => {
                                clearTimeout(this.interval);
                                callback("playergone", null);
                            });

                        }
                        else {
                            clearTimeout(this.interval);
                            callback(null, gameStartObj);
                          }
                    } else {
                        if(i==0){
                            io.sockets.to(reqobj.roomName).emit('noUser', response.generate(constants.ERROR_STATUS, {message: "Unable to found room"}));
                        }
                        else {
                            gameStartObj.i = i
                            timer = setTimeout(gameStartTimmer, 30000, gameStartObj);
                        }
                    }
                } else {
                    console.log("player left");
                    callback("playergone", null);
               }
            }, 30000,reqobj);
        }
    }
    function waitingForUserOrg(reqobj) {
        let count    = 0;
        return function (callback) {
            var i = constants.GAME_TIMMER;
            logger.print("  ************  game start call");
            let timer = setTimeout(function gameStartTimmer(gameStartObj) {
                i--;
                if (i === 0) {
                    clearTimeout(this.interval);
                    callback(null, gameStartObj);
                } else {
                    gameStartObj.i = i
                    timer = setTimeout(gameStartTimmer, 1000, gameStartObj);
                }
            }, 1000, reqobj);
        }
    }

    function gameStartMod(reqobj) {
        return function (callback) {
            room.updateRoomDetails({roomName: reqobj.roomName}, {status: "open"}).then(function (roomStatusUpdate) {
                inmRoom.getRoomDetails({roomName: roomStatusUpdate}, {status: "closed"}).then(function (roomDetails) {
                    var gameUserList = (!roomDetails) ? [] : roomDetails.users;
                    if (gameUserList.length == 1) {
                        callback(null, reqobj);
                    } else if (gameUserList.length == 2) {
                        room.updateRoomDetails({roomName: reqobj.roomName}, {status: "closed"}).then(function (roomStatusUpdate) {
                            callback(null, reqobj);
                        });
                    } else {
                        inmRoom.removeRoom({roomName: reqobj.roomName}).then(function (roomupdate) {
                            io.to(reqobj.roomName).emit('gameClosed', response.generate(constants.ERROR_STATUS, {}, "Sorry!! All player leave"));
                            callback("all player leave", null);
                        })
                    }
                }).catch(err => {
                    callback("err", null);
                })
            });
        }
    }

    function gameStart(reqobj, callback) {
        inmRoom.getRoomDetails({roomName: reqobj.roomName}, {status: "closed"}).then(function (roomDetails) {
            var gameUserList = (!roomDetails) ? [] : roomDetails.users;
            if (gameUserList.length == 1) {
                callback(null, reqobj);
            } else if (gameUserList.length == 2) {
                room.updateRoomDetails({roomName: reqobj.roomName}, {status: "closed"}).then(function (roomStatusUpdate) {
                    callback(null, reqobj);
                });
            } else {
                inmRoom.removeRoom({roomName: reqobj.roomName}).then(function (roomupdate) {
                    io.to(reqobj.roomName).emit('gameClosed', response.generate(constants.ERROR_STATUS, {}, "Sorry!! All player leave"));
                    callback("all player leave", null);
                })
            }
        }).catch(err => {
            callback("err", null);
        })

    }

    /**
     * @desc next user turn
     * @param reqobj
     * @param callback
     */

    function nextUserTurn(roomObj) {
        inmRoom.findNextUser({roomName: roomObj.roomName}).then(function (roomDetails) {
            if (roomDetails) {
                //new code 26 th mar//
                 var i = 3;
                logger.print("  ************  first turn loop start");
                let timer = setTimeout(function gameStartTimmer1(gameStartObj1) {
                    i--;
                    if (i === 0) {
                        console.log("first turn i turn 0 "+roomDetails.userId+gameStartObj1.roomName);
                        clearTimeout(this.interval);
                        logger.print("Next turn sent after game request"+roomDetails.userId);
                        io.to(gameStartObj1.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
                    } else {
                        gameStartObj1.i = i
                        timer = setTimeout(gameStartTimmer1, 1000, gameStartObj1);
                    }
                }, 1000, roomObj);
                //new code///////////
                //logger.print("Next turn sent after game request"+roomDetails.userId);
                //io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
            }
        }).catch(err => {
        });
    }

    function userNextStart(reqobj, callback) {
        waitingDartInterval[reqobj.roomName] = setTimeout(() => {
            nextUserTurn(reqobj, 0)
        }, 1000);
        callback(null, reqobj);
    }

    const CheckReload = (() => {
        let counter = 0;
        return () => {
            counter++;
            return counter;
        };
    })();


    function userNextStartDart(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin) {
                //ROOM EMPTY AFTER WIN
                let findIndex = allOnlineUsers.findIndex(function (elemt) {
                    return elemt.userId == reqobj.userId
                });
                let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
                    return elemt.userId != reqobj.userId
                });
                allOnlineUsers[findIndex].roomName='';
                allOnlineUsers[findIndexOpponent].roomName='';
                ///ROOM EMPTY AFTER WIN
                callback(null, reqobj);
            }
            else {
              waitingDartInterval[reqobj.roomName] = setTimeout(() => {
                //nextUserTurnDart(reqobj).then(function (nextUsers) {
                //let a=  nextUserTurnDart(reqobj, 0);  
                //console.log("pk12"+nextUserTurnDart(reqobj, 0));
                nextUserTurnDart(reqobj, 0);
                //callback(reqobj,0);
              //}); 
                //nextUserTurnDart(reqobj, 0)
            }, 3000);

            callback(null, reqobj);
           }
        })
    }
    function userNextStartDart45(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.dartPoint != '') {

                inmRoom.findNextUserDart({roomName: reqobj.roomName}).then(function (roomDetails) {
                    if (roomDetails) {
                        io.to(reqobj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));

                    }
                }).catch(err => {
                    //reject(err);
                });

            }
            else{
                waitingDartInterval[reqobj.roomName] = setTimeout(() => {
                    nextUserTurnDart(reqobj, 0)
                }, 1000);
            }
          // callback(null, reqobj);
        })
    }


    function nextUserTurnDart(roomObj) {
        inmRoom.findNextUserDart({roomName: roomObj.roomName}).then(function (roomDetails) {
            if (roomDetails) {
                logger.print("123***Next turn sent "+roomDetails.userId);
                dartArray=[];
                console.log("game timer while turn dart");
                console.log("dartArray"+dartArray.length);
                roomObj.userTurnId=roomDetails.userId;
                //return roomDetails.userId;
                //update turn timer////
                //console.log("game timer while turn dart"+g);
                RoomDb.findOne({name: roomObj.roomName},
                        {_id: 1, turn_time:1,game_time_remain:1}).then(roomresponses=> { 

                RoomDb.updateOne({name: roomObj.roomName},
                 {turn_time:roomresponses.game_time_remain},
                  function (err, updateroomresult) {
                  io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));

                  });

                }).catch(err => {
                  console.log("error while rejoin");
                            
               });
                //update turn timer////
                //io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
                //clearTimeout(waitingDartInterval[reqobj.roomName]);
            }
        }).catch(err => {
            //reject(err);
        });
    }



    function nextUserTurnDartOld(roomObj) {
        inmRoom.findNextUserDart({roomName: roomObj.roomName}).then(function (roomDetails) {
            if (roomDetails) {
                logger.print("***Next turn sent "+roomDetails.userId);
                dartArray=[];
                console.log("dartArray"+dartArray.length);
                roomObj.userTurnId=roomDetails.userId;
                return roomDetails.userId;
                //io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
                //clearTimeout(waitingDartInterval[reqobj.roomName]); 

                 if(roomObj.userTurn ==3){
                    console.log("ok");
                    roomObj.userTurn=0;
                }
                let k=20;
                darttimer = setTimeout(function gameStartTimmer4(gameStartObj4) {
                console.log("ok2");
                if(k==20){
                 io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));

                }
                k--;
                if (k === 0) {
                     console.log("timer"+darttimer);
                      clearTimeout(this.interval); 
                      roomDatastore.findOne({roomName:gameStartObj4.roomName}, function (err, result) {
                       if(result){
                           userArr = result.users;
                           userArr.findIndex(function (elemt) {
                            if (elemt.userId == roomDetails.userId) {

                                if(elemt.turn==roomObj.userTurn){
                                   io.to(roomObj.roomName).emit('dartTimer', 
                                   response.generate(constants.SUCCESS_STATUS, {dartFinish:1},
                                   "No dart thrown"));   
                                }
                                else{
                                    //not required listen
                                    console.log("not required");
                                }
                            }
                           });
                       }
                       else{
                          console.log("no result found");
                       }
                       

                      }).catch(err => {
                        reject(err);
                      });


                    
                } else {
                    //console.log("timer"+darttimer);
                    //if(dartArray.length == 0){
                    //chk user details
                    roomDatastore.findOne({roomName:gameStartObj4.roomName}, function (err, result) {
                       if(result){
                           userArr = result.users;
                           userArr.findIndex(function (elemt) {
                            if (elemt.userId == roomDetails.userId) {

                                if(elemt.turn==roomObj.userTurn){
                                   console.log("dart timer running"+k);
                                   gameStartObj4.k = k
                                   darttimer = setTimeout(gameStartTimmer4, 1000, gameStartObj4); 
                                    
                                }
                                else{
                                    //not required listen
                                    clearTimeout(this.interval); 
                                    console.log("not required");
                                }
                            }
                           });
                       }
                       else{
                          console.log("no result found");
                       }
                       

                      }).catch(err => {
                        reject(err);
                      });

                    
                    //}


                }
            }, 1000, reqobj);
               ////////new code/////////////////////




               
            }
        }).catch(err => {
            //reject(err);
        });
    }


    function nextUserTurnDartMod(roomObj) {
       // clearInterval(waitingDartInterval[roomObj.roomName]);
        inmRoom.findNextUserDartMod({roomName: roomObj.roomName,userId:roomObj.userId}).then(function (roomDetails) {
            if (roomDetails) {
                io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));

            }
        }).catch(err => {
            //reject(err);
        });
    }

    /**
     * @desc This function is used for game request
     * @param {String} accesstoken
     */
    socket.on('gameRequest', function (req) {
        req.socketId = socket.id;
        logger.print(" game request ", req);
        enqueueRequest(req);
    });

    function enqueueRequest(request) {
        gameRequestQueue.push(request);
        if (!requestIsRunning) {
            requestIsRunning = true;
            processGameRequest(gameRequestQueue[0], function callback() {
                gameRequestQueue.splice(0, 1);
                if (gameRequestQueue.length > 0)
                    processGameRequest(gameRequestQueue[0], callback);
                else
                    requestIsRunning = false;
            })
        }
    }

    function processGameRequest(req, callback) {
        if (req.userId && req.userName) {
            let coinArr=[10,50,100,250,500];
            let findCoin = coinArr.findIndex(elemt => elemt === req.roomCoin);
            console.log("findCoin"+findCoin);

           if(findCoin==-1){
               console.log("pl0"); 
               io.sockets.to(socket.id).emit('invalidCoin', 
               response.generate(constants.SUCCESS_STATUS,
                {coin: req.roomCoin
               }, "User enter coin is not valid !"));  
               callback();
            }
            else {
            user.getUserSocketDetails({userId: req.userId}).then((userDetails) => {
                var findIndex = allOnlineUsers.findIndex(function (elemt) {
                    return elemt.userId == req.userId
                });
                 console.log("process"+findIndex);
                 console.log(allOnlineUsers.length);
                 console.log("req user room"+allOnlineUsers[findIndex].roomName);

                 //if(allOnlineUsers.length==0)
                 //if(findIndex == -1 || allOnlineUsers[findIndex].roomName != ''){
                if(findIndex == -1 /*|| allOnlineUsers[findIndex].roomName != ''*/){
                    io.sockets.to(req.socketId).emit('errorJoin',response.generate( constants.ERROR_STATUS,{},"User cannot join"));
                    console.log("   connectedRoom   :"+findIndex+allOnlineUsers[findIndex].roomName,response.generate( constants.ERROR_STATUS,{},"User cannot join"))
                    callback();


                /*if (findIndex == -1 || allOnlineUsers[findIndex].roomName != '') {
                    io.sockets.to(req.socketId).emit('error', response.generate(constants.ERROR_STATUS, {}, "User cannot join"));
                    callback();*/
                } else {
                    //if(findIndex != -1 ){
                    let userSocketId = allOnlineUsers[findIndex].socketId;
                    if (io.sockets.sockets[userSocketId] != undefined) {
                        //update user online status
                        user.updateUserOnlineStatus({
                            userId: req.userId
                        }).then(function (statusResult) {
                        //find user already in room
                        room.createRoom({
                            userId: req.userId,
                            userName: req.userName,
                            colorName: req.colorName,
                            raceName: req.raceName,
                            dartName: req.dartname,
                            roomCoin: req.roomCoin,
                            totalCupWin:req.cupNumbers,
                            firstName: req.firstName,
                            lastName: req.lastName
                        }).then(function (result) {
                            let roomName = result.roomName;
                            userObj = {
                                userId: req.userId,
                                //score: 333,
                                //total: 333,
                                score: 0,
                                roundscore:0,
                                //score: 199,
                                total: 199,
                                status: "active",
                                isWin: 0,
                                turn: 0,
                                dartPoint: "",
                                userName: req.userName,
                                colorName: req.colorName,
                                raceName: req.raceName,
                                dartName: req.dartname,
                                total_no_win: 0,
                                cupNumber: 0,
                                roomCoin: req.roomCoin,
                                totalCupWin:req.cupNumbers,
                                firstName: req.firstName,
                                lastName: req.lastName,
                                totalGameScore:0,

                                ////////////
                                hitScore:0,
                                scoreMultiplier:0

                            };
                            inmRoom.roomJoineeCreation({
                                roomId: result._id,
                                roomName: result.roomName,
                                roomCoin: req.roomCoin
                            }, {userObj: userObj}).then((joineeDetails) => {
                                io.to(roomName).emit('enterUser', response.generate(constants.SUCCESS_STATUS, {user: userObj}, "Player enter to the room"));
                                io.of('/').connected[userSocketId].join(roomName, function () {
                                    allOnlineUsers[findIndex].roomName = roomName;
                                    io.sockets.to(socket.id).emit('connectedRoom', response.generate(constants.SUCCESS_STATUS, {
                                        roomName: roomName,
                                        users: joineeDetails.users
                                    }, "You are waiting in a room !"));
                                    //fetch user colorname,racename//////
                                    //user.fetchColorMod(roomName,joineeDetails.users).then((allRacerDetails)=> {

                                    if (joineeDetails.users.length === 1 && roomName != undefined) {
                                        callback();
                                        async.waterfall([
                                            waitingForUser({roomName: roomName}),
                                            gameStart,
                                            //userStatusUpdateAfterGamerequest
                                        ], function (err, result) {
                                            //console.log("result print"+result.status);
                                            if (result) {
                                                logger.print("***Done  ", result);
                                                io.sockets.to(socket.id).emit('userJoined', response.generate(constants.SUCCESS_STATUS, {
                                                    roomName: roomName,
                                                    users: joineeDetails.users
                                                }, "User enter in a room !"));
                                                //io.sockets.to(roomName).emit('gameStart',response.generate( constants.SUCCESS_STATUS,{roomName: roomName,users :joineeDetails.users },"Game start !"));
                                                if (!result.status) {
                                                    logger.print("wait status found");
                                                    io.sockets.to(socket.id).emit('userJoin', response.generate(constants.SUCCESS_STATUS, {
                                                        roomName: roomName,
                                                        users: joineeDetails.users
                                                    }, "User enter in a room !"));
                                                }
                                                else{
                                                    logger.print("Room close while no opponent found after 3 sec");
                                                }

                                            } else
                                                logger.print("***GAME START ERROR ", err);
                                            io.sockets.to(socket.id).emit('error1', response.generate(constants.ERROR_STATUS, {message: err}));
                                        });
                                    }
                                    if (joineeDetails.users.length === 2) {
                                        callback();
                                        async.waterfall([
                                            gameStartMod({roomName: roomName}),
                                            //userStatusUpdateAfterGamerequest,
                                            userNextStart,
                                            //////////////
                                            gameTimer                                        
                                            
                                            
                                        ], function (err, result) {
                                            if (result) {

                                                io.sockets.to(socket.id).emit('userJoin', response.generate(constants.SUCCESS_STATUS, {
                                                    roomName: roomName,
                                                    users: joineeDetails.users
                                                }, "User enter in a room !"));
                                                logger.print("***Done  ", result);
                                                io.sockets.to(roomName).emit('gameStart', response.generate(constants.SUCCESS_STATUS, {
                                                    roomName: roomName,
                                                    users: joineeDetails.users
                                                }, "Game start !"));
                                            } else
                                                logger.print("***GAME START ERROR ", err);
                                            io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: err}));

                                        });
                                    }
                                    // Otherwise, send an error message back to the player.
                                    else {
                                        // io.sockets.to(roomName).emit('NoUser', response.generate(constants.ERROR_STATUS, {message: "Unable to found room"}));
                                        callback();
                                    }
                                    /*}).catch(err=>{
                                            io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:err}));
                                            callback();
                                        })*/

                                })/*.catch(err => {
                                    io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: err}));
                                    callback();
                                })*/

                            }).catch(err => {
                                io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: err}));
                                callback();
                            })
                        }).catch(err => {
                            io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: "Unable to create room"}));
                            callback();
                        })

                     }).catch(userStatusUpdateErr => {
                            logger.print("unable to update user online status");
                            io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: "Unable to update online status"}));
                            callback();
                        })


                    } else {
                        io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: "No socket Id found"}));
                        callback();
                    }
                }

            }).catch(err => {
                console.log("eror while fetch socket");
                io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                callback();
            })

        }
        } else {
            io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: "User not found"}));
            callback();
        }
    }

    function updateInRoom(req) {
        room.updateRoomDetails({roomName: req.roomName}, {status: "forfeit"}).then(function (roomStatusUpdate) {
            io.to(req.roomName).emit('gameClosed', response.generate(constants.ERROR_STATUS, {}, "game closed"));
            callback(null, req);
        })
    }

    function playerLeaveOld(req) {
        return function (callback) {
            inmRoom.userLeave({roomName: req.roomName, userId: req.userId}).then(function (updateDetails) {
                room.playerLeave({
                    roomName: req.roomName,
                    userId: req.userId
                }, updateDetails).then(function (playerUpdate) {
                    callback(null, req);
                })
            }).catch(err => {
                callback("", null);
            })
        }

    }

    function totalPlayerList(req, callback) {
        inmRoom.getRoomDetails({roomName: req.roomName}).then(function (roomDetails) {
            if (!roomDetails || roomDetails.users.length <= 1) {
                callback(null, req);
            } else {
                callback("err", null);
            }
        })
    }

    function roomClosed(req, callback) {
        console.log("roomClosed", req)
        room.updateRoomDetails({roomName: req.roomName}, {status: "closed"}).then(function (roomStatusUpdate) {
            io.to(req.roomName).emit('gameClosed', response.generate(constants.ERROR_STATUS, {}, "game closed"));
            callback(null, req);
        })
    }

    function playerLeave1(req) {
        return function (callback) {
            inmRoom.userLeave({roomName: req.roomName, userId: req.userId}).then(function (updateDetails) {
                room.playerLeave({roomName: req.roomName, userId: req.userId}).then(function (playerUpdate) {
                    callback(null, req);
                })
            }).catch(err => {
                callback("", null);
            })
        }

    }

    function playerLeave(req) {
        return function (callback) {

            inmRoom.userLeave({roomName: req.roomName, userId: req.userId}).then(function (updateDetails) {
                //room.playerLeave({roomName: req.roomName, userId: req.userId}).then(function (playerUpdate) {
                    callback(null, updateDetails);
                //})
            }).catch(err => {
                callback("", null);
            })
        }

    }


    function playerLeaveMod(req) {
        return function (callback) {
            inmRoom.userLeaveNew({roomName: req.roomName, userId: req.userId}).then(function (updateDetails) {
                //room.playerLeave({roomName: req.roomName, userId: req.userId}).then(function (playerUpdate) {
                callback(null, updateDetails);
                //})
            }).catch(err => {
                callback("", null);
            })
        }

    }


    function memoryRoomRemove(req, callback) {
        inmRoom.removeRoom({roomName: req.roomName}).then(function (roomupdate) {
            callback(null, req)
        });
    }

    function RoomUpdate(req, callback) {
        room.updateRoomLeaveDisconnect({roomName: req.roomName,gameTotalTime:req.gameTotalTime,userTotal:req.roomUsers}).then(function (roomupdate) {
            callback(null, req)
        });
    }
    //user status update
    function userStatusUpdate(req, callback){

        user.userStatusUpdate({userId:req.userId,userStatus:0}).then(function(statusUpdate){
            callback(null, req)
        }).catch(err => {
            callback("", null);
        })
    }

    function userStatusUpdateAfterGamerequest(req, callback){

        user.userStatusUpdate(req.userId,1).then(function(statusUpdate){
            callback(null, req)
        }).catch(err => {
            callback("", null);
        })
    }
    /**
     * @desc This function is used for color request
     * @param {String} accesstoken
     * @param {String} sageName
     * @param {String} colorName
     */

    socket.on('colorRequest', function (req) {
        user.colorRequest({userId: req.userId}, {"colorName": req.color})
            .then(colorUpdate => {
                return user.sageRequest({userId: req.userId}, {"raceName": req.race}
                );
                //io.to(req.roomName).emit('colorRequest',response.generate(constants.SUCCESS_STATUS,{ },colorUpdate));
            })
            .then(raceUpdate => {
                return user.nameRequest({userId: req.userId}, {"dartName": req.dartName}
                );
                //io.to(req.roomName).emit('colorRequest',response.generate(constants.SUCCESS_STATUS,{ },colorUpdate));
            })
            .then(resp => {
                io.sockets.to(socket.id).emit('colorGet', response.generate(constants.SUCCESS_STATUS, {}, "Color captured"));
            })
            .catch(err => {
                console.log(err);
                io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {"message": err}));
            });
    });

    /**
     * @desc This function is used for while user leave the game
     * @param {String} accesstoken
     * @param {String} roomName
     */
    socket.on('leave1', function (req) {
        if (req.userId && req.roomName) {
            let currentSocketId = socket.id;
            var findIndex = allOnlineUsers.findIndex(function (elemt) {
                return elemt.socketId == currentSocketId
            });
            if (io.sockets.sockets[currentSocketId] != undefined)
                io.sockets.sockets[currentSocketId].leave(req.roomName);
            io.to(req.roomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: req.userId}, "Player leave from room"));

            //inmRoom.userLeaveMod({roomName:req.roomName ,userId : req.userId})
            inmRoom.userLeave({roomName: req.roomName, userId: req.userId})
                .then(updateDetails => {
                    return inmRoom.updateInmemoryRoomLeave(req, updateDetails
                    );
                })
                .then(roomUpdate => {
                    return room.updateRoomLeave(req, roomUpdate
                    );
                })
                .then(resp => {
                    logger.print("Room closed");
                    allOnlineUsers.splice(findIndex, 1);
                    io.sockets.to(req.roomName).emit('userLeave', response.generate(constants.SUCCESS_STATUS, {
                        roomName: req.roomName,
                        userName: req.userName,
                        userId: req.userId
                    }, "User Left !"));
                })
                .catch(err => {
                    logger.print("***GAME ERROR ", err);
                    io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: err}));
                });
        }
    })

    /**
     * @desc winner declare
     * @param {String}roomName
     * @param {array} user
     */

    //WINNER DECLARE
    function winnerDeclare(req) {
        return new Promise((resolve, reject) => {
            user.updatePointDetails({_id: req.userId}, {
                total_no_win: 1

            }).then(function (updateWinningDetails) {
                //users.rank = users.totalUserKill
               /* io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                    userId: req.userId,
                    roomName: req.roomName
                }, "Game is over"));*/
               let userArr=[];
               let obj={userId:req.userId};
               userArr.push(obj);
                resolve(userArr)
                //resolve(req.userId)
                //callback(null, req);
            }).catch(err => {
                reject(err);
                //io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: err}));
            });
        })
    }

    /**
     * @desc This function is used for while user disconnected
     * @param {String} accesstoken
     */


    socket.on('disconnect2', function (req) {
        let currentSocketId = socket.id;
        let findIndex = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId == currentSocketId
        });
        let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId != currentSocketId
        });
        if (findIndex != -1) {
            userRoomName = allOnlineUsers[findIndex].roomName;
            if (userRoomName != '') {
                io.to(userRoomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: allOnlineUsers[findIndex].userId}, "Player leave from room"));
                const  disconnectInterval = setInterval(() => {
                    const properID = CheckReload();
                    console.log(properID);
                    if (properID < 5) {
                        clearInterval(disconnectInterval);
                        async.waterfall([
                                drawResult(userRoomName),
                                updateRoom,
                                RoomUpdate
                            ],
                            function (err, disconnectresult) {

                                if(disconnectresult){
                                clearInterval(disconnectInterval);
                                io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                    userId: disconnectresult,
                                    roomName: req.roomName
                                }, "Game is over"));
                                return;

                             }
                                else{
                                    logger.print("***DISCONNECT ERROR ", err);
                                    io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                                }
                            });
                    }

                    else{
                        clearInterval(disconnectInterval);
                        async.waterfall([
                            playerLeave({roomName: userRoomName, userId: allOnlineUsers[findIndex].userId}),
                            updateRoom,
                            RoomUpdate
                            // totalPlayerList,
                            // roomClosed,
                            // memoryRoomRemove
                        ], function (err, result) {
                            //allOnlineUsers.splice(findIndex, 1);
                            if (result) {

                                if (findIndexOpponent != -1) {
                                    winnerDeclare({
                                        userId: allOnlineUsers[findIndexOpponent].userId,
                                        roomName: req.roomName
                                    }).then(function (roomDetails) {
                                        io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                            userId: roomDetails,
                                            roomName: req.roomName
                                        }, "Game is over"));
                                        logger.print("Room closed");
                                        return;
                                    });
                                }
                                logger.print("Room closed");
                                allOnlineUsers.splice(findIndex, 1);

                            } else
                                logger.print("***DISCONNECT ERROR ", err);
                                 io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                        });

                    }


                }, 1000);


            }
        }
    });
    socket.on('disconnecting', function (req) {
        let currentSocketId = socket.id;
        logger.print("disconnecting 12222 socket id while disconnecting"+ " " +currentSocketId)
    });

    ///////new code//////////////////////////////

    function gameStatusUpdateDisconnect(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin==1) {
            //if (reqobj.isWin) {
                //user update with coin
                //console.log("pre"+reqobj.opponentCup);
                user.updateUserCoin({userId: reqobj.opponentUserId},
                    {startCoin: reqobj.opponentCoin,
                        cupNo:reqobj.cupNumber,
                        userScore:reqobj.totalGameScores
                    }).then(function (userStatusUpdate) {
                    callback(null, reqobj);
                });
                /* user.updateUserCoin({userId: reqobj.opponentUserId},
                 {startCoin: reqobj.opponentCoin,cupNo:reqobj.cupNumber}).then(function (userStatusUpdate) {
                     callback(null, reqobj);
                });*/
            } else {
                callback(null, reqobj);
            }
        })
    }

    function gameStatusUpdateOpponentDisconnect(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin==1) {
            //if (reqobj.isWin) {
                console.log("12"+reqobj.opponentCup);
                //user update with coin
                //reqobj.roomUsers
                //let findIndex = reqobj.roomUsers.findIndex(elemt => (elemt.userId!=reqobj.userId));
                //let userOppo=reqobj.roomUsers[findIndex].userId;
                //console.log("opponent user"+userOppo);

                user.updateUserCoinOpponent({userId: reqobj.userId},
                    {startCoin: reqobj.opponentCoin,
                        cupNo:reqobj.opponentCup,
                        userScore:reqobj.gameScoreOpponent
                    }).then(function (userStatusUpdate) {
                    callback(null, reqobj);
                });
                
            } else {
                callback(null, reqobj);
            }
        })
    }
    //////////////////////////////////////////////////


    socket.on('disconnectV113', function (req) {
        let currentSocketId = socket.id;
        logger.print("socket id while disconnecting"+ " " +currentSocketId);

        /*var findIndex = allOnlineUsers.findIndex(function (elemt) {
            return elemt.userId == req.userId
        });
        let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
            return elemt.userId != req.userId
        });*/

        let findIndex = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId == currentSocketId
        });
        let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId != currentSocketId
        });
         console.log(findIndex);
        if (findIndex != -1) {
            let userRoomName = allOnlineUsers[findIndex].roomName;
            if (userRoomName != '') {
                logger.print("room name while disconnecting"+ userRoomName);
                io.to(userRoomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: allOnlineUsers[findIndex].userId}, "Player leave from room"));
                

                //////////////////////////////////////////////////

                   RoomDb.findOne({name:userRoomName},
                    {_id: 1,game_time:1, name:1}).then(gameresponses=> {

                        if(gameresponses.game_time >0){
                          //game finished//////////////
                          console.log("game finished");
                          if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';
                        }
                        else{

                      async.waterfall([
                    playerLeave({roomName: userRoomName, userId: allOnlineUsers[findIndex].userId}),
                    updateRoom,
                    RoomUpdate,
                    //new add
                    gameStatusUpdateDisconnect,
                    gameStatusUpdateOpponentDisconnect,
                    //new/////
                    userStatusUpdate
                    // totalPlayerList,
                    // roomClosed,
                    // memoryRoomRemove
                ], function (err, result) {
                    //allOnlineUsers.splice(findIndex, 1);
                    if (result) {
                        //allOnlineUsers=_.without(allOnlineUsers, _.findWhere(allOnlineUsers, {userId: req.userId}));
                        logger.print("win status after disconnecting"+result.isWin);


                        if (findIndexOpponent != -1 && result.isWin == 1) {
                            logger.print("opponent exists");
                            winnerDeclare({
                                userId: result.opponentUserId,
                                //userId: allOnlineUsers[findIndexOpponent].userId,
                                roomName: userRoomName
                            }).then(function (roomDetails) {
                                allOnlineUsers.splice(findIndex, 1);
                                //allOnlineUsers[findIndex].roomName='';
                                if(findIndex==1)
                                   allOnlineUsers[findIndexOpponent].roomName='';
                                else
                                    allOnlineUsers[findIndex].roomName='';

                                io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                    //userId: result.userId,
                                    firstUserId: result.userId,
                                    firstUserGameStatus: "Lose",
                                    secondUserId:result.opponentUserId,
                                    secondUserGameStatus: "Win",
                                    //userId: roomDetails,
                                    roomName: userRoomName,

                                    firstUserCupNumber:result.cupNumber,
                                    secondUserCupNumber:result.opponentCup

                                    //gameStatus: "Lose"
                                    //gameStatus: "Win"
                                }, "Game is over"));
                                logger.print("Room closed");
                            });
                        } else if (findIndexOpponent != -1 && result.isWin == 2) {
                            logger.print("opponent exists");
                            allOnlineUsers.splice(findIndex, 1);
                            //allOnlineUsers[findIndex].roomName='';
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';

                            io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "Draw",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "Draw",
                                //userId: result.roomUsers,
                                roomName: userRoomName,

                                firstUserCupNumber:result.cupNumber,
                                secondUserCupNumber:result.opponentCup
                               // gameStatus: "Draw"
                            }, "Game is over"));
                            logger.print("Room closed");
                        } else {
                            logger.print("opponent not exists");
                           // allOnlineUsers.splice(findIndex, 1);
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';
                            io.sockets.to(socket.id).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "",
                                //userId: result.roomUsers,
                                roomName: userRoomName,

                                firstUserCupNumber:result.cupNumber,
                                secondUserCupNumber:result.opponentCup
                                //gameStatus: ""
                            }, "Game is over"));
                            /*io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                userId: result.roomUsers,
                                roomName: userRoomName,
                                gameStatus: ""
                            }, "Game is over"));*/
                            logger.print("Room closed");
                        }
                        logger.print("Room closed");



                    } else{
                        logger.print("***DISCONNECT ERROR ", err);
                        allOnlineUsers.splice(findIndex, 1);
                        //allOnlineUsers[findIndex].roomName='';
                        if(findIndex==1)
                            allOnlineUsers[findIndexOpponent].roomName='';
                        else
                            allOnlineUsers[findIndex].roomName='';
                        io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                   }
                });
                           
                        }

                      }).catch(err => {
                        reject(err);
                      });
                 
                //////////////////////////////////////////////////
               

            }
            else{
                logger.print("Room not found");
                allOnlineUsers.splice(findIndex, 1);
              }
        }
        else{
            logger.print("no socket id found");
        }
    });

    socket.on('disconnect-run', function (req) {
        let currentSocketId = socket.id;
        logger.print("socket id while disconnecting"+ " " +currentSocketId);

        /*var findIndex = allOnlineUsers.findIndex(function (elemt) {
            return elemt.userId == req.userId
        });
        let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
            return elemt.userId != req.userId
        });*/

        let findIndex = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId == currentSocketId
        });
        let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId != currentSocketId
        });
         console.log(findIndex);
        if (findIndex != -1) {
            let userRoomName = allOnlineUsers[findIndex].roomName;
            if (userRoomName != '') {
                logger.print("room name while disconnecting"+ userRoomName);
                io.to(userRoomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: allOnlineUsers[findIndex].userId}, "Player leave from room"));
                

                //////////////////////////////////////////////////

                 
                //////////////////////////////////////////////////
                async.waterfall([
                    playerLeave({roomName: userRoomName, userId: allOnlineUsers[findIndex].userId}),
                    updateRoom,
                    RoomUpdate,
                    //new add
                    gameStatusUpdateDisconnect,
                    gameStatusUpdateOpponentDisconnect,
                    //new/////
                    userStatusUpdate
                    // totalPlayerList,
                    // roomClosed,
                    // memoryRoomRemove
                ], function (err, result) {
                    //allOnlineUsers.splice(findIndex, 1);
                    if (result) {
                        //allOnlineUsers=_.without(allOnlineUsers, _.findWhere(allOnlineUsers, {userId: req.userId}));
                        logger.print("win status after disconnecting"+result.isWin);


                        if (findIndexOpponent != -1 && result.isWin == 1) {
                            logger.print("opponent exists");
                            winnerDeclare({
                                userId: result.opponentUserId,
                                //userId: allOnlineUsers[findIndexOpponent].userId,
                                roomName: userRoomName
                            }).then(function (roomDetails) {
                                allOnlineUsers.splice(findIndex, 1);
                                //allOnlineUsers[findIndex].roomName='';
                                if(findIndex==1)
                                   allOnlineUsers[findIndexOpponent].roomName='';
                                else
                                    allOnlineUsers[findIndex].roomName='';

                                io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                    //userId: result.userId,
                                    firstUserId: result.userId,
                                    firstUserGameStatus: "Lose",
                                    secondUserId:result.opponentUserId,
                                    secondUserGameStatus: "Win",
                                    //userId: roomDetails,
                                    roomName: userRoomName,
                                    //gameStatus: "Lose"
                                    //gameStatus: "Win"
                                }, "Game is over"));
                                logger.print("Room closed");
                            });
                        } else if (findIndexOpponent != -1 && result.isWin == 2) {
                            logger.print("opponent exists");
                            allOnlineUsers.splice(findIndex, 1);
                            //allOnlineUsers[findIndex].roomName='';
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';

                            io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "Draw",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "Draw",
                                //userId: result.roomUsers,
                                roomName: userRoomName,
                               // gameStatus: "Draw"
                            }, "Game is over"));
                            logger.print("Room closed");
                        } else {
                            logger.print("opponent not exists");
                           // allOnlineUsers.splice(findIndex, 1);
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';
                            io.sockets.to(socket.id).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "",
                                //userId: result.roomUsers,
                                roomName: userRoomName,
                                //gameStatus: ""
                            }, "Game is over"));
                            /*io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                userId: result.roomUsers,
                                roomName: userRoomName,
                                gameStatus: ""
                            }, "Game is over"));*/
                            logger.print("Room closed");
                        }
                        logger.print("Room closed");



                    } else{
                        logger.print("***DISCONNECT ERROR ", err);
                        allOnlineUsers.splice(findIndex, 1);
                        //allOnlineUsers[findIndex].roomName='';
                        if(findIndex==1)
                            allOnlineUsers[findIndexOpponent].roomName='';
                        else
                            allOnlineUsers[findIndex].roomName='';
                        io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                   }
                });

            }
            else{
                logger.print("Room not found");
                allOnlineUsers.splice(findIndex, 1);
              }
        }
        else{
            logger.print("no socket id found");
        }
    });

 
    socket.on('leave', function (req) {
        console.log("leave called");
        if (req.userId && req.roomName) {
            let currentSocketId = socket.id;

            var findIndex = allOnlineUsers.findIndex(function (elemt) {
                return elemt.userId == req.userId
            });
            let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
                return elemt.userId != req.userId
            });

            /*var findIndex = allOnlineUsers.findIndex(function (elemt) {
                return elemt.socketId == currentSocketId
            });
            let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
                return elemt.socketId != currentSocketId
            });*/
            //if (io.sockets.sockets[currentSocketId] != undefined)
               // io.sockets.sockets[currentSocketId].leave(req.roomName);
            //io.to(req.roomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: req.userId}, "Player leave from room"));
            //if (allOnlineUsers[findIndexOpponent]){
            if (findIndex != -1) {
            async.waterfall([
                playerLeaveMod({roomName: req.roomName, userId: req.userId}),
                updateRoom,
                RoomUpdate,
                gameStatusUpdateLeaveMod,
                gameStatusUpdateOpponentLeaveMod  

            ], function (err, result) {
                if (result) {
                    console.log("new field"+result.opponentCup);
                    console.log("opponent coin"+result.opponentCoin);
                    console.log("user coin"+result.availableCoin);
                    console.log("result.cupNumber"+result.cupNumber);
                    console.log("result.opponentCup"+result.opponentCup);
                    //console.log("opponent cup"+result.opponentCup);
                    
                    user.findDetailsGame({_id:result.userId}).then((firstUserTotalCup)=>{
                        console.log("firstUserTotalCup"+firstUserTotalCup.cupNo);    
                         
                    user.findDetailsGame({_id:result.opponentUserId}).then((secondUserTotalCup)=>{

                    if (findIndexOpponent != -1 && result.isWin == 1) {
                        winnerDeclare({
                            //useropponentId: result.opponentUserId,
                            userId: result.opponentUserId,
                            roomName: req.roomName
                        }).then(function (roomDetails) {
                            console.log("splice");
                            console.log(allOnlineUsers.findIndex.userId);
                            console.log(allOnlineUsers.findIndex.roomName);
                            allOnlineUsers[findIndex].roomName='';
                            allOnlineUsers[findIndexOpponent].roomName='';
                            console.log("after splice"+allOnlineUsers.findIndex.roomName);
                            io.sockets.sockets[currentSocketId].leave(req.roomName);
                            io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                firstUserId: result.userId,
                                firstUserGameStatus: "Lose",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "Win",
                               // userId: roomDetails,
                                roomName: req.roomName,

                                //gameStatus: "Win"
                                firstUserCupNumber: result.cupNumber,
                                secondUserCupNumber: result.opponentCup,

                                firstUserCoinNumber: result.availableCoin,
                                secondUserCoinNumber: result.opponentCoin,

                                completeStatus:0,

                                firstUserTotalCup: firstUserTotalCup.cupNo,
                                secondUserTotalCup: secondUserTotalCup.cupNo
                            }, "Game is over"));
                            logger.print("Room closed");
                        });
                    } else if (findIndexOpponent != -1 && result.isWin == 2) {
                        console.log("splice"+allOnlineUsers);
                        console.log(allOnlineUsers.findIndex.userId);
                        console.log(allOnlineUsers.findIndex.roomName);
                        allOnlineUsers[findIndex].roomName='';
                        allOnlineUsers[findIndexOpponent].roomName='';
                        console.log("after splice"+allOnlineUsers.findIndex.roomName);
                        io.sockets.sockets[currentSocketId].leave(req.roomName);
                        io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                            firstUserId: result.userId,
                            firstUserGameStatus: "Draw",
                            secondUserId:result.opponentUserId,
                            secondUserGameStatus: "Draw",
                            //userId: result.roomUsers,
                            roomName: req.roomName,
                            firstUserCupNumber: result.cupNumber,
                            secondUserCupNumber: result.opponentCup,

                            firstUserCoinNumber: result.availableCoin,
                            secondUserCoinNumber: result.opponentCoin,

                            completeStatus:0,

                            firstUserTotalCup: firstUserTotalCup.cupNo,
                            secondUserTotalCup: secondUserTotalCup.cupNo
                            //gameStatus: "Draw"
                        }, "Game is over"));
                        logger.print("Room closed");
                    } else {
                        //console.log("splice"+allOnlineUsers);
                        console.log(allOnlineUsers.findIndex.userId);
                        console.log(allOnlineUsers.findIndex.roomName);
                        allOnlineUsers[findIndex].roomName='';
                        allOnlineUsers[findIndexOpponent].roomName='';
                        console.log("after splice"+allOnlineUsers.findIndex.roomName);
                        io.sockets.sockets[currentSocketId].leave(req.roomName);
                        io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                            firstUserId: result.userId,
                            firstUserGameStatus: "",
                            secondUserId:result.opponentUserId,
                            secondUserGameStatus: "",
                            //userId: result.roomUsers,
                            roomName: req.roomName,
                            firstUserCupNumber: '',
                            secondUserCupNumber: '',

                            firstUserCoinNumber: '',
                            secondUserCoinNumber: '',

                            completeStatus:0,

                            firstUserTotalCup: firstUserTotalCup.cupNo,
                            secondUserTotalCup: secondUserTotalCup.cupNo
                            //gameStatus: ""
                        }, "Game is over"));
                        logger.print("Room closed");
                    }

                    logger.print("Room closed");


                    //////if clause////

                       }).catch(secondUserTotalCupErr=>{
                        console.log("secondUserTotalCupErr"+secondUserTotalCupErr);
                       })

                       }).catch(firstUserTotalCupErr=>{
                        console.log("firstUserTotalCupErr"+firstUserTotalCupErr);
                      })

                } else {
                    console.log("splice"+allOnlineUsers);
                    console.log(allOnlineUsers.findIndex.userId);
                    console.log(allOnlineUsers.findIndex.roomName);
                    allOnlineUsers[findIndex].roomName='';
                    allOnlineUsers[findIndexOpponent].roomName='';
                    io.sockets.sockets[currentSocketId].leave(req.roomName);
                   // allOnlineUsers.splice(findIndex, 1);
                    //allOnlineUsers.splice(findIndex, 2);
                    console.log("after splice"+allOnlineUsers.findIndex.roomName);
                    logger.print("***LEAVE ERROR ", err);
                }
            });
        }
            else{
                logger.print("no user index found in leave");
            }
            //}


        }
    })

    socket.on('leave_run', function (req) {
        console.log("leave called");
        if (req.userId && req.roomName) {
            let currentSocketId = socket.id;

            var findIndex = allOnlineUsers.findIndex(function (elemt) {
                return elemt.userId == req.userId
            });
            let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
                return elemt.userId != req.userId
            });

            /*var findIndex = allOnlineUsers.findIndex(function (elemt) {
                return elemt.socketId == currentSocketId
            });
            let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
                return elemt.socketId != currentSocketId
            });*/
            //if (io.sockets.sockets[currentSocketId] != undefined)
            // io.sockets.sockets[currentSocketId].leave(req.roomName);
            //io.to(req.roomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: req.userId}, "Player leave from room"));
            //if (allOnlineUsers[findIndexOpponent]){
            if (findIndex != -1) {
                async.waterfall([
                    playerLeaveMod({roomName: req.roomName, userId: req.userId}),
                    updateRoom,
                    RoomUpdate

                ], function (err, result) {
                    if (result) {
                        console.log("new field"+result.opponentCup);
                        console.log("opponent coin"+result.opponentCoin);
                        console.log("user coin"+result.userCoin);
                        //console.log("opponent cup"+result.opponentCup);


                        if (findIndexOpponent != -1 && result.isWin == 1) {
                            winnerDeclare({
                                //useropponentId: result.opponentUserId,
                                userId: result.opponentUserId,
                                roomName: req.roomName
                            }).then(function (roomDetails) {
                                console.log("splice");
                                console.log(allOnlineUsers.findIndex.userId);
                                console.log(allOnlineUsers.findIndex.roomName);
                                allOnlineUsers[findIndex].roomName='';
                                allOnlineUsers[findIndexOpponent].roomName='';
                                console.log("after splice"+allOnlineUsers.findIndex.roomName);
                                io.sockets.sockets[currentSocketId].leave(req.roomName);
                                io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                    userId: roomDetails,
                                    roomName: req.roomName,
                                    gameStatus: "Win"
                                }, "Game is over"));
                                logger.print("Room closed");
                            });
                        } else if (findIndexOpponent != -1 && result.isWin == 2) {
                            console.log("splice"+allOnlineUsers);
                            console.log(allOnlineUsers.findIndex.userId);
                            console.log(allOnlineUsers.findIndex.roomName);
                            allOnlineUsers[findIndex].roomName='';
                            allOnlineUsers[findIndexOpponent].roomName='';
                            console.log("after splice"+allOnlineUsers.findIndex.roomName);
                            io.sockets.sockets[currentSocketId].leave(req.roomName);
                            io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                userId: result.roomUsers,
                                roomName: req.roomName,
                                gameStatus: "Draw"
                            }, "Game is over"));
                            logger.print("Room closed");
                        } else {
                            //console.log("splice"+allOnlineUsers);
                            console.log(allOnlineUsers.findIndex.userId);
                            console.log(allOnlineUsers.findIndex.roomName);
                            allOnlineUsers[findIndex].roomName='';
                            allOnlineUsers[findIndexOpponent].roomName='';
                            console.log("after splice"+allOnlineUsers.findIndex.roomName);
                            io.sockets.sockets[currentSocketId].leave(req.roomName);
                            io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                userId: result.roomUsers,
                                roomName: req.roomName,
                                gameStatus: ""
                            }, "Game is over"));
                            logger.print("Room closed");
                        }

                        logger.print("Room closed");
                    } else {
                        console.log("splice"+allOnlineUsers);
                        console.log(allOnlineUsers.findIndex.userId);
                        console.log(allOnlineUsers.findIndex.roomName);
                        allOnlineUsers[findIndex].roomName='';
                        allOnlineUsers[findIndexOpponent].roomName='';
                        io.sockets.sockets[currentSocketId].leave(req.roomName);
                        // allOnlineUsers.splice(findIndex, 1);
                        //allOnlineUsers.splice(findIndex, 2);
                        console.log("after splice"+allOnlineUsers.findIndex.roomName);
                        logger.print("***LEAVE ERROR ", err);
                    }
                });
            }
            else{
                logger.print("no user index found in leave");
            }
            //}


        }
    })


    /**
     * @desc This function is used for color request coin cancel
     * @param {String} accesstoken
     * @param {String} coinNumber
     */

     socket.on('gameRequestCancel', function (req) {
        return new Promise((resolve, reject) => {
        room.updateRoomAfterWait({roomName: req.roomName}).then(responses => {
            logger.print("Coin canceled");
            io.sockets.to(socket.id).emit('CANCELED', response.generate(constants.ERROR_STATUS, {message: "Coin is canceled"}));
        }).catch(err => {
            logger.print("error while cancel coin");
            io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));

        });

     })

    });

    socket.onclose = function(reason){
        console.log("close event fired");
        console.log(socket.adapter.sids[socket.id]);
        Object.getPrototypeOf(this).onclose.call(this,reason);

        /* Delay of 1 seconds to remove from all rooms and disconnect the id */
        setTimeout(function() {
            if (!this.connected) return this;
            debug('closing socket - reason %s', reason);
            this.leaveAll();
            this.nsp.remove(this);
            this.client.remove(this);
            this.connected = false;
            this.disconnected = true;
            delete this.nsp.connected[this.id];
            this.emit('disconnect', reason);
        }, 60 * 1000);
    }

    //dart Timer ///////////////

    //dartTimer

    function DoThis(){

     // function DoThis actions 
     //note that timer is done.
     Timer_Started = false;

    }



      function dartTimer(reqobj, callback) {
        return new Promise((resolve, reject) => {
            //callback(null, reqobj);
            //console.log("reqobj.userTurnId"+reqobj.nextUserId);
            //console.log("timer start");


             //var darttimer;
        
        

           if(!Timer_Started){
              console.log(darttimer); 
              Timer_Started=true;
              dartArray.push(1);
              console.log("dartArraylength"+dartArray.length);
              clearTimeout(darttimer);
              console.log("timer running");

              ///////new/////////////////////
              let k=13;
              darttimer = setTimeout(function gameStartTimmer4(gameStartObj4) {
                console.log("lp"+darttimer);
                Timer_Started = false;
                if(k===13){
                   io.to(reqobj.roomName).emit('gameThrow', response.generate(constants.SUCCESS_STATUS, {
                        userId: reqobj.userId,
                        roomName: reqobj.roomName,
                        remainingScore: parseInt(reqobj.remainingScore),
                        dartPoint: reqobj.dartPoint,
                        playStatus: reqobj.playStatus,
                        playerScore: reqobj.playerScore,
                        cupNumber: reqobj.cupNumber,
                        hitScore:reqobj.hitScore,
                        scoreMultiplier:reqobj.scoreMultiplier,
                        roundScore:reqobj.roundScore
                    }, "Dart thrown"));
 
                }
                k--;
                if (k === 0) {
                     console.log("timer"+darttimer);

                      RoomDb.findOne({name:gameStartObj4.roomName}, {_id: 1,game_time:1, name:1}).then(gameresponses=> {

                        if(gameresponses.game_time >0){
                          //game finished//////////////
                          clearTimeout(this.interval);
                          Timer_Started=true;
                          callback(null, gameStartObj4);
                          console.log("game finished not dart timer listen");
                        }
                        else{
                            console.log("dartArray.length"+dartArray.length);
                            if(dartArray.length >0){
                              console.log("dart thrown no dart timer required");  
                              clearTimeout(this.interval);
                              Timer_Started=true;
                              callback(null, gameStartObj4);
                            }
                            else{
                                console.log("plo"+Timer_Started);

                                console.log("no dart thrown dart timer working");
                                clearTimeout(this.interval);
                                Timer_Started=true;
                                io.to(gameStartObj4.roomName).emit('dartTimer', 
                               response.generate(constants.SUCCESS_STATUS, {dartFinish:1},
                               "No dart thrown"));

                                callback(null, gameStartObj4);


                            }                           
                         
                        }

                      }).catch(err => {
                        reject(err);
                      });


                    
                } else {
                    //console.log("timer"+darttimer);
                    //if(dartArray.length == 0){

                    console.log("dart timer running"+k);
                    gameStartObj4.k = k
                    darttimer = setTimeout(gameStartTimmer4, 1000, gameStartObj4);
                    //}


                }
            }, 1000, reqobj);
              ///new ////////////////////////
           }else{
              console.log("The timer not running.");

              console.log("call");
          //clearTimeout(timer);
          let k=13;
          darttimer = setTimeout(function gameStartTimmer4(gameStartObj4) {
                console.log("lp"+darttimer);
                Timer_Started = false;

                 if(k===13){
                   io.to(reqobj.roomName).emit('gameThrow', response.generate(constants.SUCCESS_STATUS, {
                        userId: reqobj.userId,
                        roomName: reqobj.roomName,
                        remainingScore: parseInt(reqobj.remainingScore),
                        dartPoint: reqobj.dartPoint,
                        playStatus: reqobj.playStatus,
                        playerScore: reqobj.playerScore,
                        cupNumber: reqobj.cupNumber,
                        hitScore:reqobj.hitScore,
                        scoreMultiplier:reqobj.scoreMultiplier,
                        roundScore:reqobj.roundScore
                    }, "Dart thrown"));
 
                }
                k--;
                if (k === 0) {
                     console.log("timer"+darttimer);

                      RoomDb.findOne({name:gameStartObj4.roomName}, {_id: 1,game_time:1, name:1}).then(gameresponses=> {

                        if(gameresponses.game_time >0){
                          //game finished//////////////
                          clearTimeout(this.interval);
                          Timer_Started=true;
                          callback(null, gameStartObj4);
                          console.log("game finished not dart timer listen");
                        }
                        else{
                            console.log("dartArray.length"+dartArray.length);
                            if(dartArray.length >0){
                              console.log("dart thrown no dart timer required");  
                              clearTimeout(this.interval);
                              Timer_Started=true;
                              callback(null, gameStartObj4);
                            }
                            else{
                                console.log("plo"+Timer_Started);

                                console.log("no dart thrown dart timer working");
                                clearTimeout(this.interval);
                                Timer_Started=true;
                                io.to(gameStartObj4.roomName).emit('dartTimer', 
                               response.generate(constants.SUCCESS_STATUS, {dartFinish:1},
                               "No dart thrown"));

                                callback(null, gameStartObj4);


                            }                           
                         
                        }

                      }).catch(err => {
                        reject(err);
                      });


                    
                } else {
                    //console.log("timer"+darttimer);
                    //if(dartArray.length == 0){

                    console.log("dart timer running"+k);
                    gameStartObj4.k = k
                    darttimer = setTimeout(gameStartTimmer4, 1000, gameStartObj4);
                    //}


                }
            }, 1000, reqobj);
       // }

           }

         

        //startTimer();
        //..
        //var startTimer = function() {
            

            //let k=20;
            //let darttimer;
            //console.log("darttimer"+this.interval);

            

            

            /*darttimer = setTimeout(function gameStartTimmer4(gameStartObj4) {
                k--;
                if (k === 0) {
                     console.log("timer"+darttimer);

                      RoomDb.findOne({name:gameStartObj4.roomName}, {_id: 1,game_time:1, name:1}).then(gameresponses=> {

                        if(gameresponses.game_time >0){
                          
                          clearTimeout(this.interval);
                          callback(null, gameStartObj4);
                          console.log("game finished not dart timer listen");
                        }
                        else{
                            if(dartArray.length >0){
                              console.log("dart thrown no dart timer required");  
                              clearTimeout(this.interval);
                              callback(null, gameStartObj4);
                            }
                            else{
                                console.log("no dart thrown dart timer working");
                                clearTimeout(this.interval);
                                io.to(gameStartObj4.roomName).emit('dartTimer', 
                               response.generate(constants.SUCCESS_STATUS, {dartFinish:1},
                               "No dart thrown"));

                                callback(null, gameStartObj4);


                            }                           
                         
                        }

                      }).catch(err => {
                        reject(err);
                      });


                    
                } else {
                    

                    console.log("dart timer running"+k);
                    gameStartObj4.k = k
                    darttimer = setTimeout(gameStartTimmer4, 1000, gameStartObj4);
                    


                }
            }, 1000, reqobj);*/



            
        })
    }

    /////disconnect logic modified with timer///////////////
     socket.on('disconnect', function (req) {
        let currentSocketId = socket.id;
        logger.print("socket id while disconnecting"+ " " +currentSocketId);
             

        let findIndex = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId == currentSocketId
        });
        let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId != currentSocketId
        });
         console.log(findIndex);
        if (findIndex != -1) {
            let userRoomName = allOnlineUsers[findIndex].roomName;
            if (userRoomName != '') {
                logger.print("room name while disconnecting"+ userRoomName);
                //io.to(userRoomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: allOnlineUsers[findIndex].userId}, "Player leave from room"));
                
                //////////////////////////////////////////////////

                   RoomDb.findOne({name:userRoomName},
                    {_id: 1,game_time:1, name:1}).then(gameresponses=> {

                        if(gameresponses.game_time >0){
                          //game finished//////////////
                          console.log("game finished");
                          if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';
                        }
                        else{

                      let m = 5;

                     let timer8 = setTimeout(function gameStartTimmer8(gameStartObj8) {
                    //if(g===20){
                    if(m===5){ 
                         user.userStatusUpdate({userId:allOnlineUsers[findIndex].userId,userStatus:0}).then(function(statusUpdate){
                          
                          io.to(userRoomName).emit('temporaryDisconnect', response.generate(constants.SUCCESS_STATUS, {
                                    //userId: result.userId,
                                    userId: allOnlineUsers[findIndex].userId,
                                    userGameStatus: "User temporarily unavailable",                                    
                                    roomName: userRoomName                                    
                                }, "User temporarily unavailable"));

                         console.log("disconnect timer start");
                         console.log("user update responses"+statusUpdate);  
                       }).catch(err => {
                          console.log("user update error"+err);
                       }); 
                       
                    }

                    m--;                   

                    if (m === 0) {
                        //fetch user status///
                       user.checkOnlineOrNot({_id:allOnlineUsers[findIndex].userId,onlineStatus:1}).then((userOnlineStatusRes)=>{
                        console.log("user status check"+userOnlineStatusRes.length);
                        if(userOnlineStatusRes.length >0){
                            console.log("not required to disconnect..user reconnect");
                        }

                        else{
                            console.log("user not reconnect after 5 sec");

                        console.log("gameStartObj8"+gameStartObj8);
                        console.log("only 0 sec remaining in disconnect timer");
                        console.log(m);
                        var clientsInRoom = io.nsps['/'].adapter.rooms[gameStartObj8];
                        var numClients = clientsInRoom === undefined ? 0 : Object.keys(clientsInRoom.sockets).length;
                        console.log("numClients"+numClients);
                        ////normal winner declare/////
                        console.log("allOnlineUsers"+allOnlineUsers[findIndex].userId); 
                      async.waterfall([
                    playerLeave({roomName: gameStartObj8, userId: allOnlineUsers[findIndex].userId}),
                    updateRoom,
                    RoomUpdate,
                    //new add
                    gameStatusUpdateDisconnect,
                    gameStatusUpdateOpponentDisconnect,
                    //new/////
                    userStatusUpdate
                    // totalPlayerList,
                    // roomClosed,
                    // memoryRoomRemove
                ], function (err, result) {
                    //allOnlineUsers.splice(findIndex, 1);
                    if (result) {

                        //allOnlineUsers=_.without(allOnlineUsers, _.findWhere(allOnlineUsers, {userId: req.userId}));
                        logger.print("win status after disconnecting"+result.isWin);

                        user.findDetailsGame({_id:result.userId}).then((firstUserTotalCup)=>{
                        console.log("firstUserTotalCup"+firstUserTotalCup.cupNo);    
                         
                        user.findDetailsGame({_id:result.opponentUserId}).then((secondUserTotalCup)=>{
                        
                        console.log("secondUserTotalCup"+secondUserTotalCup.cupNo); 

                        if (findIndexOpponent != -1 && result.isWin == 1) {
                            logger.print("opponent exists");
                            winnerDeclare({
                                userId: result.opponentUserId,
                                //userId: allOnlineUsers[findIndexOpponent].userId,
                                roomName: userRoomName
                            }).then(function (roomDetails) {
                                allOnlineUsers.splice(findIndex, 1);
                                //allOnlineUsers[findIndex].roomName='';
                                if(findIndex==1)
                                   allOnlineUsers[findIndexOpponent].roomName='';
                                else
                                    allOnlineUsers[findIndex].roomName='';
                              
                                  io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                    //userId: result.userId,
                                    firstUserId: result.userId,
                                    firstUserGameStatus: "Lose",
                                    secondUserId:result.opponentUserId,
                                    secondUserGameStatus: "Win",
                                    //userId: roomDetails,
                                    roomName: userRoomName,

                                    firstUserCupNumber:result.cupNumber,
                                    secondUserCupNumber:result.opponentCup,

                                    firstUserCoinNumber: result.availableCoin,
                                    secondUserCoinNumber: result.opponentCoin,

                                    completeStatus:0,

                                    firstUserTotalCup: firstUserTotalCup.cupNo,
                                    secondUserTotalCup: secondUserTotalCup.cupNo
                                    //gameStatus: "Lose"
                                    //gameStatus: "Win"
                                }, "Game is over"));
                                logger.print("Room closed");
                            });
                        } else if (findIndexOpponent != -1 && result.isWin == 2) {
                            logger.print("opponent exists");
                            allOnlineUsers.splice(findIndex, 1);
                            //allOnlineUsers[findIndex].roomName='';
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';

                            io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "Draw",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "Draw",
                                //userId: result.roomUsers,
                                roomName: userRoomName,

                                firstUserCupNumber:result.cupNumber,
                                secondUserCupNumber:result.opponentCup,

                                firstUserCoinNumber: result.availableCoin,
                                secondUserCoinNumber: result.opponentCoin,

                                completeStatus:0,

                                firstUserTotalCup: firstUserTotalCup.cupNo,
                                secondUserTotalCup: secondUserTotalCup.cupNo
                               // gameStatus: "Draw"
                            }, "Game is over"));
                            logger.print("Room closed");
                        } else {
                            logger.print("opponent not exists");
                           // allOnlineUsers.splice(findIndex, 1);
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else {
                                allOnlineUsers[findIndex].roomName='';
                                io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "",
                                //userId: result.roomUsers,
                                roomName: userRoomName,

                                firstUserCupNumber:result.cupNumber,
                                secondUserCupNumber:result.opponentCup,

                                firstUserCoinNumber: result.availableCoin,
                                secondUserCoinNumber: result.opponentCoin,

                                completeStatus:0,

                                firstUserTotalCup: firstUserTotalCup.cupNo,
                                secondUserTotalCup: secondUserTotalCup.cupNo
                                //gameStatus: ""
                            }, "Game is over"));
                            /*io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                userId: result.roomUsers,
                                roomName: userRoomName,
                                gameStatus: ""
                            }, "Game is over"));*/
                            logger.print("Room closed");
                           //nes///
                          }
                        }
                        logger.print("Room closed");

                       /////

                       //if clause

                       }).catch(secondUserTotalCupErr=>{
                        console.log("secondUserTotalCupErr"+secondUserTotalCupErr);
                       })

                       }).catch(firstUserTotalCupErr=>{
                        console.log("firstUserTotalCupErr"+firstUserTotalCupErr);
                      })


                    } else{
                        logger.print("***DISCONNECT ERROR ", err);
                        allOnlineUsers.splice(findIndex, 1);
                        //allOnlineUsers[findIndex].roomName='';
                        if(findIndex==1)
                            allOnlineUsers[findIndexOpponent].roomName='';
                        else
                            allOnlineUsers[findIndex].roomName='';
                        io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                       }
                       });

                        /////normal winner declare////

                        }
                       }).catch(err=>{
                         console.log("error while fetch user sttaus"+err);
                       })                       

                        

                        }
                        else{
                            console.log("disconnect timer running");
                            console.log(m);
                            gameStartObj8.m = m;
                            timer8 = setTimeout(gameStartTimmer8, 1000, gameStartObj8);
                        }

                        }, 1000, userRoomName);


                       }

                      }).catch(err => {
                        reject(err);
                      });
                 
                //////////////////////////////////////////////////
               

            }
            else{
                logger.print("Room not found");
                allOnlineUsers.splice(findIndex, 1);
              }
        }
        else{
            logger.print("no socket id found");
        }
    });
    //////disconnect logic modified////////////////////////
      socket.on("reconnecting", function(req) {
        console.log("reconnecting");
     });

    //new event fired for rejoin///////
    socket.on('reJoin', function (req) {
       RoomDb.findOne({name:req.roomName},
         {_id: 1,game_time:1, name:1,users:1}).then(gameresponses=> { 
           
           if(gameresponses.game_time >0){
               //game finished//////////////
              console.log("game finished while rejoin");    
              io.sockets.to(socket.id).emit('rejoinFailure',
                         response.generate(constants.SUCCESS_STATUS, {                            
                            userLastDetails:gameresponses.users
                         }));          
             }
             else{
                console.log("game not finished and user rejoined");
                ////check if user exist in that room or not///////
                RoomlogDb.find({name: req.roomName,
                    userId: req.userId},
                    {_id: 1, name:1, userId:1, status:1,
                    total:1,score:1,isWin:1,turn:1,
                    dartPoint:1,cupNumber:1,roomCoin:1,
                    scoreMultiplier:1,hitScore:1

                   }).sort({"_id":-1}).limit(3).then(responses=> {



                  RoomlogDb.find({name: req.roomName,
                    userId: { $ne: req.userId }},
                    {_id: 1, name:1, userId:1, status:1,
                    total:1,score:1,isWin:1,turn:1,
                    dartPoint:1,cupNumber:1,roomCoin:1,
                    scoreMultiplier:1,hitScore:1

                   }).sort({"_id":-1}).limit(3).then(opponentResponses=> {

                   console.log("responseParams"+responses);
                    if(responses.length >0){
                        console.log("user valid in that room while rejoin");
                        //////////roomdetails///////////////////
                        RoomDb.findOne({name: req.roomName},
                        {_id: 1, turn_time:1,game_time_remain:1}).then(roomresponses=> { 

                        ////fetch current turn///
                        inmRoom.findNextUserDart({roomName: req.roomName}).then(function (roomDetails) {    

                        inmRoom.findNextRejoinMod({roomName: req.roomName}).then(function (turnres) {    
  

                         console.log("roomresponses in rejoin"+roomresponses.turn_time);   
                           
                         io.sockets.to(socket.id).emit('rejoinSuccess',
                         response.generate(constants.SUCCESS_STATUS, {
                            lastTurnTime:roomresponses.turn_time,
                            gameRemainingTime:roomresponses.game_time_remain,
                            //gameRemainingTime:g,
                            currentUserTurn:roomDetails.userId,
                            turnStatus:turnres.turnstat,
                            firstUserLastDetails:responses,
                            secondUserLastDetails:opponentResponses
                         }));


                         io.to(req.roomName).emit('opponentReconnect',
                         response.generate(constants.SUCCESS_STATUS, {
                            userId:req.userId                            
                         }, "Oponent rejoined"));

                        }).catch(err => {
                            console.log("error while rejoin");
                            
                         });
                        ///////


                         }).catch(err => {
                            console.log("error while rejoin");
                            
                         });


                         }).catch(err => {
                            console.log("error while rejoin");
                            
                         });

                        

                        //////////roomdetails//////////////////
                    }
                    else{
                        console.log("user not valid in that room");
                    }      
                  //return resolve(responses);

                  }).catch(err => {
                 console.log("error while rejoin");
                 });

               }).catch(err => {
                 console.log("error while rejoin");
              });

             }
          
          }).catch(err => {
              console.log("error while rejoin");
          });
    });
    //new event fired for rejoin///////  
    /////game start timer logic//////////////////////

    function gameTimer(reqobj, callback) {
        console.log("game timer start");
        waitingDartInterval[reqobj.roomName] = setTimeout(() => {
            gameTimer1(reqobj, 0)
        }, 1000);
        callback(null, reqobj);
    }

    function gameTimer1(roomObj) {
       // inmRoom.gameTimerStart({roomName: roomObj.roomName}).then(function (roomDetails) {
            //if (roomDetails) {
                //new code 26 th mar//
                 //let g = 20;
                 //let g = 20;
                 let g = 360;
                 //let g = 360;
                //logger.print("  ************  first turn loop start");
                let timer2 = setTimeout(function gameStartTimmer2(gameStartObj2) {
                    //if(g===20){
                    if(g===360){  
                        //game_time_remain//
                        let updateCreateTime=moment().format('MM/DD/YYYY HH:mm:ss');
                        //update memory room with create time/////
                        console.log("updateCreateTime"+updateCreateTime);
                        roomDatastore.update({roomName: roomObj.roomName},
                         {$set: {createtime:updateCreateTime}},
                          //room.update({roomName : userObj.roomName},{ $set: { users: updateArr.finalArr.users }},
                          function (err, updateroomresult) {

                             console.log("game time start");
                             io.to(roomObj.roomName).emit('gameTimer',
                              response.generate(constants.SUCCESS_STATUS, {totalGameTime: 360}, "Your game time start"));
                           }); 
                      
                       //console.log("game time start");
                      //io.to(roomObj.roomName).emit('gameTimer',
                      //response.generate(constants.SUCCESS_STATUS, {totalGameTime: 360}, "Your game time start"));


                    }

                    g--;

                    if(g===10){
                      console.log(g);
                      
                      console.log("only 10 sec remaining"); 

                      RoomDb.findOne({name:gameStartObj2.roomName}, {_id: 1,game_time:1, name:1}).then(gameresponses=> {
                        console.log("gameresponses.game_time"+gameresponses.game_time);
                        if(gameresponses.game_time >0){
                          //game finished//////////////
                          console.log("game finished not required 10 sec listen");
                        }
                        else{
                         room.updateRoomDetails({roomName: roomObj.roomName}, {game_time_remain: g}).then(function (gameremainOver) {


                           io.to(gameStartObj2.roomName).emit('gameTimer', 
                           response.generate(constants.SUCCESS_STATUS, {gameFinish:1},
                          "Only 10 seconds remaining"));

                         gameStartObj2.g = g;
                         timer2 = setTimeout(gameStartTimmer2, 1000, gameStartObj2); 
                          ////

                          }).catch(err => {
                             logger.print("***Room update error ", err);
                         })


                        }

                      }).catch(err => {
                        console.log("error"+err);
                        reject(err);
                      });

                       

                    }

                    else if (g === 0) {
                        console.log("only 0 sec remaining");
                        console.log(g);
                        //game winner loser clculation///////////////
                        inmRoom.winAfterTimerEnd(gameStartObj2.roomName).then(function(roomDetails){
                            console.log("game calculation done");
                            console.log("roomDetails.userCoin"+roomDetails.userCoin);
                            if(roomDetails.isWin==1){
                            //console.log("gameres"+roomDetails.gameres.length);
                            inmRoom.updateInmemoryRoomMod12(roomDetails).then(function(roomDetails1){
                              console.log("roomdetails",roomDetails1)
                              let roomDetailsAll=roomDetails1;
                              //if (roomDetails1.isWin) {
                                //user update with coin
                                 user.updateUserCoin({userId: roomDetails1.userId},
                                  {startCoin: roomDetails1.availableCoin,
                                    cupNo:roomDetails1.cupNumber,
                                    userScore:roomDetails1.totalGameScores
                                }).then(function (userStatusUpdate) {
                                     console.log("user update"); 
                                     //opponent user update ///////
                                     let findIndex = roomDetails1.roomUsers.findIndex(elemt => (elemt.userId!=roomDetails1.userId));
                                     let userOppo=roomDetails1.roomUsers[findIndex].userId;
                                      console.log("opponent user"+userOppo);
                                     user.updateUserCoinOpponent({userId: userOppo},
                                      {startCoin: roomDetails1.availableCoin,
                                        cupNo:roomDetails1.opponentCup,
                                        userScore:roomDetails1.gameScoreOpponent
                                    }).then(function (userStatusUpdateOpponent) {
                                        console.log("opponent user update");
                                        //game over call ///////
                                        room.updateRoomGameOver({roomName: roomDetails1.roomName,gameTotalTime:roomDetails1.gameTotalTime}, {userObj: roomDetails1.roomUsers}).then(function (gameOver) {
                                            console.log("update room db");
                                            Notification.createNotification({
                                                //sent_by_user     : req.user_id ,
                                                received_by_user : roomDetails1.userId,
                                                subject          : "You are winner",
                                                message          : "You are winner",
                                                read_unread      : 0
                                            }).then(function(notificationdetails){
                                                logger.print("***Game Notification added ");
                                                logger.print("***Game over successfully ");

                                                user.findDetailsGame({_id:roomDetails1.userId}).then((firstUserTotalCup)=>{
                                                console.log("firstUserTotalCup"+firstUserTotalCup.cupNo);    
                                                 
                                                user.findDetailsGame({_id:roomDetails1.opponentUserId}).then((secondUserTotalCup)=>{


                                               // io.sockets.to(socket.id).emit('gameWin',response.generate( constants.SUCCESS_STATUS,{},"You won the match!"));
                                                io.to(roomDetails1.roomName).emit('gameOver', 
                                                    response.generate(constants.SUCCESS_STATUS, {
                                                   // userId: reqobj.userId,
                                                    firstUserId: roomDetails1.userId,
                                                    firstUserGameStatus: "Win",
                                                    secondUserId:roomDetails1.opponentUserId,
                                                    secondUserGameStatus: "Lose",
                                                    roomName: roomDetails1.roomName,

                                                    firstUserCupNumber: roomDetails1.cupNumber,
                                                    secondUserCupNumber: roomDetails1.opponentCup,

                                                    firstUserCoinNumber: roomDetails1.availableCoin,
                                                    secondUserCoinNumber: roomDetails1.opponentCoin,

                                                    completeStatus:1,

                                                    firstUserTotalCup: firstUserTotalCup.cupNo,
                                                    secondUserTotalCup: secondUserTotalCup.cupNo


                                                    //gameStatus:"Win"
                                                }, "Game is over"));
                                                clearTimeout(this.interval); 

                                                //////////
                                                  }).catch(secondUserTotalCupErr=>{
                                                    console.log("secondUserTotalCupErr"+secondUserTotalCupErr);
                                                   })

                                                   }).catch(firstUserTotalCupErr=>{
                                                    console.log("firstUserTotalCupErr"+firstUserTotalCupErr);
                                                  })


                                                //////////
                                            });

                                            }).catch(err => {
                                                logger.print("***Room update error ", err);
                                            })
                                        //clearTimeout(this.interval); 
                                     });
                                       
                                     
                                });
                             /* } else {
                                logger.print("***No calculation as game already finish ", err);
                                
                             }*/
                               
                            }).catch(err=>{
                                //callback("err", null);
                            })

                        }

                            else if(roomDetails.isWin==2){
                            //console.log("gameres"+roomDetails.gameres.length);
                            inmRoom.updateInmemoryRoomMod12(roomDetails).then(function(roomDetails1){
                              console.log("roomdetails",roomDetails1)
                              let roomDetailsAll=roomDetails1;
                              //if (roomDetails1.isWin) {
                                //user update with coin
                                 /*user.updateUserCoin({userId: roomDetails1.userId},
                                  {startCoin: roomDetails1.availableCoin,
                                    cupNo:roomDetails1.cupNumber,
                                    userScore:roomDetails1.totalGameScores
                                }).then(function (userStatusUpdate) {*/
                                     console.log("user update"); 
                                     //opponent user update ///////
                                     let findIndex = roomDetails1.roomUsers.findIndex(elemt => (elemt.userId!=roomDetails1.userId));
                                     let userOppo=roomDetails1.roomUsers[findIndex].userId;
                                      console.log("opponent user"+userOppo);
                                      /*user.updateUserCoin(
                                        {userId: userOppo}, 
                                        {startCoin: roomDetails1.availableCoin,
                                            cupNo:roomDetails1.opponentCup,
                                            userScore:roomDetails1.gameScoreOpponent
                                        }).then(function (userStatusUpdateOpponent) {*/
                                        console.log("opponent user update");
                                        //game over call ///////
                                        room.updateRoomGameOver({roomName: roomDetails1.roomName,gameTotalTime:roomDetails1.gameTotalTime}, {userObj: roomDetails1.roomUsers}).then(function (gameOver) {
                                            console.log("update room db");
                                              logger.print("***Game draw added ");
                                                logger.print("***Game draw successfully ");

                                                 

                                                 user.findDetailsGame({_id:roomDetails1.userId}).then((firstUserTotalCup)=>{
                                                  console.log("firstUserTotalCup"+firstUserTotalCup.cupNo);    
                         
                                                 user.findDetailsGame({_id:roomDetails1.opponentUserId}).then((secondUserTotalCup)=>{
                                               // io.sockets.to(socket.id).emit('gameWin',response.generate( constants.SUCCESS_STATUS,{},"You won the match!"));
                                                io.to(roomDetails1.roomName).emit('gameOver', 
                                                    response.generate(constants.SUCCESS_STATUS, {
                                                   // userId: reqobj.userId,
                                                    firstUserId: roomDetails1.userId,
                                                    firstUserGameStatus: "Draw",
                                                    secondUserId:roomDetails1.opponentUserId,
                                                    secondUserGameStatus: "Draw",
                                                    roomName: roomDetails1.roomName,

                                                    firstUserCupNumber: roomDetails1.cupNumber,
                                                    secondUserCupNumber: roomDetails1.opponentCup,

                                                    firstUserCoinNumber: roomDetails1.availableCoin,
                                                    secondUserCoinNumber: roomDetails1.opponentCoin,

                                                    completeStatus:1,

                                                    firstUserTotalCup: firstUserTotalCup.cupNo,
                                                    secondUserTotalCup: secondUserTotalCup.cupNo
                                                    //gameStatus:"Win"
                                                }, "Game is over"));
                                                clearTimeout(this.interval); 


                                                ////////////////////

                                                }).catch(secondUserTotalCupErr=>{
                                                console.log("secondUserTotalCupErr"+secondUserTotalCupErr);
                                               })

                                               }).catch(firstUserTotalCupErr=>{
                                                console.log("firstUserTotalCupErr"+firstUserTotalCupErr);
                                              })


                                                ///////////////////


                                            }).catch(err => {
                                                logger.print("***Room update error ", err);
                                            })
                                        //clearTimeout(this.interval); 
                                     //update coin////   
                                     //});
                                       
                                     
                                //});
                                /////update coin////
                             /* } else {
                                logger.print("***No calculation as game already finish ", err);
                                
                             }*/
                               
                            }).catch(err=>{
                                //callback("err", null);
                            })

                        }
                        else{
                          logger.print("***No calculation as game already finish ", err);

                        }



                           //clearTimeout(this.interval);
                           //callback(null, gameStartObj2);
                         }).catch(err => {
                         });    
                        //console.log("first turn i turn 0 "+roomDetails.userId+gameStartObj1.roomName);
                        //clearTimeout(this.interval);
                        //callback(null, gameStartObj2);
                        //logger.print("Next turn sent after game request"+roomDetails.userId);
                        //io.to(gameStartObj1.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
                    }
                    
                    
                     else {
                        room.updateRoomDetails({roomName: roomObj.roomName}, {game_time_remain: g}).then(function (gameremainOver) {
                           console.log("game timer running");
                           console.log(g);
                           gameStartObj2.g = g;
                           timer2 = setTimeout(gameStartTimmer2, 1000, gameStartObj2);

                         }).catch(err => {
                             logger.print("***Room update error ", err);
                         })
                        /*console.log("game timer running");
                        console.log(g);
                        gameStartObj2.g = g;
                        timer2 = setTimeout(gameStartTimmer2, 1000, gameStartObj2);*/
                    }
                }, 1000, roomObj);
                //new code///////////
                //logger.print("Next turn sent after game request"+roomDetails.userId);
                //io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
           // }
       /* }).catch(err => {
        });*/
    }  


 socket.on('disconnectv2', function (req) {
        let currentSocketId = socket.id;
        logger.print("socket id while disconnecting"+ " " +currentSocketId);
             

        let findIndex = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId == currentSocketId
        });
        let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
            return elemt.socketId != currentSocketId
        });
         console.log(findIndex);
        if (findIndex != -1) {
            let userRoomName = allOnlineUsers[findIndex].roomName;
            if (userRoomName != '') {
                logger.print("room name while disconnecting"+ userRoomName);
                //io.to(userRoomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: allOnlineUsers[findIndex].userId}, "Player leave from room"));
                
                //////////////////////////////////////////////////

                   RoomDb.findOne({name:userRoomName},
                    {_id: 1,game_time:1, name:1}).then(gameresponses=> {

                        if(gameresponses.game_time >0){
                          //game finished//////////////
                          console.log("game finished");
                          if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';
                        }
                        else{

                      let m = 5;

                     let timer8 = setTimeout(function gameStartTimmer8(gameStartObj8) {
                    //if(g===20){
                    if(m===5){ 
                         user.userStatusUpdate({userId:allOnlineUsers[findIndex].userId,userStatus:0}).then(function(statusUpdate){
                          
                          io.to(userRoomName).emit('temporaryDisconnect', response.generate(constants.SUCCESS_STATUS, {
                                    //userId: result.userId,
                                    userId: allOnlineUsers[findIndex].userId,
                                    userGameStatus: "User temporarily unavailable",                                    
                                    roomName: userRoomName                                    
                                }, "User temporarily unavailable"));

                         console.log("disconnect timer start");
                         console.log("user update responses"+statusUpdate);  
                       }).catch(err => {
                          console.log("user update error"+err);
                       }); 
                       
                    }

                    m--;                   

                    if (m === 0) {
                        //fetch user status///
                       user.checkOnlineOrNot({_id:allOnlineUsers[findIndex].userId,onlineStatus:1}).then((userOnlineStatusRes)=>{
                        console.log("user status check"+userOnlineStatusRes.length);
                        if(userOnlineStatusRes.length >0){
                            console.log("not required to disconnect..user reconnect");
                        }

                        else{
                            console.log("user not reconnect after 5 sec");

                        console.log("gameStartObj8"+gameStartObj8);
                        console.log("only 0 sec remaining in disconnect timer");
                        console.log(m);
                        var clientsInRoom = io.nsps['/'].adapter.rooms[gameStartObj8];
                        var numClients = clientsInRoom === undefined ? 0 : Object.keys(clientsInRoom.sockets).length;
                        console.log("numClients"+numClients);
                        ////normal winner declare/////
                        console.log("allOnlineUsers"+allOnlineUsers[findIndex].userId); 
                      async.waterfall([
                    playerLeave({roomName: gameStartObj8, userId: allOnlineUsers[findIndex].userId}),
                    updateRoom,
                    RoomUpdate,
                    //new add
                    gameStatusUpdateDisconnect,
                    gameStatusUpdateOpponentDisconnect,
                    //new/////
                    userStatusUpdate
                    // totalPlayerList,
                    // roomClosed,
                    // memoryRoomRemove
                ], function (err, result) {
                    //allOnlineUsers.splice(findIndex, 1);
                    if (result) {

                        //allOnlineUsers=_.without(allOnlineUsers, _.findWhere(allOnlineUsers, {userId: req.userId}));
                        logger.print("win status after disconnecting"+result.isWin);


                        if (findIndexOpponent != -1 && result.isWin == 1) {
                            logger.print("opponent exists");
                            winnerDeclare({
                                userId: result.opponentUserId,
                                //userId: allOnlineUsers[findIndexOpponent].userId,
                                roomName: userRoomName
                            }).then(function (roomDetails) {
                                allOnlineUsers.splice(findIndex, 1);
                                //allOnlineUsers[findIndex].roomName='';
                                if(findIndex==1)
                                   allOnlineUsers[findIndexOpponent].roomName='';
                                else
                                    allOnlineUsers[findIndex].roomName='';
                              
                                io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                    //userId: result.userId,
                                    firstUserId: result.userId,
                                    firstUserGameStatus: "Lose",
                                    secondUserId:result.opponentUserId,
                                    secondUserGameStatus: "Win",
                                    //userId: roomDetails,
                                    roomName: userRoomName,

                                    firstUserCupNumber:result.cupNumber,
                                    secondUserCupNumber:result.opponentCup,

                                    firstUserCoinNumber: result.availableCoin,
                                    secondUserCoinNumber: result.opponentCoin,

                                    completeStatus:0

                                    //gameStatus: "Lose"
                                    //gameStatus: "Win"
                                }, "Game is over"));
                                logger.print("Room closed");
                            });
                        } else if (findIndexOpponent != -1 && result.isWin == 2) {
                            logger.print("opponent exists");
                            allOnlineUsers.splice(findIndex, 1);
                            //allOnlineUsers[findIndex].roomName='';
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';

                            io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "Draw",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "Draw",
                                //userId: result.roomUsers,
                                roomName: userRoomName,

                                firstUserCupNumber:result.cupNumber,
                                secondUserCupNumber:result.opponentCup,

                                firstUserCoinNumber: result.availableCoin,
                                secondUserCoinNumber: result.opponentCoin,

                                completeStatus:0
                               // gameStatus: "Draw"
                            }, "Game is over"));
                            logger.print("Room closed");
                        } else {
                            logger.print("opponent not exists");
                           // allOnlineUsers.splice(findIndex, 1);
                            if(findIndex==1)
                                allOnlineUsers[findIndexOpponent].roomName='';
                            else
                                allOnlineUsers[findIndex].roomName='';
                            io.sockets.to(socket.id).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                //userId: result.userId,
                                firstUserId: result.userId,
                                firstUserGameStatus: "",
                                secondUserId:result.opponentUserId,
                                secondUserGameStatus: "",
                                //userId: result.roomUsers,
                                roomName: userRoomName,

                                firstUserCupNumber:result.cupNumber,
                                secondUserCupNumber:result.opponentCup,

                                firstUserCoinNumber: result.availableCoin,
                                secondUserCoinNumber: result.opponentCoin,

                                completeStatus:0
                                //gameStatus: ""
                            }, "Game is over"));
                            /*io.to(userRoomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                                userId: result.roomUsers,
                                roomName: userRoomName,
                                gameStatus: ""
                            }, "Game is over"));*/
                            logger.print("Room closed");
                        }
                        logger.print("Room closed");

                       /////

                       //if clause
                       


                    } else{
                        logger.print("***DISCONNECT ERROR ", err);
                        allOnlineUsers.splice(findIndex, 1);
                        //allOnlineUsers[findIndex].roomName='';
                        if(findIndex==1)
                            allOnlineUsers[findIndexOpponent].roomName='';
                        else
                            allOnlineUsers[findIndex].roomName='';
                        io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                       }
                       });

                        /////normal winner declare////

                        }
                       }).catch(err=>{
                         console.log("error while fetch user sttaus"+err);
                       })                       

                        

                        }
                        else{
                            console.log("disconnect timer running");
                            console.log(m);
                            gameStartObj8.m = m;
                            timer8 = setTimeout(gameStartTimmer8, 1000, gameStartObj8);
                        }

                        }, 1000, userRoomName);


                       }

                      }).catch(err => {
                        reject(err);
                      });
                 
                //////////////////////////////////////////////////
               

            }
            else{
                logger.print("Room not found");
                allOnlineUsers.splice(findIndex, 1);
              }
        }
        else{
            logger.print("no socket id found");
        }
    });

});
