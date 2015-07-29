using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsAPI
{
    public static class APIConstants
    {
        public const string API_VERSION_HEADER = "X-API-VERSION";
        public const string API_VERSION_VALUE	= "1";
        public const string DEFAULT_URL_SERVER	= "http://aspmvcsignalrtest.azurewebsites.net/";
        public const string FormsAuthentication_FormsCookieName	= ".ASPXAUTH";
        public const string URL_ACCOUNTS	= "api/account";
        public const string URL_CREATE_PRODUCT_ENTRY	= "api/productlists/{0}/create";
        public const string URL_CREATE_PRODUCT_LIST	= "api/productlists/create";
        public const string URL_DELETE_PRODUCT_ENTRY	= "api/productlists/{0}/delete/{1}";
        public const string URL_DELETE_PRODUCT_LIST	= "api/productlists/delete/{0}";
        public const string URL_EDIT_PRODUCT_ENTRY	= "api/productlists/{0}/edit/{1}";
        public const string URL_LOGIN_USER	= "api/account/login";
        public const string URL_LOGOUT_USER	= "api/account/logout";
        public const string URL_PRODUCT_ENTRIES	= "api/productlists/{0}";
        public const string URL_PRODUCT_LISTS	= "api/productlists";
        public const string URL_REGISTER_USER	= "api/account/register";
        public const string URL_SIGNALR_HUB	= "/signalr";
    }
}
