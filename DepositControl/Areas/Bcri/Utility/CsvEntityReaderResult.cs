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
    public class CsvEntityReaderResult : ActionResult

    {
        public CsvEntityReaderResult(string entityName, long? repositoryId = null, string separator = ";")
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
            response.ContentType = "text/csv";
            response.AddHeader("content-disposition", $"attachment; filename = {repo?.Name ?? entity.Name}.csv");
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
                string header = structs.Select(x => string.IsNullOrEmpty(x.Description) ? x.Name : x.Description)
                    .ToJoin(separator);
                sw.WriteLine(header);

                while (DRFilas.Read())
                {
                    string FilaExcel = "";
                    for (int index = 0; index < structs.Count; index++)
                    {
                        Struct Columna = structs[index];

                        if (!DRFilas[Columna.Name].Equals(DBNull.Value)) //si es nulo no se imprime nada
                        {
                            string CeldaExcel = "";
                            if (!string.IsNullOrEmpty(Columna.RefEntity) && ColEntities.ContainsKey(Columna.Name)
                            ) //parseo columnas con ref entity foren key
                            {
                                Dictionary<long, string> RefValues = ColEntities[Columna.Name];

                                CeldaExcel = RefValues.ContainsKey(long.Parse(DRFilas[Columna.Name].ToString()))
                                    ? RefValues[long.Parse(DRFilas[Columna.Name].ToString())]
                                    : DRFilas[Columna.Name].ToString(); //si no esta en el diccionario mando el id
                            }
                            else if (Columna.DataType.Behavior == 4) //parseo bool
                                CeldaExcel = (bool) DRFilas[Columna.Name] ? "Yes" : "No";
                            else
                            {
                                if (Columna.DataType.Type == typeof(int))
                                    CeldaExcel = string.Format("{0}", Convert.ToInt32(DRFilas[Columna.Name]));
                                else if (Columna.DataType.Type == typeof(decimal))
                                    CeldaExcel = string.Format("{0:N" + Columna.LengthDecimal + "}", (decimal)DRFilas[Columna.Name]);
                                else
                                    CeldaExcel = DRFilas[Columna.Name].ToString();
                            }


                            if (CeldaExcel.IndexOf("\"", StringComparison.Ordinal) >= 0)
                                CeldaExcel = CeldaExcel.Replace("\"", "\"\"");
                            //If separtor are is in value, ensure it is put in double quotes.
                            if (CeldaExcel.IndexOf(separator, StringComparison.Ordinal) >= 0)
                                CeldaExcel = "\"" + CeldaExcel + "\"";

                            sw.Write(CeldaExcel);
                        }

                        if (index < structs.Count - 1)
                            sw.Write(separator);
                    }
                    sw.WriteLine();
                }
                sw.Flush();
            }
            finally
            {
                DRFilas.Close(); 
                
            }
        }


    }

}
