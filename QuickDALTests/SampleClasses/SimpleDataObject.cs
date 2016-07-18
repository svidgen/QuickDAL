using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDAL;

namespace QuickDALTests.SampleClasses
{
    public class SimpleDataObject : DataObject<SimpleDataObject>
    {

        public String StringValue { get; set; }
        public Int32 Int32Value { get; set; }
        public Double DoubleValue { get; set; }
        public Boolean BooleanValue { get; set; }
        public Guid GuidValue { get; set; }

        public override DataDefinition GetDefinition()
        {
            return new DataDefinition()
            {
                PrimaryKey = "GuidValue",
                DataEntity = "SimpleDataObject",
                Maps = new Dictionary<string, IReference>()
                {
                    {"StringValue", new Reference<String>(() => StringValue, (v) => StringValue = v)},
                    {"Int32Value", new Reference<Int32>(() => Int32Value, (v) => Int32Value = v)},
                    {"DoubleValue", new Reference<Double>(() => DoubleValue, (v) => DoubleValue = v)},
                    {"BooleanValue", new Reference<Boolean>(() => BooleanValue, (v) => BooleanValue = v)},
                    {"GuidValue", new Reference<Guid>(() => GuidValue, (v) => GuidValue = v)}
                }
            };
        }

        public override QueryBuilder GetQueryBuilder()
        {
            return QueryBuilderLocator.GetQueryBuilder();
        }

    }
}
