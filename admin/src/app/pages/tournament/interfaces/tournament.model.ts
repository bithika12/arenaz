export class Tournament {
  id: number;
  name: string;
  round: number;
  price: number;
  image: string;
  status: string;
  labels: any;  

  constructor(tournament) {
    this.id = tournament.id;
    this.name = tournament.name;
    this.round = tournament.round;
    this.price = tournament.price;
    this.image = tournament.image;
    this.status = tournament.status;
    this.labels = tournament.labels;    
  }

}
