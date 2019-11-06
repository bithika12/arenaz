var express = require('express');
var router = express.Router();
//var fileUpload= require("../middleware/FileUpload");
var authetication= require("../middleware/Auth");
//var io;
/* TIME  ZONE SET*/
process.env.TZ = 'Asia/Kolkata' ;


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

  /**  LOGIN ROUTING **/
  router.post('/registration',userController.registration);
  //router.post('/socal-login',userController.socialLogin);
  router.post('/login',userController.login);
  router.post('/logout',authetication.authChecker,userController.logout);

 /* FORGET PASSWORD RESET PASSWORD*/
  //RESET PASSWORD
  //router.post('/forget-password-generate-otp',resetPasswordController.sendResetOtp);
  //router.post('/verify-forget-otp',resetPasswordController.verifyOtp);
  //router.post('/reset-password',resetPasswordController.resetPassword);

/* PROFILES DETAILS*/
 // router.post('/profile-details',authetication.authChecker,profileController.profiledDetails);
 // router.post('/update-profile',authetication.authChecker,profileController.updateProfile);
 // router.post('/update-password',authetication.authChecker,profileController.updatePassword);
 // router.post('/profile-image-upload',authetication.authChecker,profileController.profiledDetails);
// router.post('/leader-board',authetication.authChecker,profileController.leaderBoard);

//ADMIN LOGIN
  router.post('/admin/login',adminController.login);







module.exports = router;
