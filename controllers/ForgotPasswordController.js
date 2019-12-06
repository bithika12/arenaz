/**  Import Package**/
let async = require('async');
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
const { callEmailChkByUserName,callEmailUpdatePassword,callEmailSend,callEmailChkByEmail } = require(appRoot +'/models/ForgotPassword');

/**
 * @desc This function is used for forgot password
 *
 * @param {String} email
 * {String} password
 */

exports.forgotPassword= function(req,res) {
    /*
      * Joi is used for validation
     */
    let schema = Joi.object().keys({
        email: Joi.optional(),
        userName: Joi.optional()
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
        let pass=randomstring.generate(7);
        if(!req.body.email) {
            callEmailChkByUserName(req.body.userName)
                .then(emailResponse => {
                    let hashPassword  =  password.hashPassword(pass);
                    return callEmailUpdatePassword(emailResponse,hashPassword
                    );
                })
                .then(resp=>{
                    return callEmailSend(resp,pass
                    );

                })
                .then(resp=>{
                    res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Your login credentials have been emailed to you."})
                })
                .catch(err=>{
                    res.status(constants.API_ERROR).send(err);
                });
        }
        else{

            callEmailChkByEmail(req.body.email)
                .then(emailResponse => {
                    let hashPassword  =  password.hashPassword(pass);
                    return callEmailUpdatePassword(emailResponse,hashPassword
                    );
                })
                .then(resp=>{
                    return callEmailSend(resp,pass
                    );
                })
                .then(resp=>{
                    res.status(constants.HTTP_OK_STATUS).send({status:constants.HTTP_OK_STATUS,message:"Your login credentials have been emailed to you."})
                })
                .catch(err=>{
                    res.status(constants.API_ERROR).send(err);
                });
        }
    }
}


