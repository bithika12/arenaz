/* Import module*/
var constants = require("../config/constants");
/* Import model*/
var User = require('../schema/Schema').userModel; 
/** function check Authentication  Token */
var auth={}
auth.authChecker = function(req, res, next) {
    if(!req.header("access_token")){
       return res.send({"status":constants.PARAMMISSING_STATUS,"error":{},"message":"Access token missing !"});
    }
    User.checkUserToken({"accessToken":req.header("access_token")}).then(function (userAuthDetails) {
       if( userAuthDetails.deviceDetails[0].status == "active"){
           if( userAuthDetails.status == "active" ){            
                res.userData ={
                        _id              :   userAuthDetails._id,
                        userName         :   (!userAuthDetails.userName)?"":userAuthDetails.userName,
                        email            :   (!userAuthDetails.email)?"":userAuthDetails.email,
                        accessToken      :   userAuthDetails.deviceDetails[0].accessToken
                };
                next();
            }else{
                return res.send({"status":constants.BLOCKED_BY_ADMIN,"error":{},"message":"You are blocked by admin!!"});
            }    
        }else if(userAuthDetails.deviceDetails[0].status == "inactive"){
            return res.send({"status":constants.LOGIN_ANOTHER_DEVICE,"error":{},"message":"You have login another device!!"});
        }else{
            return res.send({"status":constants.INVALID_TOKEN,"error":{},"message":"Invalid auth token !!"});
        }
    }).catch(err => {
       return res.send({"status":constants.INVALID_TOKEN,"error":err,"message":"Invalid auth token !!"});
    });
}
module.exports =auth;