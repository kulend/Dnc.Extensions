using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ku.Core.Extensions.Dapper
{
    public interface ITransation : IDisposable
    {
        IDbTransaction Transaction { set; get; }

        void Commit();

        void Rollback();
    }
}
