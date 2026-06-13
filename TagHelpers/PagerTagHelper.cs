using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Shekordo.UI.TagHelpers;

[HtmlTargetElement("pager")]
public class PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : TagHelper
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Category { get; set; }
    public bool Admin { get; set; }

    int Prev => CurrentPage == 1 ? 1 : CurrentPage - 1;
    int Next => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (TotalPages <= 1)
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = "div";
        output.AddClass("row", HtmlEncoder.Default);
        output.AddClass("mt-4", HtmlEncoder.Default);

        var nav = new TagBuilder("nav");
        nav.Attributes.Add("aria-label", "pagination");

        var ul = new TagBuilder("ul");
        ul.AddCssClass("pagination");
        ul.AddCssClass("justify-content-center");

        ul.InnerHtml.AppendHtml(CreateListItem(Prev, "<span aria-hidden=\"true\">&laquo;</span>", CurrentPage == 1));
        for (var index = 1; index <= TotalPages; index++)
        {
            ul.InnerHtml.AppendHtml(CreateListItem(index, index.ToString(), false, index == CurrentPage));
        }

        ul.InnerHtml.AppendHtml(CreateListItem(Next, "<span aria-hidden=\"true\">&raquo;</span>", CurrentPage == TotalPages));

        nav.InnerHtml.AppendHtml(ul);
        output.Content.AppendHtml(nav);
    }

    TagBuilder CreateListItem(int pageNo, string innerText, bool isDisabled, bool isActive = false)
    {
        var li = new TagBuilder("li");
        li.AddCssClass("page-item");
        if (isDisabled) li.AddCssClass("disabled");
        if (isActive) li.AddCssClass("active");

        var a = new TagBuilder("a");
        a.AddCssClass("page-link");

        if (!isDisabled)
        {
            var httpContext = httpContextAccessor.HttpContext!;
            string url = Admin
                ? linkGenerator.GetPathByPage(httpContext, page: "/Index", values: new { pageNo }) ?? "#"
                : linkGenerator.GetPathByAction(httpContext, action: "Index", controller: "Product",
                    values: new { category = Category, pageNo }) ?? "#";
            a.Attributes.Add("href", url);
        }

        a.InnerHtml.AppendHtml(innerText);
        li.InnerHtml.AppendHtml(a);
        return li;
    }
}
