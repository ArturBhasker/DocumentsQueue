using System.Collections.Concurrent;
using System.Reflection.Metadata;
using ArturBhasker.TerralinkTestProject.Extensions;

namespace ArturBhasker.TerralinkTestProject.Services
{
    internal class DocumentsQueue : IDocumentsQueue, IDisposable
    {
        private const int SendIntervalInMilliseconds = 5000;
        private const int MaxDocumentsCount = 10;

        private readonly ConcurrentQueue<Document> _documentItemsQueue = new();
        private readonly ExternalSystemConnector _externalSystemConnector;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private System.Timers.Timer? DocumentSendTimer { get; set; }

        public DocumentsQueue(
            ExternalSystemConnector externalSystemConnector,
            CancellationTokenSource cancellationTokenSource
        )
        {
            _externalSystemConnector = externalSystemConnector ?? throw new ArgumentNullException(nameof(externalSystemConnector));
            _cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        public void Enqueue(Document document)
        {
            _documentItemsQueue.Enqueue(document);
        }

        public void SetDocumentQueueTimer(
            CancellationToken cancellationToken)
        {
            DocumentSendTimer = new System.Timers.Timer(SendIntervalInMilliseconds);

            DocumentSendTimer.Elapsed += async (sender, e) => await SendMessagesAsync(this, cancellationToken);
            DocumentSendTimer.AutoReset = true;
            DocumentSendTimer.Enabled = true;

            DocumentSendTimer.Start();
        }

        private static async Task SendMessagesAsync(
            DocumentsQueue documentsQueue,
            CancellationToken cancellationToken
        )
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (documentsQueue._documentItemsQueue.Any())
            {
                var documentsList = documentsQueue._documentItemsQueue
                    .DequeueList(MaxDocumentsCount)
                    .ToList();

                await
                    Task.Run(
                        async () => await documentsQueue._externalSystemConnector.SendDocument(documentsList),
                        cancellationToken
                    );
            }
        }

        public void Dispose()
        {
            DocumentSendTimer?.Stop();
            _documentItemsQueue.Clear();
            _cancellationTokenSource.Cancel();
        }
    }
}