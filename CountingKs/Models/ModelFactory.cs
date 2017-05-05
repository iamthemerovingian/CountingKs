﻿using CountingKs.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        private UrlHelper _UrlHelper;

        public ModelFactory(HttpRequestMessage request)
        {
            _UrlHelper = new UrlHelper(request);
        }
        public FoodModel Create(Food food)
        {
            return new FoodModel
            {
                Url = _UrlHelper.Link("Food",new { foodid = food.Id }),
                Description = food.Description,
                Measures = food.Measures.Select(m => Create(m))
            };
        }

        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel
            {
                Url = _UrlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id}),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories)
            };
        }


        public DiaryModel Create(Diary d)
        {
            return new DiaryModel
            {
                Url = _UrlHelper.Link("Diaries", new { diaryid = d.CurrentDate.ToString("yyyy-MM-dd") }),
                CurrentDate = d.CurrentDate,
                Entries = d.Entries.Select(e => Create(e))
            };
        }

        public DiaryEntryModel Create(DiaryEntry diaryEntry)
        {
            return new DiaryEntryModel
            {
                Url = _UrlHelper.Link("DiaryEntries", new { diaryid = diaryEntry.Diary.CurrentDate.ToString("yyyy-MM-dd"), id = diaryEntry.Id }),
                FoodDescription = diaryEntry.FoodItem.Description,
                MeasureDescription = diaryEntry.Measure.Description,
                MeasureUrl = _UrlHelper.Link("Measures", new { foodid = diaryEntry.Measure.Food.Id, id = diaryEntry.Measure.Id }),
                Quantity = diaryEntry.Quantity
            };
        }
    }
}