using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusyManager
{
    [Serializable]
    public enum TargetObjectState { Free, Busy, Available, notAvailable, Maintenance }
    public class TargetObject
    {
        public const TargetObjectState DefaultState = TargetObjectState.Available;
        private string idname;
        public string IDName { get { return idname; } }
        private Stack<TargetTimeState> stateChangesList;
        public System.Windows.Thickness Margin { set; get; }
        public string Properties { set; get; }
        public TargetObject(string name, System.Windows.Thickness margin)
        {
            this.idname = name;
            this.Properties = "Simple object";
            this.Margin = margin;
            stateChangesList = new Stack<TargetTimeState>();
        }
        public TargetObject(string name, string properties, System.Windows.Thickness margin)
        {
            this.idname = name;
            this.Properties = properties;
            this.Margin = margin;
            stateChangesList = new Stack<TargetTimeState>();
        }
        public bool InsertChangeBrut(TargetTimeState change)
        {
            if (stateChangesList.Count == 0)
            {
                stateChangesList.Push(change);
                return true;
            }
            else
            {
                Stack<TargetTimeState> buffer = new Stack<TargetTimeState>();
                bool Achieved = false;
                while (stateChangesList.Count > 0 && !Achieved)
                {
                    if (stateChangesList.Peek().End > change.Begin)
                        if (stateChangesList.Peek().Begin > change.Begin)
                            buffer.Push(stateChangesList.Pop());
                        else
                        {
                            TargetTimeState previous = stateChangesList.Pop();
                            if (buffer.Count > 0 && (buffer.Peek().Begin < change.End))
                            {
                                TargetTimeState next = buffer.Pop();
                                stateChangesList.Push(new TargetTimeState(previous.State, previous.Customer, previous.Begin, change.Begin));
                                stateChangesList.Push(change);
                                stateChangesList.Push(new TargetTimeState(next.State, next.Customer, change.End, next.End));
                            }
                            else
                            {
                                stateChangesList.Push(new TargetTimeState(previous.State, previous.Customer, previous.Begin, change.Begin));
                                stateChangesList.Push(change);
                            }
                            Achieved = true;
                        }
                    else
                    {
                        if (buffer.Count > 0 && (buffer.Peek().Begin < change.End))
                        {
                            TargetTimeState next = buffer.Pop();
                            stateChangesList.Push(change);
                            stateChangesList.Push(new TargetTimeState(next.State, next.Customer, change.End, next.End));
                        }
                        else
                        {
                            stateChangesList.Push(change);
                        }
                        Achieved = true;
                    }
                }
                while (buffer.Count > 0)
                    stateChangesList.Push(buffer.Pop());
                return true;
            }
        }
        public bool InsertChangeOnFree(TargetTimeState change)
        {
            if (stateChangesList.Count == 0)
            {
                stateChangesList.Push(change);
                return true;
            }
            else
            {
                Stack<TargetTimeState> buffer = new Stack<TargetTimeState>();
                bool Achieved = false;
                bool Inserted = false;
                while (stateChangesList.Count > 0 && !Achieved)
                {
                    if (stateChangesList.Peek().End > change.Begin)
                    {
                        if (stateChangesList.Peek().Begin > change.Begin)
                            buffer.Push(stateChangesList.Pop());
                        else
                            Achieved = true;
                    }
                    else
                    {
                        if (buffer.Count < 1 || (buffer.Peek().Begin > change.End))
                        {
                            stateChangesList.Push(change);
                            Achieved = true;
                            Inserted = true;
                        }
                    }
                }
                while (buffer.Count > 0)
                    stateChangesList.Push(buffer.Pop());
                return Inserted;
            }
        }
        public bool ClearChanges()
        {
            stateChangesList.Clear();
            return true;
        }
        public bool PushChanges(TargetTimeState change)
        {
            stateChangesList.Push(change);
            return true;
        }
        //public bool FillStateChangesList(Stack<TargetTimeState> changes)
        //{
        //    this.StateChangesList.Clear();
        //    this.StateChangesList.Concat(changes);
        //    return true;
        //}
        public bool ExpandCurent(TimeSpan period)
        {
            if (stateChangesList.Count > 0)
            {
                TargetTimeState previous = stateChangesList.Pop();
                stateChangesList.Push(new TargetTimeState(previous.State, previous.Customer, previous.Begin, previous.End + period));
                return true;
            }
            else return false;
        }
        public bool ChangeCurent(TargetObjectState newState)
        {
            if (stateChangesList.Count > 0)
            {
                TargetTimeState previous = stateChangesList.Pop();
                stateChangesList.Push(new TargetTimeState(newState, previous.Customer, previous.Begin, previous.End));
                return true;
            }
            else return false;
        }
        public bool AppendState(TargetObjectState newState, string customer, TimeSpan period)
        {
            if (stateChangesList.Count > 0)
            {
                TargetTimeState previous = stateChangesList.Peek();
                stateChangesList.Push(new TargetTimeState(newState, customer, previous.End, period));
                return true;
            }
            else return false;
        }
        public bool AppendState(TargetObjectState newState, string customer, DateTime end)
        {
            if (stateChangesList.Count > 0)
            {
                TargetTimeState previous = stateChangesList.Peek();
                stateChangesList.Push(new TargetTimeState(newState, customer, previous.End, end));
                return true;
            }
            else return false;
        }
        public TargetObjectState GetState(DateTime date)
        {
            Stack<TargetTimeState> buffer = new Stack<TargetTimeState>();
            bool Achieved = false;
            TargetObjectState output = DefaultState;
            while (stateChangesList.Count > 0 && !Achieved)
            {
                if (stateChangesList.Peek().End > date)
                {
                    if (stateChangesList.Peek().Begin > date)
                        buffer.Push(stateChangesList.Pop());
                    else
                    {
                        Achieved = true;
                        output = stateChangesList.Peek().State;
                    }
                }
                else
                {
                    Achieved = true;
                    output = DefaultState;
                }
            }
            while (buffer.Count > 0)
                stateChangesList.Push(buffer.Pop());
            return output;
        }
        public TargetTimeState[] GetAllChanges()
        {
            return stateChangesList.ToArray();
        }
        public void ChangeName(string name)
        {
            this.idname = name;
        }
    }
    public class TargetTimeState
    {
        private TargetObjectState state;
        private DateTime begin;
        private DateTime end;
        private string customer;
        public TargetTimeState(TargetObjectState state, string customer, DateTime end)
        {
            this.state = state;
            this.customer = customer;
            this.begin = DateTime.Now;
            this.end = end;
        }
        public TargetTimeState(TargetObjectState state, string customer, TimeSpan period)
        {
            this.state = state;
            this.customer = customer;
            this.begin = DateTime.Now;
            this.end = DateTime.Now + period;
        }
        public TargetTimeState(TargetObjectState state, string customer, DateTime begin, DateTime end)
        {
            this.state = state;
            this.customer = customer;
            this.begin = begin;
            this.end = end;
        }
        public TargetTimeState(TargetObjectState state, string customer, DateTime begin, TimeSpan period)
        {
            this.state = state;
            this.customer = customer;
            this.begin = begin;
            this.end = begin + period;
        }
        public TargetObjectState State { get { return state; } }
        public string Customer { get { return customer; } }
        public DateTime Begin { get { return begin; } }
        public DateTime End { get { return end; } }
        public TimeSpan Period { get { return end - begin; } }
    }
    [Serializable]
    public class TargetObjectSer
    {
        public string idname;
        public TargetTimeStateSer[] changes;
        public System.Windows.Thickness margin;
        public string properties;
        public TargetObjectSer() { }
        public TargetObjectSer(TargetObject toPack)
        {
            this.idname = toPack.IDName;
            this.margin = toPack.Margin;
            this.properties = toPack.Properties;
            TargetTimeState[] buffer = toPack.GetAllChanges();
            this.changes = new TargetTimeStateSer[buffer.Length];
            for(int i = 0; i<buffer.Length;i++)
                changes[i] = new TargetTimeStateSer(buffer[i]);
        }
        public TargetObject Unpack()
        {
            TargetObject outp = new TargetObject(idname, properties, margin);
            for (int i = 0; i < changes.Length; i++)
                outp.PushChanges(changes[i].Unpack());
            return outp;
        }
    }
    [Serializable]
    public class TargetTimeStateSer
    {
        public TargetObjectState state;
        public DateTime begin;
        public DateTime end;
        public string customer;
        public TargetTimeStateSer() { }
        public TargetTimeStateSer(TargetTimeState toPack)
        {
            this.state = toPack.State;
            this.customer = toPack.Customer;
            this.begin = toPack.Begin;
            this.end = toPack.End;
        }
        public TargetTimeState Unpack()
        {
            return new TargetTimeState(this.state, this.customer, this.begin, this.end);
        }
    }
}
