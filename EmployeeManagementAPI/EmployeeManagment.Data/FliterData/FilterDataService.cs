using EmployeeManagment.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagment.Services.FliterData
{
    public class FilterDataService : IFilterDataService
    {
        private readonly string connectionString;
        public FilterDataService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("BatchedConnectionString")!;
        }
        public FilteredDataResult GetFilterData(string spName, SPPararm? parameters = null)
        {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_GetFilteredData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        SqlParameter ticketNumbersParam = command.Parameters.AddWithValue("@TicketNumbers", GetTicketNumbersDataTable(parameters!.TicketNumbers!));
                        ticketNumbersParam.SqlDbType = SqlDbType.Structured;
                        ticketNumbersParam.TypeName = "udt_TicketNumberList";

                        command.Parameters.AddWithValue("@EquipmentName", parameters.EquipmentName);
                        command.Parameters.AddWithValue("@TaskName", parameters.TaskName);
                        command.Parameters.AddWithValue("@WorkcenterName", parameters.WorkcenterName);
                        command.Parameters.AddWithValue("@PageSize", parameters.PageSize);
                        command.Parameters.AddWithValue("@PageNumber", parameters.PageNumber);
                        command.Parameters.AddWithValue("@orderBy", parameters.OrderBy);

                        DataSet dataSet = new DataSet();
                        List<FilterDataSourceId> filterDataSourceIds = new List<FilterDataSourceId>();
                        List<FilterDataTaskName> filterDataTaskName = new List<FilterDataTaskName>();
                        List<FIlterDataEqpName> filterDataEqp = new List<FIlterDataEqpName>();
                        List<FilterDataWorkcenterName> filterDataWorkCenterName = new List<FilterDataWorkcenterName>();
                        List<FilteredData> filterData = new List<FilteredData>();


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                filterDataSourceIds.Add(new FilterDataSourceId()
                                {
                                    SourceTicketId = reader["sourceTicketId"].ToString()
                                });
                            }
                            reader.NextResult();

                            while (reader.Read())
                            {
                                filterDataTaskName.Add(new()
                                {
                                    TaskName = reader["TaskName"].ToString()
                                });
                            }

                            reader.NextResult();
                            while (reader.Read())
                            {
                                filterDataWorkCenterName.Add(new()
                                {
                                    WorkcenterName = reader["WorkCenterName"].ToString()
                                });
                            }

                            reader.NextResult();
                            while (reader.Read())
                            {
                                filterDataEqp.Add(new()
                                {
                                    EquipmentName = reader["EquipmentName"].ToString()
                                });
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                filterData.Add(new FilteredData()
                                {
                                    SourceTicketId = reader["SourceTicketId"].ToString()!,
                                    TaskName = reader["TaskName"].ToString(),
                                    ChangeoverCount = (reader["ChangeoverCount"]).ToString(),
                                    ChangeoverDescription = reader["ChangeoverDescription"].ToString(),
                                    CustomerName = reader["CustomerName"].ToString(),
                                    PriceMode = (reader["PriceMode"]).ToString(),
                                    GeneralDescription = reader["GeneralDescription"].ToString(),
                                    WorkCenterName = reader["WorkCenterName"].ToString(),
                                    EquipmentName = reader["EquipmentName"].ToString(),
                                    DisplayName = reader["DisplayName"].ToString(),
                                    CalcCoreWidth = Convert.ToSingle(reader["CalcCoreWidth"]),
                                    CalcFinishedRollLength = Convert.ToSingle(reader["CalcFinishedRollLength"]),
                                    CalcLinearLength = Convert.ToSingle(reader["CalcLinearLength"]),
                                    CalcNumStops = Convert.ToSingle(reader["CalcNumStops"]),
                                    EndUserName = reader["EndUserName"].ToString(),
                                    IsBackSidePrinted = Convert.ToBoolean(reader["isBackSidePrinted"]),
                                    StockReceived = (reader["StockReceived"]).ToString(),
                                    DueOnSiteDate = Convert.ToDateTime(reader["DueOnSiteDate"]),
                                    ShippingStatus = reader["ShippingStatus"].ToString(),
                                    ShippingCity = reader["ShippingCity"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    PlateComplete = Convert.ToBoolean(reader["PlateComplete"]),
                                    Stock2Width = (reader["Stock2Width"]).ToString(),
                                    Equipment1Workcenter = reader["Equipment1Workcenter"].ToString(),
                                    Equipment2Workcenter = reader["Equipment2Workcenter"].ToString(),
                                    ToolingShape = reader["ToolingShape"].ToString()
                                });
                            }

                        }
                        var result = new FilteredDataResult(){  SourceIds= filterDataSourceIds, TaskNames = filterDataTaskName, EquipmentNames = filterDataEqp, WorkCenterName = filterDataWorkCenterName, data = filterData };
                        return result;
                    }
                }
            
        }
        private DataTable GetTicketNumbersDataTable(string tableName)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("TicketNumber", typeof(string));
            if (!string.IsNullOrEmpty(tableName))
            {
                foreach (var ticketNumber in tableName.Split(','))
                {
                    int.TryParse(ticketNumber, out int parsedTicketNumber);
                    dataTable.Rows.Add(parsedTicketNumber);
                }
            }
            return dataTable;
        }
    }
}
