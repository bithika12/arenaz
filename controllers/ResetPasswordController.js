 /**  Import Package**/
var async = require('async');
var _jade = require('jade');
var fs = require('fs');

 /** Required all module**/
 var constants = require("../config/constants");

/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response     = require('../utils/ResponseManeger');


/** Import Model **/
 var User  = require('../models/User');

// ASYNC FUNCTION

function emailCheck(reqObj){
    return function (callback) {
        User.findDetails({email:reqObj.email}).then((userdetails)=>{
            callback (null,userdetails);
        }).catch(err => {
            callback (err,null);
       });
    }
}
function updateResetToken(result,callback){
    User.updateResetToken({_id:result._id}).then(function(resetResult) {
        callback (null,{ email: result.email ,name :result.name,token : resetResult[0].token,otp : resetResult[0].otp});
    }).catch(err => {
        callback (err,null);
    });
}    


function checkValidOtp(reqObj){
    return function (callback){
        User.checkValidOtp(reqObj).then(function (otpDetails) {
            // otpDetails.set('password',reqObj.password)
            reqObj._id = otpDetails._id;

           
            callback(null,reqObj)
        }).catch(err=>{
             callback(err,null)
        })  
    }
}

function updatePassword(reqObj,callback){
    User.resetPassword({_id :reqObj._id},{password : reqObj.password}).then(function(updatedetials){
         callback(null,reqObj);
    }).catch(err=>{
         callback(err,null)
    })
}

function emailSend(result){    
 //   var resetObj={_id:result.id};
    //User.updateResetToken(resetObj).then(function(resetResult) {     
      //  console.log("resetResult ",resetResult)
        var template = process.cwd() + '/views/email/resetpasswordemail.jade';
        let  subject = " Forgot Password";
        /* Content*/ 
        var email_content =  " Your reset otp "+result.otp;
             // get template from file system
        fs.readFile(template, 'utf-8', function (err, file) {
            if (err) {
                //handle errors
                console.log('ERROR!');
            } else {
                //compile jade template into function
                var compiledTmpl = _jade.compile(file, {filename: template});
                // set context to be used in template
                var context = {
                    //token:resetResult.reset_token,
                    content: email_content,
                    BASEURL: constants.BASEURL,
                    email_content: email_content
                   
                };
                var html = compiledTmpl(context);
                var mailOptions = {
                    to: result.email,
                    subject: subject,
                    html: html
                };
                constants.TRANSPORTER.sendMail(mailOptions, function (error, info) {
                     console.log(" errorr", error);
                      console.log(" info  :",info)
                });
            }
        });
    //});
}

/** Route function **/



exports.sendResetOtp =function (req,res) {
    if(!req.body.communication_details || !req.body.communication_type ){
        return res.send({"status":"0","result": {},"message":"Parameter Missing!"});
    }
    var userObj={email:req.body.communication_details};
    async.waterfall(
        [
            emailCheck(userObj),
            updateResetToken
        ],
        function (err, result) {
           if (result) {
               emailSend(result);
               res.send(response.generate(constants.SUCCESS_STATUS,{token: result.token}, 'Please check your email'));
             }else if (!Object.keys(err).length) {
                res.send(response.error(constants.ERROR_STATUS,{},"Sorry !! Email does not exist!!"))
            }else{
                res.send(response.error(constants.ERROR_STATUS,err,"Something wrong!!"));
            }
        });
};

 exports.verifyOtp=function(req,res){
    if(!req.body.token || !req.body.otp ){
        return res.send({"status":"0","result": {},"message":"Parameter Missing!"});
    }
    var otpObj={ token: req.body.token , otp :req.body.otp };

    async.waterfall(
        [
            checkValidOtp(otpObj)
        ],
        function (err, result) {
            if (result) {
                res.send({"status": constants.SUCCESS_STATUS, "result":{token:req.body.token}, "message": "Valid Otp"});
            }else{
                 res.send({"status": constants.ERROR_STATUS, "result": err, "message": "Invalid Otp"});
            }
        });

};


exports.resetPassword = function(req,res){
    if(!req.body.token || !req.body.password){
        return res.send({"status":"0","result": {},"message":"Parameter Missing!"});
    }
    
    async.waterfall(
            [
                checkValidOtp({token:req.body.token,password:req.body.password }),
                updatePassword
            ],
    function (err, result) {
        if (result) {
            res.send({"status": constants.SUCCESS_STATUS, "result":{}, "message": "Password reset successfully"});
        }else{
            res.send({"status": constants.ERROR_STATUS, "result": err, "message": "Something went wrong!!"});
        }
    });
}