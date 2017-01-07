using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Knapcode.NuGetTools.Website.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("parse-framework")]
    public class ParseFrameworkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ParseFrameworkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // get the framework
            var valueAttribute = context.AllAttributes["value"];
            if (valueAttribute == null ||
                valueAttribute.Value == null ||
                !(valueAttribute.Value is HtmlString))
            {
                return;
            }

            var framework = ((HtmlString)valueAttribute.Value).Value;

            // get the action URL
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            var href = urlHelper.Action(
                nameof(HomeController.ParseFramework),
                HomeController.ControllerName,
                new { framework = framework });

            // build the output
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content = new DefaultTagHelperContent().Append(framework);
            output.Attributes.Add("href", href);
        }
    }
}
