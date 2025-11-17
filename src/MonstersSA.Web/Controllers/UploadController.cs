using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonstersSA.Web.Services;
using System.IO;
using System.Threading.Tasks;

namespace MonstersSA.Web.Controllers
{
    public class UploadController : Controller
    {
        // Controlador da página de dar upload do arquivo .xlsh
        private readonly IStatementProcessingService _processingService;

        public UploadController(IStatementProcessingService processingService)
        {
            _processingService = processingService;
        }

        public IActionResult Index() // /Upload
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessStatement(IFormFile statementFile)
        {
            /// Função assincrona que carrega os dados do arquivo .xlsh em memória utilizando o stream reader IFormFile
            if (statementFile == null || statementFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Por favor, selecione um arquivo.";
                return View("Index");
            }

            using (var stream = statementFile.OpenReadStream())
            {
                await _processingService.ProcessStatementStreamAsync(
                    stream, 
                    statementFile.FileName, 
                    "Iago Carvalho" // Placeholder para o nome do jogador
                );
            }

            TempData["success"] = "Arquivo processado com sucesso!";
            return RedirectToAction("Index", "Reports"); // Redireciona para a página de relatórios
        }
    }
}