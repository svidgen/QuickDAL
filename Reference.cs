using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{

    public interface IReference
    {
        Func<Object> Get { get; set; }
        Action<Object> Set { get; set; }
        Type PropertyType { get; set; }
    }


    public class Reference : IReference
    {
        public Func<Object> Get { get; set; }
        public Action<Object> Set { get; set; }
        public Type PropertyType { get; set; }

        public Reference(Func<Object> Getter, Action<Object> Setter, Type type = null)
        {
            Get = Getter;
            Set = Setter;
            PropertyType = type != null ? type : typeof(Object);
        }

    }


    public class Reference<T> : IReference
    {
        public Func<Object> Get { get; set; }
        public Action<Object> Set { get; set; }
        public Type PropertyType { get; set; }

        public Reference(Func<T> Getter, Action<T> Setter)
        {
            Get = () => (T)Getter();
            Set = (o) => Setter((T)o);
            PropertyType = typeof(T);
        }

    }
}
