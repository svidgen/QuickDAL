using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class DataRelationship
    {
        public String LocalField;
        public String RemoteField;
        public DataObject LocalEntity;
        public DataObject RemoteEntity;

        public DataRelationship()
        {
        }

        public DataRelationship(DataObject local, String localField, DataObject remote, String remoteField)
        {
            this.LocalEntity = local;
            this.LocalField = localField;
            this.RemoteEntity = remote;
            this.RemoteField = remoteField;
        }
        
        public DataRelationship(DataObject local, DataObject remote, String field)
            : this(local, field, remote, field)
        {
        }

        public String InnerJoinSQL {
            get {
                return "inner join [" + RemoteEntity.GetDefinition().DataEntity + "] [" + RemoteEntity.GetDefinition().Name + "] on (["
                    + LocalEntity.GetDefinition().Name + "]." + LocalField + "=[" + RemoteEntity.GetDefinition().Name + "]." + RemoteField + ")";
            }
        }

    }
}
