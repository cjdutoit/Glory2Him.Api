// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using G2H.Api.Web.Tests.Acceptance.Models.PostTypes;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.PostTypes
{
    public partial class PostTypesApiTests
    {
        [Fact]
        public async Task ShouldGetAllPostTypesAsync()
        {
            // given
            List<PostType> reactions = await CreatePostTypesAsync();
            List<PostType> expectedPostTypes = reactions;

            // when
            List<PostType> actualPostTypes = await this.apiBroker.GetAllPostTypesAsync();

            // then
            foreach (PostType expectedPostType in expectedPostTypes)
            {
                PostType actualPostType = actualPostTypes.Single(status => status.Id == expectedPostType.Id);
                actualPostType.Name.Should().Be(expectedPostType.Name);
            }
        }
    }
}
