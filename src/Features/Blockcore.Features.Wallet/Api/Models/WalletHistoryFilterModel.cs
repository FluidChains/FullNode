using System;
using System.Collections.Generic;
using Blockcore.Utilities.JsonConverters;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blockcore.Features.Wallet.Api.Models
{
    public class WalletHistoryFilterModel
    {
        public WalletHistoryFilterModel()
        {
            this.AccountsHistoryModel = new List<HistoryFilterModel>();
        }

        [JsonProperty(PropertyName = "history")]
        public ICollection<HistoryFilterModel> AccountsHistoryModel { get; set; }
    }

    public class HistoryFilterModel
    {
        public HistoryFilterModel()
        {
            this.TransactionsHistory = new List<TransactionHistoryItemModel>();
        }

        [JsonProperty(PropertyName = "accountName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "accountHdPath")]
        public string HdPath { get; set; }

        [JsonProperty(PropertyName = "coinType")]
        public int CoinType { get; set; }

        [JsonProperty(PropertyName = "transactionsHistory")]
        public ICollection<TransactionHistoryItemModel> TransactionsHistory { get; set; }
    }

    public class TransactionHistoryItemModel
    {
        public TransactionHistoryItemModel()
        {
            this.Payments = new List<PaymentHistoryDetailModel>();
        }

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public TransactionItemType Type { get; set; }

        /// <summary>
        /// The Base58 representation of this address.
        /// </summary>
        [JsonProperty(PropertyName = "toAddress", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ToAddress { get; set; }

        [JsonProperty(PropertyName = "fromAddress", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> FromAddress { get; set; }

        [JsonProperty(PropertyName = "id")]
        [JsonConverter(typeof(UInt256JsonConverter))]
        public uint256 Id { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public Money Amount { get; set; }

        /// <summary>
        /// A list of payments made out in this transaction.
        /// </summary>
        [JsonProperty(PropertyName = "payments", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<PaymentHistoryDetailModel> Payments { get; set; }

        [JsonProperty(PropertyName = "fee", NullValueHandling = NullValueHandling.Ignore)]
        public Money Fee { get; set; }

        /// <summary>
        /// The height of the block in which this transaction was confirmed.
        /// </summary>
        [JsonProperty(PropertyName = "confirmedInBlock", NullValueHandling = NullValueHandling.Ignore)]
        public int? ConfirmedInBlock { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The index of this transaction in the block in which it is contained.
        /// </summary>
        [JsonProperty(PropertyName = "blockIndex", NullValueHandling = NullValueHandling.Ignore)]
        public int? BlockIndex { get; set; }
    }

    public class PaymentHistoryDetailModel
    {
        /// <summary>
        /// The Base58 representation of the destination  address.
        /// </summary>
        [JsonProperty(PropertyName = "destinationAddress")]
        public string DestinationAddress { get; set; }

        /// <summary>
        /// The transaction amount.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        [JsonConverter(typeof(MoneyJsonConverter))]
        public Money Amount { get; set; }

        [JsonProperty(PropertyName = "payToSelf")]
        public bool? PayToSelf { get; set; }
    }

    public enum TransactionHistoryItemType
    {
        Received,
        Send,
        Staked
    }
}
