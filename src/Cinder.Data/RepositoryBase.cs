﻿using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Driver;

namespace Cinder.Data
{
    public abstract class RepositoryBase<TDocument> : IRepository where TDocument : IDocument
    {
        protected readonly IMongoClient Client;
        protected readonly IMongoCollection<TDocument> Collection;
        protected readonly IMongoDatabase Database;

        protected RepositoryBase(IMongoClient client, string databaseName, CollectionName collectionName)
        {
            Client = client;
            Database = Client.GetDatabase(databaseName);
            Collection = Database.GetCollection<TDocument>(collectionName.ToCollectionName());
        }

        protected virtual FilterDefinition<TDocument> CreateDocumentFilter(string id)
        {
            return Builders<TDocument>.Filter.Eq(document => document.Id, id);
        }

        protected virtual FilterDefinition<TDocument> CreateDocumentFilter(TDocument entity)
        {
            return Builders<TDocument>.Filter.Eq(document => document.Id, entity.Id);
        }
    }
}