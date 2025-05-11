using Bcri.Core.Bussines;
using DNF.Security.Bussines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DepositControl.Areas.Bcri.Utility
{
    public static class ProcessUtility
    {

         public static String AddValidateProcessPresented(string processCode, DateTime Period)
        {
            //1. Buscar configuración de Proceso
            var proccessConfig = ProcessConfig.Dao.GetByCode(processCode);
            //2. Buscar si existe instancia de Proceso para el período indicado
            var process = Process.Dao.GetByFilter(new { ProcessConfig_Id = proccessConfig.Id, Period = Period }).FirstOrDefault();

            try
            {
                //2.1. Si existe si estado !Presentado
                if ((process != null) && (process.Status.Code != "Presented"))
                {

                    //2.1.1. Eliminar Instancia
                    process.Delete();
                    LogAccion.Dao.AddLog("ProcessInstanceDelete"
                   , process.Config.Name + " " + process.CodeId + ";#;"
                   , null
                   , process.Config.Name + process.CodeId, process.Config.Id);

                }

                if (((process != null) && (process.Status.Code != "Presented")) || (process == null))
                {
                    //2.1.2. Crear Instancia y ejecutar
                    process = proccessConfig.Create(Period);
                    LogAccion.Dao.AddLog("ProcessInstanceNew"
                     , process.Config.Name + " " + process.CodeId + ";#;"
                     , null
                     , process.Config.Name + process.CodeId, process.Config.Id);

                    var task = Task.Run(() => process.Run(false));
                    while (task.Status != TaskStatus.RanToCompletion) ;
                }
               
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
}

                   
