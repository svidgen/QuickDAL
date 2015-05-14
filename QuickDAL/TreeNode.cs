using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    class TreeNode<T> where T : new()
    {

        public TreeNode<T> Parent;
        public T Value;

        public TreeNode(T value, TreeNode<T> parent = null)
        {
            Value = value;
            Parent = parent;
        }

    }
}
