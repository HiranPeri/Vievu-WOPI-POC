using MainWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Net.Http;
//using System.Web.Mvc;
using System.Web.Http.Filters;
using System.Web.Mvc;
using WopiHost.Helpers;

namespace WopiHost.Models
{
    public class ProofKeyValidatorFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext httpContext)
        {
            string AccessToken = string.Empty,
                    Url = string.Empty,
                    ProofSignature = string.Empty,
                    OldProofSignature = string.Empty;
            long XWopiTimeStamp = 0;
            WopiAppHelper wopiHelper = new WopiAppHelper(WebConfigurationManager.AppSettings["appDiscoveryXml"]);

            ProofKeysHelper proofer = new ProofKeysHelper(wopiHelper.GetProofKey(), wopiHelper.GetProofKey(false));
            var queryString = httpContext.Request
                            .GetQueryNameValuePairs()
                            .ToDictionary(x => x.Key, x => x.Value);

            if (queryString["access_token"] != null)
            {
                AccessToken = queryString["access_token"];
            }
            Url = httpContext.Request.RequestUri.ToString();            
            if (httpContext.Request.Headers.Any(s => s.Key.Equals("X-WOPI-Proof")))
            {
                ProofSignature = httpContext.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-WOPI-Proof")).Value.First();
            }
            if (httpContext.Request.Headers.Any(x => x.Key.Equals("X-WOPI-ProofOld")))
            {
                OldProofSignature = httpContext.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-WOPI-ProofOld")).Value.First();
            }
            if (httpContext.Request.Headers.Any(x => x.Key.Equals("X-WOPI-TimeStamp")))
            {
                XWopiTimeStamp = Convert.ToInt64(httpContext.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-WOPI-TimeStamp")).Value.First());
            }
            proofer.Validate(new ProofKeyValidationInput(AccessToken, XWopiTimeStamp, Url, ProofSignature, OldProofSignature));
        }
    }
}