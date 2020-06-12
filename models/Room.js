/** Import Module*/
var underscore  = require('underscore');
const appRoot = require('app-root-path');
var Room = require(appRoot + '/schema/Schema').roomModel;
//const client = require(appRoot + "/config/radisConfig.js");
//const  roomPlayer      = require(appRoot + '/utils/RoomPlayerManage');
var room ={}
var constants          =  require(appRoot + "/config/constants");
var mongoose = require('mongoose');
var ObjectID = require("mongodb").ObjectID;
const moment=require("moment");
var Roomlog = require(appRoot + '/schema/Schema').roomLogModel;



room.createRoom = function(condObj){
    return new Promise((resolve,reject) => {


        Room.aggregate([
            {$match :{"status":"open"}},
            { "$redact": {
                    "$cond": {
                        "if": {
                            "$lt": [
                                { "$size": {
                                        "$concatArrays": [
                                            { "$ifNull": [ "$users", [] ] }
                                        ]
                                    } },
                                constants.GAME_MAXIMUM_USER
                            ]
                        },
                        "then": "$$KEEP",
                        "else": "$$PRUNE"
                    }
                }},
            { "$project": { "_id": 1 ,name:1,users:1} }
        ]).limit(1).then(responses=> {
            if(!responses|| responses.length ==0 ){
                roomObj ={ name : 'RM'+new Date().getTime(),
                           "status":"open",
                           users : {userId : condObj.userId ,
                                     status: "active",
                                       total:"199",
                                       score:"0",
                                       //score:"199",
                                       //total:"333",
                                       //score:"333",
                                       isWin:0,
                                       turn:0,
                                       dartPoint:"",
                                       userName:condObj.userName,
                                       colorName:condObj.colorName,
                                       raceName:condObj.raceName,
                                       dartName:condObj.dartName,
                                       total_no_win:0,
                                       cupNumber:0,
                                       roomCoin:condObj.roomCoin,
                                       firstName:condObj.firstName,
                                       lastName:condObj.lastName
                                        }}

                console.log(" roomObj",roomObj)
                Room.create(roomObj).then(responses=> {
                    resolve({ roomName : responses.name ,_id :responses._id });
                }).catch(err => {
                    reject(err);
                });
            }else {
                if (responses[0].users[0]['roomCoin'] != condObj.roomCoin) {
                    //add user to a seperate room
                     let roomObj ={
                         name : 'RM'+new Date().getTime(),
                        "status":"open",
                        users : {userId : condObj.userId ,
                            status: "active",
                            total:"199",
                            score:"0",
                            //score:"199",
                            //total:"333",
                            //score:"333",
                            isWin:0,
                            turn:0,
                            dartPoint:"",
                            userName:condObj.userName,
                            colorName:condObj.colorName,
                            raceName:condObj.raceName,
                            dartName:condObj.dartName,
                            total_no_win:0,
                            cupNumber:0,
                            roomCoin:condObj.roomCoin,
                            firstName:condObj.firstName,
                            lastName:condObj.lastName
                             }}

                    console.log(" roomObj",roomObj)
                    Room.create(roomObj).then(responses=> {
                        resolve({ roomName : responses.name ,_id :responses._id });
                    }).catch(err => {
                        reject(err);
                    });
                }
                else {
                updateObj = {}
                Room.updateOne({"_id": responses[0]._id},
                    {
                        $push: {
                            users: {
                                userId: condObj.userId,
                                status: "active",
                                total: 199,
                                score: 0,
                                //score: 199,
                                //total: 333,
                               // score: 333,
                                isWin: 0,
                                turn: 0,
                                userName: condObj.userName,
                                colorName: condObj.colorName,
                                raceName: condObj.raceName,
                                dartName: condObj.dartName,
                                total_no_win: 0,
                                cupNumber: 0,
                                roomCoin: condObj.roomCoin,
                                firstName:condObj.firstName,
                                lastName:condObj.lastName
                            }
                        }
                    },
                    {multi: true}).then(updateRoomDetails => {
                    return resolve({roomName: responses[0].name, _id: responses[0]._id});

                }).catch(err => {
                    reject(err);
                });

                // }
              }
            }
        }).catch(err => {
            reject(err);
        });
    })

}



