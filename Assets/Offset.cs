using System;
using System.Collections.Generic;

public class Offset
{
    public class BoolTuple
    {
        public bool Item1 { get; set; }
        public bool Item2 { get; set; }
        public bool Item3 { get; set; }
        public bool Item4 { get; set; }

        public BoolTuple(bool item1, bool item2, bool item3, bool item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BoolTuple t = (BoolTuple)obj;
            return (Item1 == t.Item1) && (Item2 == t.Item2) && (Item3 == t.Item3) && (Item4 == t.Item4);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Item1.GetHashCode();
            hash = hash * 23 + Item2.GetHashCode();
            hash = hash * 23 + Item3.GetHashCode();
            hash = hash * 23 + Item4.GetHashCode();
            return hash;
        }
    }

    public int SelectOffset(bool[] config)
    {
        var offsets = new Dictionary<BoolTuple, int>();
        offsets.Add(new BoolTuple(true, true, false, false), 14);
        offsets.Add(new BoolTuple(true, false, true, false), 16);
        offsets.Add(new BoolTuple(true, false, false, true), 8);
        offsets.Add(new BoolTuple(false, true, true, false), 3);
        offsets.Add(new BoolTuple(false, true, false, true), 15);
        offsets.Add(new BoolTuple(false, false, true, true), 10);
        offsets.Add(new BoolTuple(true, true, true, false), 12);
        offsets.Add(new BoolTuple(true, true, false, true), 2);
        offsets.Add(new BoolTuple(true, false, true, true), 5);
        offsets.Add(new BoolTuple(false, true, true, true), 11);
        offsets.Add(new BoolTuple(true, true, true, true), 9);
        offsets.Add(new BoolTuple(true, false, false, false), 13);
        offsets.Add(new BoolTuple(false, true, false, false), 6);
        offsets.Add(new BoolTuple(false, false, true, false), 7);
        offsets.Add(new BoolTuple(false, false, false, true), 4);
        offsets.Add(new BoolTuple(false, false, false, false), 1);

        var key = new BoolTuple(config[0], config[1], config[2], config[3]);

        int offset;
        if (offsets.TryGetValue(key, out offset))
        {
            return offset;
        }
        return 0;
    }
}