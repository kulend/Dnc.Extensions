using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vino.Core.Extensions.Layui.Test.Models;

namespace Vino.Core.Extensions.Layui.Test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Index2()
        {
            var dto = new DemoModel {
                Id = 1,
                Name = "AAAAA",
                Mobile = "18812348888",
                Url = "http://www.baidu.com",
                Email = "kulend@qq.com",
                OrderIndex = 99,
                Dec = 55.78m,
                Switch = true,
                Date = DateTime.Now,
                Year = DateTime.Now.Year,
                Month = "201805",
                Sex = EmSex.Female,
                Remark = "以上信息纯属虚构，如有雷同，概不负责。"
            };
            return View(dto);
        }

        public IActionResult Index3()
        {
            return View();
        }
    }
}
