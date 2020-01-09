var bcrypt = require('bcryptjs');
//var bcrypt = require('bcrypt');
var salt = bcrypt.genSaltSync(10);


let hashPassword =(originalpassword)=>{
  let hash = bcrypt.hashSync(originalpassword, salt)
  return hash
}

let comparePassword = (oldPassword, hashpassword, cb) => {
  bcrypt.compare(oldPassword, hashpassword, (err, res) => {
    if (err) {
       cb(err, null)
    } else {
      cb(null, res)
    }
  })
}


let comparePasswordSync = (originalpassword,hashpasword ) => {
  return bcrypt.compareSync(originalpassword, hashpasword)
}

module.exports = {
  hashPassword          : hashPassword,
  comparePassword      : comparePassword,
  comparePasswordSync : comparePasswordSync
}
