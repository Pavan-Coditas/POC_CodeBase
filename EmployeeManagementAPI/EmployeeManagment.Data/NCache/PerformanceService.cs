using EmployeeManagement;
using EmployeeManagement.Context;
using EmployeeManagement.Domain.Models.ResponseModel;
using EmployeeManagement.Domain.Models.RulesEngineModels;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services.Cache;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RuleEnginePerformanceTest;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.Json;

namespace EmployeeManagment.Services.NCache
{
    public class PerformanceService : IPerformanceService
    {
        private SqlConnection _connection;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ICacheService _cacheService;
        private readonly string _redisKey;
        public PerformanceService(IUnitOfWorkFactory unitOfWorkFactory, ICacheService cacheService)
        {

            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var connectionString = configuration.GetConnectionString("SecureConnection");
            _connection = SQLConnectionManager.Instance.GetConnection(connectionString);
            //_connection.Open();
            _unitOfWorkFactory = unitOfWorkFactory;
            _redisKey = UserContext.UserName;
            _cacheService = cacheService;
        }
        //public PerformanceResponse GetAllUsingNcash()
        //{
        //    try
        //    {
        //        var stopwatch = Stopwatch.StartNew();
        //        var cacheData = _nCacheService.GetData<IEnumerable<Person>>(_redisKey);
        //        stopwatch.Stop();

        //        if (cacheData != null)
        //        {
        //            var response = new PerformanceResponse()
        //            {
        //                TimeTaken = stopwatch.ElapsedMilliseconds.ToString() + " ms",
        //                Message = "NCache"
        //            };
        //            return response;
        //        }

        //        using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
        //        {
        //            cacheData = unitOfWork.GetRepository<Person>().GetAll();
        //        }

        //        var expirationTime = TimeSpan.FromMinutes(10);
        //        var res = _nCacheService.SetData(_redisKey, cacheData, Alachisoft.NCache.Runtime.Caching.ExpirationType.Sliding, expirationTime);
        //        var responseModel = new PerformanceResponse()
        //        {
        //            TimeTaken = "0",
        //            Message = "Cache is empty try to send request one more time"
        //        };
        //        return responseModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public PerformanceResponse GetUsingRedis()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var cacheData = _cacheService.GetData<IEnumerable<Person>>(_redisKey);
                stopwatch.Stop();

                if (cacheData != null)
                {
                    var response = new PerformanceResponse()
                    {
                        TimeTaken = stopwatch.ElapsedMilliseconds.ToString() + " ms",
                        Message = "RedisCache"
                    };
                    return response;
                }

                using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
                {
                    cacheData = unitOfWork.GetRepository<Person>().GetAll();
                }

                var expirationTime = DateTimeOffset.Now.AddMinutes(10.0);
                var res = _cacheService.SetData(_redisKey, cacheData, expirationTime);
                var responseModel = new PerformanceResponse()
                {
                    TimeTaken = "0",
                    Message = "Cache is empty try to send request one more time"
                };
                return responseModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetPersons()
        {
            string filepath = null!;
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                var data = unitOfWork.GetRepository<Person>().GetAll().GetRange(0, 10000);
                filepath = ConvertDataToJsonFile(data);
                return filepath;
            }
        }

        public List<Person> GetAllPersons()
        {
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                var data = unitOfWork.GetRepository<Person>().GetAll().GetRange(0, 10000);
                return CalculateSalary(data);
            }
        }

        public List<Person> CalculateSalary(List<Person> employees)
        {
            string rulesJsonFilePath = "C:\\Users\\Coditas\\source\\repos\\EmployeeManagementAPI\\EmployeeManagment.Data\\RulesEngine\\Rules.json";
            string jsonContent = File.ReadAllText(rulesJsonFilePath);

            // Deserialize the JSON data into a list of WorkflowRule objects
            var workflowRules = JsonConvert.DeserializeObject<List<WorkflowRule>>(jsonContent)!;
            var ruleEngine = new RuleEngine(workflowRules);
            var res = ruleEngine.CalculateSalary(employees);
            return res;
        }

        public async Task DeserializeAndBulkInsert(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            using (var streamReader = new StreamReader(fileStream))
            {
                string line;
                var persons = new List<Person>();

                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    var person = System.Text.Json.JsonSerializer.Deserialize<Person>(line);
                    persons.Add(person!);
                }

                // Perform bulk insert
                await BulkInsertFromJsonFileAsync(persons, "PersonCopy");
            }

        }

        public async Task BulkInsertFromJsonFileAsync(List<Person> data, string tableName)
        {

            var dataTable = ConvertToDataTable(data!);

            using (var sqlBulkCopy = new SqlBulkCopy(_connection))
            {
                // Map the columns in the DataTable to the destination table columns
                foreach (DataColumn column in dataTable.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                // Set the destination table name
                sqlBulkCopy.BulkCopyTimeout = 0;
                sqlBulkCopy.DestinationTableName = tableName;

                try
                {
                    // Perform the bulk insert
                    await sqlBulkCopy.WriteToServerAsync(dataTable);
                    _connection.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private DataTable ConvertToDataTable<T>(List<T> data)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            foreach (var item in data)
            {
                var row = dataTable.NewRow();

                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static string ConvertDataToJsonFile(List<Person> data)
        {
            var stopwatch = Stopwatch.StartNew();
            string folderPath = "C:\\Users\\Coditas\\source\\repos\\EmployeeManagementAPI\\EmployeeManagementAPI\\JsonData\\";
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, "output.json");

            using (var streamWriter = new StreamWriter(filePath))
            {
                foreach (var person in data)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = false
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(person, jsonOptions);
                    streamWriter.WriteLine(json);
                }
            }
            stopwatch.Stop();
            return filePath;
        }

    }
}
