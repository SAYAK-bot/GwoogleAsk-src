// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

//using Microsoft.EchoBot.Bots;
using Microsoft.Bot.Builder.EchoBot;
using EchoBot.Dialogs;
using Microsoft.BotBuilderSamples.Bots;
using EchoBot;
using Microsoft.Bot.Builder.AI.Luis;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        private const string BotOpenIdMetadataKey = "BotOpenIdMetadata";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<UserProfileDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, EchoBot<UserProfileDialog>>();

//            services.AddSingleton(sp =>
//            {
//                // Get LUIS information
//                var luisApp = new LuisApplication("2434c13d-e2c6-48ba-ac06-ac6ee930ff79", "4484eb67e5d44da2a847f8f12fbd97cf", "
//https://westus.api.cognitive.microsoft.com/luis/api/v2.0");

//                // Specify LUIS options. These may vary for your bot.
//                var luisPredictionOptions = new LuisPredictionOptions
//                {
//                    IncludeAllIntents = true,
//                };

//                // Create the recognizer
//                var recognizer = new Bot.Builder.AI.Luis.LuisRecognizer(luisApp, luisPredictionOptions, true, null);
//                return recognizer;
//            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();

            app.UseMvc();
        }
    }
}
