﻿using CacheCow.Server;
using CacheCow.Server.EntityTagStore.SqlServer;
using CountingKs.Converters;
using CountingKs.Filters;
using CountingKs.Services;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using WebApiContrib.Formatting.Jsonp;

namespace CountingKs
{
  public static class WebApiConfig
  {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Food",
                routeTemplate: "api/nutrition/foods/{foodid}",
                defaults: new { controller = "foods", foodid = RouteParameter.Optional }
                //constraints: new { id = "/d+"}
            );

            config.Routes.MapHttpRoute(
                 name: "Measures",
                 routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
                 defaults: new { controller = "measures", id = RouteParameter.Optional }
             );

            config.Routes.MapHttpRoute(
                  name: "Diaries",
                  routeTemplate: "api/user/diaries/{diaryid}",
                  defaults: new { controller = "diaries", diaryid = RouteParameter.Optional }
              );

            config.Routes.MapHttpRoute(
                  name: "DiaryEntries",
                  routeTemplate: "api/user/diaries/{diaryid}/entries/{id}",
                  defaults: new { controller = "diaryentries", id = RouteParameter.Optional }
              );

            config.Routes.MapHttpRoute(
                 name: "DiarySummary",
                 routeTemplate: "api/user/diaries/{diaryid}/summary",
                 defaults: new { controller = "diarysummary" }
             );

            config.Routes.MapHttpRoute(
                  name: "Token",
                  routeTemplate: "api/token",
                  defaults: new { controller = "token" }
              );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();

            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.Converters.Add(new LinkModelConverter());
            CreteMediaTypes(jsonFormatter);
            //Add Supportfor JsonP.
            var formatter = new JsonpMediaTypeFormatter(jsonFormatter, "cb");
            config.Formatters.Insert(0, formatter);

            //Replace the controllerselector with our customController selector
            config.Services.Replace(typeof(IHttpControllerSelector), new CountingKsControllerSelector(config));

            //Configure caching/Etag Support
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var sqlServerEntityStore = new SqlServerEntityTagStore(connectionString);
            var cacheHandler = new CachingHandler(sqlServerEntityStore);
            config.MessageHandlers.Add(cacheHandler);

#if !DEBUG
            //Forces the entire API to use SSL encryption.
            config.Filters.Add(new RequireHttpsAttribute());
#endif
        }

        private static void CreteMediaTypes(JsonMediaTypeFormatter jsonFormatter)
        {
            var mediaTypes = new string[]
            {
                "application/vnd.countingks.food.v1+json",
                "application/vnd.countingks.measure.v1+json",
                "application/vnd.countingks.measure.v2+json",
                "application/vnd.countingks.diary.v1+json",
                "application/vnd.countingks.diaryEntry.v1+json"
            };

            foreach (var mediaType in mediaTypes)
            {
                jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }
        }
    }
}