using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class JoinPath
    {

        public List<DataRelationship> Relationships = new List<DataRelationship>();
        public List<String> SqlInnerJoins
        {
            get
            {
                return Relationships.Select(r => r.InnerJoinSQL).ToList();
            }
        }

        public JoinPath(DataObject a, DataObject b, Int32 maxNodes = 512)
            : this(a.GetDefinition(), b.GetDefinition(), maxNodes)
        {
        }

        public JoinPath(DataDefinition a, DataDefinition b, Int32 maxNodes = 512)
        {
            if (a == b)
            {
                return;
            }

            var q = new Queue<TreeNode<DataRelationship>>();
            EnqueueRelations(q, a);

            while (q.Count > 0 && maxNodes > 0)
            {
                TreeNode<DataRelationship> path = q.Dequeue();
                DataRelationship d = path.Value;

                // we found it!
                if (d.RemoteEntity.GetDefinition() == b)
                {
                    RecordPath(path);
                    break;
                }
                else
                {
                    EnqueueRelations(q, d.RemoteEntity.GetDefinition(), path);
                }

                maxNodes--;
            }

            if (Relationships.Count == 0)
            {
                throw new Exception("No path exists between " + a.DataEntity + " and " + b.DataEntity);
            }
        }

        private void RecordPath(TreeNode<DataRelationship> path)
        {
            Relationships.Clear();

            while (path != null && path.Value != null)
            {
                Relationships.Add(path.Value);
                path = path.Parent;
            }

            // necessary? ... not sure.
            Relationships.Reverse();
        }

        private void EnqueueRelations(Queue<TreeNode<DataRelationship>> q, DataDefinition target, TreeNode<DataRelationship> parentPath = null)
        {
            foreach (DataRelationship r in target.Parents)
            {
                q.Enqueue(new TreeNode<DataRelationship>(r, parentPath));
            }

            foreach (DataRelationship r in target.Children)
            {
                q.Enqueue(new TreeNode<DataRelationship>(r, parentPath));
            }
        }
    }
}
