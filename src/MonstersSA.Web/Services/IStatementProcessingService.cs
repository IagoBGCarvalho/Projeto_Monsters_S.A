// Salvar como: MonstersSA.Web/Services/IStatementProcessingService.cs
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace MonstersSA.Web.Services
{
    public interface IStatementProcessingService
    {
        Task ProcessStatementStreamAsync(Stream fileStream, string fileName, string playerName);
    }
}