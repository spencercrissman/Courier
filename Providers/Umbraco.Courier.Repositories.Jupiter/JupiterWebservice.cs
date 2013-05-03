using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Umbraco.Courier.Core;

namespace Umbraco.Courier.Repositories.Jupiter
{
    // <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "CourierRepositoryASMXSoap", Namespace = "http://courier.umbraco.org/")]
    public partial class JupiterWebservice : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback OpenSessionOperationCompleted;
        private System.Threading.SendOrPostCallback CloseSessionOperationCompleted;
        private System.Threading.SendOrPostCallback CommitOperationCompleted;
        private System.Threading.SendOrPostCallback RollbackOperationCompleted;
        private System.Threading.SendOrPostCallback ExtractOperationCompleted;
        private System.Threading.SendOrPostCallback TransferResourceOperationCompleted;


        /// <remarks/>
        public JupiterWebservice(string url)
        {
            this.Url = url;
        }

        /// <remarks/>
        public event OpenSessionCompletedEventHandler OpenSessionCompleted;

        /// <remarks/>
        public event CloseSessionCompletedEventHandler CloseSessionCompleted;

        /// <remarks/>
        public event CommitCompletedEventHandler CommitCompleted;

        /// <remarks/>
        public event RollbackCompletedEventHandler RollbackCompleted;

        /// <remarks/>
        public event ExtractCompletedEventHandler ExtractCompleted;

        /// <remarks/>
        public event TransferResourceCompletedEventHandler TransferResourceCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://courier.umbraco.org/OpenSession", RequestNamespace = "http://courier.umbraco.org/", ResponseNamespace = "http://courier.umbraco.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void OpenSession(string sessionKey, string username, string password)
        {
            this.Invoke("OpenSession", new object[] {
                    sessionKey,
                    username,
                    password});
        }

        /// <remarks/>
        public System.IAsyncResult BeginOpenSession(string sessionKey, string username, string password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("OpenSession", new object[] {
                    sessionKey,
                    username,
                    password}, callback, asyncState);
        }

        /// <remarks/>
        public void EndOpenSession(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void OpenSessionAsync(string sessionKey, string username, string password)
        {
            this.OpenSessionAsync(sessionKey, username, password, null);
        }

        /// <remarks/>
        public void OpenSessionAsync(string sessionKey, string username, string password, object userState)
        {
            if ((this.OpenSessionOperationCompleted == null))
            {
                this.OpenSessionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnOpenSessionOperationCompleted);
            }
            this.InvokeAsync("OpenSession", new object[] {
                    sessionKey,
                    username,
                    password}, this.OpenSessionOperationCompleted, userState);
        }

