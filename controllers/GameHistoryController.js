/**  Import Package**/
let async = require('async');
var underscore  = require('underscore');
let _jade = require('jade');
const Joi = require('joi');
const appRoot = require('app-root-path');
const password = require('../utils/PasswordManage');
/** Required all module**/
let constants = require("../config/constants");
/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response     = require('../utils/ResponseManeger');
let randomstring = require("randomstring");
/** Import Model **/
let User  = require('../models/User');
const { fetchHistoryUser,userGame,fetchHistory,userValidChk} = require(appRoot +'/models/FetchHistory');
const Games = require(appRoot +'/models/Game');

/**
 * @desc This function is used for forgot password
 *
 * @param {String} email
 * {String} password
 */

exports.fetchGame= function(req,res) {
    /*
      * Joi is used for validation
     */
    let schema = Joi.object().keys({
        userEmail: Joi.required(),
        //userName: Joi.optional()
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = { status: constants.VALIDATION_ERROR, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
        return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else{
           userValidChk(req.body.userEmail)
            .then(validResponse => {
                return fetchHistory(validResponse);
            })

            .then(resp=>{
                let obj={matchDetails:resp}
                console.log("plp");
                res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,result:obj,message:"Game history fetched successfully."})
            })
            .catch(err=>{
                console.log("err12"+JSON.stringify(err));
                res.status(constants.API_ERROR).send(err);
            });
    }
}
//userGame
exports.userGame= function(req,res) {
    /*
      * Joi is used for validation
     */
    let schema = Joi.object().keys({
        userEmail: Joi.required(),
        //userName: Joi.optional()
    });
    const {body} = req;
    let result = Joi.validate(body, schema);
    const {value, error} = result;
    const valid = error == null;
    if (!valid) {
        let data = { status: constants.VALIDATION_ERROR, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
        return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
    }
    else{
           userValidChk(req.body.userEmail)
            .then(validResponse => {
                return fetchHistoryUser(validResponse);
            })

            .then(resp=>{
                let obj={matchDetails:resp}
                res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,result:obj,message:"Game history fetched successfully."})
            })
            .catch(err=>{
                res.status(constants.API_ERROR).send(err);
            });
    }
}

// Fetch all games

exports.gamesLists = function(req,res){
    console.log('Reached game history controller....');
    var condObj = {"status": "active"};
    Games.allGames(condObj).then((allgames) => {
        res.send({status:constants.SUCCESS_STATUS,result:{allgames},message:"Game history fetched successfully."})
    }).catch(err => {
   res.status(constants.API_ERROR).send(err);
  });
}
