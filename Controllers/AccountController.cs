using QuiGigAPI.DataBase;
using QuiGigAPI.Models;
using System;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using QuiGigAPI.Common;
using System.Configuration;
using System.Text;
using Facebook;
using System.Collections.Generic;
using RestSharp;
using System.Security.Claims;
using static QuiGigAPI.Controllers.CommonController;

namespace QuiGigAPI.Controllers
{
    //[Authorize]
    public class AccountController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

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
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        [HttpPost]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("api/Login")]
        public async Task<IHttpActionResult> Login(LoginModel model)
        {
            string currRole = "";
            string message = "";
            string userId = "";
            string userName = "";
            bool success = false;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
                return Ok(new
                {
                    Success = success,
                    Message = message,
                    CurrentRole = currRole,
                    UserName = userName,
                    UserId = userId
                });
            }
            else
            {
                try
                {
                    var user = await UserManager.FindByNameAsync(model.Email);
                    if (user != null)
                    {
                        var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, true, shouldLockout: false);

                        switch (result)
                        {
                            case SignInStatus.Success:
                                {
                                    var detail = context.UserDetails.Where(a => a.UserID == user.Id).FirstOrDefault();
                                    if (detail.IsDelete)
                                    {
                                        return Ok(new
                                        {
                                            Success = false,
                                            Message = "Currently your account is de-activated please contact to admin",
                                            UserName = userName,
                                            UserId = userId,
                                            CurrentRole = currRole,
                                            IsEmailConfirmed = false
                                        });
                                    }
                                    else
                                    {
                                        if (UserManager.IsInRole(user.Id, UserRoleEnum.SuperAdmin.ToString()))
                                            currRole = UserRoleEnum.SuperAdmin.ToString();
                                        else if (UserManager.IsInRole(user.Id, UserRoleEnum.SubAdmin.ToString()))
                                            currRole = UserRoleEnum.SubAdmin.ToString();
                                        else if (UserManager.IsInRole(user.Id, UserRoleEnum.ServiceProvider.ToString()))
                                            currRole = UserRoleEnum.ServiceProvider.ToString();
                                        else
                                            currRole = UserRoleEnum.Customer.ToString();

                                        return Ok(new
                                        {
                                            Success = true,
                                            Message = "Login successfully. Please wait....",
                                            UserName = detail.FirstName,
                                            UserId = user.Id,
                                            CurrentRole = currRole,
                                            IsEmailConfirmed = user.EmailConfirmed
                                        });
                                    }
                                }

                            case SignInStatus.LockedOut:
                                {
                                    return Ok(new
                                    {
                                        success = false,
                                        message = "This account is locked, please contact administrator to unlock account.",
                                        UserName = userName,
                                        UserId = userId,
                                        CurrentRole = currRole,
                                        IsEmailConfirmed = false
                                    });
                                }
                            case SignInStatus.RequiresVerification:
                                {
                                    return Ok(new
                                    {
                                        success = false,
                                        message = "This account is locked, please contact administrator to unlock account.",
                                        UserName = userName,
                                        UserId = userId,
                                        CurrentRole = currRole,
                                        IsEmailConfirmed = false
                                    });
                                }
                            case SignInStatus.Failure:
                                return Ok(new
                                {
                                    success = false,
                                    Message = "Invalid login attempt.",
                                    UserName = userName,
                                    UserId = userId,
                                    CurrentRole = currRole,
                                    IsEmailConfirmed = false
                                });
                            default:
                                return Ok(new
                                {
                                    success = false,
                                    message = "Invalid login attempt.",
                                    UserName = userName,
                                    UserId = userId,
                                    CurrentRole = currRole,
                                    IsEmailConfirmed = false
                                });
                        }
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Invalid login attempt.",
                            UserName = userName,
                            UserId = userId,
                            CurrentRole = currRole
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        success = false,
                        message = ex.Message,
                        UserName = userName,
                        UserId = userId,
                        CurrentRole = currRole
                    });
                }
            }
        }

        [HttpPost]
        [Route("api/Register")]
        public async Task<IHttpActionResult> Register(RegisterModel model)
        {
            string currRole = "";

            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = false,
                    };
                    var userExist = await UserManager.FindByNameAsync(model.Email);
                    if (userExist != null)
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = model.Email + " is already taken.",
                        });
                    }
                    else
                    {
                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            var userRole = UserRoleEnum.Customer.ToString();
                            if (!string.IsNullOrEmpty(model.UserRole))
                            {
                                if (model.UserRole == UserRoleEnum.ServiceProvider.ToString())
                                {
                                    userRole = UserRoleEnum.ServiceProvider.ToString();
                                }
                            }
                            var userEmailCode = RandomStringAndNumeric(6);

                            UserManager.AddToRole(user.Id, userRole);
                            UserDetail userEntity = new UserDetail();
                            userEntity.FirstName = model.FullName;
                            userEntity.CreatedDate = DateTime.UtcNow;
                            userEntity.UpdatedDate = DateTime.UtcNow;
                            userEntity.UserID = user.Id;
                            userEntity.UserEmailCode = userEmailCode;
                            userEntity.IsActive = true;
                            userEntity.IsDelete = false;
                            userEntity.IsPublish = true;
                            userEntity.CarryForwordCoin = 0;
                            context.UserDetails.Add(userEntity);
                            context.SaveChanges();

                            var nCount = context.Proc_SetUserDefaultNotifications(user.Id, userRole);

                            if (!string.IsNullOrEmpty(model.City) && !string.IsNullOrEmpty(model.StateName))
                            {
                                var stateDetail = context.StateMasters.Where(x => x.Name.ToLower() == model.StateName.ToLower()).FirstOrDefault();
                                if (stateDetail != null)
                                {
                                    var cityDetail = context.CityMasters.Where(x => x.Name.ToLower() == model.City.ToLower() && x.StateID == stateDetail.ID).FirstOrDefault();
                                    UserAddress entity = new UserAddress();
                                    entity.City = model.City;
                                    entity.CityId = cityDetail.ID;
                                    entity.State = stateDetail.Name;
                                    entity.StateId = stateDetail.ID;
                                    entity.CountryID = stateDetail.CountryID;
                                    entity.UserID = user.Id;
                                    entity.IsDraft = true;
                                    entity.CreatedDate = DateTime.UtcNow;
                                    entity.IsActive = true;
                                    entity.IsDelete = false;
                                    entity.UpdatedDate = DateTime.UtcNow;
                                    context.UserAddresses.Add(entity);
                                    context.SaveChanges();
                                }
                            }

                            #region Email Sending Code
                            try
                            {
                                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                string emailBody = CommonLib.GetEmailTemplateValue("VerifyAccount/Body");
                                string emailSubject = CommonLib.GetEmailTemplateValue("VerifyAccount/Subject");
                                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                emailBody = emailBody.Replace("@@@Path", path);
                                emailBody = emailBody.Replace("@@@UserName", model.FullName);
                                emailBody = emailBody.Replace("@@@UserEmailCode", userEmailCode);
                                CommonLib.SendMail(strFromEmailAddress, model.Email, emailSubject, emailBody);
                            }
                            catch (Exception ex)
                            {
                                return Ok(new
                                {
                                    Success = false,
                                    Message = ex.Message
                                });
                            }
                            #endregion

                            currRole = UserRoleEnum.Customer.ToString();
                            return Ok(new
                            {
                                Success = true,
                                Message = "You have successfully registered, Please verify your account from registered email.",
                                CurrentRole = currRole,
                                UserName = model.FullName,
                                UserId = user.Id
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                Success = false,
                                Message = ((string[])(result.Errors))[0]
                            });
                        }
                    }
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                    var er = errors.Errors[0].ErrorMessage;
                    return Ok(new
                    {
                        Success = false,
                        Message = er
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("api/EmailExists")]
        public async Task<IHttpActionResult> EmailExists(string email)
        {
            string Msg = string.Empty;
            try
            {
                var userExist = await UserManager.FindByNameAsync(email);
                if (userExist != null)
                {
                    Msg = email + " already taken.";
                    return Ok(new
                    {
                        Success = false,
                        Message = Msg
                    });
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }

            return Ok(new
            {
                Success = true,
                Message = Msg
            });
        }

        [HttpPost]
        [Route("api/ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var message = "";
            bool success = false;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var userEmailCode = GeneratePassword(true, true, true, true, false, 8);
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var result = await UserManager.ResetPasswordAsync(user.Id, code, userEmailCode);
                    if (result.Succeeded)
                    {
                        var userDetail = context.UserDetails.Where(x => x.UserID == user.Id).FirstOrDefault();
                        string emailBody = CommonLib.GetEmailTemplateValue("ForgotPassword/Body");
                        string emailSubject = CommonLib.GetEmailTemplateValue("ForgotPassword/Subject");
                        var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                        emailBody = emailBody.Replace("@@@Path", path);
                        emailBody = emailBody.Replace("@@@UserName", userDetail.FirstName);
                        emailBody = emailBody.Replace("@@@UserEmailCode", userEmailCode);
                        CommonLib.SendMail(strFromEmailAddress, model.Email, emailSubject, emailBody);
                        success = true;
                        message = "Your new password sent on you email address.";
                    }
                }
                else
                {
                    success = false;
                    message = "Invaid email address";
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    var result = await UserManager.ChangePasswordAsync(model.UserId, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok(new
                        {
                            Success = true,
                            Message = "You have successfully changed your Password!"
                        });
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            if (item == "Incorrect password.")
                            {
                                return Ok(new
                                {
                                    Success = false,
                                    Message = "The Old Password you entered is incorrect. Please try again!."
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    Success = false,
                                    Message = item
                                });
                            }
                        }
                    }
                }
                else
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = "Invaid user"
                    });
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        if (error.ErrorMessage == "The new password and confirmation password do not match.")
                        {
                            return Ok(new
                            {
                                Success = false,
                                Message = "The Current Password you entered does not match with New Password. Please try again!"
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                Success = false,
                                Message = error.ErrorMessage,
                            });
                        }

                    }
                }
            }
            return Ok(new
            {
                Success = false,
                Message = "Invaid user"
            });
        }

        [HttpPost]
        [Route("api/GetUserInfo")]
        public async Task<IHttpActionResult> GetUserInfo()
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            var userDetail = context.UserDetails.Where(x => x.UserID == userId).FirstOrDefault();
            if (userDetail != null)
            {
                var model = new UserInfoModel()
                {
                    FirstName = userDetail.FirstName,
                    ProfilePic = userDetail.ProfilePic,
                    TimeZoneID = Convert.ToInt64(userDetail.TimeZoneID),
                    Email = user.Email
                };

                return Ok(new
                {
                    Success = true,
                    Result = model
                });
            }
            else
            {
                return Ok(new
                {
                    Success = false,
                    Message = "User info not exist",
                    Result = ""
                });
            }
        }

        [HttpPost]
        [Route("api/GetTimeZoneList")]
        public IHttpActionResult GetTimeZoneList()
        {
            var model = context.TimeZones.Select(x => new System.Web.Mvc.SelectListItem
            {
                Text = x.TimeZoneName,
                Value = x.ID.ToString()
            }).ToList();

            model.Insert(0, new System.Web.Mvc.SelectListItem { Text = "-- Select Time Zone --", Value = "" });
            return Ok(new
            {
                Success = true,
                Result = model
            });
        }

        [AllowAnonymous]
        [Route("api/ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(ConfirmEmailModel model)
        {
            bool success = false;
            var message = "";
            try
            {
                var user = await UserManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    var detail = context.UserDetails.Where(a => a.UserID == user.Id).FirstOrDefault();
                    if (detail.UserEmailCode == model.Code)
                    {
                        user.EmailConfirmed = true;
                        var result = await UserManager.UpdateAsync(user);
                        var roleId = user.Roles.Select(c => c.RoleId).FirstOrDefault();
                        var role = Util.GetRoleNameById(roleId, context);
                        var alreadyTakePlan = context.UserPlans.Where(x => x.UserID == model.UserId && x.ExpiryDate >= DateTime.UtcNow).FirstOrDefault();
                        if (alreadyTakePlan == null)
                            FreePlan(user.Id, role);
                        success = true;
                        message = "Email confirmed successfully.";
                    }
                    else
                        message = "Invalid code";
                }
                else
                    message = "Invalid user detail.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        private static Random random = new Random();
        public string RandomStringAndNumeric(int Size)
        {
            string input = "abcdefghijklmnopqrstuvwxyz0123456789#+@&$ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
        public static string GeneratePassword(bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial, bool includeSpaces, int lengthOfPassword)
        {
            const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
            const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMERIC_CHARACTERS = "0123456789";
            const string SPECIAL_CHARACTERS = @"!#$%&*@\";
            const string SPACE_CHARACTER = " ";
            const int PASSWORD_LENGTH_MIN = 8;
            const int PASSWORD_LENGTH_MAX = 128;

            if (lengthOfPassword < PASSWORD_LENGTH_MIN || lengthOfPassword > PASSWORD_LENGTH_MAX)
            {
                return "Password length must be between 8 and 128.";
            }

            string characterSet = "";

            if (includeLowercase)
            {
                characterSet += LOWERCASE_CHARACTERS;
            }

            if (includeUppercase)
            {
                characterSet += UPPERCASE_CHARACTERS;
            }

            if (includeNumeric)
            {
                characterSet += NUMERIC_CHARACTERS;
            }

            if (includeSpecial)
            {
                characterSet += SPECIAL_CHARACTERS;
            }

            if (includeSpaces)
            {
                characterSet += SPACE_CHARACTER;
            }

            char[] password = new char[lengthOfPassword];
            int characterSetLength = characterSet.Length;

            Random random = new Random();
            for (int characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
            {
                password[characterPosition] = characterSet[random.Next(characterSetLength - 1)];

                bool moreThanTwoIdenticalInARow =
                    characterPosition > 2
                    && password[characterPosition] == password[characterPosition - 1]
                    && password[characterPosition - 1] == password[characterPosition - 2];

                if (moreThanTwoIdenticalInARow)
                {
                    characterPosition--;
                }
            }

            return string.Join(null, password);
        }
        public void FreePlan(string userId, string role)
        {
            try
            {
                var currencyDef = context.CurrencyDefinations.Where(x => x.CurrencyCode == "USD").FirstOrDefault();
                decimal amt = 5 / currencyDef.CurrencyValue;
                UserPlan plan = new UserPlan();
                plan.UserID = userId;
                plan.PlanDurationID = plan.PlanDurationID;
                plan.RoleID = Util.GetRoleIdByName(role, context);
                plan.ExpiryDate = DateTime.UtcNow.AddYears(1);
                plan.IsActive = true;
                plan.IsDelete = false;
                plan.PlanAmount = 0;
                plan.PaymentStatus = PaymentStatus.Completed.ToString();
                plan.CreatedDate = DateTime.UtcNow;
                plan.UpdatedDate = DateTime.UtcNow;
                context.UserPlans.Add(plan);
                context.SaveChanges();
                Util.SaveUserWallet(context, userId, 0, "Sign Up", Convert.ToInt32(amt), 0, 0, PaymentStatus.Completed.ToString(), PaymentFromSite.QuiGig.ToString(), 0, 0, "Admin");
                Util.SaveActivity(context, "Congratulation! You have received " + amt + " free Quigs.", userId, userId, ProfileParameterEnum.SIGNUP.ToString(), 0, role, true, false);
            }
            catch (Exception ex)
            {
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Account/ExternalLoginCallback")]
        public async Task<IHttpActionResult> ExternalLoginCallback()
        {
            var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
            bool success = false;
            var message = "";

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return Ok(new
                {
                    Success = success,
                    Message = message
                });
            }
            var identity = AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            var lastName = identity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            var firstName = identity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);

            // Sign in the user with this external login provider if the user already has a login
            //var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            var user = await UserManager.FindByEmailAsync(loginInfo.Email);
            string password = RandomStringAndNumeric(12);
            string status = string.Empty;
            string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();

            if (user == null)
            {
                #region User is not exist

                var userDetail = new ApplicationUser
                {
                    UserName = loginInfo.Email,
                    Email = loginInfo.Email,
                    EmailConfirmed = true,
                    PhoneNumber = ""
                };
                var pass = RandomStringAndNumeric(12);
                var IsUser = await UserManager.CreateAsync(userDetail, pass);
                user = await UserManager.FindByEmailAsync(loginInfo.Email);

                var userRole = UserRoleEnum.Customer.ToString();
                UserManager.AddToRole(user.Id, userRole);
                var userExist = context.UserDetails.Where(x => x.UserID == user.Id).FirstOrDefault();
                if (userExist == null)
                {
                    UserDetail userEntity = new UserDetail();
                    userEntity.FirstName = firstName.Value + " " + lastName.Value;
                    userEntity.CreatedDate = DateTime.UtcNow;
                    userEntity.UpdatedDate = DateTime.UtcNow;
                    userEntity.UserID = user.Id;
                    userEntity.IsActive = true;
                    userEntity.IsPublish = true;
                    userEntity.IsDelete = false;
                    userEntity.CarryForwordCoin = 0;
                    //if (!string.IsNullOrEmpty(model.IpAddress))
                    //{
                    //    userEntity.IpAddress = model.IpAddress;
                    //    var url = "http://freegeoip.net/json/" + model.IpAddress;
                    //    var client = new RestClient(url);
                    //    var response = client.Execute<List<LocationDetailModel>>(new RestRequest());
                    //    var releases = response.Data;
                    //    var city = releases[0].city;
                    //    var state = releases[0].region_name;
                    //    var country = releases[0].country_name;
                    //    if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(country))
                    //        userEntity.Location = city + ", " + state + ", " + country;
                    //}
                    context.UserDetails.Add(userEntity);
                    context.SaveChanges();
                    var roleId = user.Roles.Select(c => c.RoleId).FirstOrDefault();
                    var role = Util.GetRoleNameById(roleId, context);
                    var alreadyTakePlan = context.UserPlans.Where(x => x.UserID == user.Id && x.ExpiryDate >= DateTime.UtcNow).FirstOrDefault();
                    if (alreadyTakePlan == null)
                        FreePlan(user.Id, role);

                    var nCount = context.Proc_SetUserDefaultNotifications(user.Id, userRole);
                    var userName = firstName.Value + " " + lastName.Value;

                    #region Welcome Email
                    try
                    {
                        string emailBody = CommonLib.GetEmailTemplateValue("WelcomeEmail/Body");
                        string emailSubject = CommonLib.GetEmailTemplateValue("WelcomeEmail/Subject");
                        CommonLib.SendMail(strFromEmailAddress, user.Email, emailSubject, emailBody);
                    }
                    catch (Exception ex)
                    {
                    }
                    #endregion


                    return Ok(new
                    {
                        Success = true,
                        Message = "You have successfully registered.",
                        CurrentRole = userRole,
                        UserName = userName,
                        UserId = user.Id
                    });
                }

                #endregion
            }
            else
            {
                var userExist = context.UserDetails.Where(x => x.UserID == user.Id).FirstOrDefault();
                var roleId = user.Roles.Select(c => c.RoleId).FirstOrDefault();
                var currRole = Util.GetRoleNameById(roleId, context);
                return Ok(new
                {
                    Success = true,
                    Message = "Login successfully. Please wait....",
                    CurrentRole = currRole,
                    UserName = userExist.FirstName,
                    UserId = userExist.UserID
                });
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        #region Facebook
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Facebook")]
        public IHttpActionResult Facebook()
        {
            var fb = new FacebookClient();
            var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "1718148838206370",
                client_secret = "c9fe8d683464ad721b990e7b9cd1facb",
                redirect_uri = path + "/api/FacebookCallback",
                response_type = "code",
                scope = "email" // Add other permissions as needed
            });
            return Ok(new
            {
                Success = true,
                Message = "Please wait....",
                RedirectUrl = loginUrl.AbsoluteUri
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/FacebookCallback")]
        public async Task<IHttpActionResult> FacebookCallback(string code)
        {
            var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
            bool success = false;
            var message = "";
            try
            {
                var fb = new FacebookClient();
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = "1718148838206370",
                    client_secret = "c9fe8d683464ad721b990e7b9cd1facb",
                    redirect_uri = path + "/api/FacebookCallback",
                    code = code
                });

                var accessToken = result.access_token;

                // update the facebook client with the access token so 
                // we can make requests on behalf of the user
                fb.AccessToken = accessToken;

                // Get the user's information
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;
                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();

                var IsUser = await UserManager.FindByEmailAsync(email);
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumber = ""
                };
                if (IsUser == null)
                {
                    var pass = RandomStringAndNumeric(12);
                    var resultUser = await UserManager.CreateAsync(user, pass);
                }
                IsUser = await UserManager.FindByEmailAsync(email);
                var userExist = context.UserDetails.Where(x => x.UserID == IsUser.Id).FirstOrDefault();
                if (userExist == null)
                {
                    #region User is not exist

                    var userRole = UserRoleEnum.Customer.ToString();
                    UserManager.AddToRole(user.Id, userRole);
                    UserDetail userEntity = new UserDetail();
                    userEntity.FirstName = me.first_name + " " + me.last_name;
                    userEntity.CreatedDate = DateTime.UtcNow;
                    userEntity.UpdatedDate = DateTime.UtcNow;
                    userEntity.UserID = user.Id;
                    userEntity.CarryForwordCoin = 0;
                    userEntity.IsActive = true;
                    userEntity.IsPublish = true;
                    userEntity.IsDelete = false;

                    context.UserDetails.Add(userEntity);
                    context.SaveChanges();

                    var nCount = context.Proc_SetUserDefaultNotifications(user.Id, userRole);

                    var userName = me.first_name + " " + me.last_name;
                    #region Welcome Email
                    try
                    {
                        string emailBody = CommonLib.GetEmailTemplateValue("WelcomeEmail/Body");
                        string emailSubject = CommonLib.GetEmailTemplateValue("WelcomeEmail/Subject");
                        CommonLib.SendMail(strFromEmailAddress, user.Email, emailSubject, emailBody);
                    }
                    catch (Exception ex)
                    {
                    }
                    #endregion

                    var roleId = user.Roles.Select(c => c.RoleId).FirstOrDefault();
                    var role = Util.GetRoleNameById(roleId, context);
                    var alreadyTakePlan = context.UserPlans.Where(x => x.UserID == user.Id && x.ExpiryDate >= DateTime.UtcNow).FirstOrDefault();
                    if (alreadyTakePlan == null)
                        FreePlan(user.Id, role);

                    return Ok(new
                    {
                        Success = true,
                        Message = "You have successfully registered.",
                        CurrentRole = userRole,
                        UserName = userName,
                        UserId = user.Id
                    });
                    #endregion
                }
                else
                {
                    var roleId = IsUser.Roles.Select(c => c.RoleId).FirstOrDefault();
                    var currRole = Util.GetRoleNameById(roleId, context);
                    return Ok(new
                    {
                        Success = true,
                        Message = "Login successfully. Please wait....",
                        CurrentRole = currRole,
                        UserName = userExist.FirstName,
                        UserId = userExist.UserID
                    });
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }
        #endregion                
    }
}
