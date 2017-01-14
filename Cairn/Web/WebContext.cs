using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cairn.Web {
    public class WebContext : Context, IContext {
        private readonly HttpContext _httpContext;
        private readonly HttpRequest _httpRequest;
        private readonly HttpResponse _httpResponse;
        private readonly UrlInfo _urlInfo;
        private readonly Dictionary<string, string[]> _queryParameters;
        private readonly IEnumerable<string> _flags;
        private readonly HttpApplication _httpApplication;

        protected HttpContext HttpContext {
            get { return _httpContext; }
        }

        public HttpRequest Request {
            get { return _httpRequest; }
        }

        public HttpResponse Response {
            get { return _httpResponse; }
        }

        public UrlInfo UrlInfo {
            get { return _urlInfo; }
        }

        public Dictionary<string, string[]> QueryParameters {
            get { return _queryParameters; }
        }

        public IEnumerable<string> Flags {
            get { return _flags; }
        }

        // not sure how I feel about this access
        public HttpApplication HttpApplication {
            get { return _httpApplication; }
        }

        public WebContext(BehaviourService application, HttpContext httpContext) : base(application) {
            this._httpContext = httpContext;
            this._httpApplication = this.HttpContext.ApplicationInstance as HttpApplication;
            this._httpRequest = this._httpContext.Request;
            this._httpResponse = this._httpContext.Response;
            _urlInfo = new UrlInfo(this._httpRequest.Url);                                   

            Dictionary<string, string[]> parameters = new Dictionary<string, string[]>();
            List<string> flags = new List<string>();

            for (int i = 0; i < this._httpRequest.QueryString.Count; i++) {
                string key = this._httpRequest.QueryString.GetKey(i);
                string[] values = this._httpRequest.QueryString.GetValues(i);
                // check for valueless parameters and use as flags
                if (key == null && values != null) {
                    flags.InsertRange(0, values);
                } else {
                    if (values != null) parameters.Add(key, values);
                }
            }

            _flags = flags;
            _queryParameters = parameters;
        }
    }
}
