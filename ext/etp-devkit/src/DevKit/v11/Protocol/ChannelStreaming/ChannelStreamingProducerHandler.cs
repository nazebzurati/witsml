﻿//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2019 Energistics
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes;
using Energistics.Etp.v11.Datatypes.ChannelData;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelStreamingProducer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.ChannelStreaming.IChannelStreamingProducer" />
    public class ChannelStreamingProducerHandler : Etp11ProtocolHandler, IChannelStreamingProducer
    {
        /// <summary>
        /// The SimpleStreamer protocol capability key.
        /// </summary>
        public const string SimpleStreamer = "SimpleStreamer";
        /// <summary>
        /// The DefaultUri protocol capability key.
        /// </summary>
        public const string DefaultUri = "DefaultUri";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelStreamingProducerHandler"/> class.
        /// </summary>
        public ChannelStreamingProducerHandler() : base((int)Protocols.ChannelStreaming, "producer", "consumer")
        {
            RegisterMessageHandler<Start>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.Start, HandleStart);
            RegisterMessageHandler<ChannelDescribe>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelDescribe, HandleChannelDescribe);
            RegisterMessageHandler<ChannelStreamingStart>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStreamingStart, HandleChannelStreamingStart);
            RegisterMessageHandler<ChannelStreamingStop>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStreamingStop, HandleChannelStreamingStop);
            RegisterMessageHandler<ChannelRangeRequest>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelRangeRequest, HandleChannelRangeRequest);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a Simple Streamer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a Simple Streamer; otherwise, <c>false</c>.
        /// </value>
        public bool IsSimpleStreamer { get; set; }

        /// <summary>
        /// Gets or sets the default describe URI.
        /// </summary>
        /// <value>The default describe URI.</value>
        public string DefaultDescribeUri { get; set; }

        /// <summary>
        /// Gets the maximum data items.
        /// </summary>
        /// <value>The maximum data items.</value>
        public int MaxDataItems { get; private set; }

        /// <summary>
        /// Gets the maximum message rate.
        /// </summary>
        /// <value>The maximum message rate.</value>
        public int MaxMessageRate { get; private set; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <returns>A collection of protocol capabilities.</returns>
        public override IDictionary<string, IDataValue> GetCapabilities()
        {
            var capabilities = base.GetCapabilities();

            if (IsSimpleStreamer)
                capabilities[SimpleStreamer] = new DataValue { Item = true };

            if (!string.IsNullOrWhiteSpace(DefaultDescribeUri))
                capabilities[DefaultUri] = new DataValue { Item = DefaultDescribeUri };

            return capabilities;
        }

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channelMetadataRecords">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelMetadata(IMessageHeader request, IList<ChannelMetadataRecord> channelMetadataRecords, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelMetadata, request.MessageId, messageFlag);

            var channelMetadata = new ChannelMetadata()
            {
                Channels = channelMetadataRecords
            };

            return Session.SendMessage(header, channelMetadata);
        }

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelData(IMessageHeader request, IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart)
        {
            var correlationId = 0L;

            // NOTE: CorrelationId is only specified when responding to a ChannelRangeRequest message
            if (request != null && request.MessageType == (int)MessageTypes.ChannelStreaming.ChannelRangeRequest)
                correlationId = request.MessageId;

            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelData, correlationId, messageFlag);

            var channelData = new ChannelData()
            {
                Data = dataItems
            };

            return Session.SendMessage(header, channelData);
        }

        /// <summary>
        /// Sends a ChannelDataChange message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelDataChange(long channelId, long startIndex, long endIndex, IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelDataChange);

            var channelDataChange = new ChannelDataChange()
            {
                ChannelId = channelId,
                StartIndex = startIndex,
                EndIndex = endIndex,
                Data = dataItems
            };

            return Session.SendMessage(header, channelDataChange);
        }

        /// <summary>
        /// Sends a ChannelStatusChange message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="status">The channel status.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelStatusChange(long channelId, ChannelStatuses status)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStatusChange);

            var channelStatusChange = new ChannelStatusChange()
            {
                ChannelId = channelId,
                Status = status
            };

            return Session.SendMessage(header, channelStatusChange);
        }

        /// <summary>
        /// Sends a ChannelRemove message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelRemove(long channelId, string reason = null)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelRemove);

            var channelRemove = new ChannelRemove()
            {
                ChannelId = channelId,
                RemoveReason = reason
            };

            return Session.SendMessage(header, channelRemove);
        }

        /// <summary>
        /// Handles the Start event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<Start> OnStart;

        /// <summary>
        /// Handles the ChannelDescribe event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<ChannelDescribe, IList<ChannelMetadataRecord>> OnChannelDescribe;

        /// <summary>
        /// Handles the ChannelStreamingStart event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<ChannelStreamingStart> OnChannelStreamingStart;

        /// <summary>
        /// Handles the ChannelStreamingStop event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<ChannelStreamingStop> OnChannelStreamingStop;

        /// <summary>
        /// Handles the ChannelRangeRequest event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<ChannelRangeRequest> OnChannelRangeRequest;

        /// <summary>
        /// Handles the Start message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="start">The Start message.</param>
        protected virtual void HandleStart(IMessageHeader header, Start start)
        {
            MaxDataItems = start.MaxDataItems;
            MaxMessageRate = start.MaxMessageRate;
            Notify(OnStart, header, start);
        }

        /// <summary>
        /// Handles the ChannelDescribe message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelDescribe">The ChannelDescribe message.</param>
        protected virtual void HandleChannelDescribe(IMessageHeader header, ChannelDescribe channelDescribe)
        {
            var args = Notify(OnChannelDescribe, header, channelDescribe, new List<ChannelMetadataRecord>());
            HandleChannelDescribe(args);

            if (!args.Cancel)
            {
                ChannelMetadata(header, args.Context);
            }
        }

        /// <summary>
        /// Handles the ChannelDescribe message from a consumer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{ChannelDescribe}"/> instance containing the event data.</param>
        protected virtual void HandleChannelDescribe(ProtocolEventArgs<ChannelDescribe, IList<ChannelMetadataRecord>> args)
        {
        }

        /// <summary>
        /// Handles the ChannelStreamingStart message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelStreamingStart">The ChannelStreamingStart message.</param>
        protected virtual void HandleChannelStreamingStart(IMessageHeader header, ChannelStreamingStart channelStreamingStart)
        {
            Notify(OnChannelStreamingStart, header, channelStreamingStart);
        }

        /// <summary>
        /// Handles the ChannelStreamingStop message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelStreamingStop">The ChannelStreamingStop message.</param>
        protected virtual void HandleChannelStreamingStop(IMessageHeader header, ChannelStreamingStop channelStreamingStop)
        {
            Notify(OnChannelStreamingStop, header, channelStreamingStop);
        }

        /// <summary>
        /// Handles the ChannelRangeRequest message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelRangeRequest">The ChannelRangeRequest message.</param>
        protected virtual void HandleChannelRangeRequest(IMessageHeader header, ChannelRangeRequest channelRangeRequest)
        {
            Notify(OnChannelRangeRequest, header, channelRangeRequest);
        }
    }
}
