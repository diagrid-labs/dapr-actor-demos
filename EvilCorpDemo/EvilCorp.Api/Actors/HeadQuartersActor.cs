using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class HeadQuartersActor : Actor, IHeadQuarters
    {
        private const string EMPLOYEE_IDS_KEY = "employee-ids";
        private const string FIRED_EMPLOYEE_IDS_KEY = "fired-employee-ids";
        private const string REGIONAL_OFFICE_IDS_KEY = "regional-office-ids";

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
            var employeesMinusFired = employeeIds[regionalOfficeId].Where(emp => emp != employeeId);
            employeeIds[regionalOfficeId] = employeesMinusFired.ToArray();
            await SetEmployeeIdsAsync(employeeIds);
            await UpdateFiredEmployeeIdsAsync(regionalOfficeId, employeeId);
        }
        public async Task<Dictionary<string, string[]>> GetFiredEmployeeIdsAsync()
        {
            var stateResult = await StateManager.TryGetStateAsync<Dictionary<string, string[]>>(FIRED_EMPLOYEE_IDS_KEY);
            if (stateResult.HasValue)
            {
                return stateResult.Value;
            }
            else
            {
                return new Dictionary<string, string[]>();
            }
        }

        private async Task UpdateFiredEmployeeIdsAsync(string regionalOfficeId, string employeeId)
        {
            var firedEmployees = await GetFiredEmployeeIdsAsync();
            if (firedEmployees.ContainsKey(regionalOfficeId))
            {
                var employees = firedEmployees[regionalOfficeId].ToList();
                employees.Add(employeeId);
                firedEmployees[regionalOfficeId] = employees.ToArray();
            }
            else
            {
                firedEmployees.Add(regionalOfficeId, new[] {employeeId});
            }
            await StateManager.SetStateAsync(FIRED_EMPLOYEE_IDS_KEY, firedEmployees);
        }

    }
}