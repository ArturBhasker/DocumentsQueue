using System.Collections.Concurrent;
using System.Reflection.Metadata;
using ArturBhasker.TerralinkTestProject.Helpers;

namespace ArturBhasker.TerralinkTestProject.Services
{
    internal class DocumentsQueue : IDocumentsQueue, IDisposable
    {
        private const int SendIntervalInMilliseconds = 5000;

        private readonly ConcurrentQueue<Document> _documentItemsQueue = new();
        private readonly ExternalSystemConnector _externalSystemConnector;
        private readonly CancellationTokenSource _cts;

        public DocumentsQueue(
            ExternalSystemConnector externalSystemConnector
        )
        {
            _externalSystemConnector = externalSystemConnector;

            _cts = new CancellationTokenSource();

            SetDocumentQueueTimer(_cts.Token);
        }

        public void Enqueue(Document document)
        {
            _documentItemsQueue.Enqueue(document);
        }

        private void SetDocumentQueueTimer(
            CancellationToken cancellationToken)
        {
            var documentSendTimer = new System.Timers.Timer(SendIntervalInMilliseconds);

            documentSendTimer.Elapsed += async (sender, e) => await EnqueueMessages(this, cancellationToken);
            documentSendTimer.AutoReset = true;
            documentSendTimer.Enabled = true;

            documentSendTimer.Start();
        }

        private static async Task EnqueueMessages(
            DocumentsQueue documentsQueue,
            CancellationToken cancellationToken
        )
        {
            if (documentsQueue._documentItemsQueue.Any())
            {
                var documentsList = documentsQueue._documentItemsQueue
                    .DequeueList(10)
                    .ToList();

                await
                    Task.Run(
                        async () => await documentsQueue._externalSystemConnector.SendDocument(documentsList),
                        cancellationToken);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _documentItemsQueue.Clear();
        }
    }
}