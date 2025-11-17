using EV.DataProviderService.API.Models.Entites;
using System.Threading.Tasks;

namespace EV.DataProviderService.API.Repositories
{
    public interface IAnonymizationRepository
    {
        Task<AnonymizationLog> LogAnonymizationAsync(AnonymizationLog log);
        Task<bool> AnonymizeDatasetFilesAsync(Guid datasetVersionId, List<string> fields, string method);
    }
}