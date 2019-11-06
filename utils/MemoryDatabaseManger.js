
var  MemoryDatabaseManger = (function () {
    let instance;
    var Datastore = require('nedb');   
    if (!this.room) {
        console.log(" new datastore room creation");
        this.room = new Datastore();
    }
    return this;
})();

module.exports = MemoryDatabaseManger;