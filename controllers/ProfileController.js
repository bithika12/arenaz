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

 const { fetchHistoryAdmin,userValidChkAdmin,updateProfileAdmin,modifyProfileDetails,fetchRoleName} = require(appRoot +'/models/FetchHistory');
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
         let updateObj={
             firstName:req.body.firstName,
             lastName:req.body.lastName,
             startCoin:req.body.coinNumber,
             roleId:id1

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





