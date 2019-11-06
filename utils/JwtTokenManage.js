const jwt = require('jsonwebtoken')
const shortid = require('shortid')
const secretKey = 'someVeryRandomStringThatNobodyCanGuess';
var jwtToken ={}

jwtToken.generateToken = (data) => {
   return  new Promise((resolve,reject) => {
     let claims = {
      jwtid: shortid.generate(),
      iat: Date.now(),
      exp: Math.floor(Date.now() / 1000) + (60 * 60 * 24),
      sub: 'authToken',
      iss: 'edChat',
      data: data
    }
    jwt.sign(claims, secretKey,function (err, token) {
      if(err){
         reject({})
      }else{
        resolve(token)
      }

    });
   });
}// end generate token 

jwtToken.verifyClaim = (token) => {
  // verify a token symmetric
    return  new Promise((resolve,reject) => {
      jwt.verify(token, secretKey, function (err, decoded) {
        if(err){
          reject({})
        }else{
          //console.log("user verified");
          resolve(decoded)
        } 
     });
  });


}// end verify claim 

jwtToken.verifyClaimWithoutSecret = (token,cb) => {
  // verify a token symmetric
  jwt.verify(token, secretKey, function (err, decoded) {
    if(err){
     // console.log("error while verify token");
      //console.log(err);
      cb("Authentication Faliure",null)
    }
    else{
      //console.log("user verified");
      cb (null,decoded)
    }  
 
 
  });
}// end verify claim 




module.exports = jwtToken;
