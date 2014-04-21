using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace OpenTrack.Utilities
{
    /// <summary>
    /// Behavior that can be added to the WCF client to getting request and response raw XML.
    /// </summary>
    internal class MessageInspectorBehavior : IEndpointBehavior
    {
        private readonly IMessageLogger _messageLogger;

        public MessageInspectorBehavior(IMessageLogger messageLogger)
        {
            this._messageLogger = messageLogger;
        }

        public string Path { get; set; }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new RawMessageInspector(this._messageLogger));
        }

        private class RawMessageInspector : IClientMessageInspector
        {
            private readonly IMessageLogger _messageLogger;

            public RawMessageInspector(IMessageLogger messageLogger)
            {
                this._messageLogger = messageLogger;
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                if (_messageLogger == null)
                {
                    return;
                }
                this._messageLogger.MessageReceived(reply.ToString());
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                if (_messageLogger == null)
                {
                    return null;
                }
                this._messageLogger.MessageSending(request.ToString());
                return null;
            }
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // Nothing to do here.
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // Nothing to do here.
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // Nothing to do here.
        }
    }
}