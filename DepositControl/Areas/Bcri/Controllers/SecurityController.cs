using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DNF.Entity;
using DNF.Security.Bussines;
using DNF.Structure.Bussines;
using Type = DNF.Type.Bussines.Type;
using Profile = DNF.Security.Bussines.Profile;
using Struct = DNF.Structure.Bussines.Struct;
using System.Globalization;
using DepositControl.Areas.Bcri.Models;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using PdfText = iTextSharp.text;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using System.IO;
using System.Configuration;


namespace DepositControl.Areas.Bcri.Controllers
{
    [Authenticated]
    public class SecurityController : Controller
    {
        public void Navigation(int id)
        {
            var access = Access.Dao.Get(id);

            if (Current.User == null)
                RedirectToAction("Login", "Login");

            if (Current.User != null && (access != null && Current.User.HasAccess(access.Code)))
            {
                Current.Access = access;
                var acceso = SecurityUtility.GetParentAccess(access);

                LogAccion.Dao.AddLog("Navigation"
                    //el usuario X a navegado a Y
                    , Current.User.Name + ";#;" + acceso
                    );
                Response.Redirect(access.Url);
            }
        }

        [HttpGet]
        public ActionResult HasAccess(string code = "")
            => Json(Current.User.HasAccess(code), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetAllAccess()
            => Json(Current.User.Accesses.Select(x => new {
                x.Id,
                x.Code,
                x.Name,
                x.Url,
                Type = x.Type.Code,
                x.Icon,
                x.Data,
                x.Description,
                x.Posicion,
                Parent = x.Parent?.Id
            }).ToArray(), JsonRequestBehavior.AllowGet);

        [AccessCode("Users")]
        public ActionResult Users()
        {
            ViewBag.Title = "Users";
            ViewBag.Edit = Current.User.HasAccess("UserEdit");
            ViewBag.New = Current.User.HasAccess("UserDelete");
            ViewBag.Delete = Current.User.HasAccess("UserNew");
            ViewBag.HabilitadoBotonExportXls = Current.User.HasAccess("UsersExportExcel");
            ViewBag.HabilitadoBotonExportPdf = Current.User.HasAccess("UsersExportPDF");
            ViewBag.HabilitadoBotonExportTxt = Current.User.HasAccess("UsersExportTXT");

            ViewBag.Profiles = DNF.Security.Bussines.Profile.Dao.GetAll().Where(x => x.State.Code != "Delete").ToListItem();

            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            LogAccion.Dao.AddLog("SecurityAccess"
                //el usuario X a navegado a Y
                , Current.User.Name + ";#;" + acceso
                );

            return View();
        }
        [AccessCode("Users")]
        [Authenticated]
        public ActionResult UsersData()
        {
            var users = DNF.Security.Bussines.User.Dao.GetAll();
            users.LoadRelationList(x => x.UserProfiles);
            users.SelectMany(x => x.UserProfiles).LoadRelation(x => x.Profile);
            users.ForEach(x => x.Profiles = x.UserProfiles.Select(up => up.Profile).ToList());
            var usersData = users.Select(x => new
            {
                x.Id,
                x.FullName,
                x.Name,
                x.Email,
                State_Code = x.State.Name,
                LastLogin = x.LastLogin?.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                Password = "",
                Profiles = x.Profiles.Where(p => p.State.Code != "Delete").Select(p => p.Name).ToJoin(", "),
                ProfilesIds = x.Profiles.Select(p => p.Id.ToString()).ToJoin()
            });
            return Json(usersData, JsonRequestBehavior.AllowGet);
        }

        [AccessCode("UserEdit")]
        [Authenticated]
        public ActionResult UserEdit(User user, string oper)
        {

            if (oper == "del")
            {
                var finalUser = DNF.Security.Bussines.User.Dao.Get(user.Id);
                if (finalUser.State.Name != @Res.res.deleted)
                {
                    finalUser.State = new Type("UserState", "Delete");
                    var origUser = DNF.Security.Bussines.User.Dao.Get(user.Id);
                    LogAccion.Dao.AddLog("UserDelete"
                        , finalUser.Name + ";#;" + finalUser.State.Name.ToLower()
                        , DynamicDataSet.Dao.CreateChangeSet(origUser, finalUser)
                        , finalUser.Name, finalUser.Id);
                    finalUser.Save();
                }
            }
            else
            {
                var finalUser = user.IsNew() ? new User() : DNF.Security.Bussines.User.Dao.Get(user.Id);

                if (finalUser.IsNew()) finalUser.Company = new Company { Id = 0 };

                var origUser = new User();
                finalUser.CopyTo(origUser);
                // Datos de Formulario de Usuario
                var listEditedProfiles = DNF.Security.Bussines.Profile.Dao.Get(
                Request.Form["Profiles"] //"1,2,3"
               .Split(char.Parse(","))
               .Where(x => x != "") // ["1","2","3"]
               .Select(long.Parse) //[1, 2, 3]
               .ToList()); //traigo todos los perfiles elegidos de la lista de checks 
                var listOriginalProfiles = finalUser.UserProfiles.Select(x => x.Profile).ToList();

                finalUser.Name = user.Name;
                finalUser.FullName = user.FullName;
                finalUser.Email = user.Email;

                if (!string.IsNullOrEmpty(user.Password))
                    finalUser.Password = user.Password.Trim();

                finalUser.State = new Type("UserState", Request.Form["State_Code"]);

                if (finalUser.IsNew())
                {
                    LogAccion.Dao.AddLog("UserNew"
                    //, "The user " + user.Name + " has been created."
                    , user.Name
                    , DynamicDataSet.Dao.CreateChangeSet(null, finalUser)
                    , user.Name, user.Id);
                }
                else
                {
                    if (origUser.Name != finalUser.Name || origUser.FullName != finalUser.FullName
                        || origUser.Email != finalUser.Email)
                    {
                        LogAccion.Dao.AddLog("UserEdit"
                        //, "The user " + user.Name + " has been changed."
                        , user.Name
                        , DynamicDataSet.Dao.CreateChangeSet(origUser, finalUser)
                        , user.Name, user.Id);
                    }

                    if (finalUser.State.Name != origUser.State.Name)
                    {
                        finalUser.GetEntity();
                        LogAccion.Dao.AddLog("UserChangeState"
                        //, "The user " + user.Name + " has changed the state to " + finalUser.State.Name.ToLower() + "."
                        , user.Name + ";#;" + finalUser.State.Name.ToLower()
                        , DynamicDataSet.Dao.CreateChangeSet(origUser, finalUser)
                        , user.Name, user.Id);
                    }
                }

                finalUser.Save();

                var listaSeleccionados = listEditedProfiles.Select(x => x.Id).ToList();
                var listaOriginales = listOriginalProfiles.Select(x => x.Id).ToList();
                var interseccion = listaSeleccionados.Intersect(listaOriginales).Count();
                var ProfilesChanged = interseccion != listaOriginales.Count || interseccion != listaSeleccionados.Count;

                if (ProfilesChanged)
                {
                    finalUser.UserProfiles.Delete();
                    finalUser.UserProfiles =
                            listEditedProfiles.Select(p => new UserProfile
                            {
                                Profile = p,
                                User = finalUser
                            }).ToList();
                    finalUser.UserProfiles.Save();

                    if (listEditedProfiles.Any())
                    {

                        LogAccion.Dao.AddLog("UserProfilesEdit"
                            //, "The profiles for user " + finalUser.Name + " has been changed."
                            , finalUser.Name
                            , DynamicDataSet.Dao.CreateChangeSet(listOriginalProfiles, listEditedProfiles)
                            , user.Name, user.Id);
                    }


                }
            }
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);


        }

        
        private List<string> UserGetColNames()
        {
            List<string> colNames = new List<string>();

            colNames.Add(@Res.res.user);
            colNames.Add(@Res.res.fullname);
            colNames.Add(@Res.res.mail);
            colNames.Add(@Res.res.state);
            colNames.Add(@Res.res.lastLogin);
            colNames.Add(@Res.res.profiles);
            return colNames;
        }

