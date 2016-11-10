using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusyManager
{
    enum TargetObjectState { Free, Busy, Available, notAvailable, Maintenance }
    class TargetObject
    {
        public const TargetObjectState DefaultState = TargetObjectState.Available;
        private string idname;
        public string IDName { get { return idname; } }
        public Stack<TargetTimeState> StateChangesList;
        public System.Windows.Thickness Margin { set; get; }
        public string Properties { set; get; }
        public TargetObject(string name, System.Windows.Thickness margin)
        {
            this.idname = name;
            this.Properties = "Simple object";
            this.Margin = margin;
            StateChangesList = new Stack<TargetTimeState>();
        }
        public TargetObject(string name, string properties, System.Windows.Thickness margin)
        {
            this.idname = name;
            this.Properties = properties;
            this.Margin = margin;
            StateChangesList = new Stack<TargetTimeState>();
        }
        public bool InsertChangeBrut(TargetTimeState change)
        {
            Stack<TargetTimeState> buffer = new Stack<TargetTimeState>();
            bool Achieved = false;
            while (StateChangesList.Count > 0 && !Achieved)
            {
                if (StateChangesList.Peek().End > change.Begin)
                    if (StateChangesList.Peek().Begin > change.Begin)
                        buffer.Push(StateChangesList.Pop());
                    else
                    {
                        TargetTimeState previous = StateChangesList.Pop();
                        if (buffer.Count > 0&&(buffer.Peek().Begin<change.End))
                        {
                            TargetTimeState next = buffer.Pop();
                            StateChangesList.Push(new TargetTimeState(previous.State, previous.Customer, previous.Begin, change.Begin));
                            StateChangesList.Push(change);
                            StateChangesList.Push(new TargetTimeState(next.State, next.Customer, change.End, next.End));
                        }
                        else
                        {
                            StateChangesList.Push(new TargetTimeState(previous.State, previous.Customer, previous.Begin, change.Begin));
                            StateChangesList.Push(change);
                        }
                        Achieved = true;
                    }
                else
                {
                    if (buffer.Count > 0 && (buffer.Peek().Begin < change.End))
                    {
                        TargetTimeState next = buffer.Pop();
                        StateChangesList.Push(change);
                        StateChangesList.Push(new TargetTimeState(next.State, next.Customer, change.End, next.End));
                    }
                    else
                    {
                        StateChangesList.Push(change);
                    }
                    Achieved = true;
                }
            }
            while (buffer.Count > 0)
                StateChangesList.Push(buffer.Pop());
            return true;
        }
        public bool InsertChangeOnFree(TargetTimeState change)
        {
            Stack<TargetTimeState> buffer = new Stack<TargetTimeState>();
            bool Achieved = false;
            bool Inserted = false;
            while (StateChangesList.Count > 0 && !Achieved)
            {
                if (StateChangesList.Peek().End > change.Begin)
                {
                    if (StateChangesList.Peek().Begin > change.Begin)
                        buffer.Push(StateChangesList.Pop());
                    else
                        Achieved = true;
                }
                else
                {
                    if (buffer.Count < 1 || (buffer.Peek().Begin > change.End))
                    {
                        StateChangesList.Push(change);
                        Achieved = true;
                        Inserted = true;
                    }
                }
            }
            while (buffer.Count > 0)
                StateChangesList.Push(buffer.Pop());
            return Inserted;
        }
        public bool ExpandCurent(TimeSpan period)
        {
            if (StateChangesList.Count > 0)
            {
                TargetTimeState previous = StateChangesList.Pop();
                StateChangesList.Push(new TargetTimeState(previous.State, previous.Customer, previous.Begin, previous.End + period));
                return true;
            }
            else return false;
        }
        public bool ChangeCurent(TargetObjectState newState)
        {
            if (StateChangesList.Count > 0)
            {
                TargetTimeState previous = StateChangesList.Pop();
                StateChangesList.Push(new TargetTimeState(newState, previous.Customer, previous.Begin, previous.End));
                return true;
            }
            else return false;
        }
        public bool AppendState(TargetObjectState newState, string customer, TimeSpan period)
        {
            if (StateChangesList.Count > 0)
            {
                TargetTimeState previous = StateChangesList.Peek();
                StateChangesList.Push(new TargetTimeState(newState, customer, previous.End, period));
                return true;
            }
            else return false;
        }
        public bool AppendState(TargetObjectState newState, string customer, DateTime end)
        {
            if (StateChangesList.Count > 0)
            {
                TargetTimeState previous = StateChangesList.Peek();
                StateChangesList.Push(new TargetTimeState(newState, customer, previous.End, end));
                return true;
            }
            else return false;
        }
    }
    class TargetTimeState
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
}
