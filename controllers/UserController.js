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
const Joi = require('joi');

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
        User.totalUser({email : reqObj.email}).then((totalUser) => {
            if(totalUser == 0) {

                User.totalUser({userName : reqObj.userName}).then((totalUserList) => {
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

/*
   * This function is used for user registration
   * @params
 */
exports.registration= function(req,res) {

    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required(),
        userName: Joi.string().min(3).trim().required(),
        password: Joi.string().max(8).regex(/^(?=.*\d)(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z]).{8,}$/).trim().required()
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
        return res.send(data);
    }
    else {
    if (!req.body.email || !req.body.userName || !req.body.password) {
        //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
        return res.send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
    }

    /*if(validateInput.password(req.body.password) == false){
        return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS,{},"Password format doesn't match"));
    }*/
    if (validateInput.email(req.body.email) == false) {
        //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS,{},"Email format doesn't match"));
        return res.send(response.error(constants.ERROR_STATUS, {}, "Invalid Email address. Please try again."));
    }
    /* if(validateInput.userName(req.body.userName) == false){
         return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS,{},"Username format doesn't match"));
    }*/


    var userObj = {
        email: req.body.email,
        password: req.body.password,
        userName: req.body.userName,
        userType: "registered-game-user"
    }
    async.waterfall([
            //checkUnique(userObj),
            checkUniqueEmailUserName(userObj),
            //checkRole,
            createUser
        ],
        function (err, result) {
            if (result) {
                //res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS,{"userId" : result._id,"userName":result.userName,email:result.email,score:result.score,"accessToken":result.deviceDetails[0].accessToken}, 'User register successfully !!'));
                res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                    "userId": result._id,
                    "userName": result.userName,
                    email: result.email,
                    score: result.score,
                    "accessToken": result.deviceDetails[0].accessToken
                }, 'You have successfully registered. You will be logged in.'));
            } else {
                if (err == constants.UNIQUIE_EMAIL)
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_EMAIL,{}," Email Already exist"));
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_EMAIL, {}, "Email address entered already exists. Please use forgot password to login."));
                    res.send(response.error(constants.UNIQUIE_EMAIL, {}, "Email address entered already exists. Please use forgot password to login."));
                 else if(err == constants.UNIQUIE_USERNAME)
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_USERNAME, {}, "The username you entered already exists. Please re-enter a new one."));
                    res.send(response.error(constants.UNIQUIE_USERNAME, {}, "The username you entered already exists. Please re-enter a new one."));
                else
                    //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Something went Wrong!!"));
                    res.send(response.error(constants.ERROR_STATUS, err, "Something went Wrong!!"));
            }
        }
    );
}
}

/*
 * This function is used for user login
 * @params---email,password
 */
exports.login= function(req,res) {
   /*
     * Joi is used for validation
    */
    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required(),
        password: Joi.string().trim().required()
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = { status: 422, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
        return res.send(data);
    }
    else{

    if (!req.body.email || !req.body.password) {
        //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
        return res.send(response.error(constants.PARAMMISSING_STATUS, {}, "The email address and password you entered is incorrect. Please try again."));
    }
    var userObj = {email: req.body.email, password: req.body.password}
    async.waterfall([
            getUserDetails(userObj),
            updateToken
        ],
        function (err, result) {
            if (result) {
                res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                    "userId": result._id,
                    "userName": result.userName,
                    email: result.email,
                    score: result.score,
                    "accessToken": result.get('accessToken')
                }, 'User login successfully !!'));
            } else {
                //res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Invalid password!!"));
                res.send(response.error(constants.ERROR_STATUS, err, "The email address and password you entered is incorrect. Please try again."));

            }
        });
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
    User.removeToken({accessToken: req.header("access-token"), _id : res.userData._id}).then(function (result) {
    		if(result) {
    		    res.status(constants.HTTP_OK_STATUS).send({"status": constants.SUCCESS_STATUS, "result":{ }, "message": "Logout successfully"});
    		}else{
    		    res.status(constants.BAD_REQUEST_STATUS).send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});
    		}
   }).catch(err => {
    	    res.status(constants.BAD_REQUEST_STATUS).send({"status":constants.ERROR_STATUS,"result":err,"message":"Something went Wrong!!"});
   }); 
};




