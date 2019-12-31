const  Point            = require('./Point');
class Dart  extends Point {
    constructor() {
        super(Point);
        this.calls = {
            'NT' : { name : 'No Trump', },
            'S'  : { name : 'Spades',   },
            'H'  : { name : 'Hearts',   },
            'D'  : { name : 'Diamonds', },
            'C'  : { name : 'Club',     },
            'PASS' : { name : 'Pass',   },
            'D'  : { name : 'Double',   },
            'RD'  : { name : 'Double',  }
        };
    }
   /* openerBid(cardSuits){
        cardSuits           =  this.stringToObject(cardSuits);
        let pointDetails    =  this.pointCalculation(cardSuits);
        let fiveCardSuit    =  this.checkFiveCardSuit(pointDetails.totalSpades,pointDetails.totalClub,pointDetails.totalDiamond,pointDetails.totalHeart);
        let balanceHand     =  this.checkBalanceHand(pointDetails.totalDP);
        let bid = 'PASS';
        let num = 1
        let totalPoint  = pointDetails.totalHCP+pointDetails.totalDP;
        if(balanceHand == false && fiveCardSuit !=''){
            if(totalPoint >13 && totalPoint <=19) bid =   num+'-'+fiveCardSuit;
        }else{
            if(totalPoint >16 && totalPoint <=19) bid = num+'-NT';
            else if (totalPoint == 14)  bid ='1-C'
        }
        return bid;
    }*/


}
module.exports = Bid;
