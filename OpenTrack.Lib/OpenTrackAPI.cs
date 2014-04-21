using System.Text;
using OpenTrack.Requests;
using OpenTrack.Responses;
using OpenTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace OpenTrack
{
    // Staging URLs
    // https://otstaging.arkona.com/OpenTrack/WebService.asmx
    // https://otstaging.arkona.com/opentrack/ServiceAPI.asmx
    // https://otstaging.arkona.com/OpenTrack/PartsAPI.asmx

    // Pre-Prod URLs
    // https://otcert.arkona.com/OpenTrack/WebService.asmx
    // https://otcert.arkona.com/OpenTrack/ServiceAPI.asmx
    // https://otcert.arkona.com/OpenTrack/PartsAPI.asmx

    // Production URLs
    // https://ot.dms.dealertrack.com/WebService.asmx
    // https://ot.dms.dealertrack.com/ServiceAPI.asmx
    // https://ot.dms.dealertrack.com/PartsAPI.asmx

    /// <summary>
    /// Basic implementation of the OpenTrack API interface that performs and processes the requests.
    /// </summary>
    public class OpenTrackAPI : IOpenTrackAPI
    {
        private readonly IMessageLogger _messageLogger;

        /// <summary>
        /// The Base Url of the web service end points, i.e. https://ot.dms.dealertrack.com
        /// </summary>
        public String BaseUrl { get; private set; }

        /// <summary>
        /// The username to authenticate with the web services
        /// </summary>
        public String Username { get; private set; }

        /// <summary>
        /// The password to authenticate with the web services
        /// </summary>
        public String Password { get; private set; }

        /// <summary>
        /// The amount of time to wait before timing out the request
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Flag to true if you want to spit out all request and response XML's
        /// </summary>
        public Boolean DebugMode { get; set; }

        /// <summary>
        /// The file path to write the xml files to.  If left null or white space the files will write
        /// where the program is executing.
        /// </summary>
        public string DebugModeOutputPath { get; set; }

        /// <summary>
        /// Create a new instance of the interface to the OpenTrack web services
        /// </summary>
        /// <param name="BaseUrl">The Base Url of the web service end points, i.e. https://ot.dms.dealertrack.com</param>
        /// <param name="Username">The username to authenticate with the web services</param>
        /// <param name="Password">The password to authenticate with the web services</param>
        public OpenTrackAPI(String BaseUrl, String Username, String Password)
        {
            if (String.IsNullOrWhiteSpace(BaseUrl)) throw new ArgumentNullException("Invalid Url provided.");
            if (String.IsNullOrWhiteSpace(Username)) throw new ArgumentNullException("Invalid Username provided.");
            if (String.IsNullOrWhiteSpace(Password)) throw new ArgumentNullException("Invalid Password provided.");

            this.BaseUrl = BaseUrl;
            this.Username = Username;
            this.Password = Password;

            this.Timeout = TimeSpan.FromMinutes(2);

            this.DebugMode = false;
        }

        public OpenTrackAPI(String BaseUrl, String Username, String Password, IMessageLogger messageLogger) : this(BaseUrl, Username, Password)
        {
            this._messageLogger = messageLogger;
        }

        public OpenTrackAPI(String BaseUrl, String Username, String Password, Action<string> messageSending,
            Action<string> messageReceived) : this(BaseUrl, Username, Password)
        {
            this._messageLogger = new DelegateMessageLogger(messageSending, messageReceived);
        }

        public IEnumerable<OpenRepairOrderLookupResponseOpenRepairOrder> FindOpenRepairOrders(OpenRepairOrderLookup query)
        {
            return SubmitRequest<OpenRepairOrderLookupResponse>(query).Items;
        }

        public IEnumerable<ClosedRepairOrderLookupResponseClosedRepairOrder> FindClosedRepairOrders(GetClosedRepairOrderRequest query)
        {
            return SubmitRequest<ClosedRepairOrderLookupResponse>(query).Items;
        }

        public IEnumerable<ServiceWritersTableServiceWriterRecord> GetServiceAdvisors(ServiceWritersTableRequest query)
        {
            return SubmitRequest<ServiceWritersTable>(query).Items;
        }

        public IEnumerable<ServiceTechsTableServiceTechRecord> GetTechnicians(ServiceTechsTableRequest query)
        {
            return SubmitRequest<ServiceTechsTable>(query).Items;
        }

        public IEnumerable<ServiceLaborOpcodesTableServiceLaborOpcodeRecord> GetOpcodes(ServiceLaborOpcodesTableRequest query)
        {
            return SubmitRequest<ServiceLaborOpcodesTable>(query).Items;
        }

        public IEnumerable<PartsInventoryResponsePart> GetPartsInventory(PartsInventoryRequest query)
        {
            return SubmitRequest<PartsInventoryResponse>(query).Items;
        }

        public IEnumerable<PartsTransactionsResponseTransactions> GetPartsTransactions(PartsTransactionsRequest query)
        {
            return SubmitRequest<PartsTransactionsResponse>(query).Items;
        }

        public AddRepairOrderResponse AddRepairOrder(AddRepairOrderRequest query)
        {
            return SubmitRequest<AddRepairOrderResponse>(query);
        }

        public AddRepairOrderLinesResponse AddRepairOrderLines(AddRepairOrderLinesRequest query)
        {
            return SubmitRequest<AddRepairOrderLinesResponse>(query);
        }

        public CustomerSearchResponse FindCustomers(CustomerSearchRequest query)
        {
            return SubmitRequest<CustomerSearchResponse>(query);
        }

        public CustomerLookupResponseCustomer GetCustomer(CustomerLookupRequest query)
        {
            return SubmitRequest<CustomerLookupResponse>(query).Items.Single();
        }

        public CustomerAddResponse AddCustomer(CustomerAddRequest query)
        {
            return SubmitRequest<CustomerAddResponse>(query);
        }

        public CustomerUpdateResponse UpdateCustomer(CustomerUpdateRequest query)
        {
            return SubmitRequest<CustomerUpdateResponse>(query);
        }

        public IEnumerable<VehicleInventoryResponseVehicle> GetVehicleInventory(VehicleInventoryRequest query)
        {
            return SubmitRequest<VehicleInventoryResponse>(query).Items;
        }

        public VehicleLookupResponseVehicle GetVehicle(VehicleLookupRequest query)
        {
            return SubmitRequest<VehicleLookupResponse>(query).Items.SingleOrDefault();
        }

        public IEnumerable<VehicleSearchResponseVehicleSearchResult> FindVehicles(VehicleSearchRequest query)
        {
            return SubmitRequest<VehicleSearchResponse>(query).Items;
        }

        public VehicleAddResponse AddVehicle(VehicleAddRequest query)
        {
            return this.SubmitRequest<VehicleAddResponse>(query);
        }

        public VehicleUpdateResponse UpdateVehicle(VehicleUpdateRequest query)
        {
            return this.SubmitRequest<VehicleUpdateResponse>(query);
        }

        public IEnumerable<AppointmentLookupResponseAppointment> FindAppointments(AppointmentLookupRequest query)
        {
            return SubmitRequest<AppointmentLookupResponse>(query).Items;
        }

        public IEnumerable<PartsManufacturersTablePartsManufacturer> GetPartManufacturers(PartsManufacturersTableRequest query)
        {
            return SubmitRequest<PartsManufacturersTable>(query).Items;
        }

        public AppointmentAddResponse AddAppointment(AppointmentAddRequest query)
        {
            return SubmitRequest<AppointmentAddResponse>(query);
        }

        public AppointmentUpdateResponse UpdateAppointment(AppointmentUpdateRequest query)
        {
            return SubmitRequest<AppointmentUpdateResponse>(query);
        }

        public AppointmentDeleteResponse DeleteAppointment(AppointmentDeleteRequest query)
        {
            return SubmitRequest<AppointmentDeleteResponse>(query);
        }

        public IEnumerable<ServiceAPI.ClosedRepairOrder> GetClosedRepairOrders(GetClosedRepairOrdersRequest request)
        {
            return GetROService().GetClosedRepairOrders(request.Dealer, request.Request).ClosedRepairOrders;
        }

        public ServiceAPI.ClosedRepairOrder GetClosedRepairOrderDetails(GetClosedRepairOrderDetailsRequest request)
        {
            return GetROService().GetClosedRepairOrderDetails(request.Dealer, request.Request).ClosedRepairOrder;
        }

        /// <summary>
        /// Submit the prepared request to the OpenTrack API and get the response back for processing.
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>
        /// Some of this is cribbed from http://www.starstandard.org/guidelines/Architecture/QuickStart2011v1/ch05s04.html#NETClient.
        /// Go ahead and read some of the STAR spec. It'll make you weep for humanity.
        /// </remarks>
        internal virtual T SubmitRequest<T>(IRequest<T> request)
        {
            // <soap:Envelope>
            //            <soap:Header>
            //                <wsse:Security>
            //                    <wsse:UsernameToken>
            //                        <wsse:Username>${USERNAME}</wsse:Username>
            //                        <wsse:Password>${PASSWORD}</wsse:Password>
            //                    </wsse:UsernameToken>
            //                </wsse:Security>
            //                <tran:PayloadManifest>
            //                    <tran:manifest contentID="Content0" namespaceURI="CrownDMSInterop" element="1" relatedID="1" version="1.0"/>
            //                </tran:PayloadManifest>
            //            </soap:Header>
            //            <soap:Body>
            //                <tran:ProcessMessage>
            //                    <tran:payload>
            //                        <tran:content id="Content0">
            //                          ${REQUEST_CONTENT}
            //                        </tran:content>
            //               </tran:payload>
            //        </tran:ProcessMessage>
            //    </soap:Body>
            // </soap:Envelope>

            using (var svc = GetStarService())
            {
                var xml = new XmlDocument();

                // Load up the request XML into a document object.
                xml.LoadXml(request.XML);

                // Create a unique request identifier.
                var requestId = Guid.NewGuid().ToString();

                var element = xml.DocumentElement;

                // Create the message payload that will be processed.
                var payload = new OpenTrack.WebService.Payload()
                {
                    content = new OpenTrack.WebService.Content[]
                    {
                        new OpenTrack.WebService.Content()
                        {
                            id = requestId,
                            Any = element
                        }
                    }
                };

                // Tell the web service how to interpret the XML we're sending along.
                var manifest = new OpenTrack.WebService.PayloadManifest()
                {
                    manifest = new[]
                    {
                        new OpenTrack.WebService.Manifest()
                        {
                            element = element.LocalName,
                            namespaceURI = element.NamespaceURI,
                            contentID = requestId
                        }
                    }
                };

                // Send the message and it'll load the response into the same object.
                svc.ProcessMessage(ref manifest, ref payload);

                // Take the element from the first content item of the response payload.
                var response = payload.content[0].Any;

                // Check for errors
                ErrorCheck(response);

                // Process the request with the appropriate parser/handler.
                return request.ProcessResponse(response);
            }
        }

        internal virtual void ErrorCheck(XmlElement xml)
        {
            // TODO This could probably be done more efficiently?

            foreach (XmlNode child in xml.ChildNodes)
            {
                if ("Error" == child.Name)
                {
                    // <Error>
                    //      <Code>311</Code>  {{optional}}
                    //      <Message>Access validation error: Dealer has not granted access to Vendor.</Message>
                    // </Error>

                    String errorCode = "Unknown";
                    String message = child.InnerText;

                    foreach (XmlNode n in child.ChildNodes)
                    {
                        if (n.Name == "Code")
                        {
                            errorCode = n.InnerText;
                        }
                        else if (n.Name == "Message")
                        {
                            message = n.InnerText;
                        }
                    }

                    throw new OpenTrackException(errorCode, message, xml);
                }
            }
        }

        internal virtual ServiceAPI.ServiceAPISoapClient GetROService()
        {
            String Url = String.Format("{0}/{1}", this.BaseUrl, "ServiceAPI.asmx");

            var client = new ServiceAPI.ServiceAPISoapClient(GetBinding(), new EndpointAddress(Url));

            client.ClientCredentials.UserName.UserName = this.Username;
            client.ClientCredentials.UserName.Password = this.Password;

            if (this.DebugMode)
            {
                client.Endpoint.EndpointBehaviors.Add(new MessageInspectorBehavior(this._messageLogger));
            }

            return client;
        }

        internal virtual PartsAPI.PartsAPISoapClient GetPartsService()
        {
            String Url = String.Format("{0}/{1}", this.BaseUrl, "PartsAPI.asmx");

            var client = new PartsAPI.PartsAPISoapClient(GetBinding(), new EndpointAddress(Url));

            client.ClientCredentials.UserName.UserName = this.Username;
            client.ClientCredentials.UserName.Password = this.Password;

            if (this.DebugMode)
            {
                client.Endpoint.EndpointBehaviors.Add(new MessageInspectorBehavior(this._messageLogger));
            }

            return client;
        }

        /// <summary>
        /// Return a configured proxy reference to the web service.
        /// </summary>
        internal virtual WebService.starTransportClient GetStarService()
        {
            String Url = String.Format("{0}/{1}", this.BaseUrl, "WebService.asmx");

            // Create a client with the given endpoint.
            var client = new WebService.starTransportClient(GetBinding(), new EndpointAddress(Url));

            client.ClientCredentials.UserName.UserName = this.Username;
            client.ClientCredentials.UserName.Password = this.Password;

            if (this.DebugMode)
            {
                client.Endpoint.EndpointBehaviors.Add(new MessageInspectorBehavior(this._messageLogger));
            }

            return client;
        }

        internal virtual Binding GetBinding()
        {
            // We need to send the credential along with the message.
            return new BasicHttpsBinding(BasicHttpsSecurityMode.TransportWithMessageCredential)
            {
                // We could be getting back a lot of data. Let's just try and get it all!
                MaxReceivedMessageSize = int.MaxValue,
                SendTimeout = this.Timeout
            };
        }
    }
}