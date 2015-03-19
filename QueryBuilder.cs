using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Transactions;
// using Microsoft.Practices.EnterpriseLibrary.Data;

namespace QuickDAL
{
    public class QueryBuilder : IDisposable
    {

        public enum ConnectionHandlingOption
        {
            CloseOnDispose = 1,
            LeaveOpenOnDispose = 2
        }

        public enum QueryProviderOption
        {
            Connection = 1,
            Delegate = 2
        }

        private Int32 GlobalParamterCount = 0;

        private IDbConnection _Connection;
        public IDbConnection Connection
        {
            get
            {
                return _Connection;
            }

            set
            {
                if (_Connection != null && _Connection != value)
                {
                    _Connection.Close();
                }
                _Connection = value;
            }
        }

        public ConnectionHandlingOption ConnectionHandling = ConnectionHandlingOption.CloseOnDispose;
        private QueryProviderOption QueryProvider;

        // delegates ... if needed.
        private Func<IDbCommand> _CreateCommandDelegate;
        private Func<IDbCommand, IDataReader> _ExecuteReader;
        private Func<IDbCommand, Int32> _ExecuteNonQuery;
        private Func<IDbCommand, Object> _ExecuteScalar;


        public QueryBuilder(IDbConnection connection, ConnectionHandlingOption connectionHandling = ConnectionHandlingOption.CloseOnDispose)
        {
            Connection = connection;
            ConnectionHandling = connectionHandling;
            QueryProvider = QueryProviderOption.Connection;
        }


        public QueryBuilder(Func<IDbCommand> createCommand,
            Func<IDbCommand, IDataReader> executeReader,
            Func<IDbCommand, Int32> executeNonQuery,
            Func<IDbCommand, Object> executeScalar
        )
        {
            _CreateCommandDelegate = createCommand;
            _ExecuteReader = executeReader;
            _ExecuteNonQuery = executeNonQuery;
            _ExecuteScalar = executeScalar;
            QueryProvider = QueryProviderOption.Delegate;
            ConnectionHandling = ConnectionHandlingOption.LeaveOpenOnDispose;
        }


        private IDbCommand CreateCommand()
        {
            if (QueryProvider == QueryProviderOption.Connection)
            {
                return Connection.CreateCommand();
            }
            else
            {
                return _CreateCommandDelegate();
            }
        }


        private IDataReader ExecuteReader(IDbCommand query)
        {
            if (QueryProvider == QueryProviderOption.Connection)
            {
                if (query.Connection.State != ConnectionState.Open) query.Connection.Open();
                return query.ExecuteReader();
            }
            else
            {
                return _ExecuteReader(query);
            }
        }

        private Int32 ExecuteNonQuery(IDbCommand query)
        {
            if (QueryProvider == QueryProviderOption.Connection)
            {
                if (query.Connection.State != ConnectionState.Open) query.Connection.Open();
                return query.ExecuteNonQuery();
            }
            else
            {
                return _ExecuteNonQuery(query);
            }
        }


        private Object ExecuteScalar(IDbCommand query)
        {
            if (QueryProvider == QueryProviderOption.Connection)
            {
                if (query.Connection.State != ConnectionState.Open) query.Connection.Open();
                return query.ExecuteScalar();
            }
            else
            {
                return _ExecuteScalar(query);
            }
        }


        public static Dictionary<String, String> GetDictionary(IDataReader r)
        {
            Dictionary<String, String> rv = null;
            if (r != null)
            {
                rv = new Dictionary<String, String>();
                for (Int16 i = 0; i < r.FieldCount; i++)
                {
                    rv.Add(r.GetName(i), r.IsDBNull(i) ? null : Convert.ToString(r[i]));
                }
            }
            return rv;
        }


        [Obsolete("Use list.Cast<DataObject>().ToList()")]
        public static List<DataObject> Translate<T>(List<T> list) where T : DataObject
        {
            if (list == null)
            {
                return null;
            }
            else
            {
                return list.Cast<DataObject>().ToList();
                //List<DataObject> rv = new List<DataObject>();
                //foreach (T o in list)
                //{
                //    rv.Add(o);
                //}
                //return rv;
            }
        } // Translate()


