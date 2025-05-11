using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DepositControl.Areas.Bcri.Controllers;
using DNF.Entity;
using DNF.Structure.Bussines;

namespace DepositControl.Areas.Bcri.Utility
{
    public class XlsEntityReaderResult : ActionResult

    {
        public XlsEntityReaderResult(string entityName, long? repositoryId = null, string separator = ";")
        {
            this.entityName = entityName;
            this.repositoryId = repositoryId;
            this.separator = separator;
            
        }

        private string entityName;
        private string separator;
        private long? repositoryId;
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            var sw = new StreamWriter(response.OutputStream, Encoding.UTF8);
            var entity = Entity.Dao.GetByName(entityName);
            var repo = RepositoryConfig.Dao.GetBy(entity).FirstOrDefault();
            response.ClearHeaders();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("content-disposition", $"attachment; filename = {repo?.Name ?? entity.Name}.xls");
            List<Struct> structs = entity.Structs.Where(x => x.Name != "Repository_Id"
                                                               && x.Name != "ProcessConfig_Id"
                                                               && x.Name != "Id"
                                                               && (x.InTable || x.InView)
                                                               ).OrderBy(x => x.ImportOrder)
                                                               .ToList();
            structs.LoadRelation(x => x.Entity);
            Dictionary<string, Dictionary<long, string>> ColEntities = new Dictionary<string, Dictionary<long, string>>();
            foreach (var it in structs)
            {
                var hasRelatedEntity = !string.IsNullOrEmpty(it.RefEntity);
                string selectValues = null;
                string searchSelectValues = null;
                if (hasRelatedEntity)
                {
                    var selectItems = new DbReader(it.RefEntity).GetListItemDictionay(it);
                    if (selectItems != null && selectItems.Count > 0)
                        ColEntities.Add(it.Name, selectItems);
                }
            }

            var GenericEntity = new DbReader(entityName, repositoryId);
            IDataReader DRFilas = GenericEntity.GetAll();

            try
            {


                var xlsHeader = "<?xml version=\"1.0\"?> " +
                    "<Workbook xmlns =\"urn:schemas-microsoft-com:office:spreadsheet\"" +
                    " xmlns:o=\"urn:schemas-microsoft-com:office:office\"" +
                    " xmlns:x=\"urn:schemas-microsoft-com:office:excel\"" +
                    " xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" " +
                    " xmlns:html=\"http://www.w3.org/TR/REC-html40\">" + Environment.NewLine +
                    "<Styles><Style ss:ID=\"s22\"><NumberFormat ss:Format=\"yyyy-mm-dd\"/></Style></Styles>" + Environment.NewLine +
                    "<Worksheet ss:Name=\"Sheet1\">" + Environment.NewLine +
                    "<Table>";

                sw.WriteLine(xlsHeader);

                sw.Write("<Row>");
                string[] headers = structs.Select(x => string.IsNullOrEmpty(x.Description) ? x.Name : x.Description).ToArray();
                foreach (var head in headers)
                    sw.Write($"<Cell><Data ss:Type=\"String\">{head}</Data></Cell>");

                sw.WriteLine("</Row>");


                while (DRFilas.Read())
                {
                    sw.Write("<Row>");
                    for (int index = 0; index < structs.Count; index++)
                    {
                        Struct Columna = structs[index];
                        string CeldaExcel = " ";
                        string CeldaStyle = "";
                        string CeldaType = "String";

                        if (!DRFilas[Columna.Name].Equals(DBNull.Value)) //si es nulo no se imprime nada
                        {
                            
                            if (!string.IsNullOrEmpty(Columna.RefEntity) && ColEntities.ContainsKey(Columna.Name)) //parseo columnas con ref entity foren key
                            {
                                Dictionary<long, string> RefValues = ColEntities[Columna.Name];

                                CeldaExcel = RefValues.ContainsKey(long.Parse(DRFilas[Columna.Name].ToString()))
                                    ? RefValues[long.Parse(DRFilas[Columna.Name].ToString())]
                                    : DRFilas[Columna.Name].ToString(); //si no esta en el diccionario mando el id
                            }
                            else if (Columna.DataType.Behavior == 4) //parseo bool
                                CeldaExcel = (bool)DRFilas[Columna.Name] ? "Yes" : "No";
                            else
                            {
                                if (Columna.DataType.Behavior == 2)
                                    CeldaType = "Number";
                                else if (Columna.DataType.Behavior == 3)
                                    CeldaType = "DateTime";

                                if (Columna.DataType.Type == typeof(int))
                                    CeldaExcel = string.Format("{0}", Convert.ToInt32(DRFilas[Columna.Name]));
                                else if (Columna.DataType.Type == typeof(decimal))
                                    CeldaExcel = string.Format("{0:N" + Columna.LengthDecimal + "}", (decimal)DRFilas[Columna.Name]);
                                else if (Columna.DataType.Type == typeof(DateTime))
                                {
                                    CeldaExcel = string.Format("{0:yyyy-MM-dd}", (DateTime)DRFilas[Columna.Name]);
                                    CeldaStyle = " ss:StyleID=\"s22\"";
                                }
                                else
                                    CeldaExcel = DRFilas[Columna.Name].ToString();
                            }
                        }

                        sw.Write($"<Cell{CeldaStyle}><Data ss:Type=\"{CeldaType}\">{CeldaExcel}</Data></Cell>");
                    }
                    sw.WriteLine("</Row>");
                }
                sw.Write("</Table></Worksheet></Workbook>");
                sw.Flush();
            }
            finally
            {
                DRFilas.Close();
                sw.Close();
            }
        }


    }

}
