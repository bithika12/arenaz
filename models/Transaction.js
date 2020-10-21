 /* INCLUDE  PACKAGE */
const uuidv4 = require('uuid/v4');

/*Include Constants */
var constants = require("../config/constants");


/* INCLUDE UTILS  */
const timeManage  = require('../utils/TimeManager');
const password = require('../utils/PasswordManage');
/*Include model */
var Version = require('../schema/Schema').versionModel;


Version.details = function(condObj){
  return new Promise((resolve,reject)=>{  
    Version.findOne({status:"active"},{_id: 1,minimum_deposit:1,minimum_withdrawl:1,coin_price_usd:1,transaction_fee_deposit:1,transaction_fee_withdrawl:1,e_currency_price_api:1,wallet_api_link:1,wallet_key:1}).then(response=> {
         console.log("op")
         resolve(response)
    }).catch(err=>{
    	 reject(err);
    }) 	
  })
}



 module.exports= Version;


