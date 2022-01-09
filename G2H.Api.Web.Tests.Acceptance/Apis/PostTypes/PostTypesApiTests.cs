// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Tests.Acceptance.Brokers;
using Microsoft.OpenApi.Extensions;
using Tynamix.ObjectFiller;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.PostTypes
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PostTypesApiTests
    {
        private readonly ApiBroker apiBroker;

        public PostTypesApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private async ValueTask<List<Models.PostTypes.PostType>> CreatePostTypesAsync()
        {
            var userId = Guid.NewGuid();
            List<Web.Models.PostTypes.PostType> postTypes = GetStoragePostTypes(userId);

            foreach (var item in postTypes)
            {
                if (!apiBroker.PostTypeService.RetrieveAllPostTypes().Any(status => status.Id == item.Id))
                {
                    await apiBroker.PostTypeService.AddPostTypeAsync(item);
                }
            }

            return GetClientPostTypes(userId);
        }

        private static List<Web.Models.PostTypes.PostType> GetStoragePostTypes(Guid userId)
        {
            List<Web.Models.PostTypes.PostType> postTypes =
                new List<Web.Models.PostTypes.PostType>();

            foreach (Web.Models.PostTypes.PostTypeId statusId
                in Enum.GetValues(typeof(Web.Models.PostTypes.PostTypeId)))
            {
                var dateTimeOffset = DateTimeOffset.UtcNow;
                var filler = new Filler<Web.Models.PostTypes.PostType>();

                filler.Setup()
                    .OnProperty(status => status.Id).Use(statusId)
                    .OnProperty(status => status.Name).Use(statusId.GetDisplayName())
                    .OnType<DateTimeOffset>().Use(dateTimeOffset)
                    .OnProperty(status => status.CreatedByUserId).Use(userId)
                    .OnProperty(status => status.UpdatedByUserId).Use(userId)
                    .OnProperty(status => status.Posts).IgnoreIt();

                postTypes.Add(filler.Create());
            }

            return postTypes;
        }

        private static List<Models.PostTypes.PostType> GetClientPostTypes(Guid userId)
        {
            List<Models.PostTypes.PostType> postTypes = new List<Models.PostTypes.PostType>();

            foreach (Models.PostTypes.PostTypeId statusId in Enum.GetValues(typeof(Models.PostTypes.PostTypeId)))
            {
                var dateTimeOffset = DateTimeOffset.UtcNow;
                var filler = new Filler<Models.PostTypes.PostType>();

                filler.Setup()
                    .OnProperty(status => status.Id).Use(statusId)
                    .OnProperty(status => status.Name).Use(statusId.GetDisplayName())
                    .OnType<DateTimeOffset>().Use(dateTimeOffset)
                    .OnProperty(status => status.CreatedByUserId).Use(userId)
                    .OnProperty(status => status.UpdatedByUserId).Use(userId);

                postTypes.Add(filler.Create());
            }

            return postTypes;
        }

        private static Models.PostTypes.PostType GetRandomPostType()
        {
            var userId = Guid.NewGuid();
            var dateTimeOffset = DateTimeOffset.UtcNow;
            Array values = Enum.GetValues(typeof(Models.PostTypes.PostTypeId));
            Random random = new Random();
            Models.PostTypes.PostTypeId randomPostTypeId = (Models.PostTypes.PostTypeId)values.GetValue(random.Next(values.Length));

            var filler = new Filler<Models.PostTypes.PostType>();

            filler.Setup()
                .OnProperty(postType => postType.Id).Use(randomPostTypeId)
                .OnProperty(postType => postType.Name).Use(randomPostTypeId.GetDisplayName())
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(postType => postType.CreatedByUserId).Use(userId)
                .OnProperty(postType => postType.UpdatedByUserId).Use(userId);

            return filler.Create();
        }
    }
}
