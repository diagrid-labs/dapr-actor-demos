using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IHeadQuarters : IActor
    {
        Task SetRegionalOfficeIdsAsync(string[] localOfficeIds);
        Task<string[]> GetRegionalOfficeIdsAsync();
        Task SetEmployeeIdsAsync(IEnumerable<string> employeeIds);
        Task<string[]> GetEmployeeIdsAsync();
        Task FireEmployeeAsync(string employeeId);
    }
}