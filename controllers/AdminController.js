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
const appRoot = require('app-root-path');
const { fetchHistoryAdmin,userValidChkAdmin} = require(appRoot +'/models/FetchHistory');

// Role.createUser().then((details)=>{

// })
/* Async function*/



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
    userValidChkAdmin(req.body.userEmail)
        .then(validResponse => {
            return fetchHistoryAdmin(validResponse);
        })

        .then(resp=>{
            res.status(constants.HTTP_OK_STATUS).send({status:constants.SUCCESS_STATUS,result:resp,message:"Game history fetched successfully."})
        })
        .catch(err=>{
            res.status(constants.API_ERROR).send(err);
        });
};
