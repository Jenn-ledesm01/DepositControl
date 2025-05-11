using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Bcri.Core.Bussines;
using DNF.Security.Bussines;


namespace DepositControl.Areas.Bcri.Controllers
{

    public class GridCustomViewController : Controller
    {
        public ActionResult Index(string gridCode)
        {

            var grid = Grid.Dao.GetByCode(gridCode);
            List<GridCustomView> customViews = GridCustomView.Dao.GetForUser(grid);
            GridCustomViewDefault ItemDefault = GetDefault(gridCode);
            List<SelectListItem> ddlItems = customViews.OrderBy(x => x.Name).ToListItem("(default)");
            if (ItemDefault != null)
            {
                ddlItems = ddlItems.Select(
                    n => new SelectListItem()
                    {
                        Text = n.Text,
                        Value = n.Value,
                        Selected = (Convert.ToInt32(n.Value) == ItemDefault.GridCustomView.Id)
                    }).ToList();
            }
            ViewBag.GridCustomViews = ddlItems;
            ViewBag.GridCode = gridCode;
            return View();
        }

        public ActionResult Load(int id, string gridCode)
        {
            if (gridCode != "")
            {
                GridCustomViewDefault ItemDefault = GetDefault(gridCode);
                if (ItemDefault != null)
                    return Json(ItemDefault.GridCustomView.State, JsonRequestBehavior.AllowGet);
            }

            if (id != 0)
                return Json(GridCustomView.Dao.Get(id).State, JsonRequestBehavior.AllowGet);


            return Json("", JsonRequestBehavior.AllowGet);
        }
        public string Save(int id, string name, string gridCode)
        {
            string state = Request.Form[0].Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[2];
            var grid = Grid.Dao.GetByCode(gridCode);

            var customView = (id == 0)
                                ? new GridCustomView()
                                : GridCustomView.Dao.Get(id);
            customView.Name = name;
            customView.Grid = grid;
            customView.State = state;
            customView.Owner = Current.User;
            customView.Save();
            return customView.Id.ToString();
        }
        public bool Delete(int id)
        {
            var customView = GridCustomView.Dao.Get(id);
            if (customView?.Owner.Id != Current.User.Id)
            {
                return false;
                // completar para que borre el compartido y no el item
            }
            customView.Delete();
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////// Share and defaults ////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult ChooseProfiles(long gridCustomViewId, string entity)
        {
            List<CheckBox> Items;
            switch (entity)
            {
                case "share":
                    List<GridCustomViewShared> ProfilesShared = GridCustomViewShared.Dao.GetByFilter(new { GridCustomView_Id = gridCustomViewId });
                    Items = DNF.Security.Bussines.Profile.Dao.GetAll()
                    .Select(n => new CheckBox()
                    {
                        Text = n.Name,
                        ID = n.Id.ToString(),
                        Checked = ProfilesShared.Any(m => n.Id == m.Profile.Id)
                    }).ToList();
                    return View(Items);
                case "default":
                    List<GridCustomViewDefault> ProfilesDefault = GridCustomViewDefault.Dao.GetByFilter(new { GridCustomView_Id = gridCustomViewId }).Where(n => n.Profile != null).ToList();
                    try
                    {
                        Items = DNF.Security.Bussines.Profile.Dao.GetAll()
                           .Select(n => new CheckBox()
                           {
                               Text = n.Name,
                               ID = n.Id.ToString(),
                               Checked = ProfilesDefault.Any(m => n.Id == m.Profile.Id)
                           }).ToList();
                    }
                    catch (Exception e)
                    {


                        throw;
                    }

                    return View(Items);

                default:
                    return View(new List<CheckBox>());
            }
        }
        public GridCustomViewDefault GetDefault(String gridCode)
        {
            GridCustomViewDefault ItemDefault;
            ItemDefault = GridCustomViewDefault.Dao.GetForUser(gridCode);
            if (ItemDefault != null)
            {
                ItemDefault.GridCustomView = GridCustomView.Dao.Get(ItemDefault.GridCustomView.Id);
                if (ItemDefault.GridCustomView == null)
                    return null;
                return ItemDefault;
            }
            return null;
        }
        [AccessCode("GridCustomViewDefaultMe")]
        public string SetAsDefaultMe(Int32 Index)
        {
            GridCustomViewDefault.Dao.SetAsDefaultMe(Index);
            return "";
        }
        [AccessCode("GridCustomViewSetDefaultFor")]
        public string SetAsDefaultForUsers(Int32 gridCustomViewId, String[] ProfilesChecked)
        {
            string NewId = GridCustomViewDefault.Dao.SetAsDefaultForUsers(gridCustomViewId, ProfilesChecked ?? new string[] { });
            if (NewId == "0")
                return "";
            else
                return NewId + "!" + GridCustomView.Dao.Get(Convert.ToInt64(NewId)).Name;
        }
        //[AccessCode("CustomViewSetDefault")]
        [AccessCode("GridCustomViewShare")]
        public void Share(Int32 gridCustomViewId, String[] ProfilesChecked)
        {
            GridCustomViewShared.Dao.Share(gridCustomViewId, ProfilesChecked ?? new string[] { });
        }

        public string Rename(Int32 gridCustomViewId, String Name)
        {
            //string state = Request.Form[0].Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[2];
            GridCustomView CustomView;
            CustomView = GridCustomView.Dao.Get(gridCustomViewId);
            if (CustomView.Owner.Id != Current.User.Id)
                return "404";
            CustomView.Name = Name;
            CustomView.Save();
            return "The custom view was renamed.";
        }
        public String GetOwner(Int32 gridCustomViewId)
        {
            GridCustomView GridView = GridCustomView.Dao.GetByFilter(new { Id = gridCustomViewId }).First();
            if (GridView.Owner.Id == Current.User.Id)
                return "";
            else
                return DNF.Security.Bussines.User.Dao.Get(GridView.Owner.Id).Name;
        }

    }

}

