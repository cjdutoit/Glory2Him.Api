﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Net.Http;
using G2H.Api.Web.Services.Foundations.PostTypes;
using G2H.Api.Web.Services.Foundations.Reactions;
using G2H.Api.Web.Services.Foundations.Statuses;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RESTFulSense.Clients;

namespace G2H.Api.Web.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private readonly WebApplicationFactory<Startup> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;
        internal IStatusService StatusService;
        internal IReactionService ReactionService;
        internal IPostTypeService PostTypeService;

        public ApiBroker()
        {
            this.webApplicationFactory = new WebApplicationFactory<Startup>();

            this.StatusService = (StatusService)webApplicationFactory.Services.GetService<IStatusService>();
            this.ReactionService = (ReactionService)webApplicationFactory.Services.GetService<IReactionService>();
            this.PostTypeService = (PostTypeService)webApplicationFactory.Services.GetService<IPostTypeService>();

            this.httpClient = this.webApplicationFactory.CreateClient();
            this.apiFactoryClient = new RESTFulApiFactoryClient(this.httpClient);
        }
    }
}
