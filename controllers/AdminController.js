/**  Import Package**/
var async = require('async');
/* Import */
var constants = require("../config/constants");
var mongoose = require('mongoose');
/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response  = require('../utils/ResponseManeger');
//const jwtTokenManage   = require('../utils/JwtTokenManage');

const password = require('../utils/PasswordManage');
const Joi = require('joi');
/**  Import model **/
var User  = require('../models/User');
var Transaction  = require('../models/Transaction');
var Role  = require('../models/Role');
const appRoot = require('app-root-path');
const { updateMail,updateTransaction,addMail,addTransaction,updateGameAdmin,addMatch,fetchMatches,fetchUserList,updateRoomAdmin,
    fetchHistoryAdmin,userValidChkAdmin,fetchCoin,addCoin,updateCoinAdmin,fetchMail,transactionList,updateTransactionStatusDelete,findTransactionListAdmin,deleteTransaction,editTransaction} = require(appRoot +'/models/FetchHistory');
const UserController  = require('../controllers/UserController');
const moment = require('moment');

// Role.createUser().then((details)=>{

// })
/* Async function*/


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

function getUserDetails(reqObj){
  return function(callback){
      let roleId;
      Role.findOne({ slug: "admin"},{_id: 1,name:1,slug:1}).then(roledetails=> {
          roleId = roledetails._id+"";
          User.findDetailsAdmin({email:reqObj.email,roleId:roleId}).then((userdetails)=>{
              if(password.comparePasswordSync(reqObj.password, userdetails.password)){
                  callback (null,userdetails);
              }else{
                  callback(err,null)
              }
          }).catch(err=>{
              callback(err,null);
          })
      }).catch(err=>{
          callback(err,null);
      })

  }
}

function createUserAdmin(reqObj,callback){
    User.createUserAdmin(reqObj).then((user) => {
        callback (null,user);
    }).catch(err => {
        callback (err,null);
    });
}

function updateToken(user,callback){
    console.log(" user",user)
    User.updateToken({_id :user._id},{}).then((tokendetails) => {
      user.set('accessToken', tokendetails[0].accessToken)
      callback (null,user);
    }).catch(err => {
        callback (err,null);
    });

}


exports.login= function(req,res){
    if(!req.body.email || !req.body.password  ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }

    var userObj  ={email: req.body.email,password: req.body.password,role :"admin"}
    async.waterfall([
            getUserDetails(userObj),
            updateToken
        ],
        function (err, result) {
            if(result){
                res.send(response.generate(constants.SUCCESS_STATUS,
                    {"_id":result._id,
                        "name":result.firstName,email:result.email,
                        "access_token":result.get('accessToken')}, 'User login successfully !!'));
            }else{
                res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
            }
        });
}


exports.logout = function (req,res) {
    User.removeToken({access_token: req.header("access_token")}).then(function (result) {
    		if(result) {
    		    res.send({"status": constants.SUCCESS_STATUS, "result":{ }, "message": "Logout successfully"});
    		}else{
    		    res.send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});
    		}
   }).catch(err => {
    	    res.send({"status":constants.ERROR_STATUS,"result":err,"message":"Something went Wrong!!"});
   }); 
};

exports.userList = function (req,res) {
    User.findUserListAdmin().then((userdetails)=>{
        res.send(response.generate(constants.SUCCESS_STATUS,
            userdetails, 'User List fetched successfully !!'));
    }).catch(err=>{
        res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch user list"));
    })
};
//fetch role list
exports.getRole = function (req,res) {
    Role.detailsAdmin({status:"active"}).then((roledetails)=>{
        res.send(response.generate(constants.SUCCESS_STATUS,
            roledetails, 'Role List fetched successfully !!'));
    }).catch(err=>{
        res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch role list"));
    })
};
 //fetch game list
