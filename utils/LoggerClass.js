const fs = require('fs');
print  =  function(caption,content){
	
   // fs.appendFile(process.cwd()+"/logfile", " \n ******* "+caption+"  ", function(err) {
      
        if(content){
			//fs.appendFile(process.cwd()+"/logfile",JSON.stringify(content), function(err) {
			    // if(err) {
			    //     return console.log(err);
			    // }
			   console.log(caption,content);

			//})
	   }else console.log(caption);
    //});		
}


module.exports = {
             print : print
}