/** Import Module*/
var underscore  = require('underscore');
var Room = require('../schema/Schema').roomModel; 
const client = require("../config/radisConfig.js");
const  roomPlayer      = require('../utils/RoomPlayerManage');
var room ={}
  //  client.HGETALL(['status','open'],function (err, roomdata) {
  //   console.log(roomdata);
    
  // });

// client.flushdb( function (err, succeeded) {
//     console.log(succeeded); // will be true if successfull
// });
//client.json_set('object','.','{"foo": "bar", "ans": 42}',function(err,succeeded) {  console.log(succeeded);  })


room.createRoom =function (reqObj) {
  return new Promise((resolve,reject) => {

     // console.log("  reqestObj",reqestObj)
      var users = [];
      client.SMEMBERS("room",function (err, roomlist) {
          //console.log(" roomlist ",roomlist)
          if(roomlist.length >= 1){
              client.HGETALL(roomlist[0],function (err, roomdata) {  
                  var status =(roomdata.totaluser == 7)?"closed":"open";       
                  let reqestObj = roomPlayer._playerCreation(reqObj.user_id,reqObj.name,reqObj.texture,roomdata.totaluser)   
                  users.push(reqestObj);      
                  client.HMSET(roomlist[0],[ 
                                            "status",status,
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
                      client.HINCRBY(roomlist[0],["totaluser",1],function (err, roomdata) {                        
                          if(err){
                              reject();
                          }else{
                              resolve({"name":roomlist[0],status:"open",userObj:reqestObj});  
                          }
                      });    
                  })
              })
          }else{
              var room = 'RM'+new Date().getTime();
              client.SADD("room",[room],function (err, addroom) {
              let reqestObj = roomPlayer._playerCreation(reqObj.user_id,reqObj.name,reqObj.texture,0)  
              users.push(reqestObj)               
                client.HMSET(room,[ "status","open",
                                    "totaluser", 1,
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
                      console.log("  err  ",err)
                      if(err){
                          reject();
                      }else{
                          resolve({"name":room,status:"open",userObj:reqestObj});
                      }
                  }) 
              });
          }
      });
  });  
  // body...
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
   // console.log("   store  room details  ",roomObj)
    return new Promise((resolve,reject) => {
        Room.create(roomObj).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        })
    })  
}

room.updateRoomDetails = function(roomObj){
    console.log("   store  room details  ",roomObj)
    return new Promise((resolve,reject) => {
        Room.create(roomObj).then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        })
    })  
}



module.exports =room;