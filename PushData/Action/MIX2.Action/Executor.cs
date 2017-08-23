using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace MIX2.Action
{
    public class Executor: Xcellence.Scripting.Executor
    {
        private Xcellence.IInput input;
        private Xcellence.IStorage storage;
        private ManualResetEvent executedEvent;
        private string identifier;

        public Executor(string identifier, List<List<Xcellence.Scripting.Nodes.Node>> sequences, List<Xcellence.MemoryStorage> memoryStores): base(sequences, memoryStores)
        {
            this.identifier = identifier;
        }

        public string Identifier
        {
            get { return this.identifier; }
        }

        public override void Execute()
        {
            string source = "MIX2.Action";

            try
            {
                Xcellence.Environment.Input = input;
                Xcellence.Environment.GlobalStore = storage;

                base.Execute();

                Xcellence.Environment.Input = null;
                Xcellence.Environment.GlobalStore = null;

                System.Diagnostics.EventLog.WriteEntry(source, "Thread " + this.identifier + " started.", EventLogEntryType.Information);

            }
            catch(Exception exc)
            {
                System.Diagnostics.EventLog.WriteEntry(source, "Thread " + this.identifier + " failed: " + exc.Message, EventLogEntryType.Error);
            }

            this.executedEvent.Set();
        }

        public Xcellence.IInput Input
        {
            set { this.input = value; }
        }

        public Xcellence.IStorage Storage
        {
            set { this.storage = value; }
        }

        public ManualResetEvent ExecutedEvent
        {
            set { this.executedEvent = value; }
        }
    }
}
