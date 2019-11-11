/**  Import Package**/
var async = require('async');
/**  Import model **/
var User  = require('../models/User');
var Role  = require('../models/Role');
/* Import */
var constants = require("../config/constants");

/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response  = require('../utils/ResponseManeger');
//const jwtTokenManage   = require('../utils/JwtTokenManage');

const password = require('../utils/PasswordManage');

/* Async function*/


function checkUnique(reqObj){
    return function (callback) {
       User.totalUser({email : reqObj.email}).then((totalUser) => {
          if(totalUser == 0)
             callback (null,reqObj);
          else{
             callback (constants.UNIQUIE_EMAIL,null);
          } 
        }).catch(err => {
            callback (err,null);
       });
    }
}


function getUserDetails(reqObj){
  return function(callback){
    User.findDetails({email:reqObj.email}).then((userDetails)=>{
       if(password.comparePasswordSync(reqObj.password, userDetails.password)){
            callback (null,userDetails);
       }else{
            callback(err,null)
       }
    }).catch(err=>{
         callback(err,null);
    })
  }
}

function checkRole(reqObj,callback){
    Role.details({ slug : reqObj.userType
}).then((roleDetails) => {
        reqObj.role = roleDetails._id
        delete reqObj.userType
;
        callback (null,reqObj);
    }).catch(err => {
        callback (err,null);
    });
}


function createUser(reqObj,callback){
    User.createUser(reqObj).then((user) => {
        callback (null,user);
    }).catch(err => {
        callback (err,null);
    });
}



function createUpdateUser(reqObj){
    return function (callback) {
       User.createUpdateUser(reqObj,{}).then((user) => {
            callback (null,user);
        }).catch(err => {
            callback (err,null);
       });
    }
}

// function genarateToken(reqObj,user,callback){
//     jwtTokenManage.generateToken({_id: user._id,facebook_id: user.facebook_id ,name :user.name}).then((tokenDetails) => {        // 
//         callback (null, user,tokenDetails);
//     }).catch(err => {
//         callback (err,null);
//     });
// }
  
function updateToken(user,callback){
   User.updateToken({_id :user._id},{}).then((tokenDetails) => {
      user.set('accessToken', tokenDetails[0].accessToken)
      callback (null,user);
    }).catch(err => {
        callback (err,null);
    });

}


exports.registration= function(req,res){
    if(!req.body.email || !req.body.userName || !req.body.password  ){
         return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }

    if(validateInput.password(req.body.password) == false){
        return res.send(response.error(constants.ERROR_STATUS,{},"Password format doesn't match"));
    }
    if(validateInput.email(req.body.email) == false){
        return res.send(response.error(constants.ERROR_STATUS,{},"Email format doesn't match"));
    }
    if(validateInput.userName(req.body.userName) == false){
        return res.send(response.error(constants.ERROR_STATUS,{},"Username format doesn't match"));
   }



    var userObj  ={email: req.body.email,password: req.body.password, userName:req.body.userName, userType:"registered-game-user" }
    async.waterfall([
         checkUnique(userObj),
         //checkRole,
         createUser
      ],
      function (err, result) {
          if(result){
             res.send(response.generate(constants.SUCCESS_STATUS,{"_id" : result._id,"userName":result.userName,email:result.email,score:result.score,"accessToken":result.deviceDetails[0].accessToken}, 'User register successfully !!'));
          }else{
            if( err == constants.UNIQUIE_EMAIL)
               res.send(response.error(constants.UNIQUIE_EMAIL,{}," Email Already exist"));
            else 
               res.send(response.error(constants.ERROR_STATUS,err,"Something went Wrong!!"));
          }
      }
    );
}

exports.login= function(req,res){
    if(!req.body.email || !req.body.password  ){
         return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    var userObj  ={email: req.body.email,password: req.body.password}
    async.waterfall([
         getUserDetails(userObj),
         updateToken
      ],
      function (err, result) {
          if(result){
              res.send(response.generate(constants.SUCCESS_STATUS,{"_id":result._id,"userName":result.userName,email:result.email,score:result.score,"accessToken":result.get('accessToken')}, 'User login successfully !!'));
          }else{
              res.send(response.error(constants.ERROR_STATUS,err,"Invalid password!!"));
          }
    });
}

exports.socialLogin= function(req,res){
    var userObject ={};   
    if(!req.body.socialLoginType || !req.body.uniqueLoginId){
         return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }

    var userObject ={ socialLoginType : req.body.socialLoginType , uniqueLoginId : req.body.uniqueLoginId , userType:"registered-game-user" }; 
    
    if(req.body.email)
        userObject.email = req.body.email
    if(req.body.name)
        userObject.name = req.body.name  
    async.waterfall([
       createUpdateUser(userObject),
       updateToken
    ],function (err, result) {
          if(result){
              res.send(response.generate(constants.SUCCESS_STATUS,{"_id" : result._id,"name":result.name,email:result.email,"score":result.score,"accessToken":result.get('accessToken')}, 'User login successfully !!'));
          }else res.send(response.error(constants.ERROR_STATUS,{},"Something went Wrong!!"));
          
      }
    );
}


exports.logout = function (req,res) {
    User.removeToken({accessToken: req.header("access-token"), _id : res.userData._id}).then(function (result) {
    		if(result) {
    		    res.send({"status": constants.SUCCESS_STATUS, "result":{ }, "message": "Logout successfully"});
    		}else{
    		    res.send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});
    		}
   }).catch(err => {
    	    res.send({"status":constants.ERROR_STATUS,"result":err,"message":"Something went Wrong!!"});
   }); 
};




