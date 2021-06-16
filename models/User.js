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
 let Coin = require('../schema/Schema').coinModel; 
 let Appversion=require('../schema/Schema').versionModel; 
 let Transaction  = require('../models/Transaction');

 let userCoin = require('../schema/Schema').userCoinModel;
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
//fetchFreeCoin
User.fetchFreeCoin =function(reqObj){
    return  new Promise((resolve,reject) => {
      console.log("ppp")
     
      Transaction.details().then((appList) => {
         let new_account_gift_coins=appList.new_account_gift_coins;
          console.log("new_account_gift_coins"+new_account_gift_coins);
          //reqObj.new_account_gift_coins=appList.new_account_gift_coins;
          resolve(appList);
      }).catch(fetchErr => {
                       reject(fetchErr);
                      });       
        
    });
}
//CREATE
User.createUser = function(reqObj,version){
    console.log("reqObj.new_account_gift_coins"+version.new_account_gift_coins);
      return new Promise((resolve,reject)=>{
      /*let msg="Welcome to Arena Z!\
In the Arena you can test your skills and wage a war against other players.\
It will take some time for each game to be fully tested and launched. You can enjoy the games we currently have available for all players. Before playing go to training and find out how the game is played.\
In order to fill your account with game coins click the “+” by the coins at the bottom or top of the page and purchase coins. You can also request to receive the coins you have available back for a small transaction fee.\
It is time to Pick & Play!\
Enjoy… \
Arena Z Team" ; */

 var msg="Welcome to Arena Z! \n";
msg+="In the Arena you can test your skills and wage a war against other players.We will launch new games in the Arena every 6 months. \n"

msg+="Before playing go to training and find out how the game is played.\
In order to fill your account with game coins click the “+” by the coins at the bottom or top of the page and follow the instructions on how to purchase coins. \n";

msg+="You can also request to receive the coins you have.  \n";

msg+="It is time to Pick & Play!  \n";
msg+="Enjoy… \n";
msg+="Arena Z Team";


      reqObj.password  =  password.hashPassword(reqObj.password);
      
       let startCoin = reqObj.userType =='regular-player'
              ? version.new_account_gift_coins
              : 0;
      /*let startCoin = reqObj.userType =='regular-player'
              ? 50
              : 0;*/

      /*let startCoin = reqObj.userType =='regular-player'
              ? 3000
              : 0;*/        
      reqObj.startCoin=startCoin;
      reqObj.userScore=3000;
      reqObj.cupNo=3000;
      reqObj.deviceDetails = [{accessToken :  uuidv4(), deviceId:"", deviceToken: "",status: "active" ,createdAt : timeManage.now(),updatedAt : timeManage.now()}];

             Role.findOne({ slug: reqObj.userType},{_id: 1,name:1,slug:1}).then(roledetails=> {
                 reqObj.roleId = roledetails._id;
              User.create(reqObj).then(response => {

                  Notification.createNotification({
                      //sent_by_user     : req.user_id ,
                      received_by_user : response._id,
                      subject          : "Welcome to Arena Z",
                      message          :  msg,
                      read_unread      : 0
                  }).then(function(notificationdetails){

                    ////insert in user coin table ///////////

                    //userCoin
                    let usercoins={
                      user_name:reqObj.userName,
                      type:"Deposit",
                      coins:version.new_account_gift_coins,
                      reference:"Welcome Gift"
                    }
                    userCoin.create(usercoins).then(userCoinresponse => {
                        resolve(response);
                     }).catch(err => {
                          reject(err);
                     })
                     ////usercoin
                      //resolve(response);
                  }).catch(err => {
                      reject(err);
                  });


              }).catch(err => {
                  reject(err);
              })
          })
   })
}


