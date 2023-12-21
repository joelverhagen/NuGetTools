using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Knapcode.NuGetTools.Website;

[HtmlTargetElement("parse-framework")]
public class ParseFrameworkTagHelper : ActionTagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    public ParseFrameworkTagHelper(IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory)
    {
        _urlHelperFactory = urlHelperFactory;
    }

    protected override string ActionName => nameof(HomeController.ParseFramework);

    protected override RouteValueDictionary GetRouteValues(string value)
    {
        return new RouteValueDictionary(new { framework = value });
    }
}
