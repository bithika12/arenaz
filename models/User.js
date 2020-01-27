 /* INCLUDE  PACKAGE */
const uuidv4 = require('uuid/v4');
const appRoot = require('app-root-path');
/*Include Constants */
var constants = require("../config/constants");
 const time  = require('../utils/TimeManager');

/* INCLUDE UTILS  */
const timeManage  = require('../utils/TimeManager');
const password = require('../utils/PasswordManage');
/*Include model */
var User = require('../schema/Schema').userModel; 
var Role = require('../schema/Schema').roleModel;
const moment = require('moment');

 var Room = require('../schema/Schema').roomModel;
 let Notification  = require(appRoot +'/models/Notification');


 /** TOTAL USER **/
User.totalUser =function(condObj){
    return  new Promise((resolve,reject) => {
        User.countDocuments(condObj).then(responses=> {
             return resolve(responses);
        }).catch(err => {
             return reject(err);
        });
    });
}

User.countUser = function(condObj){
  return  new Promise((resolve,reject) => {
      User.count(condObj, function(err, count){
           return resolve(count);
      }).catch(err => {
            return reject(err);
      });
  });
}

//CREATE
User.createUser = function(reqObj){
      return new Promise((resolve,reject)=>{
      reqObj.password  =  password.hashPassword(reqObj.password);
      let startCoin = reqObj.userType =='regular-player'
              ? 500
              : 0;
      reqObj.startCoin=startCoin;
      reqObj.deviceDetails = [{accessToken :  uuidv4(), deviceId:"", deviceToken: "",status: "active" ,createdAt : timeManage.now(),updatedAt : timeManage.now()}];

             Role.findOne({ slug: reqObj.userType},{_id: 1,name:1,slug:1}).then(roledetails=> {
                 reqObj.roleId = roledetails._id;
              User.create(reqObj).then(response => {

                  Notification.createNotification({
                      //sent_by_user     : req.user_id ,
                      received_by_user : response._id,
                      message          : "You are successfully created account",
                      read_unread      : 0
                  }).then(function(notificationdetails){
                      resolve(response);
                  }).catch(err => {
                      reject(err);
                  });


              }).catch(err => {
                  reject(err);
              })
          })
   })
}

/** USER CREATE **/
User.createUpdateUser =function(condObj){
    return  new Promise((resolve,reject) => {
        var matchCond =  { socialLogin: { $elemMatch: { uniqueLoginId: condObj.uniqueLoginId, "loginBy" : condObj.socialLoginType } } }
        var updateObj ={ name : condObj.name};
        if(condObj.email){
           matchCond       = { $or:[{email : condObj.email},matchCond]}
           updateObj.email = updateObj.email
        }
        updateSocialObj =[{ uniqueLoginId: condObj.uniqueLoginId , loginBy : condObj.socialLoginType  }] 
        User.findOne(matchCond,{_id: 1,name:1,email:1,score:1,status:1,socialLogin: {$elemMatch: {loginBy: condObj.type }}}).then(responses=> {
            if(responses){           
              if(responses.socialLogin.length == 0){
                   User.updateOne({ _id :responses._id},{$push :{ "socialLogin":updateSocialObj}}).then(updateresponse=> {
                      return resolve(responses);
                  });
              }else{
                 return resolve(responses);
              }   
            }else{ 
                 updateObj.socialLogin =  updateSocialObj;
                 //Role.findOne({ slug: condObj.userType},{_id: 1,name:1,slug:1}).then(roledetails=> {
                   // updateObj.role = roledetails._id;
                    User.create(updateObj).then(responses=> {
                        return resolve(responses);
                    }).catch(err => {
                        return reject(err);
                    });
                //})    
            }
       })        
    });
}

/** USER UPDATE **/
User.updateUserDetails =function(condObj,updateObj){
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

//user status update
// updateUserCoin
 User.updateUserCoin =function(condObj,updateObj){
     return  new Promise((resolve,reject) => {

         User.findOne({_id: condObj.userId},{deviceDetails:0,resetOtp:0}).then(responses=> {
             let updatedCoin=responses.startCoin + updateObj.startCoin;
             let updatedCup=responses.cupNo + updateObj.cupNo;
             let total_no_win=responses.total_no_win+1;
             User.updateOne({_id:condObj.userId},{ $set : {startCoin:updatedCoin,cupNo:updatedCup,total_no_win:total_no_win} }).then(updatedResponses=> {
                 return resolve(updatedResponses);
             }).catch(updatedResponsesErr => {
                 return reject(updatedResponsesErr);
             });
         }).catch(err => {
             return reject(err);
         });

     });
 }

 //UPDATE COIN FOR OPPONENT
 User.updateUserCoinOpponent =function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         User.findOne({_id: condObj.userId},{deviceDetails:0,resetOtp:0}).then(responses=> {
             let updatedCoin=responses.startCoin - updateObj.startCoin;
             let updatedCup=responses.cupNo+updateObj.cupNo;
             /*if(updatedCoin ==0){
                 updatedCoin=500;
             }*/
             User.updateOne({_id:condObj.userId},{ $set : {startCoin:updatedCoin,cupNo:updatedCup} }).then(updatedResponses=> {
                 return resolve(updatedResponses);
             }).catch(updatedResponsesErr => {
                 return reject(updatedResponsesErr);
             });
         }).catch(err => {
             return reject(err);
         });

     });
 }