        private dynamic UserloadData()
        {
            var userProfile = new List<UserProfile>();
            userProfile = UserProfile.Dao.GetAll();
            userProfile.LoadRelation(s => s.User);
            userProfile.LoadRelation(s => s.Profile);
            return new
            {
                rows = userProfile
            };
        }
        public ActionResult exportExcel(string tittle, string fromDate, string toDate, string act01, string all)
        {

            int cantidadColumnas = 0;
            dynamic dataHeader = "";
            string codeLogAccion = "";
            dynamic colNames = "";
            switch (tittle)
            {
                case "Usuarios":
                    cantidadColumnas = UserGetColNames().Count;
                    dataHeader = DNF.Security.Bussines.User.Dao.GetAll();
                    codeLogAccion = "UsersExportExcel";
                    colNames = UserGetColNames();
                    break;
                case "Perfiles":
                    cantidadColumnas = ProfilesGetColNames().Count;
                    dataHeader = DNF.Security.Bussines.Profile.Dao.GetAll().Where(z => z.State.Code != "Delete").ToList();
                    codeLogAccion = "ProfilesExportExcel";
                    colNames = ProfilesGetColNames();
                    break;
                case "Auditoria":
                    dataHeader = AuditLoadData(fromDate, toDate, act01);
                    codeLogAccion = "AuditExportExcel";
                    colNames = AuditGetColNames();
                    cantidadColumnas = colNames.Count;
                    break;
            }


            var workbook = new XSSFWorkbook();
            var excelUtil = new ExcelUtility(workbook);
            var fechaConsulta = DateTime.Today;
            var colFinTitulo = 7;
            var sheetHeader = excelUtil.CreateSheet($"{Convert.ToString(ConfigurationManager.AppSettings["Aplication"])} - {tittle}", fechaConsulta, colFinTitulo);

            sheetHeader.SetColumnWidth(0, (30 * 256) + 5);
            for (int i = 0; i < cantidadColumnas - 3; i++)
            {
                sheetHeader.SetColumnWidth(i, (25 * 256) + 5);
            }
            sheetHeader.SetColumnWidth(cantidadColumnas - 3, (25 * 256) + 5);
            sheetHeader.SetColumnWidth(cantidadColumnas - 2, (25 * 256) + 5);
            sheetHeader.SetColumnWidth(cantidadColumnas - 1, (75 * 256) + 5);


            int rowNumberMain = 1;

            //TITULOS
            switch (tittle)
            {
                case "Auditoria":



                    int numRowMainDateFrom = rowNumberMain++;
                    var rowMainDateFrom = sheetHeader.CreateRow(numRowMainDateFrom);
                    var cellMainDateFrom = rowMainDateFrom.CreateCell(0);
                    cellMainDateFrom.SetCellValue($"{Res.res.dateFrom}: {fromDate}");
                    var craDateFrom = new NPOI.SS.Util.CellRangeAddress(numRowMainDateFrom, numRowMainDateFrom, 0, 4);
                    sheetHeader.AddMergedRegion(craDateFrom);

                    int numRowMainDateTo = rowNumberMain++;
                    var rowMainDateTo = sheetHeader.CreateRow(numRowMainDateTo);
                    var cellMainDateTo = rowMainDateTo.CreateCell(0);
                    cellMainDateTo.SetCellValue($"{Res.res.dateTo}: {toDate}");
                    var craDateTo = new NPOI.SS.Util.CellRangeAddress(numRowMainDateTo, numRowMainDateTo, 0, 4);
                    sheetHeader.AddMergedRegion(craDateTo);

                    int numRowMainAction = rowNumberMain++;
                    var rowMainAction = sheetHeader.CreateRow(numRowMainAction);
                    var cellMainAction = rowMainAction.CreateCell(0);
                    if (all != "0")
                    {
                        cellMainAction.SetCellValue($"{Res.res.selectedactions}: {Res.res.selectAllText}");
                    }
                    else
                    {
                        act01 = act01.Remove(act01.Length - 1);

                        var actions = Accion.Dao.GetByFilter(new
                        {
                            Accion_Ids = act01
                        }).ToList();
                        var actionsTitle = actions.Select(i => i.Name)
                              .ToArray();
                        cellMainAction.SetCellValue($"{Res.res.selectedactions}:  {string.Join(", ", actionsTitle)}");

                    }



                    var craAction = new NPOI.SS.Util.CellRangeAddress(numRowMainAction, numRowMainAction, 0, 4);
                    sheetHeader.AddMergedRegion(craAction);
                    sheetHeader.CreateFreezePane(0, 5);
                    break;
                default:
                    sheetHeader.CreateFreezePane(0, 3);
                    rowNumberMain = 2;
                    break;
            }



            int k = 0;
            var rowTitulos = sheetHeader.CreateRow(rowNumberMain++);
            foreach (var item in colNames)
            {
                excelUtil.CreateCell(rowTitulos, k++, item, ExcelTypes.Text, ExcelStyles.HeaderC);

            }

            foreach (var item in dataHeader)
            {
                var rowMain = sheetHeader.CreateRow(rowNumberMain++);
                switch (tittle)
                {
                    case "Auditoria":
                        excelUtil.CreateCell(rowMain, 0, Convert.ToString(item.Date.ToString("dd/MM/yyyy HH:mm:ss")), ExcelTypes.Text, ExcelStyles.RowC);
                        if (item.Accion.Code == "UserDoesNotExist")
                        {
                            excelUtil.CreateCell(rowMain, 1, "", ExcelTypes.Text, ExcelStyles.RowL);
                        }
                        else
                        {
                            excelUtil.CreateCell(rowMain, 1, item.User.Name, ExcelTypes.Text, ExcelStyles.RowL);

                        }

                        excelUtil.CreateCell(rowMain, 2, item.Accion.Name, ExcelTypes.Text, ExcelStyles.RowL);
                        if (string.IsNullOrEmpty(item.Data) && string.IsNullOrEmpty(item.Accion.Message))
                        {
                            excelUtil.CreateCell(rowMain, 3, "", ExcelTypes.Text, ExcelStyles.RowL); ;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.Accion.Message))
                            {
                                var mensaje = item.Accion.Message;
                                var regex = new Regex(Regex.Escape(";#;"));
                                foreach (var varReemplazo in Regex.Split(item.Data ?? "", ";#;"))
                                {
                                    mensaje = regex.Replace(mensaje, varReemplazo, 1);
                                }
                                item.Data = mensaje;
                            }
                            excelUtil.CreateCell(rowMain, 3, item.Data.ToString(), ExcelTypes.Text, ExcelStyles.RowL);

                        }
                        if (item.DynamicDataSet != null && item.DynamicDataSet.DynamicDataSetRows.Count > 0)
                        {
                            var oldValue = "";
                            var value = "";
                            var lst = new List<VDetailAccion>();

                            var val = new string[] { };
                            var old = new string[] { };

                            Accion a = Accion.Dao.Get(item.Accion.Id);
                            if (a != null)
                            {
                                ViewData["Audit.DetailAccion.AccionLogAccion"] = a.Name;
                                switch (a.Code)
                                {

                                    case "UserProfilesEdit":
                                        value = Res.res.addedprofiles;
                                        oldValue = Res.res.deletedprofiles;

                                        var dataSet = DynamicDataSet.Dao.Get(item.DynamicDataSet.Id);
                                        var profileEdit = GetProfiles(dataSet.DynamicDataSetRows);
                                        if (profileEdit.Count > 1)
                                        {
                                            VDetailAccion itvDetailAccionPEdit = new VDetailAccion
                                            {
                                                StructDescription = @Res.res.associatedprofiles,
                                                OldValue = profileEdit[1],
                                                Value = profileEdit[0]
                                            };
                                            lst.Add(itvDetailAccionPEdit);
                                        }
                                        val = lst.Select(i => i.StructDescription + ": " + i.Value + " | " + oldValue + ": " + i.OldValue)
                                             .ToArray();
                                        excelUtil.CreateCell(rowMain, 4, string.Join(" ", val), ExcelTypes.Text, ExcelStyles.RowL);
                                        break;

                                    case "ProfileAccessEdit":
                                        var objDetail = new List<VDetailAccion>();
                                        //objDetail = GetLogAccionDetail(item);
                                        value = Res.res.addedaccess;
                                        oldValue = Res.res.deletedaccess;

                                        var dataSetPAE = DynamicDataSet.Dao.Get(item.DynamicDataSet.Id);
                                        var treeEdit = GetAccessComplete(dataSetPAE.DynamicDataSetRows);
                                        VDetailAccion itvDetailAccionAEdit = new VDetailAccion
                                        {
                                            StructDescription = @Res.res.associatedaccess,
                                            OldValue = treeEdit[1],
                                            Value = treeEdit[0]
                                        };
                                        // Eliminados
                                        // Añadidos
                                        objDetail.Add(itvDetailAccionAEdit);
                                        if (item.AccionToId != null)
                                        {
                                            val = objDetail.Select(i => $"{value}:{Environment.NewLine}{i.Value}{Environment.NewLine}{oldValue}:{Environment.NewLine}{i.OldValue}")
                                              .ToArray();

                                        }
                                        excelUtil.CreateCell(rowMain, 4, string.Join(" ", val), ExcelTypes.Text, ExcelStyles.RowL);
                                        break;
                                    default:
                                        lst = GetLogAccionDetalle(item);
                                        value = Res.res.actualvalue;
                                        oldValue = Res.res.previousvalue;
                                        val = lst.Select(i => $"- {i.StructDescription}: {i.Value} | {oldValue}: {i.OldValue}" + Environment.NewLine)
                                              .ToArray();
                                        excelUtil.CreateCell(rowMain, 4, string.Join("", val), ExcelTypes.Text, ExcelStyles.RowL);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            excelUtil.CreateCell(rowMain, 4, "", ExcelTypes.Text, ExcelStyles.RowL);
                        }
                        break;
                    case "Usuarios":
                        excelUtil.CreateCell(rowMain, 0, item.Name, ExcelTypes.Text, ExcelStyles.RowC);
                        excelUtil.CreateCell(rowMain, 1, item.FullName, ExcelTypes.Text, ExcelStyles.RowL);
                        excelUtil.CreateCell(rowMain, 2, item.Email, ExcelTypes.Text, ExcelStyles.RowL);
                        excelUtil.CreateCell(rowMain, 3, item.State.Name, ExcelTypes.Text, ExcelStyles.RowL);
                        excelUtil.CreateCell(rowMain, 4, Convert.ToString(item.LastLogin), ExcelTypes.Text, ExcelStyles.RowL);
                        var profiles = DNF.Security.Bussines.UserProfile.Dao
                           .GetBy(new User { Id = item.Id });
                        profiles.LoadRelation(x => x.Profile);
                        excelUtil.CreateCell(rowMain, 5, profiles.Select(x => x.Profile.Name).ToJoin(","), ExcelTypes.Text, ExcelStyles.RowL);
                        break;
                    case "Perfiles":
                        var rowInicial = rowMain.RowNum;
                        excelUtil.CreateCell(rowMain, 0, item.Name, ExcelTypes.Text, ExcelStyles.RowC);
                        excelUtil.CreateCell(rowMain, 1, item.State.Name, ExcelTypes.Text, ExcelStyles.RowL);
                        var access = DNF.Security.Bussines.ProfileAccess.Dao
                          .GetBy(new Profile { Id = item.Id });
                        access.LoadRelation(x => x.Access);
                        foreach (ProfileAccess ac in access)
                        {
                            var accessObject = new Access();
                            if (ac.Access.Name == null)
                            {
                                accessObject = Access.Dao.Get(ac.Access.Id);
                                ac.Access.Name = accessObject.Name;
                            }
                        }
                        var count = 0;
                        foreach (ProfileAccess pa in access)
                        {
                            if (count == 0)
                            {
                                excelUtil.CreateCell(rowMain, 2, GetAccessTreeNavigation(pa.Access), ExcelTypes.Text, ExcelStyles.RowL);

                            }
                            else
                            {
                                rowMain = sheetHeader.CreateRow(rowNumberMain++);

                                excelUtil.CreateCell(rowMain, 0, "", ExcelTypes.Text, ExcelStyles.RowC);
                                excelUtil.CreateCell(rowMain, 1, "", ExcelTypes.Text, ExcelStyles.RowL);
                                excelUtil.CreateCell(rowMain, 2, GetAccessTreeNavigation(pa.Access), ExcelTypes.Text, ExcelStyles.RowL);
                            }
                            count++;
                        }
                        if (count == 0)
                        {
                            excelUtil.CreateCell(rowMain, 2, "", ExcelTypes.Text, ExcelStyles.RowL);
                        }
                        var cellRange1 = new CellRangeAddress(rowInicial, rowMain.RowNum, 0, 0);
                        sheetHeader.AddMergedRegion(cellRange1);

                        var cellRange2 = new CellRangeAddress(rowInicial, rowMain.RowNum, 1, 1);
                        sheetHeader.AddMergedRegion(cellRange2);


                        break;

                }
            }
            MemoryStream output = new MemoryStream();
            workbook.Write(output);
            LogAccion.Dao.AddLog(codeLogAccion, Current.User.Name, null);
            // BPAIR_Usuarios_mmolina271020170935
            return this.File(output.ToArray(), "application/vnd.ms-excel", Convert.ToString(ConfigurationManager.AppSettings["Aplication"]) + "_" + tittle + "_" + Current.User.Name + " " + DateTime.Now.ToString("yyyy-MM-dd") + "-" + DateTime.Now.ToString("HH.mm") + ".xlsx");
        }

        public ActionResult exportPDF(string tittle, string fromDate, string toDate, string act01, string all)
        {


            int cantidadColumnas = 0;
            dynamic dataHeader = "";
            string codeLogAccion = "";
            dynamic nombreColumnas = "";
            switch (tittle)
            {
                case "Usuarios":
                    cantidadColumnas = UserGetColNames().Count;
                    dataHeader = DNF.Security.Bussines.User.Dao.GetAll();
                    codeLogAccion = "UsersExportPDF";
                    nombreColumnas = UserGetColNames();
                    break;
                case "Perfiles":
                    cantidadColumnas = ProfilesGetColNames().Count;
                    dataHeader = DNF.Security.Bussines.Profile.Dao.GetAll().Where(z => z.State.Code != "Delete").ToList();
                    codeLogAccion = "ProfilesExportPDF";
                    nombreColumnas = ProfilesGetColNames();
                    break;
                case "Auditoria":
                    dataHeader = AuditLoadData(fromDate, toDate, act01);
                    codeLogAccion = "AuditExportPDF";
                    nombreColumnas = AuditGetColNames();
                    cantidadColumnas = nombreColumnas.Count;
                    break;
            }


            var document = new PdfText.Document(PdfText.PageSize.A4.Rotate(), 40, 40, 70, 55);
            var output = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, output);
            PdfUtility.HeaderFooterReportes pageEventHandler = new PdfUtility.HeaderFooterReportes();
            writer.PageEvent = pageEventHandler;
            pageEventHandler.UserName = Current.User.FullName;
            pageEventHandler.ImageUrl = this.Request.MapPath("~/Content/Images/logo-cliente-top.png");
            pageEventHandler.TitlePage = Convert.ToString(ConfigurationManager.AppSettings["Aplication"]) + " - " + tittle;

