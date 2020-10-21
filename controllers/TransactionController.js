/**  Import Package**/
var async = require('async');
/* Import */
var constants = require("../config/constants");
var mongoose = require('mongoose');
/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response  = require('../utils/ResponseManeger');
//const jwtTokenManage   = require('../utils/JwtTokenManage');

const password = require('../utils/PasswordManage');
const Joi = require('joi');
/**  Import model **/
var User  = require('../models/User');
// /FetchHistory
var AddTrans  = require('../models/FetchHistory');

var Transaction  = require('../models/Transaction');
const appRoot = require('app-root-path');
const {chkValidTransaction,updateTransactionConfirm, updateMail,updateTransaction,addMail,addTransaction,updateGameAdmin,addMatch,fetchMatches,fetchUserList,updateRoomAdmin,
  fetchHistoryAdmin,userValidChkAdmin,fetchCoin,addCoin,updateCoinAdmin,fetchMail} = require(appRoot +'/models/FetchHistory');
  const UserController  = require('../controllers/UserController');
  const moment = require('moment');
  const axios = require('axios');
  var CircularJSON = require('circular-json');


  function saveDepositV1(userObj){
   return function (callback) {
    //first add transaction
    var transactionObjV1 = {
                              user_name : userObj.user_name,                              
                              amount : userObj.coinAmount,
                              amount_usd : userObj.amount_usd,                              
                              status : "New",                                                             
                              type : "Deposit"
                            }

  AddTrans.addTransaction(transactionObjV1).then((resTransV1) => { 


    Transaction.details().then((appList) => {
                         //let api_url="https://www.qqxm.com/arz/wallet.cfm?key=dhsDJDKkdhjsjatSTYI3ks&userid=123456789&amt=550";
                         let api_url=appList.wallet_api_link+appList.wallet_key+
                         "&type=Deposit&transid="+resTransV1+"&amt="+userObj.coinAmount

                         console.log("api_url"+api_url)

                         axios.get(api_url)
                         .then(function (response) {
                            // handle success

                            console.log("ok"+ CircularJSON.stringify(response.data));
                            console.log("ok1"+response.data);
                            
                            let lastPart = response.data.split("-->").pop();
                            console.log("klllP"+lastPart);

                            //save it to transaction
                            if(lastPart=="Error!"){
                               //delete transaction record
                                console.log("err");
                            }
                            else {
                            var travelTime = moment().add(30, 'minutes').format('hh:mm A');
                            

                            console.log("travelTime"+travelTime);
                            let curtime= moment().format('hh:mm A');
                            console.log("curtime"+curtime);
                            let curdate= moment().format('YYYY-MM-DD');
                            console.log("curdate"+curdate);
                            let expired_at=curdate+" "+travelTime;
                            
                            var transactionObj = {
                              //user_name : userObj.user_name,
                              user_confirmation : lastPart,
                              //amount : userObj.coinAmount,
                              //amount_usd : userObj.amount_usd,
                              transaction_key : lastPart,
                              //status : "New",
                              expired_at : expired_at,                                
                              //type : "Deposit"
                            }


                            AddTrans.updateTransactionConfirm({_id:mongoose.Types.ObjectId(resTransV1)},transactionObj).then((resTrans) => { 
                             let responseObj={
                               address_part:lastPart,
                               expired_at:expired_at,
                               transactionId:resTransV1
                             }
                             callback (null,responseObj);
                           })
                            .catch(function (error) {
                             console.log(error);
                           })   



                             //callback (null,lastPart);

                             }
                           })
                         .catch(function (error) {
                            // handle error
                            console.log(error);
                          })
                         .then(function () {
                            // always executed
                          });

                       }).catch(fetchErr => {
                        callback (fetchErr,null);
                      });


                       //////////////////pp////
                      })
                            .catch(function (error) {
                             console.log(error);
                           }) 
         

        }

      }

      function saveDeposit(userObj){
       return function (callback) {

        Transaction.details().then((appList) => {
                         //let api_url="https://www.qqxm.com/arz/wallet.cfm?key=dhsDJDKkdhjsjatSTYI3ks&userid=123456789&amt=550";
                         let api_url=appList.wallet_api_link+appList.wallet_key+
                         "&type=Deposit&transid=123456789&amt="+userObj.coinAmount

                         console.log("api_url"+api_url)

                         axios.get(api_url)
                         .then(function (response) {
                            // handle success

                            console.log("ok"+ CircularJSON.stringify(response.data));
                            console.log("ok1"+response.data);
                            
                            let lastPart = response.data.split("-->").pop();
                            console.log("klll"+lastPart)
                            //save it to transaction
                            //if(lastPart="Error!")
                            var travelTime = moment().add(30, 'minutes').format('hh:mm A');
                            //var travelTime = moment().add(11, 'minutes').format('hh:mm A');

                            console.log("travelTime"+travelTime);
                            let curtime= moment().format('hh:mm A');
                            console.log("curtime"+curtime);
                            let curdate= moment().format('YYYY-MM-DD');
                            console.log("curdate"+curdate);
                            let expired_at=curdate+" "+travelTime;
                            /*if("02:40 PM" >= "02:46 PM" ){
                              console.log("expired")
                            }
                            else{
                              console.log("not expire");
                            }*/
                            var transactionObj = {
                              user_name : userObj.user_name,
                              user_confirmation : lastPart,
                              amount : userObj.coinAmount,
                              amount_usd : userObj.amount_usd,
                              transaction_key : lastPart,
                              status : "New",
                              expired_at : expired_at,
                                //expired_at : req.body.expired_at,
                                type : "Deposit"
                              }


                              AddTrans.addTransaction(transactionObj).then((resTrans) => { 
                               let responseObj={
                                 address_part:lastPart,
                                 expired_at:expired_at,
                                 transactionId:resTrans
                               }
                               callback (null,responseObj);
                             })
                              .catch(function (error) {
                            // handle error
                            console.log(error);
                          })   



                             //callback (null,lastPart);

                             
                           })
                         .catch(function (error) {
                            // handle error
                            console.log(error);
                          })
                         .then(function () {
                            // always executed
                          });

                       }).catch(fetchErr => {
                        callback (fetchErr,null);
                      });

         /*axios.get('https://www.qqxm.com/arz/wallet.cfm?key=dhsDJDKkdhjsjatSTYI3ks&type=Deposit&transid=123456789&amt=55.50')
          .then(function (response) {
            // handle success
            console.log("ok"+(response.data));
            console.log("ok1"+CircularJSON.stringify(response));
             let lastPart = response.data.split("-->").pop();
             callback (null,lastPart);
          })
          .catch(function (error) {
            // handle error
            console.log(error);
          })
          .then(function () {
            // always executed
          });*/

        }

      }

      function getPriceDetails(userObj){
       return function (callback) {

        Transaction.details().then((appList) => {

          axios.get(appList.e_currency_price_api)
          .then(function (response) {
                            // handle success
                            console.log("ok"+ CircularJSON.stringify(response.data));
                            console.log("ok1"+response.data);
                            let dollar_value=( userObj.coin_number / appList.coin_price_usd )  +   ( ( userObj.coin_number  / appList.coin_price_usd ) * appList.transaction_fee_deposit);
                            console.log("dollar_value"+dollar_value);
                            let btc_value=dollar_value / response.data;
                            console.log("btc_value"+btc_value);
                            let resultValue={
                              dollar_amount:dollar_value,
                              btc_amount:btc_value,
                              transaction_fee:appList.transaction_fee_withdrawl
                            }

                            callback (null,resultValue);
                          })
          .catch(function (error) {
                            // handle error
                            console.log(error);
                          })
          .then(function () {
                            // always executed
                          });

        }).catch(fetchErr => {
          callback (fetchErr,null);
        });

         /*axios.get('https://www.qqxm.com/arz/currencyprice.cfm?gtc=dsjS3')
          .then(function (response) {
            // handle success
            console.log("ok"+ CircularJSON.stringify(response.data));
            console.log("ok1"+response.data);
             //callback (null,response);
          })
          .catch(function (error) {
            // handle error
            console.log(error);
          })
          .then(function () {
            // always executed
          });*/

        }

      }

      function getWalletDetails(userObj){
       return function (callback) {

        Transaction.details().then((appList) => {

          callback (null,appList);

        }).catch(fetchErr => {
          callback (fetchErr,null);
        });

        
      }

    }
    function apiTransactionStatusCheck(transObj){
      return function (callback) {

      axios.get('https://www.qqxm.com/arz/wallet.cfm?key=dhsDJDKkdhjsjatSTYI3ks&type=Check&transid=123456')
          .then(function (response) {
            // handle success
            console.log("ok"+(response.data));
            console.log("ok1"+CircularJSON.stringify(response));
             let lastPart = response.data.split("-->").pop();
             console.log("lastPart"+lastPart);
             let sendMsg;
             let sendVariable;
             if(lastPart=="New"){
                sendMsg="This transaction has not been completed yet. Please make sure you have send coins to the right address."


             }

             else if(lastPart == "Completed"){
               sendMsg="This transaction has been completed."
                //update db


             }
             else if(lastPart == "Expired"){
               sendMsg="This transaction has expired. Please send another request."
                //update db
             }
                sendVariable={
                  message:sendMsg,
                  transactionId:transObj._id
                }
                
             callback (null,sendVariable);
          })
          .catch(function (error) {
            // handle error
            console.log(error);
          })
          .then(function () {
            // always executed
          });


       }
    }
    function updateDeposit(transObj,callback){
      
      AddTrans.updateTransactionConfirm({_id:transObj.transactionId},{status:"Completed"}).then((appList) => {

        callback (null,transObj);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
      


   
 }

 function cancelDeposit(transObj,callback){
      
      AddTrans.updateTransactionConfirm({_id:transObj.transactionId},{status:"Cancelled"}).then((appList) => {

        callback (null,transObj);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
      


   
 }

 function fetchCoinUser(transObj,callback){
      
      AddTrans.chkValidTransaction({_id:transObj.transactionId}).then((appList) => {
        console.log("appList in fetch coin user"+JSON.stringify(appList));
        callback (null,appList);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
      


   
 }

 function updateCoinUser(userObj,callback){
      
      User.updateUserCoinTransaction({userName:userObj.user_name},userObj.amount).then((appList) => {

        callback (null,appList);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
      


   
 }
    function updateDepositold(transObj){
      AddTrans.updateTransactionConfirm(transObj,{status:"Completed"}).then((appList) => {

        callback (null,appList);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });

    }

    function checkTransactionStatus(transObj){
      AddTrans.chkValidTransaction(transObj).then((transactionValidStatus) => {
        if(transactionValidStatus.status=='Completed'){
          callback ({message:"Not valid"},null);
        }
        else{
          callback (null,transObj);
        }


      }).catch(fetchErr => {
        callback (fetchErr,null);
      });

    }

    exports.currencyPrice= function(req,res){
      if(!req.body.coin_number || !req.body.userEmail  ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
      }

      var userObj  ={coin_number: req.body.coin_number}
      async.waterfall([
            //getWalletDetails(userObj),
            getPriceDetails(userObj)
            
            ],
            function (err, result) {
              if(result){
                res.send(response.generate(constants.SUCCESS_STATUS,
                  result, 'Currency details fetched successfully !!'));
              }else{
                res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
              }
            });
    }
    function getCoinMinimumDeposit(userObj,callback){
      console.log("userObj"+userObj.startCoin)
    //return function (callback) {

      Transaction.details().then((appList) => {
       console.log("appList"+appList)
       let resObj={
        minimum_deposit:appList.minimum_deposit,
        minimum_withdrawl:appList.minimum_withdrawl,
        user_total_coin:userObj.startCoin
      }
      callback (null,resObj);

    }).catch(fetchErr => {
      callback (fetchErr,null);
    });


   // }
 }

 function getUserCoins(userObj){
    //findDetailsByEmail
    return function (callback) {

      User.findDetailsByEmail({email:userObj.email}).then((appList) => {

        callback (null,appList);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });


    }
  }


  exports.getUserDetails = function (req,res) {
    if(!req.body.userEmail ){
      return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }

    var userObj  ={email: req.body.userEmail}
    async.waterfall([
      getUserCoins(userObj),
      getCoinMinimumDeposit

      ],
      function (err, result) {
        if(result){
          res.send(response.generate(constants.SUCCESS_STATUS,
            result, 'User details fetched successfully !!'));
        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }
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

//This will hit after request button click in deposit form
exports.requestDeposit = function (req,res) {
  if(!req.body.userEmail || !req.body.coinAmount){
    return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
  }
    //var userObj  ={email: req.body.userEmail,coinAmount:req.body.coinAmount}

    var userObj  ={user_name:req.body.user_name,amount_usd:req.body.amount_usd,email: req.body.userEmail,coinAmount:req.body.coinAmount}
    async.waterfall([
      saveDepositV1(userObj)


      ],
      function (err, result) {
        if(result){
          res.send(response.generate(constants.SUCCESS_STATUS,
            result, 'User details fetched successfully !!'));
        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }
      });



  };

//THis will hit after confirm button hit in coin deposit

exports.confirmDeposit = function (req,res) {
  if(!req.body.userEmail || !req.body.transactionId){
    return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
  }
    //var userObj  ={email: req.body.userEmail,coinAmount:req.body.coinAmount}

    var userObj  ={_id:mongoose.Types.ObjectId(req.body.transactionId)}
    async.waterfall([
      apiTransactionStatusCheck(userObj),
      updateDeposit,
      fetchCoinUser,
      updateCoinUser


      ],
      function (err, result) {
        if(result){
          res.send(response.generate(constants.SUCCESS_STATUS,
            result, 'User transaction details updated successfully !!'));
        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }
      });



  };
  //This will hit after cancel button click in deposit 
  exports.cancelDeposit = function (req,res) {
  if(!req.body.userEmail || !req.body.transactionId){
    return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
  }
    //var userObj  ={email: req.body.userEmail,coinAmount:req.body.coinAmount}

    var userObj  ={_id:mongoose.Types.ObjectId(req.body.transactionId)}
    async.waterfall([
      apiTransactionStatusCheck(userObj),
      cancelDeposit
      


      ],
      function (err, result) {
        if(result){
          res.send(response.generate(constants.SUCCESS_STATUS,
            result, 'User transaction details updated successfully !!'));
        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }
      });



  };




