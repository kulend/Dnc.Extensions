using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Layui.Test.Controllers
{
    public interface IUaaa
    {
        [HttpGet("aaaa")]
        string Save([FromQuery]string p, [FromQuery]string kk);

        string Kkksadpas(string p, string ff);
    }
}
