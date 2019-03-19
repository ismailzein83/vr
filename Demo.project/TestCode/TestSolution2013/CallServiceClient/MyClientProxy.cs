﻿namespace CallServiceClient.MyClientProxy
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2558.0")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "WebService1Soap", Namespace = "http://tempuri.org/")]
    public partial class WebService1 : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback HelloWorldOperationCompleted;

        private System.Threading.SendOrPostCallback TestMethod2OperationCompleted;

        private System.Threading.SendOrPostCallback TestMethodOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        public WebService1()
        {
            this.Url = global::CallServiceClient.Properties.Settings.Default.CallServiceClient_WebService1Soap_WebService1;
            if ((this.IsLocalFileSystemWebService(this.Url) == true))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                            && (this.useDefaultCredentialsSetExplicitly == false))
                            && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        /// <remarks/>
        public event HelloWorldCompletedEventHandler HelloWorldCompleted;

        /// <remarks/>
        public event TestMethod2CompletedEventHandler TestMethod2Completed;

        /// <remarks/>
        public event TestMethodCompletedEventHandler TestMethodCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/HelloWorld", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string HelloWorld()
        {
            object[] results = this.Invoke("HelloWorld", new object[0]);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void HelloWorldAsync()
        {
            this.HelloWorldAsync(null);
        }

        /// <remarks/>
        public void HelloWorldAsync(object userState)
        {
            if ((this.HelloWorldOperationCompleted == null))
            {
                this.HelloWorldOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHelloWorldOperationCompleted);
            }
            this.InvokeAsync("HelloWorld", new object[0], this.HelloWorldOperationCompleted, userState);
        }

        private void OnHelloWorldOperationCompleted(object arg)
        {
            if ((this.HelloWorldCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HelloWorldCompleted(this, new HelloWorldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/TestMethod2", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public MyClientProxy2.TestMethodOutput2 TestMethod2(TestMethodInput input, TestMethodInput input2)
        {
            object[] results = this.Invoke("TestMethod2", new object[] {
                        input,
                        input2});
            return ((MyClientProxy2.TestMethodOutput2)(results[0]));
        }

        /// <remarks/>
        public void TestMethod2Async(TestMethodInput input, TestMethodInput input2)
        {
            this.TestMethod2Async(input, input2, null);
        }

        /// <remarks/>
        public void TestMethod2Async(TestMethodInput input, TestMethodInput input2, object userState)
        {
            if ((this.TestMethod2OperationCompleted == null))
            {
                this.TestMethod2OperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestMethod2OperationCompleted);
            }
            this.InvokeAsync("TestMethod2", new object[] {
                        input,
                        input2}, this.TestMethod2OperationCompleted, userState);
        }

        private void OnTestMethod2OperationCompleted(object arg)
        {
            if ((this.TestMethod2Completed != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestMethod2Completed(this, new TestMethod2CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/TestMethod", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public MyTestMethodOutput TestMethod(MyTestMethodInput input1)
        {
            object[] results = this.Invoke("TestMethod", new object[] {
                        input1});
            return ((MyTestMethodOutput)(results[0]));
        }

        /// <remarks/>
        public void TestMethodAsync(MyTestMethodInput input1)
        {
            this.TestMethodAsync(input1, null);
        }

        /// <remarks/>
        public void TestMethodAsync(MyTestMethodInput input1, object userState)
        {
            if ((this.TestMethodOperationCompleted == null))
            {
                this.TestMethodOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestMethodOperationCompleted);
            }
            this.InvokeAsync("TestMethod", new object[] {
                        input1}, this.TestMethodOperationCompleted, userState);
        }

        private void OnTestMethodOperationCompleted(object arg)
        {
            if ((this.TestMethodCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestMethodCompleted(this, new TestMethodCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (((url == null)
                        || (url == string.Empty)))
            {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024)
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class TestMethodInput
    {

        private string textField;

        /// <remarks/>
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class MyTestMethodOutput
    {

        private string text1Field;

        private string text2Field;

        private string[] listField;

        private MyTestMethodOutput[] listOfObjectsField;

        /// <remarks/>
        public string Text1
        {
            get
            {
                return this.text1Field;
            }
            set
            {
                this.text1Field = value;
            }
        }

        /// <remarks/>
        public string Text2
        {
            get
            {
                return this.text2Field;
            }
            set
            {
                this.text2Field = value;
            }
        }

        /// <remarks/>
        public string[] List
        {
            get
            {
                return this.listField;
            }
            set
            {
                this.listField = value;
            }
        }

        /// <remarks/>
        public MyTestMethodOutput[] ListOfObjects
        {
            get
            {
                return this.listOfObjectsField;
            }
            set
            {
                this.listOfObjectsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class MyTestMethodInput
    {

        private string textField;

        /// <remarks/>
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2558.0")]
    public delegate void HelloWorldCompletedEventHandler(object sender, HelloWorldCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2558.0")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HelloWorldCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal HelloWorldCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2558.0")]
    public delegate void TestMethod2CompletedEventHandler(object sender, TestMethod2CompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2558.0")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TestMethod2CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal TestMethod2CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public WebService1Soap.TestMethodOutput Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((WebService1Soap.TestMethodOutput)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2558.0")]
    public delegate void TestMethodCompletedEventHandler(object sender, TestMethodCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2558.0")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TestMethodCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal TestMethodCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public MyTestMethodOutput Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((MyTestMethodOutput)(this.results[0]));
            }
        }
    }
}

namespace MyClientProxy2
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class TestMethodOutput
    {

        private string text1Field;

        private string text2Field;

        /// <remarks/>
        public string Text1
        {
            get
            {
                return this.text1Field;
            }
            set
            {
                this.text1Field = value;
            }
        }

        /// <remarks/>
        public string Text2
        {
            get
            {
                return this.text2Field;
            }
            set
            {
                this.text2Field = value;
            }
        }
    }


    public partial class TestMethodOutput2
    {

        private string text1Field;

        private string text2Field;

        /// <remarks/>
        public string Text1
        {
            get
            {
                return this.text1Field;
            }
            set
            {
                this.text1Field = value;
            }
        }

        /// <remarks/>
        public string Text2
        {
            get
            {
                return this.text2Field;
            }
            set
            {
                this.text2Field = value;
            }
        }
    }

}