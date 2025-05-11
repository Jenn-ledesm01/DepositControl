using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DepositControl.Areas.Bcri.Utility;
using DNF.Structure.Bussines;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DepositControl
{
    public class CsvReaderResult : ActionResult
    {
        private readonly DbDataReader dataReader;
        private readonly bool includeHeaderAsFirstRow;
        private readonly string separator;
        private readonly string fileName;
        private readonly Entity entity;
        public CsvReaderResult(DbDataReader reader, string fileName, bool includeHeaderAsFirstRow = true, string separator = ";")
        {
            dataReader = reader;
            this.includeHeaderAsFirstRow = includeHeaderAsFirstRow;
            this.separator = separator;
            this.fileName = fileName;
        }

        public CsvReaderResult(DbDataReader reader, string fileName, Entity entity, bool includeHeaderAsFirstRow = true, string separator = ";")
        {
            dataReader = reader;
            this.includeHeaderAsFirstRow = includeHeaderAsFirstRow;
            this.separator = separator;
            this.fileName = fileName;
            this.entity = entity;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            try
            {
                var response = context.HttpContext.Response;
                response.ClearHeaders();
                response.ContentType = "text/csv";
                response.AddHeader("content-disposition", $"attachment; filename = '{fileName}.csv'");
                var sw = new StreamWriter(response.OutputStream);
                // dicionario de nombres para columnas con related id 
                var dicColNames = new Dictionary<string, Dictionary<string, string>>();
                if (entity != null)
                {
                    for (int index = 0; index < dataReader.FieldCount; index++)
                    {
                        bool custom;
                        if (dataReader.GetName(index) != null)
                        {
                            var colName = dataReader.GetName(index);
                            Dictionary<string, string> dicNames = entity[colName]?.GetDicNameByRelated(out custom);
                            if (dicNames != null && dicNames.Count != 0)
                                dicColNames.Add(colName, dicNames);
                        }
                    }
                }


                if (includeHeaderAsFirstRow)
                {
                    for (int index = 0; index < dataReader.FieldCount; index++)
                    {
                        var colName = dataReader.GetName(index);
                        if (colName != null)
                        {
                            sw.Write(colName);

                            if (dicColNames.ContainsKey(colName))
                                sw.Write(separator + colName.Replace("_Id", "_Name"));
                        }

                        if (index < dataReader.FieldCount - 1)
                            sw.Write(separator);

                    }
                    sw.WriteLine();
                }

                while (dataReader.Read())
                {
                    for (var index = 0; index < dataReader.FieldCount; index++)
                    {
                        var colName = dataReader.GetName(index);
                        if (!dataReader.IsDBNull(index))
                        {
                            var value = dataReader.GetValue(index).ToString();
                            if (dataReader.GetFieldType(index) == typeof(string))
                            {
                                value = prepareValue(value);
                            }
                            sw.Write(value);

                            if (dicColNames.ContainsKey(colName))
                            {
                                sw.Write(separator);
                                var dicNames = dicColNames[colName];
                                if (dicNames.ContainsKey(value))
                                    sw.Write(prepareValue(dicNames[value]));
                            }
                        }

                        if (index < dataReader.FieldCount - 1)
                            sw.Write(separator);
                    }

                    //if (!dataReader.IsDBNull(dataReader.FieldCount - 1)) //no entiendo por que hace eso
                    //    sw.Write(dataReader.GetValue(dataReader.FieldCount - 1).ToString().Replace(separator, " "));

                    sw.WriteLine();

                }
                sw.Flush();
            }
            finally
            {
                dataReader.Close();
            }
        }

        private string prepareValue(string value)
        {
            if (value.IndexOf("\"", StringComparison.Ordinal) >= 0)
                value = value.Replace("\"", "\"\"");

            //If separtor are is in value, ensure it is put in double quotes.
            if (value.IndexOf(separator, StringComparison.Ordinal) >= 0)
                value = "\"" + value + "\"";
            return value;
        }

    }
}
