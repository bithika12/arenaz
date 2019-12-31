/** Required Packeges include.*/
const appRoot = require('app-root-path');
var async              = require('async');
/* Include Constants*/
var constants          =  require(appRoot + "/config/constants");
/** Including Models for db operations.*/
var user               = require(appRoot + '/models/User');
var room               = require(appRoot + '/models/Room');
var inmRoom            = require(appRoot + '/models/inmemory/Room');
/*include utils packages */
var io                 = require(appRoot + '/utils/SocketManager').io;
const  response        = require(appRoot + '/utils/ResponseManeger');
const  logger          = require(appRoot + '/utils/LoggerClass');
const  AI              = require(appRoot + '/models/gameplay/AI');

var roomDatastore      = require(appRoot + '/utils/MemoryDatabaseManger').room;
let allOnlineUsers     = require(appRoot + '/utils/MemoryDatabaseManger').allOnlineUsers;

let gameRequestQueue =[];
let requestIsRunning = false;
let waitingDartInterval =[];
/*room.createRoom({userId : "5de7ac25c9dba27a72be9023"}).then(function(result){
	console.log("success",result);
}).catch(err=>{
	console.log("error",err);
})*/


io.on('connection', function(socket){


	function dartProcess(reqobj){
		return function (callback) {
			inmRoom.throwDartDetails(reqobj).then(function(roomDetails){
				callback(null,roomDetails);
		     }).catch(err=>{
	          callback("err", null);
             })

		}
	}

	function updateRoom(reqobj,callback){
		inmRoom.updateInmemoryRoomMod(reqobj).then(function(roomDetails){
			callback(null, roomDetails);
		}).catch(err=>{
			callback("err", null);
		})

	}

	function gameOverProcess(reqobj,callback){
		return new Promise((resolve,reject)=> {
			if (reqobj.isWin) {
				room.updateRoomGameOver({roomName: reqobj.roomName}, {userObj: reqobj.roomUsers}).then(function (updateRoom) {
					io.to(reqobj.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
						userId: reqobj.userId,
						roomName: reqobj.roomName
					}, "Game is over"));
					callback(null, reqobj);
				}).catch(err => {
					logger.print("***Room update error ", err);
				})
			} else{
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

		async.waterfall([
			dartProcess(req),
			updateRoom,
			//roomClosed,
			gameOverProcess,
			userNextStartDart,

		],function (err, result) {
			if (result){
				//allOnlineUsers.splice(findIndex, 1);
				//logger.print("Room closed");
				logger.print(" throw dart done",req);
				/*if(result.isWin==1) {
					//update room details from nedb//

					io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
						userId: result.userId,
						roomName: result.roomName
					}, "Game is over"));
				}*/
				io.to(req.roomName).emit('gameThrow',response.generate(constants.SUCCESS_STATUS,{ userId : result.userId,roomName:result.roomName,
					remainingScore:result.remainingScore,dartPoint:result.dartPoint,playStatus:result.playStatus,playerScore:result.playerScore},"Dart thrown"));
			}else
				logger.print("***GAME ERROR ",err);
		});


		/*inmRoom.throwDartDetails(req)
			.then(roomStatusUpdate => {
				return inmRoom.updateInmemoryRoom(req,roomStatusUpdate
				);
			})

			.then(res => {
				return nextUserTurnDart(res)
				return userNextStartDart(res
				);
			})
			.then(resp=>{
				logger.print(" throw dart done",req);
				return io.to(req.roomName).emit('gameThrow',response.generate(constants.SUCCESS_STATUS,{ userId : resp.userId,roomName:resp.roomName,remainingScore:resp.remainingScore,dartPoint:resp.dartPoint},"Dart thrown"));
			})
			.catch(err=>{
				console.log('dart error' + err);
				io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{"err":err},"Something went wrong!"));
			});*/

	});

    //GAME START
	function waitingForUser(reqobj){
		return function (callback) {
			var i = constants.GAME_TIMMER;
			logger.print("  ************  game start call");
			let timer = setTimeout(function gameStartTimmer(gameStartObj) {
				i--;
				if (i === 0) {
					clearTimeout(this.interval);
					callback(null, gameStartObj);
				}else{
					gameStartObj.i = i
					timer = setTimeout(gameStartTimmer, 1000,gameStartObj);
				}
			}, 1000,reqobj);
		}
	}
	function gameStartMod(reqobj){
		return function (callback) {
			room.updateRoomDetails({roomName:reqobj.roomName},{status:"open"}).then(function(roomStatusUpdate){
				inmRoom.getRoomDetails({roomName:roomStatusUpdate},{status:"closed"}).then(function(roomDetails){
					var gameUserList = (!roomDetails)?[]:roomDetails.users ;
					if(gameUserList.length == 1){
						callback(null, reqobj);
					}
					else if(gameUserList.length ==2){
						room.updateRoomDetails({roomName:reqobj.roomName},{status:"closed"}).then(function(roomStatusUpdate) {
							callback(null, reqobj);
						});
					}
					else{
						inmRoom.removeRoom({roomName:reqobj.roomName}).then(function(roomupdate){
							io.to(reqobj.roomName).emit('gameClosed',response.generate(constants.ERROR_STATUS,{ },"Sorry!! All player leave"));
							callback("all player leave", null);
						})
					}
				}).catch(err=>{
					callback("err", null);
				})
			});
		}
	}
	function gameStart(reqobj,callback){
			inmRoom.getRoomDetails({roomName:reqobj.roomName},{status:"closed"}).then(function(roomDetails){
				var gameUserList = (!roomDetails)?[]:roomDetails.users ;
				if(gameUserList.length == 1){
					callback(null, reqobj);
				}
				else if(gameUserList.length ==2){
					room.updateRoomDetails({roomName:reqobj.roomName},{status:"closed"}).then(function(roomStatusUpdate) {
						callback(null, reqobj);
					});
				}
				else{
					inmRoom.removeRoom({roomName:reqobj.roomName}).then(function(roomupdate){
						io.to(reqobj.roomName).emit('gameClosed',response.generate(constants.ERROR_STATUS,{ },"Sorry!! All player leave"));
						callback("all player leave", null);
					})
				}
			}).catch(err=>{
				callback("err", null);
			})

	}

	/**
	 * @desc next user turn
	 * @param reqobj
	 * @param callback
	 */

	function nextUserTurn(roomObj)
	{
		inmRoom.findNextUser({roomName : roomObj.roomName}).then(function(roomDetails){
			if(roomDetails){
				io.to(roomObj.roomName).emit('nextTurn',response.generate(constants.SUCCESS_STATUS,{ userId : roomDetails.userId},"Next User"));
			}
		}).catch(err=>{ });
	}

	function userNextStart(reqobj,callback){
		waitingDartInterval[reqobj.roomName] = setTimeout(() => { nextUserTurn(reqobj, 0) }, 1000);
		callback(null, reqobj);
	}

	function userNextStartDart(reqobj,callback){
		return new Promise((resolve,reject)=>{
			waitingDartInterval[reqobj.roomName] = setTimeout(() => { nextUserTurnDart(reqobj, 0) }, 1000);
			callback(null, reqobj);
		})
	}

	function nextUserTurnDart(roomObj)
	{
			inmRoom.findNextUserDart({roomName: roomObj.roomName}).then(function (roomDetails) {
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
    	logger.print(" game request ",req);
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

	function processGameRequest(req, callback)  {
		if (req.userId && req.userName) {
			user.getUserSocketDetails({userId: req.userId}).then((userDetails)=>{
				var findIndex = allOnlineUsers.findIndex(function(elemt) {return elemt.userId == req.userId });

				if(findIndex == -1 || allOnlineUsers[findIndex].roomName != ''){
					io.sockets.to(req.socketId).emit('error',response.generate( constants.ERROR_STATUS,{},"User cannot join"));
					callback();
				}
				else{
				//if(findIndex != -1 ){
					let userSocketId  = allOnlineUsers[findIndex].socketId;
					if(io.sockets.sockets[userSocketId] != undefined){
						//find user already in room
						room.createRoom({userId: req.userId, userName:req.userName,colorName:req.colorName,raceName:req.raceName}).then(function(result){
							let roomName = result.roomName;
							userObj ={userId : req.userId ,score:100,total:100,status:"active",isWin:0,turn:0,dartPoint:"",
								userName:req.userName,colorName:req.colorName,raceName:req.raceName,total_no_win:0};
							inmRoom.roomJoineeCreation({roomId:result._id,roomName:result.roomName},{userObj : userObj}).then((joineeDetails)=>{
								io.to(roomName).emit('enterUser',response.generate(constants.SUCCESS_STATUS,{ user : userObj},"Player enter to the room"));
								io.of('/').connected[userSocketId].join(roomName, function () {
									allOnlineUsers[findIndex].roomName = roomName;
									io.sockets.to(socket.id).emit('connectedRoom',response.generate( constants.SUCCESS_STATUS,{roomName: roomName,users :joineeDetails.users },"You are waiting in a room !"));
									//fetch user colorname,racename//////
									//user.fetchColorMod(roomName,joineeDetails.users).then((allRacerDetails)=> {

									if(joineeDetails.users.length === 1  && roomName != undefined ){
										callback();
										async.waterfall([
											waitingForUser({roomName:roomName}),
											gameStart
										],function (err, result) {
											if (result){
												logger.print("***Done  ", result);
												//io.sockets.to(roomName).emit('gameStart',response.generate( constants.SUCCESS_STATUS,{roomName: roomName,users :joineeDetails.users },"Game start !"));
												io.sockets.to(socket.id).emit('userJoin',response.generate( constants.SUCCESS_STATUS,{roomName: roomName,users :joineeDetails.users },"User enter in a room !"));
											}else
												logger.print("***GAME START ERROR ", err);
											    io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:err}));
										});
									}
									if(joineeDetails.users.length === 2  ){
										callback();
										async.waterfall([
											gameStartMod({roomName:roomName}),
											userNextStart
										],function (err, result) {
											if (result){
												io.sockets.to(socket.id).emit('userJoin',response.generate( constants.SUCCESS_STATUS,{roomName: roomName,users :joineeDetails.users },"User enter in a room !"));
												logger.print("***Done  ", result);
                                                io.sockets.to(roomName).emit('gameStart',response.generate( constants.SUCCESS_STATUS,{roomName: roomName,users :joineeDetails.users },"Game start !"));
											}else
												logger.print("***GAME START ERROR ", err);
												io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:err}));

										});
									}
									// Otherwise, send an error message back to the player.
									else{
										io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:"Unable to found room"}));
										callback();
									}
								/*}).catch(err=>{
										io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:err}));
										callback();
									})*/

							}).catch(err=>{
								io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:err}));
								callback();
							})

							}).catch(err=>{
								io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:err}));
								callback();
							})
						}).catch(err=>{
							io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:"Unable to create room"}));
							callback();
						})
					}else{
						io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:"No socket Id found"}));
						callback();
					}
				}

			}).catch(err=>{
				io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,err));
				callback();
			})
		}else
		{
			io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:"User not found"}));
			callback();
		}
	}

	function updateInRoom(req){
		room.updateRoomDetails({roomName:req.roomName},{status:"forfeit"}).then(function(roomStatusUpdate){
			io.to(req.roomName).emit('gameClosed',response.generate(constants.ERROR_STATUS,{ },"game closed"));
			callback(null,req);
		})
	}

	function playerLeaveOld(req){
		return function (callback) {
			inmRoom.userLeave({roomName:req.roomName ,userId : req.userId}).then(function(updateDetails){
				room.playerLeave({roomName:req.roomName,userId : req.userId},updateDetails).then(function(playerUpdate){
					callback(null,req);
				})
			}).catch(err=>{
				callback("",null);
			})
		}

	}
	function totalPlayerList(req,callback){
		inmRoom.getRoomDetails({roomName:req.roomName}).then(function(roomDetails){
			if(!roomDetails || roomDetails.users.length <= 1){
				callback(null,req);
			}else{
				callback("err",null);
			}
		})
	}
	function roomClosed(req,callback){
		console.log("roomClosed",req)
		room.updateRoomDetails({roomName:req.roomName},{status:"closed"}).then(function(roomStatusUpdate){
			io.to(req.roomName).emit('gameClosed',response.generate(constants.ERROR_STATUS,{ },"game closed"));
			callback(null,req);
		})
	}

	function playerLeave(req){
		return function (callback) {
			inmRoom.userLeave({roomName:req.roomName ,userId : req.userId}).then(function(updateDetails){
				room.playerLeave({roomName:req.roomName,userId : req.userId}).then(function(playerUpdate){
					callback(null,req);
				})
			}).catch(err=>{
				callback("",null);
			})
		}

	}


	function memoryRoomRemove(req,callback){
		inmRoom.removeRoom({roomName:req.roomName}).then(function(roomupdate){
			callback(null,req)
		});
	}

	/**
	 * @desc This function is used for color request
	 * @param {String} accesstoken
	 * @param {String} sageName
	 * @param {String} colorName
	 */

	socket.on('colorRequest', function (req) {
		user.colorRequest({userId:req.userId},{"colorName"  :req.color})
			.then(colorUpdate => {
				return user.sageRequest({userId:req.userId},{"raceName"  :req.race}
				);
				//io.to(req.roomName).emit('colorRequest',response.generate(constants.SUCCESS_STATUS,{ },colorUpdate));
			})
			.then(resp=>{
				io.sockets.to(socket.id).emit('colorGet',response.generate(constants.SUCCESS_STATUS,{ },"Color captured"));
			})
			.catch(err=>{
				console.log(err);
				io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{"message":err}));
			});
	});

	/**
	 * @desc This function is used for while user leave the game
	 * @param {String} accesstoken
	 * @param {String} roomName
	 */
	socket.on('leave1',function(req){
		if (req.userId && req.roomName) {
			let currentSocketId = socket.id;
			var findIndex = allOnlineUsers.findIndex(function(elemt) {return elemt.socketId == currentSocketId });
			if(io.sockets.sockets[currentSocketId]!=undefined)
				io.sockets.sockets[currentSocketId].leave(req.roomName);
			io.to(req.roomName).emit('playerLeave',response.generate(constants.SUCCESS_STATUS,{ userId : req.userId },"Player leave from room"));

			//inmRoom.userLeaveMod({roomName:req.roomName ,userId : req.userId})
			inmRoom.userLeave({roomName:req.roomName ,userId : req.userId})
				.then(updateDetails => {
					return inmRoom.updateInmemoryRoomLeave(req,updateDetails
					);
				})
				.then(roomUpdate => {
					return room.updateRoomLeave(req,roomUpdate
					);
				})
				.then(resp=>{
					logger.print("Room closed");
					allOnlineUsers.splice(findIndex, 1);
					io.sockets.to(req.roomName).emit('userLeave',response.generate( constants.SUCCESS_STATUS,{roomName: req.roomName,userName :req.userName,userId:req.userId },"User Left !"));
				})
				.catch(err=>{
					logger.print("***GAME ERROR ",err);
					io.sockets.to(socket.id).emit('error',response.generate( constants.ERROR_STATUS,{message:err}));
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
		return new Promise((resolve,reject)=>{
		user.updatePointDetails({_id: req.userId}, {
			total_no_win: 1,

		}).then(function (updateWinningDetails) {
			//users.rank = users.totalUserKill
			io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
				userId: req.userId,
				roomName: req.roomName
			}, "Game is over"));
			resolve(req.userId)
			//callback(null, req);
		})
	   })
	}
	/**
	 * @desc This function is used for while user disconnected
	 * @param {String} accesstoken
	 */


	socket.on('disconnect', function(req){
		let currentSocketId = socket.id;
		let findIndex = allOnlineUsers.findIndex(function(elemt) {return elemt.socketId == currentSocketId });
		let findIndexOpponent = allOnlineUsers.findIndex(function(elemt) {return elemt.socketId != currentSocketId });
		if(findIndex != -1){
			userRoomName  =allOnlineUsers[findIndex].roomName;
			if(userRoomName != ''){
				io.to(userRoomName).emit('playerLeave',response.generate(constants.SUCCESS_STATUS,{ userId : allOnlineUsers[findIndex].userId},"Player leave from room"));

					async.waterfall([
						playerLeave({roomName: userRoomName, userId: allOnlineUsers[findIndex].userId}),
						totalPlayerList,
						roomClosed,
						memoryRoomRemove
					], function (err, result) {
						//allOnlineUsers.splice(findIndex, 1);
						if (result) {

							if(findIndexOpponent!=-1){
								winnerDeclare({userId: allOnlineUsers[findIndexOpponent].userId,roomName:req.roomName}).then(function(roomDetails){
									io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
										userId: roomDetails,
										roomName: req.roomName
									}, "Game is over"));
									logger.print("Room closed");
								});
							}
							logger.print("Room closed");
							allOnlineUsers.splice(findIndex, 1);

							/*if (allOnlineUsers.length == 1) {
								io.to(userRoomName).emit('player_leave', response.generate(constants.SUCCESS_STATUS, {user_id: allOnlineUsers[findIndex].id}, "user has been disconnected!!"));

							} else {
								io.to(userRoomName).emit('player_leave', response.generate(constants.SUCCESS_STATUS, {user_id: allOnlineUsers[findIndex].id}, "user has been disconnected!!"));
							}*/

						} else
							logger.print("***DISCONNECT ERROR ", err);
						    io.sockets.to(socket.id).emit('error', response.generate(constants.ERROR_STATUS, err));
					});

			}
		}
	});


	socket.on('leave',function(req){
		if (req.userId && req.roomName) {
			let currentSocketId = socket.id;

			var findIndex = allOnlineUsers.findIndex(function (elemt) {
				return elemt.socketId == currentSocketId
			});
			let findIndexOpponent = allOnlineUsers.findIndex(function (elemt) {
				return elemt.socketId != currentSocketId
			});
			if (io.sockets.sockets[currentSocketId] != undefined)
				io.sockets.sockets[currentSocketId].leave(req.roomName);
			io.to(req.roomName).emit('playerLeave', response.generate(constants.SUCCESS_STATUS, {userId: req.userId}, "Player leave from room"));
			//if (allOnlineUsers[findIndexOpponent]){
				async.waterfall([
					playerLeave({roomName: req.roomName, userId: req.userId}),
					totalPlayerList,
					roomClosed,
					memoryRoomRemove,
					//winnerDeclare({userId: allOnlineUsers[findIndexOpponent].userId})
				], function (err, result) {
					if (result) {
						if (findIndexOpponent!=-1){
							winnerDeclare({userId: allOnlineUsers[findIndexOpponent].userId,roomName:req.roomName}).then(function(roomDetails){
								io.to(req.roomName).emit('gameOver', response.generate(constants.SUCCESS_STATUS, {
									userId: roomDetails,
									roomName: req.roomName
								}, "Game is over"));
								logger.print("Room closed");
							});
					      }
						logger.print("Room closed");
						allOnlineUsers.splice(findIndex, 1);
					} else
						logger.print("***GAME ERROR ", err);
				});
		//}


		}
	})


});
