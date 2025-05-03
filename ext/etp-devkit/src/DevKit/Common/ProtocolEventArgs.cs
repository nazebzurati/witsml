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

using System;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides data for protocol handler events.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class ProtocolEventArgs<T> : EventArgs where T : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolEventArgs{T}"/> class.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message body.</param>
        public ProtocolEventArgs(IMessageHeader header, T message)
        {
            Header = header;
            Message = message;
        }

        /// <summary>
        /// Gets the message header.
        /// </summary>
        /// <value>The message header.</value>
        public IMessageHeader Header { get; }

        /// <summary>
        /// Gets the message body.
        /// </summary>
        /// <value>The message body.</value>
        public T Message { get; }

        /// <summary>
        /// Gets or sets a value indicating whether further processing should be cancelled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cancel; otherwise, <c>false</c>.
        /// </value>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// Provides data for protocol handler events.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class ProtocolEventArgs<T, TContext> : ProtocolEventArgs<T> where T : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolEventArgs{T, TContext}"/> class.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message body.</param>
        /// <param name="context">The additional message context.</param>
        public ProtocolEventArgs(IMessageHeader header, T message, TContext context) : base(header, message)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the additional message context.
        /// </summary>
        /// <value>The message context.</value>
        public TContext Context { get; }
    }
}
