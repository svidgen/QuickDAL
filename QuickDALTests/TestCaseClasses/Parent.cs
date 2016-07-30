using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDAL;

namespace QuickDALTests.TestCaseClasses
{
    public class Parent : DataObject<Parent>
    {
        public Guid ParentId { get; set; }
        public String Name { get; set; }

        public List<Child> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = Child.Get(this);
                }
                return _Children;
            }
            set
            {
                _Children = value;
            }
        }
        private List<Child> _Children;

        public override QueryBuilder GetQueryBuilder()
        {
            return QueryBuilderLocator.GetQueryBuilder();
        }

        public override DataDefinition GetDefinition()
        {
            return new DataDefinition()
            {
                DataEntity = "Parents",
                PrimaryKey = "ParentId",
                Maps = new Dictionary<string, IReference>()
                {
                    {"ParentId", new Reference<Guid>(() => ParentId, (v) => ParentId = v)},
                    {"Name", new Reference<String>(() => Name, (v) => Name = v)}
                },
                Children = new List<DataRelationship>()
                {
                    new DataRelationship(this, new Child(), "ParentId")
                }
            };
        }
    }
}
