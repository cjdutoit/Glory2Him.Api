// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Brokers.DateTimes;
using G2H.Api.Web.Brokers.Loggings;
using G2H.Api.Web.Brokers.Storages;
using G2H.Api.Web.Models.Reactions;

namespace G2H.Api.Web.Services.Foundations.Reactions
{
    public partial class ReactionService : IReactionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReactionService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Reaction> AddReactionAsync(Reaction reaction) =>
        TryCatch(async () =>
        {
            ValidateReactionOnAdd(reaction);

            return await this.storageBroker.InsertReactionAsync(reaction);
        });

        public IQueryable<Reaction> RetrieveAllReactions() =>
        TryCatch(() => this.storageBroker.SelectAllReactions());

        public ValueTask<Reaction> RetrieveReactionByIdAsync(ReactionId reactionId) =>
        TryCatch(async () =>
        {
            ValidateReactionId(reactionId);

            Reaction maybeReaction = await this.storageBroker
                .SelectReactionByIdAsync(reactionId);

            ValidateStorageReaction(maybeReaction, reactionId);

            return maybeReaction;
        });

        public ValueTask<Reaction> ModifyReactionAsync(Reaction reaction) =>
        TryCatch(async () =>
        {
            return await this.storageBroker.UpdateReactionAsync(reaction);
        });
    }
}
