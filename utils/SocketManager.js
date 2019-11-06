var SocketManager = (function () {

    let instance;

    function init(server) {
        this.io = require('socket.io')(server,{ wsEngine: 'ws' });
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