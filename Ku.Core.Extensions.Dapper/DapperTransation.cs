using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ku.Core.Extensions.Dapper
{
    public class DapperTransation : ITransation
    {
        public IDbTransaction Transaction { set; get; }

        public DapperTransation(IDbTransaction trans)
        {
            Transaction = trans;
        }

        public void Dispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
            }
            this.Transaction = null;
        }

        public void Commit()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
            }
        }

        public void Rollback()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Rollback();
            }
        }
    }
}
