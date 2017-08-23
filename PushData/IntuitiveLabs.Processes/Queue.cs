using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IntuitiveLabs.Processes
{
    public class ProcessScheduler
    {
        private List<Process> items;
        private List<ManualResetEvent> resetEvents;

        public ProcessScheduler()
        {
            this.items = new List<Process>();
            this.resetEvents = new List<ManualResetEvent>();
        }

        public void AddProcess(IActor actor, string description, TimeSpan period )
        {
            ManualResetEvent rt = new ManualResetEvent( false );
            this.resetEvents.Add(rt);

            Process proc = new Process(actor, description, period, rt);
            proc.LongDuration += new Process.LongDurationHandler(proc_LongDuration);
                        
            this.items.Add(proc);
        }

        void proc_LongDuration(ProcessScheduler.Process proc, TimeSpan duration)
        {
            if (this.LongDuration != null)
                LongDuration(this, proc.Actor, duration);
        }

        public delegate void LongDurationHandler(object sender, IActor actor, TimeSpan duration);
        public event LongDurationHandler LongDuration;

        public void Start()
        {
            Console.WriteLine( this.items.Count.ToString() );

            TimeSpan pause = TimeSpan.MaxValue;

            foreach (Process proc in this.items)
            {
                //Console.WriteLine(pause.ToString() + ":" + proc.Period.ToString());

                if (proc.Period < pause )
                    pause = proc.Period;
            }

            Console.WriteLine(pause.ToString());


        Start:
            DateTime nextTime = DateTime.MaxValue;

            foreach (Process proc in this.items)
            {
                //Threading must be removed, untill a total redesign of code due to eroor arising when DataSourceSchema is read by mutliple threads
                //Code was not written for multi threading!!! Code changes will affect MIXFacories. Wait with threading till release 3.0.

                //Check if the Process is overdue for execution and that it isn't already in the thread pool

                if (proc.NextTime <= DateTime.Now)                
                {
                    if (proc.State == ProcessState.Completed || proc.State == ProcessState.Initiated )
                    {
                        //Reset the Process and then place the Process in the thread pool.
                        proc.Reset();
                        ThreadPool.QueueUserWorkItem(proc.Execute, proc.Actor.ThreadContext);

                        //proc.Execute();

                        //Remove this line when threading is reintroduced. Also Calculating pause will no longer be neccessary
                        //proc.ScheduleNext();
                    }
                    else
                    {
                        //If the Process is already in thread pool, we will try to execute it again in another cycle.
                        proc.ScheduleNext();

                        //We should add a signal that the period for the execution cycle is in fact too short
                        //This may be because there are too many threads or because the IActor took to long to perform.
                        //Look for ways to distinguish which is the cause.
                    }                    
                }

                if (proc.NextTime < nextTime)
                    nextTime = proc.NextTime;
            }

            Thread.Sleep(pause);

            /*
            pause = nextTime.Subtract(DateTime.Now);

            Console.WriteLine(pause.ToString());

            if( pause > TimeSpan.Zero )
                Thread.Sleep(pause);
            */

            goto Start;             
        }

        public List<Process> Items
        {
            get { return this.items; }
        }

        public sealed class Process
        {
            private string description;
            private IActor actor;
            private Guid guid;
            private DateTime nextTime;
            private TimeSpan period;
            private TimeSpan avgDuration;
            private ManualResetEvent resetEvent;
            private ProcessState state;            

            public Process( IActor actor, string description, TimeSpan period, ManualResetEvent resetEvent )
            {
                //IActor encapsulates the object that has that actual method that is the process itself (the IActor.Perform method).
                this.guid = Guid.NewGuid();
                this.actor = actor;
                this.description = description;
                this.period = period;
                this.resetEvent = resetEvent;
                this.state = ProcessState.Initiated;
                this.nextTime = DateTime.Now;
            }

            public IActor Actor
            {
                get{ return this.actor; }
            }

            public DateTime NextTime
            {
                get { return this.nextTime; }
            }

            public void Execute()
            {
                if (this.state != ProcessState.Initiated && this.state != ProcessState.Reset)
                    throw new Exception("Process must be reset by calling the Reset() method before being executed again.");

                this.state = ProcessState.Started;

                DateTime startTime = DateTime.Now;

                //Thread.Sleep(5000);

                this.Actor.Perform();

                this.state = ProcessState.Completed;

                if (this.Completed != null)
                    this.Completed(this);

                this.resetEvent.Set();

                TimeSpan dur = DateTime.Now.Subtract(startTime);

                Console.WriteLine("Period: " + this.period.ToString());
                Console.WriteLine("Execution time: " + dur.ToString());

                if (dur > this.period && this.period != TimeSpan.Zero && this.LongDuration != null)
                    LongDuration(this, dur);
            }

            public void Execute(object threadContext)
            {
                if (this.state != ProcessState.Initiated && this.state != ProcessState.Reset)
                    throw new Exception("Process must be reset by calling the Reset() method before being executed again.");

                this.state = ProcessState.Started;

                DateTime startTime = DateTime.Now;

                //Thread.Sleep(5000);

                this.Actor.Perform( threadContext );

                this.state = ProcessState.Completed;

                if (this.Completed != null)
                    this.Completed(this);

                this.resetEvent.Set();

                TimeSpan dur = DateTime.Now.Subtract(startTime);

                Console.WriteLine("Period: " + this.period.ToString());
                Console.WriteLine("Execution time: " + dur.ToString());

                if (dur > this.period && this.period != TimeSpan.Zero && this.LongDuration != null)
                    LongDuration(this, dur);
            }


            public delegate void CompletedHandler(Process proc);
            public event CompletedHandler Completed;

            public delegate void LongDurationHandler(Process proc, TimeSpan duration);
            public event LongDurationHandler LongDuration;

            public TimeSpan Period
            {
                get { return this.period; }
            }
   
            public void Reset()
            {
                this.state = ProcessState.Reset;
            }

            public void ScheduleNext()
            {
                this.nextTime = DateTime.Now.Add(this.period);
            }

            public ProcessState State
            {
                get { return this.state; }
            }

        }


        public enum ProcessState
        {
            Initiated, Reset, Started, Completed
        }
        
    }
}
