using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Blockcore.Features.BlockExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Blockcore.Base;
using Blockcore.Connection;
using Blockcore.Controllers.Models;
using Blockcore.Features.BlockStore;
using Blockcore.Features.BlockStore.Models;
using Blockcore.Features.Wallet.Interfaces;
using Blockcore.Interfaces;
using Blockcore.Utilities;
using Blockcore.Utilities.JsonErrors;
using Blockcore.Utilities.ModelStateErrors;
using Blockcore.Consensus.Chain;
using Blockcore.Networks;
using Blockcore.Features.BlockStore.Repository;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus.TransactionInfo;
using Blockcore.Features.Wallet.Api.Models;

namespace Blockcore.Features.BlockExplorer.Controllers
{
    /// <summary>
    /// Controller providing operations on a blockstore.
    /// </summary>
    [Route("api/features/explorer/transactions")]
    public class TransactionStoreController : Controller
    {
        private readonly IWalletManager walletManager;

        /// <summary>An interface for getting blocks asynchronously from the blockstore cache.</summary>
        private readonly IBlockStore blockStoreCache;

        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>An interface that provides information about the chain and validation.</summary>
        private readonly IChainState chainState;

        /// <summary>
        /// Current network for the active controller instance.
        /// </summary>
        private readonly Network network;

        private readonly IBlockRepository blockRepository;

        private readonly ChainIndexer chain;

        private readonly IBroadcasterManager broadcasterManager;

        private readonly IConnectionManager connectionManager;

        public TransactionStoreController(
            Network network,
            IWalletManager walletManager,
            ILoggerFactory loggerFactory,
            IBlockStore blockStoreCache,
         ChainIndexer chain,
            IBroadcasterManager broadcasterManager,
            IBlockRepository blockRepository,
            IConnectionManager connectionManager,
            IChainState chainState)
        {
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(blockStoreCache, nameof(blockStoreCache));
            Guard.NotNull(chainState, nameof(chainState));

            this.logger = loggerFactory.CreateLogger(GetType().FullName);
            this.network = network;
            this.walletManager = walletManager;
            this.blockStoreCache = blockStoreCache;
            this.connectionManager = connectionManager;
            this.chain = chain;
            this.chainState = chainState;
            this.blockRepository = blockRepository;
            this.broadcasterManager = broadcasterManager;
        }

        /// <summary>
        /// Retrieves a given block given a block hash.
        /// </summary>
        /// <param name="query">A <see cref="SearchByHashRequest"/> model with a specific hash.</param>
        /// <returns><see cref="BlockModel"/> if block is found, <see cref="NotFoundObjectResult"/> if not found. Returns <see cref="IActionResult"/> with error information if exception thrown.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PosBlockModel[]), 200)]
        public IActionResult GetTransactionsAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return ModelStateErrors.BuildErrorResponse(this.ModelState);
            }

            int pageSize = 10; // Should we allow page size to be set in query?
                               //this.logger.LogTrace("(Hash:'{1}')", hash);

            try
            {
                ChainedHeader chainHeader = this.chain.Tip;

                var transactions = new List<TransactionVerboseModel>();

                while (chainHeader != null && transactions.Count < pageSize)
                {
                    Block block = this.blockStoreCache.GetBlock(chainHeader.HashBlock);

                    var blockModel = new PosBlockModel(block, this.chain);

                    foreach (Transaction trx in block.Transactions)
                    {
                        // Since we got Chainheader and Tip available, we'll supply those in this query. That means this query will
                        // return more metadata than specific query using transaction ID.
                        transactions.Add(new TransactionVerboseModel(trx, this.network, chainHeader, this.chainState.BlockStoreTip));
                    }

                    chainHeader = chainHeader.Previous;
                }

                return Json(transactions);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Retrieves a given block given a block hash or block height.
        /// </summary>
        /// <param name="query">A <see cref="SearchByHashRequest"/> model with a specific hash.</param>
        /// <returns><see cref="BlockModel"/> if block is found, <see cref="NotFoundObjectResult"/> if not found. Returns <see cref="IActionResult"/> with error information if exception thrown.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionVerboseModel), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetTransactionAsync(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return ModelStateErrors.BuildErrorResponse(this.ModelState);
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id", "id must be specified");
            }

            try
            {
                Transaction trx = this.blockRepository.GetTransactionById(new uint256(id));
                var model = new TransactionVerboseModel(trx, this.network);
                return Json(model);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        [Route("history-filter")]
        [HttpGet]
        public IActionResult GetHistoryFilter([FromQuery] WalletHistoryFilterRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (!this.ModelState.IsValid)
            {
                return ModelStateErrors.BuildErrorResponse(this.ModelState);
            }

            try
            {
                WalletHistoryFilterModel model = HistoryModelBuilder.GetHistoryFilter(this.walletManager, this.blockRepository, this.network, request);

                return this.Json(model);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }


        [HttpGet("{wallet}")]
        [ProducesResponseType(typeof(TransactionVerboseModel), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetHistoryFiler(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return ModelStateErrors.BuildErrorResponse(this.ModelState);
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id", "id must be specified");
            }

            try
            {
                Transaction trx = this.blockRepository.GetTransactionById(new uint256(id));
                var model = new TransactionVerboseModel(trx, this.network);
                return Json(model);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
    }
}

