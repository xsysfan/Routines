﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    /// <summary>
    /// Just a sample of TagHelper. Exactly this tagHelper is not working  because it has a problem with multiline content (and exception text is multiline).
    /// Razor @... operator changes the text therefore new lines become encoded through escape characters.
    /// Need further investigation.
    /// </summary>
    [HtmlTargetElement("markdown")]
    [OutputElementHint("p")]
    public class MarkdownTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var c = (await output.GetChildContentAsync()).GetContent();
            var html = InjectedManager.Markdown(c);
            var htmlString = new HtmlString(html);
            output.Content.SetHtmlContent(htmlString);
        }
    }
}