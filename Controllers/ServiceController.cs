using QuiGig.Models;
using QuiGigAPI.Common;
using QuiGigAPI.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuiGigAPI.Controllers
{
    public class ServiceController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();

        [Route("api/GetStateList")]
        [HttpPost]
        public IHttpActionResult GetStateList()
        {
            var model = new List<StateModel>();
            bool status = false;
            var message = "";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    model = context.StateMasters.Where(x => x.CountryID == 231).Select(x => new StateModel
                    {
                        StateName = x.Name,
                        StateCode = x.StateCode,
                        ID = x.ID
                    }).Distinct().OrderBy(x => x.StateName).ToList();
                    status = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message,
                Result = model
            });
        }

        [Route("api/GetCityList")]
        [HttpPost]
        public IHttpActionResult GetCityList(StateViewModel viewModel)
        {
            var model = new List<CityModel>();
            bool status = false;
            var message = "";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    model = context.CityMasters.Where(x => x.StateID == viewModel.StateId).Select(x => new CityModel
                    {
                        CityName = x.Name,
                        ID = x.ID,
                        IsSelected = x.UserAddresses.Where(us => us.UserID == viewModel.UserId && us.IsActive == true && us.IsDelete == false && us.StateId == x.StateID && us.CityId == x.ID).Count() > 0 ? true : false
                    }).Distinct().OrderBy(x => x.CityName).ToList();
                    status = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message,
                Result = model
            });
        }

        [Route("api/GetCategoryList")]
        [HttpPost]
        public IHttpActionResult GetCategoryList(ServiceViewModel viewModel)
        {
            var model = new ExploreCategoriesModel();
            bool status = false;
            var message = "";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(viewModel.ServiceName))
                    {
                        model.ParentService = viewModel.ServiceName;
                        model.ParentServiceList = context.Services.Where(x => x.ParentID == null && x.ServiceName.ToLower() == viewModel.ServiceName.ToLower() && x.IsActive == true && x.IsDelete == false).Select(x => new ParentServiceListModel
                        {
                            ID = x.ID,
                            ServiceName = x.ServiceName
                        }).Distinct().OrderByDescending(x => x.ID).ToList();
                        long parentId = model.ParentServiceList[0].ID;
                        model.ServiceList = context.Services.Where(x => x.ParentID == parentId && x.IsActive == true && x.IsDelete == false).Select(x => new ParentServiceListModel
                        {
                            ID = x.ID,
                            ServiceName = x.ServiceName
                        }).Distinct().ToList();
                    }
                    else
                    {
                        model.ParentServiceList = context.Services.Where(x => x.ParentID == null && x.IsActive == true && x.IsDelete == false).Select(x => new ParentServiceListModel
                        {
                            ID = x.ID,
                            ServiceName = x.ServiceName
                        }).Distinct().OrderByDescending(x => x.ID).ToList();
                        model.ParentService = model.ParentServiceList[0].ServiceName;
                        long parentId = model.ParentServiceList[0].ID;
                        model.ServiceList = context.Services.Where(x => x.ParentID == parentId && x.IsActive == true && x.IsDelete == false).Select(x => new ParentServiceListModel
                        {
                            ID = x.ID,
                            ServiceName = x.ServiceName
                        }).Distinct().ToList();
                    }
                    status = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message,
                Result = model
            });
        }

        [Route("api/CityDetail")]
        [HttpPost]
        public IHttpActionResult CityDetail(CityViewModel viewModel)
        {
            var model = new List<ExploreCitiesDetailModel>();
            var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
            bool status = false;
            var msg = "";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                msg = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    //model = context.GetNearService(viewModel.CityName).Select(x => new ExploreCitiesDetailModel
                    //{
                    //    ServiceID = x.ServiceID,
                    //    ServiceName = x.ServiceName,
                    //    Url = x.Url,
                    //    ServiceIcon = x.ServiceIcon == null ? path + "/Content/images/no-service-icon.png" : path + x.ServiceIcon,
                    //}).Distinct().ToList();
                    status = true;
                    msg = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    msg = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = msg,
                Result = model
            });
        }

        [Route("api/ServiceDetail")]
        [HttpPost]
        public IHttpActionResult ServiceDetail(CityServiceViewModel viewModel)
        {
            var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
            var model = new ExploreServiceDetailModel();
            bool status = false;
            var msg = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                msg = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(viewModel.CityName) && viewModel.ServiceId > 0)
                    {
                        var serviceDetail = context.Services.Where(x => x.ID == viewModel.ServiceId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                        if (serviceDetail != null)
                        {
                            model.ServiceName = serviceDetail.ServiceName;
                            var location = context.CityServices.Where(x => x.CityName.ToLower().Trim() == viewModel.CityName.ToLower().Trim() && x.ServiceName.ToLower() == serviceDetail.ServiceName.ToLower() && x.IsActive == true).FirstOrDefault();
                            model.CityDescription = location.ServiceCityContent;
                            model.PopularSPList = context.GetPopularSPById(viewModel.ServiceId, viewModel.CityName).Select(x => new UserDetailModel
                            {
                                UserName = x.UserName,
                                ProfilePic = x.ProfilePic == null ? path + "/Content/images/profile-progress-avtar.svg" : path + x.ProfilePic,
                                CreatedDate = Util.TimeAgo(Convert.ToDateTime(x.CreatedDate)),
                                HiredCount = x.HiredCount
                            }).ToList();
                            model.SPAvailableOtherCity = context.GetOtherSPCityList(viewModel.ServiceId, viewModel.CityName).Select(x => new OtherSPCityModel
                            {
                                CityName = x.ToString()
                            }).ToList();
                            if (model.SPAvailableOtherCity.Count() == 0)
                            {
                                model.SPAvailableOtherCity.Insert(0, new OtherSPCityModel { CityName = "" });
                            }
                            status = true;
                            msg = "Get List";
                        }
                        else
                        {
                            status = false;
                            msg = "Result is not exist";
                        }
                    }
                    else
                    {
                        status = false;
                        msg = "Please send state id and city name";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    msg = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = msg,
                Result = model
            });
        }

        [Route("api/HomePageService")]
        [HttpPost]
        public IHttpActionResult HomePageService()
        {
            var model = new List<HomePageServiceListModel>();
            bool status = false;
            var message = "";
            try
            {
                List<long> productIdList = new List<long> { 36, 37, 286, 67, 251, 804, 403 };
                var query = context.Services.Where(x => x.ParentID == null && productIdList.Contains(x.ID) && x.IsActive == true && x.IsDelete == false);

                model = query.Select(x => new HomePageServiceListModel
                {
                    ID = x.ID,
                    Name = x.ServiceName,
                    Icon = x.ServiceIcon,
                    OrderNo = x.OrderNo
                }).OrderBy(x => x.OrderNo).ToList();
                status = true;
                message = "Get List";
            }
            catch (Exception ex)
            {
                status = false;
                message = ex.Message;
            }
            return Ok(new
            {
                Success = status,
                Result = model,
                Message = message
            });
        }

        [Route("api/HomePageChildService")]
        [HttpPost]
        public IHttpActionResult HomePageChildService(HomePageViewModel viewModel)
        {
            var model = new List<HomePageChildServiceModel>();
            bool status = false;
            var message = "";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Services.Where(x => x.ParentID == viewModel.ParentId && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new HomePageChildServiceModel
                    {
                        ID = x.ID,
                        Name = x.ServiceName,
                        IsSelected = x.UserServices.Where(us => us.UserID == viewModel.UserId && us.IsActive == true).Count() > 0 ? true : false
                    }).ToList();
                    status = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Result = model,
                Message = message
            });
        }

        [Route("api/ChildServiceList")]
        [HttpGet]
        public IHttpActionResult ChildServiceList()
        {
            var model = new List<HomePageChildServiceModel>();
            bool status = false;
            var message = "";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Services.Where(x => x.ParentID != null && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new HomePageChildServiceModel
                    {
                        ID = x.ID,
                        Name = x.ServiceName                      
                    }).ToList();
                    status = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Result = model,
                Message = message
            });
        }


        [HttpGet]
        [Route("api/StateCityList")]
        public IHttpActionResult StateCityList()
        {
            var model = new List<SearchCityModel>();
            bool status = false;
            var message = "";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.CityMasters.Where(x=> x.StateMaster.CountryID == 231);
                    model = query.Select(x => new SearchCityModel
                    {
                        City = x.Name,
                        StateId = x.StateMaster.ID,
                        StateName = x.StateMaster.Name,
                        StateCode = x.StateMaster.StateCode,
                        CityWithCode = x.Name + ", " + x.StateMaster.StateCode
                    }).Distinct().ToList();
                    status = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Result = model,
                Message = message
            });
        }
    }
}