exports.getGameList = function (req,res) {
    if (!req.body.userEmail) {
        return res.send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
    }
    else {
    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return fetchHistoryAdmin(validResponse);
        })

        .then(resp => {
            res.status(constants.HTTP_OK_STATUS).send({
                status: constants.SUCCESS_STATUS,
                result: resp,
                message: "Game history fetched successfully."
            })
        })
        .catch(err => {
            res.status(constants.API_ERROR).send(err);
        });
   }
};

exports.addUser= function(req,res) {
     const ipInfo = req.ipInfo;
    // console.log("ipInfo"+JSON.stringify(ipInfo));
    let schema = Joi.object().keys({
        email: Joi.string().max(254).trim().required(),
        userName: Joi.string().min(3).trim().required(),
        password: Joi.string().required(),

        //password: Joi.string().min(8).regex(/^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])/).trim().required(),
        roleId: Joi.string().trim().required(),
        coinNumber: Joi.number().required(),
        firstName:Joi.string().trim().required(),
        lastName:Joi.string().trim().required()
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
        return res.status(constants.HTTP_OK_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing! or invalid password"));

        //return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else {
        if (!req.body.email || !req.body.userName || !req.body.password) {
            //return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
            return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
        }
        if (validateInput.email(req.body.email) == false) {
            return res.status(constants.BAD_REQUEST_STATUS).send(response.error(constants.ERROR_STATUS, {}, "Invalid Email address. Please try again."));
        }


        var userObj = {
            email: req.body.email,
            password: req.body.password,
            userName: req.body.userName,
            roleId: mongoose.Types.ObjectId(req.body.roleId),
            startCoin:req.body.coinNumber,
            firstName:req.body.firstName,
            lastName:req.body.lastName,
            countryName:ipInfo.country,
            loginIp:ipInfo.ip,
            cupNo:3000,
            userScore : 3000
            //userType: "registered-game-user"
        }
        async.waterfall([
                //checkUnique(userObj),
                checkUniqueEmailUserName(userObj),
                //checkRole,
                createUserAdmin

            ],
            function (err, result) {
                if (result) {
                    //res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS,{"userId" : result._id,"userName":result.userName,email:result.email,score:result.score,"accessToken":result.deviceDetails[0].accessToken}, 'User register successfully !!'));
                    res.status(constants.HTTP_OK_STATUS).send(response.generate(constants.SUCCESS_STATUS, {
                        "userId": result._id,
                        "userName": result.userName,
                        email: result.email,
                        //score: result.score,
                        "accessToken": result.deviceDetails[0].accessToken
                    }, 'You have successfully registered. You will be logged in.'));
                } else {
                    if (err == constants.UNIQUIE_EMAIL)
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_EMAIL, {}, "Email address entered already exists. Please use forgot password to login."));
                    else if(err == constants.UNIQUIE_USERNAME)
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.UNIQUIE_USERNAME, {}, "The username you entered already exists. Please re-enter a new one."));
                    else
                        res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(response.error(constants.ERROR_STATUS, err, "Something went Wrong!!"));
                }
            }
        );
    }
}

exports.getCoinList = function (req,res) {
    if (!req.body.userEmail) {
        return res.send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
    }
    else {
        userValidChkAdmin(req.body.userEmail)
            .then(validResponse => {
                return fetchCoin(validResponse);
            })

            .then(resp => {
                res.status(constants.HTTP_OK_STATUS).send({
                    status: constants.SUCCESS_STATUS,
                    result: resp,
                    message: "Coin list fetched successfully."
                })
            })
            .catch(err => {
                res.status(constants.API_ERROR).send(err);
            });
    }
};

