using System.ComponentModel;
using Verse;
using RimWorld;

namespace UnificaMagica {


    public static class Helpers {

        public static void PrintObject(object _obj) {
            Log.Message("PrintObject :"+_obj.GetType().ToString());
            foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(_obj))
            {
                string name=descriptor.Name;
                object value=descriptor.GetValue(_obj);
                Log.Message(string.Format("  {0}={1}",name,value));
            }
        }
    }

}
