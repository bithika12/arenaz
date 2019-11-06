var redis = require('redis')

rejson = require('redis-rejson');

rejson(redis);
client ={}
var client = redis.createClient({
           host : '127.0.0.1',  
           no_ready_check: true,
           auth_pass: "radis@redapple2019",                                                                                                                                                         
 })
 client.on('connect', () => {   
     //  global.console.log("connected");
 });     

 // client.json_set('myJson', '.test.here.now', '{"user": 1234}', function (err) {
	// if (err) { throw err; }
	//     console.log('Set JSON at key  myJson.');
	// client.json_get('myJson', '.test.here.now', function (err, value) {
	//   if (err) { throw err; }
	//   console.log('value of .test.here.now:', value); //outputs 1234
	//   client.quit();
	// });
 //  });                          
                              
// client.smembers("room",function (err, res) {
// 	console.log("res",res)
//  })
// client.LSET("room","name","test2" ,function (err, res) {
// })
// client.on('error', function (err) {
//   console.log('Error ' + err)
// })
// client.set("foo", 'bar');         

// client.get("foo", function (err, reply) {
//         global.console.log(reply.toString())
// })
module.exports = client;