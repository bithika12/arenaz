var express = require('express');
var router = express.Router();
var authetication= require("../middleware/Auth");
var adminController = require('../controllers/AdminController');



//ADMIN LOGIN
 router.post('/login',adminController.login);
//router.post('',adminController.login);



module.exports = router;
