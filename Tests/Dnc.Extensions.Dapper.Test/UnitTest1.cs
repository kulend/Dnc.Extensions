using Dnc.Extensions.Dapper.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dnc.Extensions.Dapper.Test
{
    public class UnitTest1 : BaseTest
    {
        [Fact]
        public void Test1()
        {
            var builder = new QueryBuilder();
            builder.Select<User>(x => new { x.Name, x.Account, x.Id }).
                From<User>("m")
                .InnerJoin<UserInfo>("t")
                .On(new ConditionBuilder().Equal<User, UserInfo>(m => m.Id, t => t.Id))
                .Where(new ConditionBuilder().Equal<User>(m => m.Account, "112233").And().Like<User>(m => m.Name, "ff"))
                .OrderBy<User>(u => u.Name).ThenByDesc<UserInfo>(t => t.Age);
            var result = builder.Build();
            Assert.NotNull(result.sql);
        }

        [Fact]
        public void Test2()
        {
            var builder = new QueryBuilder();
            builder.Select<User>()
                .Concat<UserInfo>()
                .From<User>("m")
                .InnerJoin<UserInfo>("t")
                .On(new ConditionBuilder().Equal<User, UserInfo>(m => m.Id, t => t.Id).And().Less<UserInfo>(t => t.Age, 20))
                .Where(new ConditionBuilder().Equal<User>(m => m.Account, "112233"))
                .OrderBy<User>(u => u.Name).ThenByDesc<UserInfo>(t => t.Age);
            var result = builder.Build();
            Assert.NotNull(result.sql);
        }

        [Table("mm_user")]
        public class User
        {
            public long Id { set; get; }

            public string Name { set; get; }

            public string Account { set; get; }
        }

        [Table("mm_user_info")]
        public class UserInfo
        {
            public long Id { set; get; }

            public short Sex { set; get; }

            public int Age { set; get; }
        }
    }
}
