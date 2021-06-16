 /**  Import Package**/
var async = require('async');
 const appRoot = require('app-root-path');
 var mongoose = require('mongoose');
 /** Required all module**/
 var constants = require("../config/constants");
/** Import Model **/
 var User  = require('../models/User');

 const Joi = require('joi');
 let user = require(appRoot + '/models/User');

/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response     = require('../utils/ResponseManeger');

const CountryCodes = require('country-code-info');

 const { updateVersionAdmin,fetchHistoryAdmin,userValidChkAdmin,updateProfileAdmin,modifyProfileDetails,fetchRoleName} = require(appRoot +'/models/FetchHistory');
/** Route function **/


function updateProfile(condObj,updateObj){
  return function (callback) {
    User.updateUserDetails(condObj,updateObj).then((updatedetails) => {
       callback (null,updatedetails);
    }).catch(err => {
        callback (err,null);
    });
  } 

}

exports.profiledDetails = function(req, res) {  
    return res.send({
                    "status": '1',
                    "result":{
                                _id               :  res.userData._id,
                                name              :  res.userData.name,
                                email             :  res.userData.email,
                                avatar_id         :  res.userData.avatar_id,
                                image             :  res.userData.image,
                                score             :  res.userData.score,
                              },
                   "message": "Profile Details"
                 });
        
};


exports.updateProfile = function (req,res){
   if(!req.body.name && !req.body.avatar_id ){
       return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
   } 
    var updateObj ={}
    if(req.body.name){
        updateObj.name = req.body.name
    }
    if(req.body.avatar_id){
        updateObj.avatar_id = req.body.avatar_id
    }

    async.waterfall([
         updateProfile({_id: res.userData. _id},updateObj)
    ],function (err, result) {
        if(result){
            res.send(response.generate(constants.SUCCESS_STATUS,updateObj, 'User updated successfully !!'));
        }else{
            res.send(response.error(constants.ERROR_STATUS,err,"Something went wrong!!"));
        }
    });

}

 exports.disableProfile = function (req,res){
     if(!req.body.userName ){
         return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
     }
     let updateObj ={status:"inactive"};

       userValidChkAdmin(res.userData.email)
         .then(validResponse => {
             return updateProfileAdmin({userName: req.body.userName},updateObj);
             //return updateProfileAdmin({_id: res.userData. _id},updateObj);
         })
         .then(resp=>{
             res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"Account deleted ."})
         })
         .catch(err=>{
             res.status(constants.API_ERROR).send(err);
         });

     /*async.waterfall([
         updateProfile({_id: res.userData. _id},updateObj)
     ],function (err, result) {
         if(result){
             res.send(response.generate(constants.SUCCESS_STATUS,updateObj, 'User updated successfully !!'));
         }else{
             res.send(response.error(constants.ERROR_STATUS,err,"Something went wrong!!"));
         }
     });*/

 }
