using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.DbMigration
{
    public class DbMigrationJsonResult : JsonResult
    {
        public DbMigrationJsonResult(object value) : base(value)
        {

        }
    }

    public class LayuiPagerResult<T>
    {
        [JsonProperty("code")]
        public int Code { set; get; } = 0;

        [JsonProperty("msg")]
        public string Message { set; get; } = "";

        [JsonProperty("count")]
        public int Count { set; get; }

        [JsonProperty("data")]
        public IEnumerable<T> Items { set; get; }

        public LayuiPagerResult(IEnumerable<T> items, int page, int size, int total)
        {
            this.Items = items;
            this.Count = total;
        }
    }
}
