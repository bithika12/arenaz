using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArenaZ.Wallet
{
    public class WalletDataClass
    {

    }

    [System.Serializable]
    public class WalletDetailsRequest
    {
        [JsonProperty("userEmail")]
        public string UserEmail;
    }

    [System.Serializable]
    public class WalletDetailsResponse
    {
        [JsonProperty("minimum_deposit")]
        public string MinimumDeposit;
        [JsonProperty("minimum_withdrawl")]
        public string MinimumWithdrawl;
        [JsonProperty("user_total_coin")]
        public int UserTotalCoin;
        [JsonProperty("transaction_fee_withdrawl")]
        public string TransactionFeeWithdrawl = "0";
        [JsonProperty("transaction_fee_deposit")]
        public string TransactionFeeDeposit = "0";
        [JsonProperty("allow_mini_account_withdrawal")]
        public string AllowMiniAccountWithdrawal;
        [JsonProperty("market_volatility")] 
        public string MarketVolatility;
    }

    [System.Serializable]
    public class ConvertedCoinRequest
    {
        [JsonProperty("coin_number")]
        public int CoinNumber;
        [JsonProperty("userEmail")]
        public string UserEmail;
        [JsonProperty("transactionType")]
        public string TransactionType;
    }

    [System.Serializable]
    public class ConvertedCoinResponse
    {
        [JsonProperty("dollar_amount")]
        public double DollarAmount;
        [JsonProperty("btc_amount")]
        public string BtcAmount;
        [JsonProperty("transaction_fee")]
        public string TransactionFee;
    }

    [System.Serializable]
    public class RequestDepositRequest
    {
        [JsonProperty("userEmail")]
        public string UserEmail;
        [JsonProperty("coinAmount")]
        public int CoinAmount;
        [JsonProperty("user_name")]
        public string UserName;
        [JsonProperty("amount_usd")]
        public double AmountUsd;
    }

    [System.Serializable]
    public class RequestDepositResponse
    {
        [JsonProperty("address_part")]
        public string AddressPart;
        [JsonProperty("expired_at")]
        public string ExpiredAt;
        [JsonProperty("expired_at_inSecond")]
        public int ExpiredAtInSecond;
        [JsonProperty("transactionId")]
        public string TransactionId;
    }

    [System.Serializable]
    public class RequestWithdrawRequest
    {
        [JsonProperty("userEmail")]
        public string UserEmail;
        [JsonProperty("coinAmount")]
        public int CoinAmount;
        [JsonProperty("user_name")]
        public string UserName;
        [JsonProperty("amount_usd")]
        public double AmountUsd;
        [JsonProperty("wallet_key")]
        public string WalletKey;
    }

    [System.Serializable]
    public class RequestWithdrawResponse
    {
        [JsonProperty("transaction_details")]
        public TransactionDetails TransactionDetailsObj;
        [JsonProperty("user_details")]
        public UserDetails UserDetailsObj;
    }

    [System.Serializable]
    public class TransactionDetails
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("expired_at")]
        public string ExpiredAt;
        [JsonProperty("transactionId")]
        public string TransactionId;
    }

    [System.Serializable]
    public class UserDetails
    {
        [JsonProperty("userName")]
        public string UserName;
        [JsonProperty("total_amount")]
        public int TotalAmount;
        [JsonProperty("coinstatus")]
        public string Coinstatus;
    }

    [System.Serializable]
    public class CancelDepositRequest
    {
        [JsonProperty("userEmail")]
        public string UserEmail;
        [JsonProperty("transactionId")]
        public string TransactionId;
    }

    [System.Serializable]
    public class CancelDepositResponse
    {
        [JsonProperty("transactionId")]
        public string TransactionId;
    }

    [System.Serializable]
    public class ConfirmDepositRequest
    {
        [JsonProperty("userEmail")]
        public string UserEmail;
        [JsonProperty("transactionId")]
        public string TransactionId;
    }

    [System.Serializable]
    public class ConfirmDepositResponse
    {
        [JsonProperty("_id")]
        public string Id;
        [JsonProperty("user_name")]
        public string UserName;
        [JsonProperty("user_email")]
        public string UserEmail;
        [JsonProperty("amount")]
        public string Amount;
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("expired_at")]
        public string ExpiredAt;
        [JsonProperty("expire_at_inSecond")]
        public float ExpireAtInSecond;
        [JsonProperty("apiStat")]
        public string ApiStat;
        [JsonProperty("dbStat")]
        public int DbStat;
        [JsonProperty("msg")]
        public string Msg;
    }

    [System.Serializable]
    public class TransactionStatusRequest
    {
        [JsonProperty("userEmail")]
        public string UserEmail;
        [JsonProperty("transactionId")]
        public string TransactionId;
    }

    [System.Serializable]
    public class TransactionStatusResponse
    {
        [JsonProperty("_id")]
        public string Id;
        [JsonProperty("user_name")]
        public string UserName;
        [JsonProperty("user_email")]
        public string UserEmail;
        [JsonProperty("amount")]
        public string Amount;
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("expired_at")]
        public string ExpiredAt;
        [JsonProperty("expire_at_inSecond")]
        public float ExpireAtInSecond;
        [JsonProperty("apiStat")]
        public string ApiStat;
        [JsonProperty("dbStat")]
        public int DbStat;
        [JsonProperty("msg")]
        public string Msg;
    }
}