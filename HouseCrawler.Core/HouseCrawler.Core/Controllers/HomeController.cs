﻿using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Jobs;
using StackExchange.Redis;
using HouseCrawler.Core.DataContent;
using System;
using HouseCrawler.Core.Common;
using HouseCrawler.Core.Service;
using System.Linq;

namespace HouseCrawler.Core.Controllers
{
    public class HomeController : Controller
    {

        private HouseDapper houseDapper;

        private HouseDashboardJob houseDashboardJob;

        private HouseDashboardService houseDashboardService;


        private PinPaiGongYuHouseCrawler pinpai;
        private PeopleRentingCrawler people;
        private DoubanHouseCrawler douban;
        private CCBHouesCrawler ccbHouse;
        private ZuberHouseCrawler zuber;
        private MoGuHouseCrawler mogu;
        private HKSpaciousCrawler hkSpacious;

        public HomeController(HouseDashboardJob houseDashboardJob,
                              HouseDapper houseDapper,
                              HouseDashboardService houseDashboardService,
                              PinPaiGongYuHouseCrawler pinpai,
                              PeopleRentingCrawler people,
                              DoubanHouseCrawler douban,
                              CCBHouesCrawler ccbHouse,
                              ZuberHouseCrawler zuber,
                              MoGuHouseCrawler mogu,
                              HKSpaciousCrawler hkSpacious)
        {
            this.houseDashboardJob = houseDashboardJob;
            this.houseDapper = houseDapper;
            this.houseDashboardService = houseDashboardService;
            this.pinpai = pinpai;
            this.people = people;
            this.douban = douban;
            this.ccbHouse = ccbHouse;
            this.zuber = zuber;
            this.mogu = mogu;
            this.hkSpacious = hkSpacious;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(houseDashboardService.LoadDashboard());
        }

        public IActionResult HouseList()
        {
            return View();
        }

        public IActionResult GetHouseInfo(string cityName, string source = "", int houseCount = 500,
            int withAnyDays = 7, string keyword = "")
        {

            var houses = houseDapper.SearchHouseInfo(cityName, source, houseCount, withAnyDays, keyword);
            var rooms = houses.Select(house =>
                {
                    var markBGType = string.Empty;
                    int housePrice = (int)house.HousePrice;
                    if (housePrice > 0)
                    {
                        markBGType = LocationMarkBGType.SelectColor(housePrice / 1000);
                    }

                    return new HouseInfo
                    {
                        Source = house.Source,
                        Money = house.DisPlayPrice,
                        HouseURL = house.HouseOnlineURL,
                        HouseLocation = house.HouseLocation,
                        HouseTime = house.PubTime.ToString(),
                        HouseTitle = house.HouseTitle,
                        HousePrice = housePrice,
                        LocationMarkBG = markBGType,
                        DisplaySource = ConstConfigurationName.ConvertToDisPlayName(house.Source)
                    };
                });
            return Json(new { IsSuccess = true, HouseInfos = rooms });

        }


        public IActionResult RunJobs()
        {
            douban.Run();
            pinpai.Run();
            ccbHouse.Run();
            mogu.Run();
            return View();
        }

        public IActionResult RunZuber()
        {
            zuber.Run();
            return View();
        }

        public IActionResult RunHK()
        {
            hkSpacious.Run();
            return Json(new { success = true });
        }


        public IActionResult RunStatJob()
        {
            houseDashboardJob.Run();
            return Json(new { success = true });
        }


        public IActionResult StartGC()
        {
            new GCJob().Run();
            return Json(new { isSuccess = true });
        }
    }
}
