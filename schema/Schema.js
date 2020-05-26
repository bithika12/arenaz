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
                        colorName        :  [{ colorName: String, status: Number,createdAt : Date, updatedAt:Date}],
                        raceName         :  [{ raceName: String, status: Number,createdAt : Date, updatedAt:Date}],
                        dartName         :  [{ dartName: String, status: Number,createdAt : Date, updatedAt:Date}],
                        characterName      :  [{ characterName: String, status: Number,createdAt : Date, updatedAt:Date}],
                        onlineStatus     :  {  type : String , enum: ['0', '1']  ,  default  : '0'},/* 0=>  offline ,1 => online */
                        loginType        :  {  type : String ,enum :["normal","social"],default :'normal'},
                        status           :  {  type : String , enum: ['active','inactive','delete'] , default : 'active'},
                        total_no_win     :  { type : Number , default  : 0},
                        isPlaying        :  {  type : String , enum: ['yes','no'] , default : 'no'},
                        socialLogin      :  [{ loginBy : String , uniqueLoginId : String }],
                        resetOtp         :  [{ otp : String     , token :String, status: {type:String , enum: ['active','inactive'] , default : 'inactive'},createdAt : Date,updatedAt:Date}],    
                        deviceDetails    :  [{ accessToken : String, deviceId: String, deviceToken : String,expiryAt : Date, status: {type:String , enum: ['active','inactive'] , default : 'active'}, createdAt : Date,updatedAt:Date}],
                        sockets          :  [{ socketId: String, status: Number,createdAt : Date, updatedAt:Date}], 
                        createdAt        :  Date,
                        updatedAt        :  Date,
                        roleId           :  Schema.Types.ObjectId,
                        startCoin        :  Number,
                        userScore        :  Number,
                        cupNo            :  { type : Number , default  : 0},
                        loggedIn         :  { type : Number , default  : 0},
                        countryName      :  { type : String , default  : ''},
                        languageName     :  { type : String , default  : ''},
                  },{
                     strict:false
                  });




// GAME PLAY TABLE DECLARATION



// GAME PLAY TABLE DECLARATION

var roomSchema = new Schema({
    name          : String,
    status        : String,
    users        : [{userId:Schema.Types.ObjectId, status: String ,total:String,score:String,isWin:Number,turn:Number,dartPoint:String,colorName:String,raceName:String,dartName:String,userName:String,cupNumber:Number,roomCoin:Number,totalCupWin:Number,firstName:String,lastName:String}],
    //users        : [{userId:String, status: String ,total:String,score:String,isWin:Number,turn:Number,dartPoint:String}],
    created_at: {
        type: Date,
        default: Date.now()
    },
    updated_at: {
        type: Date,
       // default: Date.now
    },
    game_time: {
        type: Number,
        default: 0
    },
    //game_time:Number,
    turn_time:{
        type: Number,
        default: 0
    }

});

///room log table/////////
var roomlogSchema = new Schema({
     name          : String,
     status        : String,
     userId        : Schema.Types.ObjectId, 
     total         : String,
     score:  String,
     isWin:Number,
     turn:Number,
     dartPoint:String,
     colorName:String,
     raceName:String,
     dartName:String,
     userName:String,
     cupNumber:Number,
     roomCoin:Number,
     totalCupWin:Number,
     firstName:String,
     lastName:String,
     //cupNumber:Number,
     totalCupWin:Number,
     scoreMultiplier:String,
     hitScore:String,
   
    created_at: {
        type: Date,
        default: Date.now()
    },
    updated_at: {
        type: Date,
       // default: Date.now
    },
    game_time:Number,
    turn_time:{
        type: Number,
        default: 0
    }

});

// the schema is useless so far
// we need to create a model using it
var notificationSchema = Schema({
    /*sent_by_user:{
        type: Schema.Types.ObjectId,
        ref: 'User'
    },*/
    received_by_user:{
        type: Schema.Types.ObjectId,
        ref: 'User'
    },
    subject:String,
    message:String,
    read_unread: {
        type: Number,
        default: 0
    },
    created_at: {
        type: Date,
        default: Date.now
    },
    updated_at: {
        type: Date,
        default: Date.now
    },
    status : {  type : String , enum: ['active','inactive','delete'] , default : 'active'}
},{
    versionKey: false
});


var coinSchema      = new Schema({
    number : Number,
    status :  {  type : String , enum: ['active','inactive','delete'] , default : 'active'},

})
//
var gameSchema      = new Schema({
    name : String,
    score: String,
    details:String,
    status :  {  type : String , enum: ['active','inactive','delete'] , default : 'active'},
});



var versionSchema      = new Schema({
    app_version : String,
    status :  {  type : String , enum: ['active','inactive','delete'] , default : 'active'}
    
});
//MODEL DECLARATION

schema.roleModel   = mongoose.model('roles'  , roleSchema);
schema.moduleModel = mongoose.model('modules', modulesSchema);
schema.avatarModel = mongoose.model('avatar' , avatarSchema);
schema.userModel   = mongoose.model('users'  , userSchema);
schema.roomModel   = mongoose.model('rooms'  , roomSchema);
schema.notificationModel   = mongoose.model('notifications'  , notificationSchema);
schema.coinModel   = mongoose.model('coins'  , coinSchema);
schema.gameModel   = mongoose.model('games'  , gameSchema);
schema.versionModel   = mongoose.model('appversions'  , versionSchema);
schema.roomLogModel   = mongoose.model('roomlogs'  , roomlogSchema);

module.exports = schema;
