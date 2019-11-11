 var constants = require("../config/constants");

let email = (email) => {
    let emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/
    if (email.match(emailRegex)) {
      return email
    } else {
      return false
    }
  }
  
    /* Minimum 8 characters which contain only characters,numeric digits, underscore and first character must be a letter */
  let password = (password) => {
  //  let passwordRegex = /^[A-Za-z0-9]\w{7,}$/
    let  passwordRegex ="^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).{8,16}$";
    if (password.match(passwordRegex)) {
      return password
    } else {
      return false
    }
  }
  let userName = (username) => {
 //    let  userNameRegex ="^([a-zA-Z])([a-zA-Z_0-9])+$";
    let userNameRegex ="^[a-zA-Z0-9_!@#\$%\^&\*]*$";
    if (username.match(userNameRegex)) {
      console.log(username);
   
      return username
    } else {
      return false
    }
  }
  
  let missingParamiter= (paramObj) => {
    if(!paramObj.facebook_id || !paramObj.name  ){
       return false
    }else{
       return true
    }

  }


  
  module.exports = {
    email: email,
    password: password,
    userName :userName,
    missingParamiter:missingParamiter
  }
  