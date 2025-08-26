using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using log4net;
using System.Diagnostics;
using static GHAPIServices.DBHelper;

namespace GHAPIServices
{
    public class CommonGHAPIService
    {
        private DBHelper _dbHelper;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommonGHAPIService));
        public CommonGHAPIService()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
            _dbHelper = new DBHelper(connectionString);
        }

        public List<AccountCustomField> GetAccountData()
        {
            try
            {
                var FieldsData = _dbHelper.ExecuteDataTable("sp_AccountMaster_GreenHouse", System.Data.CommandType.StoredProcedure);
                Logger.Info("Getting Account data using sp");
                var customFields = new List<AccountCustomField>();
                if (FieldsData.Rows.Count > 0)
                {
                    foreach (DataRow row in FieldsData.Rows)
                    {
                        var customField = new AccountCustomField();
                        {
                            customField.Name = row["CustomerName"].ToString();
                        }
                        //customField.Id = int.Parse(row["CustomerID"].ToString());

                        if (!string.IsNullOrEmpty(customField.Name))
                        {
                            customFields.Add(customField);
                        }
                    }
                }
                return customFields;
            }
            catch (Exception ex)
            {
                Logger.Error(ex + "Unable to get Account data on GetAccountData");
            }
            return null;
        }

        public List<ProjectCustomField> GetProjectData()
        {
            try
            {
                var FieldsData = _dbHelper.ExecuteDataTable("sp_ProjectMaster_GreenHouse", System.Data.CommandType.StoredProcedure);
                Logger.Info("Getting Project data using sp");
                var customFields = new List<ProjectCustomField>();
                if (FieldsData.Rows.Count > 0)
                {
                    foreach (DataRow row in FieldsData.Rows)
                    {
                        var customField = new ProjectCustomField();
                        {
                            customField.Name = row["ProjName"].ToString();
                        }


                        if (!string.IsNullOrEmpty(customField.Name))
                        {
                            customFields.Add(customField);
                        }
                    }
                }
                return customFields;
            }
            catch(Exception ex)
            {
                Logger.Error(ex + "Unable to get Project data on GetProjectData");
            }
            return null;
        }

        public string ConvertAccountDataToJson(List<AccountCustomField> customFields)
        {
            try
            {
                Logger.Info("Converting Account data into json format");
                var options = new
                {
                    options = customFields.Select(cf => new
                    {
                        name = cf.Name,
                        priority = 0,
                        external_id = (string)null
                    }).ToList()
                };
                return JsonConvert.SerializeObject(options);
                
            }
            catch(Exception ex)
            {
                Logger.Error(ex + "Error on ConvertAccountDataToJson method");
            }
            return null;
        }

        public string ConvertProjectDataToJson(List<ProjectCustomField> customFields)
        {
            try
            {
                Logger.Info("Converting Project data into json format");
                var options = new
                {
                    options = customFields.Select(cf => new
                    {
                        name = cf.Name,
                        priority = 0,
                        external_id = (string)null
                    }).ToList()
                };
                return JsonConvert.SerializeObject(options);
            }
            catch(Exception ex)
            {
                Logger.Error(ex + "Error on ConvertProjectDataToJson method");
            }
            return null;
        }

        public void AddAccountHistory(AccountHistory accountHistories)
        {
            try
            {           
                var paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("@AccountName", accountHistories.Name ?? (object)DBNull.Value));
                paramCollection.Add(new DBParameter("@StatusCode", accountHistories.StatusCode ?? (object)DBNull.Value));
                paramCollection.Add(new DBParameter("@Content", accountHistories.Content ?? (object)DBNull.Value));
                paramCollection.Add(new DBParameter("@CreateDate", DateTime.Now));
                _dbHelper.ExecuteNonQuery("sp_AccountMaster_GreenHouseHistory", paramCollection, CommandType.StoredProcedure);
                Logger.Info("Added Account history in to sp");
            }
            catch (Exception ex)
            {
                Logger.Error(ex + "Error on AddAccountHistory Method");
            }
            
        }

        public void AddProjectHistory(ProjectHistory projectHistories)
        {
            try
            {
                var paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("@ProjectName", projectHistories.Name ?? (object)DBNull.Value));
                paramCollection.Add(new DBParameter("@StatusCode", projectHistories.StatusCode ?? (object)DBNull.Value));
                paramCollection.Add(new DBParameter("@Content", projectHistories.Content ?? (object)DBNull.Value));
                paramCollection.Add(new DBParameter("@CreateDate", DateTime.Now));
                _dbHelper.ExecuteNonQuery("sp_ProjectMaster_GreenHouseHistory", paramCollection, CommandType.StoredProcedure);
                Logger.Info("Added Project history in to sp");
            }
            catch (Exception ex)
            {
                Logger.Error(ex + "Error on AddProjectHistory Method");
            }
          
        }
    }
}
