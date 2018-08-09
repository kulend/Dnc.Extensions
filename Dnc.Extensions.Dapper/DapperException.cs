using System;
using System.Collections.Generic;
using System.Text;

namespace Dnc.Extensions.Dapper
{
    public class DapperException : Exception
    {
        public DapperException(string message) : base(message)
        {

        }
    }
}
