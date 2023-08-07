using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AjaxPageHelper;

/// <summary>
/// This pagination Helper configured By MohamadHosein Hajiloo in 2023/07/08
/// 
/// You can call your Ajax method by setting the JavaScript function, which must have three inputs, 1-CurrentPage, 2-MaxPagerItems,
/// and 3-ItemsPerPage Respectively, or if you don't want to, you can leave it blank and customize it using the attributes (
/// data-pageId , data-pageCount , data-pageSize ) of each page item.
/// Complete the operation of sending the request to your action method
/// 
/// </summary>
[HtmlTargetElement("pager", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PagerHelper : TagHelper
{
    private readonly HttpContext _httpContext;
    private readonly Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor _actionContextAccessor;


    [ViewContext]
    public ViewContext ViewContext { get; set; }

    public PagerHelper(IHttpContextAccessor accessor, IActionContextAccessor actionContextAccessor)
    {
        _actionContextAccessor = actionContextAccessor;
        _httpContext = accessor.HttpContext;
    }

    /// <summary>
    /// model that you must passing pagination pages number data
    /// </summary>
    [HtmlAttributeName("pager-model")]
    public Paging Model { get; set; }

    /// <summary>
    /// your js function name after selected on any page item (except current page and under zero number and upper than totalPages for prevent extra requests)
    /// optional
    /// </summary>
    [HtmlAttributeName("pager-ajax-function")]
    public string? AjaxFunctionName
    {
        get;
        set;
    }
    /// <summary>
    /// html class names of pager
    /// optional
    /// </summary>
    [HtmlAttributeName("class")]
    public string? Class
    {
        get;
        set;
    }
    /// <summary>
    ///  html id of pager
    /// optional
    /// </summary>
    [HtmlAttributeName("id")]
    public string? Id
    {
        get;
        set;
    }

    /// <summary>
    ///  show item per page dropdown or not
    /// optional
    /// </summary>
    [HtmlAttributeName("show-item-per-page-select")]
    public bool ShowItemPerPage
    {
        get;
        set;
    } = true;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Model == null)
        {
            return;
        }

        if (Model.MaxPagerItems == 0)
        {
            return;
        }

        if (ShowItemPerPage)
        {
            string selectStr = "";
            bool itemPerPageIsInDefaultNumbers = false;
            var itemPerPages = new int[]
            {
                10, 25, 50, 100
            };
            foreach (var itemPerPage in itemPerPages)
            {
                if (Model.ItemsPerPage == itemPerPage)
                {
                    itemPerPageIsInDefaultNumbers = true;
                    selectStr += $" <option value=\"{itemPerPage}\" selected>{itemPerPage}</option>";
                }
                else
                {
                    selectStr += $" <option value=\"{itemPerPage}\">{itemPerPage}</option>";

                }
            }
            if (!itemPerPageIsInDefaultNumbers)
            {
                selectStr += $" <option value=\"{Model.ItemsPerPage}\" selected>{Model.ItemsPerPage}</option>";
            }
            output.Content.AppendHtml(" <div class=\"col-sm-12 col-md-6\">\r\n     <div class=\"me-3\">\r\n       " +
                                      "     <div class=\" pb-3 pb-md-0 m-0\" id=\"kt_datatable_length\">\r\n\r\n  " +
                                      "              <label>\r\n              " +
                                      "      انتخاب\r\n              " +
                                      "      <select id=\"pager-item-per-page\"  class=\"form-select\">\r\n " +

                                      selectStr +
                                      "         </select>\r\n         " +
                                      "       </label>\r\n       " +
                                      "     </div>\r\n     " +
                                      "   </div>\r\n   " +
                                      "  \r\n  " +
                                      "  </div>\r\n" +
                                      "<div class=\"col-sm-12 col-md-6\">\r\n        <div  class=\" paging_simple_numbers\" >\r\n\r\n  ");
        }
        else
        {

        }

        output.Content.AppendHtml("<div class=\"col-sm-12 \">\r\n        <div  class=\" paging_simple_numbers\" >\r\n\r\n  ");


        output.TagName = "";
        output.Content.AppendHtml($"<ul id=\"{Id}\" class=\"pagination {Class}\">");

        AddPageLink(output, "قبلی", "previous",
            !string.IsNullOrEmpty(AjaxFunctionName)
                ? AjaxFunctionName + $"({Model.CurrentPage - 1}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
                : "", Model.CurrentPage - 1, Model.MaxPagerItems, Model.ItemsPerPage);
        if (Model.MaxPagerItems > 10)
        {
            if (Model.CurrentPage >= 10)
            {
                AddPageLink(output, "1", "",
                    !string.IsNullOrEmpty(AjaxFunctionName)
                        ? AjaxFunctionName + $"({1}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
                        : "", 1, Model.MaxPagerItems, Model.ItemsPerPage);
                AddPageLink(output, "...", "", "", clickable: false);
            }

            if (Model.CurrentPage >= 10 && Model.CurrentPage <= Model.MaxPagerItems - 10)
            {
                for (int i = Model.CurrentPage - 5; i <= Model.CurrentPage + 5; i++)
                {
                    if (i == Model.CurrentPage)
                    {
                        AddCurrentPageLink(output, i, "");
                    }
                    else
                    {
                        AddPageLink(output, i.ToString(), "", !string.IsNullOrEmpty(AjaxFunctionName)
                                ? AjaxFunctionName +
                                  $"({i}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
                                : ""
                            , i, Model.MaxPagerItems, Model.ItemsPerPage);
                    }
                }

                AddPageLink(output, "...", "", "", clickable: false);
                AddPageLink(output, Model.MaxPagerItems.ToString(), "",
                    !string.IsNullOrEmpty(AjaxFunctionName)
                        ? AjaxFunctionName + $"({Model.MaxPagerItems}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
                        : "", Model.MaxPagerItems, Model.MaxPagerItems, Model.ItemsPerPage);

            }
            else if (Model.CurrentPage >= 10)
            {
                for (int i = Model.MaxPagerItems - 10; i <= Model.MaxPagerItems; i++)
                {
                    if (i == Model.CurrentPage)
                    {
                        AddCurrentPageLink(output, i, "");
                    }
                    else
                    {
                        AddPageLink(output, i.ToString(), "", !string.IsNullOrEmpty(AjaxFunctionName)
                                ? AjaxFunctionName +
                                  $"({i}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
                                : ""
                            , i, Model.MaxPagerItems, Model.ItemsPerPage);
                    }
                }
            }
            else if (Model.CurrentPage < 10)
            {
                for (int i = 1; i <= 10; i++)
                {
                    if (i == Model.CurrentPage)
                    {
                        AddCurrentPageLink(output, i, "");
                    }
                    else
                    {
                        AddPageLink(output, i.ToString(), "",
                            !string.IsNullOrEmpty(AjaxFunctionName)
                                ? AjaxFunctionName +
                                  $"({Model.CurrentPage - 1}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
                                : "", Model.CurrentPage - 1, Model.MaxPagerItems, Model.ItemsPerPage);

                    }
                }
            }
        }
        else
        {
            for (int i = 1; i <= Model.MaxPagerItems; i++)
            {
                if (i == Model.CurrentPage)
                {
                    AddCurrentPageLink(output, i, "");
                }
                else
                {
                    AddPageLink(output, i.ToString(), "", !string.IsNullOrEmpty(AjaxFunctionName)
                            ? AjaxFunctionName +
                              $"({i}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
                            : ""
                        , i, Model.MaxPagerItems, Model.ItemsPerPage);
                }
            }




        }
        AddPageLink(output, "بعدی", "next", !string.IsNullOrEmpty(AjaxFunctionName)
            ? AjaxFunctionName +
              $"({Model.CurrentPage + 1}, {Model.MaxPagerItems}, {Model.ItemsPerPage})"
            : "", Model.CurrentPage + 1, Model.MaxPagerItems, Model.ItemsPerPage);

        output.Content.AppendHtml("</ul>\r\n\r\n       " + " </div>\r\n    </div>");
        output.Content.AppendHtml($" <input type=\"hidden\" id=\"pager-page-count\" value=\"{Model.MaxPagerItems}\"/>\r\n    <input type=\"hidden\" id=\"pager-page-size\" value=\"{Model.ItemsPerPage}\"/>");
    }

    private void AddPageLink(TagHelperOutput output, string text, string liClass, string onClick, int pageId = 1, int pageCount = 1, int pageSize = 20, bool clickable = true)
    {
        if (clickable)
        {
            output.Content.AppendHtml($"<li class=\"page-item {liClass}\"><a class=\"page-link actional-page-link\" onclick=\"{onClick}\" href=\"#\" data-pageId=\"{pageId}\" data-pageCount=\"{pageCount}\" data-pageSize=\"{pageSize}\">");

            output.Content.AppendHtml(text);
            output.Content.AppendHtml("</a>");
            output.Content.AppendHtml("</li>");
        }
        else
        {
            output.Content.AppendHtml($"<li class=\"page-item {liClass}\"><a class=\"page-link \" href=\"#\" >");
            output.Content.AppendHtml(text);
            output.Content.AppendHtml("</a>");
            output.Content.AppendHtml("</li>");
        }

    }

    private void AddCurrentPageLink(TagHelperOutput output, int page, string liClass, string onClick = "")
    {
        output.Content.AppendHtml("<li class=\"page-item active\">");
        output.Content.AppendHtml($"<a class=\"page-link \" onclick=\"{onClick}\" href=\"#\"  >");
        output.Content.AppendHtml(page.ToString());
        output.Content.AppendHtml("</a>");
        output.Content.AppendHtml("</li>");
    }
}