User.listing = function(condObj){
  return  new Promise((resolve,reject) => {
        User.find(condObj,{id: 1,name: 1,total_kill : 1}).limit(20).sort({ total_kill: -1 }).then(responses=> {
              return resolve(responses);
        }).catch(err => {
              return reject(err);
        });
    });
}


User.findDetails = function(condObj){
  console.log(" condObj",)
  return  new Promise((resolve,reject) => {
        User.findOne({email: condObj.email},{deviceDetails:0,resetOtp:0}).then(responses=> {
              return resolve(responses);
        }).catch(err => {
              return reject(err);
        });
    });
}
 User.findDetailsAdmin = function(condObj){
     return  new Promise((resolve,reject) => {
         User.findOne({email: condObj.email,roleId:condObj.roleId,status:"active"},{password:1,firstName:1,email:1,"deviceDetails.accessToken":1,resetOtp:1}).then(responses=> {
             return resolve(responses);
         }).catch(err => {
             return reject(err);
         });
     });
 }
 User.findDetailsByEmail = function(condObj){
     console.log(" condObj",)
     return  new Promise((resolve,reject) => {
         User.findOne({email: condObj.email}).then(responses=> {
             return resolve(responses);
         }).catch(err => {
             return reject(err);
         });
     });
 }


/*******   CHECK USER TOKEN   *******/

User.checkUserToken = function(condObj){
    return  new Promise((resolve,reject) => {
        User.findOne({"deviceDetails.accessToken":condObj.accessToken},
                     {_id: 1,name:1,email:1,status:1,deviceDetails: {$elemMatch: {accessToken: condObj.accessToken}}})
        .then(responses=> {
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });
    });
}

 User.checkUserTokenMod = function(condObj){
     return  new Promise((resolve,reject) => {
         User.findOne({"deviceDetails.accessToken":condObj.accessToken},
             {_id: 1,name:1,email:1,status:1,userName:1,deviceDetails: {$elemMatch: {accessToken: condObj.accessToken}},
                 colorName:{$elemMatch: {status: 1}},
                 raceName:{$elemMatch: {status: 1}},
                 dartName:{$elemMatch: {status: 1}}})
             .then(responses=> {
                 return resolve(responses);
             }).catch(err => {
             return reject(err);
         });
     });
 }
 User.getUserSocketDetails = function(condObj){
     return  new Promise((resolve,reject) => {
         User.findOne({_id:condObj.userId}, {_id: 1, userName:1, email:1, status:1, sockets:1}).then(responses=> {
             return resolve({_id :responses._id ,userName : responses.userName, socketId :  responses.sockets[responses.sockets.length -1]._id });
         }).catch(err => {
             return reject({message:err});
         });
     });
 }
 //userStatusUpdate
 User.userStatusUpdate = function(condObj){
     return  new Promise((resolve,reject) => {
         User.updateOne({_id:condObj.userId},{ $set : {onlineStatus:condObj.userStatus} }).then(updatedResponses=> {
             return resolve(updatedResponses);
         }).catch(updatedResponsesErr => {
             //console.log(updatedResponsesErr);
             return reject(updatedResponsesErr);
         });
     });
 }
/**UPDATE DEVICE **/

User.updateDeviceToken  = function(condObj){
   return  new Promise((resolve,reject) => {
        var  deviceDetail = [{accessToken : uuidv4(), deviceId:"", deviceToken: "",status: "active" ,createdAt : moment(new Date()),updatedAt:moment(new Date())}];
        User.update({ _id :condObj._id ,"deviceDetails.status": "active"  },{  $set:{ "deviceDetails.$[].status":"inactive"}}).then(responses=> {
            User.update({ _id :condObj._id},{$push :{ "deviceDetails":deviceDetail}}).then(responses=> {
                return resolve(deviceDetail);            
            }).catch(err => { return reject(err); });        
        }).catch(err => { return reject(err); });
    });
}