room.createBot =function (room_name,reqestObj) {
   return new Promise((resolve,reject) => {
          client.HMSET(room_name,[ 
                                 //   "status",status,
                                    "users:"+reqestObj.user_id+":id",reqestObj.user_id,
                                    "users:"+reqestObj.user_id+":name",reqestObj.name,
                                    "users:"+reqestObj.user_id+":position:X",reqestObj.position.X,
                                    "users:"+reqestObj.user_id+":position:Y",reqestObj.position.Y,
                                    "users:"+reqestObj.user_id+":playerScale:X",reqestObj.playerScale.X,
                                    "users:"+reqestObj.user_id+":playerScale:Y",reqestObj.playerScale.Y,
                                    "users:"+reqestObj.user_id+":texture",reqestObj.texture,
                                    "users:"+reqestObj.user_id+":playerAngle",reqestObj.playerAngle,
                                    "users:"+reqestObj.user_id+":playerSpeed",reqestObj.playerSpeed,
                                    "users:"+reqestObj.user_id+":type",reqestObj.type,
                                    "users:"+reqestObj.user_id+":totalUserKill",reqestObj.totalUserKill,  
                                    // "users",JSON.stringify(users)
                                    ],function (err, roomdata) {
          if(err){
              reject();
          }else{
              resolve({"name":reqestObj.name,status:"open",users:reqestObj});  
          }
      })  

   });

};
room.roomClosed =function (reqestObj) {
  return new Promise((resolve,reject) => {
   // console.log("reqestObj   :",reqestObj);
     client.SREM("room",reqestObj.room_name,function (err, removeroom) {
        client.HMSET(reqestObj.room_name,["status",'closed'],function (err, roomdata) {
          if(err){
              reject();
          }else{
              resolve({removeroom});
          }
        });   

     }) 
  })
}
    
room.roomDetails =function (reqestObj) {
    return new Promise((resolve,reject) => {
      client.HGETALL(reqestObj.room_name,function (err, roomdata) {
        console.log(" roomdata",roomdata)
     //   client.del("users:name:5d7a56ac0deb0d492fbe991f","users:name:5d7a56ac0deb0d492fbe991f");

//         client.multi([
//     ["del", "users:name:5d7a56ac0deb0d492fbe991f"],
//     ["del", "users:name:5d7a56ac0deb0d492fbe991f"]
// ]).exec(function (err, replies) {
// })


      });
    });   
}

room.deleteKeyValue = function(reqestObj){
  return new Promise((resolve,reject) => {
  client.HDEL('RM1568630584016',"0","match","*:5d7f67218db1613940a7fd8c:*", function(err, o) {
     //client.HSCAN(reqestObj.room_name,"1","match","*:position:*",function(err,o){
      console.log("o",o)
     console.log("err",err)
   });
   
  })  

}
room.getKeyValueDetails = function(reqestObj){
  return new Promise((resolve,reject) => {
     //  client.georadius(
     //  'Runa',    //va-universities is the key where our geo data is stored
     //  360,            //the longitude from the user
     //  640,             //the latitude from the user
     //  '100',                //radius value
     //  '',                 //radius unit (in this case, Miles)
     //  'WITHCOORD',          //include the coordinates in the result
     //  'WITHDIST',           //include the distance from the supplied latitude & longitude
     //  'ASC',                //sort with closest first
     //  function(err, results) {
     //     if(err){
     //          reject();
     //      }else{
     //          resolve(results);
     //      }
     // });
    client.HSCAN(reqestObj.room_name,"1","match","*:position:*",function(err,roomdetails){
         if(err){
              reject();
          }else{
              resolve({roomdetails});
          }
     });
   })

}





