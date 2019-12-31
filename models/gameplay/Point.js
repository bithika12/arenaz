class Deck {
    constructor() {
        this.card = []
        this.suit = {
            'S' : { name : 'Spades'  , index : 0   ,type :"minor" },
            'H' : { name : 'Hearts'  , index : 1   ,type: "minor" },
            'D' : { name : 'Diamonds', index : 2   ,type :"mejor" },
            'C' : { name : 'Clubs'   , index : 3   ,type :"mejor" }
        };
        this.value = {
            '14' : { index : 0,  name: 'A' , HCP:4 },
            '13' : { index : 1,  name: 'K' , HCP:3 },
            '12' : { index : 2,  name: 'Q' , HCP:2 },
            '11' : { index : 3,  name: 'J' , HCP:1 },
            '10' : { index : 4,  name: 'T' , HCP:0 },
            '9'  : { index : 5,  name: '9' , HCP:0 },
            '8'  : { index : 6,  name: '8' , HCP:0 },
            '7'  : { index : 7,  name: '7' , HCP:0 },
            '6'  : { index : 8,  name: '6' , HCP:0 },
            '5'  : { index : 9,  name: '5' , HCP:0 },
            '4'  : { index : 10, name: '4' , HCP:0 },
            '3'  : { index : 11, name: '3' , HCP:0 },
            '2'  : { index : 12, name: '2' , HCP:0 },
        };
        this.suits =Object.keys(this.suit).map(i => i)
        this.values =Object.keys(this.value).map(i =>i);


        for(var i =0;i<this.suits.length ;i++){
            for(var j=0; j<this.values.length ;j++){
                this.card.push({"suit":this.suits[i],"value":this.values[j]})
            }
        }
    }
    shuffle(){
        const { card } = this;
        let m = card.length, i;
        while(m){
            i = Math.floor(Math.random() * m--);
            [card[m], card[i]] = [card[i], card[m]];
        }
        return this.card;
    }


    checkFiveCardSuit(totalSpades,totalClub,totalDiamond,totalHeart){
        let suit =''
        if(totalSpades > 5) suit ='S'
        else if (totalClub > 5)   suit = 'C'
        else if (totalDiamond >5) suit = 'D'
        else if (totalHeart >5)  suit ='H'
        return  suit;

    }
    checkBalanceHand(totalDP ){
        return (totalDP >1)?false : true;
    }

    pointCalculation(cardArr){
        let totalHCP = 0;
        let spadesDP = 3;
        let clubDP = 3;
        let heartDP = 3;
        let diamondDP = 3;
        let totalSpades =0;
        let totalClub  =0;
        let totalHeart =0;
        let totalDiamond =0;

        cardArr.forEach(function (card){
            totalHCP+= this.value[card.value].HCP;
            switch (card.suit) {
                case "S":
                    spadesDP = spadesDP>0? spadesDP-1:0;
                    totalSpades++;
                    break;
                case "C":
                    clubDP = clubDP>0? clubDP-1:0;
                    totalClub++;
                    break;
                case "D":
                    heartDP = heartDP>0? heartDP-1:0;
                    totalDiamond++;
                    break;
                case "H":
                    diamondDP = diamondDP>0? diamondDP-1:0;
                    totalHeart++;
                    break;
            }
        },this);

        const totalDP = spadesDP + heartDP + clubDP + diamondDP;
        return {
            totalHCP     : totalHCP ,
            totalDP      : totalDP,
            totalSpades  : totalSpades,
            totalClub    : totalClub,
            totalDiamond : totalDiamond,
            totalHeart   : totalHeart
        }
    }

    stringToObject(cards){
        let    cardSuits= []
        for(let i =0; i < cards.length; i++){
            let card = cards[i].split("-");
            cardSuits.push({suit : card[0], value : card[1]});
        }
        return cardSuits;
    }
}
makeString = (cardSuits)=>{
    var  card = []
    for(i =0; i < cardSuits.length; i++){
        card.push(cardSuits[i].suit+"-"+cardSuits[i].value);
    }
    //cardSuits.map(x => x.suit+'-'+x.value)
    return card;
}
Deck.deal = (userArr) =>{
    let card = new Deck();
    suffleCard  = card.shuffle();
    for(var i =0 ; i < suffleCard.length;i++){
        if(userArr[i%userArr.length].cardSuits == undefined)  userArr[i%userArr.length].cardSuits =[];
        userArr[i%userArr.length].cardSuits.push(suffleCard[i].suit+"-"+suffleCard[i].value)
        //userArr[i%userArr.length].cardSuits.push({ suit : suffleCard[i].suit, "value":suffleCard[i].value })
    }
    return userArr;
}
module.exports = Deck;
