export class Player {
  id: number;
  image: string;
  username: string;
  firstname: string;
  lastname: string;
  email: string;
  country: string;
  contact_no: string;
  coins: number;
  vip_coins: number;
  player_rank: number;
  rank_progression: number;
  status: string;
  online_status: string;
  labels: any;

  constructor(player) {
    this.id = player.id;
    this.image = player.image;
    this.username = player.username;
    this.firstname = player.firstname;
    this.lastname = player.lastname;
    this.email = player.email;
    this.country = player.country;
    this.contact_no = player.contact_no;
    this.coins = player.coins;
    this.vip_coins = player.vip_coins;
    this.player_rank = player.player_rank;
    this.rank_progression = player.rank_progression;
    this.status = player.status;
    this.online_status = player.online_status;
    this.labels = player.labels;
  }

  get name() {
    let name = '';

    if (this.firstname && this.lastname) {
      name = this.firstname + ' ' + this.lastname;
    } else if (this.firstname) {
      name = this.firstname;
    } else if (this.lastname) {
      name = this.lastname;
    }

    return name;
  }

  set name(value) {
  }

  /*get address() {
    return `${this.street}, ${this.zipcode} ${this.city}`;
  }

  set address(value) {
  }*/
}
