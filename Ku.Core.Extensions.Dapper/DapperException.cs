using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.Dapper
{
    public class DapperException : Exception
    {
        public DapperException(string message) : base(message)
        {

        }
    }
}
