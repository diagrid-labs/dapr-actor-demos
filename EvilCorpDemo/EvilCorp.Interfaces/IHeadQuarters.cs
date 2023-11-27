using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IHeadQuarters : IActor
    {
        Task SetRegionalOfficeIdsAsync(string[] regionalOfficeIds);
        Task<string[]> GetRegionalOfficeIdsAsync();
        Task SetEmployeeIdsAsync(Dictionary<string, string[]> employeeIds);
        Task<Dictionary<string,string[]>> GetEmployeeIdsAsync();
        Task<Dictionary<string,string[]>> GetFiredEmployeeIdsAsync();
        Task<string[]> GetEmployeeIdsForRegionalOfficeAsync(string regionalOfficeId);
        Task FireEmployeeAsync(string regionalOfficeId, string employeeId);
    }
}