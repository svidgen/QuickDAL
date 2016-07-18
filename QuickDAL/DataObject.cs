using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace QuickDAL
{
    [Serializable]
    public abstract class DataObject
    {
        public abstract DataDefinition GetDefinition();

        public static ICacheCollection Items { get; set; }

        public Boolean _OmitBooleansInSearch = true;
        public Boolean OmitBooleansInSearch
        {
            get
            {
                return _OmitBooleansInSearch;
            }
            set
            {
                _OmitBooleansInSearch = value;
            }
        }


        /// <summary>
        /// Determines whether the object permits the Domain/context to Get and operate on the object.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean AuthorizeGet() { return true; }

        /// <summary>
        /// Determines whether the attached client is permitted to view the object. This is enforced in the CRUD API, and SHOULD be checked any time an object passes out of the Domain.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean AuthorizeView() { return true; }

        public virtual Boolean AuthorizeInsert() { return true; }
        public virtual Boolean AuthorizeUpdate() { return true; }
        public virtual Boolean AuthorizeDelete() { return true; }

        /// <summary>
        /// Determines whether the object is in a consistent, valid state.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean Validate() {return true; }


        public Dictionary<String, String> Compare(DataObject comparedObject)
        {
            return this.ToDictionary().Where(entry => comparedObject.ToDictionary()[entry.Key] != entry.Value).ToDictionary(entry => entry.Key, entry => entry.Value);
        }


        public virtual Dictionary<String, String> ToDictionary(Boolean includeAllFields = false)
        {
            DataDefinition d = GetDefinition();
            Dictionary<String, String> rv = new Dictionary<String, String>();

            foreach (KeyValuePair<String, IReference> m in d.Maps)
            {

                Object Value = m.Value.Get();
                Type t = m.Value.PropertyType;


                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && Value == null)
                {
                    if (includeAllFields)
                    {
                        rv.Add(m.Key, null);
                    }
                    else
                    {
                        // omit the value.
                    }
                }
                else if (Value is String)
                {
                    rv.Add(m.Key, (String)Value);
                }
                else if (t.Equals(typeof(Int16?)) || t.Equals(typeof(UInt16?)) || t.Equals(typeof(Int32?)) || t.Equals(typeof(UInt32?)) || t.Equals(typeof(Int64?)))
                {
                    String v = Value.ToString();
                    if (includeAllFields || !String.IsNullOrEmpty(v))
                    {
                        rv.Add(m.Key, v);
                    }
                }
                else if (Value is Int16 || Value is UInt16 || Value is Int32 || Value is UInt32 || Value is Int64)
                {
                    Int64 v = Convert.ToInt64(Value);
                    if (includeAllFields || v != 0)
                    {
                        rv.Add(m.Key, v.ToString());
                    }
                }
                else if (Value is Single || Value is Double || Value is Decimal)
                {
                    Decimal v = Convert.ToDecimal(Value);
                    if (includeAllFields || !v.Equals(new Decimal(0)))
                    {
                        rv.Add(m.Key, v.ToString());
                    }
                }
                else if (Value is DateTime)
                {
                    DateTime v = (DateTime)Value;
                    if (includeAllFields || (v != null && v != DateTime.MinValue))
                    {
                        rv.Add(m.Key, v.ToString());
                    }
                }
                else if (t.Equals(typeof(Guid?)))
                {
                    Guid? v = (Guid?)Value;
                    if (includeAllFields || v.HasValue)
                    {
                        rv.Add(m.Key, (v == null || v == Guid.Empty) ? Guid.Empty.ToString() : v.ToString());
                    }
                }
                else if (Value is Guid)
                {
                    Guid v = (Guid)Value;
                    if (includeAllFields || (v != null && v != Guid.Empty))
                    {
                        if (v == Guid.Empty)
                        {
                            rv.Add(m.Key, null);
                        }
                        else
                        {
                            rv.Add(m.Key, v.ToString());
                        }
                    }
                }
                else if (t.Equals(typeof(Boolean?)))
                {
                    Boolean? v = (Boolean?)Value;
                    if (includeAllFields || v.HasValue)
                    {
                        rv.Add(m.Key, v == true ? "1" : "0");
                    }
                }
                else if (Value is Boolean)
                {
                    Boolean v = (Boolean)Value;
                    if (includeAllFields || !OmitBooleansInSearch)
                    {
                        rv.Add(m.Key, v ? "1" : "0");
                    }
                }
                else if (Value is Byte[])
                {
                    Byte[] v = (Byte[])Value;
                    if (includeAllFields || (v != null && v.Length > 0))
                    {
                        rv.Add(m.Key, System.Text.Encoding.UTF8.GetString(v));
                    }
                }

            }

            return rv;
        } // ToDictionary()


        public virtual void Populate(Dictionary<String, String> row)
        {
            DataDefinition d = GetDefinition();
            foreach (KeyValuePair<String, String> pair in row)
            {
                if (d.Maps.ContainsKey(pair.Key))
                {
                    IReference property = d.Maps[pair.Key];
                    var thisType = GetType();
                    Type t = property.PropertyType;

                    if (t.IsGenericType && t.GetGenericArguments().Length > 0)
                    {
                        t = t.GetGenericArguments()[0];
                    }

                    if (t == null)
                    {
                        var propertyType = thisType.GetProperty(pair.Key);
                        var fieldType = thisType.GetField(pair.Key);

                        if (propertyType != null)
                        {
                            t = propertyType.PropertyType;
                        }
                        else if (fieldType != null)
                        {
                            t = fieldType.FieldType;
                        }
                        else
                        {
                            t = typeof(Object);
                        }
                    }

                    if (!String.IsNullOrEmpty(pair.Value)) {
                        if (t == typeof(Int16))
                        {
                            property.Set(Convert.ToInt16(pair.Value));
                        }
                        else if (t == typeof(UInt16))
                        {
                            property.Set(Convert.ToUInt16(pair.Value));
                        }
                        else if (t == typeof(Int32))
                        {
                            property.Set(Convert.ToInt32(pair.Value));
                        }
                        else if (t == typeof(UInt32))
                        {
                            property.Set(Convert.ToUInt32(pair.Value));
                        }
                        else if (t == typeof(Int64))
                        {
                            property.Set(Convert.ToInt64(pair.Value));
                        }
                        else if (t == typeof(Single))
                        {
                            property.Set(Convert.ToSingle(pair.Value));
                        }
                        else if (t == typeof(Double))
                        {
                            property.Set(Convert.ToDouble(pair.Value));
                        }
                        else if (t == typeof(Decimal))
                        {
                            property.Set(Convert.ToDecimal(pair.Value));
                        }
                        else if (t == typeof(DateTime))
                        {
                            property.Set(Convert.ToDateTime(pair.Value));
                        }
                        else if (t == typeof(Guid))
                        {
                            Guid g = Guid.Empty;
                            try
                            {
                                g = new Guid(pair.Value);
                            }
                            catch
                            {
                                // do nothing.
                            }
                            property.Set(g);
                        }
                        else if (t == typeof(Nullable<Boolean>) || t == typeof(Boolean))
                        {
                            if (pair.Value != null) {
                                Boolean v = false;
                                switch (pair.Value.ToLower())
                                {
                                    case "1":
                                    case "true":
                                        v = true;
                                        break;
                                    default:
                                        break;
                                }
                                property.Set(v);
                            }
                        }
                        else if (t == typeof(String))
                        {
                            property.Set(Convert.ToString(pair.Value));
                        }
                        else if (t == typeof(Byte[]))
                        {
                            property.Set(System.Text.Encoding.UTF8.GetBytes(pair.Value));
                        }
                        else
                        {
                            property.Set(pair.Value);
                        }
                    }
                }
            }
        } // Populate(Dictionary<String,String>)


        public virtual void Populate(System.Data.IDataReader reader)
        {
            Populate(SqlQueryBuilder.GetDictionary(reader));
        } // Populate(IDataReader)


        public virtual void Populate(String key, String value)
        {
            var row = new Dictionary<String, String>();
            row.Add(key, value);
            Populate(row);
        } // Populate(String, String)


        public virtual DataObject Copy()
        {
            return (DataObject)this.MemberwiseClone();
        }

    }



    [Serializable]
    public abstract class DataObject<T, IDT> : DataObject where T : DataObject<T, IDT>, new()
    {

        public abstract QueryBuilder GetQueryBuilder();

        public static T Get(IDT id)
        {

            // avoid unwanted table scans
            if (Guid.Empty.Equals(id))
            {
                return null;
            }

            T t = new T();
            DataDefinition d = t.GetDefinition();
            //if (!String.IsNullOrEmpty(d.PrimaryKey) && d.Maps[d.PrimaryKey].PropertyType == typeof(Guid)) //causing Get to fail if type was anything but a GUID
            if (!String.IsNullOrEmpty(d.PrimaryKey) && d.Maps[d.PrimaryKey].PropertyType == typeof(IDT))
            {
                d.Maps[d.PrimaryKey].Set(id);
                var list = Get(t);

                if (list.Count == 0)
                {
                    return null;
                }
                else if (list.Count != 1)
                {
                    throw new Exception("Query against PK (" + d.DataEntity + "." + d.PrimaryKey + ") returned more than one row.");
                }
                else
                {
                    return list[0];
                }
            }

            return null;
        }

        public static List<T> Get(
            DataObject condition,
            Boolean fuzzy = false,
            String order = null,
            Int32 limit = Int32.MaxValue,
            T start = null)
        {
            return Get(new List<DataObject>() { condition }, fuzzy, order, limit, start);
        }

        public static List<T> Get(
            List<DataObject> conditions,
            Boolean fuzzy = false,
            String order = null,
            Int32 limit = Int32.MaxValue,
            T start = null)
        {
            using (var qb = (new T()).GetQueryBuilder())
            {
                return qb.Find<T>(conditions, fuzzy, order, limit, start);
            }
        }

        public static List<T> GetAll(
            String order = null,
            Int32 limit = Int32.MaxValue,
            T start = null)
        {
            return Get(new List<DataObject>() { }, false, order, limit, start);
        }

        /// <summary>
        /// Updates or Inserts the record accordingly.
        /// </summary>
        /// <param name="fullUpdate">Whether to include "empty" values (like 0, NULL, "") and booleans in the save.</param>
        /// <returns></returns>
        public Boolean Save(Boolean fullUpdate = false)
        {
            using (var qb = (new T()).GetQueryBuilder())
            {
                return qb.Save(this, fullUpdate) == 1;
            }
        }

        public Int32 Delete()
        {
            using (var qb = (new T()).GetQueryBuilder())
            {
                return qb.Delete(this);
            }
        }

        //public override DataObject Copy()
        //{
        //    var rv = new T();
        //    rv.Populate(ToDictionary(true));
        //    return rv;
        //}

    }

    [Serializable]
    public abstract class DataObject <T> : DataObject<T, Guid> where T : DataObject<T>, new()
    {
    }

}