        private void OnOpenSessionOperationCompleted(object arg)
        {
            if ((this.OpenSessionCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.OpenSessionCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://courier.umbraco.org/CloseSession", RequestNamespace = "http://courier.umbraco.org/", ResponseNamespace = "http://courier.umbraco.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void CloseSession(string sessionKey, string username, string password)
        {
            this.Invoke("CloseSession", new object[] {
                    sessionKey,
                    username,
                    password});
        }

        /// <remarks/>
        public System.IAsyncResult BeginCloseSession(string sessionKey, string username, string password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CloseSession", new object[] {
                    sessionKey,
                    username,
                    password}, callback, asyncState);
        }

        /// <remarks/>
        public void EndCloseSession(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void CloseSessionAsync(string sessionKey, string username, string password)
        {
            this.CloseSessionAsync(sessionKey, username, password, null);
        }

        /// <remarks/>
        public void CloseSessionAsync(string sessionKey, string username, string password, object userState)
        {
            if ((this.CloseSessionOperationCompleted == null))
            {
                this.CloseSessionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCloseSessionOperationCompleted);
            }
            this.InvokeAsync("CloseSession", new object[] {
                    sessionKey,
                    username,
                    password}, this.CloseSessionOperationCompleted, userState);
        }

        private void OnCloseSessionOperationCompleted(object arg)
        {
            if ((this.CloseSessionCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CloseSessionCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://courier.umbraco.org/Commit", RequestNamespace = "http://courier.umbraco.org/", ResponseNamespace = "http://courier.umbraco.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Commit(string sessionKey, string username, string password)
        {
            this.Invoke("Commit", new object[] {
                    sessionKey,
                    username,
                    password});
        }

        /// <remarks/>
        public System.IAsyncResult BeginCommit(string sessionKey, string username, string password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Commit", new object[] {
                    sessionKey,
                    username,
                    password}, callback, asyncState);
        }

        /// <remarks/>
        public void EndCommit(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void CommitAsync(string sessionKey, string username, string password)
        {
            this.CommitAsync(sessionKey, username, password, null);
        }

        /// <remarks/>
        public void CommitAsync(string sessionKey, string username, string password, object userState)
        {
            if ((this.CommitOperationCompleted == null))
            {
                this.CommitOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCommitOperationCompleted);
            }
            this.InvokeAsync("Commit", new object[] {
                    sessionKey,
                    username,
                    password}, this.CommitOperationCompleted, userState);
        }

        private void OnCommitOperationCompleted(object arg)
        {
            if ((this.CommitCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CommitCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://courier.umbraco.org/Rollback", RequestNamespace = "http://courier.umbraco.org/", ResponseNamespace = "http://courier.umbraco.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Rollback(string sessionKey, string username, string password)
        {
            this.Invoke("Rollback", new object[] {
                    sessionKey,
                    username,
                    password});
        }

        /// <remarks/>
        public System.IAsyncResult BeginRollback(string sessionKey, string username, string password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Rollback", new object[] {
                    sessionKey,
                    username,
                    password}, callback, asyncState);
        }

        /// <remarks/>
        public void EndRollback(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void RollbackAsync(string sessionKey, string username, string password)
        {
            this.RollbackAsync(sessionKey, username, password, null);
        }

        /// <remarks/>
        public void RollbackAsync(string sessionKey, string username, string password, object userState)
        {
            if ((this.RollbackOperationCompleted == null))
            {
                this.RollbackOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRollbackOperationCompleted);
            }
            this.InvokeAsync("Rollback", new object[] {
                    sessionKey,
                    username,
                    password}, this.RollbackOperationCompleted, userState);
        }

        private void OnRollbackOperationCompleted(object arg)
        {
            if ((this.RollbackCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RollbackCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://courier.umbraco.org/Extract", RequestNamespace = "http://courier.umbraco.org/", ResponseNamespace = "http://courier.umbraco.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public RepositoryActionResponse Extract(string sessionKey, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] item, ItemIdentifier itemId, bool overwrite, string user, string pass)
        {
            object[] results = this.Invoke("Extract", new object[] {
                    sessionKey,
                    item,
                    itemId,
                    overwrite,
                    user,
                    pass});
            return ((RepositoryActionResponse)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginExtract(string sessionKey, byte[] item, ItemIdentifier itemId, bool overwrite, string user, string pass, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Extract", new object[] {
                    sessionKey,
                    item,
                    itemId,
                    overwrite,
                    user,
                    pass}, callback, asyncState);
        }

        /// <remarks/>
        public RepositoryActionResponse EndExtract(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((RepositoryActionResponse)(results[0]));
        }

        /// <remarks/>
        public void ExtractAsync(string sessionKey, byte[] item, ItemIdentifier itemId, bool overwrite, string user, string pass)
        {
            this.ExtractAsync(sessionKey, item, itemId, overwrite, user, pass, null);
        }

        /// <remarks/>
        public void ExtractAsync(string sessionKey, byte[] item, ItemIdentifier itemId, bool overwrite, string user, string pass, object userState)
        {
            if ((this.ExtractOperationCompleted == null))
            {
                this.ExtractOperationCompleted = new System.Threading.SendOrPostCallback(this.OnExtractOperationCompleted);
            }
            this.InvokeAsync("Extract", new object[] {
                    sessionKey,
                    item,
                    itemId,
                    overwrite,
                    user,
                    pass}, this.ExtractOperationCompleted, userState);
        }

        private void OnExtractOperationCompleted(object arg)
        {
            if ((this.ExtractCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ExtractCompleted(this, new ExtractCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://courier.umbraco.org/TransferResource", RequestNamespace = "http://courier.umbraco.org/", ResponseNamespace = "http://courier.umbraco.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool TransferResource(string sessionKey, ItemIdentifier itemId, string type, Resource resource, bool overwrite, string user, string pass)
        {
            object[] results = this.Invoke("TransferResource", new object[] {
                    sessionKey,
                    itemId,
                    type,
                    resource,
                    overwrite,
                    user,
                    pass});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginTransferResource(string sessionKey, ItemIdentifier itemId, string type, Resource resource, bool overwrite, string user, string pass, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TransferResource", new object[] {
                    sessionKey,
                    itemId,
                    type,
                    resource,
                    overwrite,
                    user,
                    pass}, callback, asyncState);
        }

        /// <remarks/>
        public bool EndTransferResource(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void TransferResourceAsync(string sessionKey, ItemIdentifier itemId, string type, Resource resource, bool overwrite, string user, string pass)
        {
            this.TransferResourceAsync(sessionKey, itemId, type, resource, overwrite, user, pass, null);
        }

        /// <remarks/>
        public void TransferResourceAsync(string sessionKey, ItemIdentifier itemId, string type, Resource resource, bool overwrite, string user, string pass, object userState)
        {
            if ((this.TransferResourceOperationCompleted == null))
            {
                this.TransferResourceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTransferResourceOperationCompleted);
            }
            this.InvokeAsync("TransferResource", new object[] {
                    sessionKey,
                    itemId,
                    type,
                    resource,
                    overwrite,
                    user,
                    pass}, this.TransferResourceOperationCompleted, userState);
        }

        private void OnTransferResourceOperationCompleted(object arg)
        {
            if ((this.TransferResourceCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TransferResourceCompleted(this, new TransferResourceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void OpenSessionCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void CloseSessionCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void CommitCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void RollbackCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void ExtractCompletedEventHandler(object sender, ExtractCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExtractCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ExtractCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public RepositoryActionResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((RepositoryActionResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void TransferResourceCompletedEventHandler(object sender, TransferResourceCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TransferResourceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal TransferResourceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
}