        public List<T> Find<T>(
            List<DataObject> conditions,
            Boolean fuzzy = false,
            String order = null,
            Int32 limit = Int32.MaxValue,
            T start = null) where T : DataObject, new()
        {
            // for grabbing the DataDefinition of T
            T _t = new T();

            // query components to be join()'d and append()'d to the query
            DataDefinition d = _t.GetDefinition();
            String distinctString = d.Distinctable ? "distinct" : "";

            List<String> tables = new List<String>() { "[" + d.DataEntity + "] [" + d.Name + "]" };
            List<DataDefinition> condition_types = new List<DataDefinition>();
            Dictionary<String, List<Dictionary<String, String>>> where = new Dictionary<String, List<Dictionary<String, String>>>();

            if (conditions != null && conditions.Count > 0)
            {
                foreach (DataObject o in conditions)
                {

                    DataDefinition od = o.GetDefinition();
                    if (!where.ContainsKey(od.Name))
                    {
                        where.Add(od.Name, new List<Dictionary<String, String>>());
                    }

                    Dictionary<String, String> this_clause = new Dictionary<String, String>();

                    if (!condition_types.Contains(od))
                    {
                        if (od == d)
                        {
                            condition_types.Add(od);
                        }
                        else
                        {
                            List<String> path = RelationshipPath(_t.GetDefinition(), od);
                            if (path.Count > 0)
                            {
                                foreach (var t in path) {
                                    if (!tables.Contains(t))
                                    {
                                        tables.Add(t);
                                    }
                                }
                                condition_types.Add(od);
                            }
                        }
                    }

                    if (condition_types.Contains(od))
                    {
                        Dictionary<String, String> c = o.ToDictionary();

                        if (c.Count > 0)
                        {
                            if (!String.IsNullOrEmpty(od.PrimaryKey)
                                && c.ContainsKey(od.PrimaryKey) && c[od.PrimaryKey] != null)
                            {
                                this_clause.Add(od.Name + "." + od.PrimaryKey, c[od.PrimaryKey]);
                            }
                            else
                            {
                                foreach (KeyValuePair<String, String> p in c)
                                {
                                    if (p.Value != null)
                                    {
                                        this_clause.Add(od.Name + "." + p.Key, p.Value);
                                    }
                                }
                            }

                            where[od.Name].Add(this_clause);
                        }
                    }
                    else
                    {
                        throw new Exception("A path does not exist from " + _t.GetDefinition().DataEntity + " to " + od.DataEntity + ". Aborting query.");
                    }
                }
            }

            String orderClause = "";
            String fullOrderField = "";
            String effectiveOrderColumn = order;
            if (!string.IsNullOrEmpty(order))
            {

                if (order.Contains("."))
                {
                    fullOrderField = "[" + order.Replace(".", "].[") + "]";
                }
                else
                {
                    fullOrderField = "[" + d.Name + "].[" + order + "]";
                }

            }
            else if (!String.IsNullOrEmpty(d.DefaultOrder))
            {
                fullOrderField = "[" + d.Name + "].[" + d.DefaultOrder + "]";
                effectiveOrderColumn = d.DefaultOrder;
            }
            else if (!String.IsNullOrEmpty(d.PrimaryKey))
            {
                fullOrderField = "[" + d.Name + "].[" + d.PrimaryKey + "]";
                effectiveOrderColumn = d.PrimaryKey;
            }

            if (fullOrderField != "")
            {
                orderClause = " order by " + fullOrderField;
            }

            String topOrderByFieldAlias = "";
            if (!String.IsNullOrEmpty(fullOrderField))
            {
                topOrderByFieldAlias = " ___orderField, ";
            }

            using (IDbCommand query = CreateCommand())
            {
                query.CommandText = "select " + distinctString + " top " + limit + " " + fullOrderField.Replace(" DESC","") + " " + topOrderByFieldAlias + "[" + d.Name + "].* from " + String.Join("\r\n", tables.ToArray());
                AppendWhere(query, where, fuzzy);

                if (start != null && !String.IsNullOrEmpty(effectiveOrderColumn))
                {
                    Dictionary<String, String> startD = start.ToDictionary();
                    if (startD.Keys.Contains(effectiveOrderColumn))
                    {
                        AppendWhere(query, effectiveOrderColumn, ">=", startD[effectiveOrderColumn], where.Count > 0);
                    }
                }

                query.CommandText += " " + orderClause;

                List<T> rv = new List<T>();
                using (IDataReader r = ExecuteReader(query))
                {
                    while (r.Read())
                    {
                        T row = new T();
                        row.Populate(QueryBuilder.GetDictionary(r));
                        if (!row.AuthorizeGet())
                        {
                            throw new Exception("Not authorized to Get.");
                        }
                        rv.Add(row);
                    }
                }

                return rv;
            }
        } // Find()


