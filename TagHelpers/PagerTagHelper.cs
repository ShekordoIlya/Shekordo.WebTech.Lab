using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System.Text.Encodings.Web;

namespace Shekordo.UI.TagHelpers;

[HtmlTargetElement("pager")]
public class PagerTagHelper : TagHelper
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Category { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (TotalPages <= 1) return;

        output.TagName = "div";
        output.AddClass("row", HtmlEncoder.Default);
        output.AddClass("mt-4", HtmlEncoder.Default);

        var nav = new TagBuilder("nav");
        nav.Attributes.Add("aria-label", "Page navigation");

        var ul = new TagBuilder("ul");
        ul.AddCssClass("pagination");
        ul.AddCssClass("justify-content-center");

        int prev = CurrentPage == 1 ? 1 : CurrentPage - 1;
        int next = CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;

        ul.InnerHtml.AppendHtml(CreateListItem(Category, prev, "&laquo;", CurrentPage == 1));

        for (int i = 1; i <= TotalPages; i++)
        {
            ul.InnerHtml.AppendHtml(CreateListItem(Category, i, i.ToString(), false, i == CurrentPage));
        }

        ul.InnerHtml.AppendHtml(CreateListItem(Category, next, "&raquo;", CurrentPage == TotalPages));

        nav.InnerHtml.AppendHtml(ul);
        output.Content.AppendHtml(nav);
    }

    private TagBuilder CreateListItem(string? category, int pageNo, string displayText, bool isDisabled, bool isActive = false)
    {
        var li = new TagBuilder("li");
        li.AddCssClass("page-item");
        if (isDisabled) li.AddCssClass("disabled");
        if (isActive) li.AddCssClass("active");

        var a = new TagBuilder("a");
        a.AddCssClass("page-link");

        if (!isDisabled)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var url = _linkGenerator.GetPathByAction(
                    httpContext,
                    action: "Index",
                    controller: "Product",
                    values: new { category, pageNo });
                a.Attributes.Add("href", url);
            }
        }

        a.InnerHtml.AppendHtml(displayText);
        li.InnerHtml.AppendHtml(a);
        return li;
    }
}