            document.Open();

            PdfText.Font brown = PdfText.FontFactory.GetFont("arial", 20f, new PdfText.BaseColor(163, 21, 21));
            PdfText.Font negroArial = PdfText.FontFactory.GetFont("arial", 16f, new PdfText.BaseColor(0, 0, 0));
            PdfText.Font negroArial2 = PdfText.FontFactory.GetFont("arial", 10f, new PdfText.BaseColor(0, 0, 0));
            PdfText.Font negroArial3 = PdfText.FontFactory.GetFont("arial", 8f, new PdfText.BaseColor(0, 0, 0));
            PdfText.Font georgia2 = PdfText.FontFactory.GetFont("arial", 9f, PdfText.BaseColor.WHITE);
            PdfText.Font georgia3 = PdfText.FontFactory.GetFont("arial", 9f, PdfText.BaseColor.BLACK);
            PdfText.Font georgia3BoldFont = PdfText.FontFactory.GetFont("arial", 9f, 1, PdfText.BaseColor.BLACK);
            PdfText.Font georgia4 = PdfText.FontFactory.GetFont("arial", 10f, PdfText.BaseColor.BLACK);
            PdfText.Font georgia5 = PdfText.FontFactory.GetFont("arial", 10f, 1, PdfText.BaseColor.BLACK);
            Font arialFont = FontFactory.GetFont("arial", 9f, BaseColor.BLACK);
            Font arialBoldFont = FontFactory.GetFont("arial", 10f, 1, BaseColor.BLACK);


            var dt = DateTime.Today;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-AR", true);
            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string[] newNames = { "Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado" };
            ci.DateTimeFormat.DayNames = newNames;
            string[] newMonths = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre", "" };
            ci.DateTimeFormat.MonthGenitiveNames = newMonths;
            var fechaConsulta = DateTime.Today;

            //TITULOS
            switch (tittle)
            {
                case "Auditoria":


                    PdfText.Phrase phraseTitleA = new PdfText.Phrase(tittle + Environment.NewLine, negroArial);
                    //PdfText.Phrase phraseTitleB = new PdfText.Phrase($"{Res.res.selecteddate}: {Res.res.dateFrom} {fromDate} / {Res.res.dateTo} {toDate}" + Environment.NewLine, negroArial2);
                    //PdfText.Phrase phraseTitleC = new PdfText.Phrase($"{Res.res.selectedactions}:  {string.Join(", ", actionsTitle)}", negroArial3);
                    PdfText.Phrase phraseTitleB1 = new PdfText.Phrase($"{Res.res.dateFrom}: {fromDate}" + Environment.NewLine, negroArial3);
                    PdfText.Phrase phraseTitleB2 = new PdfText.Phrase($"{Res.res.dateTo}: {toDate}" + Environment.NewLine, negroArial3);
                    PdfText.Paragraph paragraphTitleA = new PdfText.Paragraph { Alignment = 0 }; //0=Left, 1=Centre, 2=Right  
                    paragraphTitleA.Add(phraseTitleA);
                    PdfText.Paragraph paragraphTitleB1 = new PdfText.Paragraph { Alignment = 0 }; //0=Left, 1=Centre, 2=Right  
                    paragraphTitleA.Add(phraseTitleB1);
                    PdfText.Paragraph paragraphTitleB2 = new PdfText.Paragraph { Alignment = 0 }; //0=Left, 1=Centre, 2=Right  
                    paragraphTitleA.Add(phraseTitleB2);
                    PdfText.Paragraph paragraphTitleC = new PdfText.Paragraph { Alignment = 0 }; //0=Left, 1=Centre, 2=Right 
                    if (all != "0")
                    {
                        PdfText.Phrase phraseTitleC = new PdfText.Phrase($"{Res.res.selectedactions}:{Res.res.selectAllText}", negroArial3);
                        paragraphTitleA.Add(phraseTitleC);
                    }
                    else
                    {
                        act01 = act01.Remove(act01.Length - 1);

                        var actions = Accion.Dao.GetByFilter(new
                        {
                            Accion_Ids = act01
                        }).ToList();
                        var actionsTitle = actions.Select(i => i.Name)
                              .ToArray();
                        PdfText.Phrase phraseTitleC = new PdfText.Phrase($"{Res.res.selectedactions}:  {string.Join(", ", actionsTitle)}", negroArial3);
                        paragraphTitleA.Add(phraseTitleC);
                    }


                    document.Add(paragraphTitleA);
                    document.Add(paragraphTitleB1);
                    document.Add(paragraphTitleB2);
                    document.Add(paragraphTitleC);
                    break;
                default:
                    PdfText.Phrase phraseTitleD = new PdfText.Phrase(tittle, negroArial);
                    PdfText.Paragraph paragraphTitleD = new PdfText.Paragraph { Alignment = 0 }; //0=Left, 1=Centre, 2=Right  
                    paragraphTitleD.Add(phraseTitleD);
                    document.Add(paragraphTitleD);
                    break;
            }


            PdfPTable dataTableGrid = new PdfPTable(cantidadColumnas)
            {
                SpacingAfter = 10f,
                SpacingBefore = 10f,
                TotalWidth = 800f,
                LockedWidth = true,
                HeaderRows = 0,
                DefaultCell = { Padding = 5 },

            };

            var anchoColumnas1 = new List<float>();
            for (int i = 0; i < cantidadColumnas - 1; i++)
            {
                anchoColumnas1.Add(8f);
            }
            anchoColumnas1.Add(24f);
            dataTableGrid.SetWidths(anchoColumnas1.ToArray());
            foreach (var item in nombreColumnas)
            {
                PdfPCell pdfTableDatosClienteCell2 = new PdfPCell(new PdfText.Phrase(item, georgia2))
                {
                    HorizontalAlignment = PdfText.Element.ALIGN_CENTER,
                    VerticalAlignment = PdfText.Element.ALIGN_MIDDLE,
                    BackgroundColor = new PdfText.BaseColor(27, 78, 120),
                    Padding = 5,
                    Rowspan = 2
                };
                dataTableGrid.AddCell(pdfTableDatosClienteCell2);

            }

            foreach (var item in dataHeader)
            {


                switch (tittle)
                {
                    case "Auditoria":
                        PdfPCell celA1 = new PdfPCell(new Phrase(Convert.ToString(item.Date.ToString("dd/MM/yyyy HH:mm:ss")), arialFont));
                        celA1.HorizontalAlignment = Element.ALIGN_CENTER;
                        celA1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celA1.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celA1);

                        if (item.Accion.Code == "UserDoesNotExist")
                        {
                            PdfPCell celA2 = new PdfPCell(new Phrase("", arialFont));
                            celA2.HorizontalAlignment = Element.ALIGN_LEFT;
                            celA2.VerticalAlignment = Element.ALIGN_MIDDLE;
                            celA2.MinimumHeight = 13f;
                            dataTableGrid.AddCell(celA2);

                        }
                        else
                        {
                            PdfPCell celA2 = new PdfPCell(new Phrase(item.User.Name, arialFont));
                            celA2.HorizontalAlignment = Element.ALIGN_LEFT;
                            celA2.VerticalAlignment = Element.ALIGN_MIDDLE;
                            celA2.MinimumHeight = 13f;
                            dataTableGrid.AddCell(celA2);

                        }


                        PdfPCell celA3 = new PdfPCell(new Phrase(item.Accion.Name, arialFont));
                        celA3.HorizontalAlignment = Element.ALIGN_LEFT;
                        celA3.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celA3.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celA3);


