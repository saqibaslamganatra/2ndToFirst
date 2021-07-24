using Recaptcha.Web.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using RainWorx.FrameWorx.Clients;

namespace RainWorx.FrameWorx.MVC.Helpers
{
    public static class RecaptchaMVC
    {
        private const string RESPONSE_FIELD_KEY = "g-recaptcha-response";

        public static MvcHtmlString RenderCaptcha(this HtmlHelper helper)
        {
            return RenderCaptcha(helper, Recaptcha.Web.ColorTheme.Light);
        }

        public static MvcHtmlString RenderCaptcha(this HtmlHelper helper, Recaptcha.Web.ColorTheme theme)
        {
            //publicKey = "6LewRO0SAAAAAJUYn6Bx0r_o0Kv28M00DqGSgD_K"; //ConfigurationManager.AppSettings["RecaptchaPublicKey"];
            string publicKey = SiteClient.Settings["RecaptchaPublicKey"];
            //privateKey = "6LewRO0SAAAAAJAoXVti8TUHirbipRarVZxNu_bs"; //ConfigurationManager.AppSettings["RecaptchaPrivateKey"];
            string privateKey = SiteClient.Settings["RecaptchaPrivateKey"];

            if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
            {
                throw new ApplicationException("reCAPTCHA needs to be configured with a public & private key.");
            }

            return RecaptchaMvcExtensions.RecaptchaWidget(helper, publicKey, theme).ToMvcHtmlString();
        }

        public static async Task ValidateCaptcha(this Controller controller, ValidationResults validation, object sender)
        {
            if (controller.HttpContext.Request.Form.AllKeys.Contains(RESPONSE_FIELD_KEY))
            {
                var recaptchaVerifier = controller.GetRecaptchaVerifier(SiteClient.Settings["RecaptchaPrivateKey"]);
                bool captchaValid = (await recaptchaVerifier.VerifyIfSolvedAsync());
                if (!captchaValid)
                {
                    //invalid captcha
                    validation.AddResult(new ValidationResult("Invalid_Captcha", sender,
                                                          string.Empty, string.Empty, null));
                }
            }
        }
    }
}