exports.updateProfileImage = function(req,res){
  
}
//modify profile
 exports.modifyProfile = function (req,res){

     let schema = Joi.object().keys({
         //userName:  Joi.string().required(),
         coinNumber: Joi.optional(),
         //coinNumber: Joi.number().required(),
         firstName: Joi.optional(),
         lastName:  Joi.optional(),
         roleName:  Joi.string().required(),
         userEmail:  Joi.string().required(),
         roleId:    Joi.string().required(),
         status:    Joi.string().optional()

     });
     //const {value, error} = result;
     const {body} = req;
     let result = Joi.validate(body, schema);
     const {value, error} = result;
     const valid = error == null;
     if (!valid) {
         let data = { status: constants.VALIDATION_ERROR, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
         return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
     }
     else{
         let id1 = mongoose.Types.ObjectId(req.body.roleId);
         let updateObj={
             firstName:req.body.firstName,
             lastName:req.body.lastName,
             //startCoin:req.body.coinNumber,
             roleId:id1,
             status:req.body.status

         };
         modifyProfileDetails({email:req.body.userEmail},updateObj)
             /*.then(resp=>{
                 return modifyProfileDetails({email:req.body.userEmail},updateObj)
             })*/
             .then(resp1=>{
                 console.log("profile have been modified");
                 res.status(constants.HTTP_OK_STATUS).send({status:constants.HTTP_OK_STATUS,message:"Your profile have been modified."})
             })
             .catch(err=>{
                 res.status(constants.API_ERROR).send(err);
             });
     }

 }
 exports.modifyProfileoRG1 = function (req,res){

     let schema = Joi.object().keys({
         //userName:  Joi.string().required(),
         coinNumber: Joi.number().required(),
         firstName: Joi.optional(),
         lastName:  Joi.optional(),
         roleName:  Joi.string().required(),
         userEmail:  Joi.string().required(),
         roleId:    Joi.string().required()

     });
     //const {value, error} = result;
     const {body} = req;
     let result = Joi.validate(body, schema);
     const {value, error} = result;
     const valid = error == null;
     if (!valid) {
         let data = { status: constants.VALIDATION_ERROR, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
         return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
     }
     else{
         let id1 = mongoose.Types.ObjectId(req.body.roleId);
         console.log(id1);
          fetchRoleName({slug:req.body.roleName})
             .then(resp=>{
                 let updateObj={
                     firstName:req.body.firstName,
                     lastName:req.body.lastName,
                     startCoin:req.body.coinNumber,
                     roleId:id1

                 };
                 return modifyProfileDetails({email:req.body.userEmail},updateObj)
                 //res.status(constants.HTTP_OK_STATUS).send({status:constants.HTTP_OK_STATUS,message:"Your profile have been modified."})
             })
              .then(resp1=>{
                  console.log("profile have been modified");
                  res.status(constants.HTTP_OK_STATUS).send({status:constants.HTTP_OK_STATUS,message:"Your profile have been modified."})
              })
             .catch(err=>{
                 res.status(constants.API_ERROR).send(err);
             });
     }

 }
exports.updatePassword = function (req,res){
    if(!req.body.password ){
       return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    async.waterfall([
         updateProfile({_id: res.userData. _id },{ password : req.body.password})
    ],function (err, result) {
        if(result){
            res.send(response.generate(constants.SUCCESS_STATUS,{}, 'User updated successfully !!'));
        }else{
            res.send(response.error(constants.ERROR_STATUS,err,"Something went wrong!!"));
        }
    });


}

//colorChange api

 exports.colorReg = function (req,res){
     let schema = Joi.object().keys({
         userEmail: Joi.string().required(),
         colorName: Joi.string().required(),
         raceName:  Joi.string().required(),
         dartName:  Joi.string().required(),
         characterId:  Joi.string().required(),
         countryName: Joi.string().required(),
         languageName: Joi.string().required(),
   });

     const {body} = req;
     console.log(req.body);
     let result = Joi.validate(body, schema);
     const {value, error} = result;
     const valid = error == null;
     if (!valid) {
         let data = { status: constants.VALIDATION_ERROR, result: result.error.name, message: result.error.details[0].message.replace(new RegExp('"', "g"), '') };
         return res.status(constants.UNAUTHERIZED_HTTP_STATUS).send(data);
     }
     else{
           user.colorRequestProfile({userEmail: req.body.userEmail}, {"colorName": req.body.colorName})
             .then(colorUpdate => {
                 return user.sageRequestProfile({userEmail: req.body.userEmail}, {"raceName": req.body.raceName}
                 );
             })
             .then(raceUpdate => {
                 return user.nameRequestProfile({userEmail: req.body.userEmail}, {"dartName": req.body.dartName}
                 );
             })
               .then(nameUpdate => {
                   return user.characterRequestProfile({userEmail: req.body.userEmail}, {"characterName": req.body.characterId}
                   );
               })
               .then(characterUpdate => {
                   return user.countryRequestProfile({userEmail: req.body.userEmail}, {"countryName": req.body.countryName}
                   );
               })
               //languageRequestProfile
               .then(countryUpdate => {
                   return user.languageRequestProfile({userEmail: req.body.userEmail}, {"languageName": req.body.languageName}
                   );
               })
             .then(resp => {
                 res.send(response.generate(constants.SUCCESS_STATUS,{}, 'User details updated successfully !!'));
              })
             .catch(err => {
                 console.log(err);
                 res.send(response.error(constants.ERROR_STATUS,err,"Unable to update user details!!"));
             });
     }


 }

 //GET COLOR
   //getColorReg
 exports.getColorReg = function (req,res) {

     let schema = Joi.object().keys({
         userEmail: Joi.string().required()
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
     User.checkColorMod({email: req.body.userEmail}).then((userDetails) => {
         if (userDetails) {
             //res.send(userDetails);
             res.send(response.generate(constants.SUCCESS_STATUS,userDetails, 'User details fetched successfully !!'));
         }
         else {
             res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch user details!!"));
         }
     })

   }


 }

 //fetchVersion

 exports.fetchVersion = function (req,res) {

     let schema = Joi.object().keys({
         userEmail: Joi.string().required()
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
     User.fetchVersion().then((versionDetails) => {
         if (versionDetails) {
            console.log("versionDetails.banned_country"+(versionDetails.market_volatility))
            //versionDetails.banned_country=JSON.parse(versionDetails.banned_country)
             //res.send(userDetails);
             res.send(response.generate(constants.SUCCESS_STATUS,versionDetails, 'Version details fetched successfully !!'));
         }
         else {
             res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch version details!!"));
         }
     })

   }


 }





  // exports.leaderBoard =function(req, res) { 
  //     User.listing().then(function (result) {             
  //         if(result) {
  //            User.countUser({total_kill :{$gt : res.userData.total_kill }}).then(function (totalUser) {  
  //                var rankerArr =[]
  //                let  rank =1;
  //                result.forEach(function (key) {
  //                       rankerArr.push({rank: rank,_id :key._id,name: key.name, total_kill : (!key.total_kill)?0:key.total_kill });
  //                       rank++;
  //                }); 
  //               res.send({"status": '1', "result":{"rankerArr": rankerArr,"yourRank" :{ "rank" :  totalUser, "id":res.userData.user_id,"name":res.userData.name,"total_kill": res.userData.total_kill}}, "message": "Leaderboard listing successfully"});  
  //           })
  //         }else{
  //             res.send({"status": '3', "result": {}, "message": "Something went Wrong!!"});
  //         }
  //     }).catch(err => {
  //         res.send({"status":'3',"result":err,"message":"Something went Wrong!!"});
  //     });
  // };


  exports.updateVersion = function (req,res) {

    console.log("req.body"+JSON.stringify(req.body))

     if(!req.body.versionId || !req.body.userEmail){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }
    let updateObj ={
        app_version:req.body.app_version,
        download_link:req.body.download_link,
        coin_price_usd : req.body.coin_price_usd,
        wallet_api_link : req.body.wallet_api_link,
        wallet_key : req.body.wallet_key,
        api_expiration_time : req.body.api_expiration_time,
        e_currency_price_api : req.body.e_currency_price_api,
        transaction_fee_withdrawl : req.body.transaction_fee_withdrawl,
        transaction_fee_deposit : req.body.transaction_fee_deposit,
        minimum_deposit : req.body.minimum_deposit,
        minimum_withdrawl : req.body.minimum_withdrawl,
        new_account_gift_coins : req.body.new_account_gift_coins,
        master_message : req.body.master_message,
        allow_mini_account_withdrawal : req.body.allow_mini_account_withdrawal,
        support_email : req.body.support_email,
        market_volatility:req.body.market_volatility,
        banned_country:req.body.banned_country,

        email_verify:req.body.email_verify,
        game_deactivation:req.body.game_deactivation,
        ip_verify:req.body.ip_verify,
        auto_refill_coins:req.body.auto_refill_coins,
        free_coin_incentive:req.body.free_coin_incentive
    };
    console.log("updateObj"+updateObj)
     userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            console.log("pll");
            return updateVersionAdmin({_id: mongoose.Types.ObjectId(req.body.versionId)},updateObj);
            //return updateProfileAdmin({_id: res.userData. _id},updateObj);
        })
        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,message:"version modified ."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });


 }


 exports.fetchAppVersion = function (req,res) {

     let schema = Joi.object().keys({
         //userEmail: Joi.string().required(),
         app_version: Joi.string().required()
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
     User.fetchVersion().then((versionDetails) => {
         if (versionDetails) {
            console.log("versionDetails"+versionDetails[0].download_link);
             //res.send(userDetails);
             let countryres=JSON.parse(versionDetails[0].banned_country);
             let bannedStatus=countryres.filter((item)=>{
                            return item===cName
             });

             let flag=false;
             if(versionDetails[0].app_version==req.body.app_version){
                 flag=true;

             }
                let versionObj={
                    status:flag,
                    bannedStatus:bannedStatus.length,
                    game_deactivation:versionDetails[0].game_deactivation,
                    download_link:versionDetails[0].download_link
                }

                /*let versionObj={
                    status:flag,
                    coin_price_usd : versionDetails[0].coin_price_usd,
                    wallet_api_link : versionDetails[0].wallet_api_link,
                    wallet_key : versionDetails[0].wallet_key,
                    api_expiration_time : versionDetails[0].api_expiration_time,
                    e_currency_price_api : versionDetails[0].e_currency_price_api,
                    transaction_fee_withdrawl : versionDetails[0].transaction_fee_withdrawl,
                    transaction_fee_deposit : versionDetails[0].transaction_fee_deposit,
                    minimum_deposit : versionDetails[0].minimum_deposit,
                    minimum_withdrawl : versionDetails[0].minimum_withdrawl,
                    download_link:versionDetails[0].download_link
                }*/
             
             res.send(response.generate(constants.SUCCESS_STATUS,versionObj, 'Version details fetched successfully !!'));
         }
         else {
             res.send(response.error(constants.ERROR_STATUS,err,"Unable to fetch version details!!"));
         }
     })

   }


 }