        //public List<T> Find<T>(List<DataObject> conditions, String order = null, Int32 limit = Int32.MaxValue, T start = null) where T : DataObject, new()
        //{
        //    return Find<T>(conditions, false, order, limit, start);
        //}


        public List<T> Find<T>(
            DataObject condition,
            Boolean fuzzy = false,
            String order = null,
            Int32 limit = Int32.MaxValue,
            T start = null) where T : DataObject, new()
        {
            return Find<T>(new List<DataObject>() { condition }, fuzzy, order, limit, start);
        } // Find(DataObject)


        public List<T> Find<T>() where T : DataObject, new()
        {
            return Find<T>(new List<DataObject>() { });
        }  // Find() ... (all)


        /// <summary>
        /// UPDATEs the provided DataObject if a PK is present, INSERTs if a PK is not present.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">The object to save</param>
        /// <param name="fullUpdate">Whether to set nulls, 0's, and empty strings values in the update, which are otherwise ignored.</param>
        /// <returns>number of rows affected</returns>
        public Int32 Save(DataObject o, Boolean fullUpdate = false)
        {
            if (!o.Validate())
            {
                throw new Exception("Invalid object state for saving.");
            }

            DataDefinition d = o.GetDefinition();
            Dictionary<String, String> v = o.ToDictionary();
            if (d.PrimaryKey != null && v.ContainsKey(d.PrimaryKey) && v[d.PrimaryKey] != null)
            {
                return Update(o, fullUpdate);
            }
            else
            {
                return Insert(o, fullUpdate);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="fullUpdate"></param>
        /// <returns>number of rows affected</returns>
        public Int32 Insert(DataObject o, Boolean fullUpdate = false)
        {
            if (!o.AuthorizeInsert())
            {
                throw new Exception("Not authorized for Insert.");
            }

            var dv = o.ToDictionary(fullUpdate);
            DataDefinition d = o.GetDefinition();
            Boolean GuidPK = true;
            if (d.PrimaryKey != null)
            {
                Guid newGuid = Guid.NewGuid();
                GuidPK = d.Maps[d.PrimaryKey].PropertyType.Equals(typeof(Guid));

                if (GuidPK && (!dv.ContainsKey(d.PrimaryKey) || dv[d.PrimaryKey].Equals(Guid.Empty.ToString())))
                {
                    dv[d.PrimaryKey] = newGuid.ToString();
                }
            }
            using (IDbCommand query = CreateCommand())
            {
                query.CommandText = "insert into [" + o.GetDefinition().DataEntity + "]";
                query.CommandText += " (" + String.Join(",", dv.Keys.ToArray()) + ")";
                query.CommandText += " values (@" + String.Join(",@", dv.Keys.ToArray()) + ")";

                IDbDataParameter insertid = null;
                if (!GuidPK)
                {
                    query.CommandText += " SET @insert_id = SCOPE_IDENTITY()";
                    insertid = query.CreateParameter();
                    insertid.Direction = ParameterDirection.Output;
                    insertid.ParameterName = "insert_id";
                    insertid.DbType = DbType.Int32;
                    insertid.Size = sizeof(Int32); // Convert.ToInt32(Math.Pow(2, 16));
                    query.Parameters.Add(insertid);
                }

                foreach (String k in dv.Keys)
                {
                    IDbDataParameter parameter = query.CreateParameter();
                    parameter.ParameterName = k;
                    parameter.Value = dv[k];
                    query.Parameters.Add(parameter);
                }
                
                Int32 rv = ExecuteNonQuery(query);
                
                //
                // NOTE : No idea whether the following insertid handling is correct.
                // It works in my single test-case. Much more testing is required ... 
                //
                if (rv > 0)
                {
                    if (d.PrimaryKey != null)
                    {
                        if (GuidPK)
                        {
                            o.Populate(d.PrimaryKey, dv[d.PrimaryKey]);
                        }
                        else
                        {
                            o.Populate(d.PrimaryKey, insertid.Value.ToString());
                        }
                    }
                }

                return rv;
            }
        } // Insert()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="fullUpdate"></param>
        /// <returns>number of rows affected</returns>
        public Int32 Update(DataObject o, Boolean fullUpdate = false)
        {
            if (!o.AuthorizeUpdate())
            {
                throw new Exception("Not authorized for Update.");
            }

            DataDefinition d = o.GetDefinition();
            var dv = o.ToDictionary(fullUpdate);

            var PK_key = d.PrimaryKey;
            var PK_value = dv[d.PrimaryKey];
            dv.Remove(PK_key);

            using (IDbCommand query = CreateCommand())
            {
                query.CommandText = "update " + d.DataEntity;
                AppendSet(query, dv);
                AppendWhere(query, PK_key, "=", PK_value);

                return ExecuteNonQuery(query);
            }
        } // Update();


        /// <summary>
        /// Deletes DataObjects as indicated by the matching fields in the supplied object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public Int32 Delete(DataObject o)
        {
            if (!o.AuthorizeDelete())
            {
                throw new Exception("Not authorized for Delete.");
            }

            var dv = o.ToDictionary();

            if (dv.Count > 0)
            {
                using (IDbCommand query = CreateCommand())
                {
                    query.CommandText = "delete from " + o.GetDefinition().DataEntity;
                    AppendWhere(query, dv);
                    return ExecuteNonQuery(query);
                }
            }
            else
            {
                return 0;
            }
        }


        public List<String> RelationshipPath(DataDefinition a, DataDefinition b)
        {
            return RelationshipPath(a, b, 512);
        } // RelationshipPath()


        public List<String> RelationshipPath(DataDefinition a, DataDefinition b, Int32 maxNodes)
        {
            var rv = new List<String>();

            if (a != b)
            {
                Queue<TreeNode<DataRelationship>> q = new Queue<TreeNode<DataRelationship>>();

                foreach (DataRelationship d in a.Parents)
                {
                    q.Enqueue(new TreeNode<DataRelationship>(d));
                }

                foreach (DataRelationship d in a.Children)
                {
                    q.Enqueue(new TreeNode<DataRelationship>(d));
                }

                while (q.Count > 0 && maxNodes > 0)
                {
                    TreeNode<DataRelationship> path = q.Dequeue();
                    DataRelationship d = path.Value;

                    // we found it!
                    if (d.RemoteEntity.GetDefinition() == b)
                    {
                        while (path != null && path.Value != null)
                        {
                            rv.Add(path.Value.InnerJoinSQL);
                            path = path.Parent;
                        }

                        // necessary? ... not sure.
                        rv.Reverse();

                        maxNodes = 0;
                    }
                    else
                    {
                        foreach (DataRelationship subd in d.RemoteEntity.GetDefinition().Parents)
                        {
                            q.Enqueue(new TreeNode<DataRelationship>(subd, path));
                        }

                        foreach (DataRelationship subd in d.RemoteEntity.GetDefinition().Children)
                        {
                            q.Enqueue(new TreeNode<DataRelationship>(subd, path));
                        }
                    }

                    maxNodes--;
                }
            }

            return rv;
        } // RelationshipPath()


        public void AppendKeyValuePairs(IDbCommand query, Dictionary<String, String> pairs, String glue, String open = "(", String close = ")", String comparison = "=")
        {
            List<String> c = new List<String>();
            foreach (KeyValuePair<String, String> pair in pairs)
            {
                if (pair.Value != null && !String.IsNullOrEmpty(pair.Key))
                {
                    GlobalParamterCount = (GlobalParamterCount + 1) % 1000;
                    String pname = pair.Key.Replace(".", "_") + GlobalParamterCount.ToString();
                    String kname = "[" + pair.Key.Replace(".", "].");
                    if (!kname.Contains("].")) kname += "]";
                    c.Add(kname + " " + comparison + " @" + pname);
                    IDbDataParameter parameter = query.CreateParameter();
                    parameter.ParameterName = pname;
                    parameter.Value = pair.Value;
                    query.Parameters.Add(parameter);
                }
            }

            if (c.Count > 0)
            {
                query.CommandText += " " + open + " " + String.Join(" " + glue + " ", c.ToArray()) + " " + close + " ";
            }
        } // BuildKeyValueClause()


        public void AppendWhere(IDbCommand query, String field, String comparison, String value, Boolean omitWhere = false)
        {
            if (omitWhere)
                AppendKeyValuePairs(query, new Dictionary<string, string>() { { field, value } }, "and", "and (", ")", comparison);
            else
                AppendKeyValuePairs(query, new Dictionary<string, string>() { { field, value } }, "and", "where (", ")", comparison);
        } // AppendWhere(query, field, comparison, value)


        public void AppendWhere(IDbCommand query, Dictionary<String, String> pairs, Boolean fuzzy = false)
        {
            if (fuzzy)
            {
                AppendKeyValuePairs(query, pairs, "and", "where (", ")", "like");
            }
            else
            {
                AppendKeyValuePairs(query, pairs, "and", "where (", ")");
            }
        } // AppendWhere(query, pairs)


        //public static void AppendWhere(SqlCommand query, List<Dictionary<String, String>> objects)
        //{
        //} // AppendWhere(query, list<pairs>)


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="wheres">Dict<tablename, Dict<field, value>></param>
        /// <param name="fuzzy"></param>
        public void AppendWhere(IDbCommand query, Dictionary<String, List<Dictionary<String, String>>> wheres, Boolean fuzzy = false)
        {
            if (wheres.Count > 0)
            {
                // query.CommandText += "";
                String glue = " where ";
                String comparison = fuzzy ? "like" : "=";

                // for each TABLE
                foreach (String k in wheres.Keys)
                {
                    // glue should be blank the first time and " and " after each clause is
                    // added to the query


                    // if multiple K-V pairs (objects) are present for that table
                    if (wheres[k].Count > 1)
                    {
                        if (wheres[k][0].Count > 0)
                        {
                            query.CommandText += glue;

                            // the complicated case: many objects for this table
                            // any row matching any one of the objects in full
                            AppendKeyValuePairs(query, wheres[k][0], "and", "((", ")", comparison);
                            for (Int32 i = 1; i < wheres[k].Count; i++)
                            {
                                query.CommandText += " or ";
                                AppendKeyValuePairs(query, wheres[k][i], "and", "(", ")", comparison);
                            }
                            query.CommandText += ") ";

                            glue = " and ";
                        }
                    }
                    else if (wheres[k].Count == 1)
                    {
                        if (wheres[k][0].Count > 0)
                        {
                            query.CommandText += glue;

                            // the simple case: single object for this table
                            // only rows fully matching the object
                            AppendKeyValuePairs(query, wheres[k][0], "and", "(", ")", comparison);

                            glue = " and ";
                        }
                    }
                    else
                    {
                        // do nothing.
                    }

                }
            }
        } // AppendWhere(query, multi)


        public void AppendSet(IDbCommand query, Dictionary<String, String> pairs)
        {
            AppendKeyValuePairs(query, pairs, ",", "set", " ");
        } // BuildSetClause()


        public void Dispose()
        {
            if (ConnectionHandling == ConnectionHandlingOption.CloseOnDispose && QueryProvider == QueryProviderOption.Connection)
            {
                Connection.Close();
            }
        }
    }
}
