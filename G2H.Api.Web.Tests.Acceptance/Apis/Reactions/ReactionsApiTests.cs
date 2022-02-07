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
using G2H.Api.Web.Tests.Acceptance.Models.Reactions;
using Microsoft.OpenApi.Extensions;
using Tynamix.ObjectFiller;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.Reactions
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ReactionsApiTests
    {
        private readonly ApiBroker apiBroker;

        public ReactionsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private async ValueTask<List<Reaction>> CreateReactionsAsync()
        {
            Guid userId = Guid.NewGuid();
            List<Web.Models.Reactions.Reaction> reactions = GetStorageReactions(userId);

            foreach (var item in reactions)
            {
                if (!apiBroker.ReactionService.RetrieveAllReactions().Any(status => status.Id == item.Id))
                {
                    await this.apiBroker.ReactionService.AddReactionAsync(item);
                }
            }

            return GetClientReactions(userId);
        }

        private static List<Web.Models.Reactions.Reaction> GetStorageReactions(Guid userId)
        {
            var reactions =
                 new List<Web.Models.Reactions.Reaction>();

            foreach (Web.Models.Reactions.ReactionId statusId
                in Enum.GetValues(typeof(Web.Models.Reactions.ReactionId)))
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
                var filler = new Filler<Web.Models.Reactions.Reaction>();

                filler.Setup()
                    .OnProperty(status => status.Id).Use(statusId)
                    .OnProperty(status => status.Name).Use(statusId.GetDisplayName())
                    .OnType<DateTimeOffset>().Use(dateTimeOffset)
                    .OnProperty(status => status.CreatedByUserId).Use(userId)
                    .OnProperty(status => status.UpdatedByUserId).Use(userId)
                    .OnProperty(status => status.PostReactions).IgnoreIt()
                    .OnProperty(status => status.CommentReactions).IgnoreIt();

                reactions.Add(filler.Create());
            }

            return reactions;
        }

        private static List<Models.Reactions.Reaction> GetClientReactions(Guid userId)
        {
            List<Models.Reactions.Reaction> reactions = new List<Models.Reactions.Reaction>();

            foreach (Models.Reactions.ReactionId statusId in Enum.GetValues(typeof(Models.Reactions.ReactionId)))
            {
                var dateTimeOffset = DateTimeOffset.UtcNow;
                var filler = new Filler<Models.Reactions.Reaction>();

                filler.Setup()
                    .OnProperty(status => status.Id).Use(statusId)
                    .OnProperty(status => status.Name).Use(statusId.GetDisplayName())
                    .OnType<DateTimeOffset>().Use(dateTimeOffset)
                    .OnProperty(status => status.CreatedByUserId).Use(userId)
                    .OnProperty(status => status.UpdatedByUserId).Use(userId);

                reactions.Add(filler.Create());
            }

            return reactions;
        }

        private static Models.Reactions.Reaction GetRandomReaction()
        {
            Guid userId = Guid.NewGuid();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            Array values = Enum.GetValues(typeof(Models.Reactions.ReactionId));
            Random random = new Random();
            Models.Reactions.ReactionId randomReactionId = (Models.Reactions.ReactionId)values.GetValue(random.Next(values.Length));
            var filler = new Filler<Models.Reactions.Reaction>();

            filler.Setup()
                .OnProperty(reaction => reaction.Id).Use(randomReactionId)
                .OnProperty(reaction => reaction.Name).Use(randomReactionId.GetDisplayName())
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(reaction => reaction.CreatedByUserId).Use(userId)
                .OnProperty(reaction => reaction.UpdatedByUserId).Use(userId);

            return filler.Create();
        }
    }
}
