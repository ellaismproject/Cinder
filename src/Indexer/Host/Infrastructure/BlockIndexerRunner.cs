﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Exceptions;
using Cinder.Data.Repositories;
using Cinder.Extensions;
using Cinder.Indexer.Host.Infrastructure.StepsHandlers;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing;
using Nethereum.Parity;

namespace Cinder.Indexer.Host.Infrastructure
{
    public class BlockIndexerRunner : IBlockIndexerRunner
    {
        private readonly IBlockProgressRepository _blockProgressRepository;
        private readonly CinderBlockStorageStepHandler _blockStorageStepHandler;
        private readonly CinderContractCreationStorageStepHandler _contractCreationStorageStepHandler;
        private readonly CinderFilterLogStorageStepHandler _filterLogStorageStepHandler;
        private readonly ILogger<BlockIndexerRunner> _logger;
        private readonly CinderTransactionReceiptStorageStepHandler _transactionReceiptStorageStepHandler;
        private readonly IWeb3Parity _web3;

        public BlockIndexerRunner(ILogger<BlockIndexerRunner> logger, IWeb3Parity web3,
            IBlockProgressRepository blockProgressRepository, CinderBlockStorageStepHandler blockStorageStepHandler,
            CinderContractCreationStorageStepHandler contractCreationStorageStepHandler,
            CinderFilterLogStorageStepHandler filterLogStorageStepHandler,
            CinderTransactionReceiptStorageStepHandler transactionReceiptStorageStepHandler)
        {
            _logger = logger;
            _web3 = web3;
            _blockProgressRepository = blockProgressRepository;
            _blockStorageStepHandler = blockStorageStepHandler;
            _contractCreationStorageStepHandler = contractCreationStorageStepHandler;
            _filterLogStorageStepHandler = filterLogStorageStepHandler;
            _transactionReceiptStorageStepHandler = transactionReceiptStorageStepHandler;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            BlockchainProcessor processor = _web3.Processing.Blocks.CreateBlockProcessor(_blockProgressRepository, steps =>
            {
                steps.BlockStep.AddProcessorHandler(_blockStorageStepHandler);
                steps.ContractCreationStep.AddProcessorHandler(_contractCreationStorageStepHandler);
                steps.FilterLogStep.AddProcessorHandler(_filterLogStorageStepHandler);
                steps.TransactionReceiptStep.AddProcessorHandler(_transactionReceiptStorageStepHandler);
            }, 1);

            try
            {
                await processor.ExecuteAsync(cancellationToken).AnyContext();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Class} -> {Method} -> Unexpected error with BlockchainProcessor",
                    nameof(BlockIndexerRunner), nameof(Run));
                throw new LoggedException(e);
            }
        }
    }
}