var mongoose = require('mongoose');
/*mongoose.connect('mongodb://localhost:27017/bridge', {
	useNewUrlParser: true});*/

// Redapple Server
/*mongoose.connect('mongodb://superAdmin:admin123@52.66.82.72:27017/arenaz?authSource=admin', {
    useNewUrlParser: true,
    useUnifiedTopology: true,
})*/

// Local
mongoose.connect('mongodb://localhost:27017/arenaz', {
    useNewUrlParser: true,
    useUnifiedTopology: true,
})
	mongoose.set('debug', true);
// mongoose.connection.dropDatabase(function(err, result) {
// 	console.log(" deleted")
// });

module.exports= mongoose;
