/* Import module*/
var constants = require("../config/constants");
/* Import model*/
var User = require('../schema/Schema').userModel; 
/** function check Authentication  Token */
var auth={}
auth.authChecker = function(req, res, next) {
    if(!req.header("access-token")){
       return res.send({"status":constants.PARAMMISSING_STATUS,"error":{},"message":"Access token missing !"});
    }
    User.checkUserToken({"access_token":req.header("access-token")}).then(function (userauthdetails) {
       if( userauthdetails.device_details[0].status == "active"){
           if( userauthdetails.status == "active" ){            
                res.userData ={
                        _id              :   userauthdetails._id,
                        name             :   (!userauthdetails.name)?"":userauthdetails.name,
                        email            :   (!userauthdetails.email)?"":userauthdetails.email,
                        avatar_id        :   (!userauthdetails.avatar_id)?"":userauthdetails.avatar_id,
                        image            :   (!userauthdetails.image)?"":userauthdetails.image,
                        score            :   (!userauthdetails.score)?0.0:userauthdetails.score,
                        access_token     :   userauthdetails.device_details[0].access_token
                };
                next();
            }else{
                return res.send({"status":constants.BLOCKED_BY_ADMIN,"error":{},"message":"You are blocked by admin!!"});
            }    
        }else if(userauthdetails.device_details[0].status == "inactive"){
            return res.send({"status":constants.LOGIN_ANOTHER_DEVICE,"error":{},"message":"You have login another device!!"});
        }else{
            return res.send({"status":constants.INVALID_TOKEN,"error":{},"message":"Invalid auth token !!"});
        }
    }).catch(err => {
       return res.send({"status":constants.INVALID_TOKEN,"error":err,"message":"Invalid auth token !!"});
    });
}
module.exports =auth;