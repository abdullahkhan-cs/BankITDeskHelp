using System.Threading.Tasks;
using BankITDeskHelp.ViewModels;

namespace BankITDeskHelp.Services
{
    public interface ITicketMetricsService
    {
        Task<TicketMetricsDto> GetMetricsAsync();
    }
}
