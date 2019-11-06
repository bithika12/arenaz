
var multer  = require('multer');
var path = require('path');
/** IMAGE FILE UPLOADING**/
var fileUpload={}
fileUpload.upload =  multer({ storage: multer.diskStorage({
        destination: function(req, file, callback) {
            callback(null, './public/images/user_images')
        },
        filename: function(req, file, callback,next) {
            var ext = path.extname(file.originalname);
            if (ext !== '.png' && ext !== '.jpg' && ext !== '.jpeg') {
                callback({status:0})
            }else {
                callback(null, file.fieldname + '-' + Date.now() + path.extname(file.originalname))
            }
        },
        onError: function(err, next){
            console.log("error", err);
            next(err);
        }
    })
});
fileUpload.categoryfileupload =  multer({ storage: multer.diskStorage({
        destination: function(req, file, callback) {
            callback(null, './public/images/sports_category')
        },
        filename: function(req, file, callback,next) {
            var ext = path.extname(file.originalname);
            if (ext !== '.png' && ext !== '.jpg' && ext !== '.jpeg') {
                callback({status:0})
            }else {
                callback(null, file.fieldname + '-' + Date.now() + path.extname(file.originalname))
            }
        },
        onError: function(err, next){
            next(err);
        }
    })
});

module.exports =fileUpload;