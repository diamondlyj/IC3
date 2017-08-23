using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Xcellence;

namespace MIX2.Action
{
    public class Process
    {
        Xcellence.IStorage storage;
        bool isTest;
        string processIdentifier;
        Mutex mutex;
        string loopScript;

        PerformanceCounter inputCounter;
        PerformanceCounter readCounter;
        PerformanceCounter readDatabaseCounter;
        PerformanceCounter readMemoryCounter;
        PerformanceCounter categorizeCounter;
        EventLog log;


        public Process(string processIdentifier, bool isTest)
        {
            Console.WriteLine("Process launched.");

            this.processIdentifier = processIdentifier;

            Storage s = new Storage(isTest);
            s.ProcessIdentifier = processIdentifier;
            this.storage = s;
            this.storage.OnRead += new ReadObjectHandler(storage_OnRead);
            this.storage.OnCategorize += new CategorizeObjectHandler(storage_OnCategorize);
            this.isTest = isTest;

            //PerformanceCounterCategory.Delete("MIX2.Action");

            if (!PerformanceCounterCategory.Exists("MIX2.Action"))
            {
                PerformanceCounterCategory.Create("MIX2.Action", "Objects processed by Xcellecne script", PerformanceCounterCategoryType.MultiInstance, "Objects read/second","Total");
                //PerformanceCounterCategory.Create("MIX2.Action", "Objects processed by Xcellecne script", PerformanceCounterCategoryType.SingleInstance, "Objects read/second", "Input");
                //PerformanceCounterCategory.Create("MIX2.Action", "Objects processed by Xcellecne script", PerformanceCounterCategoryType.SingleInstance, "Objects read/second", "Database");
                //PerformanceCounterCategory.Create("MIX2.Action", "Objects processed by Xcellecne script", PerformanceCounterCategoryType.SingleInstance, "Objects read/second", "Memory");
                //PerformanceCounterCategory.Create("MIX2.Action", "Objects processed by Xcellecne script", PerformanceCounterCategoryType.SingleInstance, "Objects categorized/second", "Total");
            }

            this.readCounter = new PerformanceCounter("MIX2.Action", "Objects read/second", "Total", false);
            this.inputCounter = new PerformanceCounter("MIX2.Action", "Objects read/second", "Input", false);
            this.readDatabaseCounter = new PerformanceCounter("MIX2.Action", "Objects read/second", "Database", false);
            this.readMemoryCounter = new PerformanceCounter("MIX2.Action", "Objects read/second", "Memory", false);
            this.categorizeCounter = new PerformanceCounter("MIX2.Action", "Objects categorized/second", "Total", false);
        }

        void storage_OnCategorize(IStorage storage, Xcellence.Object obj, string category)
        {
            this.categorizeCounter.Increment();
        }

        void storage_OnRead(IStorage storage, Xcellence.Object obj)
        {
            this.readCounter.Increment();
            this.readDatabaseCounter.Increment();
        }

        public Mutex Mutex
        {
            set { this.mutex = value; }
        }

        public string LoopScript
        {
            set { this.loopScript = value; }
        }

        public void Execute( string scriptPath )
        {
            Dictionary<string, MemoryStorage> dict = this.GetQueues(isTest);

            if (this.isTest)
                Console.WriteLine("Testing...\n\n");
            else
                Console.WriteLine("Executing...\n\n");


            Xcellence.Environment.Input = new Input(dict, isTest);
            Xcellence.Environment.Input.OnRead += new ReadInputHandler(Input_OnRead);

            Xcellence.Environment.GlobalStore = storage;

            Xcellence.Environment.Intialize();

            System.IO.FileStream stream = new System.IO.FileStream(scriptPath, System.IO.FileMode.Open);
            Xcellence.Scripting.Lexer lexer = new Xcellence.Scripting.Lexer(stream.Name, stream);
            Xcellence.Scripting.Parser parser = new Xcellence.Scripting.Parser(lexer);
            parser.Parse();
            parser.Close();

            parser.Execute(true);

        }

        void Input_OnRead(IInput input, object obj)
        {
            //this.readCounter.Increment();
        }

