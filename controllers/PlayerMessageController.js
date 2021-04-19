/**  Import Package**/
var async = require('async');
const appRoot = require('app-root-path');
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
let Notification  = require(appRoot +'/models/Notification');
const CountryCodes = require('country-code-info');

exports.viewMessage = function (req,res) {
  console.log("ok");
    Notification.viewMessage({userName: req.body.userName}).then(function (result) {
        if(result) {
            let mesageRes={
                messageList:result
            }
                
            res.status(constants.HTTP_OK_STATUS).send({"status": constants.SUCCESS_STATUS, "result":mesageRes, "message": "Fetched message successfully"});
        }else{
            res.status(constants.BAD_REQUEST_STATUS).send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});
        }
   }).catch(err => {
          res.status(constants.BAD_REQUEST_STATUS).send({"status":constants.ERROR_STATUS,"result":err,"message":"Something went Wrong!!"});
   }); 
};






