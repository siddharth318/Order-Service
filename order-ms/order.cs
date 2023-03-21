using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace order_ms
{
    public static class order
    {
        [FunctionName("order")]
        public static async Task Run([ServiceBusTrigger("ffff", Connection = "con")]string myQueueItem, ILogger log)
        {
            dynamic data = JsonConvert.DeserializeObject(myQueueItem);


            string prodCost = data.prodCost;
            string prodID = data.prodId;
            int result = 0;


            //SQL connection
            SqlConnection conObj = new SqlConnection("Data Source=quickcart-server.database.windows.net;Initial Catalog=QuickCart-DB;user id=demouser; password=Siddharth@1234");

            //command
            SqlCommand cmdObj = new SqlCommand("usp_AddOrder", conObj);
            cmdObj.CommandType = CommandType.StoredProcedure;      

            try
            {
                SqlParameter prmReturnValue = new SqlParameter();
                prmReturnValue.Direction = ParameterDirection.ReturnValue;
                cmdObj.Parameters.Add(prmReturnValue);
                conObj.Open();
                cmdObj.ExecuteNonQuery();
                int res = Convert.ToInt32(prmReturnValue.Value);
                if (res == 1)
                    result = 1;//it means added
                else
                    result = 0;//error
            }
            catch (Exception e)
            {
                result = -1;

            }
            finally
            {
                conObj.Close();
            }

            log.LogInformation(""+result);


        }

       
    }
}
