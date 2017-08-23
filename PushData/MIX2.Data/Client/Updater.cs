using System;
using System.Collections.Generic;
using System.Text;

namespace MIX.Data.Client
{
    public class UpdaterException : ApplicationException
    {
        Uri WebServiceLocation;

        public UpdaterException(string Message, Exception InnerException, Uri WebServiceLocation)
            :
            base(Message, InnerException)
        {
            this.WebServiceLocation = WebServiceLocation;
        }

    }


    public delegate void DataPointUpdateSucceededDelegat(DataPoint dp);
    public delegate void DataPointUpdateFailedDelegat(DataPoint dp);
    public delegate void TimeoutUpperLimitDelegate(int Timeout);
    public delegate void TimeoutBackToNormDelegate(int Timeout);
    public delegate void NonTimeoutExceptionDelegate(DataPoint dp, Exception ex);

    public class Updater
    {
        int m_MinTimeout;
        int m_Timeout;
        int m_MaxTimeout;
        int m_Attempts;

        MIX.Data.WebService.Updater wsUpdater;
        public Updater(Uri urlWebServiceLocation, int MinTimeout /* ms */, int MaxTimeout /* ms */, int Attempts )
        {
            m_MinTimeout = MinTimeout;
            m_MaxTimeout = MaxTimeout;

            m_Timeout = m_MinTimeout;

            m_Attempts = Attempts;

            try
            {
                wsUpdater = new MIX.Data.WebService.Updater();
                wsUpdater.Url = urlWebServiceLocation.AbsoluteUri;
            }
            catch(Exception ex)
            {
                throw new UpdaterException("Updater: error when trying to instantiate the object", ex, urlWebServiceLocation);
            }
        }

        public event DataPointUpdateSucceededDelegat DataPointUpdateSucceeded;
        public event DataPointUpdateFailedDelegat DataPointUpdateFailed;
        public event TimeoutUpperLimitDelegate TimeoutUpperLimit;
        public event TimeoutBackToNormDelegate TimeoutBackToNorm;
        public event NonTimeoutExceptionDelegate NonTimeoutException;

        protected bool SetTimeout(int NewTimeout)
        {
            bool UpperLimit = false;

            if (m_MaxTimeout <= NewTimeout)
            {
                if (m_Timeout < m_MaxTimeout && this.TimeoutUpperLimit != null)
                {
                    //  only if timeout reached upperlimit from lower value
                    UpperLimit = true;
                    this.TimeoutUpperLimit(m_MaxTimeout);
                }
                    
                m_Timeout = m_MaxTimeout;
            }
            else 
            {
                if( NewTimeout <= m_MinTimeout )
                    NewTimeout = m_MinTimeout;

                if( m_Timeout >= m_MaxTimeout && this.TimeoutBackToNorm != null)
                {
                    // only when timeout decreased from the max value
                    this.TimeoutBackToNorm(NewTimeout);
                }
                m_Timeout = NewTimeout;
                
            }
            return UpperLimit;
        }

        public void TryUpdate(int Attempts, DataPoint dp)
        {
            if (Attempts > 0)
            {
                try
                {
                    wsUpdater.Timeout = m_Timeout;  // ms 
                    DateTime dtStart = DateTime.Now;
                    
                    System.Console.WriteLine("Replace this with a call to the MIX2.Recognition WebService");

                    /*
                    wsUpdater.UpdateAttribute(dp.DataSource,
                                             dp.Index.ObjectClass, dp.Index.DataClass, dp.Index.Attribute,
                                             dp.Value.Object, dp.Value.Instance, dp.Value.Value, dp.Value.Time
                                         );
                     */
                    TimeSpan ts = DateTime.Now.Subtract(dtStart);
                    SetTimeout(ts.Milliseconds);
                    this.DataPointUpdateSucceeded(dp);
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("timed out") != -1 )
                    {
                        if (SetTimeout(m_Timeout*2) )
                        {
                            TryUpdate(Attempts, dp);
                        }
                        else
                        {
                            TryUpdate(Attempts - 1, dp);
                        }
                    }
                    else
                    {
                        if (this.NonTimeoutException != null)
                            this.NonTimeoutException(dp, ex);
                        TryUpdate(Attempts - 1, dp);
                    }
                }
            }
            else
            {
                this.DataPointUpdateFailed(dp);
            }
        }


        //       public string UpdateAttribute(string DataSource, string ObjectClass, string DataClass, string Attribute, string Obj, string Instance, string Value, System.DateTime Updated) {
        public string UpdateAttribute(DataPoint dp)
        {
            string Result = "OK";
            TryUpdate(m_Attempts, dp);
            return Result;
        }

    }
}
