/**  Import Package**/
var async = require('async');
const appRoot = require('app-root-path');
/**  Import model **/
var User  = require('../models/User');
var Role  = require('../models/Role');
/* Import */
var constants = require("../config/constants");

/* UTILS PACKAGE*/
const response  = require('../utils/ResponseManeger');
//const jwtTokenManage   = require('../utils/JwtTokenManage');
const validateInput = require('../utils/ParamsValidation');

const password = require('../utils/PasswordManage');
const Joi = require('joi');
let Notification  = require(appRoot +'/models/Notification');
const CountryCodes = require('country-code-info');

let ForgotPass  = require(appRoot +'/models/ForgotPassword');
let randomstring = require("randomstring");

var Room  = require('../models/Room');
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
/*
 * This function is used for check email or username already exists or not
 * @params--object
 * output--object
 */

function checkUniqueEmailUserName(reqObj){
    return function (callback) {
        User.totalUser({email : reqObj.email,status:"active"}).then((totalUser) => {
        //User.totalUser({email : reqObj.email}).then((totalUser) => {
            if(totalUser == 0) {

                User.totalUser({userName : reqObj.userName,status:"active"}).then((totalUserList) => {

                //User.totalUser({userName : reqObj.userName}).then((totalUserList) => {
                    if(totalUserList == 0)
                        callback (null,reqObj);
                    else{
                        callback (constants.UNIQUIE_USERNAME,null);
                    }
                }).catch(userNameUniqueErr => {
                    callback (userNameUniqueErr,null);
                });

            }
            else{
                callback (constants.UNIQUIE_EMAIL,null);
            }
        }).catch(emailUniqueErr => {
            callback (emailUniqueErr,null);
        });
    }
}

