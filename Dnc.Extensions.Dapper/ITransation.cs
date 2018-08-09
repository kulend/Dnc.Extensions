using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dnc.Extensions.Dapper
{
    public interface ITransation : IDisposable
    {
        IDbTransaction Transaction { set; get; }

        void Commit();

        void Rollback();
    }
}
