using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Shekordo.UI.TagHelpers
{
    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Номер текущей страницы
        public int CurrentPage { get; set; }

        // Общее количество страниц
        public int TotalPages { get; set; }

        // Имя категории объектов
        public string? Category { get; set; }

        // Признак страниц администратора (Razor Pages)
        public bool Admin { get; set; } = false;

        // Номер предыдущей страницы
        int Prev
        {
            get => CurrentPage == 1 ? 1 : CurrentPage - 1;
        }

        // Номер следующей страницы
        int Next
        {
            get => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;
        }

        public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.AddClass("row", HtmlEncoder.Default);

            var nav = new TagBuilder("nav");
            nav.Attributes.Add("aria-label", "pagination");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            // Кнопка предыдущей страницы
            ul.InnerHtml.AppendHtml(
                CreateListItem(Category, Prev, "<span aria-hidden=\"true\">&laquo;</span>"));

            // Кнопки с номерами страниц
            for (int i = 1; i <= TotalPages; i++)
            {
                ul.InnerHtml.AppendHtml(
                    CreateListItem(Category, i, i.ToString()));
            }

            // Кнопка следующей страницы
            ul.InnerHtml.AppendHtml(
                CreateListItem(Category, Next, "<span aria-hidden=\"true\">&raquo;</span>"));

            nav.InnerHtml.AppendHtml(ul);
            output.Content.AppendHtml(nav);
        }

        private TagBuilder CreateListItem(string? category, int pageNo, string content)
        {
            var li = new TagBuilder("li");

            string url;

            // Для страниц администратора используем Razor Pages
            if (Admin)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                url = _linkGenerator.GetPathByPage(httpContext, page: "./Index", values: new { pageNo }) ?? "#";
            }
            else
            {
                // Для обычных страниц используем MVC
                url = _linkGenerator.GetPathByAction(
                    action: "Index",
                    controller: "Product",
                    values: new { category = category, pageNo = pageNo }) ?? "#";
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            a.Attributes.Add("href", url);
            a.InnerHtml.AppendHtml(content);

            // Если текущая страница — делаем её активной
            if (pageNo == CurrentPage)
            {
                li.AddCssClass("page-item active");
            }
            // Если это кнопка prev/next и мы на границе — делаем неактивной
            else if (pageNo == Prev && CurrentPage == 1
                  || pageNo == Next && CurrentPage == TotalPages)
            {
                li.AddCssClass("page-item disabled");
            }
            else
            {
                li.AddCssClass("page-item");
            }

            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}