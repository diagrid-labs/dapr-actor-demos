using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class HeadQuartersActor : Actor, IHeadQuarters
    {
        private const string EMPLOYEE_IDS_KEY = "employee-ids";
        private const string REGIONAL_OFFICE_IDS_KEY = "local-office-ids";

        public HeadQuartersActor(ActorHost host) : base(host)
        {
        }

        public async Task SetRegionalOfficeIdsAsync(string[] regionalOfficeIds)
        {
            await StateManager.SetStateAsync(REGIONAL_OFFICE_IDS_KEY, regionalOfficeIds);
        }

        public async Task<string[]> GetRegionalOfficeIdsAsync()
        {
            return await StateManager.GetStateAsync<string[]>(REGIONAL_OFFICE_IDS_KEY);
        }

        public async Task SetEmployeeIdsAsync(IEnumerable<string> employeeIds)
        {
            await StateManager.SetStateAsync(EMPLOYEE_IDS_KEY, employeeIds);
        }

        public async Task<string[]> GetEmployeeIdsAsync()
        {
            return await StateManager.GetStateAsync<string[]>(EMPLOYEE_IDS_KEY);
        }

        public async Task FireEmployeeAsync(string employeeId)
        {
            Console.WriteLine("Firing employee {EmployeeId}!", employeeId);
            var employeeIds = await GetEmployeeIdsAsync();
            var employeesMinusFired = employeeIds.Where(emp => emp != employeeId);
            await SetEmployeeIdsAsync(employeesMinusFired);
        }
    }
}