﻿using System;
using System.Collections.Generic;
using Grapevine.Util;
using Shouldly;
using Xunit;

namespace Grapevine.Tests.Util
{
    public class ContentTypeMetadataTester
    {
        [Fact]
        public void is_attribute_multiple_false()
        {
            var attributes = (IList<AttributeUsageAttribute>)typeof(ContentTypeMetadata).GetCustomAttributes(typeof(AttributeUsageAttribute), false);
            attributes.Count.ShouldBe(1);

            var attribute = attributes[0];
            attribute.AllowMultiple.ShouldBeFalse();
        }
    }
}