function checkUniqueUserName(reqObj,callback){
    callback(null, reqObj);
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

function getUserCodeDetails(reqObj){
  return function(callback){
    User.findDetailsGame(reqObj).then((userDetails)=>{
       if(userDetails){
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



function createUserAdmin(reqObj,callback){
    User.createUserAdmin(reqObj).then((user) => {
        callback (null,user);
    }).catch(err => {
        callback (err,null);
    });
}
function createUser(reqObj,version,callback){
    User.createUser(reqObj,version).then((user) => {
        callback (null,user);
    }).catch(err => {
        callback (err,null);
    });
}
//fetchFreeCoin
function fetchFreeCoin(reqObj,callback){
    User.fetchFreeCoin(reqObj).then((user) => {
        callback (null,reqObj,user);
    }).catch(err => {
        callback (err,null);
    });
}
function createNotification(reqObj,callback){

    Notification.createNotification({
        //sent_by_user     : req.user_id ,
        received_by_user : reqobj._id,
        message          : "You are successfully created account",
        read_unread      : 0
    }).then(function(notificationdetails){
        callback (null,notificationdetails);
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
    //let verifyCode=randomstring.generate(7);
   User.updateToken({_id :user._id},{}).then((tokenDetails) => {
      user.set('accessToken', tokenDetails[0].accessToken)
      
      callback (null,user);
    }).catch(err => {
        callback (err,null);
    });

}

function updateVerification(user,callback){
    //let verifyCode=randomstring.generate(7);
   User.updateOne({_id: user._id}, {$set: {"emailVerified": "yes"}}).then(responses => {
        callback (null,user);
    }).catch(err => {
        callback (err,null);
    });

}
function updateLogIn(user,callback) {
    let verifyCode=Math.floor(100000 + Math.random() * 900000)
    user.verifyCode=verifyCode;
    user.set('verifyCode', verifyCode)

    User.updateOne({_id: user._id}, {$set: {"loggedIn": 1,"emailVerifiedCode":verifyCode,"onlineStatus":1}}).then(responses => {
        callback (null,user);
    }).catch(err => {
        callback (err,null);
    });
}
function checkValid(user,callback) {
    User.fetchVersion().then(responses => {
        //console.log("responses.banned_country"+responses[0].banned_country)
       // user.responses[0].banned_country=responses[0].banned_country;
      //user["banned_country"]=responses[0].banned_country;
      //user.key3="kyuu"
      //console.log("users"+user)
      let obj={
          users:user,
          country:responses[0].banned_country,
          email_verify:responses[0].email_verify,
          game_deactivation:(responses[0].game_deactivation==="Yes") ? 0 :1,
          ip_verify:(responses[0].ip_verify==="Yes") ? 0 :1,
          auto_refill_coins:responses[0].auto_refill_coins
      }

        callback (null,obj);
    }).catch(err => {
        callback (err,null);
    });
}
function sendMail(user,callback) {
    console.log("send mail"+JSON.stringify(user.users.get('verifyCode')))
   // if(user.users.emailVerified==="no"){

    setTimeout(function () {

       console.log('done'+user.users)
       
       //console.log("user.users"+JSON.stringify(user.users))
        ForgotPass.callEmailSendLogin(user.users.email,user.users.get('verifyCode')).then(responses => {
          console.log("email send done");
        }).catch(err => {
            console.log("error occured in sending mail")
        });
   }, 0)
   
   //}
    callback (null,user);

    /*console.log("user.users"+JSON.stringify(user.users))
    ForgotPass.callEmailSendLogin(user.users).then(responses => {
      callback (null,user);
    }).catch(err => {
        callback (err,null);
    });*/
}
//
function getUserSettingDetails(user) {
    return function(callback){
User.fetchVersion().then(responses => {
        console.log("responses.banned_country"+user.email)
       // user.responses[0].banned_country=responses[0].banned_country;
      //user["banned_country"]=responses[0].banned_country;
      //user.key3="kyuu"
      //console.log("users"+user)
      let obj={
          email:user.email,
          free_coin_incentive:responses[0].free_coin_incentive
          
      }
        callback (null,obj);
    }).catch(err => {
        console.log("err"+err)
        callback (err,null);
    });
}

}
//fetchUserCount
function fetchUserCount(user,callback) {
User.findDetails({email:user.email}).then(responses => {
      let countStatus=1;
      if(user.free_coin_incentive >0){
          countStatus=0;
      }
      /*if(user.free_coin_incentive >0 && responses.gameCount >=user.free_coin_incentive){
          countStatus=0;
      }*/      
      let obj={
          countStatus:countStatus,
          userGameCount:responses.gameCount,
          free_coin_incentive:user.free_coin_incentive
      }

        callback (null,obj);
    }).catch(err => {
        callback (err,null);
    });

}
function getUser(reqObj){
  return function(callback){
    User.findDetails({email:reqObj.email}).then((userDetails)=>{
       let obj={
         _id:userDetails._id
         //coin:reqObj.coin
       }
       callback (null,obj);
       
    }).catch(err=>{
         callback(err,null);
    })
  }
}
function fetchRequest(user,callback) {
Room.findRequest({userId:user._id/*,coin:user.coin*/}).then(responses => {
      
      /*if(user.free_coin_incentive >0 && responses.gameCount >=user.free_coin_incentive){
          countStatus=0;
      }*/   
      delete responses._id;   
      let obj={
          countStatus:responses,
          //userGameCount:responses.gameCount
      }

        callback (null,responses);
    }).catch(err => {
        callback (err,null);
    });

}



/*
   * This function is used for user registration
   * @params
 */
exports.registration= function(req,res) {

    const ipInfo = req.ipInfo;
    //console.log("pl"+ipInfo.country);
    //console.log("pl12"+JSON.stringify(ipInfo));
    if(!ipInfo.country){
        console.log("no country found")
    }
    else {
        let countryNameDetails = CountryCodes.findCountry({'gec': ipInfo.country});
        console.log(countryNameDetails.name);
    }
    //res.send(ipInfo);

    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required(),
        userName: Joi.string().min(3).trim().required(),
        password: Joi.string().min(8).regex(/^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])/).trim().required()
        //password with special character
        //password: Joi.string().min(8).regex(/^(?=.*\d)(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z]).{8,}$/).trim().required()
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = {
            status: constants.VALIDATION_ERROR,
            result: result.error.name,
            message: result.error.details[0].message.replace(new RegExp('"', "g"), '')
        };
        return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else {
    if (!req.body.email || !req.body.userName || !req.body.password) {
        //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
        return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
    }

    /*if(validateInput.password(req.body.password) == false){
        return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS,{},"Password format doesn't match"));
    }*/
    if (validateInput.email(req.body.email) == false) {
        //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS,{},"Email format doesn't match"));
        return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS, {}, "Invalid Email address. Please try again."));
    }
    /* if(validateInput.userName(req.body.userName) == false){
         return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS,{},"Username format doesn't match"));
    }*/


    var userObj = {
        email: req.body.email,
        password: req.body.password,
        userName: req.body.userName,
        userType: "regular-player",
        countryName:(!ipInfo.country) ? "" : ipInfo.country,
        loginIp:(!ipInfo.ip) ? "" : ipInfo.ip
        //loginIp:ipInfo
        //countryName:countryNameDetails.name
        //userType: "registered-game-user"
    }
    async.waterfall([
            //checkUnique(userObj),
            checkUniqueEmailUserName(userObj),
            //checkRole,
            fetchFreeCoin,
            createUser

        ],
        function (err, result) {
            if (result) {
                User.coinDetails({ status : "active"
                }).then((coinDetails) => {
                   //console.log("coinnumber"+coinDetails[0].number);
                   let ipStatus=0;
                   let verifyCode=Math.floor(100000 + Math.random() * 900000);
                      setTimeout(function () {
                               //console.log("user.users"+JSON.stringify(user.users))
                                ForgotPass.callEmailSendLogin(result.email,verifyCode).then(responses => {
                                  //console.log("email send done");
                                   User.updateOne({email: result.email}, {$set: {"emailVerifiedCode": verifyCode}}).then(responses => {
                                    //res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, result, 'Email verified successfully !!'));
                                    console.log("mail send in registration")

                                    }).catch(err => {
                                     console.log("error occured in mail send")
                                });

                                }).catch(err => {
                                    console.log("error occured in sending mail")
                                });
                      }, 0)


                   res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                    "userId": result._id,
                    "userName": result.userName,
                    email: result.email,
                    score: result.score,
                    "accessToken": result.deviceDetails[0].accessToken,
                    "userCoin":result.startCoin,
                    "new_account_gift_coins":result.startCoin,
                    "userCup":3000,
                    "ip_verify":ipStatus
                    //"userCoin":3000,
                    //"userCup":0
                }, 'You have successfully registered. You will be logged in.')); 
          
                }).catch(err => {
                  res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Something went Wrong!!"));

                });    
                //res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS,{"userId" : result._id,"userName":result.userName,email:result.email,score:result.score,"accessToken":result.deviceDetails[0].accessToken}, 'User register successfully !!'));
                /*res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                    "userId": result._id,
                    "userName": result.userName,
                    email: result.email,
                    score: result.score,
                    "accessToken": result.deviceDetails[0].accessToken,
                    "userCoin":500,
                    "userCup":0
                }, 'You have successfully registered. You will be logged in.'));*/
            } else {
                if (err == constants.UNIQUIE_EMAIL)
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_EMAIL,{}," Email Already exist"));
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_EMAIL, {}, "Email address entered already exists. Please use forgot password to login."));
                    res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_EMAIL, {}, "Email address entered already exists. Please use forgot password to login."));
                 else if(err == constants.UNIQUIE_USERNAME)
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_USERNAME, {}, "The username you entered already exists. Please re-enter a new one."));
                    res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_USERNAME, {}, "The username you entered already exists. Please re-enter a new one."));
                else
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Something went Wrong!!"));
                    res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Something went Wrong!!"));
            }
        }
    );
}
}


