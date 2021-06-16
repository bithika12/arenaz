/**  Import Package**/
var async = require('async');
/* Import */
var constants = require("../config/constants");
var mongoose = require('mongoose');
/* UTILS PACKAGE*/
const validateInput = require('../utils/ParamsValidation');
const response  = require('../utils/ResponseManeger');
//const jwtTokenManage   = require('../utils/JwtTokenManage');
//let Notification  = require(appRoot +'/models/Notification');
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
let Notification  = require(appRoot +'/models/Notification');
let Transaction1 = require(appRoot +'/schema/Schema').userTransactionModel;


  function saveDepositV1(userObj){
   return function (callback) {
    //first add transaction
    var transactionObjV1 = {
                              user_name : userObj.user_name,                              
                              user_email : userObj.email,                              
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

                            console.log("axios response"+response)
                            
                            let lastPart = response.data.split("-->").pop();
                            
                            let responsePart = lastPart.split(',')[0];
                            let transactionPart=lastPart.split(',')[1];

                            console.log("responsePart"+responsePart);

                            console.log("transactionPart"+transactionPart);
                            //save it to transaction
                            if(lastPart=="Error!"){
                               //delete transaction record
                                console.log("err");
                            }
                            else {
                            var travelTime = moment().add(appList.api_expiration_time, 'minutes').format('hh:mm A');
                            //var travelTime = moment().add(30, 'minutes').format('hh:mm A');

                            

                            console.log("travelTime"+travelTime);
                            let curtime= moment().format('hh:mm A');
                            console.log("curtime"+curtime);
                            let curdate= moment().format('YYYY-MM-DD');
                            console.log("curdate"+curdate);
                            let expired_at=curdate+" "+travelTime;

                            var x = new moment(expired_at);
                            var y = new moment();
                            var duration = moment.duration(x.diff(y)).as('minutes');
                            var duration_second = Math.round(duration * 60);
                            
                            var transactionObj = {
                              //user_name : userObj.user_name,
                              //user_confirmation : lastPart,
                              user_confirmation : responsePart,
                              //amount : userObj.coinAmount,
                              //amount_usd : userObj.amount_usd,
                              transaction_key : lastPart,
                              //status : "New",
                              expired_at : expired_at,
                              transaction_id:transactionPart                                
                              //type : "Deposit"
                            }


                            AddTrans.updateTransactionConfirm({_id:mongoose.Types.ObjectId(resTransV1)},transactionObj).then((resTrans) => { 
                             let responseObj={
                               address_part:lastPart,
                               expired_at:expired_at,
                               //expired_at_inMinute:duration,
                               expired_at_inSecond:duration_second,
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

        }

      }

      function getPriceDetails(userObj){
       return function (callback) {

        Transaction.details().then((appList) => {
          console.log(appList);
          axios.get(appList.e_currency_price_api).then(function (response) {
            // handle success
            console.log("ok => "+ CircularJSON.stringify(response.data));
            console.log("ok1 => "+response.data);
           
            if(userObj.transaction_type=='withdraw'){
              var t_fee = appList.transaction_fee_withdrawl;

              var dollar_value=(userObj.coin_number/appList.coin_price_usd)-((userObj.coin_number/appList.coin_price_usd)*(t_fee/100));
            }else{
              var t_fee = appList.transaction_fee_deposit;

              var dollar_value=(userObj.coin_number/appList.coin_price_usd)+((userObj.coin_number/appList.coin_price_usd)*(t_fee/100));
            }
            
            let btc_value=dollar_value / response.data;
            console.log("btc_value => "+btc_value);


            let resultValue={
              dollar_amount:dollar_value,
              btc_amount:btc_value.toFixed(8),
              transaction_fee:t_fee
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
        Transaction.details().then((appList) => {
          let api_url=appList.wallet_api_link+appList.wallet_key+
                         "&type=Check&transid="+transObj._id;
          axios.get(api_url).then(function (response) {
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
          }).catch(fetchErr => {
            callback (fetchErr,null);
          });

       }
    }

  function updateDeposit(transObj,callback){
      
      AddTrans.updateTransactionConfirm({_id:transObj.transactionId},{status:transObj.apiStat}).then((appList) => {

        callback (null,transObj);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
  }

  function cancelDeposit(transObj){
    return function(callback){  
      AddTrans.updateTransactionConfirm({_id:transObj.transactionId},{status:"Cancelled"}).then((appList) => {

        callback (null,transObj);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
    }
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
        userObj.coinstatus = 'Updated';
        callback (null,userObj);

      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
  }

  function updateCoinUserDeposit(userObj){
      return function(callback){
        User.updateUserCoinTransaction({userName:userObj.user_name},userObj.amount).then((appList) => {
          userObj.coinstatus = 'Updated';
          callback (null,userObj);

        }).catch(fetchErr => {
          callback (fetchErr,null);
        });
      }
  }
   function updateCoinUserWithdraw(userObj,callback){
      //return function(callback){
        console.log("update call"+JSON.stringify(userObj));
        User.updateUserCoinTransactionWithDraw({userName:userObj.user_name},userObj.amount).then((appList) => {
          console.log("ok12");
          userObj.result=appList;
          callback (null,userObj);

        }).catch(fetchErr => {
          callback (fetchErr,null);
        });
     // }
  }
  
  function updateCoinUserWithdrawold(userObj,callback){
      return function(callback){
        console.log("update call");
        User.updateUserCoinTransactionWithDraw({userName:userObj.user_name},userObj.amount).then((appList) => {
          
          callback (null,appList);

        }).catch(fetchErr => {
          callback (fetchErr,null);
        });
      }
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
      if(!req.body.coin_number || !req.body.userEmail || !req.body.transactionType ){
        return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
      }

      Transaction.details().then((appList) => {
    
        var userObj  ={coin_number: req.body.coin_number,transaction_type: req.body.transactionType,email: req.body.userEmail}
       
        if(req.body.transactionType=='withdraw'){
            if(parseInt(req.body.coin_number) < parseInt(appList.minimum_withdrawl) && appList.allow_mini_account_withdrawal=="No"){
              return res.send(response.error(constants.VALIDATION_CHECK_ERR,{},"Coins will be equal or more than minimum withdrawal amount"));
            }else{
              async.waterfall([
                getPriceDetails(userObj),
              ],
              function (err, result) {
                if(result){
                    var userObj1  ={email: req.body.userEmail}
                    async.waterfall([
                      getUserCoins(userObj1),
                    ],
                    function (err, result1) {
                      if(result1){
                        if(parseInt(req.body.coin_number) > parseInt(result1.startCoin)){
                          return res.send(response.error(constants.VALIDATION_CHECK_ERR,{},"Coins entered should not be more than the existing coins"));
                        }else{
                          res.send(response.generate(constants.SUCCESS_STATUS,result, 'Currency details fetched successfully !!'));
                        }
                      }else{
                        res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
                      }
                    });
                }else{
                  res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
                }
              });
          }
        }else{

          if(parseInt(req.body.coin_number) < parseInt(appList.minimum_deposit) && appList.allow_mini_account_withdrawal=="No"){
            return res.send(response.error(constants.VALIDATION_CHECK_ERR,{},"Coins will be equal or more than minimum deposit amount"));
          }else{
            async.waterfall([
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
        }

      }).catch(fetchErr => {
      
        callback (fetchErr,null);
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
        user_total_coin:userObj.startCoin,
        transaction_fee_withdrawl:appList.transaction_fee_withdrawl,
        transaction_fee_deposit:appList.transaction_fee_deposit,
        master_message:appList.master_message,
        allow_mini_account_withdrawal:appList.allow_mini_account_withdrawal,
        support_email:appList.support_email,
        market_volatility:appList.market_volatility
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
  if(!req.body.userEmail || !req.body.coinAmount || !req.body.amount_usd || !req.body.user_name){
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
          result, 'Transaction details fetched successfully !!'));

          let userObj1={user_email: req.body.userEmail,_id: mongoose.Types.ObjectId(result.transactionId)}

          var x = new moment(result.expired_at);
          var y = new moment();
          var duration = moment.duration(x.diff(y)).as('minutes');
          var duration_second = duration * 60;

          // Interval checking for status and update
          //var intervalCheck = setInterval(function(){ 

             async.waterfall([
                getTransactionStatus(userObj1),
                apiCheckTransactionStatusUpdate,
                function (condition, callback) {
                  console.log('===========> Check Condition');
                  console.log(condition);
                    if (condition.dbStat == 1) {
                        async.waterfall([
                            noCoinUpdate(condition),
                            /*updated on 18.03.21*/
                            //updateCoinUserDeposit(condition),
                        ], callback);
                    }else{
                       async.waterfall([
                            noCoinUpdate(condition),
                        ], callback);
                    }
                },
                ],function (err, result) {
                    console.log('Successfully---Working....');
                    console.log(result);
                   // console.log(err);
                    if((duration < 0) || (result.status=='Expired') || (result.status=='Completed') || (result.status=='Cancelled')){
                      console.log('================ Stop Interval Timer ======================');
                      //clearInterval(intervalCheck);
                    }
                });
           
           //}, 30000);


        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }

      });
  };

//This will hit after confirm button hit in coin deposit

exports.confirmDeposit = function (req,res) {
  //let a1="2021-03-16T07:56:46.359Z";
  //let time=moment(a1).format('MM/DD/YYYY hh:mm a');
  //console.log(time);
  if(!req.body.userEmail || !req.body.transactionId){
    return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
  }
  var userObj  ={user_email: req.body.userEmail,_id: mongoose.Types.ObjectId(req.body.transactionId)}
  async.waterfall([
    getTransactionStatus(userObj),
    apiCheckTransactionStatusUpdate,
    function (condition, callback) {
        if (condition.dbStat == 1) {
            async.waterfall([
                /*updated on 18.03.21*/
                  noCoinUpdate(condition),
                //updateCoinUserDeposit(condition),
            ], callback);
        }else{
           async.waterfall([
                noCoinUpdate(condition),
            ], callback);
        }
    },
  ],
  function (err, result) {
    if(result){
      console.log("resultconfirm"+JSON.stringify(result));
      User.findDetailsByEmail({email:result.user_email}).then((userDet)=>{
        console.log("userDet"+userDet._id);
        let msg="You have requested to buy "+result.amount+" coins on "+moment(result.created_at).format('MM/DD/YY hh:mm a')+" . Your receive code is "+ result.user_confirmation+" .  You will only have 22 minutes to make this transaction before this request expires. Once you have completed the transaction, your account will reflect the new coins the next time you enter the app.";
        Notification.createNotification({
                        //sent_by_user     : req.user_id ,
                        received_by_user : userDet._id,
                        subject          : "Confirm Deposit",
                        message          : msg,
                        read_unread      : 0
                    }).then(function(notificationdetails){
                       res.send(response.generate(constants.SUCCESS_STATUS,
                       result, 'Transaction confirmed successfully !!'));
                    });

      

      });

      
      /*res.send(response.generate(constants.SUCCESS_STATUS,
        result, 'Transaction confirmed successfully !!'));
       */
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

    var userObj  ={transactionId:mongoose.Types.ObjectId(req.body.transactionId)}

    async.waterfall([
      cancelDeposit(userObj),
      ],
      function (err, result) {
        if(result){
          res.send(response.generate(constants.SUCCESS_STATUS,
            result, 'User transaction cancelled successfully !!'));
        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }
      });
};

  /* ---------------- 03.11.2020 ------------------ */
  exports.requestWithdraw123 = function (req,res) {
  if(!req.body.userEmail || !req.body.coinAmount || !req.body.wallet_key){
    return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
  }
  
  var userObj  ={user_name:req.body.user_name,amount_usd:req.body.amount_usd,email: req.body.userEmail,coinAmount:req.body.coinAmount,transaction_key:req.body.wallet_key}
    async.waterfall([
      saveWithdrawRequest(userObj),
      ],
      function (err, result) {
        if(result){
          console.log("res123"+JSON.stringify(result));
          var userObj1  ={user_name:req.body.user_name,amount:req.body.coinAmount}
          async.waterfall([
            updateCoinUserWithdraw(userObj1),
            //fetchCoinUser
            ],function (err, result1) {
              if(result1){                

                resultObj={
                  transaction_details : result,
                  user_details        : result1
                }
                res.send(response.generate(constants.SUCCESS_STATUS,resultObj, 'Transaction details fetched successfully !!'));
              

              }else{
                res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
              }

            });

        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }

      });
  };

  exports.requestWithdraw = function (req,res) {
  if(!req.body.userEmail || !req.body.coinAmount || !req.body.wallet_key){
    return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
  }

  Transaction.details().then((appList) => {

  if(appList.allow_mini_account_withdrawal=="Yes"){

    console.log("allow_mini_account_withdrawal"+appList.allow_mini_account_withdrawal);
       

       var userObj  ={user_name:req.body.user_name,amount_usd:req.body.amount_usd,email: req.body.userEmail,coinAmount:req.body.coinAmount,transaction_key:req.body.wallet_key}
       async.waterfall([
      saveWithdrawRequest(userObj),
      updateCoinUserWithdraw
      ],
      function (err, result) {
        if(result){
          console.log("res123"+JSON.stringify(result))
          //var userObj1  ={user_name:req.body.user_name,amount:req.body.coinAmount}
          /*async.waterfall([
            updateCoinUserWithdraw(userObj1),
            //fetchCoinUser
            ],function (err, result1) {*/
              if(result){
                //console.log("plll12"+JSON.stringify(result1));
                Transaction.trandetails({_id:mongoose.Types.ObjectId(result.transactionId)}).then(transactionValidStatus=> {
                  transactionValidStatus=transactionValidStatus[0];
                  console.log("transactionValidStatus"+transactionValidStatus);
                //AddTrans.chkTransactionStatus({_id:mongoose.Types.ObjectId(result1.transaction_details.transactionId)}).then((transactionValidStatus) => {
                              //callback (null,transactionValidStatus);
                   User.findDetailsByEmail({email:req.body.userEmail}).then((userDet)=>{
                  console.log("userDet"+userDet._id);
                  let msg="You have requested to withdraw "+transactionValidStatus.amount+" coins on "+moment(transactionValidStatus.created_at).format('MM/DD/YY hh:mm a')+". The receive code submitted is "+transactionValidStatus.user_confirmation+".  The transaction will complete within 48 hours. Once we process your request the transaction status will be set to ‘Completed’.  If there are any issues the transaction will be cancelled and coins returned back to your account."
                  //let msg="You have requested to withdraw "+transactionValidStatus.amount+" coins on "+moment(transactionValidStatus.created_at).format('MM/DD/YYYY hh:mm a')+" . Your receive code is %"+ transactionValidStatus.user_confirmation+" %.  You will only have 30 minutes to make this transaction before this request expires. Once you have completed the transaction, your account will reflect the new coins once you restart the app.";
                  Notification.createNotification({
                                  //sent_by_user     : req.user_id ,
                                  received_by_user : userDet._id,
                                  subject          : "Confirm Withdraw",
                                  message          : msg,
                                  read_unread      : 0
                              }).then(function(notificationdetails){
                                 resultObj={
                                  transaction_details : result,
                                  user_details        : result.result
                               }
                              res.send(response.generate(constants.SUCCESS_STATUS,resultObj, 'Transaction details fetched successfully !!'));
                              });

      

                   });


                }).catch(fetchErr => {
                  console.log("fetchErr"+fetchErr);
                 res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));

                  //callback (fetchErr,null);
                });

                /*resultObj={
                  transaction_details : result,
                  user_details        : result1
                }
                res.send(response.generate(constants.SUCCESS_STATUS,resultObj, 'Transaction details fetched successfully !!'));
              */

              }else{
                res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
              }

            //});

        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }

      });

   }

else{
  console.log("allow_mini_account_withdrawal"+appList.allow_mini_account_withdrawal);
   User.findDetailsByEmail({email:req.body.userEmail}).then((userDet)=>{
   //check user current balance
    if(userDet.startCoin < appList.minimum_withdrawl ){
      //error
      res.send(response.error(constants.ERROR_STATUS,{},"User has insuficient balance to withdraw!!"));

    }
    else{

       var userObj  ={user_name:req.body.user_name,amount_usd:req.body.amount_usd,email: req.body.userEmail,coinAmount:req.body.coinAmount,transaction_key:req.body.wallet_key}
       async.waterfall([
      saveWithdrawRequest(userObj),
      updateCoinUserWithdraw
      ],
      function (err, result) {
        if(result){
          console.log("res123"+JSON.stringify(result))
          //var userObj1  ={user_name:req.body.user_name,amount:req.body.coinAmount}
          /*async.waterfall([
            updateCoinUserWithdraw(userObj1),
            //fetchCoinUser
            ],function (err, result1) {*/
              if(result){
                //console.log("plll12"+JSON.stringify(result1));
                Transaction.trandetails({_id:mongoose.Types.ObjectId(result.transactionId)}).then(transactionValidStatus=> {
                  transactionValidStatus=transactionValidStatus[0];
                  console.log("transactionValidStatus"+transactionValidStatus[0]);
                //AddTrans.chkTransactionStatus({_id:mongoose.Types.ObjectId(result1.transaction_details.transactionId)}).then((transactionValidStatus) => {
                              //callback (null,transactionValidStatus);
                   User.findDetailsByEmail({email:req.body.userEmail}).then((userDet)=>{
                  console.log("userDet"+userDet._id);
                  let msg="You have requested to withdraw "+transactionValidStatus.amount+" coins on "+moment(transactionValidStatus.created_at).format('MM/DD/YYYY hh:mm a')+". The receive code submitted is "+transactionValidStatus.user_confirmation+".  The transaction will complete within 48 hours. Once we process your request the transaction status will be set to ‘Completed’.  If there are any issues the transaction will be canceled and coins returned back to your account."
                  //let msg="You have requested to withdraw "+transactionValidStatus.amount+" coins on "+moment(transactionValidStatus.created_at).format('MM/DD/YYYY hh:mm a')+" . Your receive code is %"+ transactionValidStatus.user_confirmation+" %.  You will only have 30 minutes to make this transaction before this request expires. Once you have completed the transaction, your account will reflect the new coins once you restart the app.";
                  Notification.createNotification({
                                  //sent_by_user     : req.user_id ,
                                  received_by_user : userDet._id,
                                  subject          : "Confirm Withdraw",
                                  message          : msg,
                                  read_unread      : 0
                              }).then(function(notificationdetails){
                                 resultObj={
                                  transaction_details : result,
                                  user_details        : result.result
                               }
                              res.send(response.generate(constants.SUCCESS_STATUS,resultObj, 'Transaction details fetched successfully !!'));
                              });

      

                   });


                }).catch(fetchErr => {
                  console.log("fetchErr"+fetchErr);
                 res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));

                  //callback (fetchErr,null);
                });

                /*resultObj={
                  transaction_details : result,
                  user_details        : result1
                }
                res.send(response.generate(constants.SUCCESS_STATUS,resultObj, 'Transaction details fetched successfully !!'));
              */

              }else{
                res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
              }

            //});

        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }

      });


    }


   });

    
   
}

}).catch(fetchErr => {
     callback (fetchErr,null);
});
  
  
  };
  //This will hit after request button click in deposit form
exports.requestWithdrawRunning = function (req,res) {
  if(!req.body.userEmail || !req.body.coinAmount || !req.body.wallet_key){
    return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
  }
  
  var userObj  ={user_name:req.body.user_name,amount_usd:req.body.amount_usd,email: req.body.userEmail,coinAmount:req.body.coinAmount,transaction_key:req.body.wallet_key}
    async.waterfall([
      saveWithdrawRequest(userObj),
      updateCoinUserWithdraw
      ],
      function (err, result) {
        if(result){
          console.log("res123"+JSON.stringify(result))
          //var userObj1  ={user_name:req.body.user_name,amount:req.body.coinAmount}
          /*async.waterfall([
            updateCoinUserWithdraw(userObj1),
            //fetchCoinUser
            ],function (err, result1) {*/
              if(result){
                //console.log("plll12"+JSON.stringify(result1));
                Transaction.trandetails({_id:mongoose.Types.ObjectId(result.transactionId)}).then(transactionValidStatus=> {
                  console.log("transactionValidStatus"+transactionValidStatus);
                //AddTrans.chkTransactionStatus({_id:mongoose.Types.ObjectId(result1.transaction_details.transactionId)}).then((transactionValidStatus) => {
                              //callback (null,transactionValidStatus);
                   User.findDetailsByEmail({email:req.body.userEmail}).then((userDet)=>{
                  console.log("userDet"+userDet._id);
                  let msg="You have requested to withdraw "+transactionValidStatus.amount+" coins on "+moment(transactionValidStatus.created_at).format('MM/DD/YYYY hh:mm a')+". The receive code submitted is "+transactionValidStatus.user_confirmation+".  The transaction will complete within 48 hours. Once we process your request the transaction status will be set to ‘Completed’.  If there are any issues the transaction will be canceled and coins returned back to your account."
                  //let msg="You have requested to withdraw "+transactionValidStatus.amount+" coins on "+moment(transactionValidStatus.created_at).format('MM/DD/YYYY hh:mm a')+" . Your receive code is %"+ transactionValidStatus.user_confirmation+" %.  You will only have 30 minutes to make this transaction before this request expires. Once you have completed the transaction, your account will reflect the new coins once you restart the app.";
                  Notification.createNotification({
                                  //sent_by_user     : req.user_id ,
                                  received_by_user : userDet._id,
                                  subject          : "Confirm Withdraw",
                                  message          : msg,
                                  read_unread      : 0
                              }).then(function(notificationdetails){
                                 resultObj={
                                  transaction_details : result,
                                  user_details        : result.result
                               }
                              res.send(response.generate(constants.SUCCESS_STATUS,resultObj, 'Transaction details fetched successfully !!'));
                              });

      

                   });


                }).catch(fetchErr => {
                  console.log("fetchErr"+fetchErr);
                 res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));

                  //callback (fetchErr,null);
                });

                /*resultObj={
                  transaction_details : result,
                  user_details        : result1
                }
                res.send(response.generate(constants.SUCCESS_STATUS,resultObj, 'Transaction details fetched successfully !!'));
              */

              }else{
                res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
              }

            //});

        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }

      });
  };

//This will hit after confirm button hit in coin deposit



   exports.checkTransactionStatus = function (req,res) {

    if(!req.body.userEmail || !req.body.transactionId){
      return res.send(response.error(constants.PARAMMISSING_STATUS,{},"Parameter Missing!"));
    }

    var userObj  ={user_email: req.body.userEmail,_id: mongoose.Types.ObjectId(req.body.transactionId)}
    async.waterfall([
      getTransactionStatus(userObj),
      apiCheckTransactionStatusUpdate,
        function (condition, callback) {
          console.log('===========> Check Condition');
          console.log(condition);
            if (condition.dbStat == 1) {
                async.waterfall([
                    updateCoinUserDeposit(condition),
                ], callback);
            }else{
               async.waterfall([
                    noCoinUpdate(condition),
                ], callback);
            }
        },
      ],
      function (err, result) {
        console.log('Result Generated........');
        console.log(result);
        if(result){
          res.send(response.generate(constants.SUCCESS_STATUS,
            result, 'Transaction Status fetched successfully !!'));
        }else{
          res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
        }
      });
  };


  function getTransactionStatus(userObj){
     return function (callback) {
      AddTrans.chkTransactionStatus(userObj).then((transactionValidStatus) => {
          callback (null,transactionValidStatus);
      }).catch(fetchErr => {
        callback (fetchErr,null);
      });
    }
  }

  function apiCheckTransactionStatusUpdate(userObj,callback){
    console.log("userObj"+JSON.stringify(userObj));
    Transaction.details().then((appList) => {
      let api_url=appList.wallet_api_link+appList.wallet_key+"&type=Check&transid="+userObj.transaction_id;
      //let api_url=appList.wallet_api_link+appList.wallet_key+"&type=Check&transid="+userObj._id;


        axios.get(api_url).then(function (response) {
          // handle success
          //console.log("ok"+(response.data));
          //console.log("ok1"+CircularJSON.stringify(response));
          let lastPart = response.data.split("-->").pop();
          if(lastPart == 'Completed,500'){
            lastPart = lastPart.split(",", 1)[0];
          }
          console.log("lastPart"+lastPart);
          
          var newresponse = {
                              _id               : userObj._id,
                              user_name         : userObj.user_name,
                              user_email        : userObj.user_email,
                              amount            : userObj.amount,
                              status            : userObj.status,
                              expired_at        : userObj.expired_at,
                             // expire_in_minute  : userObj.expire_in_minute
                              expire_at_inSecond  : userObj.expire_at_inSecond,
                              created_at:userObj.created_at,
                              user_confirmation:userObj.user_confirmation 
                            }

          if(userObj.status == 'Expired'){ 
            console.log('Transaction Already expired in system!');
            newresponse.apiStat = lastPart;
            newresponse.dbStat  = 5;
            newresponse.msg     = 'Transaction Already expired!';
            callback (null,newresponse);                   
            
          }else if(userObj.status == 'Cancelled'){
            console.log('Transaction Already cancelled!');
            newresponse.apiStat = lastPart;
            newresponse.dbStat  = 6;
            newresponse.msg     = 'Transaction Already cancelled!';
            callback (null,newresponse);
          }else{
            if(lastPart==userObj.status){
              console.log('Status Same in DB and API');

              newresponse.apiStat = lastPart;
              newresponse.dbStat  = 0;
              newresponse.msg     = 'Status Same in DB and API';
              callback (null,newresponse);
            }else{
              console.log('Status are different in DB and API');

              if((userObj.status == "New") && (lastPart == "Completed")){
                console.log('Transaction Completed from New');
                //AddTrans.updateTransactionConfirm({_id:userObj._id},{status:lastPart}).then((appList) => {

                  newresponse.apiStat = lastPart;
                  newresponse.dbStat  = 1;
                  newresponse.msg     = 'Transaction Completed from New';
                  callback (null,newresponse);
                /*}).catch(fetchErr => {
                  callback (fetchErr,null);
                });*/
              }else if((userObj.status == "New") && (lastPart == "Expired")){
                console.log('Transaction Expired from New');
               
                //AddTrans.updateTransactionConfirm({_id:userObj._id},{status:lastPart}).then((appList) => {
                  newresponse.apiStat = lastPart;
                  newresponse.dbStat  = 2;
                  newresponse.msg     = 'Transaction Expired from New';
                  callback (null,newresponse);
                /*}).catch(fetchErr => {
                  callback (fetchErr,null);
                });*/
                
              }else if((userObj.status == "Completed") && (lastPart == "Expired")){
                console.log('Transaction Expired from Completed');
                
                 //AddTrans.updateTransactionConfirm({_id:userObj._id},{status:lastPart}).then((appList) => {
                  newresponse.apiStat = lastPart;
                  newresponse.dbStat  = 3;
                  newresponse.msg     = 'Transaction Expired from Completed';
                  callback (null,newresponse);
                /*}).catch(fetchErr => {
                  callback (fetchErr,null);
                });*/

                callback (null,newresponse);

              }else if(userObj.status == "Expired"){
                
                console.log('Transaction Already expired!');
                newresponse.apiStat = lastPart;
                newresponse.dbStat  = 4;
                newresponse.msg     = 'Transaction Already expired!';
                callback (null,newresponse);
              }
            }

          }  
        }).catch(function (error) {
            
            console.log(error);
        });
    }).catch(fetchErr => {
      
      callback (fetchErr,null);
    });
  }

function noCoinUpdate(newresponse,callback){
    return function (callback) {
      console.log(newresponse);
      console.log('Call back return');
    callback (null,newresponse);
  }
}



function saveWithdrawRequest(userObj){
   
   return function (callback) {
    
    var transactionObjV1 = {
                              user_name : userObj.user_name,                              
                              user_email : userObj.email,                              
                              amount : userObj.coinAmount,
                              amount_usd : userObj.amount_usd,                              
                              transaction_key : userObj.transaction_key,                              
                              status : "New",                                                             
                              type : "Withdraw"
                            }

      AddTrans.addTransaction(transactionObjV1).then((resTransV1) => { 

        Transaction.details().then((appList) => {
          //let api_url="https://www.qqxm.com/arz/wallet.cfm?key=dhsDJDKkdhjsjatSTYI3ks&type=Withdraw&transid=%idoftrans%&amt=%amount_usd%&rcv_wlt=sjdkSHWYEIDHD";
          let api_url=appList.wallet_api_link+appList.wallet_key+"&type=Withdraw&transid="+resTransV1+"&amt="+userObj.coinAmount+"&rcv_wlt="+userObj.transaction_key;

          console.log("api_url"+api_url)

            axios.get(api_url).then(function (response) {
                // handle success
                console.log("ok"+ CircularJSON.stringify(response.data));
                console.log("ok1"+response.data);
                
                let lastPart = response.data.split("-->").pop();
                console.log("API Status ==>> "+lastPart);

                if(lastPart=="Error!"){
                   var api_status = 'Error';
                }
                if(lastPart=="Success!"){
                   var api_status = 'New';
                }
                //appList.api_expiration_time
                var travelTime  = moment().add(appList.api_expiration_time, 'minutes').format('hh:mm A');
 
                //var travelTime  = moment().add(30, 'minutes').format('hh:mm A');
                let curtime     = moment().format('hh:mm A');
                let curdate     = moment().format('YYYY-MM-DD');
                let expired_at  = curdate+" "+travelTime;
                
                var transactionObj = {
                                        user_confirmation : userObj.transaction_key,
                                        transaction_key   : userObj.transaction_key,
                                        expired_at        : expired_at, 
                                        status            : api_status                              
                                      }

                AddTrans.updateTransactionConfirm({_id:mongoose.Types.ObjectId(resTransV1)},transactionObj).then((resTrans) => { 
                  let responseObj={
                                    status       :api_status,
                                    expired_at   :expired_at,
                                    transactionId:resTransV1,
                                    user_email : userObj.email,                              
                                    amount : userObj.coinAmount,
                                    user_name : userObj.user_name,
                                  }

                  /*let responseObj={
                                    status       :api_status,
                                    expired_at   :expired_at,
                                    transactionId:resTransV1
                                  }*/
                     console.log("responseObj"+JSON.stringify(responseObj));             
                    callback (null,responseObj);
                }).catch(function (error) {
                   console.log(error);
                });   
              }).catch(function (error) {
                console.log(error);
              });
         }).catch(fetchErr => {
          callback (fetchErr,null);
        });
      }).catch(function (error) {
        console.log(error);
      }) 
    }
  }


  exports.getMasterDetails = function (req,res) {
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
        console.log("result"+JSON.stringify(result));
        let res1={
          master_message:result.master_message,
          allow_mini_account_withdrawal:result.allow_mini_account_withdrawal,
          support_email:result.support_email
        }
        res.send(response.generate(constants.SUCCESS_STATUS,
          res1, 'User details fetched successfully !!'));
      }else{
        res.send(response.error(constants.ERROR_STATUS,err,"Invalid authentication!!"));
      }
    });
};

  

