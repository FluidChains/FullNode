using System;
using System.Collections.Generic;
using System.Linq;
using Blockcore.Consensus.ScriptInfo;
using Blockcore.Consensus.TransactionInfo;
using Blockcore.Controllers.Models;
using Blockcore.Features.BlockStore.Repository;
using Blockcore.Features.Wallet.Api.Models;
using Blockcore.Features.Wallet.Database;
using Blockcore.Features.Wallet.Interfaces;
using Blockcore.Features.Wallet.Types;
using Blockcore.Networks;
using NBitcoin;

namespace Blockcore.Features.BlockExplorer.Controllers
{
    /// <summary>
    /// A helper class to build complex history models.
    /// </summary>
    public static class HistoryModelBuilder
    {
        public static WalletHistoryFilterModel GetHistoryFilter(IWalletManager walletManager, IBlockRepository blockRepository, Network network, WalletHistoryFilterRequest request)
        {
            bool isAddressFilter = request.Address == null ? false : true;

            var model = new WalletHistoryFilterModel();

            // Get a list of all the transactions found in an account (or in a wallet if no account is specified), with the addresses associated with them.
            IEnumerable<HistoryFilter> historyFilters = walletManager.GetHistoryFilter(request.WalletName, request.Address, request.AccountName, request.FromDate);


            foreach (HistoryFilter historyFilter in historyFilters)
            {

                var transactionItems = new List<TransactionHistoryItemModel>();
                
                foreach (FlatHistorySlim item in historyFilter.History)
                {
                    var isConfirmed = item.Transaction.BlockHeight.HasValue;

                    Transaction tx = new Transaction();
                    if (isConfirmed)
                    {
                        tx = blockRepository.GetTransactionById(new uint256(item.Transaction.IsSent ? item.Transaction.SentTo : item.Transaction.OutPoint.Hash));
                    } 
                    
                    bool isOutputContained = false;
                    bool isInputContained = false;
                    if (isAddressFilter)
                    {
                        if (item.Transaction.IsSent)
                        {
                            if (item.Transaction.IsCoinStake.HasValue && item.Transaction.IsCoinStake.Value == true)
                            {
                                // We don't show in history transactions that are outputs of staking transactions.
                                continue;
                            }
                            foreach (TxOut outp in tx.Outputs)
                            {
                                if (!isOutputContained)
                                {
                                    if (outp.ScriptPubKey.IsUnspendable)
                                    {
                                        continue;
                                    }
                                    if (outp.ScriptPubKey.GetDestinationAddress(network).ToString() != (request.Address))
                                    {
                                        continue;
                                    }
                                    isOutputContained = true;
                                }

                            }

                        }
                        else
                        {

                            foreach (TxOut txOut in tx.Outputs)
                            {
                                if (!isOutputContained && !tx.IsCoinStake)
                                {
                                    if (!txOut.ScriptPubKey.IsUnspendable)
                                    {
                                        if (txOut.ScriptPubKey.GetDestinationAddress(network).ToString() != request.Address)
                                        {
                                            continue;
                                        }
                                        isOutputContained = true;
                                    }
                                    
                                }
                            }

                        }

                    }

                    var modelItem = new TransactionHistoryItemModel
                    {
                        Type = item.Transaction.IsSent ? TransactionItemType.Send : TransactionItemType.Received,
                        Amount = item.Transaction.IsSent == false ? item.Transaction.Amount : Money.Zero,
                        Id = item.Transaction.IsSent ? item.Transaction.SentTo : item.Transaction.OutPoint.Hash,
                        Timestamp = item.Transaction.CreationTime,
                        ConfirmedInBlock = item.Transaction.BlockHeight,
                        BlockIndex = item.Transaction.BlockIndex
                    };


                    int n = 0;
                    modelItem.Outputs.AddRange(tx.Outputs.Select(x => new Vout(n++, x, network)).Where(y => y.ScriptPubKey.Type == "nulldata"));

                    if (item.Transaction.IsSent == true) // handle send entries
                    {
                        // First we look for staking transaction as they require special attention.
                        // A staking transaction spends one of our inputs into 2 outputs or more, paid to the same address.
                        if ((item.Transaction.IsCoinStake ?? false == true))
                        {
                            if (item.Transaction.IsSent == true)
                            {
                                modelItem.Type = TransactionItemType.Staked;
                                var amount = item.Transaction.SentPayments.Sum(p => p.Amount);
                                modelItem.Amount = amount - item.Transaction.Amount;
                            }
                            else
                            {
                                // We don't show in history transactions that are outputs of staking transactions.
                                continue;
                            }
                        }
                        else
                        {
                            if (item.Transaction.SentPayments.All(a => a.PayToSelf == true))
                            {
                                // if all outputs are to ourself
                                // we don't show that in history
                                continue;
                            }

                            modelItem.Amount = item.Transaction.SentPayments.Where(x => x.PayToSelf == false).Sum(p => p.Amount);

                            foreach (TxIn input in tx.Inputs)
                            {

                                InputHistoryDetailModel inputHistoryDetail = new InputHistoryDetailModel()
                                {
                                    ScriptSig = input.ScriptSig,
                                    Sequence = input.Sequence,
                                    TxId = input.PrevOut.Hash.ToString(),
                                    VOut = input.PrevOut.N,
                                };

                                if (input.ScriptSig.GetSigner(network) == null)
                                {
                                    TxOut prevOutTx = blockRepository.GetTransactionById(input.PrevOut.Hash).Outputs[input.PrevOut.N];
                                    string address = prevOutTx.ScriptPubKey.GetDestinationPublicKeys(network).FirstOrDefault().GetAddress(network).ToString();
                                    inputHistoryDetail.Address = address;
                                }
                                else
                                {

                                    inputHistoryDetail.Address = input.ScriptSig.GetSignerAddress(network).ToString();
                                }

                                modelItem.Inputs.Add(inputHistoryDetail);

                            }


                            foreach (WalletHistoryPaymentData payment in item.Transaction.SentPayments)
                            {
                                if (payment.PayToSelf == false)
                                {
                                    PaymentHistoryDetailModel paymentDetail = new PaymentHistoryDetailModel()
                                    {
                                        Amount = payment.Amount,
                                        DestinationAddress = payment.DestinationAddress,
                                        PayToSelf = payment.PayToSelf
                                    };

                                    modelItem.Payments.Add(paymentDetail);

                                }
                            }
                        }
                    }
                    else // handle receive entries
                    {
                        if (item.Address.IsChangeAddress())
                        {
                            // we don't display transactions sent to self
                            continue;
                        }

                        if (item.Transaction.IsCoinStake.HasValue && item.Transaction.IsCoinStake.Value == true)
                        {
                            // We don't show in history transactions that are outputs of staking transactions.
                            continue;
                        }

                        PaymentHistoryDetailModel paymentDetail = new PaymentHistoryDetailModel()
                        {
                            Amount = item.Transaction.Amount,
                            DestinationAddress = item.Transaction.Address,
                            PayToSelf = true
                        };
                        modelItem.Payments.Add(paymentDetail);

                        foreach (TxIn input in tx.Inputs)
                        {

                            InputHistoryDetailModel inputHistoryDetail = new InputHistoryDetailModel()
                            {
                                ScriptSig = input.ScriptSig,
                                Sequence = input.Sequence,
                                TxId = input.PrevOut.Hash.ToString(),
                                VOut = input.PrevOut.N,
                            };

                            if (input.ScriptSig.GetSigner(network) == null)
                            {
                                TxOut prevOutTx = blockRepository.GetTransactionById(input.PrevOut.Hash).Outputs[input.PrevOut.N];
                                string address = prevOutTx.ScriptPubKey.GetDestinationPublicKeys(network).FirstOrDefault().GetAddress(network).ToString();
                                inputHistoryDetail.Address = address;
                            }
                            else
                            {

                                inputHistoryDetail.Address = input.ScriptSig.GetSignerAddress(network).ToString();
                            }

                            if (isAddressFilter)
                            {

                                if (!isOutputContained)
                                {
                                    if (!inputHistoryDetail.Address.Contains(request.Address))
                                    {
                                        continue;
                                    }
                                    isInputContained = true;
                                }
                            }

                            modelItem.Inputs.Add(inputHistoryDetail);

                        }



                    }

                    if (isAddressFilter == true && isInputContained == false && isOutputContained == false)
                    {
                        continue;
                    }
                    transactionItems.Add(modelItem);
                }

                model.AccountsHistoryModel.Add(new HistoryFilterModel
                {
                    TransactionsHistory = transactionItems,
                    Name = historyFilter.Account.Name,
                    CoinType = network.Consensus.CoinType,
                    HdPath = historyFilter.Account.HdPath
                });
            }

            return model;
        }
    }
}
