using System.Threading.Tasks;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CORSProxy
{
    public class ProxyController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ProxyOptions proxyOptions;

        public ProxyController(IConfiguration configuration, ProxyOptions proxyOptions)
        {
            Configuration = configuration;
            this.proxyOptions = proxyOptions;
        }

        [Route("{*args}")]
        public Task Index()
        {
            return this.ProxyAsync(Configuration.GetValue<string>("ProxyHostAddress").TrimEnd('/') + '/' + Request.Path + Request.QueryString, proxyOptions);
        }
    }
}
