using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Bcri.Core.Bussines;
using DepositControl.Areas.Bcri.Models;
using DNF.Entity;
using DNF.Structure.Bussines;

namespace DepositControl.Areas.Bcri.Utility
{
    public static class JqGridExtended
    {
        public static JqGrid ToJqGrid(Entity entity, GridModel gridModel)
        {

            return new JqGrid
            {
                datatype = "json",
                colModel = entity.Structs.ToJqGridColModels(),
                loadonce = !gridModel.ServerSide,
                pager = gridModel.GridId + "-Pager",

                sortable = true,
                sortname = "Id",
                sortorder = "ASC",
                ignoreCase = true,
                viewrecords = true,
                rownumbers = false,

                shrinkToFit = false,
                forceFit = false,
                width = 450,
                height = 350,
                rowNum = 20

            };


        }

        public static List<JqGridColModel> ToJqGridColModels(this List<Struct> structs)
        {
            var list = new List<JqGridColModel>();
            var dicEditType = new Dictionary<int, string> //agregar columna al datatype con el tipo para la grilla
                                   {
                                       {1, "text"},
                                       {2, "text"},
                                       {3, "date"},
                                       {4, "checkbox"},
                                   };

            structs = structs.Where(x => x.Name != "Repository_Id"
                                                               && x.Name != "ProcessConfig_Id"
                                                               && (x.InTable || x.InView)
                                                               ).OrderBy(x => x.ImportOrder)
                                                               .ToList();
            structs.LoadRelation(x => x.Entity);

            foreach (var it in structs)
            {
                var hasRelatedEntity = !string.IsNullOrEmpty(it.RefEntity);
                if (hasRelatedEntity && it.RefEntityClass == "File")
                    continue;

                string selectValues = null;
                string searchSelectValues = null;
                if (hasRelatedEntity)
                {

                    selectValues = GetSelectItemForRelated(it);
                    searchSelectValues = selectValues;
                    if (selectValues == null)
                        hasRelatedEntity = false;
                    else if (it.Nullable)
                        selectValues = selectValues.Insert(0, "_empty: ;");

                }




                var colModle = new JqGridColModel
                {
                    label = it.Description?.Trim() ?? it.Name.Trim().Split(char.Parse("_"))[0].CamelToDescription(),
                    name = it.Name.Trim(),
                    //width = it.Identity ? 50 : 100,
                    align = hasRelatedEntity ? "left" : GetAlignForStruct(it),
                    resizable = true,
                    search = true,

                    key = it.Identity,
                    hidden = it.PK,
                    editable = !it.PK && it.InTable && !(it.Editable.HasValue && !it.Editable.Value),
                    formatter = hasRelatedEntity ? "select" : GetFormmaterforStruc(it),
                    edittype = hasRelatedEntity ? "select" : dicEditType[it.DataType.Behavior],
                    stype = hasRelatedEntity || it.DataType.C == "bool" ? "select" : "text",
                    sorttype = GetSorttypeforStruc(it),
                    searchoptions = new
                    {
                        sopt = (hasRelatedEntity || (it.DataType.Behavior == 3)) ? new[] { "eq" } : new[] { "cn", "eq", "ne", "lt", "le", "gt", "ge", "bw", "bn", "in", "ni", "ew", "en", "nc" },
                        value = it.DataType.C == "bool" ? ": ;True:Yes;False:No"
                                : hasRelatedEntity ? ": ;" + searchSelectValues : null,
                        date = (it.DataType.Behavior == 3)
                    },

                    editoptions = new
                    {
                        maxlength = it.DataType.C == "string"
                                            ? it.Length
                                            : GetMaxNumericValue(it)?.Length, //lo manda pero al parecer la jqgrid no lo toma para los numeros
                        value = it.DataType.C == "bool" ? "True:False" //para parseo del servidor al guardar
                                : hasRelatedEntity ? selectValues : null

                    },
                    editrules = new
                    {
                        required = !it.Nullable,
                        number = (it.DataType.C == "decimal" || it.DataType.C == "double"),
                        integer = (!hasRelatedEntity &&
                                        (it.DataType.C == "int" || it.DataType.C == "long" || it.DataType.C == "Single")),
                        minValue = GetMinNumericValue(it),
                        maxValue = GetMaxNumericValue(it),
                        date = (it.DataType.Behavior == 3)
                    },

                    //@fixed = true 
                };
                if (hasRelatedEntity)
                {
                    colModle.formatoptions = new { value = selectValues, delimiter = ";", separator = ":" };
                }
                else
                {
                    colModle.formatoptions = new { decimalPlaces = it.LengthDecimal ?? 0 };
                }
                list.Add(colModle);
            }



            return list;
        }

        private static string GetMaxNumericValue(Struct stru)
        {
            switch (stru.DataType.C)
            {
                case "int":
                    return int.MaxValue.ToString();
                case "long":
                    return long.MaxValue.ToString();
                case "decimal":
                    if (stru.Length.HasValue && Convert.ToInt16(stru.LengthDecimal) == 0)
                        return new string(char.Parse("9"), stru.Length.Value);
                    if (stru.Length.HasValue && stru.LengthDecimal.HasValue)
                        return new string(char.Parse("9"), stru.Length.Value - stru.LengthDecimal.Value)
                                    + "."
                                    + new string(char.Parse("9"), stru.Length.Value - stru.LengthDecimal.Value);
                    break;
                case "Single":
                    return null;
                case "double":
                    return null;
            }
            return null;

        }

