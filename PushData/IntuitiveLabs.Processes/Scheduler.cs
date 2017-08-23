using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace IntuitiveLabs.Processes
{
    public interface IActor
    {
        string ID { get; }
        object ThreadContext { get; }
        void Perform();
        void Perform(object threadContext);
    }

    public class SchedulerEventLongDurationArgs : EventArgs
    {
        public object ID;
        public TimeSpan Duration;
    }

    public class Scheduler
    {
        private List<ScheduleItem> m_Schedule;
        private IActor m_Actor;
        Thread m_Main = null;
        Mutex m_Mutex = new Mutex();

        public delegate void LongDurationDelegate(object sender, EventArgs e);

        public event LongDurationDelegate LongDuration;
        
        public Scheduler()
        {
            m_Schedule = new List<ScheduleItem>();
        }

        public void AddItem(IActor actor, object ID, TimeSpan Period)
        {           
            if (Period <= TimeSpan.Zero)
                throw new Exception("Period should be > 0 ");
            if (actor == null)
                throw new Exception("Actor is not defined");

            //Console.WriteLine("Adding item: ID=" + ID + " ; actor.ID=" + actor.ID);

            m_Mutex.WaitOne();
            m_Schedule.Add(new ScheduleItem(actor, ID, Period));
            m_Mutex.ReleaseMutex();
        }

        public void RemoveItem(IActor Actor)
        {
            m_Mutex.WaitOne();
            List<ScheduleItem> items = this.FindAll(Actor);
            foreach (ScheduleItem item in items)
                m_Schedule.Remove(item);
            m_Mutex.ReleaseMutex();
        }



        public void RescheduleItem(IActor Actor, TimeSpan NewPeriod)
        {
            m_Mutex.WaitOne();
            List<ScheduleItem> items = this.FindAll(Actor);
            
            foreach (ScheduleItem item in items)
                item.Period = NewPeriod;

            m_Mutex.ReleaseMutex();
        }

        public List<ScheduleItem> ScheduledItems
        {
            get { return this.m_Schedule; }
        }

        public void Start()
        {
            if (m_Main != null)
            {
                throw new Exception("Scheduler is already started");
            }
            m_Main = new Thread(new ThreadStart(Perform));
            m_Main.Start();
        }

        public void Stop()
        {
            if (m_Main == null)
            {
                throw new Exception("Scheduler is already aborted");
            }
            m_Main.Abort();
            m_Main = null;
        }

        protected void Perform()
        {
            try
            {
                m_Mutex.WaitOne();
                while (m_Schedule.Count > 0)
                {
                    //Console.WriteLine("Number of Items:" + m_Schedule.Count);

                    foreach (ScheduleItem item in m_Schedule)
                    {
                        //Console.WriteLine("item.ID=" + item.ID.ToString() + "item.Actor.ID=" + item.Actor.ID);

                        bool done = item.PerformIfBehind();

                        if (!done)
                        {
                            SchedulerEventLongDurationArgs eventArgs = new SchedulerEventLongDurationArgs();
                            eventArgs.ID = item.ID;
                            eventArgs.Duration = item.Duration;
                            this.LongDuration(item.Actor, eventArgs);
                        }
                    }

                    //m_Schedule.Sort();  
                  
                    TimeSpan Pause = m_Schedule[0].NextPerformance - DateTime.Now;

                    if (Pause > TimeSpan.Zero)
                        Thread.Sleep(Pause);
                }
            }
            catch (ThreadAbortException taex)
            {
                m_Mutex.ReleaseMutex();
            }
            finally
            {


            }
        
        }

        private bool Equal(ScheduleItem item)
        {
            return (m_Actor == item.Actor);
        }

        private List<ScheduleItem> FindAll(IActor Actor)
        {
            m_Actor = Actor;
            return m_Schedule.FindAll(Equal);
        }

        public class ScheduleItem : IComparable<ScheduleItem>
        {
            //Scheduler m_Scheduler;
            private IActor actor;
            private TimeSpan period;
            private DateTime performed;
            private TimeSpan duration;

            object id;

            public object ID
            {
                get { return id; }
            }
            
            public int CompareTo(ScheduleItem item)
            {
                if (this.NextPerformance < item.NextPerformance)
                    return -1;
                else if (this.NextPerformance == item.NextPerformance)
                    return 0;
                else
                    return 1;

            }

            internal ScheduleItem(IActor MyActor, object ID, TimeSpan Period)
            {
                //Console.WriteLine("new ScheduleItem{ ID: " + ID + "; MyActor.ID: " + MyActor.ID + "}");

                this.id = ID;
                this.actor = MyActor;
                this.period = Period;
                this.performed = DateTime.Now.Subtract(period);
            }

            public IActor Actor
            {
                get { return this.actor; }
            }

            internal TimeSpan Period
            {
                get { return period; }
                set { period = value; }
            }

            internal DateTime NextPerformance
            {
                get { return performed.Add(period); }
            }

            internal TimeSpan Duration
            {
                get { return duration; }
            }


            internal bool PerformIfBehind()
            {
                //Console.WriteLine( "this.ID: " + this.ID + "; actor.ID: " + actor.ID );
                //System.Threading.Thread.CurrentThread.Join(10000);

                if (Behind)
                {
                    DateTime dtStart = DateTime.Now;
                    MarkPerformed(dtStart);

                    actor.Perform();
                    
                    DateTime dtEnd = DateTime.Now;
                    duration = dtEnd.Subtract(dtStart);
                    if (duration > period)
                    {
                        // skip one cycle - postpone the action and report the error
                        MarkPerformed(dtEnd.Add(duration));
                        return false;
                    }


                }
                return true;
            }

            DateTime Performed
            {
                get { return performed; }
            }


            void MarkPerformed(DateTime Performed)
            {
                performed = Performed;
            }


            bool Behind
            {
                get { return NextPerformance <= DateTime.Now; }
            }
        }
    }
}
