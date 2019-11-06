 var constants = require("../config/constants");

let Email = (email) => {
    let emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/
    if (email.match(emailRegex)) {
      return email
    } else {
      return false
    }
  }
  
    /* Minimum 8 characters which contain only characters,numeric digits, underscore and first character must be a letter */
  let Password = (password) => {
    let passwordRegex = /^[A-Za-z0-9]\w{7,}$/
    if (password.match(passwordRegex)) {
      return password
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
    Email: Email,
    Password: Password,
    missingParamiter:missingParamiter
  }
  