                        if (string.IsNullOrEmpty(item.Data) && string.IsNullOrEmpty(item.Accion.Message))
                        {
                            PdfPCell celA4 = new PdfPCell(new Phrase("", arialFont));
                            celA4.HorizontalAlignment = Element.ALIGN_LEFT;
                            celA4.VerticalAlignment = Element.ALIGN_MIDDLE;
                            celA4.MinimumHeight = 13f;
                            dataTableGrid.AddCell(celA4);

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.Accion.Message))
                            {
                                var mensaje = item.Accion.Message;
                                var regex = new Regex(Regex.Escape(";#;"));
                                foreach (var varReemplazo in Regex.Split(item.Data ?? "", ";#;"))
                                {
                                    mensaje = regex.Replace(mensaje, varReemplazo, 1);
                                }
                                item.Data = mensaje;
                            }
                            PdfPCell celA5 = new PdfPCell(new Phrase(item.Data.ToString(), arialFont));
                            celA5.HorizontalAlignment = Element.ALIGN_LEFT;
                            celA5.VerticalAlignment = Element.ALIGN_MIDDLE;
                            celA5.MinimumHeight = 13f;
                            dataTableGrid.AddCell(celA5);


                        }
                        if (item.DynamicDataSet != null && item.DynamicDataSet.DynamicDataSetRows.Count > 0)
                        {
                            var oldValue = "";
                            var value = "";
                            var lst = new List<VDetailAccion>();

                            var val = new string[] { };
                            var old = new string[] { };

                            Accion a = Accion.Dao.Get(item.Accion.Id);
                            if (a != null)
                            {
                                ViewData["Audit.DetailAccion.AccionLogAccion"] = a.Name;
                                switch (a.Code)
                                {

                                    case "UserProfilesEdit":
                                        value = Res.res.addedprofiles;
                                        oldValue = Res.res.deletedprofiles;
                                        var dataSet = DynamicDataSet.Dao.Get(item.DynamicDataSet.Id);
                                        var profileEdit = GetProfiles(dataSet.DynamicDataSetRows);
                                        if (profileEdit.Count > 1)
                                        {
                                            VDetailAccion itvDetailAccionPEdit = new VDetailAccion
                                            {
                                                StructDescription = @Res.res.associatedprofiles,
                                                OldValue = profileEdit[1],
                                                Value = profileEdit[0]
                                            };
                                            lst.Add(itvDetailAccionPEdit);
                                        }
                                        val = lst.Select(i => i.StructDescription + ": " + i.Value + " | " + oldValue + ": " + i.OldValue)
                                             .ToArray();

                                        PdfPCell celA6 = new PdfPCell(new Phrase(string.Join(" ", val), arialFont));
                                        celA6.HorizontalAlignment = Element.ALIGN_LEFT;
                                        celA6.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        celA6.MinimumHeight = 13f;
                                        dataTableGrid.AddCell(celA6);
                                        break;

                                    case "ProfileAccessEdit":
                                        var objDetail = new List<VDetailAccion>();
                                        //objDetail = GetLogAccionDetail(item);
                                        value = Res.res.addedaccess;
                                        oldValue = Res.res.deletedaccess;

                                        var dataSetPAE = DynamicDataSet.Dao.Get(item.DynamicDataSet.Id);
                                        var treeEdit = GetAccessComplete(dataSetPAE.DynamicDataSetRows);
                                        VDetailAccion itvDetailAccionAEdit = new VDetailAccion
                                        {
                                            StructDescription = @Res.res.associatedaccess,
                                            OldValue = treeEdit[1],
                                            Value = treeEdit[0]
                                        };
                                        // Eliminados
                                        // Añadidos
                                        objDetail.Add(itvDetailAccionAEdit);
                                        if (item.AccionToId != null)
                                        {
                                            val = objDetail.Select(i => $"{value}:{Environment.NewLine}{i.Value}{Environment.NewLine}{oldValue}:{Environment.NewLine}{i.OldValue}")
                                              .ToArray();

                                        }

                                        PdfPCell celA7 = new PdfPCell(new Phrase(string.Join(" ", val), arialFont));
                                        celA7.HorizontalAlignment = Element.ALIGN_LEFT;
                                        celA7.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        celA7.MinimumHeight = 13f;
                                        dataTableGrid.AddCell(celA7);
                                        break;
                                    default:
                                        lst = GetLogAccionDetalle(item);
                                        value = Res.res.actualvalue;
                                        oldValue = Res.res.previousvalue;
                                        val = lst.Select(i => $"- {i.StructDescription}: {i.Value} | {oldValue}: {i.OldValue}" + Environment.NewLine)
                                             .ToArray();
                                        PdfPCell celA8 = new PdfPCell(new Phrase(string.Join("", val), arialFont));
                                        celA8.HorizontalAlignment = Element.ALIGN_LEFT;
                                        celA8.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        celA8.MinimumHeight = 13f;
                                        dataTableGrid.AddCell(celA8);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            PdfPCell celA8 = new PdfPCell(new Phrase("", arialFont));
                            celA8.HorizontalAlignment = Element.ALIGN_LEFT;
                            celA8.VerticalAlignment = Element.ALIGN_MIDDLE;
                            celA8.MinimumHeight = 13f;
                            dataTableGrid.AddCell(celA8);
                        }
                        break;
                    case "Usuarios":
                        PdfPCell celU1 = new PdfPCell(new Phrase(Convert.ToString(item.Name), arialFont));
                        celU1.HorizontalAlignment = Element.ALIGN_CENTER;
                        celU1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celU1.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celU1);

                        PdfPCell celU2 = new PdfPCell(new Phrase(item.FullName, arialFont));
                        celU2.HorizontalAlignment = Element.ALIGN_LEFT;
                        celU2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celU2.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celU2);

                        PdfPCell celU3 = new PdfPCell(new Phrase(item.Email, arialFont));
                        celU3.HorizontalAlignment = Element.ALIGN_LEFT;
                        celU3.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celU3.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celU3);

                        PdfPCell celU4 = new PdfPCell(new Phrase(item.State.Name, arialFont));
                        celU4.HorizontalAlignment = Element.ALIGN_LEFT;
                        celU4.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celU4.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celU4);

                        PdfPCell celU5 = new PdfPCell(new Phrase(Convert.ToString(item.LastLogin), arialFont));
                        celU5.HorizontalAlignment = Element.ALIGN_LEFT;
                        celU5.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celU5.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celU5);

                        var profiles = DNF.Security.Bussines.UserProfile.Dao
                           .GetBy(new User { Id = item.Id });
                        profiles.LoadRelation(x => x.Profile);
                        PdfPCell celU6 = new PdfPCell(new Phrase(profiles.Select(x => x.Profile.Name).ToJoin(","), arialFont));
                        celU6.HorizontalAlignment = Element.ALIGN_LEFT;
                        celU6.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celU6.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celU6);
                        break;

                    case "Perfiles":
                        var access = DNF.Security.Bussines.ProfileAccess.Dao
                          .GetBy(new Profile { Id = item.Id });
                        access.LoadRelation(x => x.Access);
                        foreach (ProfileAccess a in access)
                        {
                            var accessObject = new Access();
                            if (a.Access.Name == null)
                            {
                                accessObject = Access.Dao.Get(a.Access.Id);
                                a.Access.Name = accessObject.Name;
                            }
                        }
                        PdfPCell celP1 = new PdfPCell(new Phrase(Convert.ToString(item.Name), arialFont));
                        celP1.Rowspan = access.Count;
                        celP1.HorizontalAlignment = Element.ALIGN_CENTER;
                        celP1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celP1.MinimumHeight = 13f;

                        dataTableGrid.AddCell(celP1);

                        PdfPCell celP2 = new PdfPCell(new Phrase(item.State.Name, arialFont));
                        celP2.Rowspan = access.Count;
                        celP2.HorizontalAlignment = Element.ALIGN_LEFT;
                        celP2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        celP2.MinimumHeight = 13f;
                        dataTableGrid.AddCell(celP2);

                        foreach (ProfileAccess a in access)
                        {
                            if (a.Access.Name != "")
                            {

                                PdfPCell celP3 = new PdfPCell(new Phrase(GetAccessTreeNavigation(a.Access), arialFont));
                                celP3.HorizontalAlignment = Element.ALIGN_LEFT;
                                celP3.VerticalAlignment = Element.ALIGN_MIDDLE;
                                celP3.MinimumHeight = 13f;
                                dataTableGrid.AddCell(celP3);


                            }

                        }
                        if (access.Count < 1)
                        {
                            PdfPCell celP3 = new PdfPCell(new Phrase("", arialFont));
                            celP3.MinimumHeight = 13f;
                            dataTableGrid.AddCell(celP3);
                        }


                        break;
                }



            }
            document.Add(dataTableGrid);
            document.Close();
            LogAccion.Dao.AddLog(codeLogAccion, Current.User.FullName, null);

            return this.File(output.ToArray(), "application/pdf", Convert.ToString(ConfigurationManager.AppSettings["Aplication"]) + "_" + tittle + "_" + Current.User.Name + " " + DateTime.Now.ToString("yyyy-MM-dd") + "-" + DateTime.Now.ToString("HH.mm") + ".pdf");
        }

        public FileContentResult exportTXT(string tittle)
        {

            dynamic data = "";
            string codeLogAccion = "";
            StringWriter sw = new StringWriter();
            sw.WriteLine("Listado de " + tittle + " de Seguridad - Fecha de Consulta: " + DateTime.Now.ToString());
            switch (tittle)
            {
                case "Usuarios":
                    data = DNF.Security.Bussines.User.Dao.GetAll();
                    codeLogAccion = "UsersExportTXT";
                    sw.WriteLine("================================================================================================================================================================================================================================================================================");
                    sw.WriteLine(FormatedSpace(@Res.res.name, 20) + "\t"
                    + FormatedSpace(@Res.res.fullname, 40) + "\t" + FormatedSpace(@Res.res.mail, 40) + "\t"
                    + FormatedSpace(@Res.res.state, 10) + "\t" + FormatedSpace(@Res.res.lastLogin, 25) + "\t" + FormatedSpace(@Res.res.profiles, 50) + "\t");
                    sw.WriteLine("================================================================================================================================================================================================================================================================================");
                    break;
                case "Perfiles":
                    data = DNF.Security.Bussines.Profile.Dao.GetAll().Where(z => z.State.Code != "Delete").ToList();
                    codeLogAccion = "ProfilesExportTXT";
                    sw.WriteLine("================================================================================================================================================================================================================================================================================");
                    sw.WriteLine(FormatedSpace(@Res.res.name, 35) + "\t"
                    + FormatedSpace(@Res.res.state, 10) + "\t" + FormatedSpace(@Res.res.accesses, 50) + "\t");
                    sw.WriteLine("================================================================================================================================================================================================================================================================================");
                    break;
            }

            using (sw)
            {
                foreach (var item in data)
                {
                    switch (tittle)
                    {
                        case "Usuarios":
                            var profiles = DNF.Security.Bussines.UserProfile.Dao
                            .GetBy(new User { Id = item.Id });
                            profiles.LoadRelation(x => x.Profile);
                            sw.WriteLine(FormatedSpace(item.Name, 20) + "\t"
                                        + FormatedSpace(item.FullName, 40) + "\t" + FormatedSpace(item.Email, 40) + "\t"
                                        + FormatedSpace(item.State.Name, 10) + "\t" + FormatedSpace(Convert.ToString(item.LastLogin), 25) + "\t"
                                        + FormatedSpace(profiles.Select(x => x.Profile.Name).ToJoin(","), 50) + "\t");
                            sw.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ");
                            break;

                        case "Perfiles":

                            var access = DNF.Security.Bussines.ProfileAccess.Dao
                            .GetBy(new Profile { Id = item.Id });
                            access.LoadRelation(x => x.Access);
                            foreach (ProfileAccess a in access)
                            {
                                var accessObject = new Access();
                                if (a.Access.Name == null)
                                {
                                    accessObject = Access.Dao.Get(a.Access.Id);
                                    a.Access.Name = accessObject.Name;
                                }
                            }
                            var count = 0;
                            foreach (ProfileAccess a in access)
                            {
                                if (count == 0)
                                {
                                    sw.WriteLine(FormatedSpace(item.Name, 35) + "\t"
                                                + FormatedSpace(item.State.Name, 10) + "\t"
                                                + FormatedSpace(GetAccessTreeNavigation(a.Access), 50) + "\t");
                                }
                                else
                                {
                                    sw.WriteLine(FormatedSpace("", 35) + "\t"
                                               + FormatedSpace("", 10) + "\t"
                                               + FormatedSpace(GetAccessTreeNavigation(a.Access), 50) + "\t");
                                }
                                count++;
                            }
                            sw.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ");
                            break;

                    }


                }
            }

            String contenido = sw.ToString();
            String NombreArchivo = Convert.ToString(ConfigurationManager.AppSettings["Aplication"]) + "_" + tittle + "_" + Current.User.Name + " " + DateTime.Now.ToString("yyyy-MM-dd") + "-" + DateTime.Now.ToString("HH.mm");
            String ExtensionArchivo = "txt";
            LogAccion.Dao.AddLog(codeLogAccion, Current.User.Name, null);
            return File(new System.Text.UTF8Encoding().GetBytes(contenido), "text/" + ExtensionArchivo, NombreArchivo + "." + ExtensionArchivo);

        }
        public static string FormatedSpace(string val, int fixedLen)
        {
            if (val == null)
            {
                val = "";
            }
            int len = 0;
            string retVal = string.Empty;
            try
            {
                len = val.Length;
                retVal = val;
                for (int cnt = 0; cnt < fixedLen - len - 1; cnt++)
                {
                    retVal = retVal + " ";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retVal;
        }
        [AccessCode("Profiles")]
        public ActionResult Profiles()
        {
            ViewBag.Title = "Profiles";

            ViewBag.HabilitadoBotonExportXls = Current.User.HasAccess("ProfilesExportExcel");
            ViewBag.HabilitadoBotonExportPdf = Current.User.HasAccess("ProfilesExportPDF");
            ViewBag.HabilitadoBotonExportTxt = Current.User.HasAccess("ProfilesExportTXT");
            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            LogAccion.Dao.AddLog("SecurityAccess"
                //el usuario X a navegado a Y
                , Current.User.Name + ";#;" + acceso
                );

            return View();
        }
        [AccessCode("Profiles")]
        public ActionResult ProfilesData()
        {

            var profilesData = DNF.Security.Bussines.Profile.Dao
                .GetAll().Where(z => z.State.Code != "Delete").Select(x => new
                {
                    x.Id,
                    x.Name,
                    State_Code = x.State.Name,
                    Access = Access.Dao.GetUnNested(x.Accesses)
                        .Select(a => a.Id)
                        .ToArray()
                }).ToArray();
            Session["profileAcces"] = profilesData;
            return Json(profilesData, JsonRequestBehavior.AllowGet);
        }
        [AccessCode("ProfileEdit")]
        public ActionResult ProfilesEdit(Profile profile, string oper)
        {
            if (oper == "del")
            {
                var finalProfileDel = DNF.Security.Bussines.Profile.Dao.Get(profile.Id);
                if (finalProfileDel.State.Name != @Res.res.deleted)
                {
                    finalProfileDel.State = new Type("UserState", "Delete");
                    var origProfileDel = DNF.Security.Bussines.Profile.Dao.Get(profile.Id);
                    LogAccion.Dao.AddLog("ProfileDelete"
                       //, "The profile " + finalProfile.Name + " has changed the state to " + finalProfile.State.Name.ToLower() + "."
                       , finalProfileDel.Name + ";#;" + finalProfileDel.State.Name.ToLower()
                       , DynamicDataSet.Dao.CreateChangeSet(origProfileDel, finalProfileDel)
                       , finalProfileDel.Name, finalProfileDel.Id);
                        finalProfileDel.Save();
                }
                return Json(new { ok = true }, JsonRequestBehavior.AllowGet);

            }
            //info del profile 
            var finalProfile = profile.IsNew() ? new Profile() : DNF.Security.Bussines.Profile.Dao.Get(profile.Id);

            var origProfile = new Profile();
            finalProfile.CopyTo(origProfile);

            finalProfile.Name = profile.Name;
            finalProfile.State = new Type("UserState", Request.Form["State_Code"]);

            if (finalProfile.IsNew())
            {
                LogAccion.Dao.AddLog("ProfileNew"
                //, "The profile " + finalProfile.Name + " has been created."
                , finalProfile.Name
                , DynamicDataSet.Dao.CreateChangeSet(null, finalProfile)
                , finalProfile.Name, finalProfile.Id);
            }
            else
            {
                if (origProfile.Name != finalProfile.Name)
                {
                    LogAccion.Dao.AddLog("ProfileEdit"
                    //, "The profile " + finalProfile.Name + " has been changed."
                    , finalProfile.Name
                    , DynamicDataSet.Dao.CreateChangeSet(origProfile, finalProfile)
                    , finalProfile.Name, finalProfile.Id);
                }

                if (finalProfile.State.Name != origProfile.State.Name)
                {
                    finalProfile.GetEntity();
                    LogAccion.Dao.AddLog("ProfileChangeState"
                    //, "The profile " + finalProfile.Name + " has changed the state to " + finalProfile.State.Name.ToLower() + "."
                    , finalProfile.Name + ";#;" + finalProfile.State.Name.ToLower()
                    , DynamicDataSet.Dao.CreateChangeSet(origProfile, finalProfile)
                    , finalProfile.Name, finalProfile.Id);
                }
            }

            finalProfile.Save();



            //access asociados al profile 
            var listEditedAccesss = Access.Dao.Get(
                    Request.Form["Access"] //"1,2,3"
                        .Split(char.Parse(","))
                        .Where(x => x != "") // ["1","2","3"]
                        .Select(long.Parse) //[1, 2, 3]
                        .ToList()
                    ); //traigo todos los acces elegidos de la lista de checks 
            listEditedAccesss = Access.Dao.GetUnNested(listEditedAccesss);

            finalProfile.ProfileAccess.LoadRelation(x => x.Access);

            var dicEdited = listEditedAccesss.ToDictionary();
            var dicOrig = finalProfile.ProfileAccess.Select(x => x.Access).ToDictionary();

            finalProfile.ProfileAccess.Where(x => !dicEdited.ContainsKey(x.Access.Id)).Delete();
            listEditedAccesss
                .Where(x => !dicOrig.ContainsKey(x.Id))
                .Select(a => new ProfileAccess { Access = a, Profile = finalProfile })
                .Save();

            finalProfile.ProfileAccess = null; // vuelve a activar el lazzy loading

            var interseccion = dicEdited.Keys.Intersect(dicOrig.Keys).Count();
            var AccessChanged = interseccion != dicOrig.Keys.Count || interseccion != dicEdited.Keys.Count;

            if (AccessChanged && listEditedAccesss.Any())
                LogAccion.Dao.AddLog("ProfileAccessEdit"
                    //, "The access for profile " + finalProfile.Name + " has been changed."
                    , finalProfile.Name
                    , DynamicDataSet.Dao.CreateChangeSet(dicOrig.Values.ToList(), dicEdited.Values.ToList())
                    , finalProfile.Name, finalProfile.Id);



            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AccessTreeData()
        {
            // todos los accesos correctamente anidados uno dentro del otro 
            var access = Access.Dao.GetAll();

            return Json(ToTreeView(access), JsonRequestBehavior.AllowGet);
        }
        private static object ToTreeView(IEnumerable<Access> access)
        {
            return access
                .Select(x => new
                {
                    text = x.Name,
                    icon = !string.IsNullOrEmpty(x.Icon) ? x.Icon : x.Type.Icon ?? "",
                    Id = x.Id.ToString(),
                    nodeId = x.Id.ToString(),
                    nodes = x.Accesss.Any() ? ToTreeView(x.Accesss) : null
                }).ToArray();
        }

        private List<string> ProfilesGetColNames()
        {

            List<string> colNames = new List<string>();

            colNames.Add(@Res.res.name);
            colNames.Add(@Res.res.state);
            colNames.Add(@Res.res.accesses);

            return colNames;
        }

        private dynamic ProfilesLoadData()
        {
            var profilesAcces = new List<ProfileAccess>();
            var profilesAccesFinal = new List<ProfileAccess>();
            profilesAcces = ProfileAccess.Dao.GetAll();
            profilesAcces.LoadRelation(s => s.Profile);
            profilesAcces.LoadRelation(s => s.Access);
            foreach (ProfileAccess access in profilesAcces)
            {
                var accessObject = new Access();
                if (access.Access.Name == null)
                {
                    accessObject = Access.Dao.Get(access.Access.Id);
                    access.Access.Name = accessObject.Name;
                }
            }
            return new
            {
                rows = profilesAcces
            };
        }


        [AccessCode("Audit")]
        public ActionResult Audit()
        {
            ViewBag.Title = "Audit";
            ViewBag.tmpTipoAccion = Type.Dao.GetByFilter(new
            {
                TypeConfig_Id = 13
            }).ToList();

            ViewBag.HabilitadoBotonExportXls = Current.User.HasAccess("AuditExportExcel");
            ViewBag.HabilitadoBotonExportPdf = Current.User.HasAccess("AuditExportPDF");

            var list = Accion.Dao.GetAll();

            var listData = new MultiSelectList(list, "Id", "Name");

            ViewBag.tmpDataSource2 = listData;
            //System.Web.Mvc.MultiSelectList listData =(System.Web.Mvc.MultiSelectList) Accion.Dao.GetAll().ToListItemCode("Todas ");
            ViewBag.Acciones = Accion.Dao.GetAll()
               .Select(x => new SelectListItem()
               {
                   Text = x.Name,
                   Value = x.Id.ToString()
               }).ToList();
            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            LogAccion.Dao.AddLog("SecurityAccess"
                //el usuario X a navegado a Y
                , Current.User.Name + ";#;" + acceso
                );

            return View();
        }
        [AccessCode("Audit")]
        public ActionResult GetAccions(long? id)
        {
            List<SelectListItem> accesList = new List<SelectListItem>();

            var list = id.HasValue
                        ? Accion.Dao.GetByFilter(new { Category_Type_Id = id.Value })
                        : Accion.Dao.GetAll();
            var listData = new MultiSelectList(list.OrderBy(x => x.Name), "Id", "Name");

            return Json(listData, JsonRequestBehavior.AllowGet);
        }
        [AccessCode("Audit")]
        public ActionResult GetActions(string fromDate, string toDate, string act01, string page, string rows)
        {
            return Json(GetJsonResponse(fromDate, toDate, act01, Convert.ToInt32(page), Convert.ToInt32(rows)), JsonRequestBehavior.AllowGet);
        }
        [Authenticated]
        private ResponseAuditJson GetJsonResponse(string fromDate, string toDate, string act01, int page, int rows)
        {
            var dataAction = new List<LogAccion>();
            var dataActionTemp = new List<LogAccion>();

            if (!(page > 0)) page = 1;
            if (!(rows > 0)) rows = 20;

            if (fromDate != "" && toDate != "" && act01 == "0")
            {
                DateTime fDate;
                if (!DateTime.TryParse(fromDate, out fDate))
                    return null;

                DateTime tDate;
                if (!DateTime.TryParse(toDate, out tDate))
                    return null;
                dataAction = LogAccion.Dao.GetByFilter(new
                {
                    dateStart = fromDate,
                    DateEnd = toDate,
                    Page = page,
                    PageSize = rows
                }).ToList();
                dataAction.LoadRelation(y => y.User);
                dataAction.LoadRelation(y => y.Accion);
                dataAction.LoadRelation(y => y.DynamicDataSet);

            }


            if (fromDate != "" && toDate != "" && act01 != "0")
            {
                act01 = act01.Remove(act01.Length - 1);

                dataActionTemp = LogAccion.Dao.GetByFilter(new
                {
                    DateStart = fromDate,
                    DateEnd = toDate,
                    Accion_Ids = act01,
                    Page = page,
                    PageSize = rows
                }).ToList();
                dataActionTemp.LoadRelation(y => y.User);
                dataActionTemp.LoadRelation(y => y.Accion);
                dataActionTemp.LoadRelation(y => y.DynamicDataSet);

                dataAction = dataActionTemp;
            }

            //preparo el json para devolver
            var recordsTemp = (dataAction != null && dataAction.Count > 0) ? dataAction[0].RowsTotal.Value : 0;

            var totalTemp = 0;
            if (recordsTemp > 0)
                totalTemp = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(recordsTemp) / Convert.ToDecimal(rows)));

            var jsonResponse = new ResponseAuditJson()
            {
                groupHeader = new List<GroupHeaderAudit>(),
                ColModel = new List<ColJqGridAudit>(),
                ColNames = new List<string>(),
                rows = new List<Dictionary<string, string>>(),
                userdata = new Dictionary<string, string>(),
                Page = page,
                PageSize = rows,
                records = recordsTemp,
                total = totalTemp
            };

            //columna descripcion
            jsonResponse.ColModel.Add(new ColJqGridAudit()
            {
                index = "Date",
                Name = "Date"
            });
            jsonResponse.ColModel.Add(new ColJqGridAudit()
            {
                index = "User",
                Name = "User"
            });
            jsonResponse.ColModel.Add(new ColJqGridAudit()
            {
                index = "Action",
                Name = "Action"
            });
            jsonResponse.ColModel.Add(new ColJqGridAudit()
            {
                index = "Detail",
                Name = "Detail"
            });
            jsonResponse.ColModel.Add(new ColJqGridAudit()
            {
                index = "View",
                Name = "View"
            });
            jsonResponse.ColNames.Add(@Res.res.date);
            jsonResponse.ColNames.Add(@Res.res.user);
            jsonResponse.ColNames.Add(@Res.res.action);
            jsonResponse.ColNames.Add(@Res.res.detail);
            jsonResponse.ColNames.Add(@Res.res.viewDetails);
            var resultData = new List<Dictionary<string, string>>();

            foreach (LogAccion item in dataAction)
            {
                var rowData = new Dictionary<string, string>();
                foreach (ColJqGridAudit prop in jsonResponse.ColModel)
                {
                    string key = "";
                    string value = "";

                    if (prop.index.Equals("Date"))
                    {
                        key = prop.index;
                        value = item.Date.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    if (prop.index.Equals("User"))
                    {
                        if (item.Accion.Code == "UserDoesNotExist")
                        {
                            key = prop.index;
                            value = "";
                        }
                        else
                        {
                            key = prop.index;
                            value = item.User.FullName.ToString();
                        }
                    }
                    if (prop.index.Equals("Action"))
                    {

                        key = prop.index;
                        value = item.Accion.Name.ToString();

                    }
                    if (prop.index.Equals("Detail"))
                    {
                        key = prop.index;
                        if (string.IsNullOrEmpty(item.Data) && string.IsNullOrEmpty(item.Accion.Message))
                        {
                            value = "";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.Accion.Message))
                            {
                                var mensaje = item.Accion.Message;
                                var regex = new Regex(Regex.Escape(";#;"));
                                foreach (var varReemplazo in Regex.Split(item.Data ?? "", ";#;"))
                                {
                                    mensaje = regex.Replace(mensaje, varReemplazo, 1);
                                }
                                item.Data = mensaje;
                            }
                            value = item.Data.ToString();
                        }
                    }
                    if (prop.index.Equals("View"))
                    {
                        key = prop.index;
                        if (item.DynamicDataSet != null && item.DynamicDataSet.DynamicDataSetRows.Count > 0)
                        {
                            value = "<div><a class='k-button' href=\"javascript:CallView('"
                                + item.Id
                                + "')\"><span class='fa fa-search'></span>" + Res.res.view + "</a></div>";
                        }
                        else
                        {
                            value = "";
                        }
                    }
                    rowData.Add(key, value);
                }
                resultData.Add(rowData);
            }
            jsonResponse.rows = resultData;

            return jsonResponse;
        }
        [AccessCode("Audit")]
        public ActionResult ViewDetail(int idLogAccion)
        {
            var lst = new List<VDetailAccion>();

            if (idLogAccion <= 0) return PartialView("_ViewDetailAccion", lst);
            LogAccion r = LogAccion.Dao.Get(idLogAccion);

            ViewData["Audit.DetailAccion.DateLogAccion"] = r.Date;

            User u = DNF.Security.Bussines.User.Dao.Get(r.User.Id);
            if (u != null)
                ViewData["Audit.DetailAccion.UserLogAccion"] = u.FullName;

            Accion a = Accion.Dao.Get(r.Accion.Id);
            if (a != null)
            {

                ViewData["Audit.DetailAccion.AccionLogAccion"] = a.Name;
                switch (a.Code)
                {

                    case "UserProfilesEdit":
                        ViewData["Audit.DetailAccion.GridTitleValue"] = Res.res.addedprofiles;
                        ViewData["Audit.DetailAccion.GridTitleOldValue"] = Res.res.deletedprofiles;
                        break;
                    case "ProfileAccessEdit":
                        ViewData["Audit.DetailAccion.GridTitleValue"] = Res.res.addedaccess;
                        ViewData["Audit.DetailAccion.GridTitleOldValue"] = Res.res.deletedaccess;
                        break;
                    default:
                        ViewData["Audit.DetailAccion.GridTitleValue"] = Res.res.actualvalue;
                        ViewData["Audit.DetailAccion.GridTitleOldValue"] = Res.res.previousvalue;


                        break;
                }


            }

            lst = GetLogAccionDetalle(r);

            return PartialView("_ViewDetailAction", lst);
        }
        private List<VDetailAccion> GetLogAccionDetalle(LogAccion objLogAccion)
        {
            var objDetail = new List<VDetailAccion>();

            var dataSet = DynamicDataSet.Dao.Get(objLogAccion.DynamicDataSet.Id);
            if (dataSet == null) return objDetail;
            var accion = Accion.Dao.Get(objLogAccion.Accion.Id);
            switch (accion.Code.Trim())
            {

                case "UserProfilesEdit":
                    var profileEdit = GetProfiles(dataSet.DynamicDataSetRows);
                    if (profileEdit.Count > 1)
                    {
                        VDetailAccion itvDetailAccionPEdit = new VDetailAccion
                        {
                            StructDescription = @Res.res.associatedprofiles,
                            OldValue = profileEdit[1],
                            Value = profileEdit[0]
                        };
                        objDetail.Add(itvDetailAccionPEdit);
                    }


                    // Eliminados
                    // Añadidos

                    break;

                case "ProfileAccessEdit":
                    var treeEdit = GetAccessTreeComplete(dataSet.DynamicDataSetRows);
                    VDetailAccion itvDetailAccionAEdit = new VDetailAccion
                    {
                        StructDescription = @Res.res.associatedaccess,
                        OldValue = treeEdit[1],
                        Value = treeEdit[0]
                    };
                    // Eliminados
                    // Añadidos
                    objDetail.Add(itvDetailAccionAEdit);
                    break;

                case "ChangeState":
                    objDetail.AddRange(dataSet.DynamicDataSetRows.SelectMany(it0 => it0.Datas, (it0, it1) => GetDetailAccionState(it1)).Where(objA => objA.RowShow));
                    break;

                default:
                    objDetail.AddRange(dataSet.DynamicDataSetRows.SelectMany(it0 => it0.Datas, (it0, it1) => GetDetailAccion(it1)).Where(objA => objA.RowShow));
                    //////////////////////pasword
                    break;
            }

            return objDetail;
        }



        private List<string> GetProfiles(IEnumerable<DynamicDataSetRow> dataSetRows)
        {
            var asProfile = new List<string>();
            string profilesAñadidos = "";
            string profilesEliminados = "";

            try
            {
                foreach (DynamicDataSetRow it0 in dataSetRows)
                {
                    var accionType = it0.Accion.Code;
                    var profile = DNF.Security.Bussines.Profile.Dao.Get(it0.UniqueId);
                    if (profile.Id > 0)
                    {
                        switch (accionType)
                        {
                            case "Insert":
                                profilesAñadidos += profile.Name + ", ";
                                break;
                            case "Delete":
                                profilesEliminados += profile.Name + ", ";
                                break;
                        }
                    }
                }

                asProfile.Add(profilesAñadidos.Length > 2
                    ? profilesAñadidos.Substring(0, profilesAñadidos.Length - 2)
                    : "-");

                asProfile.Add(profilesEliminados.Length > 2
                    ? profilesEliminados.Substring(0, profilesEliminados.Length - 2)
                    : "-");
            }
            catch
            {
                // ignored
            }

            return asProfile;
        }
        private List<string> GetAccessTreeComplete(IEnumerable<DynamicDataSetRow> dataSetRows)
        {
            var asTree = new List<string>();
            string accessTreeAñadidos = "<ul>";
            string accessTreeEliminados = "<ul>";

            try
            {
                foreach (DynamicDataSetRow it0 in dataSetRows)
                {
                    var accionType = it0.Accion.Code;
                    var access = Access.Dao.Get(it0.UniqueId);
                    if (access == null) continue;
                    if (access.Id <= 0) continue;
                    var accessType = access.Type.Code.ToString();
                    switch (accionType)
                    {
                        case "Insert":
                            switch (accessType)
                            {
                                //Folder = 1,
                                //Item = 2,
                                //Permisson = 3,
                                //Control = 4
                                //case "Folder":
                                //    accessTreeAñadidos += Environment.NewLine + "[" + access.Name + "]" + "->";
                                //    break;
                                case "Menu":
                                    // Tiene Hijos
                                    var son = false;
                                    foreach (DynamicDataSetRow x in dataSetRows)
                                    {
                                        var padreX = Access.Dao.Get(access.Parent.Id);
                                        if (padreX.Id == access.Id)
                                        {
                                            son = true;
                                            break;
                                        }
                                    }
                                    if (!son)
                                    {
                                        var padreM = Access.Dao.Get(access.Parent.Id);
                                        accessTreeAñadidos += "<li>[" + padreM.Name + "]->" + "[" + access.Name + "]</li>";
                                    }

                                    break;
                                case "Item":
                                    var padreI = Access.Dao.Get(access.Parent.Id);
                                    accessTreeAñadidos += "<li>[" + padreI.Name + "]->" + "[" + access.Name + "]</li>";
                                    break;
                                case "Permission":
                                    var padreP = access.Parent != null ? Access.Dao.Get(access.Parent.Id) : null;
                                    var abueloP = padreP?.Parent != null ? Access.Dao.Get(padreP.Parent.Id) : null;
                                    accessTreeAñadidos += "<li>[";
                                    if (abueloP != null)
                                        accessTreeAñadidos += abueloP.Name + "]->" + "[";
                                    if (padreP != null)
                                        accessTreeAñadidos += padreP.Name + "]" + "->" + "[";
                                    accessTreeAñadidos += access.Name + "]</li>";
                                    break;
                                case "Control":
                                    var padreC = Access.Dao.Get(access.Parent.Id);
                                    var abueloC = Access.Dao.Get(padreC.Parent.Id);
                                    if (abueloC != null)
                                    {
                                        var nietoC = Access.Dao.Get(abueloC.Parent.Id);
                                        if (nietoC != null)
                                        {
                                            accessTreeAñadidos += "<li>[" + nietoC.Name + "]->" + "[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]</li>";
                                        }
                                        else
                                        {
                                            accessTreeAñadidos += "<li>[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]</li>";
                                        }
                                    }
                                    else
                                    {
                                        accessTreeAñadidos += "<li>[" + padreC.Name + "]" + "->" + "[" + access.Name + "]</li>";
                                    }
                                    break;
                            }
                            break;
                        case "Delete":
                            switch (accessType)
                            {
                                //Folder = 1,
                                //Item = 2,
                                //Permisson = 3,
                                //Control = 4                              
                                case "Menu":
                                    // Tiene Hijos
                                    var son = false;
                                    foreach (DynamicDataSetRow x in dataSetRows)
                                    {
                                        var padreX = Access.Dao.Get(access.Parent.Id);
                                        if (padreX.Id == access.Id)
                                        {
                                            son = true;
                                            break;
                                        }
                                    }
                                    if (!son)
                                    {
                                        var padreM = Access.Dao.Get(access.Parent.Id);
                                        accessTreeEliminados += "<li>[" + padreM.Name + "]->" + "[" + access.Name + "]</li>";
                                    }
                                    break;
                                case "Item":
                                    var padreI = Access.Dao.Get(access.Parent.Id);
                                    accessTreeEliminados += "<li>[" + padreI.Name + "]->" + "[" + access.Name + "]</li>";
                                    break;
                                case "Permission":
                                    var padreP = access.Parent != null ? Access.Dao.Get(access.Parent.Id) : null;
                                    var abueloP = padreP?.Parent != null ? Access.Dao.Get(padreP.Parent.Id) : null;
                                    accessTreeEliminados += "<li>[";
                                    if (abueloP != null)
                                        accessTreeEliminados += abueloP.Name + "]->" + "[";
                                    if (padreP != null)
                                        accessTreeEliminados += padreP.Name + "]" + "->" + "[";
                                    accessTreeEliminados += access.Name + "]</li>";
                                    break;
                                case "Control":
                                    var padreC = Access.Dao.Get(access.Parent.Id);
                                    var abueloC = Access.Dao.Get(padreC.Parent.Id);
                                    if (abueloC != null)
                                    {
                                        var nietoC = Access.Dao.Get(abueloC.Parent.Id);
                                        if (nietoC != null)
                                        {
                                            accessTreeEliminados += "<li>[" + nietoC.Name + "]->" + "[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]</li>";
                                        }
                                        else
                                        {
                                            accessTreeEliminados += "<li>[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]</li>";
                                        }
                                    }
                                    else
                                    {
                                        accessTreeEliminados += "<li>[" + padreC.Name + "]" + "->" + "[" + access.Name + "]</li>";
                                    }
                                    break;
                            }
                            break;
                    }
                }

                if (accessTreeAñadidos != "<ul>")
                {
                    accessTreeAñadidos += "</ul>";
                    asTree.Add(accessTreeAñadidos/*accessTreeAñadidos.Substring(0, accessTreeAñadidos.Length - 2)*/);
                }
                else
                {
                    asTree.Add("-");
                }

                if (accessTreeEliminados != "<ul>")
                {
                    accessTreeEliminados += "</ul>";
                    asTree.Add(accessTreeEliminados/*accessTreeEliminados.Substring(0, accessTreeEliminados.Length - 2)*/);
                }
                else
                {
                    asTree.Add("-");
                }
            }
            catch
            {
                // ignored
            }

            return asTree;
        }

        private List<string> GetAccessComplete(IEnumerable<DynamicDataSetRow> dataSetRows)
        {
            var asTree = new List<string>();
            string accessTreeAñadidos = "";
            string accessTreeEliminados = "";

            try
            {
                foreach (DynamicDataSetRow it0 in dataSetRows)
                {
                    var accionType = it0.Accion.Code;
                    var access = Access.Dao.Get(it0.UniqueId);
                    if (access == null) continue;
                    if (access.Id <= 0) continue;
                    var accessType = access.Type.Code.ToString();
                    switch (accionType)
                    {
                        case "Insert":
                            switch (accessType)
                            {
                                //Folder = 1,
                                //Item = 2,
                                //Permisson = 3,
                                //Control = 4
                                //case "Folder":
                                //    accessTreeAñadidos += Environment.NewLine + "[" + access.Name + "]" + "->";
                                //    break;
                                case "Menu":
                                    // Tiene Hijos
                                    var son = false;
                                    foreach (DynamicDataSetRow x in dataSetRows)
                                    {
                                        var padreX = Access.Dao.Get(access.Parent.Id);
                                        if (padreX.Id == access.Id)
                                        {
                                            son = true;
                                            break;
                                        }
                                    }
                                    if (!son)
                                    {
                                        var padreM = Access.Dao.Get(access.Parent.Id);
                                        accessTreeAñadidos += "[" + padreM.Name + "]->" + "[" + access.Name + "]" + Environment.NewLine;
                                    }

                                    break;
                                case "Item":
                                    var padreI = Access.Dao.Get(access.Parent.Id);
                                    accessTreeAñadidos += "[" + padreI.Name + "]->" + "[" + access.Name + "]" + Environment.NewLine;
                                    break;
                                case "Permission":
                                    var padreP = access.Parent != null ? Access.Dao.Get(access.Parent.Id) : null;
                                    var abueloP = padreP?.Parent != null ? Access.Dao.Get(padreP.Parent.Id) : null;
                                    accessTreeAñadidos += "[";
                                    if (abueloP != null)
                                        accessTreeAñadidos += abueloP.Name + "]->" + "[";
                                    if (padreP != null)
                                        accessTreeAñadidos += padreP.Name + "]" + "->" + "[";
                                    accessTreeAñadidos += access.Name + "]" + Environment.NewLine;
                                    break;
                                case "Control":
                                    var padreC = Access.Dao.Get(access.Parent.Id);
                                    var abueloC = Access.Dao.Get(padreC.Parent.Id);
                                    if (abueloC != null)
                                    {
                                        var nietoC = Access.Dao.Get(abueloC.Parent.Id);
                                        if (nietoC != null)
                                        {
                                            accessTreeAñadidos += "[" + nietoC.Name + "]->" + "[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            accessTreeAñadidos += "[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]" + Environment.NewLine;
                                        }
                                    }
                                    else
                                    {
                                        accessTreeAñadidos += "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]" + Environment.NewLine;
                                    }
                                    break;
                            }
                            break;
                        case "Delete":
                            switch (accessType)
                            {
                                //Folder = 1,
                                //Item = 2,
                                //Permisson = 3,
                                //Control = 4                              
                                case "Menu":
                                    // Tiene Hijos
                                    var son = false;
                                    foreach (DynamicDataSetRow x in dataSetRows)
                                    {
                                        var padreX = Access.Dao.Get(access.Parent.Id);
                                        if (padreX.Id == access.Id)
                                        {
                                            son = true;
                                            break;
                                        }
                                    }
                                    if (!son)
                                    {
                                        var padreM = Access.Dao.Get(access.Parent.Id);
                                        accessTreeEliminados += "[" + padreM.Name + "]->" + "[" + access.Name + "]" + Environment.NewLine;
                                    }
                                    break;
                                case "Item":
                                    var padreI = Access.Dao.Get(access.Parent.Id);
                                    accessTreeEliminados += "[" + padreI.Name + "]->" + "[" + access.Name + "]" + Environment.NewLine;
                                    break;
                                case "Permission":
                                    var padreP = access.Parent != null ? Access.Dao.Get(access.Parent.Id) : null;
                                    var abueloP = padreP?.Parent != null ? Access.Dao.Get(padreP.Parent.Id) : null;
                                    accessTreeEliminados += "[";
                                    if (abueloP != null)
                                        accessTreeEliminados += abueloP.Name + "]->" + "[";
                                    if (padreP != null)
                                        accessTreeEliminados += padreP.Name + "]" + "->" + "[";
                                    accessTreeEliminados += access.Name + "]" + Environment.NewLine;
                                    break;
                                case "Control":
                                    var padreC = Access.Dao.Get(access.Parent.Id);
                                    var abueloC = Access.Dao.Get(padreC.Parent.Id);
                                    if (abueloC != null)
                                    {
                                        var nietoC = Access.Dao.Get(abueloC.Parent.Id);
                                        if (nietoC != null)
                                        {
                                            accessTreeEliminados += "[" + nietoC.Name + "]->" + "[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            accessTreeEliminados += "[" + abueloC.Name + "]->" + "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]" + Environment.NewLine;
                                        }
                                    }
                                    else
                                    {
                                        accessTreeEliminados += "[" + padreC.Name + "]" + "->" + "[" + access.Name + "]" + Environment.NewLine;
                                    }
                                    break;
                            }
                            break;
                    }
                }

                if (accessTreeAñadidos != "")
                {
                    asTree.Add(accessTreeAñadidos/*accessTreeAñadidos.Substring(0, accessTreeAñadidos.Length - 2)*/);
                }
                else
                {
                    asTree.Add("-");
                }

                if (accessTreeEliminados != "")
                {
                    asTree.Add(accessTreeEliminados/*accessTreeEliminados.Substring(0, accessTreeEliminados.Length - 2)*/);
                }
                else
                {
                    asTree.Add("-");
                }
            }
            catch
            {
                // ignored
            }

            return asTree;
        }
        private VDetailAccion GetDetailAccionState(DynamicDataSetData dataSetData)
        {
            VDetailAccion itvDetailAccion = new VDetailAccion { Struct = dataSetData.Struct };

            var objStruct = Struct.Dao.Get(dataSetData.Struct.Id);
            //if (objStruct != null && objStruct.Description != null)
            if (objStruct != null)
            {
                //if (objStruct.Name == "State_Type_Id" || objStruct.Name == "Name")
                if (objStruct.Name == "State_Type_Id")
                {
                    //itvDetailAccion.StructDescription = objStruct.Description ?? objStruct.Name;
                    itvDetailAccion.StructDescription = objStruct.Description ?? "Estado";
                    itvDetailAccion.OldValue = (objStruct.Name == "State_Type_Id") ? GetStateNameAudit(dataSetData.OldValue) : dataSetData.OldValue;
                    itvDetailAccion.Value = (objStruct.Name == "State_Type_Id") ? GetStateNameAudit(dataSetData.Value) : dataSetData.Value;

                    if (itvDetailAccion.OldValue == null)
                        itvDetailAccion.RowShow = false;
                    else
                        itvDetailAccion.RowShow = true;
                }
            }

            return itvDetailAccion;
        }
        private string GetStateNameAudit(string stateTypeId)
        {
            string nameCode = "";
            try
            {
                if (stateTypeId != null)
                {
                    var type = Type.Dao.Get(Convert.ToInt64(stateTypeId));
                    if (type != null)
                        nameCode = type.Name;
                }
            }
            catch
            {
                // ignored
            }

            return nameCode;
        }
        private VDetailAccion GetDetailAccion(DynamicDataSetData dataSetData)
        {
            VDetailAccion itvDetailAccion = new VDetailAccion { Struct = dataSetData.Struct };

            var objStruct = Struct.Dao.Get(dataSetData.Struct.Id);
            //if (objStruct == null || objStruct.Description == null) return itvDetailAccion;
            if (objStruct == null) return itvDetailAccion;
            if (objStruct.Description == Res.res.password) return itvDetailAccion;
            if (objStruct.Description == Res.res.sessionopen) return itvDetailAccion;
            if (objStruct.Description == Res.res.activationkey) return itvDetailAccion;
            itvDetailAccion.StructDescription = objStruct.Description ?? objStruct.Name;
            itvDetailAccion.OldValue = (objStruct.Name == "State_Type_Id") ? GetStateNameAudit(dataSetData.OldValue) : dataSetData.OldValue;
            itvDetailAccion.Value = (objStruct.Name == "State_Type_Id") ? GetStateNameAudit(dataSetData.Value) : dataSetData.Value;

            if (
                (string.IsNullOrEmpty(itvDetailAccion.OldValue) && (string.IsNullOrEmpty(itvDetailAccion.Value) || itvDetailAccion.Value == "0"))
                || (objStruct.Name == "State_Type_Id" && itvDetailAccion.OldValue == "")
                || (objStruct.Name == "Id")
                )
                itvDetailAccion.RowShow = false;
            else
                itvDetailAccion.RowShow = true;

            itvDetailAccion.StructDescription = itvDetailAccion.StructDescription == "State_Type_Id" ? "State" : itvDetailAccion.StructDescription;

            return itvDetailAccion;
        }
        private List<string> AuditGetColNames()
        {

            List<string> colNames = new List<string>();
            colNames.Add(@Res.res.date);
            colNames.Add(@Res.res.user);
            colNames.Add(@Res.res.action);
            colNames.Add(@Res.res.detail);
            colNames.Add(@Res.res.detailaction);
            return colNames;
        }
        private dynamic AuditLoadData(string fromDate, string toDate, string act01)
        {
            var logAccion = new List<LogAccion>();
            if (fromDate != "" && toDate != "" && act01 != "")
            {
                DateTime fDate;
                if (!DateTime.TryParse(fromDate, out fDate))
                    return null;

                DateTime tDate;
                if (!DateTime.TryParse(toDate, out tDate))
                    return null;
                logAccion = DNF.Security.Bussines.LogAccion.Dao.GetByFilter(new
                {
                    dateStart = fromDate,
                    DateEnd = toDate,
                    Accion_Ids = act01
                }).ToList();
                logAccion.LoadRelation(s => s.User);
                logAccion.LoadRelation(s => s.Access);
                logAccion.LoadRelation(s => s.Accion);
            }

            return logAccion;


        }
        [AccessCode("EventAudit")]
        public ActionResult Events()
        {
            ViewBag.Title = "Reporte de Eventos Auditables";
            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            LogAccion.Dao.AddLog("SecurityAccess"
                //el usuario X a navegado a Y
                , Current.User.Name + ";#;" + acceso
                );

            return View();
        }
        [AccessCode("EventAudit")]
        public ActionResult EventsAudit()
        {
            var actions = Accion.Dao.GetAll();
            var gridData = actions.Select(x => new
            {
                x.Id,
                x.Name,
                x.Code
            }).OrderBy(x => x.Name);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        [AccessCode("ProfileAudit")]
        public ActionResult ProfileAudit()
        {
            ViewBag.Title = "Reporte de Perfiles";
            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            LogAccion.Dao.AddLog("SecurityAccess"
                //el usuario X a navegado a Y
                , Current.User.Name + ";#;" + acceso
                );

            return View();
        }

        [AccessCode("ProfileAudit")]
        public ActionResult ProfilesAudit()
        {

            var profilesAcces = new List<ProfileAccess>();
            var profilesAccesFinal = new List<ProfileAccess>();
            profilesAcces = ProfileAccess.Dao.GetAll();
            profilesAcces.LoadRelation(s => s.Profile);
            profilesAcces.LoadRelation(s => s.Access);
            foreach (ProfileAccess access in profilesAcces)
            {
                var accessObject = new Access();
                if (access.Access.Name == null)
                {
                    accessObject = Access.Dao.Get(access.Access.Id);
                    access.Access.Name = accessObject.Name;
                }
            }
            var gridData = profilesAcces.Select(x => new
            {
                x.Id,
                ProfileName = x.Profile.Name,
                AccesName = GetAccessTreeNavigation(x.Access)
            }).OrderBy(x => x.ProfileName);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        private static IEnumerable<LogActionDetailModel> GetLogAccionDetail(LogAccion objLogAccion)
        {
            var objDetail = new List<LogActionDetailModel>();

            if (objLogAccion.DynamicDataSet == null) return objDetail;
            var dataSet = DynamicDataSet.Dao.Get(objLogAccion.DynamicDataSet.Id);
            if (dataSet == null) return objDetail;
            var accion = Accion.Dao.Get(objLogAccion.Accion.Id);

            switch (accion.Code.Trim())
            {

                case "UserProfilesEdit":
                    if (objLogAccion.AccionToId != null)
                    {
                        var profileEdit = GetProfiles(dataSet.DynamicDataSetRows, objLogAccion.AccionToId.Value);
                        var itvDetailAccionPEdit = new LogActionDetailModel
                        {
                            StructDescription = "Profiles",
                            OldValue = profileEdit[1],
                            Value = profileEdit[0]
                        };
                        objDetail.Add(itvDetailAccionPEdit);
                    }
                    break;

                case "ProfileAccessEdit":
                    if (objLogAccion.AccionToId != null)
                    {
                        var treeEdit = GetAccessTree(dataSet.DynamicDataSetRows, objLogAccion.AccionToId.Value);
                        var itvDetailAccionAEdit = new LogActionDetailModel
                        {
                            StructDescription = "Access",
                            OldValue = (treeEdit[1] != null) ? treeEdit[1] : "",
                            Value = treeEdit[0]
                        };
                        objDetail.Add(itvDetailAccionAEdit);
                    }
                    break;
                case "UserChangeState":
                    objDetail.AddRange(dataSet.DynamicDataSetRows.SelectMany(it0 => it0.Datas, (it0, it1) => GetDetailAccion(it1, true)).Where(w => w.OldValue != null));
                    break;
                case "ProfileChangeState":
                    objDetail.AddRange(dataSet.DynamicDataSetRows.SelectMany(it0 => it0.Datas, (it0, it1) => GetDetailAccion(it1, true)).Where(w => w.OldValue != null));
                    break;
                default:
                    objDetail.AddRange(dataSet.DynamicDataSetRows.SelectMany(it0 => it0.Datas, (it0, it1) => GetDetailAccion(it1, false)).Where(w => w.OldValue != null));
                    break;
            }

            return objDetail;
        }
        private static string GetAccessTreeNavigation(Access access)
        {
            var accessTree = "";

            try
            {
                if (access != null && !access.IsLoad) access = Access.Dao.Get(access.Id);
                if (access?.Id > 0)
                {
                    if (access.TreeLevel == 1)
                    {
                        accessTree = access.Path;
                    }
                    if (access.TreeLevel == 2)
                    {
                        accessTree = access.Path + "/" + access.Name;
                    }
                    if (access.TreeLevel == 3)
                    {
                        if (access.Parent != null)
                        {
                            accessTree = access.Path + "/" + Access.Dao.Get(access.Parent.Id)?.Name + "/" + access.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            return accessTree;
        }
        private static List<string> GetAccessTree(IEnumerable<DynamicDataSetRow> dataSetRows, long accionToId)
        {
            var asTree = new List<string>();
            var accessTreeAñadidos = new List<string>();
            var accessTreeEliminados = new List<string>();
            var currentValue = new List<string>();

            try
            {
                foreach (var it0 in dataSetRows)
                {
                    var accionType = it0.Accion.Code;
                    var access = Access.Dao.Get(it0.UniqueId);
                    if (access.Id <= 0) continue;
                    switch (accionType)
                    {
                        case "Insert":
                            accessTreeAñadidos.Add(GetAccessTreeNavigation(access));
                            break;
                        case "Delete":
                            accessTreeEliminados.Add(GetAccessTreeNavigation(access));
                            break;
                    }
                }

                if (accionToId > 0)
                {
                    currentValue = DNF.Security.Bussines.Profile.Dao.Get(accionToId)?.ProfileAccess.Select(p => GetAccessTreeNavigation(p.Access)).ToList();
                }

                if (currentValue != null)
                {
                    currentValue.Sort();
                    var oldValueTmp = currentValue.Union(accessTreeEliminados);
                    var oldValue = oldValueTmp.Except(accessTreeAñadidos);
                    asTree.Add(currentValue.ToJoin("\n"));
                    asTree.Add(oldValue.ToJoin("\n"));
                }
            }
            catch
            {
                // ignored
            }

            return asTree;
        }
        private static List<string> GetProfiles(IEnumerable<DynamicDataSetRow> dataSetRows, long accionToId)
        {
            var asProfile = new List<string>();
            var profilesAñadidos = new List<string>();
            var profilesEliminados = new List<string>();
            var currentValue = new List<string>();

            try
            {
                foreach (var it0 in dataSetRows)
                {
                    var accionType = it0.Accion.Code;
                    var profile = DNF.Security.Bussines.Profile.Dao.Get(it0.UniqueId);
                    if (profile.Id > 0)
                    {
                        switch (accionType)
                        {
                            case "Insert":
                                profilesAñadidos.Add(profile.Name);
                                break;
                            case "Delete":
                                profilesEliminados.Add(profile.Name);
                                break;
                        }
                    }
                }

                if (accionToId > 0)
                {
                    currentValue = DNF.Security.Bussines.User.Dao.Get(accionToId)?.Profiles.Select(p => p.Name).ToList();
                }

                if (currentValue != null)
                {
                    var oldValueTmp = currentValue.Union(profilesEliminados);
                    var oldValue = oldValueTmp.Except(profilesAñadidos);
                    asProfile.Add(currentValue.ToJoin("\n"));
                    asProfile.Add(oldValue.ToJoin("\n"));
                }
            }
            catch
            {
                // ignored
            }

            return asProfile;
        }
        private static string GetStateName(string stateTypeId)
        {
            var nameCode = "";
            try
            {
                if (stateTypeId != null)
                {
                    var type = Type.Dao.Get(Convert.ToInt64(stateTypeId));
                    if (type != null)
                        nameCode = type.Name;
                }
            }
            catch
            {
                // ignored
            }

            return nameCode;
        }
        private static LogActionDetailModel GetDetailAccion(DynamicDataSetData dataSetData, bool state)
        {
            var itvDetailAccion = new LogActionDetailModel();

            var objStruct = Struct.Dao.Get(dataSetData.Struct.Id);
            if (state)
            {
                if ((objStruct.Name == "State_Type_Id"))
                {
                    itvDetailAccion.StructDescription = objStruct.Description ??
                                                        objStruct.Name.Split(Convert.ToChar("_"))[0];
                    itvDetailAccion.OldValue = GetStateName(dataSetData.OldValue);
                    itvDetailAccion.Value = GetStateName(dataSetData.Value);
                }
            }
            else
            {
                if ((objStruct.Name != "State_Type_Id"))
                {
                    itvDetailAccion.StructDescription = objStruct.Description ?? objStruct.Name.Split(Convert.ToChar("_"))[0];
                    itvDetailAccion.OldValue = dataSetData.OldValue;
                    itvDetailAccion.Value = dataSetData.Value;
                }
            }

            return itvDetailAccion;
        }

        [AccessCode("AccessManagement")]
        public ViewResult AccessManagement()
        {
            return View();
        }

        [AccessCode("AccessManagement")]
        public PartialViewResult AccessData(long id)
        {
            var access = Access.Dao.Get(id);

            return PartialView(access ?? new Access());
        }

        [AccessCode("AccessEdit")]
        public ActionResult SaveAccessData(Access access)
        {
            var mensageResponse = "";
            
            var anyError = false;
            try
            {
                if (access == null)
                    throw new ApplicationException(Res.res.invalidModel);
                if (string.IsNullOrEmpty(access.Code))
                    throw new ApplicationException(Res.res.nonEmptyCodeError);

                var findeDuplicateCode = Access.Dao.GetByCode(access.Code);
                if (findeDuplicateCode != null && findeDuplicateCode.Id != access.Id)
                    throw new ApplicationException(string.Format(Res.res.nonDuplicateCodeError, findeDuplicateCode.Name));

                Access.Dao.Save(access);

                mensageResponse = Res.res.savedSuccessfully;
            }
            catch (ApplicationException ex)
            {
                mensageResponse = ex.Message;
                anyError = true;
            }
            catch (Exception ex)
            {
                mensageResponse = Res.res.error + ": " + ex.Message;
                anyError = true;
            }

            return Json(new {
                mensageResponse,
                anyError,
                access
            }, JsonRequestBehavior.AllowGet);
        }

        [AccessCode("AccessEdit")]
        public ActionResult SaveAccessTreeOrder(ICollection<Access> accesses)
        {
            var mensageResponse = "";            
            var anyError = false;

            try
            {
                var dicCurrentAcces = Access.Dao.Get(accesses.Select(x => x.Id)).ToDictionary();
                foreach (var access in accesses)
                {
                    var currentAccess = dicCurrentAcces[access.Id];
                    currentAccess.Parent = access.Parent;
                    currentAccess.Posicion = access.Posicion;
                    currentAccess.Save();
                }


                mensageResponse = Res.res.savedSuccessfully;
            }
            catch (ApplicationException ex)
            {
                mensageResponse = ex.Message;
                anyError = true;
            }
            catch (Exception ex)
            {
                mensageResponse = Res.res.error + ": " + ex.Message;
                anyError = true;
            }

            return Json(new
            {
                mensageResponse = mensageResponse,
                anyError = anyError
            }, JsonRequestBehavior.AllowGet);
        }

        [AccessCode("AccessEdit")]
        public ActionResult DeleteAccess(Access access)
        {
            var mensageResponse = "";
            var anyError = false;

            try
            {
                var acs = Access.Dao.Get(access.Id);
                Access.Dao.Delete(acs);

                mensageResponse = Res.res.deletedSuccess;
            }
            catch (ApplicationException ex)
            {
                mensageResponse = ex.Message;
                anyError = true;
            }
            catch (Exception ex)
            {
                mensageResponse = Res.res.error + ": " + ex.Message;
                anyError = true;
            }

            return Json(new
            {
                mensageResponse = mensageResponse,
                anyError = anyError
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

#region Response Json
//json que devuelvo desde el controller hacia la vista
public class ResponseAuditJson
{
    public List<GroupHeaderAudit> groupHeader { get; set; }
    public List<ColJqGridAudit> ColModel { get; set; }
    public List<string> ColNames { get; set; }
    public List<Dictionary<string, string>> rows { get; set; }
    public Dictionary<string, string> userdata { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int records { get; set; }
    public int total { get; set; }
}



public class ColJqGridAudit
{
    public string Name { get; set; }
    public string index { get; set; }
    public int width { get; set; }
}



public class GroupHeaderAudit
{
    public string startColumnName { get; set; }
    public int numberOfColumns { get; set; }
    public string titleText { get; set; }
}

#endregion
