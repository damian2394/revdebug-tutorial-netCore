using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdbExtensions
{
    public static class RdbMethods
    {
        #region NoTimeTravel
        public static string RdbDisplay(this DataRow row)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (DataColumn col in row.Table.Columns)
            {
                sb.Append($"\"{col.ColumnName}\":{row[col]} ");
            }
            sb.Append("}");
            return sb.ToString();
        }
        public static string RdbDisplay(this DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (DataColumn col in table.Columns)
            {
                sb.Append($"\"{col.ColumnName}\", ");
            }
            sb.Append("}");
            return sb.ToString();
        }

        #endregion
    }
}