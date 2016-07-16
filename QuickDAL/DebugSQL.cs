using System.Data;
using System.Data.SqlClient;

namespace QuickDAL
{
    public static class DebugSQL
    {
        public static string GetActualQuery(IDbCommand sqlCmd)
        {
            string query = sqlCmd.CommandText;
            string parameters = "";
            string[] strArray = System.Text.RegularExpressions.Regex.Split(query, " VALUES ");

            parameters = "(";

            int count = 0;
            foreach (SqlParameter p in sqlCmd.Parameters)
            {
                if (count == (sqlCmd.Parameters.Count - 1))
                {
                    parameters += p.Value.ToString();
                }
                else
                {
                    parameters += p.Value.ToString() + ", ";
                }
                count++;
            }

            parameters += ")";

            return strArray[0] + " VALUES " + parameters;
        }
    }
}
