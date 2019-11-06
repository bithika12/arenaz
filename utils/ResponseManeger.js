
var generate= (status,result,message) => {
    var response ={ "status" : status,"result":result, "message" : message}
   return response;
}
var error= (status,result,message) => {
    var response ={ "status" : status,"result":result, "message" : message}
   return response;
}

module.exports ={
	generate :  generate,
	error :error
}