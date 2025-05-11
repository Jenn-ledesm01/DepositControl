using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DNF.Entity;

namespace DepositControl
{

    public static class Extended
    {
        public static string CamelToDescription(this string text)
        {
            var output = Regex.Replace(text, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1", RegexOptions.Compiled).Trim();
            return output;
        }

        public static string ToJoin(this IEnumerable<object> list, string separator = ",", int limit = 0, string cropWith = "...")
        {
            var enumerable = list as object[] ?? list.ToArray();
            if (list == null || !enumerable.Any()) return "";

            var joinstring = string.Join(separator, enumerable);

            if (limit != 0 && joinstring.Length > limit)
                joinstring = joinstring.Substring(0, limit) + cropWith;

            return joinstring;
        }

        public static List<Dictionary<string, object>> ToJsonSerializeList(this IDataReader dataReader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            try
            {
                for (var i = 0; i < dataReader.FieldCount; i++)
                    cols.Add(dataReader.GetName(i)); // recupero todas las columnas

                while (dataReader.Read())
                    results.Add(cols.ToDictionary(col => col, //nombre columna 
                        col => dataReader[col])); // valor
            }
            finally
            {
                dataReader.Close();
                dataReader.Dispose();
            }
            
            return results;

        }
        public static Array ToJsonSerializeArray(this IDataReader dataReader)
        {
            return ToJsonSerializeList(dataReader).ToArray();
        }

        public static Dictionary<string, string> ToListItemJqGrid<T>(this IEnumerable<T> listName, string defaultName = null, string defaultValue = "0")
            where T : IName, IEntity
        {
            var listFinal = new Dictionary<string, string>();

            if (defaultName != null)
            {
                listFinal.Add(defaultValue, defaultName);
            }

            var listNames = listName as T[] ?? listName.ToArray();
            
            if (!listNames.Any()) return listFinal;
            foreach (var key in listNames)
            {
                listFinal.Add(key.Id.ToString(), key.Name);
            }

            return listFinal;
        }

        public static string ToListItemJqGridString<T>(this IEnumerable<T> listName, string defaultName = null,
            string defaultValue = "0")
            where T : IName, IEntity
            => (defaultName != null ? $"{defaultValue}:{defaultName};" : "") +
               (listName.Any() && listName.First() is IOrder && listName.Any(x => ((IOrder)x).Order != 0)
                   ? listName.OrderBy(x => ((IOrder) x).Order).Select(x => x.Id.ToString() + ":" + x.Name).ToJoin(";")
                   : listName.OrderBy(x => x.Name).Select(x => x.Id.ToString() + ":" + x.Name).ToJoin(";"));

        public static string ToListItemCodeJqGridString<T>(this IEnumerable<T> listName, string defaultName = null, string defaultValue = "0")
            where T : IName, IEntity, ICode
            => (defaultName != null ? $"{defaultValue}:{defaultName};" : "") +
             (listName.Any() && listName.First() is IOrder && listName.Any(x => ((IOrder)x).Order != 0)
                   ? listName.OrderBy(x => ((IOrder) x).Order).Select(x => x.Code + ":" + x.Name).ToJoin(";")
                   : listName.OrderBy(x => x.Code).Select(x => x.Code + ":" + x.Name).ToJoin(";"));

        public static List<SelectListItem> ToListItem<T>(this IEnumerable<T> listName, string defaultName = null, string defaultValue = "0")
           where T : IName, IEntity
        {
            var listFinal = new List<SelectListItem>();

            if (defaultName != null)
                listFinal.Add(new SelectListItem()
                {
                    Selected = true,
                    Text = defaultName,
                    Value = defaultValue
                });

            var listNames = listName as T[] ?? listName.ToArray();

            if (!listNames.Any()) return listFinal;

            if (listNames.First() is IOrder && listNames.Any(x=> ((IOrder)x).Order !=0)) //que implemente i orden y que tenga valor en el campo
                listNames = listNames.OrderBy(x => ((IOrder) x).Order).ToArray();
            else
                listNames = listNames.OrderBy(x => x.Name).ToArray();

            if (listNames.First() is IIsDefault)
                listFinal.AddRange(listNames.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = ((IIsDefault)x).IsDefault
                }));
            else
                listFinal.AddRange(listNames.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }));

            return listFinal.ToList();
        }
        public static List<SelectListItem> ToListItemCode<T>(this IEnumerable<T> listName, string defaultName = null, string defaultValue = "0")
           where T : IName, ICode, IEntity
        {
            var listFinal = new List<SelectListItem>();

            if (defaultName != null)
                listFinal.Add(new SelectListItem()
                {
                    Selected = true,
                    Text = defaultName,
                    Value = defaultValue
                });


            var listNames = listName as T[] ?? listName.ToArray();

            if (!listNames.Any()) return listFinal;

            if (listNames.First() is IOrder && listNames.Any(x => ((IOrder)x).Order != 0)) //que implemente i orden y que tenga valor en el campo
                listNames = listNames.OrderBy(x => ((IOrder)x).Order).ToArray();
            else
                listNames = listNames.OrderBy(x => x.Name).ToArray();


            if (listNames.First() is IIsDefault)
                listFinal.AddRange(listNames.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Code.ToString(),
                    Selected = ((IIsDefault)x).IsDefault
                }));
            else
                listFinal.AddRange(listNames.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Code.ToString()
                }));

            return listFinal.OrderBy(x => x.Text).ToList();
        }

        public static IEnumerable<T[]> Chunks<T>(this IEnumerable<T> xs, int size)
        {
            return xs.Select((x, i) => new { x, i })
                     .GroupBy(xi => xi.i / size, xi => xi.x)
                     .Select(g => g.ToArray());
        }



    }
}


