
var moment = require('moment');
var socketModel   = require('../models/Socket');
var io            = require('../utils/SocketManager').io;

//===== Socket Related func ===

io.on('connection', function(socket) {
    socket.on('serverstop',function (req) {
        io.emit('serverresponse',{status:"1",message:"due to some problem server will stop after few moment"});
        setTimeout(() => {
            process.exit(0);
          }, 5000);     
    });
 });

