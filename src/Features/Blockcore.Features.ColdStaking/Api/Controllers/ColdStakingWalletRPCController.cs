﻿using Blockcore.Consensus;
using Blockcore.Features.BlockStore;
using Blockcore.Features.Wallet;
using Blockcore.Features.Wallet.Api.Controllers;
using Blockcore.Features.Wallet.Interfaces;
using Blockcore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;

namespace Blockcore.Features.ColdStaking.Api.Controllers
{
    /// <summary> All functionality is in WalletRPCController, just inherit the functionality in this feature.</summary>
    [ApiVersion("1")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ColdStakingWalletRPCController : WalletRPCController
    {
        public ColdStakingWalletRPCController(
            IBlockStore blockStore,
            IBroadcasterManager broadcasterManager,
            ChainIndexer chainIndexer,
            IConsensusManager consensusManager,
            IFullNode fullNode,
            ILoggerFactory loggerFactory,
            Network network,
            IScriptAddressReader scriptAddressReader,
            StoreSettings storeSettings,
            IWalletManager walletManager,
            WalletSettings walletSettings,
            IWalletTransactionHandler walletTransactionHandler) :
            base(blockStore, broadcasterManager, chainIndexer, consensusManager, fullNode, loggerFactory, network, scriptAddressReader, storeSettings, walletManager, walletSettings, walletTransactionHandler)
        {
        }
    }
}