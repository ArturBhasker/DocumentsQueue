using ArturBhasker.TerralinkTestProject;
using ArturBhasker.TerralinkTestProject.Services;

var externalSystemConnector = new ExternalSystemConnector();
using var documentsQueue = new DocumentsQueue(externalSystemConnector);

//Отправим 100 документов для проверки
for (int i = 0; i < 100; i++)
{
    documentsQueue.Enqueue(new System.Reflection.Metadata.Document());
}


Console.ReadLine();