room.storeRoomDetails = function(roomObj){
    return new Promise((resolve,reject) => {
        Room.create(roomObj).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        })
    })
}

/*db.rooms.updateOne(
    { _id: ObjectId("5dee3d0e98e37b319712ac27"), "users._id": ObjectId("5dee3d6398e37b319712ac2a") },
    { $set: { "users.$.score" : 12 } }
)*/
//valid dart or not
//findValidDart
room.findValidDart = function(condObj){
    return  new Promise((resolve,reject) => {
        Room.findOne({name:condObj.roomName}, {_id: 1,game_time:1, name:1, score:1, total:1, sockets:1}).then(responses=> {
            if(responses.game_time){
                reject({message:"Error:Game is over"})
            }
            else{
                resolve(true);
            }
        }).catch(err => {
            return reject(err);
        });
    });
}
room.updateRoomDart = function(condObj,updateObj){
    return new Promise((resolve,reject) => {
        Room.updateOne({ "name":condObj.roomName,"users.userId":condObj.userId},updateObj).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        })
    })
}
room.updateRoomDetails = function(condObj,updateObj){
    return new Promise((resolve,reject) => {
        Room.updateOne({ "name":condObj.roomName},updateObj).then(responses=> {
            return resolve(condObj.roomName);
            //return resolve(responses);
        }).catch(err => {
            return reject(err);
        })
    })
}
room.playerLeaveOld = function(condObj,updateObj){
    console.log(" remove  room details  ",condObj)
    return new Promise((resolve,reject) => {
        Room.updateOne(
            {
                "name":condObj.roomName
            },
            updateObj
            /*{
                "$set": {
                    "users.$.status" : "inactive"
                }
            }*/

        /*updateOne({ "name":condObj.roomName},
            {
                $pull: { users: { userId: condObj.userId} }
            },
            { multi: true }*/).then(updateRoomDetails=> {
            return resolve(updateRoomDetails);
        }).catch(err => {
            return reject(err);
        })
    })
}

