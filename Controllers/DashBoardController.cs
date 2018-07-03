using QuiGigAPI.Common;
using QuiGigAPI.DataBase;
using QuiGigAPI.Models;
using QuiGigAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace QuiGigAPI.Controllers
{
    public class DashBoardController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();

        //[HttpPost]
        //[Route("api/MyBidListNew")]
        //public IHttpActionResult MyBidListNew(SearchOrderModel viewModel)
        //{
        //    bool success = false;
        //    var message = "";
        //    var model = new List<MyGigModel>();
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
        //        message = errors.Errors[0].ErrorMessage;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            var query = context.GetLatestOrderList(viewModel.UserId, viewModel.UserType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);                    
        //            model = query.Select(x => new MyGigModel
        //            {
        //                ID = x.ID,
        //                JobTitle = x.JobTitle,
        //                ReceivedBid = x.ReceivedBid,
        //                ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
        //                CreatedDate = x.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
        //                JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss")
        //            }).ToList();
        //            success = true;
        //            message = "Get List";
        //        }
        //        catch (Exception ex)
        //        {
        //            message = ex.Message;
        //        }
        //    }
        //    return Ok(new
        //    {
        //        Success = success,
        //        Message = message,
        //        Result = model
        //    });
        //}

        [HttpPost]
        [Route("api/MyGigsList")]
        public IHttpActionResult MyGigsList(SearchOrderModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<MyGigModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    if (viewModel.GigType == "Bidding")
                    {
                        var query = context.GetMyGigList(viewModel.UserId, viewModel.UserType, viewModel.GigType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);
                        model = query.Select(x => new MyGigModel
                        {
                            ID = x.ID,
                            JobTitle = x.JobTitle,
                            ReceivedBid = x.ReceivedBid,
                            ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss"),
                            CurrentProposalStatus = x.CurrentProposalStatus
                        }).ToList();
                        success = true;
                        message = "Get List";
                    }
                    if (viewModel.GigType == "Awarded")
                    {
                        var query = context.GetMyGigList(viewModel.UserId, viewModel.UserType, viewModel.GigType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);
                        model = query.Select(x => new MyGigModel
                        {
                            ID = x.ID,
                            JobTitle = x.JobTitle,
                            ReceivedBid = x.ReceivedBid,
                            ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss"),
                            CurrentProposalStatus = x.CurrentProposalStatus
                        }).ToList();
                        success = true;
                        message = "Get List";
                    }
                    if (viewModel.GigType == "Completed")
                    {
                        var query = context.GetMyGigList(viewModel.UserId, viewModel.UserType, viewModel.GigType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);
                        model = query.Select(x => new MyGigModel
                        {
                            ID = x.ID,
                            JobTitle = x.JobTitle,
                            ReceivedBid = x.ReceivedBid,
                            ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss"),
                            CurrentProposalStatus = x.CurrentProposalStatus
                        }).ToList();
                        success = true;
                        message = "Get List";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/MyBidList")]
        public IHttpActionResult MyBidList(MyBidListModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<MyBidModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    if (viewModel.BidType == "Bidding")
                    {
                        var query = context.GetMyBidList(viewModel.UserId, viewModel.UserType, viewModel.BidType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);
                        model = query.Select(x => new MyBidModel
                        {
                            JobID = x.ID,
                            JobTitle = x.JobTitle,
                            ProposalId = x.ProposalId,
                            ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss"),
                            Amount = x.Amount
                        }).ToList();
                        success = true;
                        message = "Get List";
                    }
                    if (viewModel.BidType == "Awarded")
                    {
                        var query = context.GetMyBidList(viewModel.UserId, viewModel.UserType, viewModel.BidType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);
                        model = query.Select(x => new MyBidModel
                        {
                            JobID = x.ID,
                            JobTitle = x.JobTitle,
                            ProposalId = x.ProposalId,
                            ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss"),
                            Amount = x.Amount
                        }).ToList();
                        success = true;
                        message = "Get List";
                    }
                    if (viewModel.BidType == "Completed")
                    {
                        var query = context.GetMyBidList(viewModel.UserId, viewModel.UserType, viewModel.BidType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);
                        model = query.Select(x => new MyBidModel
                        {
                            JobID = x.ID,
                            JobTitle = x.JobTitle,
                            ProposalId = x.ProposalId,
                            ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss"),
                            Amount = x.Amount
                        }).ToList();
                        success = true;
                        message = "Get List";
                    }     
                    if(viewModel.BidType == "New")
                    {
                        try
                        {
                            var query = context.GetLatestOrderList(viewModel.UserId, viewModel.UserType, viewModel.SearchKeyword == null ? "" : viewModel.SearchKeyword);
                            model = query.Select(x => new MyBidModel
                            {
                                JobID = x.ID,
                                JobTitle = x.JobTitle,
                                ReceivedBid = x.ReceivedBid,
                                ServicePic = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(x.ServicePic)) ? x.ServicePic : "/Content/images/services-bg.jpg",
                                CreatedDate = x.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                                JobExpireDate = Convert.ToDateTime(x.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss")
                            }).ToList();
                            success = true;
                            message = "Get List";
                        }
                        catch (Exception ex)
                        {
                            message = ex.Message;
                        }
                    }              
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        
    }
}
