var SocketManager = (function () {

    let instance;

    function init(server) {
        //this.io = require('socket.io')(server,{ wsEngine: 'ws' });
        this.io= require('socket.io')(server, {'pingInterval': 4000, 'pingTimeout': 8000});
    }

    return {
        getInstance: function () {

            if (!instance) {
               // console.log(" hiii")
                instance = this;
            }
            return instance;
        },
        init: init
    };
})();

module.exports = SocketManager.getInstance();