User.updateToken  = function(condObj,updateObj){
    return  new Promise((resolve,reject) => {
        var  deviceDetails = [{accessToken : uuidv4(), deviceId:"", deviceToken: "",status: "active" ,createdAt : timeManage.now(),updatedAt : timeManage.now()}];     
        User.updateOne({ _id :condObj._id},{$push :{ "deviceDetails":deviceDetails}}).then(responses=> {
             return resolve(deviceDetails);      
        }).catch(err => { return reject(err); });
    });
}

/* USER LOGOUT */

User.removeToken  = function(condObj){
  return  new Promise((resolve,reject) => {
        User.updateMany({ "deviceDetails.accessToken":condObj.accessToken},
                        {
                          $set : { onlineStatus :"0"},
                          $pull: { deviceDetails: { /*$elemMatch: { */accessToken: condObj.accessToken /*}*/ } }
                        },
                        { multi: true }).then(responses=> {
            return resolve(responses);            
        }).catch(err => {
             return reject(err);
        });  
    });
}

//RESET PASSWORD

  User.updateResetToken  = function(condObj,updateObj){
    return  new Promise((resolve,reject) => {       
        var  resetOtp = [{status: "active" , token :uuidv4(), otp : (Math.floor(Math.random() * 900000) + 100000),createdAt : timeManage._now(),updatedAt: timeManage._now() }];     
        User.update({ _id :condObj._id ,"resetOtp.status": "active"  },{  $set:{ "resetOtp.$[].status":"inactive"}}).then(responses=> {
            User.updateOne({ _id :condObj._id},{$push :{ "resetOtp":resetOtp}}).then(responses=> {
                return resolve(resetOtp);      
            }).catch(err => { return reject(err); });
        }).catch(err => { return reject(err); });   
    });
  }
User.checkValidOtp = function(reqObj){
    return  new Promise((resolve,reject) => {
        var condObj = {resetOtp: {$elemMatch: { token:reqObj.token,status:"active" }}}
        if(reqObj.otp){
            condObj.resetOtp.$elemMatch.otp =  reqObj.otp 
        }        
        console.log(" condObj  :  ",condObj);
        User.findOne(condObj,{_id: 1,name:1,email:1,status:1,resetOtp: {$elemMatch: {token: reqObj.token}}}).then(responses=> {
            //console.log(responses);
            return resolve(responses);
        }).catch(err => {
            return reject(err);
        });
    });
}
User.resetPassword = function(condObj,updateObj){
    return  new Promise((resolve,reject) => {
        console.log(" condObj :",condObj)
        var hashPassword  =  password.hashPassword(updateObj.password);            
        User.updateOne({ _id : condObj._id},{  $set:{ "resetOtp.$[].status":"inactive", "password" : hashPassword}}).then(responses=> {
            return resolve(responses);
        }).catch(err => {
           return reject(err);
        });
    });
}

