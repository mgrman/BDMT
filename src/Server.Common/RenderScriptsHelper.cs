﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BDMT.Server
{
    public static class HtmlExtensions
    {
        public static IHtmlContent RenderScripts(this IHtmlHelper htmlHelper, IEnumerable<string> scripts)
        {
            var builder = new HtmlContentBuilder();
            foreach (var script in scripts)
            {
                TagBuilder tag = new TagBuilder("script");
                if (script.EndsWith(".js"))
                {
                    tag.MergeAttribute("src", script);
                }
                else
                {
                    tag.InnerHtml.Append(script);
                }
                builder.AppendHtml(tag);
            }

            return builder;
        }
    }
}