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
    public class CommonController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();


        [HttpPost]
        [Route("api/SearchService")]
        public IHttpActionResult SearchService(SearchServiceViewModel serviceModel)
        {
            bool success = false;
            var messsage = "";
            List<SearchServiceModel> model = new List<SearchServiceModel>();
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                    var er = errors.Errors[0].ErrorMessage;
                    return Ok(new
                    {
                        Success = false,
                        Message = er,
                        RUrl = "errorpage"
                    });
                }
                else
                {
                    var query = context.Services.Where(x => x.ServiceName.ToLower().Contains(serviceModel.ServiceName.ToLower()) && x.ParentID != null && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new SearchServiceModel
                    {
                        ServiceId = x.ID,
                        ServiceName = x.ServiceName
                    }).ToList();
                    if (model.Count > 0)
                    {
                        success = true;
                        messsage = "Get List";
                    }
                    else
                    {
                        success = false;
                        messsage = "No record found!";
                    }
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/SearchCity")]
        public IHttpActionResult SearchCity(CityViewModel cityModel)
        {
            bool success = false;
            var messsage = "";
            List<SearchCityModel> model = new List<SearchCityModel>();
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                    var er = errors.Errors[0].ErrorMessage;
                    return Ok(new
                    {
                        Success = false,
                        Message = er,
                        RUrl = "errorpage"
                    });
                }
                else
                {
                    var query = context.LocationMasters.Where(x => x.City.ToLower().Contains(cityModel.City.ToLower()) && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new SearchCityModel
                    {
                        ID = x.ID,
                        City = x.City,
                        Latitude = x.Latitude,
                        Longtitude = x.Longtitude,
                        Zipcode = x.Zipcode
                    }).Distinct().ToList();
                    if (model.Count > 0)
                    {
                        success = true;
                        messsage = "Get List";
                    }
                    else
                    {
                        success = false;
                        messsage = "No record found!";
                    }
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/ServicesNearMe")]
        public IHttpActionResult ServicesNearMe(ServicesNearMeViewModel serviceCityViewModel)
        {
            bool success = false;
            var messsage = "";
            int totalServiceCount = 0;
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                    var er = errors.Errors[0].ErrorMessage;
                    return Ok(new
                    {
                        Success = false,
                        Message = er,
                        RUrl = "errorpage"
                    });
                }
                else
                {
                    totalServiceCount = context.UserServices.Where(x => x.ServiceID == serviceCityViewModel.ServiceId && x.AspNetUser.UserAddresses.Where(y => y.City.ToLower() == serviceCityViewModel.City.ToLower()).Count() > 0).Count();
                    success = true;
                    messsage = "Get List";
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                TotalServiceCount = totalServiceCount
            });
        }

        [HttpPost]
        [Route("api/GetQuestionTypeList")]
        public IHttpActionResult GetQuestionTypeList()
        {
            bool success = false;
            var messsage = "";
            var model = new List<QuestionTypeViewModel>();
            try
            {
                model = context.QuestionTypes.Select(x => new QuestionTypeViewModel
                {
                    TypeName = x.TypeName,
                    UniqueCode = x.UniqueCode
                }).ToList();
                    success = true;
                    messsage = "Get List";
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                Result = model
            });
        }

        [Route("api/GetCountryList")]
        [HttpPost]
        public IHttpActionResult GetCountryList()
        {
            bool success = false;
            var messsage = "";
            var model = new List<YearMonthModel>();
            try
            {
                var query = context.Countries;
                model = query.Select(x => new YearMonthModel
                {
                    Text = x.CountryName,
                    Value = x.ID.ToString()
                }).ToList();

                success = true;
                messsage = "Get List";
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                Result = model
            });
        }

        [Route("api/GetYearList")]
        [HttpPost]
        public IHttpActionResult GetYearList()
        {
            //var model = Year();
            var model = Year().Select(x => new YearMonthModel
            {
                Text = x.ToString(),
                Value = x.ToString()
            }).ToList();
            return Ok(new
            {
                Success = true,
                Result = model
            });
        }

        [Route("api/GetMonthList")]
        [HttpPost]
        public IHttpActionResult GetMonthList()
        {
            var model = Month().Select(x => new YearMonthModel
            {
                Text = x.ToString(),
                Value = x.ToString()
            }).ToList();
            return Ok(new
            {
                Success = true,
                Result = model
            });
        }

        [Route("api/GetNextYearList")]
        [HttpPost]
        public IHttpActionResult GetNextList()
        {
            var model = YearNext().Select(x => new YearMonthModel
            {
                Text = x.ToString(),
                Value = x.ToString()
            }).ToList();
            return Ok(new
            {
                Success = true,
                Result = model
            });
        }
        public static List<string> Month()
        {
            List<string> month = new List<string>();
            string[] localizedMonths = Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames;
            string[] invariantMonths = DateTimeFormatInfo.InvariantInfo.MonthNames;

            for (int i = 0; i < 12; i++)
            {
                month.Add(localizedMonths[i]);
            }
            return month;
        }
        public static List<int> Year()
        {
            List<int> year = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= 1968; i--)
            {
                year.Add(i);
            }
            return year;
        }
        public static List<int> YearNext()
        {
            List<int> year = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i <= 2030; i++)
            {
                year.Add(i);
            }
            return year;
        }      
    }
}
