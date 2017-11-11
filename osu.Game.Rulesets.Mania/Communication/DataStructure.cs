using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace osu.Game.Rulesets.Mania.Communication
{
    public class DataStructure<T>
    {
        public T data;
        public string type;
        public DateTime date;
        public  DataStructure(string eventName, T data)
        {
            this.type = eventName;
            this.data = data;
            date = DateTime.Now;
        }
        public override string ToString()
        {
           return JsonConvert.SerializeObject(this);
        }
    }
}