exports.login= function(req,res) {
   /*
     * Joi is used for validation
    */
    const ipInfo = req.ipInfo;
    let cName;
    //console.log("pl"+ipInfo.country);
    //console.log("pl12"+JSON.stringify(ipInfo));
    if(!ipInfo.country){
        console.log("no country found");
        ipInfo.country="IN";
        cName="India";
    }
    else {
        let countryNameDetails = CountryCodes.findCountry({'gec': ipInfo.country});
        cName=countryNameDetails.name;
        console.log(countryNameDetails.name);
    }
    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required(),
        //email: Joi.string().max(254).regex(/^(?:[A-Z\d][A-Z\d_-]{5,10}|[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4})$/i).trim().required(),
        password: Joi.string().trim().required()
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = { status: 422, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
        return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else {

        if (!req.body.email || !req.body.password) {
            //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
            return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "The email address and password you entered is incorrect. Please try again."));
        }
        //User.findDetails({email:req.body.email}).then((userDetails)=> {
            /*if (userDetails.loggedIn == 1) {
                res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, {}, "User already login."));

            }*/
            //else{
            var userObj = {email: req.body.email, password: req.body.password}
            async.waterfall([
                    getUserDetails(userObj),
                    updateToken,
                    updateLogIn,
                    checkValid
                    //sendMail

                ],
                function (err, result) {
                    if (result) {
                        let loginIpStatus="no";
                        let ipStatus=1;
                        console.log("result"+JSON.stringify(result))
                        let resuser=result.users;
                        let countryres=JSON.parse(result.country);
                        
                        let bannedStatus=countryres.filter((item)=>{
                            return item===cName
                        });
                        let loginIp=(!ipInfo.ip) ? "" : ipInfo.ip;
                        console.log("loginIp"+loginIp);
                        console.log("resuser.loginIp"+resuser.loginIp)
                        if(loginIp !=resuser.loginIp){
                            console.log("ip changed")
                            loginIpStatus="yes";
                        }
                        console.log("result.ip_verify"+result.ip_verify)
                        if((result.ip_verify ===0 && loginIpStatus ==="yes")
                            ||

                            (resuser.emailVerified==="no" && result.email_verify==="Yes" )
                            ){
                               ipStatus=0;

                              setTimeout(function () {

                               
                               
                               //console.log("user.users"+JSON.stringify(user.users))
                                ForgotPass.callEmailSendLogin(resuser.email,resuser.verifyCode).then(responses => {
                                  console.log("email send done");
                                }).catch(err => {
                                    console.log("error occured in sending mail")
                                });
                             }, 0)

                        }

                       

                        res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                            "userId": resuser._id,
                            "userName": resuser.userName,
                            "userEmailVerified": resuser.emailVerified,
                            email: resuser.email,
                            score: resuser.score,
                            "accessToken": resuser.get('accessToken'),
                            "userCoin":resuser.startCoin,
                            "userCup":resuser.cupNo,                                                        
                            "bannedStatus":bannedStatus.length,
                            //"email_verify":result.email_verify,
                            "game_deactivation":result.game_deactivation,
                            "ip_verify":ipStatus,
                            "auto_refill_coins":result.auto_refill_coins,
                        }, 'User login successfully !!'));

                        /*res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                            "userId": result._id,
                            "userName": result.userName,
                            email: result.email,
                            score: result.score,
                            "accessToken": result.get('accessToken'),
                            "userCoin":result.startCoin,
                            "userCup":result.cupNo
                        }, 'User login successfully !!'));*/
                    } else {
                        //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Invalid password!!"));
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "The email address and password you entered is incorrect. Please try again."));

                    }
                });
            //}
       // })

       }
}




