﻿//--------------------------------------------------------------------------------- 
// <copyright file="DocumentFeedObserver.cs" company="Microsoft">
// Microsoft (R)  Azure SDK 
// Software Development Kit 
//  
// Copyright (c) Microsoft Corporation. All rights reserved.   
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES  
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.  
// </copyright>
//---------------------------------------------------------------------------------

namespace ChangeFeedMigrationSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.ChangeFeedProcessor;
    using Microsoft.Azure.Documents.Client;

    /// <summary>
    /// Cass to create instance of document feed observer. 
    /// </summary>
    public class DocumentFeedObserver : IChangeFeedObserver
    {
        private static int s_totalDocs = 0;
        private DocumentCollectionInfo collectionInfo;
        private DocumentClient client;
        private Uri destinationCollectionUri;

        public DocumentFeedObserver(DocumentClient client, DocumentCollectionInfo destCollInfo)
        {
            this.client = client;
            this.collectionInfo = destCollInfo;
            this.destinationCollectionUri = UriFactory.CreateDocumentCollectionUri(this.collectionInfo.DatabaseName, this.collectionInfo.CollectionName);
        }

        public Task OpenAsync(ChangeFeedObserverContext context)
        {
            Console.WriteLine("Worker opened, {0}", context.PartitionKeyRangeId);
            return Task.CompletedTask;
        }

        public Task CloseAsync(ChangeFeedObserverContext context, ChangeFeedObserverCloseReason reason)
        {
            Console.WriteLine("Worker closed, {0}", context.PartitionKeyRangeId);
            Console.WriteLine("Reason for shutdown, {0}", reason);
            return Task.CompletedTask;
        }

        public Task ProcessChangesAsync(ChangeFeedObserverContext context, IReadOnlyList<Document> docs)
        {
            Console.WriteLine("Change feed: total {0} doc(s)", Interlocked.Add(ref s_totalDocs, docs.Count));
            foreach (Document doc in docs)
            {
                Console.WriteLine(doc.Id.ToString());
                this.client.UpsertDocumentAsync(this.destinationCollectionUri, doc);
            }

            return Task.CompletedTask;
        }
    }
}