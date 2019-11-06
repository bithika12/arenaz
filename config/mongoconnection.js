var mongoose = require('mongoose');
/*mongoose.connect('mongodb://localhost:27017/bridge', {
	useNewUrlParser: true});*/
mongoose.connect('mongodb://superAdmin:admin123@52.66.82.72:27017/arenaz?authSource=admin', {
    useNewUrlParser: true
})
	mongoose.set('debug', true);
// mongoose.connection.dropDatabase(function(err, result) {
// 	console.log(" deleted")
// });

module.exports= mongoose;