        private static string GetMinNumericValue(Struct stru)
        {
            switch (stru.DataType.C)
            {
                case "int":
                    return int.MinValue.ToString();
                case "long":
                    return long.MinValue.ToString();
                case "decimal":
                    if (stru.Length.HasValue && Convert.ToInt16(stru.LengthDecimal) == 0)
                        return "-" + new string(char.Parse("9"), stru.Length.Value);
                    if (stru.Length.HasValue && stru.LengthDecimal.HasValue)
                        return "-" + new string(char.Parse("9"), stru.Length.Value - stru.LengthDecimal.Value)
                                    + "."
                                    + new string(char.Parse("9"), stru.Length.Value - stru.LengthDecimal.Value);
                    break;
                case "Single":
                    return null;
                case "double":
                    return null;
            }
            return null;

        }

        private static string GetAlignForStruct(Struct stru)
        {
            switch (GetFormmaterforStruc(stru))
            {
                case "text":
                    return "left";
                case "date":
                    return "left";
                case "checkbox":
                    return "center";
                case "number":
                    return "right";
                case "integer":
                    return "right";

            }
            return "left";

        }
        private static string GetFormmaterforStruc(Struct stru)
        {
            if (stru.DataType.Behavior == 1) return "text";
            if (stru.DataType.Behavior == 3) return "date";
            if (stru.DataType.Behavior == 4) return "checkbox";
            if ((stru.DataType.C == "decimal" || stru.DataType.C == "double") && Convert.ToInt16(stru.LengthDecimal) > 0)
                return "number";
            if (stru.DataType.C == "int" || stru.DataType.C == "long"
             || stru.DataType.C == "Single" || stru.DataType.C == "byte"
             || stru.DataType.C == "decimal" || stru.DataType.C == "double")
                return "integer";

            return "text";

        }

        private static string GetSorttypeforStruc(Struct stru)
        {
            if (stru.DataType.Behavior == 1) return "text";

            if (stru.DataType.Behavior == 3) return "date";

            if ((stru.DataType.C == "decimal" || stru.DataType.C == "double") && Convert.ToInt16(stru.LengthDecimal) > 0)
                return "number";

            if (stru.DataType.C == "int" || stru.DataType.C == "long"
                || stru.DataType.C == "Single" || stru.DataType.C == "byte"
                || stru.DataType.C == "decimal" || stru.DataType.C == "double")
                return "integer";

            return "text";
        }

        public static Dictionary<string, string> GetDicNameByRelated(this Struct stru, out bool hasCustomSp)
        {
            hasCustomSp = false;
            var entityName = stru.RefEntity;

            if (string.IsNullOrEmpty(entityName) || Entity.Dao.GetByName(entityName) == null) //si no tiene uan entity creada no puede acceder al reader 
                return null;

            long? repoCurrentId = RepositoryConfig.Dao.GetByCode(entityName) //repo config
                                                            ?.Current //repo current instance
                                                            ?.Id;

            var reader = new DbReader(entityName, repoCurrentId);
            //permite definir en el sp como llenar el combo 
            var hasSelectItemSp = reader.CheckSpExist("GetSelectItem");


            // se le puede pasar Entity y Struc al sp, para implementar una logica personalizada sobre como llenar el combo para cada campo
            // cuando tiene esta logica personalizada no se guarda en cache hasCustomSp = true
            if (hasSelectItemSp)
                hasCustomSp = reader.GetParamsFromSp("GetSelectItem").ContainsKey("Entity");

            Dictionary<string, string> resul = null;

            //instancia actual y traigo todos los datos a un redear
            DbDataReader dataReader = hasSelectItemSp
                                    ? reader.GetFromSP("GetSelectItem", new { Entity = stru.Entity.Name, Struct = stru.Name }) //los paraetros son opionales
                                    : reader.GetAll();


            //busco columna nombre/codigo para mostrar como descripcion en el combo 
            int nameColum = -1;
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (dataReader.GetName(i).ToLower() == "name")
                {
                    nameColum = i;
                    break;
                }
                if (dataReader.GetName(i).ToLower() == "code")
                    nameColum = i;
            }

            //si no encontre columna descriptiva salgo 
            if (nameColum == -1 || !dataReader.HasRows)
            {
                dataReader.Close();
                return null;
            }

            //meto todos los id y nombre en un diccionario 
            var idcolum = dataReader.GetOrdinal("Id");

            resul = new Dictionary<string, string>();
            while (dataReader.Read())
            {
                var value = dataReader[nameColum].ToString()
                    .Replace(";", ",")
                    .Replace(":", ",")
                    .Replace("\"", "'"); //elimino esos caracteres por que mando los items como un solo texto formateado
                resul.Add(dataReader[idcolum].ToString(), value);
            }
            dataReader.Close();

            return resul;
        }


        private static Dictionary<string, string> _cacheSelectItem = new Dictionary<string, string>();
        private static string GetSelectItemForRelated(Struct stru)
        {
            var entityName = stru.RefEntity;


            //if (_cacheSelectItem.ContainsKey(entityName))
            //    return _cacheSelectItem[entityName];
            bool hasCustomSp;

            var resul = stru.GetDicNameByRelated(out hasCustomSp);

            var resulStrign = resul?.OrderBy(x => x.Value).Select(x => $"{x.Key}:{x.Value}").ToJoin(";");
            // resultado final ejemplo: "1:FedEx; 2:InTime; 3:TNT" 

            // al tener una logica personalizada no se guarda en el cache
            //if (!hasCustomSp)
            //    _cacheSelectItem.Add(entityName, resulStrign);


            return resulStrign;
        }

    }
}
