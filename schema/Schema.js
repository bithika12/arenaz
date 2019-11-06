var schema = {};
var mongoose = require('../config/mongoconnection');
  
var Schema = mongoose.Schema;

//MASTER TABLE  SCHEMA DECLARATOION


var roleSchema      = new Schema({
                                     name : String,
                                     slug: String
                                })
var modulesSchema   = new Schema({
                                     name : String,
                                });

var avatarSchema   = new Schema({
                                  name : String
                             });

//USER  RELATED SCHEMA DECLARATION

var userSchema =  new Schema({
                        userName         :  String,
                        firstName        :  {type : String ,default :''},
                        lastName         :  {type : String ,default :''},
                        password         :  String,
                        email            :  { type : String , default  : ''},
                        userCharacter    :  String,
                        userRace         :  String,
                        characterColorId :  String,
                        description      :  String,
                        loginTime        :  String,
                        loginIp          :  String,
                        onlineStatus     :  {  type : String , enum: ['0', '1']  ,  default  : '0'},/* 0=>  offline ,1 => online */
                        loginType        :  {  type : String ,enum :["normal","social"],default :'normal'},
                        status           :  {  type : String , enum: ['active','inactive','delete'] , default : 'active'}, 
                        isPlaying        :  {  type : String , enum: ['yes','no'] , default : 'no'},
                        socialLogin      :  [{ loginBy : String , uniqueLoginId : String }],
                        resetOtp         :  [{ otp : String     , token :String, status: {type:String , enum: ['active','inactive'] , default : 'inactive'},createdAt : Date,updatedAt:Date}],    
                        deviceDetails    :  [{ accessToken : String, deviceId: String, deviceToken : String,expiryAt : Date, status: {type:String , enum: ['active','inactive'] , default : 'active'}, createdAt : Date,updatedAt:Date}],
                        sockets          :  [{ socketId: String, status: Number,createdAt : Date, updatedAt:Date}], 
                        createdAt        :  Date,
                        updatedAt        :  Date
                  },{
                     strict:false
                  });




// GAME PLAY TABLE DECLARATION

var roomSchema = new Schema({
                     name          : String,
                     status        : String,
                     users         : [/*{user_id:String, name : String}*/],
                     createdAt    : Date,
                     updatedAt    : Date
});



// the schema is useless so far
// we need to create a model using it


//MODEL DECLARATION

schema.roleModel   = mongoose.model('roles'  , roleSchema);
schema.moduleModel = mongoose.model('modules', modulesSchema);
schema.avatarModel = mongoose.model('avatar' , avatarSchema);
schema.userModel   = mongoose.model('users'  , userSchema);
schema.roomModel   = mongoose.model('rooms'  , roomSchema);

module.exports = schema;
