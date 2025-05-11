using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.ElasticEntity;
using Microsoft.Ajax.Utilities;
//using TikaOnDotNet;

namespace DepositControl.Areas.Bcri.Controllers
{
    [Authenticated]
    public class DocumentSearchController : Controller
    {
        /*
        [HttpPost]
        public ActionResult GetSearchPage(Core.Bussines.Search search)
        {
            search.DoSearch();
            return View("Contracts", search.ResultSearch);
        }

        [HttpGet]
        public ActionResult Index (global::Bcri.Core.Bussines.Search search)
         {
             search = search ?? new global::Bcri.Core.Bussines.Search();  
             search.Page = 1; 
             search.DoSearch();
             return View(search);

         }

        [HttpPost]
        public ActionResult Index(string toSearch)
        {
            var search = new global::Bcri.Core.Bussines.Search();
             search.Page = 1;
            if(!toSearch.IsNullOrWhiteSpace())
                search.Query = toSearch;
            search.DoSearch(); 
            return View(search); 
        }

        public ActionResult SeeContract(string contract)
        {
            var file = new DNF.Attachmens.Bussines.File(); 

            return View();

        }

        // Receive: a path to a .doc .pdf .txt .etc
        // returns: content as a string

        private TextExtractor _cut;
        public string ConvertToText(byte[] path)
        {  
            _cut = new TextExtractor();
            var textExtractionResult = _cut.Extract(path);
            return textExtractionResult.Text; 
        }

        // Receives a path
        // Adds it to elastic
        // Returns true if it was a succes
        public bool AddToElastic(byte[] file,string name)
        {
           
           var contrato = new Document();
           contrato.DocumentFullText = ConvertToText(file);
            if (contrato.DocumentFullText == null)
            {
                return false;
            }
            contrato.Name = Path.GetFileNameWithoutExtension(name);
            contrato.Date = DateTime.Now;
            contrato.FileId = AddToDb(file, name);
            //catch possible error
            if (contrato.FileId < 0) return false;

            contrato.Save(); 
            return true;
        }

        public long AddToDb(byte[] content, string name)
        { 
                var file = new DNF.Attachmens.Bussines.File();
                file.Name = name;
                file.Content = content; 
                //Añado el contet type manualmente
                file.DetectContentType();
            try
            {
                file.Save();
            } 
            catch (Exception ex)
            {
                return -1;
            }

            return file.Id;
        }

        public ActionResult SaveUploadedFile()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                     
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {  
                        MemoryStream target = new MemoryStream();
                        file.InputStream.CopyTo(target);
                        byte[] data = target.ToArray();  
                        isSavedSuccessfully = AddToElastic(data, fName); 
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
            {
                Response.Redirect("Index.cshtml");
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }
         
        public ActionResult Get(long id)
        {
            var document = DNF.Attachmens.Bussines.File.Dao.Get(id);
            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = document.Name,

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return  File(document.Content, string.IsNullOrEmpty(document.ContentType) ?
                                            "text/plain"
                                            : document.ContentType); 
        }
        */
    }
}


