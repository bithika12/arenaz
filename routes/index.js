var express = require('express');
var router = express.Router();
//var fileUpload= require("../middleware/FileUpload");
var authetication= require("../middleware/Auth");
//var io;
/* TIME  ZONE SET*/
process.env.TZ = 'Asia/Kolkata' ;
const appRoot = require('app-root-path');


/* GET home page. */
router.get('/', function(req, res, next) {
	return res.send({"status":"1"})
 // res.render('index', { title: '' });
});



/**
 * Import All controller
 */
  var userController = require('../controllers/UserController');
  var profileController = require('../controllers/ProfileController');
  var resetPasswordController = require('../controllers/ResetPasswordController');
  var adminController = require('../controllers/AdminController');
  let forgotPasswordController=require('../controllers/ForgotPasswordController');
  const forgotPassword = require(appRoot + '/controllers/ForgotPasswordController');

  const fetchGameHistory = require(appRoot + '/controllers/GameHistoryController');
  const fetchNotification= require(appRoot + '/controllers/FetchNotificationController');
  const userLeaderBoard= require(appRoot + '/controllers/userLeaderBoardController');



/**  LOGIN ROUTING **/
  router.post('/registration',userController.registration);
  //router.post('/socal-login',userController.socialLogin);
  router.post('/login',userController.login);
  router.post('/logout',authetication.authChecker,userController.logout);

  //forgot password
router.post('/forgot/password',forgotPassword.forgotPassword);

router.post('/game/history',fetchGameHistory.fetchGame);

router.post('/add/details',profileController.colorReg);

router.post('/get/details',profileController.getColorReg);
router.post('/fetch/notifications',fetchNotification.fetchGame);
//leaderboard api
router.post('/user/board',userLeaderBoard.fetchGame);
router.post('/delete/account',userController.deleteAccount);
router.post('/change/notification/status',fetchNotification.changeStatus);
router.post('/user/game/history',fetchGameHistory.userGame);
router.post('/fetch/app/version',profileController.fetchVersion);
router.post('/fetch/unread/message',fetchNotification.fetchUnreadMessage);

 /* FORGET PASSWORD RESET PASSWORD*/
  //RESET PASSWORD
  //router.post('/forget-password-generate-otp',resetPasswordController.sendResetOtp);
  //router.post('/verify-forget-otp',resetPasswordController.verifyOtp);
  //router.post('/reset-password',resetPasswordController.resetPassword);

/* PROFILES DETAILS*/
 // router.post('/profile-details',authetication.authChecker,profileController.profiledDetails);
  //router.post('/update-profile',authetication.authChecker,profileController.updateProfile);
 // router.post('/update-password',authetication.authChecker,profileController.updatePassword);
 // router.post('/profile-image-upload',authetication.authChecker,profileController.profiledDetails);
// router.post('/leader-board',authetication.authChecker,profileController.leaderBoard);

//ADMIN LOGIN
  router.post('/admin/login',adminController.login);

  //fetch user list in admin panel
  router.post('/admin/users-list',adminController.userList);
   //admin/edit-user
  router.post('/admin/delete-user',authetication.authChecker,profileController.disableProfile);

  router.post('/admin/edit-user',authetication.authChecker,profileController.modifyProfile);
  //fetch active user
   //fetchActiveUser
  router.post('/admin/active-user'/*,authetication.authChecker*/,adminController.fetchActiveUser);
  //admin/get-role
  router.post('/admin/get-role'/*authetication.authChecker*/,adminController.getRole);

  //fetch all game list in admin panel
  router.post('/admin/game-list'/*authetication.authChecker*/,adminController.getGameList);
  //user add through admin panel
    //admin/add-user
  router.post('/admin/add-user'/*authetication.authChecker*/,adminController.addUser);
  //fetch coin
  router.post('/admin/coin-list'/*authetication.authChecker*/,adminController.getCoinList);
  //add coin
 router.post('/admin/coin-add'/*authetication.authChecker*/,adminController.addCoin);
 //admin/delete-game
 router.post('/admin/delete-game',authetication.authChecker,adminController.disableRoom);
 //delete-coin
 router.post('/admin/delete-coin'/*authetication.authChecker*/,adminController.deleteCoin);
 //admin/edit-coin
 router.post('/admin/edit-coin'/*authetication.authChecker*/,adminController.editCoin);

 //game list
router.post('/admin/fetch-game'/*authetication.authChecker*/,adminController.getMatchesList);
//admin/match-add
router.post('/admin/match-add'/*authetication.authChecker*/,adminController.addMatches);
router.post('/admin/match-edit'/*authetication.authChecker*/,adminController.editMatches);
router.post('/admin/delete-match'/*authetication.authChecker*/,adminController.deleteMatch);

//admin mailbox ///////////////
router.post('/admin/mail-list'/*authetication.authChecker*/,adminController.getMailList);
router.post('/admin/mail-add'/*authetication.authChecker*/,adminController.addMail);
router.post('/admin/mail-edit'/*authetication.authChecker*/,adminController.editMail);
router.post('/admin/delete-mail'/*authetication.authChecker*/,adminController.deleteMail);

 ///user/leaderboard

router.post('/admin/get-users'/*authetication.authChecker*/,adminController.getUsers);
router.post('/admin/get-user-coins'/*authetication.authChecker*/,adminController.getUserCoins);

router.post('/admin/add-user-coins'/*authetication.authChecker*/,adminController.addUserCoins);
//updateVersion
router.post('/update/app/version',profileController.updateVersion);

module.exports = router;