exports.socialLogin= function(req,res){
    var userObject ={};   
    if(!req.body.socialLoginType || !req.body.uniqueLoginId){
         return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
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
              res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS,{"_id" : result._id,"name":result.name,email:result.email,"score":result.score,"accessToken":result.get('accessToken')}, 'User login successfully !!'));
          }else res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS,{},"Something went Wrong!!"));
          
      }
    );
}


exports.logout = function (req,res) {
    User.removeToken({accessToken: req.header("access_token"), _id : res.userData._id}).then(function (result) {
    		if(result) {
                User.updateOne({_id: res.userData._id}, {$set: {"loggedIn": 0}}).then(responses => {
                    res.status(constants.HTTP_OK_STATUS).send({"status": constants.SUCCESS_STATUS, "result":{ }, "message": "Logout successfully"});

                }).catch(err => {
                    res.status(constants.BAD_REQUEST_STATUS).send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});

                });
    		    //res.status(constants.HTTP_OK_STATUS).send({"status": constants.SUCCESS_STATUS, "result":{ }, "message": "Logout successfully"});
    		}else{
    		    res.status(constants.BAD_REQUEST_STATUS).send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});
    		}
   }).catch(err => {
    	    res.status(constants.BAD_REQUEST_STATUS).send({"status":constants.ERROR_STATUS,"result":err,"message":"Something went Wrong!!"});
   }); 
};


function getUserDetailsMod(reqObj){
    return function(callback){
        //chk email or username///

        User.findDetails({email:reqObj.email}).then((userDetails)=>{
            if(password.comparePasswordSync(reqObj.password, userDetails.password)){
                if(userDetails.loggedIn ==1)
                    callback("err",null)
                else
                  callback (null,userDetails);
            }else{
                callback("err",null)
            }
        }).catch(err=>{
            callback("err",null);
        })
    }
}