        public void LoopExecute()
        {
        Execute:
            this.Execute(this.loopScript);

            if ( !this.isTest)
                goto Execute;
        }

        public void Run()
        {
            string source = "MIX2.Action";

            try
            {
                System.Diagnostics.EventLog.WriteEntry(source, "Process " + this.processIdentifier + " started.", EventLogEntryType.Information);

                Dictionary<string, MemoryStorage> dict = this.GetQueues(isTest);

                DirectoryInfo dinfo = new DirectoryInfo("Scripts");

                FileInfo[] files = dinfo.GetFiles();

                //List<Thread> threads = new List<Thread>();
                List<Xcellence.Scripting.Parser> parsers = new List<Xcellence.Scripting.Parser>();
                List<Executor> executors = new List<Executor>();

                for (int i = 0; i < files.Length; i++)
                {
                    string name = files[i].Name;
                    int index = name.LastIndexOf('.');

                    string ext = string.Empty;

                    if (index > -1)
                    {
                        ext = name.Substring(index);
                    }

                    this.mutex.WaitOne();

                    System.IO.FileStream stream = new System.IO.FileStream(files[i].FullName, System.IO.FileMode.Open, FileAccess.Read);
                    Xcellence.Scripting.Lexer lexer = new Xcellence.Scripting.Lexer(stream.Name, stream);
                    Xcellence.Scripting.Parser parser = new Xcellence.Scripting.Parser(lexer);

                    parser.Parse();
                    parser.Close();

                    stream.Close();

                    this.mutex.ReleaseMutex();

                    parsers.Add(parser);

                    Executor exec = new Executor(this.processIdentifier + "." + i.ToString() +  " [" + files[i].Name + "]", parser.Sequences, parser.MemoryStores);
                    //exec.Storage = storage;
                    executors.Add(exec);
                }

                //Console.WriteLine();

                WaitHandle[] waitHandles = new WaitHandle[parsers.Count];

            ExecuteThreads:
                int n = 0;
                Thread[] threads = new Thread[parsers.Count];                

                foreach (Executor exec in executors)
                {
                    ManualResetEvent mevent = new ManualResetEvent(false);                    

                    exec.Input = new Input(dict, isTest);
                    exec.Storage = storage;
                    exec.ExecutedEvent = mevent;

                    waitHandles[n] = mevent;

                    Thread thread = new Thread(exec.Execute);
                    threads[n] = thread;

                    n++;
                }

                for (int i = 0; i < threads.Length; i++)
                {
                    try
                    {
                        System.Diagnostics.EventLog.WriteEntry(source, "Starting thread " + this.processIdentifier + "." + i, EventLogEntryType.Information);
                        threads[i].Start();
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.EventLog.WriteEntry(source, "Failed to start thread " + this.processIdentifier + "." + i.ToString() + ": " + exc.Message, EventLogEntryType.Error);
                    }
                }

                WaitHandle.WaitAll(waitHandles);

                //Console.WriteLine("\nScripts executed.\n");
                System.Diagnostics.EventLog.WriteEntry(source, this.processIdentifier + " finished executing scripts.", EventLogEntryType.Information);

                //Clean everything up to avoid growing memory;
                foreach (MemoryStorage storage in dict.Values)
                {
                    storage.Clear();                    
                }

                foreach (Executor exec in executors)
                {
                    exec.Input = null;
                    exec.Storage = null;
                }

                dict.Clear();
                dict = null;

                GC.Collect();
                GC.WaitForFullGCApproach();
                GC.WaitForFullGCComplete();

                //Get new action queues
                dict = this.GetQueues(isTest);

                string queueName = System.Configuration.ConfigurationManager.AppSettings["QueueName"];

                if (dict.ContainsKey(queueName) && dict[queueName].Count > 0)
                {
                    goto StopThreads;
                }

                TimeSpan sleepTime = System.Xml.XmlConvert.ToTimeSpan(System.Configuration.ConfigurationManager.AppSettings["ReadFrequency"]);

                Console.WriteLine(string.Format("\n{0} queue is empty. Going to sleep for {1} seconds.\n", queueName, sleepTime.TotalSeconds));

                Thread.Sleep(Convert.ToInt32(sleepTime.TotalMilliseconds));

            StopThreads:
                //This seems unnecessary but just in case the garbage collector needs it....
                for (int i = 0; i < threads.Length; i++)
                {
                    threads[i].Abort();
                }

                goto ExecuteThreads;
            }
            catch( Exception exc )
            {
                System.Diagnostics.EventLog.WriteEntry(source, this.processIdentifier + " failed: " + exc.Message, EventLogEntryType.Error);
            }
        }

