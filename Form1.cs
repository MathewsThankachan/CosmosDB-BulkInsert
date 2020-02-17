using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using Container = Microsoft.Azure.Cosmos.Container;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using Microsoft.Azure.Documents.Client;
using Cosmos.Samples.Shared;
using System.Diagnostics;
using System.Text;

namespace BulkInsert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private async void btnBulkInsert_Click(object sender, EventArgs e)
        {
            btnBulkInsert.Enabled = false;
            string EndpointUrl = ConfigurationManager.AppSettings["EndpointUrl"];
            string AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
            string DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
            string ContainerName = ConfigurationManager.AppSettings["ContainerName"];

            CosmosClientOptions options = new CosmosClientOptions()
            {
                AllowBulkExecution = true,
                ConnectionMode = Microsoft.Azure.Cosmos.ConnectionMode.Direct,
                MaxRequestsPerTcpConnection = 1000,
                MaxRetryAttemptsOnRateLimitedRequests = 100,
                RequestTimeout = new TimeSpan(0, 5, 0),
                MaxTcpConnectionsPerEndpoint = 1000,
                OpenTcpConnectionTimeout = new TimeSpan(0, 5, 0),
                

            };
            CosmosClient client = new CosmosClient(EndpointUrl, AuthorizationKey,options);
            Database database = await client.CreateDatabaseIfNotExistsAsync(DatabaseName);
            Container container = await database.CreateContainerIfNotExistsAsync(ContainerName, "/Postcode");



            string filePath = Path.GetDirectoryName(Application.ExecutablePath).Replace(@"bin\Debug", ConfigurationManager.AppSettings["FilePath"]);

            List<PricePaidData> lstPricedata = new List<PricePaidData>();
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Reading File and appending to List : " + DateTime.Now.ToString("dd--MMM-yyyy hh:mm:ss"));
            lstPricedata = readfile(filePath);

            Console.WriteLine("Total Time reading the file :  " + sw.ElapsedMilliseconds.ToString());
            sw.Restart();

            // 500k --> 15 min @ 20,000RU/s, && 11 mins  @ 100,000RU/s
            // 1 mn -->  min @ 100,000RU/s

            List<PricePaidData> lstPricedata1 = lstPricedata.GetRange(0, 500000);


            sw.Start();
            int cnt = 0;


            #region async

            List<Task> tasks = new List<Task>();
            foreach (var item in lstPricedata1)
            {
                cnt++;
                tasks.Add(container.CreateItemAsync<PricePaidData>(item, new PartitionKey(item.Postcode)));
            }

            await Task.WhenAll(tasks);
            #endregion

            #region WithStreamWriter
            //foreach (var item in lstPricedata1)
            //{

            //    cnt++;
            //    MemoryStream ms = new MemoryStream();
            //    JsonSerializer Serializer = new JsonSerializer();
            //    using (StreamWriter streamwriter = new StreamWriter(ms, Encoding.UTF8))
            //    {
            //        using (JsonWriter writer = new JsonTextWriter(streamwriter))
            //        {
            //            writer.Formatting = Newtonsoft.Json.Formatting.None;
            //            Serializer.Serialize(writer, item);
            //            writer.Flush();
            //            streamwriter.Flush();
            //            ms.Position = 0;
            //            ResponseMessage response = await container.CreateItemStreamAsync(ms, new PartitionKey(item.Postcode));
            //            if (response.IsSuccessStatusCode)
            //                successCnt++;

            //        }

            //    }
            //}
            #endregion

            btnBulkInsert.Enabled = true;
            Console.WriteLine("Total Time writing to CosmosDB  :  " + (sw.ElapsedMilliseconds/1000).ToString() + "  seconds");
            MessageBox.Show("Total Time writing to CosmosDB  :  " + (sw.ElapsedMilliseconds / 1000).ToString() + "  secods");
            sw.Stop();


        }


        private List<PricePaidData> readfile(string filePath)
        {
            List<PricePaidData> lstPricedata = new List<PricePaidData>();
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] pricedataArray = line.Split(',');
                            PricePaidData ppd = new PricePaidData()
                            {
                                Transaction_unique_identifieroperty = pricedataArray[0],
                                Price = pricedataArray[1].Replace("\"", ""),
                                Date_of_Transfer = pricedataArray[2].Replace("\"", ""),
                                Postcode = pricedataArray[3].Replace("\"", ""),
                                PropertyType = pricedataArray[4].Replace("\"", ""),
                                isNew = pricedataArray[5].Replace("\"", ""),
                                Duration = pricedataArray[6].Replace("\"", ""),
                                PAON = pricedataArray[7].Replace("\"", ""),
                                SAON = pricedataArray[8].Replace("\"", ""),
                                Street = pricedataArray[9].Replace("\"", ""),
                                Locality = pricedataArray[10].Replace("\"", ""),
                                Town_City = pricedataArray[11].Replace("\"", ""),
                                District = pricedataArray[12].Replace("\"", ""),
                                County = pricedataArray[13].Replace("\"", ""),
                                PPD_Category = pricedataArray[14].Replace("\"", ""),
                                Record_Status = pricedataArray[15].Replace("\"", "")

                            };
                            lstPricedata.Add(ppd);
                        }
                    }
                }
            }
            return lstPricedata;
        }

    }
}

