/**  Import Package**/
var async = require('async');
/* Import */
var constants = require("../config/constants");

/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response  = require('../utils/ResponseManeger');
//const jwtTokenManage   = require('../utils/JwtTokenManage');

const password = require('../utils/PasswordManage');

/**  Import model **/
var User  = require('../models/User');
var Role  = require('../models/Role');


// Role.createUser().then((details)=>{

// })
/* Async function*/



function getUserDetails(reqObj){
  return function(callback){
    User.findDetails({email:reqObj.email,role:reqObj.role}).then((userdetails)=>{
       if(password._comparePasswordSync(reqObj.password, userdetails.password)){
            callback (null,userdetails);
       }else{
            callback(err,null)
       }
    }).catch(err=>{
         callback(err,null);
    })
  }
}



function updateToken(user,callback){
    console.log(" user",user)
    User.updateToken({_id :user._id},{}).then((tokendetails) => {
      user.set('access_token', tokendetails[0].access_token)
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
              res.send(response.generate(constants.SUCCESS_STATUS,{"_id":result._id,"name":result.name,email:result.email,score:result.score,"access_token":result.get('access_token')}, 'User login successfully !!'));
          }else{
              res.send(response.error(constants.ERROR_STATUS,err,"Invalid password!!"));
          }
    });
}

exports.logout = function (req,res) {
    User.removeToken({access_token: req.header("access-token")}).then(function (result) {
    		if(result) {
    		    res.send({"status": constants.SUCCESS_STATUS, "result":{ }, "message": "Logout successfully"});
    		}else{
    		    res.send({"status": constants.ERROR_STATUS, "result": {}, "message": "Something went Wrong!!"});
    		}
   }).catch(err => {
    	    res.send({"status":constants.ERROR_STATUS,"result":err,"message":"Something went Wrong!!"});
   }); 
};




