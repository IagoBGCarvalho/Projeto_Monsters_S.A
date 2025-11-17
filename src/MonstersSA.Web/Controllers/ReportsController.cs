using Microsoft.AspNetCore.Mvc;
using MonstersSA.Web.Services;
using System.Threading.Tasks;

namespace MonstersSA.Web.Controllers
{
    public class ReportsController : Controller
    {
        // Controlador da página de relatórios
        private readonly IReportsService _reportsService;
        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        public async Task<IActionResult> Index() // /Reports
        {
            var summaryData = await _reportsService.GetPerformanceSummaryAsync();
            return View(summaryData); // Passa os dados do DTO/ViewModel para a View
        }
    }
}