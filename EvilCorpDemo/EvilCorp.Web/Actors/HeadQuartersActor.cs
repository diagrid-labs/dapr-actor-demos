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

        public async Task SetEmployeeIdsAsync(Dictionary<string, string[]> employeeIds)
        {
            await StateManager.SetStateAsync(EMPLOYEE_IDS_KEY, employeeIds);
        }

        public async Task<Dictionary<string, string[]>> GetEmployeeIdsAsync()
        {
            return await StateManager.GetStateAsync<Dictionary<string, string[]>>(EMPLOYEE_IDS_KEY);
        }

        public async Task FireEmployeeAsync(string regionalOfficeId, string employeeId)
        {
            Logger.LogInformation("Firing employee {EmployeeId} at {RegionalOfficeId}!", employeeId, regionalOfficeId);

            var employeeIds = await GetEmployeeIdsAsync();
            Logger.LogInformation("Employee count before: {}", employeeIds.Values.Count);
            var employeesMinusFired = employeeIds[regionalOfficeId].Where(emp => emp != employeeId);
            employeeIds[regionalOfficeId] = employeesMinusFired.ToArray();
            await SetEmployeeIdsAsync(employeeIds);

            Logger.LogInformation("Employee count after: {}", employeeIds.Values.Count);
        }

        public async Task<string[]> GetEmployeeIdsForRegionalOfficeAsync(string regionalOfficeId)
        {
            var employeeIds = await GetEmployeeIdsAsync();
            return employeeIds[regionalOfficeId];
        }
    }
}