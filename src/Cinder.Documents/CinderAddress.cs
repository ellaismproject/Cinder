﻿using System;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Cinder.Documents
{
    public class CinderAddress : TableRow, IDocument
    {
        private string _id;

        public string Hash { get; set; }
        public string Balance { get; set; }
        public ulong BlocksMined { get; set; }
        public ulong TransactionCount { get; set; }
        public DateTime? CacheDate { get; set; }

        public string Id
        {
            get => _id ?? $"{Hash}";
            set => _id = value;
        }
    }
}
