using System.Reflection.Metadata;

namespace ArturBhasker.TerralinkTestProject.Services;

public interface IDocumentsQueue
{
    /// <summary>
    /// Ставит документ в очередь на отправку.
    /// </summary>
    /// <param name="document">
    /// Документ, который нужно отправить.
    /// </param>
    void Enqueue(Document document);
}