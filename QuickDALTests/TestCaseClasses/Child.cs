using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDAL;

namespace QuickDALTests.TestCaseClasses
{
    public class Child : DataObject<Child>
    {
        public Guid ChildId { get; set; }
        public Guid ParentId { get; set; }
        public String Name { get; set; }
        
        public Parent Parent
        {
            get
            {
                if (_Parent == null || !_Parent.ParentId.Equals(ParentId))
                {
                    _Parent = Parent.Get(ParentId);
                }
                return _Parent;
            }
            set
            {
                _Parent = value;
                if (value != null)
                {
                    ParentId = value.ParentId;
                } else
                {
                    ParentId = Guid.Empty;
                }
            }
        }
        private Parent _Parent;

        public override QueryBuilder GetQueryBuilder()
        {
            return QueryBuilderLocator.GetQueryBuilder();
        }

        public override DataDefinition GetDefinition()
        {
            return new DataDefinition()
            {
                DataEntity = "Children",
                PrimaryKey = "ChildId",
                Maps = new Dictionary<string, IReference>()
                {
                    {"ChildId", new Reference<Guid>(() => ChildId, (v) => ChildId = v)},
                    {"ParentId", new Reference<Guid>(() => ParentId, (v) => ParentId = v)},
                    {"Name", new Reference<String>(() => Name, (v) => Name = v)}
                },
                Parents = new List<DataRelationship>()
                {
                    new DataRelationship(this, new Parent(), "ParentId")
                }
            };
        }
    }
}