User.createUserOLd = function(reqObj){
      return new Promise((resolve,reject)=>{
      /*let msg="Welcome to Arena Z!\
In the Arena you can test your skills and wage a war against other players.\
It will take some time for each game to be fully tested and launched. You can enjoy the games we currently have available for all players. Before playing go to training and find out how the game is played.\
In order to fill your account with game coins click the “+” by the coins at the bottom or top of the page and purchase coins. You can also request to receive the coins you have available back for a small transaction fee.\
It is time to Pick & Play!\
Enjoy… \
Arena Z Team" ; */

   let msg="Welcome to Arena Z! \n"
msg+="In the Arena you can test your skills and wage a war against other players.\
It will take some time for each game to be fully tested and launched. You can enjoy the games we currently have available for all players. Before playing go to training and find out how the game is played.\
In order to fill your account with game coins click the “+” by the coins at the bottom or top of the page and purchase coins. You can also request to receive the coins you have available back for a small transaction fee.\
It is time to Pick & Play! \
Enjoy…  \
Arena Z Team" ; 
      reqObj.password  =  password.hashPassword(reqObj.password);
      let startCoin = reqObj.userType =='regular-player'
              ? 50
              : 0;

      /*let startCoin = reqObj.userType =='regular-player'
              ? 3000
              : 0;*/        
      reqObj.startCoin=startCoin;
      reqObj.userScore=3000;
      reqObj.cupNo=3000;
      reqObj.deviceDetails = [{accessToken :  uuidv4(), deviceId:"", deviceToken: "",status: "active" ,createdAt : timeManage.now(),updatedAt : timeManage.now()}];

             Role.findOne({ slug: reqObj.userType},{_id: 1,name:1,slug:1}).then(roledetails=> {
                 reqObj.roleId = roledetails._id;
              User.create(reqObj).then(response => {

                  Notification.createNotification({
                      //sent_by_user     : req.user_id ,
                      received_by_user : response._id,
                      subject          : "Welcome to Arena Z",
                      message          :  msg,
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
             console.log("user cup number"+responses.cupNo);
             console.log("user coin"+responses.startCoin);
             console.log("update cup"+updateObj.cupNo);
             console.log("update coin"+updateObj.startCoin);
             //let updatedCoin=parseInt(responses.startCoin)+parseInt(updateObj.startCoin);
             //update 09-06/////////
             let updatedCoin=parseInt(responses.startCoin)+parseInt(updateObj.startCoin)*2 -parseInt(updateObj.startCoin);
              //let updatedCoin=parseInt(responses.startCoin)+parseInt(updateObj.startCoin)*2;
             console.log("win coin"+updatedCoin);
             console.log("responses.total_no_win"+responses.total_no_win);
             //console.log("")
             let updatedCup=parseInt(responses.cupNo)+parseInt(updateObj.cupNo);
             let total_no_win=parseInt(responses.total_no_win)+parseInt(1);
             console.log("total_no_win12"+total_no_win);
             let userScore=parseInt(responses.userScore)+parseInt(updateObj.userScore);
             User.updateOne({_id:condObj.userId},{ $set : {startCoin:updatedCoin,cupNo:updatedCup,total_no_win:total_no_win,userScore:userScore} }).then(updatedResponses=> {
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
             console.log("responses.startCoin"+responses.startCoin);
             console.log("updateObj.startCoin"+updateObj.startCoin);
             let userScore=parseInt(responses.userScore)+parseInt(updateObj.userScore);
             let updatedCoin=responses.startCoin - updateObj.startCoin;
             let updatedCup=parseInt(responses.cupNo)-parseInt(updateObj.cupNo);
             if(updatedCup <0)
                updatedCup=0;

             if(updatedCoin <0)
                updatedCoin=0; 
             //let updatedCup=parseInt(responses.cupNo)+parseInt(updateObj.cupNo);
             /*if(updatedCoin ==0){
                 updatedCoin=500;
             }*/
             console.log("opponent cup"+updatedCup);
             console.log("opponent coin"+updatedCoin);
             User.updateOne({_id:condObj.userId},{ $set : {startCoin:updatedCoin,cupNo:updatedCup,userScore:userScore} }).then(updatedResponses=> {
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
       User.findOne({status:"active",$or: [
    {email: condObj.email},
    {userName: condObj.email}
   ]},{deviceDetails:0,resetOtp:0}).then(responses=> {
        //User.findOne({email: condObj.email},{deviceDetails:0,resetOtp:0}).then(responses=> {
              return resolve(responses);
        }).catch(err => {
              return reject(err);
        });
    });
}

User.findDetailsOld12 = function(condObj){
  console.log(" condObj",)
  return  new Promise((resolve,reject) => {
       User.findOne({$or: [
    {email: condObj.email},
    {userName: condObj.email}
   ]},{deviceDetails:0,resetOtp:0}).then(responses=> {
        //User.findOne({email: condObj.email},{deviceDetails:0,resetOtp:0}).then(responses=> {
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
 //findLeaderboard
 User.findLeaderboard = function(condObj){
     console.log(" condObj",)
     return  new Promise((resolve,reject) => {
         User.find({status: "active"},{cupNo:1,userName:1,
             colorName:{$elemMatch: {status: 1}},
             raceName:{$elemMatch: {status: 1}},
             dartName:{$elemMatch: {status: 1}},
             characterName:{$elemMatch: {status: 1}},
             countryName:1
         }).sort({ cupNo: -1 }).limit(50).then(responses=> {
             let chart=[];
             let colorName;
             let raceName;
             responses.map(function(entry,key) {
                 if(!entry.colorName.length){
                     colorName='';
                 }
                 else{
                     colorName=entry.colorName[0]['colorName'];
                 }
                 if(!entry.raceName.length){
                     raceName='';
                 }
                 else{
                     raceName=entry.colorName[0]['colorName'];
                 }
                chart.push({
                    userRank:key+1,
                    userName:entry.userName,
                    cupNumber:entry.cupNo,
                    colorName:colorName,
                    raceName:raceName,
                    countryName:entry.countryName

                });
             });
             return resolve(chart);
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
             {_id: 1,name:1,email:1,status:1,startCoin:1,cupNo:1,userName:1,firstName:1,lastName:1,
              deviceDetails: {$elemMatch: {accessToken: condObj.accessToken}},
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
         console.log("userId"+condObj.userId);
         User.findOne({_id:condObj.userId}, {_id: 1, userName:1, email:1, status:1, sockets:1}).then(responses=> {
             console.log("success for fetching socket");
             return resolve({_id :responses._id ,userName : responses.userName, socketId :  responses.sockets[responses.sockets.length -1]._id });
         }).catch(err => {
             console.log("user socket details not found while call game request");
             return reject({message:err});
         });
     });
 }
 //userStatusUpdate
 User.userStatusUpdate = function(condObj){
     return  new Promise((resolve,reject) => {
         console.log("condObj.userStatus"+condObj.userStatus);
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
              let total_win=parseInt(userDetails.total_no_win);

              //let total_win=parseInt(userDetails.total_no_win)+1;
              let incrementDetails ={ total_no_win : total_win}
              console.log("totalwin"+ incrementDetails);
              return resolve(true);
             /*User.updateOne({_id: condObj._id}, {$inc: incrementDetails}).then(responses => {
                 return resolve(responses);
             }).catch(err => {
                 return reject(err);
             })*/

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
         User.find({ userName: { $ne: "arena" } }).sort({cupNo:-1}).then(responses=> {
         //User.find().sort({userName:1}).then(responses=> {
         //User.find({status:"active"}).sort({userName:1}).then(responses=> {
             console.log("responses"+JSON.stringify(responses));
             console.log("responses"+responses.length);

             let totalArr=[];
             let noRoleArr=[];
             //return resolve(responses)
             responses.map(function(entry1,key) {
                 console.log("key"+key);
                // if (entry1.roleId) {
                  let start_coins=entry1.startCoin+"("+entry1.startCoin+")";
                 //Role.findOne({_id: entry1.roleId}, {_id: 1, name: 1, slug: 1}).then(roleResponse => {
                     
                   // console.log("roleResponse._id"+roleResponse._id);
                     totalArr.push({
                           roleId:entry1.roleId,
                         //roleId: roleResponse._id,
                         //roleId: roleResponse._id,
                        // roleName: roleResponse.slug,
                         //roleName: roleResponse.name,
                         userId:entry1.userId,
                         userName:entry1.userName,
                         firstName:entry1.firstName,
                         lastName:entry1.lastName,
                         email:entry1.email,
                         startCoin:start_coins,
                         //startCoin:entry1.startCoin,
                         userRank:key+1,
                         cupNumber:entry1.cupNo,
                         status:entry1.status,
                         ip:entry1.loginIp

                     });
                     if(key===responses.length-1){
                       console.log("totalArr.length"+totalArr.length);
                        return resolve(totalArr)
                     }
                 /*}).catch(roleErr => {

                     reject(roleErr);
                 });*/
                 //}
                 
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
                     //create user coin
                     let usercoins={
                      user_name:reqObj.userName,
                      type:"Deposit",
                      coins:reqObj.startCoin,
                      reference:"Welcome Gift"
                    }

                    console.log("user"+usercoins);

                    userCoin.create(usercoins).then(usercoinresponse=> {
                          resolve(response);
                       }).catch(usercoinerr => {
                        console.log("err"+usercoinerr)
                         reject(usercoinerr);
                       });
                    // resolve(response);
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

//dartRequestProfile
 //countryRequestProfile

 User.countryRequestProfile  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({email :condObj.userEmail },
             {_id: 1, userName:1, email:1, status:1, countryName: 1 }).then(userDetails=> {

             if(userDetails && userDetails.countryName.length > 0){
                 if(userDetails.countryName==updateObj.countryName) {
                     return resolve(userDetails);
                 }
                 else{
                     User.updateOne({email :condObj.userEmail},{$set :{ "countryName":updateObj.countryName}}).then(responses=> {
                         return resolve(responses);
                     }).catch(err => {
                         reject(err);
                     });
                 }
             }else{
                 User.updateOne({email :condObj.userEmail},{$set :{ "countryName":updateObj.countryName}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }

 User.languageRequestProfile  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({email :condObj.userEmail },
             {_id: 1, userName:1, email:1, status:1, languageName: 1 }).then(userDetails=> {

             if(userDetails && userDetails.languageName.length > 0){
                 if(userDetails.languageName==updateObj.languageName) {
                     return resolve(userDetails);
                 }
                 else{
                     User.updateOne({email :condObj.userEmail},{$set :{ "languageName":updateObj.languageName}}).then(responses=> {
                         return resolve(responses);
                     }).catch(err => {
                         reject(err);
                     });
                 }
             }else{
                 User.updateOne({email :condObj.userEmail},{$set :{ "languageName":updateObj.languageName}}).then(responses=> {
                     return resolve(responses);
                 }).catch(err => {
                     reject(err);
                 });
             }
         });
     });
 }
 User.characterRequestProfile  = function(condObj,updateObj){
     return  new Promise((resolve,reject) => {
         let currentDay = moment().format('YYYY-MM-DD');
         User.findOne({email :condObj.userEmail },
             {_id: 1, userName:1, email:1, status:1, characterName: {$elemMatch: {characterName: updateObj.characterName,status:1 } } }).then(userDetails=> {

             if(userDetails && userDetails.characterName.length > 0){

                 return resolve(userDetails);
             }else{

                 var  colors = [{characterName : updateObj.characterName,status:1,createdAt : currentDay,updatedAt : currentDay}];
                 User.updateOne({email :condObj.userEmail},{$set :{ "characterName":colors}}).then(responses=> {
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

 User.checkColorMod = function(condObj){
     return  new Promise((resolve,reject) => {
         User.findOne({email:condObj.email,status:"active"},
         //User.findOne({email:condObj.email},
             {/*_id: 1,name:1,email:1,status:1,userName:1,*/
                 countryName:1,
                 languageName:1,
                 startCoin:1,
                 cupNo:1,
                 colorName:{$elemMatch: {status: 1}},
                 raceName:{$elemMatch: {status: 1}},
                 dartName:{$elemMatch: {status: 1}},
                 characterName:{$elemMatch: {status: 1}},
             })
             .then(responses=> {
                 let totalArr=[];
                 let resObj={
                     countryName:responses.countryName,
                     languageName:responses.languageName,
                     userCoin:responses.startCoin,
                     userCup:responses.cupNo,
                     colorName: (!responses.colorName.length ? '' : responses.colorName[0]['colorName']),
                     raceName: (!responses.raceName.length ? '' : responses.raceName[0]['raceName']),
                     dartName: (!responses.dartName.length ? '' : responses.dartName[0]['dartName']),
                     characterId: (!responses.characterName.length ? '' : responses.characterName[0]['characterName'])
                 };
                 /*totalArr.push({
                     colorName: responses.colorName[0]['colorName'],
                     raceName: responses.raceName[0]['raceName'],
                     dartName: responses.dartName[0]['dartName'],
                     characterId: responses.characterName[0]['characterName']


                 });*/
                 /*responses.map(function(entry) {
                     totalArr.push({
                         colorName: entry.colorName[0]['colorName']


                     });
                 });*/
                 return resolve(resObj);
             }).catch(err => {
             return reject(err);
         });
     });
 }

 User.disableAccount  = function(condObj){
    return  new Promise((resolve,reject) => {
        //deleteOne
       User.deleteOne({"deviceDetails.accessToken":condObj.accessToken}
        ).then(responses=> {                

        //User.updateOne({"deviceDetails.accessToken":condObj.accessToken},{ $set : {status:"inactive"} }).then(responses=> {                
          return resolve(responses);
      }).catch(err => {
          return reject(err);
      });  
    });
}

User.detailsAdmin = function(condObj){
     return new Promise((resolve,reject)=>{
         User.find(condObj,{_id: 1,userName:1,email:1}).then(response=> {
             resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }

 //coinDetails
 User.coinDetails = function(condObj){
     console.log("po");
     return new Promise((resolve,reject)=>{
         Coin.find(condObj,{_id: 1,number:1}).then(response=> {
             console.log("coinres"+response);
             resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }
 //fetchVersion
 User.fetchVersion = function(){
     
     return new Promise((resolve,reject)=>{
          Appversion.find({status:"active"},{
            app_version:1,
            download_link:1,
            coin_price_usd : 1,
            wallet_api_link : 1,
            wallet_key : 1,
            api_expiration_time : 1,
            e_currency_price_api : 1,
            transaction_fee_withdrawl : 1,
            transaction_fee_deposit : 1,
            minimum_deposit : 1,
            minimum_withdrawl : 1,
            new_account_gift_coins:1,
            master_message:1,
            allow_mini_account_withdrawal:1,
            support_email:1,
            market_volatility:1,
            banned_country:1,
            email_verify:1,
            game_deactivation:1,
            ip_verify:1,
            auto_refill_coins:1
            //_id:0
          }).then(response=> {

         //Appversion.find({status:"active"},{app_version:1,download_link:1}).then(response=> {
             //console.log("versionres"+(response));
             resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }

 User.checkOnlineOrNot = function(condObj){
     return new Promise((resolve,reject)=>{
         User.find(condObj,{onlineStatus:1}).then(response=> {
             //console.log("response"+response.onlineStatus);
             resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }

User.findDetailsGame = function(condObj){
  console.log(" condObj",)
  return  new Promise((resolve,reject) => {
       User.findOne(condObj,{deviceDetails:0,resetOtp:0}).then(responses=> {
        //User.findOne({email: condObj.email},{deviceDetails:0,resetOtp:0}).then(responses=> {
              return resolve(responses);
        }).catch(err => {
              return reject(err);
        });
    });
}

User.detailsUserCoin = function(condObj){
     return new Promise((resolve,reject)=>{
        userCoin.find({},{user_name:1,email:1,coins:1,reference:1,type:1}).sort({user_name: 1}).then(response=> {

        // userCoin.find({},{_id: 1,user_name:1,email:1,coins:1,reference:1,type:1}).sort({email: 1, id: -1,user_name:1}).sort({ email: 'asc' }).then(response=> {

         //userCoin.find({},{_id: 1,user_name:1,email:1,coins:1,reference:1,type:1}).sort({ _id: -1 }).then(response=> {
             //console.log("response for user coin"+JSON.stringify(response))
             resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }
 //addUserCoin
 User.addUserCoin = function(reqObj){
     return new Promise((resolve,reject)=>{
          let updatedCoin;
          let usercoins={
                      user_name:reqObj.userName,
                      type:reqObj.type,
                      coins:reqObj.coin,
                      reference:reqObj.reference
                    }
         userCoin.create(usercoins).then(response=> {

          User.findOne({userName: reqObj.userName},{deviceDetails:0,resetOtp:0}).then(responses12=> {

           if(reqObj.type=="Withdrawal" || reqObj.type=="Withdrawl" || reqObj.type=="withdrawl") {
             updatedCoin=parseInt(responses12.startCoin)-parseInt(reqObj.coin);

           }
           else{
             updatedCoin=parseInt(responses12.startCoin)+parseInt(reqObj.coin);


           }
           if(updatedCoin <0){
              updatedCoin=0;
           }

             User.updateOne({userName:reqObj.userName},{ $set : {startCoin:updatedCoin} }).then(updatedResponses=> {
                 return resolve(updatedResponses);
             }).catch(updatedResponsesErr => {
                 return reject(updatedResponsesErr);
             });


          ////update user 

             }).catch(err=>{
             reject(err);
         })

             //resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }

 //addUserCoin
 User.addUserCoinWin = function(reqObj){
     return new Promise((resolve,reject)=>{
         
         userCoin.insertMany(reqObj).then(response=> {       

             resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }

 User.findDetailsGame12 = function(condObj){
  console.log(" condObj",)
  return  new Promise((resolve,reject) => {
       User.findOne({userName:condObj.userName},{startCoin:1,email:1}).then(responses=> {
        //User.findOne({email: condObj.email},{deviceDetails:0,resetOtp:0}).then(responses=> {
            console.log("responsesqq"+
              responses.email);

            
             

              let userObj={
                        balance:(!responses.startCoin) ? 0 : responses.startCoin,
                        user_name:(!condObj.result.user_name) ? '' : condObj.result.user_name,
                        user_email:(!responses.email) ? '' : responses.email,
                        coins:condObj.result.coins,
                        reference:condObj.result.reference,
                        type:condObj.result.type
                        
                        };
              return resolve(userObj);
        }).catch(err => {
              return reject(err);
        });
    });
}

//update coin transaction
User.updateUserCoinTransaction =function(condObj,updateObj){
     return  new Promise((resolve,reject) => {

         User.findOne({userName: condObj.userName},{deviceDetails:0,resetOtp:0}).then(responses=> {
             
             console.log("user coin"+responses.startCoin);
             
             console.log("update coin"+updateObj);
             //let updatedCoin=responses.startCoin+updateObj;
             //updatedCoin=parseInt(updatedCoin);
             let updatedCoin=parseInt(responses.startCoin)+parseInt(updateObj);
              //let updatedCoin=parseInt(responses.startCoin)+parseInt(updateObj.startCoin)*2;
             console.log("win coin"+typeof(updatedCoin));
             
             
             //let userScore=parseInt(responses.userScore)+parseInt(updateObj.userScore);
             User.updateOne({userName:condObj.userName},{ $set : {startCoin:updatedCoin} }).then(updatedResponses=> {
                 return resolve(updatedResponses);
             }).catch(updatedResponsesErr => {
                 return reject(updatedResponsesErr);
             });
         }).catch(err => {
             return reject(err);
         });

     });
 }

 //update coin transaction after Withdraw
User.updateUserCoinTransactionWithDraw =function(condObj,updateObj){
     return  new Promise((resolve,reject) => {

         User.findOne({userName: condObj.userName},{deviceDetails:0,resetOtp:0}).then(responses=> {
             
             let updatedCoin=parseInt(responses.startCoin)-parseInt(updateObj);             
              if(updatedCoin <0){
                updatedCoin=0;
              }
             User.updateOne({userName:condObj.userName},{ $set : {startCoin:updatedCoin} }).then(updatedResponses=> {
                condObj.total_amount = updatedCoin;
                condObj.coinstatus = 'Updated';
                 return resolve(condObj);
             }).catch(updatedResponsesErr => {
                 return reject(updatedResponsesErr);
             });
         }).catch(err => {
             return reject(err);
         });

     });
 }
 async function userUPdate(condObj,updatedCoin){
  return  new Promise(async (resolve,reject) => {
    User.updateOne({userName:condObj.userName},{ $set : {startCoin:updatedCoin} }).then(updatedResponses=> {
                 return resolve(updatedResponses);
             }).catch(updatedResponsesErr => {
                 return reject(updatedResponsesErr);
             });
           });
 }

 User.updateUserCoinTransactionAdmin =async function(condObj,updateObj){
     return  new Promise(async (resolve,reject) => {

         User.findOne({userName: condObj.userName},{deviceDetails:0,resetOtp:0}).then(responses=> {
             
             console.log("user coin"+responses.startCoin);
             
             console.log("update coin"+updateObj);
             //let updatedCoin=responses.startCoin+updateObj;
             //updatedCoin=parseInt(updatedCoin);
             let updatedCoin=parseInt(responses.startCoin)+parseInt(updateObj);
              //let updatedCoin=parseInt(responses.startCoin)+parseInt(updateObj.startCoin)*2;
             console.log("win coin"+typeof(updatedCoin));
             let updatedResponses=userUPdate(condObj,updatedCoin);
             
             //let userScore=parseInt(responses.userScore)+parseInt(updateObj.userScore);
             /*User.updateOne({userName:condObj.userName},{ $set : {startCoin:updatedCoin} }).then(updatedResponses=> {
                 return resolve(updatedResponses);
             }).catch(updatedResponsesErr => {
                 return reject(updatedResponsesErr);
             });*/
         }).catch(err => {
             return reject(err);
         });

     });
 }
module.exports= User;


