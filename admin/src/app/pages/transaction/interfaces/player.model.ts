export class Player {
  /*id: number;
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
  labels: any;*/

  type: string;
  _id: string;
  user_name: string;
  user_email: string;
  amount: string;
  amount_usd: string;
  status: string;
  created_at: string;
  expired_at: string;
  transaction_key: string;
  user_confirmation: string;

  constructor(player) {
    this._id                = player._id;
    this.user_name          = player.user_name;
    this.user_email         = player.user_email;
    this.amount             = player.amount;
    this.amount_usd         = player.amount_usd;
    this.status             = player.status;
    this.created_at         = player.created_at;
    this.expired_at         = player.expired_at;
    this.type               = player.type;
    this.transaction_key    = player.transaction_key;
    this.user_confirmation  = player.user_confirmation;
  }
}