        private Dictionary<string, MemoryStorage> GetQueues(bool isTest)
        {
            Console.WriteLine("Reading input.");

            Dictionary<string, MemoryStorage> dict = new Dictionary<string, MemoryStorage>();

            string queueName = System.Configuration.ConfigurationManager.AppSettings["QueueName"];
            MemoryStorage memStorage = new MemoryStorage();
            
            memStorage.OnRead += new ReadObjectHandler(memStorage_OnRead);
            memStorage.OnCategorize += new CategorizeObjectHandler(memStorage_OnCategorize);
            //make log variable instance wide
            string source = "MIX2.Action";
            string log = "MIX2";

            /*
            if (!System.Diagnostics.EventLog.Exists(log))
            {
                Console.WriteLine("Creating log");
                System.Diagnostics.EventLog.CreateEventSource(source, log);
            }
             * */

            //if (this.mutex != null)
            //    this.mutex.WaitOne();

            List<object> predicate = new List<object>();
            predicate.Add(queueName);

            //List<Xcellence.Object> result = new List<Xcellence.Object>();

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);
            SqlConnection conn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);
            SqlConnection cubeConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Cube"].ConnectionString);

            //System.Diagnostics.EventLog.WriteEntry(source, "Process " + this.processIdentifier + " reading " + queueName + " queue...", EventLogEntryType.Information);

            try
            {
                SqlCommand cubeCommand = new SqlCommand
                    (
                    
                    " merge [PropertyValue]"
                    + " using ( select @Value val, @PropertyClass prop, @ObjectGUID guid, @ObjectClass class, @FriendlyName friend, @State state, @ParentGUID pguid, @Domain dom) T"
                    + " on [Value] = T.val"
                    + "   and [PropertyClass] = T.prop"
                    + "   and [ObjectGUID] = T.guid"
                    + "   and [ObjectClass] = T.class"
                    + "   and [Domain] = T.dom"
                    + " when matched then"
                    + "   update set [Confirmed] = getutcdate(),"
                    + "      [FriendlyName] = T.friend,"
                    + "      [State] = T.state"
                    + " when not matched then"
                    + "   insert"
                    + "     ([Value], [PropertyClass], [ObjectGUID], [ObjectClass], [FriendlyName], [State], [ParentGUID], [Domain])"
                    + "     values (val, prop, guid, class, friend, state, pguid, dom);"

                    + "   update [CategoryEntry] set [FriendlyName] = @FriendlyName, [State] = @State"
                    + "     where [ObjectGUID] = @ObjectGUID"
                    + "       and [ObjectClass] = @ObjectClass"
                    + "       and [Domain] = @Domain;"

                    /*
                    " update [PropertyValue] set [Confirmed] = getutcdate()"
                    + " where [Value] = @Value"
                    + "   and [PropertyClass] = @PropertyClass"
                    + "   and [ObjectGUID] = @ObjectGUID"
                    + "   and [ObjectClass] = @ObjectClass"
                    + "   and [Domain] = @Domain;"

                    + " if @@rowcount = 0"
                    + " begin"

                    + "   insert into [PropertyValue]"
                    + "     ([Value], [PropertyClass], [ObjectGUID], [ObjectClass], [ParentGUID], [Domain])"
                    + "     values (@Value, @PropertyClass, @ObjectGUID, @ObjectClass, @ParentGUID, @Domain);"

                    + "   update [PropertyValue] set [FriendlyName] = @FriendlyName"
                    + "     where [ObjectGUID] = @ObjectGUID"
                    + "       and [ObjectClass] = @ObjectClass"
                    + "       and [Domain] = @Domain;"

                    + "   update [CategoryEntry] set [FriendlyName] = @FriendlyName"
                    + "     where [ObjectGUID] = @ObjectGUID"
                    + "       and [ObjectClass] = @ObjectClass"
                    + "       and [Domain] = @Domain;"
                    + " end "
                    */

                        , cubeConn
                    );
                SqlParameter cValParam = new SqlParameter("@Value", System.Data.SqlDbType.NVarChar, 1024);
                SqlParameter cPropParam = new SqlParameter("@PropertyClass", System.Data.SqlDbType.NVarChar, 64);
                SqlParameter cGuidParam = new SqlParameter("@ObjectGUID", System.Data.SqlDbType.UniqueIdentifier);
                SqlParameter cClassParam = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);
                SqlParameter cNameParam = new SqlParameter("@FriendlyName", System.Data.SqlDbType.NVarChar, 256);
                SqlParameter cStateParam = new SqlParameter("@State", System.Data.SqlDbType.Float);
                SqlParameter pGuidParam = new SqlParameter("@ParentGUID", System.Data.SqlDbType.UniqueIdentifier);
                SqlParameter cDomainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                cDomainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                cubeCommand.Parameters.Add(cValParam);
                cubeCommand.Parameters.Add(cPropParam);
                cubeCommand.Parameters.Add(cGuidParam);
                cubeCommand.Parameters.Add(cClassParam);
                cubeCommand.Parameters.Add(cNameParam);
                cubeCommand.Parameters.Add(cStateParam);
                cubeCommand.Parameters.Add(pGuidParam);
                cubeCommand.Parameters.Add(cDomainParam);

