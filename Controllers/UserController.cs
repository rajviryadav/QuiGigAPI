using QuiGigAPI.DataBase;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using QuiGigAPI.Models;
using QuiGigAPI.Common;
using QuiGigAPI.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;

namespace QuiGigAPI.Controllers
{
    public class UserController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpPost]
        [Route("api/EmailVerified")]
        public IHttpActionResult EmailVerified(UserViewModel user)
        {
            bool success = false;
            var message = "";
            bool isVerified = false;
            var model = new UserDetailModel();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var userExist = manager.FindById(user.UserId);
                    if (userExist != null)
                    {
                        isVerified = userExist.EmailConfirmed;
                        success = true;
                        message = "Verified";
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
                IsVerified = isVerified
            });
        }

        [HttpPost]
        [Route("api/GetUserDetailById")]
        public IHttpActionResult GetUserDetailById(UserViewModel user)
        {
            bool success = false;
            var message = "";
            int count = 0;
            var model = new UserDetailModel();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var signup = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.SIGNUP.ToString()).FirstOrDefault();
                    var emailVerification = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.EMAIL_VERIFICATION.ToString()).FirstOrDefault();
                    var profilePic = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.PROFILE_PIC.ToString()).FirstOrDefault();
                    var userDetail = context.UserDetails.Where(x => x.UserID == user.UserId).FirstOrDefault();
                    if (userDetail.ProfilePic != null)
                        count = Convert.ToInt32(signup.Value + emailVerification.Value + profilePic.Value);
                    else
                        count = Convert.ToInt32(signup.Value + emailVerification.Value);

                    model = context.GetUserDetailById(user.UserId).Select(x => new UserDetailModel
                    {
                        UserName = x.Name,
                        UserShortName = x.UserShortName,
                        ProfilePic = x.ProfilePic,
                        CurrentPlan = x.PlanName,
                        BidPoints = x.BidPoints,
                        ProfessionalDescription = x.ProfessionalDescription,
                        ProfessionalTitle = x.ProfessionalTitle,
                        ThumbsDown = Convert.ToInt32(x.ThumbsDown),
                        ThumbsUp = Convert.ToInt32(x.ThumbsUp),
                        Percentage = count
                    }).FirstOrDefault();
                    success = true;
                    message = "Get List";
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
        [Route("api/GetUserProfilePercentage")]
        public IHttpActionResult GetUserProfilePercentage(UserViewModel user)
        {
            bool success = false;
            int count = 0;
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
                    var signup = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.SIGNUP.ToString()).FirstOrDefault();
                    var emailVerification = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.EMAIL_VERIFICATION.ToString()).FirstOrDefault();
                    var profilePic = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.PROFILE_PIC.ToString()).FirstOrDefault();
                    var userDetail = context.UserDetails.Where(x => x.UserID == user.UserId).FirstOrDefault();
                    if (userDetail.ProfilePic != null)
                        count = Convert.ToInt32(signup.Value + emailVerification.Value + profilePic.Value);
                    else
                        count = Convert.ToInt32(signup.Value + emailVerification.Value);
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                }
            }
            return Ok(new
            {
                Success = success,
                Percentage = count
            });
        }       

        [Route("api/GetLocationList")]
        [HttpPost]
        public IHttpActionResult GetLocationList()
        {
            var model = new List<LocationModel>();
            bool status = false;
            try
            {
                var userId = User.Identity.GetUserId();
                model = context.StateMasters.Where(x => x.CountryID == 231).Select(x => new LocationModel
                {
                    StateName = x.Name,
                    StateCode = x.StateCode,
                    StateId = x.ID,
                    CityList = x.CityMasters.Select(y => new ProfileLocationCityVM
                    {
                        CityName = y.Name,
                        CityId = y.ID,
                        Selected = y.UserAddresses.Where(us => us.UserID == userId && us.IsActive == true && us.StateId == y.StateID && us.CityId == y.ID).Count() > 0 ? true : false
                    }).ToList()
                }).ToList();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Ok(new
            {
                Success = status,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/RemoveAllLocation")]
        public IHttpActionResult RemoveAllLocation(UserViewModel model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                var userId = User.Identity.GetUserId();
                var address = context.UserAddresses.Where(x => x.UserID == model.UserId).ToList();
                if (address != null)
                {
                    foreach (var item in address)
                        context.UserAddresses.Remove(item);
                    context.SaveChanges();
                    success = true;
                    messsage = "Delete successfully.";
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }

        [HttpPost]
        [Route("api/RemoveLocation")]
        public IHttpActionResult RemoveLocation(UserLocationDeleteModel model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                var address = context.UserAddresses.Where(x => x.CityId == model.CityId && x.UserID == model.UserID && x.IsActive == true && x.StateId == x.StateMaster.ID).FirstOrDefault();
                if (address != null)
                {
                    context.UserAddresses.Remove(address);
                    context.SaveChanges();
                    success = true;
                    messsage = "Delete successfully.";
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }

        [HttpPost]
        [Route("api/GetReviewRatingUserId")]
        public IHttpActionResult GetReviewRatingUserId(UserViewModel user)
        {
            bool success = false;
            var message = "";
            var model = new List<ReviewRatingModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    model = context.GetReviewRatingList(user.UserId).Select(x => new ReviewRatingModel
                    {
                        UserName = x.UserName,
                        UserImage = x.UserImage,
                        ThumbsValue = x.ThumbsValue,
                        WorkAgain = x.WorkAgain,
                        JobTitle = x.JobTitle,
                        Review = x.Review
                    }).ToList();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    success = false;
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


        #region Save Portfolio, Edit Portfolio, Get Portfolio List

        [HttpPost]
        [Route("api/SavePortfolio")]
        public IHttpActionResult SavePortfolio(SavePortfolioModel model)
        {
            bool success = false;
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
                    var portfolio = context.UserPortfolios.Where(x => x.ID == model.ID).FirstOrDefault();
                    portfolio = portfolio == null ? new UserPortfolio() : portfolio;

                    portfolio.Title = model.Title;
                    portfolio.Description = model.Description;
                    portfolio.Image = model.Image;
                    portfolio.IsDraft = false;
                    portfolio.UserID = model.UserID;
                    portfolio.IsActive = true;
                    if (model.ID > 0)
                        portfolio.UpdatedDate = DateTime.UtcNow;
                    else
                    {
                        portfolio.CreatedDate = DateTime.UtcNow;
                        portfolio.UpdatedDate = DateTime.UtcNow;
                        context.UserPortfolios.Add(portfolio);
                    }
                    context.SaveChanges();
                    success = true;
                    message = "Save successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/DeletePortfolio")]
        public IHttpActionResult DeletePortfolio(DeleteDetailViewModel model)
        {
            bool success = false;
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
                    var entity = context.UserPortfolios.Where(x => x.ID == model.ID && x.UserID == model.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    entity.IsActive = false;
                    entity.IsDelete = true;
                    entity.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                    success = true;
                    message = "Portfolio deleted successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [Route("api/GetPortfolioList")]
        [HttpPost]
        public IHttpActionResult GetPortfolioList(UserViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var portfolioList = new List<PortfolioModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var portfolio = context.UserPortfolios.Where(x => x.IsDelete == false && x.IsActive == true && x.UserID == viewModel.UserId).ToList();
                    foreach (var x in portfolio)
                    {
                        var model = new PortfolioModel();
                        model.ID = x.ID;
                        model.Title = x.Title;
                        model.Description = x.Description;
                        model.Image = x.Image;
                        portfolioList.Add(model);
                    }
                    success = true;
                    message = "Get List";
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
                Result = portfolioList
            });
        }

        [Route("api/EditPortfolioById")]
        [HttpPost]
        public IHttpActionResult EditPortfolioById(DeleteDetailViewModel viewModel)
        {
            bool status = false;
            var model = new PortfolioModel();
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
                    var result = context.UserPortfolios.Where(x => x.ID == viewModel.ID && x.UserID == viewModel.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    if (result != null)
                    {
                        model = new PortfolioModel
                        {
                            ID = result.ID,
                            Title = result.Title,
                            Description = result.Description,
                            Image = result.Image
                        };
                        status = true;
                        message = "Get Detail";
                    }
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

        #endregion

        #region Save Experience, Edit Experience, Get Experience List

        [HttpPost]
        [Route("api/SaveExperience")]
        public IHttpActionResult SaveExperience(SaveExperienceModel model)
        {
            bool success = false;
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
                    var experience = context.UserExperiences.Where(x => x.ID == model.ID && x.UserID == model.UserID).FirstOrDefault();
                    experience = experience == null ? new UserExperience() : experience;

                    experience.Title = model.Title;
                    experience.Company = model.Company;
                    experience.FromMonth = model.FromMonth;
                    experience.FromYear = model.FromYear;
                    experience.ToMonth = model.ToMonth;
                    experience.ToYear = model.ToYear;
                    experience.CurrentlyWorking = model.CurrentlyWorking;
                    experience.Description = model.Description;
                    experience.IsDraft = false;
                    experience.UserID = model.UserID;
                    experience.IsActive = true;
                    if (model.ID > 0)
                        experience.UpdatedDate = DateTime.UtcNow;
                    else
                    {
                        experience.CreatedDate = DateTime.UtcNow;
                        experience.UpdatedDate = DateTime.UtcNow;
                        context.UserExperiences.Add(experience);
                    }
                    context.SaveChanges();
                    success = true;
                    message = "Save successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/DeleteExperience")]
        public IHttpActionResult DeleteExperience(DeleteDetailViewModel model)
        {
            bool success = false;
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
                    var entity = context.UserExperiences.Where(x => x.ID == model.ID && x.UserID == model.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    entity.IsDelete = true;
                    entity.IsActive = false;
                    entity.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                    success = true;
                    message = "Experience deleted successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [Route("api/GetExperienceList")]
        [HttpPost]
        public IHttpActionResult GetExperienceList(UserViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<GetExperienceModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var userId = User.Identity.GetUserId();
                    var query = context.UserExperiences.Where(x => x.IsDelete == false && x.IsActive == true && x.UserID == viewModel.UserId);
                    model = query.Select(x => new GetExperienceModel
                    {
                        ID = x.ID,
                        Title = x.Title,
                        Company = x.Company,
                        FromPeriod = x.FromMonth + " " + x.FromYear,
                        ToPeriod = x.ToMonth + " " + x.ToYear,
                        CurrWorking = x.CurrentlyWorking.Value == true ? "Working" : "Not Working",
                        Description = x.Description
                    }).ToList();

                    success = true;
                    message = "Get List";
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

        [Route("api/EditExperienceById")]
        [HttpPost]
        public IHttpActionResult EditExperienceById(DeleteDetailViewModel viewModel)
        {
            bool status = false;
            var model = new ExperienceModel();
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
                    var result = context.UserExperiences.Where(x => x.ID == viewModel.ID && x.UserID == viewModel.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    if (result != null)
                    {
                        model = new ExperienceModel
                        {
                            ID = result.ID,
                            Title = result.Title,
                            Company = result.Company,
                            FromMonth = result.FromMonth,
                            FromYear = result.FromYear,
                            ToMonth = result.ToMonth,
                            ToYear = result.ToYear,
                            CurrentlyWorking = result.CurrentlyWorking.Value,
                            Description = result.Description
                        };
                        status = true;
                        message = "Get Detail";
                    }
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

        #endregion 

        #region Save Certification, Edit Certification, Get Certification List

        [HttpPost]
        [Route("api/SaveCertification")]
        public IHttpActionResult SaveCertification(SaveCertificationModel model)
        {
            bool success = false;
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
                    var certification = context.UserCertifications.Where(x => x.ID == model.ID && x.UserID == model.UserID).FirstOrDefault();
                    certification = certification == null ? new UserCertification() : certification;

                    certification.Title = model.Title;
                    certification.GrantedBy = model.GrantedBy;
                    certification.IsDraft = false;
                    certification.UserID = model.UserID;
                    certification.IsActive = true;
                    if (model.ID > 0)
                        certification.UpdatedDate = DateTime.UtcNow;
                    else
                    {
                        certification.CreatedDate = DateTime.UtcNow;
                        certification.UpdatedDate = DateTime.UtcNow;
                        context.UserCertifications.Add(certification);
                    }
                    context.SaveChanges();
                    success = true;
                    message = "Save successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/DeleteCertification")]
        public IHttpActionResult DeleteCertification(DeleteDetailViewModel viewModel)
        {
            bool success = false;
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
                    var entity = context.UserCertifications.Where(x => x.ID == viewModel.ID && x.UserID == viewModel.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    entity.IsDelete = true;
                    entity.IsActive = false;
                    entity.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                    success = true;
                    message = "Certifications deleted successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }

            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [Route("api/GetCertificationsList")]
        [HttpPost]
        public IHttpActionResult GetCertificationsList(UserViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<GetCertificationModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.UserCertifications.Where(x => x.IsDelete == false && x.IsActive == true && x.UserID == viewModel.UserId);
                    model = query.Select(x => new GetCertificationModel
                    {
                        ID = x.ID,
                        Title = x.Title,
                        GrantedBy = x.GrantedBy
                    }).ToList();

                    success = true;
                    message = "Get List";
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

        [Route("api/EditCertificationById")]
        [HttpPost]
        public IHttpActionResult EditCertificationById(DeleteDetailViewModel viewModel)
        {
            bool status = false;
            var model = new CertificationModel();
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
                    var result = context.UserCertifications.Where(x => x.ID == viewModel.ID && x.UserID == viewModel.UserId && x.IsDelete == false).FirstOrDefault();
                    if (result != null)
                    {
                        model = new CertificationModel
                        {
                            ID = result.ID,
                            Title = result.Title,
                            GrantedBy = result.GrantedBy
                        };
                        status = true;
                        message = "Get Detail";
                    }
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

        #endregion

        #region Save Education, Edit Education, Get Education List

        [HttpPost]
        [Route("api/SaveEducation")]
        public IHttpActionResult SaveEducation(SaveEducationModel model)
        {
            bool success = false;
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
                    var education = context.UserEducations.Where(x => x.ID == model.ID && x.UserID == model.UserID).FirstOrDefault();
                    education = education == null ? new UserEducation() : education;

                    education.Degree = model.Degree;
                    education.School = model.School;
                    education.ToYear = model.ToYear;
                    education.EducationLevel = model.EducationLevel;
                    education.IsDraft = false;
                    education.UserID = model.UserID;
                    education.IsActive = true;
                    if (model.ID > 0)
                        education.UpdatedDate = DateTime.UtcNow;
                    else
                    {
                        education.CreatedDate = DateTime.UtcNow;
                        education.UpdatedDate = DateTime.UtcNow;
                        context.UserEducations.Add(education);
                    }
                    context.SaveChanges();
                    success = true;
                    message = "Save successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/DeleteEducation")]
        public IHttpActionResult DeleteEducation(DeleteDetailViewModel model)
        {
            bool success = false;
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
                    var entity = context.UserEducations.Where(x => x.ID == model.ID && x.UserID == model.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    entity.IsActive = false;
                    entity.IsDelete = true;
                    entity.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                    success = true;
                    message = "Education deleted successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [Route("api/GetEducationList")]
        [HttpPost]
        public IHttpActionResult GetEducationList(UserViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<EducationModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.UserEducations.Where(x => x.IsDelete == false && x.UserID == viewModel.UserId && x.IsActive == true);
                    model = query.Select(x => new EducationModel
                    {
                        ID = x.ID,
                        Degree = x.Degree,
                        School = x.School,
                        EducationLevel = x.EducationLevel,
                        ToYear = x.ToYear
                    }).ToList();

                    success = true;
                    message = "Get List";
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

        [Route("api/EditEducationById")]
        [HttpPost]
        public IHttpActionResult EditEducationById(DeleteDetailViewModel user)
        {
            bool status = false;
            var model = new EducationModel();
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
                    var result = context.UserEducations.Where(x => x.ID == user.ID && x.UserID == user.UserId && x.IsDelete == false && x.IsActive == false).FirstOrDefault();
                    if (result != null)
                    {
                        model = new EducationModel
                        {
                            ID = result.ID,
                            Degree = result.Degree,
                            School = result.School,
                            ToYear = result.ToYear,
                            EducationLevel = result.EducationLevel,
                        };
                        status = true;
                        message = "Get Detail";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                }
            }
            return Ok(new
            {
                Success = status,
                Result = model
            });
        }

        #endregion        

        #region Save User About, Edit User About

        [HttpPost]
        [Route("api/SaveAboutUser")]
        public IHttpActionResult SaveAboutUser(AboutUserModel model)
        {
            bool success = false;
            var message = "";
            var viewModel = new UserDetailModel();
            int count = 0;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var userDetail = context.UserDetails.Where(x => x.UserID == model.UserId).FirstOrDefault();
                    userDetail.ProfessionalTitle = model.ProfessionalTitle;
                    userDetail.ProfessionalDescription = model.ProfessionalDescription;
                    userDetail.FirstName = model.UserName;
                    userDetail.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                    success = true;
                    message = "Save successfully.";

                    var signup = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.SIGNUP.ToString()).FirstOrDefault();
                    var emailVerification = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.EMAIL_VERIFICATION.ToString()).FirstOrDefault();
                    var profilePic = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.PROFILE_PIC.ToString()).FirstOrDefault();
                    if (userDetail.ProfilePic != null)
                        count = Convert.ToInt32(signup.Value + emailVerification.Value + profilePic.Value);
                    else
                        count = Convert.ToInt32(signup.Value + emailVerification.Value);

                    viewModel = context.GetUserDetailById(model.UserId).Select(x => new UserDetailModel
                    {
                        UserName = x.Name,
                        UserShortName = x.UserShortName,
                        ProfilePic = x.ProfilePic,
                        CurrentPlan = x.PlanName,
                        BidPoints = x.BidPoints,
                        ProfessionalDescription = x.ProfessionalDescription,
                        ProfessionalTitle = x.ProfessionalTitle,
                        ThumbsDown = Convert.ToInt32(x.ThumbsDown),
                        ThumbsUp = Convert.ToInt32(x.ThumbsUp),
                        Percentage = count
                    }).FirstOrDefault();
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
                Result = viewModel
            });
        }

        [Route("api/EditAboutUserById")]
        [HttpPost]
        public IHttpActionResult EditAboutUserById(UserViewModel user)
        {
            bool status = false;
            var message = "";
            var viewModel = new UserDetailModel();
            int count = 0;
            try
            {
                var userDetail = context.UserDetails.Where(x => x.UserID == user.UserId).FirstOrDefault();
                var signup = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.SIGNUP.ToString()).FirstOrDefault();
                var emailVerification = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.EMAIL_VERIFICATION.ToString()).FirstOrDefault();
                var profilePic = context.ParameterSettings.Where(x => x.UniqueName == ProfileParameterEnum.PROFILE_PIC.ToString()).FirstOrDefault();
                if (userDetail.ProfilePic != null)
                    count = Convert.ToInt32(signup.Value + emailVerification.Value + profilePic.Value);
                else
                    count = Convert.ToInt32(signup.Value + emailVerification.Value);

                viewModel = context.GetUserDetailById(user.UserId).Select(x => new UserDetailModel
                {
                    UserName = x.Name,
                    UserShortName = x.UserShortName,
                    ProfilePic = x.ProfilePic,
                    CurrentPlan = x.PlanName,
                    BidPoints = x.BidPoints,
                    ProfessionalDescription = x.ProfessionalDescription,
                    ProfessionalTitle = x.ProfessionalTitle,
                    ThumbsDown = Convert.ToInt32(x.ThumbsDown),
                    ThumbsUp = Convert.ToInt32(x.ThumbsUp),
                    Percentage = count
                }).FirstOrDefault();
                status = true;
                message = "Get Detail";
            }
            catch (Exception ex)
            {
                status = false;
                message = ex.Message;
            }
            return Ok(new
            {
                Success = status,
                Message = message,
                Result = viewModel
            });
        }

        #endregion

        #region Save Licence, Edit Licence, Get Licence List

        [HttpPost]
        [Route("api/SaveLicence")]
        public IHttpActionResult SaveLicence(SaveLicenceModel model)
        {
            bool success = false;
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
                    var licence = context.UserLicences.Where(x => x.ID == model.ID && x.UserID == model.UserID).FirstOrDefault();
                    licence = licence == null ? new UserLicence() : licence;

                    licence.LicenceNo = model.LicenceNo;
                    licence.ServiceName = model.ServiceName;
                    licence.Issuer = model.Issuer;
                    licence.ExpirationDate = model.ExpirationDate;
                    licence.IsDraft = false;
                    licence.UserID = model.UserID;
                    licence.IsActive = true;
                    if (model.ID > 0)
                        licence.UpdatedDate = DateTime.UtcNow;
                    else
                    {
                        licence.CreatedDate = DateTime.UtcNow;
                        licence.UpdatedDate = DateTime.UtcNow;
                        context.UserLicences.Add(licence);
                    }
                    context.SaveChanges();
                    success = true;
                    message = "Save successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/DeleteLicence")]
        public IHttpActionResult DeleteLicence(DeleteDetailViewModel model)
        {
            bool success = false;
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
                    var entity = context.UserLicences.Where(x => x.ID == model.ID && x.UserID == model.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    entity.IsDelete = true;
                    entity.IsActive = false;
                    entity.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                    success = true;
                    message = "Licence deleted successfully.";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [Route("api/GetLicenceList")]
        [HttpPost]
        public IHttpActionResult GetLicenceList(UserViewModel user)
        {
            bool success = false;
            var message = "";
            var model = new List<LicenceModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.UserLicences.Where(x => x.IsDelete == false && x.IsActive == true && x.UserID == user.UserId);
                    model = query.Select(x => new LicenceModel
                    {
                        ID = x.ID,
                        LicenceNo = x.LicenceNo,
                        ServiceName = x.ServiceName,
                        Issuer = x.Issuer,
                        ExpirationDate = x.ExpirationDate
                    }).ToList();

                    success = true;
                    message = "Get List";
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

        [Route("api/EditLicenceById")]
        [HttpPost]
        public IHttpActionResult EditLicenceById(DeleteDetailViewModel viewModel)
        {
            bool status = false;
            var message = "";
            var model = new LicenceModel();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var result = context.UserLicences.Where(x => x.ID == viewModel.ID && x.UserID == viewModel.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                    if (result != null)
                    {
                        model = new LicenceModel
                        {
                            ID = result.ID,
                            LicenceNo = result.LicenceNo,
                            ServiceName = result.ServiceName,
                            Issuer = result.Issuer,
                            ExpirationDate = result.ExpirationDate
                        };
                        status = true;
                        message = "Get List";
                    }
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

        #endregion

        #region Save User Service

        [Route("api/GetUserServiceList")]
        [HttpPost]
        public IHttpActionResult GetUserServiceList(UserViewModel user)
        {
            var model = new List<UserChildServiceList>();
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
                    model = context.UserServices.Where(x => x.UserID == user.UserId && x.IsActive == true).Select(y => new UserChildServiceList
                    {
                        ID = y.ID,
                        ServiceId = y.ServiceID,
                        Name = y.Service.ServiceName
                    }).ToList();
                    message = "Get List.";
                    status = true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    status = false;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/DeleteUserService")]
        public IHttpActionResult DeleteUserService(DeleteUserService model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                var service = context.UserServices.Where(x => x.ID == model.ID && x.UserID == model.UserId).FirstOrDefault();
                if (service != null)
                {
                    service.IsActive = false;
                    context.SaveChanges();
                    success = true;
                    messsage = "Delete successfully.";
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }

        [HttpPost]
        [Route("api/RemoveAllService")]
        public IHttpActionResult RemoveAllService(UserViewModel model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                var service = context.UserServices.Where(x => x.UserID == model.UserId).ToList();
                if (service != null)
                {
                    foreach (var item in service)
                        context.UserServices.Remove(item);
                    context.SaveChanges();
                    success = true;
                    messsage = "Delete successfully.";
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }

        #endregion

        [HttpPost]
        [Route("api/ChangeEmailAddress")]
        public async Task<IHttpActionResult> ChangeEmailAddress(ChangeEmailModel model)
        {
            bool success = false;
            var message = "";
            var rUrl = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var userExist = await UserManager.FindByIdAsync(model.UserId);
                    if (userExist != null && model.Email != "")
                    {
                        var emailExist = await UserManager.FindByNameAsync(model.Email);
                        if (emailExist == null && model.Email != "")
                        {
                            if (!emailExist.EmailConfirmed)
                            {
                                userExist.UserName = model.Email;
                                userExist.Email = model.Email;
                                var result = UserManager.UpdateAsync(userExist);
                                var code = UserManager.GenerateEmailConfirmationTokenAsync(model.UserId);
                                var detail = context.UserDetails.Where(a => a.UserID == model.UserId).FirstOrDefault();
                                var callbackUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/account/confirmemail?userid=" + model.UserId + "&code=" + code;
                                string emailBody = CommonLib.GetEmailTemplateValue("VerifyAccount/Body");
                                string emailSubject = CommonLib.GetEmailTemplateValue("VerifyAccount/Subject");
                                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                emailBody = emailBody.Replace("@@@UserName", detail.FirstName);
                                emailBody = emailBody.Replace("@@@link", callbackUrl);
                                CommonLib.SendMail(strFromEmailAddress, userExist.Email, emailSubject, emailBody);
                                success = true;
                                message = "Email updated successfully!";
                            }
                            else
                            {
                                success = false;
                                message = "Email already confirmed!";
                                rUrl = "/dashboard";
                            }
                        }
                        else
                        {
                            success = false;
                            message = model.Email + " is already taken.";
                        }
                    }
                    rUrl = "/resend-email";
                }
                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                    rUrl = "/resend-email";
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                RUrl = rUrl
            });
        }

        [HttpPost]
        [Route("api/ResedEmailVerification")]
        public async Task<IHttpActionResult> ResedEmailVerification(UserViewModel model)
        {
            bool success = false;
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
                    var userExist = await UserManager.FindByIdAsync(model.UserId);
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(model.UserId);
                    var detail = context.UserDetails.Where(a => a.UserID == model.UserId).FirstOrDefault();
                    var callbackUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/account/confirmemail?userid=" + model.UserId + "&code=" + code;
                    string emailBody = CommonLib.GetEmailTemplateValue("VerifyAccount/Body");
                    string emailSubject = CommonLib.GetEmailTemplateValue("VerifyAccount/Subject");
                    string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                    emailBody = emailBody.Replace("@@@UserName", detail.FirstName);
                    emailBody = emailBody.Replace("@@@link", callbackUrl);
                    CommonLib.SendMail(strFromEmailAddress, userExist.Email, emailSubject, emailBody);
                    success = true;
                    message = "Email send successfully please check your email.";
                }
                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/SaveUserService")]
        public IHttpActionResult SaveUserService(UserServiceList model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                if (model.IsSelected)
                {
                    UserService service = new UserService();
                    service.ServiceID = model.ServiceId;
                    service.UserID = model.UserId;
                    service.IsActive = true;
                    service.IsDraft = false;
                    service.CreatedDate = DateTime.UtcNow;
                    context.UserServices.Add(service);
                    context.SaveChanges();
                    success = true;
                    messsage = "Save Successfully";
                }
                else
                {
                    var service = context.UserServices.Where(x => x.ServiceID == model.ServiceId && x.UserID == model.UserId).FirstOrDefault();
                    if (service != null)
                    {
                        context.UserServices.Remove(service);
                        context.SaveChanges();
                        success = true;
                        messsage = "Save Successfully";
                    }
                    else
                    {
                        success = false;
                        messsage = "Data not exist";
                    }
                }
                var serviceDetail = context.UserServices.Where(x => x.UserID == model.UserId && x.IsActive == true).Count();
                if(serviceDetail > 0)
                {
                    UserManager.RemoveFromRole(model.UserId, UserRoleEnum.Customer.ToString());
                    UserManager.AddToRole(model.UserId, UserRoleEnum.ServiceProvider.ToString());
                    var nCount = context.Proc_SetUserDefaultNotifications(model.UserId, UserRoleEnum.ServiceProvider.ToString());
                }
                else
                {
                    UserManager.RemoveFromRole(model.UserId, UserRoleEnum.ServiceProvider.ToString());
                    UserManager.AddToRole(model.UserId, UserRoleEnum.Customer.ToString());
                    var nCount = context.Proc_SetUserDefaultNotifications(model.UserId, UserRoleEnum.Customer.ToString());
                }
               
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }

        [HttpPost]
        [Route("api/SaveUserLocation")]
        public IHttpActionResult SaveUserLocation(UserLocationList model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                if (model.IsSelected)
                {
                    var cityDetail = context.CityMasters.Where(x => x.ID == model.CityId && x.StateID == model.StateId).FirstOrDefault();
                    UserAddress address = new UserAddress();
                    address.City = cityDetail.Name;
                    address.CityId = cityDetail.ID;
                    address.State = cityDetail.StateMaster.Name;
                    address.StateId = model.StateId;
                    address.CountryID = 231;
                    address.UserID = model.UserId;
                    address.IsActive = true;
                    address.IsDraft = false;
                    address.IsDelete = false;
                    address.CreatedDate = DateTime.UtcNow;
                    address.UpdatedDate = DateTime.UtcNow;
                    context.UserAddresses.Add(address);
                    context.SaveChanges();
                }
                else
                {
                    var address = context.UserAddresses.Where(x => x.StateId == model.StateId && x.UserID == model.UserId && x.CityId == model.CityId).FirstOrDefault();
                    context.UserAddresses.Remove(address);
                    context.SaveChanges();
                }                
                success = true;
                messsage = "Save Successfully";
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }             

        [Route("api/GetUserAddressList")]
        [HttpPost]
        public IHttpActionResult GetUserAddressList(UserViewModel viewModel)
        {
            bool success = false;
            var messsage = "";
            var model = new List<UserAddressViewModel>();
            try
            {
                var query = context.UserAddresses.Where(x => x.IsDelete == false && x.UserID == viewModel.UserId && x.IsActive == true);
                model = query.Select(x => new UserAddressViewModel
                {
                    ID = x.ID,
                    StateId = x.StateId,
                    CityId = x.CityId,
                    CityWithState = x.CityMaster.Name + ", " + x.StateMaster.Name                  
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


        [HttpPost]
        [Route("api/DeleteUserAddress")]
        public IHttpActionResult DeleteUserAddress(DeleteUserService model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                var entity = context.UserAddresses.Where(x => x.ID == model.ID && x.UserID == model.UserId && x.IsDelete == false && x.IsActive == true).FirstOrDefault();
                entity.IsDelete = true;
                entity.IsActive = false;
                entity.UpdatedDate = DateTime.UtcNow;
                context.SaveChanges();
                success = true;
                messsage = "Address deleted successfully.";
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }
    }
}
