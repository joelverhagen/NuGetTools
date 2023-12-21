using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Knapcode.NuGetTools.Website;

[HtmlTargetElement("parse-version")]
public class ParseVersionTagHelper : ActionTagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    public ParseVersionTagHelper(IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory)
    {
        _urlHelperFactory = urlHelperFactory;
    }

    protected override string ActionName => nameof(HomeController.ParseVersion);

    protected override RouteValueDictionary GetRouteValues(string value)
    {
        return new RouteValueDictionary(new { version = value });
    }
}
