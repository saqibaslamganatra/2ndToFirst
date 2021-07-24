using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RainWorx.FrameWorx.Clients;
using RainWorx.FrameWorx.DTO;
using RainWorx.FrameWorx.DTO.FaultContracts;
using RainWorx.FrameWorx.MVC.Areas.API.Controllers.Base;
using RainWorx.FrameWorx.MVC.Areas.API.Controllers.Helpers;
using RainWorx.FrameWorx.MVC.Areas.API.Models;

namespace RainWorx.FrameWorx.MVC.Areas.API.Controllers
{
    /// <summary>
    /// Provides services to Create/Update/Get/Delete Users
    /// </summary>
    [RoutePrefix("api/User")]    
    public class UserController : AuctionWorxAPIController
    {
        /// <summary>
        /// Gets a User by UserName
        /// </summary>
        /// <param name="username">The UserName of the User to get</param>
        /// <returns>An HTTP Status code of 200 (OK) and the User on success.  Otherwise, HTTP Status code 404 (Not Found) if the User is not found.</returns>
        [Route("{username}")]
        [ResponseType(typeof(APIUser))]
        public HttpResponseMessage Get(string username)
        {
            try
            {
                DTO.User user = UserClient.GetUserByUserName(Strings.SystemActors.SystemUserName, username);

                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
                }
                else
                {
                    return Request.CreateResponse<APIUser>(HttpStatusCode.OK, APIUser.FromDTOUser(user));   
                }                
            }
            catch (System.ServiceModel.FaultException<InvalidArgumentFaultContract> /*iafc*/)
            {                
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
            }
        }
    }
}