/*
  * This function is used for fetch email with username
  * If email not found it throws error
  * @params -reqObj
  * output---object
 */
 User.findUser   =   function(reqObj){
     return  new Promise((resolve,reject) => {
         User.findOne(reqObj,function(err,responses){
             if (err) { return reject(err) }
             else {
                 if (responses){
                     if (!responses.email) {
                         reject({status: constants.NOT_FOUND_ERR, message: 'No data found'});
                     } else {
                         resolve(responses);
                     }
              }
                 else{
                     reject({status: constants.NOT_FOUND_ERR, message: 'No data found'});
                 }

             }
         });
         console.log(reqObj);
     });
 };


 /** SOCKET FUNCTION**/
 User.updateSocketDetails  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         User.findOne({/*"sockets.socketId":condObj.socketId*/_id :condObj.userId },
             {_id: 1, userName:1, email:1, status:1, sockets: {$elemMatch: {socketId: updateObj.socketId } } }).then(userDetails=> {

             if(userDetails && userDetails.sockets.length > 0){
                 return resolve(userDetails);
             }else{
                 var  sockets = [{socketId : updateObj.socketId,createdAt : time.now(),updatedAt : time.now()}];
                 User.updateOne({ _id :condObj.userId},{$push :{ "sockets":sockets}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }

 User.colorRequest  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({/*"sockets.socketId":condObj.socketId*/_id :condObj.userId },
             {_id: 1, userName:1, email:1, status:1, colorName: {$elemMatch: {colorName: updateObj.colorName,status:1 } } }).then(userDetails=> {

             if(userDetails && userDetails.colorName.length > 0){
                 return resolve(userDetails);
                 /*var  colors = [{colorName : updateObj.colorName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({ _id :condObj.userId ,"colorName.status": "1"  },{  $set:{ "colorName.$[].status":"0"}}).then(responses=> {
                     User.updateOne({ _id :condObj.userId},{$push :{ "colorName":colors}}).then(responses=> {
                         return resolve(userDetails);
                     }).catch(err => { return reject(err); });
                 }).catch(err => { return reject(err); });*/
             }else{
                 var  colors = [{colorName : updateObj.colorName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({ _id :condObj.userId},{$set :{ "colorName":colors}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }



 User.nameRequest  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({/*"sockets.socketId":condObj.socketId*/_id :condObj.userId },
             {_id: 1, userName:1, email:1, status:1, dartName: {$elemMatch: {dartName: updateObj.dartName,status:1 } } }).then(userDetails=> {

             if(userDetails && userDetails.dartName.length > 0){
                 //update this to inactive
                 //return resolve(userDetails);
                 /*var  colors = [{raceName : updateObj.raceName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({ _id :condObj.userId ,"raceName.status": "1"  },{  $set:{ "raceName.$[].status":"0"}}).then(responses=> {
                     User.updateOne({ _id :condObj.userId},{$push :{ "raceName":colors}}).then(responses=> {
                         return resolve(userDetails);
                     }).catch(err => { return reject(err); });
                 }).catch(err => { return reject(err); });*/

                 return resolve(userDetails);
             }else{

                 var  colors = [{dartName : updateObj.dartName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({ _id :condObj.userId},{$set :{ "dartName":colors}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }
 User.sageRequest  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({/*"sockets.socketId":condObj.socketId*/_id :condObj.userId },
             {_id: 1, userName:1, email:1, status:1, raceName: {$elemMatch: {raceName: updateObj.raceName,status:1 } } }).then(userDetails=> {

             if(userDetails && userDetails.raceName.length > 0){
                 //update this to inactive
                 //return resolve(userDetails);
                 /*var  colors = [{raceName : updateObj.raceName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({ _id :condObj.userId ,"raceName.status": "1"  },{  $set:{ "raceName.$[].status":"0"}}).then(responses=> {
                     User.updateOne({ _id :condObj.userId},{$push :{ "raceName":colors}}).then(responses=> {
                         return resolve(userDetails);
                     }).catch(err => { return reject(err); });
                 }).catch(err => { return reject(err); });*/

                 return resolve(userDetails);
             }else{

                 var  colors = [{raceName : updateObj.raceName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({ _id :condObj.userId},{$set :{ "raceName":colors}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }

 User.fetchColor  = function(roomName){
     return  new Promise((resolve,reject) => {

         Room.aggregate([
             { "$unwind": { "path": "$users", "preserveNullAndEmptyArrays": true }},
             { "$lookup": {
                     "from": "users",
                     "localField": "users.userId",
                     "foreignField": "_id",
                     "as": "users.userDetail"
                 }},

             {
                 $match:{
                     "name": roomName

                 }
             },

             { "$group": {
                     "_id": "$_id",
                     "roomname": { "$first": "$name" },
                     "users": { "$addToSet": "$users" }

                 }},

             {
                 "$project": {
                     // "_id": 1,
                     "roomname":1,
                     "users.userDetail.userName": 1,
                     "users.userDetail._id": 1,
                     "users.userDetail.colorName.colorName": 1,
                     "users.userDetail.raceName.raceName": 1,
                     //"campaign.client.username": 1
                 }
             }

         ])
             .then(userDetails=> {
              resolve(userDetails);
         }).catch(err => {
             return reject(err);
         });


     });
 }

 User.fetchColorMod  = function(roomName,userArr){
     return  new Promise((resolve,reject) => {
         let totalArr=[];
         userArr.forEach(function (val, key) {

             console.log(val);
             //totalArr.push(roomName);
             User.findOne({_id :val.userId },
                 {_id: 1, userName:1, email:1, status:1, raceName: 1,colorName: 1}).then(userDetails=> {
                     if(userDetails.raceName[0]['raceName']){
                     totalArr.push(userDetails.raceName[0]['raceName']);
                     }
                     else if(userDetails.colorName[0]['colorName']){
                         totalArr.push(userDetails.colorName[0]['colorName']);
                     }
                     else if(userDetails.userName){
                         totalArr.push(userDetails.userName);
                     }

                    // console.log(totalArr)

             });
         });

     });
 }

 /** USER UPDATE **/
 User.updatePointDetails =function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         /*if(condObj.type =="AI"){
             return resolve({});
         }else{*/
             let incrementDetails ={ total_no_win : updateObj.total_no_win}
          User.findOne({_id :condObj._id },
             {_id: 1, userName:1, email:1, status:1,total_no_win:1, dartName: {$elemMatch: {dartName: updateObj.dartName,status:1 } } }).then(userDetails=> {
              let total_win=userDetails.total_no_win+1;
              let incrementDetails ={ total_no_win : total_win}
             User.updateOne({_id: condObj._id}, {$inc: incrementDetails}).then(responses => {
                 return resolve(responses);
             }).catch(err => {
                 return reject(err);
             })

         });

        // }

     });
 }

 /*
   * This function is used for fetch users list
   * output---object
  */
 User.findUserListAdmin   =   function(){
     return  new Promise((resolve,reject) => {
         User.find().sort({userName:1}).then(responses=> {
         //User.find({status:"active"}).sort({userName:1}).then(responses=> {
             let totalArr=[];
             let noRoleArr=[];
             responses.map(function(entry1,key) {
                 if (entry1.roleId) {
                 Role.findOne({_id: entry1.roleId}, {_id: 1, name: 1, slug: 1}).then(roleResponse => {
                     totalArr.push({

                         roleId: roleResponse._id,
                         //roleId: roleResponse._id,
                         roleName: roleResponse.slug,
                         //roleName: roleResponse.name,
                         userId:entry1.userId,
                         userName:entry1.userName,
                         firstName:entry1.firstName,
                         lastName:entry1.lastName,
                         email:entry1.email,
                         startCoin:entry1.startCoin

                     });
                     if(key==responses.length-1)
                     return resolve(totalArr)
                 }).catch(roleErr => {
                     reject(roleErr);
                 });
                 }
             })
             //return resolve(totalArr);
         }).catch(err => {
             return reject(err);
         });
     });
 };

 //add user by admin
 User.createUserAdmin = function(reqObj){
     return new Promise((resolve,reject)=>{
         reqObj.password  =  password.hashPassword(reqObj.password);
         //reqObj.startCoin = reqObj.coinNumber;

         reqObj.deviceDetails = [{accessToken :  uuidv4(), deviceId:"", deviceToken: "",status: "active" ,createdAt : timeManage.now(),updatedAt : timeManage.now()}];

         //Role.findOne({ slug: reqObj.userType},{_id: 1,name:1,slug:1}).then(roledetails=> {
             //reqObj.roleId = roledetails._id;
             User.create(reqObj).then(response => {
                 Notification.createNotification({
                     //sent_by_user     : req.user_id ,
                     received_by_user : response._id,
                     message          : "You are successfully created account",
                     read_unread      : 0
                 }).then(function(notificationdetails){
                     resolve(response);
                 }).catch(err => {
                     reject(err);
                 });


             }).catch(err => {
                 reject(err);
             })
         //})
     })
 }
 //fetch color with email

 User.colorRequestProfile  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({email :condObj.userEmail },
             {_id: 1, userName:1, email:1, status:1, colorName: {$elemMatch: {colorName: updateObj.colorName,status:1 } } }).then(userDetails=> {

             if(userDetails && userDetails.colorName.length > 0){
                 return resolve(userDetails);
             }else{
                 var  colors = [{colorName : updateObj.colorName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({email :condObj.userEmail},{$set :{ "colorName":colors}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }



 User.nameRequestProfile  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({email :condObj.userEmail },
             {_id: 1, userName:1, email:1, status:1, dartName: {$elemMatch: {dartName: updateObj.dartName,status:1 } } }).then(userDetails=> {

             if(userDetails && userDetails.dartName.length > 0){

                 return resolve(userDetails);
             }else{

                 var  colors = [{dartName : updateObj.dartName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({email :condObj.userEmail},{$set :{ "dartName":colors}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }
 User.sageRequestProfile  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({email :condObj.userEmail },
             {_id: 1, userName:1, email:1, status:1, raceName: {$elemMatch: {raceName: updateObj.raceName,status:1 } } }).then(userDetails=> {

             if(userDetails && userDetails.raceName.length > 0){

                 return resolve(userDetails);
             }else{

                 var  colors = [{raceName : updateObj.raceName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({email :condObj.userEmail},{$set :{ "raceName":colors}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }
 //online status update
 User.updateUserOnlineStatus =function(condObj){
     return  new Promise((resolve,reject) => {
         User.updateOne({_id:condObj.userId},{ $set : {onlineStatus:1} }).then(updatedResponses=> {
             return resolve(updatedResponses);
         }).catch(updatedResponsesErr => {
             return reject(updatedResponsesErr);
         });

     });
 }
module.exports= User;


