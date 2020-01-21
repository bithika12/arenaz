 /* INCLUDE  PACKAGE */
const uuidv4 = require('uuid/v4');

/*Include Constants */
var constants = require("../config/constants");


/* INCLUDE UTILS  */
const timeManage  = require('../utils/TimeManager');
const password = require('../utils/PasswordManage');
/*Include model */
var Coin = require('../schema/Schema').coinModel;

 Coin.createUser = function(reqObj){
      return new Promise((resolve,reject)=>{    
      Role.insertMany().then(response=> {
          resolve(response)
      }).catch(err=>{
          reject(err);
      })  
   })
}
 Coin.details = function(condObj){
  return new Promise((resolve,reject)=>{  
    Role.findOne(condObj,{_id: 1,name:1,slug:1}).then(response=> {
         resolve(response)
    }).catch(err=>{
    	 reject(err);
    }) 	
  })
}
 Coin.detailsAdmin = function(condObj){
     return new Promise((resolve,reject)=>{
         Role.find(condObj,{_id: 1,name:1,slug:1}).then(response=> {
             resolve(response)
         }).catch(err=>{
             reject(err);
         })
     })
 }


 module.exports= Coin;


