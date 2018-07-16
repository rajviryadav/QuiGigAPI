using Microsoft.AspNet.Identity.Owin;
using QuiGigAPI.DataBase;
using QuiGigAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace QuiGigAPI.Controllers
{
    [Authorize]
    public class QuigsController : ApiController
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

        [Route("api/GetQuig")]
        [HttpGet]
        public IHttpActionResult GetQuig()
        {
            var currencyDef = context.CurrencyDefinations.Where(x => x.CurrencyCode == "USD").FirstOrDefault();
            var transFee = context.ParameterSettings.Where(x => x.UniqueName == "TRANSACTION_FEE").FirstOrDefault();
            var perQuig = currencyDef.QuigsValue / currencyDef.CurrencyValue;
            var perQuigsValue = currencyDef.CurrencyValue;           

            var result = context.QuigsPackages.Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new QuigsPackageModel
            {
                ID = x.ID,
                QuigsValue = x.QuigsAmount * perQuig,
                QuigsAmount = x.QuigsAmount             
            }).OrderBy(x => x.QuigsAmount).ToList();

            return Ok(new
            {
                Success = true,
                Result = result,
                PerQuigsValue = perQuigsValue,
                BonusCoin = currencyDef.BonusCoin,
                TransationFee = transFee.Value
            });
        }
    }
}
