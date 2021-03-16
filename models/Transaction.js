 /* INCLUDE  PACKAGE */
const uuidv4 = require('uuid/v4');

/*Include Constants */
var constants = require("../config/constants");


/* INCLUDE UTILS  */
const timeManage  = require('../utils/TimeManager');
const password = require('../utils/PasswordManage');
/*Include model */
var Version = require('../schema/Schema').versionModel;
let Transaction = require('../schema/Schema').userTransactionModel;

Version.details = function(condObj){
  return new Promise((resolve,reject)=>{  
    Version.findOne({status:"active"},
    	{_id: 1,
    	minimum_deposit:1,
    	minimum_withdrawl:1,
    	coin_price_usd:1,
    	transaction_fee_deposit:1,
    	transaction_fee_withdrawl:1,
    	e_currency_price_api:1,
    	wallet_api_link:1,
    	wallet_key:1,
    	api_expiration_time:1,
      new_account_gift_coins:1,
      master_message:1,
      allow_mini_account_withdrawal:1,
      support_email:1
    }).then(response=> {
         console.log("op")
         resolve(response)
    }).catch(err=>{
    	 reject(err);
    }) 	
  })
} 


Version.trandetails = function(condObj){
  return new Promise((resolve,reject)=>{  
    Transaction.findOne(condObj,
      {_id: 1,status:1,user_name:1,user_email:1,expired_at:1,amount:1,created_at:1,user_confirmation:1}).then(response=> {
         console.log("op")
         resolve(response)
    }).catch(err=>{
       reject(err);
    })  
  })
} 





 module.exports= Version;
// module.exports= {details,findTransactionListAdmin};
 //module.exports= Transaction;