                conn.Open();
                conn2.Open();
                cubeConn.Open();

                SqlParameter param = new SqlParameter("@Action", System.Data.SqlDbType.NVarChar, 64);

                int readCount = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReadCount"]);

                if (predicate != null)
                {
                    foreach (object s in predicate)
                    {
                        if (s is string)
                        {
                            string str = (string)s;
                            param.Value = str;

                            string query = "declare @aguid uniqueidentifier; set @aguid = (select [GUID] from [Action] where [Name] = @Action);";

                            if (!isTest)
                            {
                                query += "with AQ ( ObjectClassID, ObjectGUID, ActionGUID )"
                                    + " as ( select ";


                                if (readCount != -1)
                                    query += " top (" + readCount.ToString() + ")";

                                query += " ObjectClassID, ObjectGUID, ActionGUID from [ActionQueue]"
                                    + "         where [ActionGUID] = @aguid order by [ActionQueue].[Created] )"
                                    + " delete "
                                    + "     from AQ output";
                            }
                            else
                            {
                                query += "select";

                                if (readCount != -1)
                                    query += " top (" + readCount.ToString() + ")";

                            }

                            query += " [ObjectClass].[Name], [ObjectClass].[ID]"
                                + ", [Object].[GUID]"
                                + ", [Object].[FriendlyName]"
                                + ", [Object].[Created]"
                                + ", [Object].[Confirmed]"
                                + ", [Object].[State]"
                                + ", [Object].[ParentClassID]"
                                + ", [Object].[ParentGUID]"
                                + " from [Object], [ObjectClass]";

                            string tname = string.Empty;

                            if (isTest)
                            {
                                tname = "[ActionQueue]";
                                query += ", [ActionQueue]";
                            }
                            else
                                tname = "AQ";

                            query += " where [Object].[ObjectClassID] = [ObjectClass].[ID]"
                                + "     and " + tname + ".[ObjectClassID] = [Object].[ObjectClassID]"
                                + "     and " + tname + ".[ObjectGUID] = [Object].[GUID]";

                            if (isTest)
                            {
                                query += "     and [ActionQueue].[ActionGUID] = @aguid";

                            }


                            //Console.WriteLine(query);

                            using (SqlCommand command = new SqlCommand(query, conn))
                            {
                                command.Parameters.Add(param);

                                SqlDataReader reader = command.ExecuteReader();

                                while (reader.Read())
                                {
                                    Guid guid = reader.GetGuid(2);
                                    int ocid = reader.GetInt32(1);

                                    string objClass = reader.GetString(0);

                                    Xcellence.Object obj = new Xcellence.Object(guid, memStorage);
                                    obj.Class.Add(objClass);

                                    string fname = null;
                                    double state = 1;
                                    int pocid = -1;
                                    Guid pguid = Guid.Empty;

                                    Xcellence.Property confirmedProp = new Xcellence.Property(obj, "Confirmed");
                                    confirmedProp.Values.Add(new Xcellence.Value(confirmedProp, reader.GetDateTime(4)));
                                    obj.Meta.Property.Add(confirmedProp);

                                    Xcellence.Property createdProp = new Xcellence.Property(obj, "Created");
                                    createdProp.Values.Add(new Xcellence.Value(createdProp, reader.GetDateTime(5)));
                                    obj.Meta.Property.Add(createdProp);

                                    Xcellence.Property stateProp = new Xcellence.Property(obj, "State");
                                    stateProp.Values.Add(new Xcellence.Value(stateProp, reader.GetDouble(6)));
                                    obj.Meta.Property.Add(stateProp);

                                    state = reader.GetDouble(6);

                                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!=====>" + state);

                                    if (!reader.IsDBNull(3))
                                        fname = reader.GetString(3);

                                    if (!reader.IsDBNull(7))
                                        pocid = reader.GetInt32(7);

                                    if (!reader.IsDBNull(8))
                                        pguid = reader.GetGuid(8);

                                    if (fname != null)
                                    {
                                        Xcellence.Property fprop = new Xcellence.Property(obj, "FriendlyName");
                                        fprop.Values.Add(new Xcellence.Value(fprop, fname));
                                        obj.Meta.Property.Add(fprop);
                                    }

                                    SqlParameter guidParam = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                                    guidParam.Value = guid;


                                    SqlParameter ocidParam = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                                    ocidParam.Value = ocid;

                                    using (SqlCommand propCommand = new SqlCommand("select [PropertyClass].[Name], [Property].[Value] from [Property], [PropertyClass] where [Property].[ObjectClassID] = @ocid and [Property].[ObjectGUID] = @guid and [PropertyClass].[ID] = [Property].[PropertyClassID] order by [PropertyClass].[Name];", conn2))
                                    {
                                        propCommand.Parameters.Add(guidParam);
                                        propCommand.Parameters.Add(ocidParam);

                                        SqlDataReader propReader = propCommand.ExecuteReader();

                                        string propName = string.Empty;
                                        Xcellence.Property prop = new Xcellence.Property(obj, "");

                                        while (propReader.Read())
                                        {
                                            string nextName = propReader.GetString(0);

                                            if (nextName != propName)
                                            {
                                                if (propName != string.Empty)
                                                    obj.Signature.Property.Add(prop);

                                                propName = nextName;
                                                prop = new Xcellence.Property(obj, propName);
                                            }

                                            string val = propReader.GetString(1);

                                            prop.AddValue(val);

                                            if (!isTest)
                                            //&& pocid == -1 && pguid == Guid.Empty)
                                            {
                                                cValParam.Value = val;
                                                cPropParam.Value = propName;
                                                cGuidParam.Value = guid;
                                                cClassParam.Value = objClass;
                                                cNameParam.Value = fname;
                                                cStateParam.Value = state;

                                                if (pguid != Guid.Empty)
                                                    pGuidParam.Value = pguid;
                                                else
                                                    pGuidParam.Value = DBNull.Value;


                                                try
                                                {

                                                    cubeCommand.ExecuteNonQuery();
                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! Failed to EXECUTE CUBE COMMAND");
                                                    Console.WriteLine(exc.Message);
                                                    System.Diagnostics.EventLog.WriteEntry(source, "Process " + this.processIdentifier + " failed to update cube:" + exc.Message, EventLogEntryType.Error);
                                                }

                                                Console.WriteLine("Adding property of " + fname + "{" + guid + "} " + propName + ": " + val);
                                            }
                                        }

                                        if (propName != string.Empty)
                                            obj.Signature.Property.Add(prop);

                                        propReader.Close();
                                    }

                                    //Add kids
                                    string relSql = "select [ObjectClass].[Name], [Object].[GUID]"
                                        + " from [Object], [ObjectClass]"
                                        + " where [Object].[ParentClassID] = @ocid"
                                            + " and [Object].[ParentGUID] = @guid"
                                            + " and [ObjectClass].[ID] = [Object].[ObjectClassID]"
                                        + " order by [ObjectClass].[Name];";

                                    AddRelations(ref obj, ocid, guid, relSql, conn2);

                                    //Add parent
                                    if (pocid != -1 && pguid != Guid.Empty)
                                    {
                                        relSql = "select [ObjectClass].[Name], [Object].[GUID]"
                                        + " from [Object], [ObjectClass]"
                                        + " where [Object].[ObjectClassID] = @ocid"
                                            + " and [Object].[GUID] = @guid"
                                            + " and [ObjectClass].[ID] = [Object].[ObjectClassID]"
                                        + " order by [ObjectClass].[Name];";

                                        AddRelations(ref obj, pocid, pguid, relSql, conn2);
                                    }

                                    memStorage.AddObject(obj);

                                    this.inputCounter.Increment();
                                }

                                reader.Close();
                            }
                        }
                    }
                }

