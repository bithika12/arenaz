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
   User.updateToken({_id :user._id},{}).then((tokenDetails) => {
      user.set('accessToken', tokenDetails[0].accessToken)
      callback (null,user);
    }).catch(err => {
        callback (err,null);
    });

}
function updateLogIn(user,callback) {
    User.updateOne({_id: user._id}, {$set: {"loggedIn": 1}}).then(responses => {
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
    console.log("user.users"+JSON.stringify(user.users))
    ForgotPass.callEmailSendLogin(user.users).then(responses => {
      callback (null,user);
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
        console.log("nn")
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
                   res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                    "userId": result._id,
                    "userName": result.userName,
                    email: result.email,
                    score: result.score,
                    "accessToken": result.deviceDetails[0].accessToken,
                    "userCoin":result.startCoin,
                    "new_account_gift_coins":result.startCoin,
                    "userCup":3000
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
        console.log("nn");
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
                    checkValid,
                    sendMail

                ],
                function (err, result) {
                    if (result) {
                        console.log("result"+JSON.stringify(result))
                        let resuser=result.users;
                        let countryres=JSON.parse(result.country);
                        
                        let bannedStatus=countryres.filter((item)=>{
                            return item===cName
                        });
                         
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
                            "email_verify":result.email_verify,
                            "game_deactivation":result.game_deactivation,
                            "ip_verify":result.ip_verify,
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

