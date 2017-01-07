using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Knapcode.NuGetTools.Website
{
    [HtmlTargetElement("parse-version-range")]
    public class ParseVersionRangeTagHelper : ActionTagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ParseVersionRangeTagHelper(IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        protected override string ActionName => nameof(HomeController.ParseVersionRange);

        protected override object GetRouteValues(string value)
        {
            return new { versionRange = value };
        }
    }
}
