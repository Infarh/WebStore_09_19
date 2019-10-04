﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebStore.TagHelpers
{
    [HtmlTargetElement(Attributes = AttributeName)]
    public class ActiveRouteTagHelper : TagHelper
    {
        public const string AttributeName = "is-active-route";



        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll(AttributeName);
        }
    }
}