room.getUserRoomDetails = function(condObj){
    return  new Promise((resolve,reject) => {
        Room.findOne({name:condObj.roomName}, {_id: 1, name:1, score:1, total:1, sockets:1}).then(responses=> {
            return resolve({_id :responses._id ,name : responses.name, score :  responses.score,total: responses.total });
        }).catch(err => {
            return reject(err);
        });
    });
}
room.updateRoomLeave  = function(userObj,updateArr){
    return new Promise((resolve,reject)=>{

        Room.updateOne({name : userObj.roomName},{ $set: { users: updateArr.usertotal }},
            function (err, updateroomresult) {
                if (err)
                    reject({message:"Error:Database connection error"})
                else {
                    if(updateroomresult.nModified >0)
                        resolve(true)
                    else
                        reject({message:"Unable to update memory room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });

    })
}

room.updateRoomLeaveDisconnect  = function(updateArr){
    return new Promise((resolve,reject)=> {

        if (updateArr.userTotal.length == 1) {
            Room.updateOne({name: updateArr.roomName}, {
                    $set: {
                        status: "closed",
                        //game_time: updateArr.gameTotalTime,
                        updated_at: Date.now(),
                        users: updateArr.userTotal
                    }
                },
                function (err, updateroomresult) {
                    if (err)
                        reject({message: "Error:Database connection error"})
                    else {
                        if (updateroomresult.nModified > 0)
                            resolve(true)
                        else
                            reject({message: "Unable to update memory room"});
                        //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                    }

                });
        }
        else {
        Room.updateOne({name: updateArr.roomName}, {
                $set: {
                    status: "closed",
                    game_time: updateArr.gameTotalTime,
                    updated_at: Date.now(),
                    users: updateArr.userTotal
                }
            },
            function (err, updateroomresult) {
                if (err)
                    reject({message: "Error:Database connection error"})
                else {
                    if (updateroomresult.nModified > 0)
                        resolve(true)
                    else
                        reject({message: "Unable to update memory room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });
      }

    })
}

room.playerLeave = function(condObj){
    console.log(" remove  room details  ",condObj)
    return new Promise((resolve,reject) => {
        Room.updateOne({ "name":condObj.roomName},
            {
                $pull: { users: { userId: condObj.userId} }
            },
            { multi: true }).then(updateRoomDetails=> {
            return resolve(updateRoomDetails);
        }).catch(err => {
            return reject(err);
        })
    })
}

room.updateRoomGameOver = function(condObj,updateObj){
    return new Promise((resolve,reject) => {

        Room.updateOne({name : condObj.roomName},{ $set: { status:"closed",updated_at:Date.now(),game_time:condObj.gameTotalTime,users: updateObj.userObj }},
       // Room.updateOne({roomName : condObj.roomName},{ $set: { users: updateObj.userObj }},

            //room.update({roomName : userObj.roomName},{ $set: { users: updateArr.finalArr.users }},
            function (err, updateroomresult) {

                if (err)
                    reject({message:"Error:Database connection error"})
                else {
                    if(updateroomresult.nModified >0)
                        //resolve({roomName : userObj.roomName,userArr:updateArr})
                        resolve(true)
                    //resolve({userId: updateArr.users,remainingScore:updateArr.remainingScore,userTurn:updateArr.userTurn,dartPoint:updateArr.dartPoint})
                    else
                        reject({message:"Unable to update memory room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });
    })
}

////insert room log////
//updateRoomLogGameOver
room.updateRoomLogGameOver = function(condObj,updateObj,hitscoreObj){
    return new Promise((resolve,reject) => {

        let userArr=updateObj.userObj;
        console.log("userArr in log"+userArr);
        userArr.forEach(function(val,key){
        console.log("key in log"+key);
        console.log("val in log"+val);
        console.log("val in log"+hitscoreObj.hitScore);
        if(!val.cupNumber){
          val.cupNumber=0

        }
        if(!hitscoreObj.hitScore){
          hitscoreObj.hitScore=''

        }
        if(!hitscoreObj.scoreMultiplier){
          hitscoreObj.scoreMultiplier=''

        }
        //hitScore:updateArr.hitScore,
        //scoreMultiplier:updateArr.scoreMultiplier
        //console.log("val in log"+val.cupNumber);
        let roomlogObj ={   name : condObj.roomName,
                           status:val.status,
                            userId : val.userId ,
                                     
                                       total:val.total,
                                       score:val.score,
                                       //score:"199",
                                       //total:"333",
                                       //score:"333",
                                       isWin:val.isWin,
                                       turn:val.turn,
                                       dartPoint:val.dartPoint,
                                       userName:val.userName,
                                       colorName:val.colorName,
                                       raceName:val.raceName,
                                       dartName:val.dartName,
                                       total_no_win:val.total_no_win,
                                       cupNumber:val.cupNumber,
                                       roomCoin:val.roomCoin,
                                       firstName:val.firstName,
                                       lastName:val.lastName,
                                       scoreMultiplier:val.scoreMultiplier,
                                       hitScore:val.hitScore
                                       //totalCupWin:val.totalCupWin
                                        }


           Roomlog.create(roomlogObj).then(responses=> {
                    if(key==userArr.length-1)
                      resolve(true)
                }).catch(err => {
                    console.log("error while insert log"+err);
                    reject({message:"Unable to insert room log"});

                });                              

      })


     
        
    })
}

room.updateRoomAfterWait = function(condObj){
    return new Promise((resolve,reject) => {

        Room.updateOne({name : condObj.roomName},{ $set: { status:"closed",updated_at:Date.now() }},
            // Room.updateOne({roomName : condObj.roomName},{ $set: { users: updateObj.userObj }},

            //room.update({roomName : userObj.roomName},{ $set: { users: updateArr.finalArr.users }},
            function (err, updateroomresult) {

                if (err)
                    reject({message:"Error:Database connection error"})
                else {
                    if(updateroomresult.nModified >0)
                        //resolve({roomName : userObj.roomName,userArr:updateArr})
                        resolve(true)
                    //resolve({userId: updateArr.users,remainingScore:updateArr.remainingScore,userTurn:updateArr.userTurn,dartPoint:updateArr.dartPoint})
                    else
                        reject({message:"Unable to update  room"});
                    //resolve({users: reqObj.userId,remainingScore:calculatedScore})
                }

            });
    })
}
/*
   * Fetch game history
 */

 room.findHistory= function(userId){
     //{game_time: {$gte : 0},status:"closed"}
    //console.log(" fetch game history  ",condObj)
    return new Promise((resolve,reject) => {
        Room.find({game_time: {$gt : 0},status:"closed",'users.userId': userId}, {_id: 1, name:1, users:1, game_time:1,updated_at:1,colorName:1,raceName:1,roomCoin:1,created_at:1}).sort({"_id":-1}).limit(50).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });
    })
 }

 room.findHistory1 = function(userId){
     //{game_time: {$gte : 0},status:"closed"}
    //console.log(" fetch game history  ",condObj)
    return new Promise((resolve,reject) => {
        Room.find({game_time: {$gte : 0},status:"closed",'users.userId': userId}, {_id: 1, name:1, users:1, game_time:1,updated_at:1,colorName:1,raceName:1,roomCoin:1,created_at:1}).sort({"_id":-1}).limit(1).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });
    })
 }
 room.findHistory123 = function(userId){
     //{game_time: {$gte : 0},status:"closed"}
    //console.log(" fetch game history  ",condObj)
    return new Promise((resolve,reject) => {
    let userid=mongoose.Types.ObjectId(userId);
    console.log("pl0"+userid);

   Room.aggregate( [ { "$match" : 
    { game_time: {$gte : 0},
    'users.userId': userid,
    status : "closed"} 
    },{ 
      $project: 
      { 
         yearMonthDay: { $dateToString: { format: "%Y-%m-%d %H-%M:%S", date: "$updated_at" } },
        _id: 1,
        dateDifference: { $subtract: [ new Date(), "$updated_at" ] },  
        name:1, 
        "users":1,
        game_time:1,
        updated_at:1,
        colorName:1,
        raceName:1,
        roomCoin:1,
        created_at:1
    } },
    { "$sort" : { created_at : -1 } },
    { "$limit" : 50 }
     ] ).then(responses=> {   
      //console.log( JSON.stringify( responses, undefined, 2 ) );

       //console.log("responses1"+responses[0]);
        //Room.find({game_time: {$gte : 0},status:"closed",'users.userId': userId}, {_id: 1, name:1, users:1, game_time:1,updated_at:1,colorName:1,raceName:1,roomCoin:1,created_at:1}).sort({"created_at":-1}).limit(50).then(responses=> {
            return resolve(JSON.stringify( responses, undefined, 2 ) );
        }).catch(err => {
            return reject(err);
        });
    })
 }

room.findHistoryAdmin = function(userId){
    //console.log(" fetch game history  ",condObj)
    return new Promise((resolve,reject) => {
        Room.find({game_time: {$gte : 0},status:"closed"}, {_id: 1, name:1, users:1, game_time:1,updated_at:1}).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });
    })
}

//findHistorySpecific
 room.findHistorySpecific = function(userId){
    console.log("ok12");
     //{game_time: {$gte : 0},status:"closed"}
    console.log(" fetch game history  ",condObj)
    return new Promise((resolve,reject) => {        
        Room.find({game_time: {$gte : 0},
          status:"closed",
          'users.userId': userId},
           {_id: 1, name:1, users:1, 
            game_time:1,updated_at:1,
            colorName:1,raceName:1,
            roomCoin:1,
            created_at:1}).sort({"created_at":-1}).limit(50).then(responses=> {

            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });
    })
 }

module.exports =room;