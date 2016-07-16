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
        public Boolean DisableTableLocking;


        public DataRelationship()
        {
        }

        public DataRelationship(DataObject local, String localField, DataObject remote, String remoteField, Boolean disableTableLocking = false)
        {
            this.LocalEntity = local;
            this.LocalField = localField;
            this.RemoteEntity = remote;
            this.RemoteField = remoteField;
            this.DisableTableLocking = disableTableLocking;
        }
        
        public DataRelationship(DataObject local, DataObject remote, String field)
            : this(local, field, remote, field)
        {
        }

        //public String InnerJoinSQL {
        //    get {
        //        return "inner join [" + RemoteEntity.GetDefinition().DataEntity + "] [" + RemoteEntity.GetDefinition().Name + "] on (["
        //            + LocalEntity.GetDefinition().Name + "]." + LocalField + "=[" + RemoteEntity.GetDefinition().Name + "]." + RemoteField + ")";
        //    }
        //}

        /// <summary>
        /// Change to InnerJoinSQL allows multi-part identifiers to be supplied in the RemoteField / LocalField connections
        /// Identifiers must be comma-delimited, of equal length, and in the necessary order to match correctly
        /// </summary>
        public String InnerJoinSQL
        {
            get
            {
                StringBuilder sb = new StringBuilder();                
                if (RemoteField.Contains(',') && LocalField.Contains(','))
                {
                    sb.AppendFormat("inner join [{0}] ", RemoteEntity.GetDefinition().DataEntity);
                    if (DisableTableLocking)
                    {
                        sb.AppendFormat("[{0}] with (nolock) on ", RemoteEntity.GetDefinition().Name);
                    }
                    else
                    {
                        sb.AppendFormat("[{0}] on ", RemoteEntity.GetDefinition().Name);
                    }
                    sb.Append("(");
                    string[] remoteFields = RemoteField.Split(',');
                    string[] localFields = LocalField.Split(',');
                    if (remoteFields.Length != localFields.Length)
                    {
                        throw new Exception("Length mismatch in field relationship between " + RemoteEntity.GetDefinition().DataEntity + " and " + LocalEntity.GetDefinition().DataEntity);
                    }
                    for (int i = 0; i < remoteFields.Length; i++) 
                    {
                        sb.AppendFormat("[{0}].[{1}] = [{2}].[{3}]", LocalEntity.GetDefinition().Name, localFields[i], RemoteEntity.GetDefinition().Name, remoteFields[i]);
                        if (i + 1 != remoteFields.Length)
                        {
                            sb.Append(" and ");
                        }
                    }
                    sb.Append(")");
                }
                else //Else clause is intended to be identical to the original InnerJoinSQL property
                {
                    if (DisableTableLocking)
                    {
                        sb.AppendFormat("inner join [{0}] [{1}] with (nolock)", RemoteEntity.GetDefinition().DataEntity, RemoteEntity.GetDefinition().Name);
                    }
                    else
                    {
                        sb.AppendFormat("inner join [{0}] [{1}]", RemoteEntity.GetDefinition().DataEntity, RemoteEntity.GetDefinition().Name);
                    }
                    sb.AppendFormat(" on ([{0}].[{1}] = [{2}].[{3}]) ", LocalEntity.GetDefinition().Name, LocalField, RemoteEntity.GetDefinition().Name, RemoteField);                  
                }               

                return sb.ToString();
            }
        }

    }
}
