/* Include Constants*/
const appRoot = require('app-root-path');
var constants          =  require(appRoot + "/config/constants");
/** Required Packeges include.*/
var async              = require('async');
/** Including Models for db operations.*/
var user               = require(appRoot + '/models/User');
//var room               = require('../../models/Room');
//var inmRoom            = require(appRoot + '/models/inmemory/Room');
/*include utils packages */
var io                 = require(appRoot + '/utils/SocketManager').io;
const  response        = require(appRoot + '/utils/ResponseManeger');
const  logger          = require(appRoot + '/utils/LoggerClass');
//GLOBAL VARIABLE
 allOnlineUsers       = require(appRoot + '/utils/MemoryDatabaseManger').allOnlineUsers;



io.on('connection', function(socket){  	

	//Socket Midileware

	socket.use((packet, next) => { 
    	user.checkUserTokenMod({accessToken: packet[1].accessToken}).then((userDetails)=>{
    	    if (userDetails) {
            	packet[1].userId = userDetails._id+"";
            	//packet[1].userName = (!userDetails.name)?"":userDetails.name;
				packet[1].userName = (!userDetails.userName)?"":userDetails.userName;
				packet[1].colorName = (!userDetails.colorName[0])?"":userDetails.colorName[0]['colorName'];
				packet[1].raceName = (!userDetails.raceName[0])?"":userDetails.raceName[0]['raceName'];
				//packet[1].colorName = (!userDetails.colorName[0]['colorName'])?"":userDetails.colorName[0]['colorName'];
				//packet[1].raceName = (!userDetails.raceName[0]['raceName'])?"":userDetails.raceName[0]['raceName'];
              return next(); 
            }else next(new Error(' error'));
        })
    });



	socket.on('addUser', function(req){
		if (req.userId) {
			user.updateSocketDetails({ "userId"    :req.userId},{"socketId"  :socket.id}).then((updateSocket)=>{
				var findIndex = allOnlineUsers.findIndex(function(elemt) {return elemt.userId == req.userId });
				if(findIndex != -1){
					allOnlineUsers[findIndex].socketId = socket.id
				}else
					allOnlineUsers.push({
						"socketId"  : socket.id,
						"userId"    : req.userId,
						"userName"  : req.userName ,
						"roomName"  :""
					});

				logger.print("   processGameRequest addUser   ",allOnlineUsers);
				io.sockets.to(socket.id).emit('userConnected',response.generate(constants.SUCCESS_STATUS,{userId:req.userId, userName :req.userName }, "Add user to socket"));
			}).catch(err=>{
				io.sockets.to(socket.id).emit('userConnected',response.generate(constants.ERROR_STATUS,{ }, "Error join to  socket"));
			});
		}
	});

   /* socket.on('addUser', function(req){
        if (req.userId) {
        	user.updateSocketDetails({ "userId"    :req.userId},{"socketId"  :socket.id}).then((updateSocket)=>{
	            allOnlineUsers.push({
					var findIndex = allOnlineUsers.findIndex(function(elemt) {return elemt.userId == req.userId });
				if(findIndex != -1){
					allOnlineUsers[findIndex].socketId = socket.id
				}
				else
	            	                  "socketId"  : socket.id,
	            	                  "userId"    : req.userId,
	            	                  "userName"  : req.userName , 
	            	                  "roomName"  :""
	            	              });
	            let newArr=[];
	            newArr.push(allOnlineUsers);
	            console.log(" allOnlineUsers",allOnlineUsers);
	           io.sockets.to(socket.id).emit('userConnected',response.generate(constants.SUCCESS_STATUS,{userId:req.userId, userName :req.userName }, "Add user to socket"));    
           }).catch(err=>{
                io.sockets.to(socket.id).emit('userConnected',response.generate(constants.ERROR_STATUS,{ }, "Add user to socket"));    
           });
        }
    });*/



});