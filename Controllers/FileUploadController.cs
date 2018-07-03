using QuiGigAPI.DataBase;
using QuiGigAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuiGigAPI.Controllers
{
    public class FileUploadController : Controller
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();

        [HttpPost]
        [Route("UploadProfileImage")]
        public ActionResult UploadProfileImage(UploadFileViewModel model)
        {
            var path = "";
            bool success = false;
            var message = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif" };
                        if (AllowedFileExtensions.Contains(extension))
                        {
                            var pathExist = HttpContext.Server.MapPath("~/Content/images/UserProfileImages/");
                            if (!Directory.Exists(pathExist))
                                Directory.CreateDirectory(pathExist);

                            if (postedFile != null && postedFile.ContentLength > 0)
                            {                                
                                try
                                {
                                    var userDetail = context.UserDetails.Where(x => x.UserID == model.UserId).FirstOrDefault();
                                    if (userDetail != null)
                                    {
                                        var imgName = DateTime.Now.Ticks.ToString() + postedFile.FileName;
                                        var filePath = pathExist + imgName;
                                        postedFile.SaveAs(filePath);
                                        path = "/Content/images/UserProfileImages/" + imgName;
                                        userDetail.ProfilePic = path;
                                        context.SaveChanges();
                                        string targetPath = @"C:\inetpub\wwwroot\Content\images\UserProfileImages";
                                        string sourceFile = Path.Combine(filePath, imgName);
                                        string destFile = Path.Combine(targetPath, imgName);
                                        System.IO.File.Copy(filePath, destFile, true);
                                        System.IO.File.Delete(filePath);
                                        success = true;
                                        message = "Upload Successfully.";
                                    }
                                    else
                                    {
                                        success = false;
                                        message = "User not exist.";
                                    }
                                }
                                catch(Exception ex)
                                {
                                    message = ex.Message;
                                }                               
                            }
                        }
                        else
                        {
                            success = false;
                            message = "Please Upload image of type";
                        }
                    }
                }
            }
            return Json(new { Status = success, Message = message, Path = path }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UploadPortfolioImage")]
        public ActionResult UploadPortfolioImage(UploadFileViewModel model)
        {
            var path = "";
            bool success = false;
            var message = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Files.Count > 0)
                {                   
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif" };
                        if (AllowedFileExtensions.Contains(extension))
                        {  
                            var pathExist = HttpContext.Server.MapPath("~/Content/images/PortfolioImages/");
                            if (!Directory.Exists(pathExist))
                                Directory.CreateDirectory(pathExist);

                            if (postedFile != null && postedFile.ContentLength > 0)
                            {
                                try
                                {
                                    var imgName = DateTime.Now.Ticks.ToString() + postedFile.FileName;
                                    var filePath = pathExist + imgName;
                                    postedFile.SaveAs(filePath);
                                    path = "/Content/images/PortfolioImages/" + imgName;
                                    string targetPath = @"C:\inetpub\wwwroot\Content\images\PortfolioImages";
                                    string sourceFile = Path.Combine(filePath, imgName);
                                    string destFile = Path.Combine(targetPath, imgName);
                                    System.IO.File.Copy(filePath, destFile, true);
                                    System.IO.File.Delete(filePath);
                                    success = true;
                                    message = "Upload Successfully.";
                                }
                                catch (Exception ex)
                                {
                                    message = ex.Message;
                                }
                            }
                        }
                        else
                        {
                            success = true;
                            message = "Please Upload image of type";
                        }
                    }
                }
            }
            return Json(new { Status = success, Message = message, Path = path }, JsonRequestBehavior.AllowGet);
        }
    }
}