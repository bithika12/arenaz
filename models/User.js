 /* INCLUDE  PACKAGE */
const uuidv4 = require('uuid/v4');
/*Include Constants */
var constants = require("../config/constants");

/* INCLUDE UTILS  */
const timeManage  = require('../utils/TimeManager');
const password = require('../utils/PasswordManage');
/*Include model */
var User = require('../schema/Schema').userModel; 
var Role = require('../schema/Schema').roleModel; 

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
      reqObj.deviceDetails = [{accessToken :  uuidv4(), deviceId:"", deviceToken: "",status: "active" ,createdAt : timeManage.now(),updatedAt : timeManage.now()}];  
        User.create(reqObj).then(response=> {
              resolve(response)
        }).catch(err=>{
              reject(err);
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
        User.findOne({email: condObj.email},{deviceDetails:0,resetOtp:0}).
                populate({
            path: 'role',
             match: { slug: condObj.role},
            // Explicitly exclude `_id`, see http://bit.ly/2aEfTdB
           // select: 'name -_id',
            //options: { limit: 5 }
          }).then(responses=> {
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
        var  deviceDetails = [{accessToken : uuidv4()/*updateObj.accessToken*/ , deviceId:"", deviceToken: "",status: "active" ,createdAt : timeManage._now(),updatedAt : timeManage._now()}];     
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
                          $pull: { deviceDetails: { $elemMatch: { accessToken: condObj.accessToken } } }
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

module.exports= User;


