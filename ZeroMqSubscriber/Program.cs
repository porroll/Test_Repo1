using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;
using ZeroMqSubscriber.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;


namespace ZeroMqSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
                string topic = "device";
                string url = "tcp://10.38.129.170:5560";
                using (var context = new ZContext())
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                {
                    subscriber.Connect("tcp://10.38.129.170:5560");
                    Console.WriteLine("Subscriber started for Topic with URL : {0} {1}", topic, url);
                    subscriber.Subscribe(topic);
                    int subscribed = 0;

                    while (true)
                    {

                        using (ZMessage message = subscriber.ReceiveMessage())
                        {
                            subscribed++;

                            // Read envelope with address
                            string address = message[0].ReadString();

                            // Read message contents
                            string contents = message[1].ReadString();

                            LocationData objLocationData = JsonConvert.DeserializeObject<ListOfArea>(contents).device_notification.records.FirstOrDefault();
                            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                            {

                                conn.Open();
                                foreach (var item in objLocationData.an)
                                {
                                    objLocationData.AreaName = item;
                                    // Create the connection to the resource!
                                    // This is the connection, that is established and
                                    // will be available throughout this block.
                                    // Add some changes here .................... again .....

                                    // Create the command, to insert the data into the Table!
                                    // this is a simple INSERT INTO command!
                                    // Create the command to execute!With the wrong name of the table (Depends on your Database tables)
                                    //SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.LocationDatas (mac,Sequence,sn,bn,fn,x,y,z,last_seen_ts,action,fix_result,AreaName) VALUES (@mac,@Sequence,@sn,@bn,@fn,@x,@y,@z,@last_seen_ts,@action,@fix_result,@AreaName)", conn);

                                    // In the command, there are some parameters denoted by @, you can 
                                    // change their value on a condition, in my code they're hardcoded.

                                    //insertCommand.Parameters.Add(new SqlParameter("@mac", objLocationData.mac));
                                    //insertCommand.Parameters.Add(new SqlParameter("@Sequence", objLocationData.sequence));
                                    //insertCommand.Parameters.Add(new SqlParameter("@sn", objLocationData.sn));
                                    //insertCommand.Parameters.Add(new SqlParameter("@bn", objLocationData.bn));
                                    //insertCommand.Parameters.Add(new SqlParameter("@fn", objLocationData.fn));
                                    //insertCommand.Parameters.Add(new SqlParameter("@x", objLocationData.x));
                                    //insertCommand.Parameters.Add(new SqlParameter("@y", objLocationData.y));
                                    //insertCommand.Parameters.Add(new SqlParameter("@z", objLocationData.z));
                                    //insertCommand.Parameters.Add(new SqlParameter("@last_seen_ts", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(objLocationData.last_seen_ts).ToString()));
                                    //insertCommand.Parameters.Add(new SqlParameter("@action", objLocationData.action));
                                    //insertCommand.Parameters.Add(new SqlParameter("@fix_result", objLocationData.fix_result));
                                    //insertCommand.Parameters.Add(new SqlParameter("@AreaName", item));

                                    // Execute the command, here the error will pop up!
                                    //Console.WriteLine("Commands executed! Total rows affected are " + insertCommand.ExecuteNonQuery());
                                    Console.WriteLine(objLocationData.mac + " - " + objLocationData.x + " - " + objLocationData.y + " - " + new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(objLocationData.last_seen_ts).ToString());
                                }
                            }
                            // Final step, close the resources flush dispose them. ReadLine to prevent the console from closing.
                            // Console.ReadLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw ex;
     
            }

        }

    }
}