//add coin
exports.addCoin= function(req,res) {

    let schema = Joi.object().keys({
        userEmail: Joi.string().max(254).trim().required(),
        coinNumber: Joi.number().required()
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
        var userObj = {
            number: req.body.coinNumber
        }
         userValidChkAdmin(req.body.userEmail)
            .then(validResponse => {
                return addCoin(userObj);
            })
            .then(resp=>{
                res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Coin added ."})
            })
            .catch(err=>{
                res.status(constants.API_ERROR).send(err);
            });
    }
}

//disable room
exports.disableRoom= function(req,res) {

    if(!req.body.roomName ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={status:"inactive"};

    userValidChkAdmin(res.userData.email)
        .then(validResponse => {
            return updateRoomAdmin({name: req.body.roomName},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Room deleted ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}
 //disable coin
 //deleteCoin
 exports.deleteCoin= function(req,res) {

    if(!req.body.coinId || !req.body.userEmail ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={status:"inactive"};

    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return updateCoinAdmin({_id: mongoose.Types.ObjectId(req.body.coinId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Coin deleted ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}
 //modify coin
 //editCoin

 exports.editCoin= function(req,res) {

    if(!req.body.coinId || !req.body.userEmail || !req.body.coinNumber){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={number:req.body.coinNumber};

     userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return updateCoinAdmin({_id: mongoose.Types.ObjectId(req.body.coinId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Coin number modified ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}
//fetch online users

 exports.fetchActiveUser= function(req,res) {

    if(!req.body.userEmail){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return fetchUserList(req.body.userEmail);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            console.log(resp);
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,result:resp,message:"Active user fetched ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}
//get game list
exports.getMatchesList = function (req,res) {
    if (!req.body.userEmail) {
        return res.send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
    }
    else {
        userValidChkAdmin(req.body.userEmail)
            .then(validResponse => {
                return fetchMatches(validResponse);
            })

            .then(resp => {
                res.status(constants.HTTP_OK_STATUS).send({
                    status: constants.SUCCESS_STATUS,
                    result: resp,
                    message: "Game list fetched successfully."
                })
            })
            .catch(err => {
                res.status(constants.API_ERROR).send(err);
            });
    }
};
//addMatches
exports.addMatches= function(req,res) {

    let schema = Joi.object().keys({
        userEmail: Joi.string().max(254).trim().required(),
        name: Joi.string().required(),
        score:Joi.string().required(),
        details:Joi.string().required(),
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
        var userObj = {
            name: req.body.name,
            score:req.body.score,
            details:req.body.details,
        }
        userValidChkAdmin(req.body.userEmail)
            .then(validResponse => {
                return addMatch(userObj);
            })
            .then(resp=>{
                res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Game added ."})
            })
            .catch(err=>{
                res.status(constants.API_ERROR).send(err);
            });
    }
}

exports.editMatches= function(req,res) {

    if(!req.body.name || !req.body.userEmail || !req.body.score || !req.body.details || !req.body.gameId){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={name:req.body.name,score:req.body.score,details:req.body.details};

    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return updateGameAdmin({_id: mongoose.Types.ObjectId(req.body.gameId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Game modified ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}
//deleteMatch
exports.deleteMatch= function(req,res) {

    if(!req.body.gameId || !req.body.userEmail ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={status:"inactive"};

    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return updateGameAdmin({_id: mongoose.Types.ObjectId(req.body.gameId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Game deleted ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}
//getMailList

exports.getMailList = function (req,res) {
    if (!req.body.userEmail) {
        return res.send(response.error(constants.PARAMMISSING_STATUS, {}, "Parameter Missing!"));
    }
    else {
        userValidChkAdmin(req.body.userEmail)
            .then(validResponse => {
                return fetchMail(validResponse);
            })

            .then(resp => {
               //let new_time = moment(responses.created_at).format('YYYY-MM-DD HH:mm:ss');
                 //console.log("resp"+JSON.stringify(resp));
                 let notifyObj={};
                 let notifyArr=[];
                 resp.forEach(function(val,key){
                    console.log("val"+val);
                    let new_time = moment(val.created_at).format('YYYY-MM-DD HH:mm:ss');
                    notifyObj={
                        read_unread:val.read_unread,
                        _id:val._id,
                        received_by_user:val.received_by_user,
                        message:val.message,
                        created_at:new_time

                    } 
                    notifyArr.push(notifyObj);

                    if(key==resp.length-1){

                         res.status(constants.HTTP_OK_STATUS).send({
                    status: constants.SUCCESS_STATUS,
                    result: notifyArr,
                    message: "Mail list fetched successfully."
                })

                    }


                 })
               /* res.status(constants.HTTP_OK_STATUS).send({
                    status: constants.SUCCESS_STATUS,
                    result: resp,
                    message: "Mail list fetched successfully."
                })*/
            })
            .catch(err => {
                res.status(constants.API_ERROR).send(err);
            });
    }
};

exports.addMail= function(req,res) {

    let schema = Joi.object().keys({
        userEmail: Joi.string().max(254).trim().required(),
        //notificationId: Joi.string().required(),
        message:Joi.string().required(),
        receivedUserId:Joi.string().required(),
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
        var userObj = {
                       received_by_user : mongoose.Types.ObjectId(req.body.receivedUserId),
                        subject          : req.body.message,
                        message          : req.body.message,
                        read_unread      : 0
        }
        userValidChkAdmin(req.body.userEmail)
            .then(validResponse => {
                return addMail(userObj);
            })
            .then(resp=>{
                res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Notification added ."})
            })
            .catch(err=>{
                res.status(constants.API_ERROR).send(err);
            });
    }
}

exports.editMail= function(req,res) {

    if(!req.body.userEmail || !req.body.message || !req.body.receivedUserId || !req.body.notificationId){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={message:req.body.message,received_by_user:mongoose.Types.ObjectId(req.body.receivedUserId)};

    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return updateMail({_id: mongoose.Types.ObjectId(req.body.notificationId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Notification modified ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}


exports.deleteMail= function(req,res) {

    if(!req.body.notificationId || !req.body.userEmail ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={status:"inactive"};

    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return updateMail({_id: mongoose.Types.ObjectId(req.body.notificationId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Notification deleted ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}

exports.getUsers = function (req,res) {
    User.detailsAdmin({status:"active"}).then((roledetails)=>{
        res.send(response.generate(constants.SUCCESS_STATUS,
            roledetails, 'User List fetched successfully !!'));
    }).catch(err=>{
        res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch role list"));
    })
};
exports.getUserCoins =  (req,res)=> {
         let userObj={};
         let userArr=[];
        return new Promise(function (resolve, reject) {
           User.detailsUserCoin().then((coindetails)=>{
               console.log("ok");
               let promiseArr = [];
               coindetails.forEach(function (val, key) {
                        console.log("val"+val);
                        //let bal=User.findDetailsGame12({userName:val.user_name});
                        //console.log("bal"+JSON.stringify(bal));
                         
                    
                        //userArr.push(userObj); 
                        promiseArr.push(User.findDetailsGame12({userName:val.user_name,result:val}),
                        );


                    });
                    return Promise.all(promiseArr).then(function (resArr) {
                        console.log("resArr"+JSON.stringify(resArr));


                        if (resArr.length) {
                            //res.send(resArr);
                              res.send(response.generate(constants.SUCCESS_STATUS,
                          resArr, 'User Coin List fetched successfully !!'));
                            
                        }

                    }).catch(function (err) {
                        reject(err);
                    });                 
                 

            }).catch(err=>{
             res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch coin list"));
           })

        }); 
        
               
   
};
//getUserCoins
exports.getUserCoinsrun = function (req,res) {
    User.detailsUserCoin().then((coindetails)=>{
         let userObj={};
         let userArr=[];
        coindetails.forEach(function(val,key){
               console.log("val"+coindetails.length);
               console.log("key"+key);
               User.findDetailsGame12({userName:val.user_name}).then((userdetails)=>{  
                    console.log("userdetails"+(userdetails.startCoin));

                    userObj={
                        balance:(userdetails.startCoin),
                        user_name:(!val.user_name) ? '' : val.user_name,
                        user_email:(!val.email) ? '' : val.email,
                        coins:val.coins,
                        reference:val.reference,
                        type:val.type
                        
                    };
                    
                    userArr.push(userObj);


                    if(key==coindetails.length-1 && coindetails.length==userArr.length){
                        res.send(response.generate(constants.SUCCESS_STATUS,
                          userArr, 'User Coin List fetched successfully !!'));

                    }


               }).catch(userdetailserr=>{
                console.log("rtyy"+userdetailserr);
                    res.send(response.error(constants.ERROR_STATUS,userdetailserr,"Unable to fetch role list"));
                 })
        });

       
        /*res.send(response.generate(constants.SUCCESS_STATUS,
            coindetails, 'User Coin List fetched successfully !!'));*/
    }).catch(err=>{
        res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch coin list"));
    })
};
//addUserCoins

exports.addUserCoins = function (req,res) {
    let formData=req.body;
    User.addUserCoin(formData).then((coindetails)=>{
        res.send(response.generate(constants.SUCCESS_STATUS,
            coindetails, 'User Coin List added successfully !!'));
    }).catch(err=>{
        res.send(response.error(constants.ERROR_STATUS,err,"Unable to add coin"));
    })
};



/* --------- Started On 09.11.2020 (Transaction Purpose )--------- */

exports.transactionList = function (req,res) {
    console.log('Reached to transaction list');
    findTransactionListAdmin().then((transactionList)=>{
        res.send(response.generate(constants.SUCCESS_STATUS,
            transactionList, 'Transaction List fetched successfully !!'));
    }).catch(err=>{
        res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch transaction list"));
    })
};

//deleteTransaction

exports.deleteTransaction= function(req,res) {
    console.log('deleteTransaction');
    if(!req.body.transactionId || !req.body.userEmail ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={delete_status:"Deleted"};

    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            console.log('validResponse');
            console.log(validResponse);
            return updateTransactionStatusDelete({_id: mongoose.Types.ObjectId(req.body.transactionId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Transaction deleted ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
}

// Add Transaction
exports.addTransaction= function(req,res) {
    console.log(req);
    
    let schema = Joi.object().keys({
        user_email: Joi.string().max(254).trim().required(),
        user_name:Joi.string().required(),
        type:Joi.string().required(),
        status:Joi.string().required(),
        amount:Joi.string().optional(),
        amount_usd:Joi.string().required(),
        transaction_key:Joi.string().required(),


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

        var travelTime = moment().add(30, 'minutes').format('hh:mm A');
        let curdate= moment().format('YYYY-MM-DD');
        let expired_at=curdate+" "+travelTime;



        var userObj = {
                        user_name : req.body.user_name,
                        user_email : req.body.user_email,
                        amount : req.body.amount,
                        amount_usd : req.body.amount_usd,
                        transaction_key : req.body.transaction_key,
                        status : req.body.status,
                        expired_at : expired_at,
                        type : req.body.type,
                        user_confirmation : req.body.transaction_key
        }
       
        addTransaction(userObj).then((transactionDetails)=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Transaction added ."})
        }).catch(err=>{
            res.send(response.error(constants.ERROR_STATUS,err,"Unable to add Transaction"));
        });
    }
}


// Edit Transaction
exports.editTransaction= function(req,res) {
    console.log(req);
    
    let schema = Joi.object().keys({
        user_email: Joi.string().max(254).trim().required(),
        user_name:Joi.string().required(),
        type:Joi.string().required(),
        status:Joi.string().required(),
        amount:Joi.string().optional(),
        amount_usd:Joi.string().required(),
        transaction_key:Joi.string().required(),
        transaction_id:Joi.string().required(),

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

        var userObj = {
                        user_name           : req.body.user_name,
                        user_email          : req.body.user_email,
                        amount              : req.body.amount,
                        amount_usd          : req.body.amount_usd,
                        transaction_key     : req.body.transaction_key,
                        status              : req.body.status,
                        type                : req.body.type,
                        user_confirmation   : req.body.transaction_key
        }
       
        editTransaction({_id: mongoose.Types.ObjectId(req.body.transaction_id)},userObj).then((transactionDetails)=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Transaction Edited..."})
        }).catch(err=>{
            res.send(response.error(constants.ERROR_STATUS,err,"Unable to add Transaction"));
        });
    }
}