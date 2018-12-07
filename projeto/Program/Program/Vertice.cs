using System;

namespace Program
{
    //[Serializable]
    public class Vertice
    {
        public int id;
        public string name;
        public int visit;
        public int feeder;
        public int index;
        public Vertice()
	    {
        
        }

        //public Vertice Clone()
        //{
        //    var clone = (Vertice)this.MemberwiseClone();
        //    return clone;
        //}
    }
}
