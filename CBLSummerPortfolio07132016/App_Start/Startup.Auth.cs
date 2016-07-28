using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin.Security.Providers.LinkedIn;
using Owin;
using Microsoft.Owin.Security.MicrosoftAccount;
using CBLSummerPortfolio07132016.Models;

namespace CBLSummerPortfolio07132016
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                //ReturnUrlParameter = "returnTo",      //code to maybe redirect(src = http://erlend.oftedal.no/blog/?blogid=55) 
                Provider = new CookieAuthenticationProvider

                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            ////Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            /*Facebook app needs to be made published in order for other people to sign in*/
            app.UseFacebookAuthentication(
               appId: "539616076226277",
               appSecret: "e6f625e56360413a4d20b9dbf230a91e");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "1048796132137-9f7u27g1h6s5a3vaotju39tdrud3qh07.apps.googleusercontent.com",
                ClientSecret = "wp5PknO2-X38eOV_VJ3UFS2m"
            });

            app.UseLinkedInAuthentication(
               "78wkpwegb43umm",
               "qcmjvR9RepnTuiH");

            /*The following needs to be updated to published site in order to function; local hosting is not supported*/
            app.UseTwitterAuthentication(
                consumerKey: "Lbvb2ad10M3y2Teu4ViM6Rhvj",
                consumerSecret: "sAwtVIpMCKIqcl5TREnNaMWurlQWswFkGECAnKx1k1zJCCSTCy");
        }
    }
}