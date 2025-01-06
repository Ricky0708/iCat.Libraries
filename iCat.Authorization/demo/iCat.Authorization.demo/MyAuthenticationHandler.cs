using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace iCat.Authorization.demo
{
    public class MyAuthenticationHandler : CookieAuthenticationHandler, IAuthenticationRequestHandler
    {
        public MyAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        public async Task<bool> HandleRequestAsync()
        {
            //(Options).Events.OnRedirectToLogin(null);
            //Context.Response.Redirect("https://google.com.tw");
            return await Task.FromResult(false);
        }
    }
    public class QQ : CookieAuthenticationOptions
    {
        public Func<string, string> OnSS { get; set; } = context => {
            var n = context;
            return n;

        };
    }

}
