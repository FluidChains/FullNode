using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Blockcore.Features.BlockExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Blockcore.Base;
using Blockcore.Features.BlockStore.Models;
using Blockcore.Features.Consensus;
using Blockcore.Interfaces;
using Blockcore.Utilities;
using Blockcore.Utilities.JsonErrors;
using Blockcore.Utilities.ModelStateErrors;
using Blockcore.Consensus.Chain;
using Blockcore.Networks;
using Blockcore.Consensus.BlockInfo;

namespace Blockcore.Features.BlockExplorer.Controllers
{
   /// <summary>
   /// Controller providing operations on a blockstore.
   /// </summary>
   [Route("api/features/explorer/blocks")]
   public class BlockExplorerController : Controller
   {
      /// <summary>An interface for getting blocks asynchronously from the blockstore cache.</summary>
      private readonly IBlockStore blockStoreCache;

      /// <summary>Instance logger.</summary>
      private readonly ILogger logger;

      /// <summary>An interface that provides information about the chain and validation.</summary>
      private readonly IChainState chainState;

      private readonly IStakeChain stakeChain;

      /// <summary>
      /// Current network for the active controller instance.
      /// </summary>
      private readonly Network network;

      private readonly ChainIndexer chain;

      public BlockExplorerController(
          Network network,
          ILoggerFactory loggerFactory,
          IBlockStore blockStoreCache,
          ChainIndexer chain,
          IChainState chainState,
          IStakeChain stakeChain = null)
      {
         Guard.NotNull(loggerFactory, nameof(loggerFactory));
         Guard.NotNull(blockStoreCache, nameof(blockStoreCache));
         Guard.NotNull(chainState, nameof(chainState));

         this.network = network;
         this.blockStoreCache = blockStoreCache;
         this.stakeChain = stakeChain;
         this.chain = chain;
         this.chainState = chainState;
         this.logger = loggerFactory.CreateLogger(GetType().FullName);
      }

      /// <summary>
      /// Retrieves a given block given a block hash.
      /// </summary>
      /// <param name="query">A <see cref="SearchByHashRequest"/> model with a specific hash.</param>
      /// <returns><see cref="BlockModel"/> if block is found, <see cref="NotFoundObjectResult"/> if not found. Returns <see cref="IActionResult"/> with error information if exception thrown.</returns>
      [HttpGet]
      [ProducesResponseType(typeof(PosBlockModel[]), 200)]
      public IActionResult GetBlocksAsync()
      {
         if (!this.ModelState.IsValid)
         {
            return ModelStateErrors.BuildErrorResponse(this.ModelState);
         }

         int pageSize = 10; // Should we allow page size to be set in query?

         try
         {
            var blocks = new List<PosBlockModel>();
            ChainedHeader chainHeader = this.chain.Tip;

            while (chainHeader != null && blocks.Count < pageSize)
            {
               Block block = this.blockStoreCache.GetBlock(chainHeader.HashBlock);

               var blockModel = new PosBlockModel(block, this.chain);
               blocks.Add(blockModel);

               chainHeader = chainHeader.Previous;
            }

            return Json(blocks);
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
      [ProducesResponseType(typeof(PosBlockModel), 200)]
      [ProducesResponseType(typeof(void), 404)]
      //[ProducesResponseType(typeof(void), 401)]
      public IActionResult GetBlockAsync(string id, [FromQuery] BlockQueryRequest query)
      {
         if (!this.ModelState.IsValid)
         {
            return ModelStateErrors.BuildErrorResponse(this.ModelState);
         }

         ChainedHeader chainHeader = null;


         if (string.IsNullOrWhiteSpace(id))
         {
            throw new ArgumentNullException("id", "id must be block hash or block height");
         }

         // If the id is more than 50 characters, it is likely hash and not height.
         if (id.Length < 50)
         {
            chainHeader = this.chain.GetHeader(int.Parse(id));
         }
         else
         {
            chainHeader = this.chain.GetHeader(new uint256(id));
         }

         if (chainHeader == null)
         {
            return new NotFoundObjectResult("Block not found");
         }

         try
         {
            Block block = this.blockStoreCache.GetBlock(chainHeader.Header.GetHash());

            if (block == null)
            {
               return new NotFoundObjectResult("Block not found");
            }

            PosBlockModel blockModel = new PosBlockModel(block, this.chain);

            if (this.stakeChain != null)
            {
               BlockStake blockStake = this.stakeChain.Get(chainHeader.HashBlock);

               if (blockStake != null)
               {
                  blockModel.StakeTime = blockStake.StakeTime;
                  blockModel.StakeModifierV2 = blockStake.StakeModifierV2;
                  blockModel.HashProof = blockStake.HashProof;
               }
            }

            return Json(blockModel);
         }
         catch (Exception e)
         {
            this.logger.LogError("Exception occurred: {0}", e.ToString());
            return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
         }
      }

      /// <summary>
      /// Retrieves a given block given a block hash.
      /// </summary>
      /// <param name="query">A <see cref="SearchByHashRequest"/> model with a specific hash.</param>
      /// <returns><see cref="BlockModel"/> if block is found, <see cref="NotFoundObjectResult"/> if not found. Returns <see cref="IActionResult"/> with error information if exception thrown.</returns>
      [HttpGet, Route("page/{index}")]
      public IActionResult GetBlocksPageAsync(int index, [FromQuery] BlockQueryRequest query)
      {
         if (!this.ModelState.IsValid)
         {
            return ModelStateErrors.BuildErrorResponse(this.ModelState);
         }

         throw new NotImplementedException("Not implemented");
      }

      /// <summary>
      /// Gets the current consensus tip height.
      /// API implementation of RPC call.
      /// </summary>
      /// <returns>The current tip height. Returns <c>null</c> if fails. Returns <see cref="IActionResult"/> with error information if exception thrown.</returns>
      [HttpGet, Route("count")]
      public IActionResult GetBlockCount()
      {
         try
         {
            return Json(this.chainState.ConsensusTip.Height);
         }
         catch (Exception e)
         {
            this.logger.LogError("Exception occurred: {0}", e.ToString());
            return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
         }
      }
   }
}
