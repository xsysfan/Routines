﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;
//using Vse.AdminkaV1.Injected;

namespace Vse.AdminkaV1.Web
{
    /// <summary>
    /// Usage like <markdown>@Vse.AdminkaV1.AdminkaManager.Markdown(Model)</markdown> doesn't work as you can expect. 
    /// Because of Razor @... operator changes text e.g. new lines become encoded through escape characters.
    /// </summary>
    [HtmlTargetElement("markdown")]
    [OutputElementHint("p")]
    public class MarkdownTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var c = (await output.GetChildContentAsync()).GetContent();
            //    var html = InjectedManager.Markdown(c);
            //    var htmlString = new HtmlString(html);
            //    output.Content.SetHtmlContent(htmlString);
        }
    }
}