                dict.Add(queueName, memStorage);

                System.Diagnostics.EventLog.WriteEntry(source, "Process " + this.processIdentifier + " finished reading " + queueName + " queue (" + memStorage.Count + " objects).", EventLogEntryType.Information);

            }
            catch (Exception Exception)
            {
                System.Diagnostics.EventLog.WriteEntry(source, "Process " + this.processIdentifier + " failed to get queues:" + Exception.Message, EventLogEntryType.Error);

            }

            if (conn.State == System.Data.ConnectionState.Open)
                conn.Close();

            if (conn2.State == System.Data.ConnectionState.Open)
                conn2.Close();

            if (cubeConn.State == System.Data.ConnectionState.Open)
                cubeConn.Close();


            //if (this.mutex != null)
            //    this.mutex.ReleaseMutex();

            return dict;
        }

        void memStorage_OnCategorize(IStorage storage, Xcellence.Object obj, string category)
        {
            this.categorizeCounter.Increment();
        }

        void memStorage_OnRead(IStorage storage, Xcellence.Object obj)
        {
            this.readCounter.Increment();
            this.readMemoryCounter.Increment();
        }

        private void AddRelations(ref Xcellence.Object obj, int objectClassID, Guid guid, string sql, SqlConnection conn)
        {
            using (SqlCommand relCommand = new SqlCommand(sql, conn))
            {
                SqlParameter guidParam2 = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                guidParam2.Value = guid;

                SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                ocidParam2.Value = objectClassID;

                relCommand.Parameters.Add(guidParam2);
                relCommand.Parameters.Add(ocidParam2);

                SqlDataReader relReader = relCommand.ExecuteReader();

                string relName = string.Empty;
                Xcellence.Relation rel = new Xcellence.Relation(obj);
                Xcellence.Link link = new Xcellence.Link(rel);

                while (relReader.Read())
                {
                    string nextName = relReader.GetString(0);

                    if (nextName != relName)
                    {
                        if (relName != string.Empty)
                        {
                            rel.Link.Add(link);
                            obj.Network.Add(rel);
                        }

                        relName = nextName;
                        rel = new Xcellence.Relation(obj);
                        rel.Name.Add(relName);
                        link = new Xcellence.Link(rel);
                    }

                    Xcellence.Reference r = new Xcellence.Reference(link);
                    r.GUID = relReader.GetGuid(1);

                    if (relName != string.Empty)
                        r.RelationType.Add(relName);

                    link.Reference.Add(r);
                }

                if (relName != string.Empty)
                {
                    rel.Link.Add(link);
                    obj.Network.Add(rel);
                }

                relReader.Close();
            }
        }
    }
}
