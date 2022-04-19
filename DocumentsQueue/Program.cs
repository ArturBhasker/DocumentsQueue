using ArturBhasker.TerralinkTestProject;
using ArturBhasker.TerralinkTestProject.Services;

var externalSystemConnector = new ExternalSystemConnector();
using var cancellationTokenSource = new CancellationTokenSource();
using var documentsQueue = new DocumentsQueue(
    externalSystemConnector: externalSystemConnector,
    cancellationTokenSource: cancellationTokenSource
);


documentsQueue.SetDocumentQueueTimer(cancellationTokenSource.Token);

//Отправим 100 документов для проверки
for (int i = 0; i < 100; i++)
{
    documentsQueue.Enqueue(new System.Reflection.Metadata.Document());
}


Console.ReadLine();