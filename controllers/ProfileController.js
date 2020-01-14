 /**  Import Package**/
var async = require('async');

 /** Required all module**/
 var constants = require("../config/constants");
/** Import Model **/
 var User  = require('../models/User');




/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response     = require('../utils/ResponseManeger');

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
exports.updateProfileImage = function(req,res){
  
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





