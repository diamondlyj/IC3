﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MIX2.Data.ServiceProxies.Recognition {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceProxies.Recognition.IRecognition")]
    public interface IRecognition {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/RegisterSource", ReplyAction="http://tempuri.org/IRecognition/RegisterSourceResponse")]
        string RegisterSource(string Nickname, string Key);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/ExchangeToken", ReplyAction="http://tempuri.org/IRecognition/ExchangeTokenResponse")]
        string ExchangeToken(string OldToken, string SourceGUID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/VerifyToken", ReplyAction="http://tempuri.org/IRecognition/VerifyTokenResponse")]
        string VerifyToken(string Token, string SourceGUID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/ExchangeKey", ReplyAction="http://tempuri.org/IRecognition/ExchangeKeyResponse")]
        string ExchangeKey(string Key, string IV, string NewKey, string Token, string SourceGUID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/GetSignatureTemplate", ReplyAction="http://tempuri.org/IRecognition/GetSignatureTemplateResponse")]
        string GetSignatureTemplate(string ObjectClass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/EmulateClientDecrypt", ReplyAction="http://tempuri.org/IRecognition/EmulateClientDecryptResponse")]
        string EmulateClientDecrypt(string Key, string IV, string Text);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/EmulateClientEncrypt", ReplyAction="http://tempuri.org/IRecognition/EmulateClientEncryptResponse")]
        string EmulateClientEncrypt(string Text, string Key);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/GetEmulationKey", ReplyAction="http://tempuri.org/IRecognition/GetEmulationKeyResponse")]
        string GetEmulationKey();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRecognition/SendObject", ReplyAction="http://tempuri.org/IRecognition/SendObjectResponse")]
        string SendObject(string Object, string Token, string SourceGUID);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IRecognitionChannel : ServiceProxies.Recognition.IRecognition, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RecognitionClient : System.ServiceModel.ClientBase<ServiceProxies.Recognition.IRecognition>, ServiceProxies.Recognition.IRecognition {
        
        public RecognitionClient() {
        }
        
        public RecognitionClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RecognitionClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RecognitionClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RecognitionClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string RegisterSource(string Nickname, string Key) {
            return base.Channel.RegisterSource(Nickname, Key);
        }
        
        public string ExchangeToken(string OldToken, string SourceGUID) {
            return base.Channel.ExchangeToken(OldToken, SourceGUID);
        }
        
        public string VerifyToken(string Token, string SourceGUID) {
            return base.Channel.VerifyToken(Token, SourceGUID);
        }
        
        public string ExchangeKey(string Key, string IV, string NewKey, string Token, string SourceGUID) {
            return base.Channel.ExchangeKey(Key, IV, NewKey, Token, SourceGUID);
        }
        
        public string GetSignatureTemplate(string ObjectClass) {
            return base.Channel.GetSignatureTemplate(ObjectClass);
        }
        
        public string EmulateClientDecrypt(string Key, string IV, string Text) {
            return base.Channel.EmulateClientDecrypt(Key, IV, Text);
        }
        
        public string EmulateClientEncrypt(string Text, string Key) {
            return base.Channel.EmulateClientEncrypt(Text, Key);
        }
        
        public string GetEmulationKey() {
            return base.Channel.GetEmulationKey();
        }
        
        public string SendObject(string Object, string Token, string SourceGUID) {
            return base.Channel.SendObject(Object, Token, SourceGUID);
        }
    }
}
