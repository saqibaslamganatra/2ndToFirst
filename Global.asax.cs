﻿using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using RainWorx.FrameWorx.BLL.ServiceImplementations;
using RainWorx.FrameWorx.DTO.EventArgs;
using RainWorx.FrameWorx.MVC.Controllers;
using RainWorx.FrameWorx.Providers.MediaAsset;
using RainWorx.FrameWorx.Providers.Payment;
using RainWorx.FrameWorx.Queueing;
using RainWorx.FrameWorx.Unity;
using RainWorx.FrameWorx.Providers.Listing;
using RainWorx.FrameWorx.Clients;
using RainWorx.FrameWorx.Providers.Fee;
using RainWorx.FrameWorx.Utility;
using RainWorx.FrameWorx.Strings;
using System.Configuration;
using System;
//using Microsoft.Web.WebPages.OAuth;
using System.Net;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Diagnostics;
using System.Collections.Generic;

namespace RainWorx.FrameWorx.MVC
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/fwlink/?LinkId=301868
    public class WebApiApplication : System.Web.HttpApplication
    {
        private SchedulerService _schedulerService;
        private NotifierService _notifierService;
        private IQueueManager _queueManager;

        protected void Application_Start()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            bool ensureTls12Support = false;
            bool.TryParse(ConfigurationManager.AppSettings["EnsureTls12Support"], out ensureTls12Support);
            if (ensureTls12Support) ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            RainWorx.FrameWorx.MVC.Areas.API.Utilities.VirtualRoot = Server.MapPath("~");

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes, SiteClient.TextSetting(SiteProperties.HomepageContent));

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            bool siteClientResetNeeded = false;
            bool somethingChanged;

            do
            {
                somethingChanged = CommonClient.Initialize(Strings.SystemActors.SystemUserName);
            } while (somethingChanged);

            CommonClient.RegisterMediaAssetProviders();

            siteClientResetNeeded = siteClientResetNeeded | somethingChanged;

            do
            {
                somethingChanged = false;
                foreach (IMediaGenerator generator in UnityResolver.UnityContainer.ResolveAll(typeof(IMediaGenerator)))
                {
                    somethingChanged = generator.RegisterSelf("StartupRegistrar") || somethingChanged;
                }
                foreach (IFeeProcessor processor in UnityResolver.UnityContainer.ResolveAll(typeof(IFeeProcessor)))
                {
                    somethingChanged = processor.RegisterSelf("StartupRegistrar") || somethingChanged;
                }
                foreach (IListing listing in UnityResolver.UnityContainer.ResolveAll(typeof(IListing)))
                {
                    somethingChanged = listing.RegisterSelf("StartupRegistrar") || somethingChanged;
                }
            } while (somethingChanged);

            siteClientResetNeeded = siteClientResetNeeded | somethingChanged;

            //call each payment provider RegisterSelf() method
            somethingChanged = false;
            foreach (IPaymentProvider paymentProvider in UnityResolver.UnityContainer.ResolveAll(typeof(IPaymentProvider)))
            {
                somethingChanged = paymentProvider.RegisterSelf();
            }

            siteClientResetNeeded = siteClientResetNeeded | somethingChanged;

            //If FrameWorx is running InProcess (In IIS, by itself...)
            if (Clients.ClientFactory.ExecutionContext == Clients.ClientFactory.ExecutionContextEnum.InProcess)
            {
                _schedulerService = new SchedulerService();
                _schedulerService.StartThreads(Server.MapPath("~"));

                _notifierService = new NotifierService();
                _notifierService.StartThreads();                
            }

            //run any custom application initializers
            foreach (ICustomApplicationInitializer initializer in UnityResolver.UnityContainer.ResolveAll(typeof(ICustomApplicationInitializer)))
            {
                initializer.InitializeApplication();
            }

            SiteClient.UpdateCustomCurrencies(ConfigurationManager.AppSettings["CustomCurrencies"]);

            //detect connection string for an rw7 import and enable site property, if applicable
            if (ConfigurationManager.ConnectionStrings["db_connection_rw7"] != null)
            {
                if (!SiteClient.BoolSetting(Strings.SiteProperties.ImportUsersEnabled) ||
                    !SiteClient.BoolSetting(Strings.SiteProperties.ImportCategoriesEnabled) ||
                    !SiteClient.BoolSetting(Strings.SiteProperties.ImportListingsEnabled) ||
                    !SiteClient.BoolSetting(Strings.SiteProperties.ImportRegionsEnabled) ||
                    !SiteClient.BoolSetting(Strings.SiteProperties.ImportFeedbacksEnabled))
                {
                    SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportUsersEnabled, true.ToString(), "en");
                    SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportCategoriesEnabled, true.ToString(), "en");
                    SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportListingsEnabled, true.ToString(), "en");
                    SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportRegionsEnabled, true.ToString(), "en");
                    SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportFeedbacksEnabled, true.ToString(), "en");
                    siteClientResetNeeded = true;
                }
            }
            else if (SiteClient.BoolSetting(Strings.SiteProperties.ImportUsersEnabled) ||
                    SiteClient.BoolSetting(Strings.SiteProperties.ImportCategoriesEnabled) ||
                    SiteClient.BoolSetting(Strings.SiteProperties.ImportListingsEnabled) ||
                    SiteClient.BoolSetting(Strings.SiteProperties.ImportRegionsEnabled) ||
                    SiteClient.BoolSetting(Strings.SiteProperties.ImportFeedbacksEnabled))
            {
                SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportUsersEnabled, false.ToString(), "en");
                SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportCategoriesEnabled, false.ToString(), "en");
                SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportListingsEnabled, false.ToString(), "en");
                SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportRegionsEnabled, false.ToString(), "en");
                SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.ImportFeedbacksEnabled, false.ToString(), "en");
                siteClientResetNeeded = true;
            }

            //detect changed Active Directory web.config settings and propagate to site properties if applicable
            bool adEnabledInWebConfig = false;
            bool.TryParse(ConfigurationManager.AppSettings["ActiveDirectoryEnabled"] ?? "false", out adEnabledInWebConfig);
            string adDomainInWebConfig = ConfigurationManager.AppSettings["ActiveDirectoryDomain"] ?? string.Empty;
            string adAdminInWebConfig = ConfigurationManager.AppSettings["ActiveDirectoryAdminUserName"] ?? string.Empty;
            if (SiteClient.BoolSetting(SiteProperties.ActiveDirectoryEnabled) != adEnabledInWebConfig ||
                SiteClient.TextSetting(SiteProperties.ActiveDirectoryDomain) != adDomainInWebConfig ||
                SiteClient.TextSetting(SiteProperties.ActiveDirectoryAdminUserName) != adAdminInWebConfig)
            {
                SiteClient.UpdateSetting(SystemActors.SystemUserName, SiteProperties.ActiveDirectoryEnabled, adEnabledInWebConfig.ToString(), "en");
                SiteClient.UpdateSetting(SystemActors.SystemUserName, SiteProperties.ActiveDirectoryDomain, adDomainInWebConfig, "en");
                SiteClient.UpdateSetting(SystemActors.SystemUserName, SiteProperties.ActiveDirectoryAdminUserName, adAdminInWebConfig, "en");
                siteClientResetNeeded = true;
            }

            _queueManager = UnityResolver.Get<IQueueManager>();
            if (_queueManager.GetType() != typeof(QueueingDisabled))
            {
                if (!SiteClient.BoolSetting(Strings.SiteProperties.SignalREnabled) ||
                    !SiteClient.BoolSetting(Strings.SiteProperties.AsyncActionProcessing))
                {
                    bool sitePropResetNeeded = false;
                    if (!SiteClient.BoolSetting(SiteProperties.SignalREnabled))
                    {
                        SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.SignalREnabled, true.ToString(), "en");
                        sitePropResetNeeded = true;
                    }
                    bool asyncBiddingRequested = true;
                    bool.TryParse(ConfigurationManager.AppSettings["AsyncActionProcessingEnabled"], out asyncBiddingRequested);
                    if (asyncBiddingRequested && !SiteClient.BoolSetting(SiteProperties.AsyncActionProcessing))
                    {
                        SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.AsyncActionProcessing, true.ToString(), "en");
                        sitePropResetNeeded = true;
                    }
                    if (sitePropResetNeeded)
                    {
                        siteClientResetNeeded = true;
                    }
                }
            }
            else
            {
                if (SiteClient.BoolSetting(Strings.SiteProperties.SignalREnabled) ||
                    SiteClient.BoolSetting(Strings.SiteProperties.AsyncActionProcessing))
                {
                    //queueing is disabled but SignalR is enabled -- set applicable site properties
                    SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.SignalREnabled, false.ToString(), "en");
                    SiteClient.UpdateSetting(Strings.SystemActors.SystemUserName, Strings.SiteProperties.AsyncActionProcessing, false.ToString(), "en");
                    siteClientResetNeeded = true;
                }
            }

            if (siteClientResetNeeded)
            {
                SiteClient.Reset();
            }

            //note: this must be called prior to any GlobalHost function calls because the underlying GlobalHost.HubPipeline.AddModule() will throw an exception if done afterwards
            GlobalConfiguration.Configuration.EnsureInitialized();

            if (SiteClient.BoolSetting(SiteProperties.SignalREnabled))
            {
                if (!_queueManager.IsSimpleQueueArchitecture())
                {
                    _queueManager.OnListingDTTMChange(Strings.QueueNames.ListingDTTMUpdateForSignalR, int.Parse(ConfigurationManager.AppSettings["ListingDTTMUpdateForSignalRThreads"]), data =>
                    {
                    //data.DTTMString = data.DTTM.AddHours(SiteClient.TimeZoneOffset).ToString(CultureInfo.InvariantCulture);
                    TimeZoneInfo siteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(SiteClient.SiteTimeZone);
                        data.DTTMString = TimeZoneInfo.ConvertTime(data.DTTM, TimeZoneInfo.Utc, siteTimeZone).ToString(CultureInfo.InvariantCulture);
                        GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.ListingID.ToString(CultureInfo.InvariantCulture)).updateListingDTTM(data);
                    });

                    _queueManager.OnListingActionChange(Strings.QueueNames.ListingActionUpdateForSignalR, int.Parse(ConfigurationManager.AppSettings["ListingActionUpdateForSignalRThreads"]), data =>
                    {
                        GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.ListingID.ToString(CultureInfo.InvariantCulture)).updateListingAction(data);
                    });

                    _queueManager.OnListingStatusChange(Strings.QueueNames.ListingStatusUpdateForSignalR, int.Parse(ConfigurationManager.AppSettings["ListingStatusUpdateForSignalRThreads"]), data =>
                    {
                        GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.ListingID.ToString(CultureInfo.InvariantCulture)).updateListingStatus(data);
                    });

                    _queueManager.OnEventStatusChange(Strings.QueueNames.EventStatusUpdateForSignalR, int.Parse(ConfigurationManager.AppSettings["EventStatusUpdateForSignalRThreads"]), data =>
                    {
                        GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.EventID.ToString(CultureInfo.InvariantCulture)).updateEventStatus(data);
                    });

                    bool forceTest = bool.Parse(ConfigurationManager.AppSettings["ForceAsyncBidWaitForTesting"]);
                    _queueManager.OnListingActionResponse(Strings.QueueNames.ListingActionResponse, int.Parse(ConfigurationManager.AppSettings["ListingActionResponseForSignalRThreads"]), data =>
                    {
                        if (forceTest)
                            ((AutoResetEvent)
                                Application[
                                    data.Action_UserName + data.Action_ListingID.ToString(CultureInfo.InvariantCulture)]).Set();
                        GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.Action_UserName).listingActionResponse(data);
                    });

                    _queueManager.OnCurrentTimeUpdate(Strings.QueueNames.GetCurrentTimeForSignalR, int.Parse(ConfigurationManager.AppSettings["GetCurrentTimeForSignalRThreads"]), data =>
                    {
                    //data = data.AddHours(SiteClient.TimeZoneOffset);
                    TimeZoneInfo siteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(SiteClient.SiteTimeZone);
                        data = TimeZoneInfo.ConvertTime(data, TimeZoneInfo.Utc, siteTimeZone);
                        GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.All.updateCurrentTime(data.ToString(CultureInfo.InvariantCulture));
                    });
                }
                else
                {
                    bool forceTest = bool.Parse(ConfigurationManager.AppSettings["ForceAsyncBidWaitForTesting"]);
                    _queueManager.OnSignalRMessage(untypedData => {
                        switch (untypedData.MessageType)
                        {
                            //var x = new ListingDTTMChange
                            case "ListingDTTMChange":
                                {
                                    var data = (ListingDTTMChange)untypedData.MessageData;
                                    TimeZoneInfo siteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(SiteClient.SiteTimeZone);
                                    data.DTTMString = TimeZoneInfo.ConvertTime(data.DTTM, TimeZoneInfo.Utc, siteTimeZone).ToString(CultureInfo.InvariantCulture);
                                    GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.ListingID.ToString(CultureInfo.InvariantCulture)).updateListingDTTM(data);
                                    break;
                                }
                            case "ListingActionChange":
                                {
                                    var data = (ListingActionChange)untypedData.MessageData;
                                    GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.ListingID.ToString(CultureInfo.InvariantCulture)).updateListingAction(data);
                                    break;
                                }
                            case "ListingStatusChange":
                                {
                                    var data = (ListingStatusChange)untypedData.MessageData;
                                    GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.ListingID.ToString(CultureInfo.InvariantCulture)).updateListingStatus(data);
                                    break;
                                }
                            case "EventStatusChange":
                                {
                                    var data = (EventStatusChange)untypedData.MessageData;
                                    GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.EventID.ToString(CultureInfo.InvariantCulture)).updateEventStatus(data);
                                    break;
                                }
                            case "ListingActionResponse":
                                {
                                    var data = (ListingActionResponse)untypedData.MessageData;
                                    if (forceTest)
                                        ((AutoResetEvent)
                                            Application[
                                                data.Action_UserName + data.Action_ListingID.ToString(CultureInfo.InvariantCulture)]).Set();
                                    GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.Group(data.Action_UserName).listingActionResponse(data);
                                    break;
                                }
                            case "CurrentTimeUpdate":
                                {
                                    var data = (DateTime)untypedData.MessageData;
                                    TimeZoneInfo siteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(SiteClient.SiteTimeZone);
                                    data = TimeZoneInfo.ConvertTime(data, TimeZoneInfo.Utc, siteTimeZone);
                                    GlobalHost.ConnectionManager.GetHubContext<ListingHub>().Clients.All.updateCurrentTime(data.ToString(CultureInfo.InvariantCulture));
                                    break;
                                }
                        }
                    });
                }
            }

            stopwatch.Stop();
            LogManager.WriteLog("App init completed", "Application_Start()", "MVC", TraceEventType.Start, null, null,
                new Dictionary<string, object>() { { "Startup Time (MS)", stopwatch.ElapsedMilliseconds } });
        }

        protected void Application_End()
        {
            //If FrameWorx is running InProcess (In IIS, by itself...)
            if (Clients.ClientFactory.ExecutionContext == Clients.ClientFactory.ExecutionContextEnum.InProcess)
            {
                //application is ending, no reason to freak out if these things are already null
                if (_notifierService != null)
                {
                    _notifierService.StopThreads();
                }
                if (_schedulerService != null)
                {
                    _schedulerService.StopThreads();                    
                }
            }
        }
    }
}
