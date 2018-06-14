using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DelgateStorePOC
{

    public class Store
    {
        public bool StoreAction(Action action)
        {
            var actionObject = action.Clone();

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, actionObject);
           
            // Set the position to the beginning of the stream.
            memoryStream.Seek(0, SeekOrigin.Begin);
            // Read the first 20 bytes from the stream.
            SaveToDataBase(memoryStream);

            var byteData = ReadActionFromDB();
            MemoryStream ms = new MemoryStream(byteData);
            var obj=  binaryFormatter.Deserialize(ms);
            var deserializedAction = (Action)obj;
            deserializedAction();
            return true;
        }

        public void SaveToDataBase(MemoryStream ms)
        {
            string connection = @"Data Source=****\SQLEXPRESS;Initial Catalog=POC;Integrated Security=True;";

            byte[] bytes = new byte[Int16.MaxValue];
             int count = 0;
            // Set the position to the beginning of the stream.
            ms.Seek(0, SeekOrigin.Begin);
            // Read the first 20 bytes from the stream.
            bytes = new byte[ms.Length];
            count = ms.Read(bytes, 0, 20);
            // Read the remaining bytes, byte by byte.
            while (count < ms.Length)
            {
                bytes[count++] = Convert.ToByte(ms.ReadByte());
            }

            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand();
                string commandtext = "SaveAction";
                cmd.CommandText = commandtext;
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@data", bytes));
                con.Open();

                cmd.ExecuteNonQuery();
            }

        }

        public byte[] ReadActionFromDB()
        {
            string connection = @"Data Source=****\SQLEXPRESS;Initial Catalog=POC;Integrated Security=True;";
            byte[] bytes = new byte[Int16.MaxValue];

            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand();
                string commandtext = "SELECT TOP 1 Action FROM [ActionInfo]";
                cmd.CommandText = commandtext;
                cmd.Connection = con;
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    bytes = (byte[])reader["Action"];
 
                }
            }

            return bytes;
        }
    }
}