//deleteAccount
exports.deleteAccount = function (req,res) {
    User.disableAccount({accessToken: req.header("access_token")}).then(function (result) {
        if(result) {
            res.status(constants.HTTP_OK_STATUS).send({"status": constants.SUCCESS_STATUS, "result":{ }, "message": "Account deleted successfully"});
        }else{
            res.status(constants.BAD_REQUEST_STATUS).send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});
        }
    }).catch(err => {
        res.status(constants.BAD_REQUEST_STATUS).send({"status":constants.ERROR_STATUS,"result":err,"message":"Something went Wrong!!"});
    });
};
//verifyCode findDetailsGame
exports.verifyCode = function (req,res) {
    const ipInfo = req.ipInfo;

    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required(),
        verifyCode: Joi.string().trim().required()
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = { status: 422, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
        return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else {

        if (!req.body.email || !req.body.verifyCode) {
            //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
            return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "The email address and verifyCode you entered is incorrect. Please try again."));
        }
        
            var userObj = {email: req.body.email, emailVerifiedCode: req.body.verifyCode}
            async.waterfall([
                    getUserCodeDetails(userObj),
                    updateVerification
                    

                ],
                function (err, result) {
                    if (result) {
                        let loginIp=(!ipInfo.ip) ? "" : ipInfo.ip
                        User.updateOne({email: req.body.email}, {$set: {"loginIp": loginIp}}).then(responses => {
                        res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, result, 'Email verified successfully !!'));
                        }).catch(err => {
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "The email address and verify code you entered is incorrect. Please try again."));

                    });
                       } else {
                        //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Invalid password!!"));
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "The email address and verify code you entered is incorrect. Please try again."));

                    }
                });
            //}
       // })

       }
   
};


exports.resendMail = function (req,res) {
    let verifyCode=Math.floor(100000 + Math.random() * 900000);
     setTimeout(function () {

       
       //console.log("user.users"+JSON.stringify(user.users))
        ForgotPass.callEmailSendLogin(req.body.email,verifyCode).then(responses => {
          console.log("email send done");
        }).catch(err => {
            console.log("error occured in sending mail")
        });
   }, 0)
    
    User.updateOne({email: req.body.email}, {$set: {"emailVerifiedCode":verifyCode}}).then(responses => {
      res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, responses, 'Email send successfully !!'));

    }).catch(err => {
       res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Email sending error. Please try again."));
    });

};

//freeCoinStatus
exports.freeCoinStatus = function (req,res) {
     const ipInfo = req.ipInfo;

    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required(),
        
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = { status: 422, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
        return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else {

        if (!req.body.email) {
            //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
            return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "The email address and verifyCode you entered is incorrect. Please try again."));
        }
        
            var userObj = {email: req.body.email}
            async.waterfall([
                    getUserSettingDetails(userObj),
                    fetchUserCount
                    

                ],
                function (err, result) {
                    if (result) {
                        
                        res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, result, 'User Count send successfully !!'));
                        
                       } else {
                        //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Invalid password!!"));
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Something went wrong. Please try again."));

                    }
                });
            //}
       // })

       }
};

//requestCount
exports.requestCount = function (req,res) {     

    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required()
        //coin: Joi.required()
        
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = { status: 422, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
        return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else {

        if (!req.body.email) {
            //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
            return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "The email address and verifyCode you entered is incorrect. Please try again."));
        }
        
            var userObj = {email: req.body.email/*,coin:req.body.coin*/}
            async.waterfall([
                    getUser(userObj),
                    fetchRequest
                    

                ],
                function (err, result) {
                    if (result) {
                        
                        res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, result, 'User Count send successfully !!'));
                        
                       } else {
                        //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Invalid password!!"));
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Something went wrong. Please try again."));

                    }
                });
            //}
       // })

       }
};

exports.addUserFreeCoins = function (req,res) {
    console.log("ok")
    let formData=req.body;
    User.addUserFreeCoin(formData).then((coindetails)=>{
        res.send(response.generate(constants.SUCCESS_STATUS,
            coindetails, 'User Coin List added successfully !!'));
    }).catch(err=>{
        res.send(response.error(constants.ERROR_STATUS,err,"Unable to add coin"));
    })
};