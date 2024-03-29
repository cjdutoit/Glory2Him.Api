﻿// --------------------------------------------------------------------------------
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
using G2H.Api.Web.Models.PostTypes;

namespace G2H.Api.Web.Services.Foundations.PostTypes
{
    public partial class PostTypeService : IPostTypeService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public PostTypeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<PostType> AddPostTypeAsync(PostType postType) =>
        TryCatch(async () =>
        {
            ValidatePostTypeOnAdd(postType);

            return await this.storageBroker.InsertPostTypeAsync(postType);
        });

        public IQueryable<PostType> RetrieveAllPostTypes() =>
        TryCatch(() => this.storageBroker.SelectAllPostTypes());

        public ValueTask<PostType> RetrievePostTypeByIdAsync(PostTypeId postTypeId) =>
        TryCatch(async () =>
        {
            ValidatePostTypeId(postTypeId);

            PostType maybePostType = await this.storageBroker
                .SelectPostTypeByIdAsync(postTypeId);

            ValidateStoragePostType(maybePostType, postTypeId);

            return maybePostType;
        });

        public ValueTask<PostType> ModifyPostTypeAsync(PostType postType) =>
        TryCatch(async () =>
        {
            ValidatePostTypeOnModify(postType);

            PostType maybePostType =
                await this.storageBroker.SelectPostTypeByIdAsync(postType.Id);

            ValidateStoragePostType(maybePostType, postType.Id);
            ValidateAgainstStoragePostTypeOnModify(inputPostType: postType, storagePostType: maybePostType);

            return await this.storageBroker.UpdatePostTypeAsync(postType);
        });
    }
}
