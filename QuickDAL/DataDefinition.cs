using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class DataDefinition
    {
        /// <summary>
        /// A unique name, which defaults to the DataEntity. Override this value if two
        /// "distinct" types live in the same table.
        /// </summary>
        public String Name {
            get
            {
                if (String.IsNullOrEmpty(_Name))
                {
                    return DataEntity;
                }
                else
                {
                    return _Name;
                }
            }
            set
            {
                _Name = value;
            }
        }
        private String _Name;

        /// <summary>
        /// The SQL Table(s) that hold this entity.
        /// </summary>
        public String DataEntity;
        public Boolean Distinctable;
        public List<String> Fields;
        public Dictionary<String, IReference> Maps;
        public String PrimaryKey;
        public String DefaultOrder;
        public List<DataRelationship> Parents;
        public List<DataRelationship> Children;
        public ICacheCollection CacheCollection;


        public DataDefinition()
        {
            Name = null;
            DataEntity = "";
            Distinctable = true;
            Fields = new List<String>();
            Maps = new Dictionary<String, IReference>();
            Parents = new List<DataRelationship>();
            Children = new List<DataRelationship>();
        }

        /// <summary>
        /// Compares DataDefinitions by Name.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public override Boolean Equals(Object a)
        {
            if (a is DataDefinition)
            {
                return Name.Equals(((DataDefinition)a).Name);
            }
            else
            {
                return false;
            }
        }

        public static Boolean operator ==(DataDefinition a, DataDefinition b)
        {
            if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null))
            {
                return true;
            }

            if (Object.ReferenceEquals(a, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static Boolean operator !=(DataDefinition a, DataDefinition b)
        {
            return !a.Equals(b);
        }
    }
}
