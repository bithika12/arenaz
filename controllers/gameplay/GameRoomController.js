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
/*room.createRoom({userId : "5de7ac25c9dba27a72be9023"}).then(function(result){
	console.log("success",result);
}).catch(err=>{
	console.log("error",err);
})*/


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
            if (reqobj.isWin) {
                //user update with coin
                 user.updateUserCoin({userId: reqobj.userId}, {startCoin: reqobj.availableCoin,cupNo:reqobj.cupNumber}).then(function (userStatusUpdate) {
                     callback(null, reqobj);
                });
            } else {
                callback(null, reqobj);
            }
        })
    }

    function gameStatusUpdateOpponent(reqobj, callback) {
        return new Promise((resolve, reject) => {
            if (reqobj.isWin) {
                //user update with coin
                //reqobj.roomUsers
                let findIndex = reqobj.roomUsers.findIndex(elemt => (elemt.userId!=reqobj.userId));
                let userOppo=reqobj.roomUsers[findIndex].userId;
                console.log("opponent user"+userOppo);
                user.updateUserCoinOpponent({userId: userOppo}, {startCoin: reqobj.availableCoin,cupNo:reqobj.cupOpponent}).then(function (userStatusUpdate) {
                    callback(null, reqobj);
                });
            } else {
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
                        message          : "You are winner",
                        read_unread      : 0
                    }).then(function(notificationdetails){
                        logger.print("***Game Notification added ");
                        logger.print("***Game over successfully ");
                        io.sockets.to(socket.id).emit('gameWin',response.generate( constants.SUCCESS_STATUS,{},"You won the match!"));
                        io.to(reqobj.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
                            userId: reqobj.userId,
                            roomName: reqobj.roomName,
                            gameStatus:"Win"
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


    /**
     * @desc This function is used for throw dart
     * @param {String} accesstoken
     * @param {String} roomName
     * @param {String} score
     */
    socket.on('throwDart', function (req) {
        req.socketId = socket.id;
        //chk valid dart or not
        room.findValidDart({roomName: req.roomName}).then(function (roomDetails) {

            async.waterfall([
                dartProcess(req),
                updateRoom,
                //gameOverProcess,
                //NEWLY ADDED FOR COIN
                gameStatusUpdate,
                gameStatusUpdateOpponent,
                gameOverProcess,
                userNextStartDart,

            ], function (err, result) {
                if (result) {
                    logger.print(" throw dart done", req);
                    if (result.playStatus == 1) {
                        logger.print("it is bust");
                    }
                    if(result.isWin==1) {
                        logger.print("winner cup number" + result.cupNumber);
                        logger.print("loser cup number" + result.cupOpponent);
                     }
                    logger.print("user remaining score"+ result.remainingScore);
                    logger.print("user score" +result.playerScore);

                    io.to(req.roomName).emit('gameThrow', response.generate(constants.SUCCESS_STATUS, {
                        userId: result.userId,
                        roomName: result.roomName,
                        remainingScore: result.remainingScore,
                        dartPoint: result.dartPoint,
                        playStatus: result.playStatus,
                        playerScore: result.playerScore,
                        cupNumber: result.cupNumber
                    }, "Dart thrown"));

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
                            //update status room
                            room.updateRoomAfterWait({roomName : reqobj.roomName}).then(responses=> {
                                io.sockets.to(reqobj.roomName).emit('noUser', response.generate(constants.ERROR_STATUS, {message: "No opponent found"}));
                                clearTimeout(this.interval);
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
                            timer = setTimeout(gameStartTimmer, 3000, gameStartObj);
                        }
                    }
                } else {
                    console.log("player left");
                    callback("playergone", null);
               }
            }, 3000,reqobj);
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
                logger.print("Next turn sent after game request"+roomDetails.userId);
                io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
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
                nextUserTurnDart(reqobj, 0)
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
                logger.print("***Next turn sent "+roomDetails.userId);
                io.to(roomObj.roomName).emit('nextTurn', response.generate(constants.SUCCESS_STATUS, {userId: roomDetails.userId}, "Next User"));
                //clearTimeout(waitingDartInterval[reqobj.roomName]);
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
                            roomCoin: req.roomCoin
                        }).then(function (result) {
                            let roomName = result.roomName;
                            userObj = {
                                userId: req.userId,
                                score: 333,
                                total: 333,
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
                                roomCoin: req.roomCoin

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
                                            if (result) {
                                                logger.print("***Done  ", result);
                                                //io.sockets.to(roomName).emit('gameStart',response.generate( constants.SUCCESS_STATUS,{roomName: roomName,users :joineeDetails.users },"Game start !"));
                                                io.sockets.to(socket.id).emit('userJoin', response.generate(constants.SUCCESS_STATUS, {
                                                    roomName: roomName,
                                                    users: joineeDetails.users
                                                }, "User enter in a room !"));
                                            } else
                                                logger.print("***GAME START ERROR ", err);
                                            io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: err}));
                                        });
                                    }
                                    if (joineeDetails.users.length === 2) {
                                        callback();
                                        async.waterfall([
                                            gameStartMod({roomName: roomName}),
                                            //userStatusUpdateAfterGamerequest,
                                            userNextStart
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

                                }).catch(err => {
                                    io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, {message: err}));
                                    callback();
                                })

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
                io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
                callback();
            })
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
                resolve(req.userId)
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

    socket.on('disconnect', function (req) {
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

                async.waterfall([
                    playerLeave({roomName: userRoomName, userId: allOnlineUsers[findIndex].userId}),
                    updateRoom,
                    RoomUpdate,
                    //new add
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
                                    userId: roomDetails,
                                    roomName: userRoomName,
                                    gameStatus: "Win"
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
                                userId: result.roomUsers,
                                roomName: userRoomName,
                                gameStatus: "Draw"
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
                                userId: result.roomUsers,
                                roomName: userRoomName,
                                gameStatus: ""
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
            io.to(req.roomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: req.userId}, "Player leave from room"));
            //if (allOnlineUsers[findIndexOpponent]){
            if (findIndex != -1) {
            async.waterfall([
                playerLeaveMod({roomName: req.roomName, userId: req.userId}),
                updateRoom,
                RoomUpdate
                //userStatusUpdate
                //totalPlayerList,
                //roomClosed,
                //memoryRoomRemove,
                //winnerDeclare({userId: allOnlineUsers[findIndexOpponent].userId})
            ], function (err, result) {
                if (result) {

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


});
