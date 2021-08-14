using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RainWorx.FrameWorx.Clients;
using RainWorx.FrameWorx.MVC.Helpers;
using RainWorx.FrameWorx.MVC.Models;
using RainWorx.FrameWorx.DTO;
using RainWorx.FrameWorx.Strings;

namespace RainWorx.FrameWorx.MVC.Controllers
{
    /// <summary>
    /// Provides methods that display custom CMS content
    /// </summary>
    [GoUnsecure]
    public class PageController : AuctionWorxController
    {
        /// <summary>
        /// Displays the specified custom CMS content
        /// </summary>
        /// <param name="name">the name of the specified custom CMS content</param>
        /// <returns>404 error if the specified content doesn't exist</returns>
        [Authenticate]
        public ActionResult Index(string name)
        {
            string culture = this.GetCookie(Strings.MVC.CultureCookie) ??
                             SiteClient.Settings[Strings.SiteProperties.SiteCulture];
            Content content = SiteClient.GetContentContainer(name, culture);
            if (content == null) return HttpNotFound();
            return View(content);
        }
       [HttpPost]
        [AllowAnonymous]
        public JsonResult AjaxMethod(string name)
        {
            PersonModel person = new PersonModel
            {
                Name = name,
                DateTime = DateTime.Now.ToString()
            };
            return Json(person);
        }


        [HttpPost]
        [Authenticate]
        public ActionResult GetListingItemInfo(int? Id)
        {
            try
            {

                Listing currentListing = ListingClient.GetListingByIDAndUserWithFillLevel(User.Identity.Name, Id.Value,
                                                                         this.FBOUserName(), Strings.ListingFillLevels.Default);

                //if this listing is awaiting payment or a draft and it's not the owner or an admin, return a "Not Found" result
                if (currentListing != null && (currentListing.Status == ListingStatuses.AwaitingPayment || currentListing.Status == ListingStatuses.Draft))
                {
                    if (currentListing.OwnerUserName != this.FBOUserName())
                    {
                        if (!User.IsInRole(Roles.Admin))
                        {
                            return NotFound();
                        }
                    }
                }
                bool showSellerLocation = false;
                CustomProperty siteProp = SiteClient.Properties.Where(
                            p => p.Field.Name == Strings.SiteProperties.ShowSellerLocationOnListingDetails).FirstOrDefault();
                if (siteProp != null)
                {
                    bool.TryParse(siteProp.Value, out showSellerLocation);
                }

                if (showSellerLocation)
                {
                    Address sellerAddress = UserClient.GetAddresses(currentListing.OwnerUserName, currentListing.Owner.UserName).Where(
                        a => a.ID == currentListing.Owner.PrimaryAddressID).SingleOrDefault();
                    if (sellerAddress != null)
                    {
                        ViewData["SellerInformation"] = sellerAddress;
                        ViewData["Listing"] = currentListing;
                    }
                    else
                    {
                        ViewData["SellerLocation"] = string.Empty;
                    }

                }

                return Json(new
                {
                    msg = "Successfully added "
                }); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
         /// Displays "Listing not found" error message.
         /// </summary>
         /// <returns>View()</returns>
        public ActionResult NotFound()
        {
            return View(Strings.MVC.NotFoundAction);
        }

    }

    public class PersonModel
    {
        ///<summary>
        /// Gets or sets Name.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// Gets or sets DateTime.
        ///</summary>
        public string DateTime { get; set; }
    }
   
}
