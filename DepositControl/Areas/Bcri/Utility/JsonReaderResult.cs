using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DepositControl
{
    public class JsonReaderResult : JsonResult
    {
        private DbDataReader Reader;
        public JsonReaderResult(DbDataReader reader, JsonSerializer serializer =  null)
        {
            Reader = reader;
            if(serializer != null)
                Serializer = serializer;
        }

        private static JsonSerializer Serializer = new JsonSerializer();

        public override void ExecuteResult(ControllerContext context)
        {
            JsonRequestBehavior =  JsonRequestBehavior.AllowGet;
            //if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
            //    string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new InvalidOperationException("GET request not allowed");
            //}
            try
            {
                var response = context.HttpContext.Response;
                if (!response.HeadersWritten)
                    response.ContentType =
                        !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

                response.ContentEncoding = this.ContentEncoding ?? response.ContentEncoding;

                var cols = new List<string>();
                for (var i = 0; i < Reader.FieldCount; i++)
                    cols.Add(Reader.GetName(i)); // recupero todas las columnas

                using (StreamWriter sw = new StreamWriter(response.OutputStream))
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {

                    writer.WriteStartArray();
                    while (Reader.Read())
                    {
                        if (!response.IsClientConnected)
                            break;

                        Serializer.Serialize(writer, cols.ToDictionary(col => col, //nombre columna 
                            col => Reader[col]));
                    }
                    writer.WriteEndArray();
                }
            }
            finally
            {
                Reader.Close();
                Reader.Dispose();
            }
        }
    }
}
