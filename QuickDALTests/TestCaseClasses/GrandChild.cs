using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDALTests.TestCaseClasses
{
    public class GrandChild
    {
        public Guid GrandChildId { get; set; }
        public Guid ChildId { get; set; }
        public String Name { get; set; }

        public Child Child
        {
            get
            {
                if (_Child == null || !_Child.ChildId.Equals(ChildId))
                {
                    _Child = Child.Get(ChildId);
                }
                return _Child;
            }
            set
            {
                _Child = value;
                if (value != null)
                {
                    ChildId = value.ChildId;
                } else
                {
                    ChildId = Guid.Empty;
                }
            }
        }
        private Child _Child;
    }
}
