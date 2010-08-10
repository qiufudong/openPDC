//*******************************************************************************************************
//  MultiProtocolFrameParser.cs - Gbtc
//
//  Tennessee Valley Authority, 2009
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  This software is made freely available under the TVA Open Source Agreement (see below).
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  03/16/2006 - J. Ritchie Carroll
//       Initial version of source generated.
//  06/26/2006 - Pinal C. Patel
//       Changed out the socket code with TcpClient and UdpClient components from TVA.Communication.
//  01/31/2007 - J. Ritchie Carroll
//       Added TCP "server" support to allow listening connections from devices that act as data
//       clients, e.g., F-NET devices.
//  05/23/2007 - Pinal C. Patel
//       Added member variable 'm_clientConnectionAttempts' to track the number of attempts made for
//       connecting to the server since this information is no longer provided by the event raised by
//       any of the Communication Client components.
//  07/05/2007 - J. Ritchie Carroll
//       Wrapped all event raising for frame parsing in Try/Catch so that any exceptions thrown in
//       consumer event handlers won't have a negative effect on continuous data parsing - exceptions
//       in consumer event handlers are duly noted and raised through the ParsingException event.
//  09/28/2007 - J. Ritchie Carroll
//       Implemented new disconnect overload on communications client that allows timeout on socket
//       close to fix an issue related non-responsive threads that "lock-up" after sending connection
//       commands that attempt to close the socket for remotely connected devices.
//  12/14/2007 - J. Ritchie Carroll
//       Implemented simulated timestamp injection for published frames to allow for real-time
//       data simulations from archived sample data.
//  10/28/2008 - J. Ritchie Carroll
//       Added support for SEL's UDP_T and UDP_U protocol implementations (UDP_S was already supported),
//       implementation handled by allowing definition of a "CommandChannel" in the connection string.
//  04/27/2009 - J. Ritchie Carroll
//       Added support for SEL Fast Message protocol.
//  08/07/2009 - Josh L. Patterson
//       Edited Comments.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  02/12/2010 - Pinal C. Patel
//       Modified to start the IFrameParser object in InitializeFrameParser() instead of Start().
//  03/20/2010 - J. Ritchie Carroll
//       Added property "SkipDisableRealTimeData" to allow consumer to bypass sending the command to
//       turn off the real-time data stream when automatically starting the data parsing sequence. This
//       is useful when using UDP multicast that may have many listeners, in these cases you don't want
//       to disable the stream on startup or shutdown since other applications may be subscribed to the
//       real-time stream.
//  03/21/2010 - J. Ritchie Carroll
//       Added parsing exception threshold settings and consumer event to handle situation.
//  06/13/2010 - J. Ritchie Carroll
//       Added several more run-time statistics to the frame parser (e.g., missing frames, CRC errors).
//
//*******************************************************************************************************

#region [ TVA Open Source Agreement ]
/*

 THIS OPEN SOURCE AGREEMENT ("AGREEMENT") DEFINES THE RIGHTS OF USE,REPRODUCTION, DISTRIBUTION,
 MODIFICATION AND REDISTRIBUTION OF CERTAIN COMPUTER SOFTWARE ORIGINALLY RELEASED BY THE
 TENNESSEE VALLEY AUTHORITY, A CORPORATE AGENCY AND INSTRUMENTALITY OF THE UNITED STATES GOVERNMENT
 ("GOVERNMENT AGENCY"). GOVERNMENT AGENCY IS AN INTENDED THIRD-PARTY BENEFICIARY OF ALL SUBSEQUENT
 DISTRIBUTIONS OR REDISTRIBUTIONS OF THE SUBJECT SOFTWARE. ANYONE WHO USES, REPRODUCES, DISTRIBUTES,
 MODIFIES OR REDISTRIBUTES THE SUBJECT SOFTWARE, AS DEFINED HEREIN, OR ANY PART THEREOF, IS, BY THAT
 ACTION, ACCEPTING IN FULL THE RESPONSIBILITIES AND OBLIGATIONS CONTAINED IN THIS AGREEMENT.

 Original Software Designation: openPDC
 Original Software Title: The TVA Open Source Phasor Data Concentrator
 User Registration Requested. Please Visit https://naspi.tva.com/Registration/
 Point of Contact for Original Software: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>

 1. DEFINITIONS

 A. "Contributor" means Government Agency, as the developer of the Original Software, and any entity
 that makes a Modification.

 B. "Covered Patents" mean patent claims licensable by a Contributor that are necessarily infringed by
 the use or sale of its Modification alone or when combined with the Subject Software.

 C. "Display" means the showing of a copy of the Subject Software, either directly or by means of an
 image, or any other device.

 D. "Distribution" means conveyance or transfer of the Subject Software, regardless of means, to
 another.

 E. "Larger Work" means computer software that combines Subject Software, or portions thereof, with
 software separate from the Subject Software that is not governed by the terms of this Agreement.

 F. "Modification" means any alteration of, including addition to or deletion from, the substance or
 structure of either the Original Software or Subject Software, and includes derivative works, as that
 term is defined in the Copyright Statute, 17 USC § 101. However, the act of including Subject Software
 as part of a Larger Work does not in and of itself constitute a Modification.

 G. "Original Software" means the computer software first released under this Agreement by Government
 Agency entitled openPDC, including source code, object code and accompanying documentation, if any.

 H. "Recipient" means anyone who acquires the Subject Software under this Agreement, including all
 Contributors.

 I. "Redistribution" means Distribution of the Subject Software after a Modification has been made.

 J. "Reproduction" means the making of a counterpart, image or copy of the Subject Software.

 K. "Sale" means the exchange of the Subject Software for money or equivalent value.

 L. "Subject Software" means the Original Software, Modifications, or any respective parts thereof.

 M. "Use" means the application or employment of the Subject Software for any purpose.

 2. GRANT OF RIGHTS

 A. Under Non-Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor,
 with respect to its own contribution to the Subject Software, hereby grants to each Recipient a
 non-exclusive, world-wide, royalty-free license to engage in the following activities pertaining to
 the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Modification

 5. Redistribution

 6. Display

 B. Under Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor, with
 respect to its own contribution to the Subject Software, hereby grants to each Recipient under Covered
 Patents a non-exclusive, world-wide, royalty-free license to engage in the following activities
 pertaining to the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Sale

 5. Offer for Sale

 C. The rights granted under Paragraph B. also apply to the combination of a Contributor's Modification
 and the Subject Software if, at the time the Modification is added by the Contributor, the addition of
 such Modification causes the combination to be covered by the Covered Patents. It does not apply to
 any other combinations that include a Modification. 

 D. The rights granted in Paragraphs A. and B. allow the Recipient to sublicense those same rights.
 Such sublicense must be under the same terms and conditions of this Agreement.

 3. OBLIGATIONS OF RECIPIENT

 A. Distribution or Redistribution of the Subject Software must be made under this Agreement except for
 additions covered under paragraph 3H. 

 1. Whenever a Recipient distributes or redistributes the Subject Software, a copy of this Agreement
 must be included with each copy of the Subject Software; and

 2. If Recipient distributes or redistributes the Subject Software in any form other than source code,
 Recipient must also make the source code freely available, and must provide with each copy of the
 Subject Software information on how to obtain the source code in a reasonable manner on or through a
 medium customarily used for software exchange.

 B. Each Recipient must ensure that the following copyright notice appears prominently in the Subject
 Software:

          No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.

 C. Each Contributor must characterize its alteration of the Subject Software as a Modification and
 must identify itself as the originator of its Modification in a manner that reasonably allows
 subsequent Recipients to identify the originator of the Modification. In fulfillment of these
 requirements, Contributor must include a file (e.g., a change log file) that describes the alterations
 made and the date of the alterations, identifies Contributor as originator of the alterations, and
 consents to characterization of the alterations as a Modification, for example, by including a
 statement that the Modification is derived, directly or indirectly, from Original Software provided by
 Government Agency. Once consent is granted, it may not thereafter be revoked.

 D. A Contributor may add its own copyright notice to the Subject Software. Once a copyright notice has
 been added to the Subject Software, a Recipient may not remove it without the express permission of
 the Contributor who added the notice.

 E. A Recipient may not make any representation in the Subject Software or in any promotional,
 advertising or other material that may be construed as an endorsement by Government Agency or by any
 prior Recipient of any product or service provided by Recipient, or that may seek to obtain commercial
 advantage by the fact of Government Agency's or a prior Recipient's participation in this Agreement.

 F. In an effort to track usage and maintain accurate records of the Subject Software, each Recipient,
 upon receipt of the Subject Software, is requested to register with Government Agency by visiting the
 following website: https://naspi.tva.com/Registration/. Recipient's name and personal information
 shall be used for statistical purposes only. Once a Recipient makes a Modification available, it is
 requested that the Recipient inform Government Agency at the web site provided above how to access the
 Modification.

 G. Each Contributor represents that that its Modification does not violate any existing agreements,
 regulations, statutes or rules, and further that Contributor has sufficient rights to grant the rights
 conveyed by this Agreement.

 H. A Recipient may choose to offer, and to charge a fee for, warranty, support, indemnity and/or
 liability obligations to one or more other Recipients of the Subject Software. A Recipient may do so,
 however, only on its own behalf and not on behalf of Government Agency or any other Recipient. Such a
 Recipient must make it absolutely clear that any such warranty, support, indemnity and/or liability
 obligation is offered by that Recipient alone. Further, such Recipient agrees to indemnify Government
 Agency and every other Recipient for any liability incurred by them as a result of warranty, support,
 indemnity and/or liability offered by such Recipient.

 I. A Recipient may create a Larger Work by combining Subject Software with separate software not
 governed by the terms of this agreement and distribute the Larger Work as a single product. In such
 case, the Recipient must make sure Subject Software, or portions thereof, included in the Larger Work
 is subject to this Agreement.

 J. Notwithstanding any provisions contained herein, Recipient is hereby put on notice that export of
 any goods or technical data from the United States may require some form of export license from the
 U.S. Government. Failure to obtain necessary export licenses may result in criminal liability under
 U.S. laws. Government Agency neither represents that a license shall not be required nor that, if
 required, it shall be issued. Nothing granted herein provides any such export license.

 4. DISCLAIMER OF WARRANTIES AND LIABILITIES; WAIVER AND INDEMNIFICATION

 A. No Warranty: THE SUBJECT SOFTWARE IS PROVIDED "AS IS" WITHOUT ANY WARRANTY OF ANY KIND, EITHER
 EXPRESSED, IMPLIED, OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, ANY WARRANTY THAT THE SUBJECT
 SOFTWARE WILL CONFORM TO SPECIFICATIONS, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 PARTICULAR PURPOSE, OR FREEDOM FROM INFRINGEMENT, ANY WARRANTY THAT THE SUBJECT SOFTWARE WILL BE ERROR
 FREE, OR ANY WARRANTY THAT DOCUMENTATION, IF PROVIDED, WILL CONFORM TO THE SUBJECT SOFTWARE. THIS
 AGREEMENT DOES NOT, IN ANY MANNER, CONSTITUTE AN ENDORSEMENT BY GOVERNMENT AGENCY OR ANY PRIOR
 RECIPIENT OF ANY RESULTS, RESULTING DESIGNS, HARDWARE, SOFTWARE PRODUCTS OR ANY OTHER APPLICATIONS
 RESULTING FROM USE OF THE SUBJECT SOFTWARE. FURTHER, GOVERNMENT AGENCY DISCLAIMS ALL WARRANTIES AND
 LIABILITIES REGARDING THIRD-PARTY SOFTWARE, IF PRESENT IN THE ORIGINAL SOFTWARE, AND DISTRIBUTES IT
 "AS IS."

 B. Waiver and Indemnity: RECIPIENT AGREES TO WAIVE ANY AND ALL CLAIMS AGAINST GOVERNMENT AGENCY, ITS
 AGENTS, EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT. IF RECIPIENT'S USE
 OF THE SUBJECT SOFTWARE RESULTS IN ANY LIABILITIES, DEMANDS, DAMAGES, EXPENSES OR LOSSES ARISING FROM
 SUCH USE, INCLUDING ANY DAMAGES FROM PRODUCTS BASED ON, OR RESULTING FROM, RECIPIENT'S USE OF THE
 SUBJECT SOFTWARE, RECIPIENT SHALL INDEMNIFY AND HOLD HARMLESS  GOVERNMENT AGENCY, ITS AGENTS,
 EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT, TO THE EXTENT PERMITTED BY
 LAW.  THE FOREGOING RELEASE AND INDEMNIFICATION SHALL APPLY EVEN IF THE LIABILITIES, DEMANDS, DAMAGES,
 EXPENSES OR LOSSES ARE CAUSED, OCCASIONED, OR CONTRIBUTED TO BY THE NEGLIGENCE, SOLE OR CONCURRENT, OF
 GOVERNMENT AGENCY OR ANY PRIOR RECIPIENT.  RECIPIENT'S SOLE REMEDY FOR ANY SUCH MATTER SHALL BE THE
 IMMEDIATE, UNILATERAL TERMINATION OF THIS AGREEMENT.

 5. GENERAL TERMS

 A. Termination: This Agreement and the rights granted hereunder will terminate automatically if a
 Recipient fails to comply with these terms and conditions, and fails to cure such noncompliance within
 thirty (30) days of becoming aware of such noncompliance. Upon termination, a Recipient agrees to
 immediately cease use and distribution of the Subject Software. All sublicenses to the Subject
 Software properly granted by the breaching Recipient shall survive any such termination of this
 Agreement.

 B. Severability: If any provision of this Agreement is invalid or unenforceable under applicable law,
 it shall not affect the validity or enforceability of the remainder of the terms of this Agreement.

 C. Applicable Law: This Agreement shall be subject to United States federal law only for all purposes,
 including, but not limited to, determining the validity of this Agreement, the meaning of its
 provisions and the rights, obligations and remedies of the parties.

 D. Entire Understanding: This Agreement constitutes the entire understanding and agreement of the
 parties relating to release of the Subject Software and may not be superseded, modified or amended
 except by further written agreement duly executed by the parties.

 E. Binding Authority: By accepting and using the Subject Software under this Agreement, a Recipient
 affirms its authority to bind the Recipient to all terms and conditions of this Agreement and that
 Recipient hereby agrees to all terms and conditions herein.

 F. Point of Contact: Any Recipient contact with Government Agency is to be directed to the designated
 representative as follows: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>.

*/
#endregion

// Define this constant to enable a raw data export for debugging - do not leave this on for deployed builds
#undef RawDataCapture

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TVA.Communication;
using TVA.IO;
using TVA.Measurements;
using TVA.Units;

namespace TVA.PhasorProtocols
{
    #region [ Enumerations ]

    /// <summary>
    /// Phasor data protocols enumeration.
    /// </summary>
    [Serializable()]
    public enum PhasorProtocol
    {
        /// <summary>
        /// IEEE C37.118-2005 protocol.
        /// </summary>
        IeeeC37_118V1,
        /// <summary>
        /// IEEE C37.118, draft 6 protocol.
        /// </summary>
        IeeeC37_118D6,
        /// <summary>
        /// IEEE 1344-1995 protocol.
        /// </summary>
        Ieee1344,
        /// <summary>
        /// BPA PDCstream protocol.
        /// </summary>
        BpaPdcStream,
        /// <summary>
        /// Virgina Tech F-NET protocol.
        /// </summary>
        FNet,
        /// <summary>
        /// SEL Fast Message protocol.
        /// </summary>
        SelFastMessage,
        /// <summary>
        /// Macrodyne protocol.
        /// </summary>
        Macrodyne
    }

    #endregion

    /// <summary>
    /// Protocol independent frame parser.
    /// </summary>
    /// <remarks>
    /// This class takes all protocol frame parsing implementations and reduces them to a single simple-to-use class exposing all
    /// data through abstract interfaces (e.g., IConfigurationFrame, IDataFrame, etc.) - this way new protocol implementations can
    /// be added without adversely affecting consuming code. Additionally, this class implements a variety of transport options
    /// (e.g., TCP, UDP, Serial, etc.) and hides the complexities of this connectivity and internally pushes all data received from
    /// the selected transport protocol to the selected phasor parsing protocol.
    /// </remarks>
    public sealed class MultiProtocolFrameParser : IFrameParser
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the default value for the <see cref="BufferSize"/> property.
        /// </summary>
        public const int DefaultBufferSize = 262144; // 256K

        /// <summary>
        /// Specifies the default value for the <see cref="DefinedFrameRate"/> property.
        /// </summary>
        public const int DefaultDefinedFrameRate = 30;

        /// <summary>
        /// Specifies the default value for the <see cref="MaximumConnectionAttempts"/> property.
        /// </summary>
        public const int DefaultMaximumConnectionAttempts = -1;

        /// <summary>
        /// Specifies the default value for the <see cref="AutoStartDataParsingSequence"/> property.
        /// </summary>
        public const bool DefaultAutoStartDataParsingSequence = true;

        /// <summary>
        /// Specfies the default value for the <see cref="AllowedParsingExceptions"/> property.
        /// </summary>
        public const int DefaultAllowedParsingExceptions = 10;

        /// <summary>
        /// Specifies the default value for the <see cref="ParsingExceptionWindow"/> property.
        /// </summary>
        public const long DefaultParsingExceptionWindow = 50000000L; // 5 seconds

        // Events

        /// <summary>
        /// Occurs when a <see cref="ICommandFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="ICommandFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<ICommandFrame>> ReceivedCommandFrame;

        /// <summary>
        /// Occurs when a <see cref="IConfigurationFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="IConfigurationFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IConfigurationFrame>> ReceivedConfigurationFrame;

        /// <summary>
        /// Occurs when a <see cref="IDataFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="IDataFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IDataFrame>> ReceivedDataFrame;

        /// <summary>
        /// Occurs when a <see cref="IHeaderFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="IHeaderFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IHeaderFrame>> ReceivedHeaderFrame;

        /// <summary>
        /// Occurs when an undetermined <see cref="IChannelFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the undetermined <see cref="IChannelFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IChannelFrame>> ReceivedUndeterminedFrame;

        /// <summary>
        /// Occurs when a frame buffer image has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument1"/> is the <see cref="FundamentalFrameType"/> of the frame buffer image that was received.<br/>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument2"/> is the buffer that contains the frame image that was received.<br/>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument3"/> is the offset into the buffer that contains the frame image that was received.<br/>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument4"/> is the length of data in the buffer that contains the frame image that was received.
        /// </remarks>
        public event EventHandler<EventArgs<FundamentalFrameType, byte[], int, int>> ReceivedFrameBufferImage;

        /// <summary>
        /// Occurs when a device sends a notification that its configuration has changed.
        /// </summary>
        public event EventHandler ConfigurationChanged;
        
        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered while parsing the data stream.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="Exception"/> encountered while parsing the data stream.
        /// </remarks>
        public event EventHandler<EventArgs<Exception>> ParsingException;

        /// <summary>
        /// Occurs when number of parsing exceptions exceed <see cref="AllowedParsingExceptions"/> during <see cref="ParsingExceptionWindow"/>.
        /// </summary>
        public event EventHandler ExceededParsingExceptionThreshold;

        /// <summary>
        /// Occurs when a <see cref="ICommandFrame"/> is sent to a device.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is a reference to the <see cref="ICommandFrame"/> that was sent to the device.
        /// </remarks>
        public event EventHandler<EventArgs<ICommandFrame>> SentCommandFrame;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered during connection attempt to a device.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T1,T2}.Argument1"/> is the exception that occured during the connection attempt.<br/>
        /// <see cref="EventArgs{T1,T2}.Argument2"/> is the number of connections attempted so far.
        /// </remarks>
        public event EventHandler<EventArgs<Exception, int>> ConnectionException;

        /// <summary>
        /// Occurs when <see cref="MultiProtocolFrameParser"/> is attempting connection to a device.
        /// </summary>
        public event EventHandler ConnectionAttempt;

        /// <summary>
        /// Occurs when <see cref="MultiProtocolFrameParser"/> has established a connection to a device.
        /// </summary>
        public event EventHandler ConnectionEstablished;

        /// <summary>
        /// Occurs when device connection has been terminated.
        /// </summary>
        public event EventHandler ConnectionTerminated;
        
        /// <summary>
        /// Occurs when the <see cref="MultiProtocolFrameParser"/> is setup as a listening connection and server connection has been started.
        /// </summary>
        public event EventHandler ServerStarted;

        /// <summary>
        /// Occurs when the <see cref="MultiProtocolFrameParser"/> is setup as a listening connection and server connection has been stopped.
        /// </summary>
        public event EventHandler ServerStopped;

        // Fields
        private PhasorProtocol m_phasorProtocol;
        private TransportProtocol m_transportProtocol;
        private string m_connectionString;
        private int m_maximumConnectionAttempts;
        private ushort m_deviceID;
        private int m_bufferSize;
        private IFrameParser m_frameParser;
        private IClient m_dataChannel;
        private IServer m_serverBasedDataChannel;
        private IClient m_commandChannel;
        private PrecisionTimer m_inputTimer;
        private System.Timers.Timer m_rateCalcTimer;
        private IConfigurationFrame m_configurationFrame;
        private AutoResetEvent m_frameWaitHandle;
        private long m_dataStreamStartTime;
        private bool m_executeParseOnSeparateThread;
        private bool m_autoRepeatCapturedPlayback;
        private bool m_injectSimulatedTimestamp;
        private long m_totalFramesReceived;
        private long m_totalMissingFrames;
        private long m_totalCrcExceptions;
        private int m_frameRateTotal;
        private int m_byteRateTotal;
        private long m_totalBytesReceived;
        private long m_configuredFrameRate;
        private double m_calculatedFrameRate;
        private double m_calculatedByteRate;
        private string m_sourceName;
        private int m_definedFrameRate;
        private double m_ticksPerFrame;
        private long m_lastFrameReceivedTime;
        private bool m_autoStartDataParsingSequence;
        private bool m_skipDisableRealTimeData;
        private bool m_initiatingDataStream;
        private long m_initialBytesReceived;
        private bool m_deviceSupportsCommands;
        private int m_parsingExceptionCount;
        private long m_lastParsingExceptionTime;
        private int m_allowedParsingExceptions;
        private Ticks m_parsingExceptionWindow;
        private bool m_enabled;
        private IConnectionParameters m_connectionParameters;
        private int m_connectionAttempts;
        private bool m_disposed;

        #if RawDataCapture
        FileStream m_rawDataCapture;
        #endif

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MultiProtocolFrameParser"/> using the default settings.
        /// </summary>
        public MultiProtocolFrameParser()
        {
            m_connectionString = "server=127.0.0.1:4712";
            m_deviceID = 1;
            m_bufferSize = DefaultBufferSize;
            m_definedFrameRate = DefaultDefinedFrameRate;
            m_maximumConnectionAttempts = DefaultMaximumConnectionAttempts;
            m_autoStartDataParsingSequence = DefaultAutoStartDataParsingSequence;
            m_allowedParsingExceptions = DefaultAllowedParsingExceptions;
            m_parsingExceptionWindow = DefaultParsingExceptionWindow;
            m_rateCalcTimer = new System.Timers.Timer();

            m_phasorProtocol = PhasorProtocol.IeeeC37_118V1;
            m_transportProtocol = TransportProtocol.Tcp;

            m_rateCalcTimer.Elapsed += m_rateCalcTimer_Elapsed;
            m_rateCalcTimer.Interval = 5000;
            m_rateCalcTimer.AutoReset = true;
            m_rateCalcTimer.Enabled = false;

            m_frameWaitHandle = new AutoResetEvent(false);

            // Set minimum timer resolution to one millisecond to improve timer accuracy
            PrecisionTimer.SetMinimumTimerResolution(1);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="MultiProtocolFrameParser"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~MultiProtocolFrameParser()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets <see cref="PhasorProtocols.PhasorProtocol"/> to use with this <see cref="MultiProtocolFrameParser"/>.
        /// </summary>
        public PhasorProtocol PhasorProtocol
        {
            get
            {
                return m_phasorProtocol;
            }
            set
            {
                m_phasorProtocol = value;
                m_deviceSupportsCommands = DeriveCommandSupport();

                // Setup protocol specific connection parameters...
                switch (value)
                {
                    case PhasorProtocols.PhasorProtocol.BpaPdcStream:
                        m_connectionParameters = new BpaPdcStream.ConnectionParameters();
                        break;
                    case PhasorProtocols.PhasorProtocol.FNet:
                        m_connectionParameters = new FNet.ConnectionParameters();
                        break;
                    case PhasorProtocols.PhasorProtocol.SelFastMessage:
                        m_connectionParameters = new SelFastMessage.ConnectionParameters();
                        break;
                    default:
                        m_connectionParameters = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="TransportProtocol"/> to use with this <see cref="MultiProtocolFrameParser"/>.
        /// </summary>
        public TransportProtocol TransportProtocol
        {
            get
            {
                return m_transportProtocol;
            }
            set
            {
                m_transportProtocol = value;
                m_deviceSupportsCommands = DeriveCommandSupport();

                // File based input connections are handled more carefully
                if (m_transportProtocol == TransportProtocol.File)
                {
                    if (m_autoRepeatCapturedPlayback)
                        m_executeParseOnSeparateThread = false;

                    if (m_maximumConnectionAttempts < 1)
                        m_maximumConnectionAttempts = 1;
                }
            }
        }

        /// <summary>
        /// Gets or sets the key/value pair based connection information required by the <see cref="MultiProtocolFrameParser"/> to connect to a device.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return m_connectionString;
            }
            set
            {
                m_connectionString = value;

                // Parse connection string to see if a phasor or transport protocol was assigned
                Dictionary<string, string> settings = m_connectionString.ParseKeyValuePairs();
                string setting;

                if (settings.TryGetValue("phasorProtocol", out setting))
                    PhasorProtocol = (PhasorProtocol)Enum.Parse(typeof(PhasorProtocol), setting, true);

                if (settings.TryGetValue("transportProtocol", out setting) || settings.TryGetValue("protocol", out setting))
                    TransportProtocol = (TransportProtocol)Enum.Parse(typeof(TransportProtocol), setting, true);

                m_deviceSupportsCommands = DeriveCommandSupport();
            }
        }

        /// <summary>
        /// Gets or sets flag that determines if a device supports commands.
        /// </summary>
        /// <remarks>
        /// This property is automatically derived based on the selected <see cref="PhasorProtocol"/>, <see cref="TransportProtocol"/>
        /// and <see cref="ConnectionString"/>, but can be overriden if the consumer already knows that a device supports commands.
        /// </remarks>
        public bool DeviceSupportsCommands
        {
            get
            {
                return m_deviceSupportsCommands;
            }
            set
            {
                // Consumers can choose to override command support if needed
                m_deviceSupportsCommands = value;
            }
        }

        /// <summary>
        /// Gets or sets the device identification code often needed to establish a connection.
        /// </summary>
        /// <remarks>
        /// Most devices validate this ID when sending commands, so it must be correct in order to start parsing sequence.
        /// </remarks>
        public ushort DeviceID
        {
            get
            {
                return m_deviceID;
            }
            set
            {
                m_deviceID = value;
            }
        }

        /// <summary>
        /// Gets or sets flag that determines if system should use precision timing for file based inputs.
        /// </summary>
        public bool UseHighResolutionInputTimer
        {
            get
            {
                return (m_inputTimer != null);
            }
            set
            {
                if (value && m_inputTimer == null)
                {
                    m_inputTimer = new PrecisionTimer();
                    m_inputTimer.Resolution = 1;
                    m_inputTimer.Period = 1;
                    m_inputTimer.AutoReset = true;
                    m_inputTimer.Tick += m_inputTimer_Tick;
                    m_inputTimer.Start();
                }
                else if (!value && m_inputTimer != null)
                {
                    m_inputTimer.Tick -= m_inputTimer_Tick;
                    m_inputTimer.Dispose();
                    m_inputTimer = null;

                    // Make sure any waiting threads are released
                    m_frameWaitHandle.Set();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the buffer used by the <see cref="MultiProtocolFrameParser"/> for sending and receiving data from a device.
        /// </summary>
        /// <exception cref="ArgumentException">The value specified is either zero or negative.</exception>
        public int BufferSize
        {
            get
            {
                return m_bufferSize;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Value cannot be zero or negative.");

                m_bufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of times the <see cref="MultiProtocolFrameParser"/> will attempt to connect to a device.
        /// </summary>
        /// <remarks>Set to -1 for infinite connection attempts.</remarks>
        public int MaximumConnectionAttempts
        {
            get
            {
                return m_maximumConnectionAttempts;
            }
            set
            {
                m_maximumConnectionAttempts = value;

                // All values below zero are assumed to mean infinite connection attempts
                if (m_maximumConnectionAttempts < 1)
                    m_maximumConnectionAttempts = -1;

                // We don't allow maximum connection attempts set to infinite if using file based source since file based
                // connection errors are like "file not found", "invalid path", etc. These connection exceptions are returned
                // so quickly that they will queue up much faster than they will be reported.
                if (m_maximumConnectionAttempts < 1 && m_transportProtocol == TransportProtocol.File)
                    m_maximumConnectionAttempts = 1;
            }
        }

        /// <summary>
        /// Gets or sets flag to automatically send the ConfigFrame2 and EnableRealTimeData command frames used to start a typical data parsing sequence.
        /// </summary>
        /// <remarks>
        /// For devices that support IEEE commands, setting this property to true will automatically start the data parsing sequence.
        /// </remarks>
        public bool AutoStartDataParsingSequence
        {
            get
            {
                return m_autoStartDataParsingSequence;
            }
            set
            {
                m_autoStartDataParsingSequence = value;
            }
        }

        /// <summary>
        /// Gets or sets flag to skip automatic disabling of the real-time data stream on shutdown or startup.
        /// </summary>
        /// <remarks>
        /// This flag may important when using UDP multicast with several subscribed clients.
        /// </remarks>
        public bool SkipDisableRealTimeData
        {
            get
            {
                return m_skipDisableRealTimeData;
            }
            set
            {
                m_skipDisableRealTimeData = value;
            }
        }

        /// <summary>
        /// Gets or sets number of parsing exceptions allowed during <see cref="ParsingExceptionWindow"/> before connection is reset.
        /// </summary>
        public int AllowedParsingExceptions
        {
            get
            {
                return m_allowedParsingExceptions;
            }
            set
            {
                m_allowedParsingExceptions = value;
            }
        }

        /// <summary>
        /// Gets or sets time duration, in <see cref="Ticks"/>, to monitor parsing exceptions.
        /// </summary>
        public Ticks ParsingExceptionWindow
        {
            get
            {
                return m_parsingExceptionWindow;
            }
            set
            {
                m_parsingExceptionWindow = value;
            }
        }

        /// <summary>
        /// Gets or sets a descriptive name for a device connection.
        /// </summary>
        public string SourceName
        {
            get
            {
                return m_sourceName;
            }
            set
            {
                m_sourceName = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag that allows frame parsing to be executed on a separate thread (i.e., other than communications thread).
        /// </summary>
        /// <remarks>
        /// This is typically only needed when data frames are very large. This change will happen dynamically, even if a connection is active.
        /// </remarks>
        public bool ExecuteParseOnSeparateThread
        {
            get
            {
                return m_executeParseOnSeparateThread;
            }
            set
            {
                // If using file based source and auto-repeat is enabled, we don't allow execution on a separate thread
                // since file based streaming data source will continue to queue data as quickly as possible and add data
                // data to processing queue much faster than it will be processed thereby consuming all available memory
                if (m_transportProtocol == TransportProtocol.File && m_autoRepeatCapturedPlayback)
                    m_executeParseOnSeparateThread = false;
                else
                    m_executeParseOnSeparateThread = value;

                // Since frame parsers support dynamic changes in this value, we'll pass this value along to the
                // the frame parser if one has been established...
                if (m_frameParser != null)
                    m_frameParser.ExecuteParseOnSeparateThread = m_executeParseOnSeparateThread;
            }
        }

        /// <summary>
        /// Gets or sets desired frame rate to use for maintaining captured frame replay timing.
        /// </summary>
        /// <remarks>
        /// This is only applicable when connection is made to a file for replay purposes.
        /// </remarks>
        public int DefinedFrameRate
        {
            get
            {
                return m_definedFrameRate;
            }
            set
            {
                m_definedFrameRate = value;
                m_ticksPerFrame = Ticks.PerSecond / (double)m_definedFrameRate;
            }
        }

        /// <summary>
        /// Gets or sets flag indicating whether or not to inject local system time into parsed data frames.
        /// </summary>
        /// <remarks>
        /// When connection is made to a file for replay purposes or consumer doesn't trust remote clock source, this flag
        /// can be set to true replace all frame timestamps with a UTC timestamp dervied from the local system clock.
        /// </remarks>
        public bool InjectSimulatedTimestamp
        {
            get
            {
                return m_injectSimulatedTimestamp;
            }
            set
            {
                m_injectSimulatedTimestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag that determines if a file used for replaying data should be restarted at the beginning once it has been completed.
        /// </summary>
        /// <remarks>
        /// This is only applicable when connection is made to a file for replay purposes.
        /// </remarks>
        public bool AutoRepeatCapturedPlayback
        {
            get
            {
                return m_autoRepeatCapturedPlayback;
            }
            set
            {
                m_autoRepeatCapturedPlayback = value;

                if (m_transportProtocol == TransportProtocol.File && m_autoRepeatCapturedPlayback)
                    ExecuteParseOnSeparateThread = false;
            }
        }

        /// <summary>
        /// Gets or sets current <see cref="IConfigurationFrame"/> used for parsing <see cref="IDataFrame"/>'s encountered in the data stream from a device.
        /// </summary>
        /// <remarks>
        /// If a <see cref="IConfigurationFrame"/> has been parsed, this will return a reference to the parsed frame.  Consumer can manually assign a
        /// <see cref="IConfigurationFrame"/> to start parsing data if one has not been encountered in the stream.
        /// </remarks>
        public IConfigurationFrame ConfigurationFrame
        {
            get
            {
                return m_configurationFrame;
            }
            set
            {
                m_configurationFrame = value;

                // Pass new config frame onto appropriate parser, casting into appropriate protocol if needed...
                if (m_frameParser != null)
                    m_frameParser.ConfigurationFrame = value;
            }
        }

        /// <summary>
        /// Gets a flag that determines if the currently selected <see cref="PhasorProtocol"/> is an IEEE standard protocol.
        /// </summary>
        public bool IsIEEEProtocol
        {
            get
            {
                return (m_phasorProtocol == PhasorProtocols.PhasorProtocol.IeeeC37_118V1 || 
                        m_phasorProtocol == PhasorProtocols.PhasorProtocol.IeeeC37_118D6 || 
                        m_phasorProtocol == PhasorProtocols.PhasorProtocol.Ieee1344);
            }
        }

        /// <summary>
        /// Gets a flag that determines if the currently selected <see cref="TransportProtocol"/> is connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (m_commandChannel != null)
                    return (m_commandChannel.CurrentState == ClientState.Connected);
                else if (m_dataChannel != null)
                    return (m_dataChannel.CurrentState == ClientState.Connected);
                else if (m_serverBasedDataChannel != null)
                    return (m_serverBasedDataChannel.ClientIDs.Length > 0);

                return false;
            }
        }

        /// <summary>
        /// Gets total time connection has been active.
        /// </summary>
        public Time ConnectionTime
        {
            get
            {
                if (m_commandChannel != null)
                    return m_commandChannel.ConnectionTime;
                else if (m_dataChannel != null)
                    return m_dataChannel.ConnectionTime;
                else if (m_serverBasedDataChannel != null)
                    return m_serverBasedDataChannel.RunTime;

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the <see cref="MultiProtocolFrameParser"/> is currently enabled.
        /// </summary>
        /// <remarks>
        /// Setting <see cref="Enabled"/> to true will start the <see cref="MultiProtocolFrameParser"/> if it is not started,
        /// setting to false will stop the <see cref="MultiProtocolFrameParser"/> if it is started.
        /// </remarks>
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                if (value && !m_enabled)
                    Start();
                else if (!value && m_enabled)
                    Stop();
            }
        }

        /// <summary>
        /// Gets the total number of buffers that are currently queued for processing, if any.
        /// </summary>
        public int QueuedBuffers
        {
            get
            {
                if (m_frameParser != null)
                    return m_frameParser.QueuedBuffers;

                return 0;
            }
        }

        /// <summary>
        /// Gets a boolean value that determines if data channel is defined as a server based connection.
        /// </summary>
        public bool DataChannelIsServerBased
        {
            get
            {
                if (m_dataChannel != null)
                    return false;

                if (m_serverBasedDataChannel == null)
                {
                    // No connection is currently active, see if connection string defines a server based connection
                    if (!string.IsNullOrEmpty(m_connectionString))
                    {
                        Dictionary<string, string> settings = m_connectionString.ParseKeyValuePairs();
                        string setting;

                        if (settings.TryGetValue("islistener", out setting))
                            return setting.ParseBoolean();

                        return false;
                    }

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets total number of frames that have been received from a device so far.
        /// </summary>
        public long TotalFramesReceived
        {
            get
            {
                return m_totalFramesReceived;
            }
        }

        /// <summary>
        /// Gets total number of bytes that have been received from a device so far.
        /// </summary>
        public long TotalBytesReceived
        {
            get
            {
                return m_totalBytesReceived;
            }
        }

        /// <summary>
        /// Gets total number of frames that were missing from device so far.
        /// </summary>
        public long TotalMissingFrames
        {
            get
            {
                return m_totalMissingFrames;
            }
        }

        /// <summary>
        /// Gets total number of CRC exceptions encountered from device so far.
        /// </summary>
        public long TotalCrcExceptions
        {
            get
            {
                return m_totalCrcExceptions;
            }
        }

        /// <summary>
        /// Gets the configured frame rate as reported by the connected device.
        /// </summary>
        public long ConfiguredFrameRate
        {
            get
            {
                return m_configuredFrameRate;
            }
        }

        /// <summary>
        /// Gets the calculated frame rate (i.e., frames per second) based on data received from device connection.
        /// </summary>
        public double CalculatedFrameRate
        {
            get
            {
                return m_calculatedFrameRate;
            }
        }

        /// <summary>
        /// Gets the calculated byte rate (i.e., bytes per second) based on data received from device connection.
        /// </summary>
        public double ByteRate
        {
            get
            {
                return m_calculatedByteRate;
            }
        }

        /// <summary>
        /// Gets the calculated bit rate (i.e., bits per second (bps)) based on data received from device connection.
        /// </summary>
        public double BitRate
        {
            get
            {
                return m_calculatedByteRate * 8.0D;
            }
        }

        /// <summary>
        /// Gets the calculated megabits per second (Mbps) rate based on data received from device connection.
        /// </summary>
        public double MegaBitRate
        {
            get
            {
                return BitRate / SI2.Mega;
            }
        }

        /// <summary>
        /// Gets a descriptive name for a device connection that includes <see cref="SourceName"/>, if provided.
        /// </summary>
        public string Name
        {
            get
            {

                if (string.IsNullOrEmpty(m_sourceName))
                    return "ID " + m_deviceID + " using " + m_phasorProtocol.GetFormattedProtocolName() + " over " + m_transportProtocol;
                else
                    return m_sourceName + " (" + m_deviceID + ")";
            }
        }

        /// <summary>
        /// Gets current descriptive status of the <see cref="MultiProtocolFrameParser"/>.
        /// </summary>
        public string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat("      Device Connection ID: {0}", m_deviceID);
                status.AppendLine();
                status.AppendFormat("           Phasor protocol: {0}", m_phasorProtocol.GetFormattedProtocolName());
                status.AppendLine();
                status.AppendFormat("           Connection type: {0}", ConnectionType);
                status.AppendLine();
                status.AppendFormat("               Buffer size: {0}", m_bufferSize);
                status.AppendLine();
                status.AppendFormat("     Total frames received: {0}", m_totalFramesReceived);
                status.AppendLine();
                status.AppendFormat("      Total missing frames: {0}", m_totalMissingFrames);
                status.AppendLine();
                status.AppendFormat("      Total CRC exceptions: {0}", m_totalCrcExceptions);
                status.AppendLine();
                status.AppendFormat("     Calculated frame rate: {0}", m_calculatedFrameRate);
                status.AppendLine();
                status.AppendFormat("      Calculated data rate: {0} bytes/sec, {1} Mbps", m_calculatedByteRate.ToString("0.0"), MegaBitRate.ToString("0.0000"));
                status.AppendLine();
                status.AppendFormat("Allowed parsing exceptions: {0}", m_allowedParsingExceptions);
                status.AppendLine();
                status.AppendFormat("  Parsing exception window: {0} seconds", m_parsingExceptionWindow.ToSeconds().ToString("0.00"));
                status.AppendLine();

                if (m_frameParser != null)
                    status.Append(m_frameParser.Status);

                if (m_dataChannel != null)
                    status.Append(m_dataChannel.Status);

                if (m_serverBasedDataChannel != null)
                    status.Append(m_serverBasedDataChannel.Status);

                if (m_commandChannel != null)
                    status.Append(m_commandChannel.Status);

                return status.ToString();
            }
        }

        /// <summary>
        /// Gets the connection type (Active, Passive or Hybrid) based on defined channels and transport selections.
        /// </summary>
        public string ConnectionType
        {
            get
            {
                switch (m_transportProtocol)
                {
                    case TransportProtocol.Tcp:
                    case TransportProtocol.Serial:
                        return "Active";
                    case TransportProtocol.Udp:
                    case TransportProtocol.File:
                        if (m_commandChannel != null)
                            return "Hybrid";
                        return "Passive";
                    default:
                        return "Undetermined";
                }
            }
        }

        /// <summary>
        /// Gets or sets any connection specific <see cref="IConnectionParameters"/> that may be applicable for the current <see cref="PhasorProtocol"/>.
        /// </summary>
        public IConnectionParameters ConnectionParameters
        {
            get
            {
                return m_connectionParameters;
            }
            set
            {
                m_connectionParameters = value;

                // Pass new connection parameters along to derived frame parser if instantiated
                if (m_frameParser != null)
                    m_frameParser.ConnectionParameters = value;
            }
        }

        ///// <summary>
        ///// Gets or sets a reference to the active <see cref="IFrameParser"/>.
        ///// </summary>
        //internal IFrameParser FrameParser
        //{
        //    get
        //    {
        //        return m_frameParser;
        //    }
        //    set
        //    {
        //        m_frameParser = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets a reference to the <see cref="IClient"/> data channel.
        ///// </summary>
        //internal IClient DataChannel
        //{
        //    get
        //    {
        //        return m_dataChannel;
        //    }
        //    set
        //    {
        //        m_dataChannel = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets a reference to the <see cref="IServer"/> server based data channel.
        ///// </summary>
        //internal IServer ServerBasedDataChannel
        //{
        //    get
        //    {
        //        return m_serverBasedDataChannel;
        //    }
        //    set
        //    {
        //        m_serverBasedDataChannel = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets a reference to the <see cref="IClient"/> command channel.
        ///// </summary>
        //internal IClient CommandChannel
        //{
        //    get
        //    {
        //        return m_commandChannel;
        //    }
        //    set
        //    {
        //        m_commandChannel = value;
        //    }
        //}

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="MultiProtocolFrameParser"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MultiProtocolFrameParser"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        Stop();

                        if (m_inputTimer != null)
                        {
                            m_inputTimer.Tick -= m_inputTimer_Tick;
                            m_inputTimer.Dispose();
                        }
                        m_inputTimer = null;

                        if (m_rateCalcTimer != null)
                        {
                            m_rateCalcTimer.Elapsed -= m_rateCalcTimer_Elapsed;
                            m_rateCalcTimer.Dispose();
                        }
                        m_rateCalcTimer = null;

                        if (m_frameWaitHandle != null)
                            m_frameWaitHandle.Dispose();

                        m_frameWaitHandle = null;

                        // Clear minimum timer resolution.
                        PrecisionTimer.ClearMinimumTimerResolution(1);
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Starts the <see cref="MultiProtocolFrameParser"/>.
        /// </summary>
        public void Start()
        {
            // Stop parser if it is already running - thus calling start after already started will have the effect
            // of "restarting" the parsing engine...
            Stop();

            // Reset statistics...
            m_totalFramesReceived = 0;
            m_totalMissingFrames = 0;
            m_totalCrcExceptions = 0;
            m_frameRateTotal = 0;
            m_byteRateTotal = 0;
            m_totalBytesReceived = 0;
            m_calculatedFrameRate = 0.0D;
            m_calculatedByteRate = 0.0D;
            m_lastParsingExceptionTime = 0;
            m_parsingExceptionCount = 0;

            try
            {
                // Parse connection string to check for special parameters
                Dictionary<string, string> settings = m_connectionString.ParseKeyValuePairs();
                string setting;

                // Reset connection attempt counter
                m_connectionAttempts = 0;

                // Establish protocol specific frame parser
                InitializeFrameParser(settings);

                // Establish command channel connection, if defined...
                if (settings.TryGetValue("commandChannel", out setting))
                    InitializeCommandChannel(setting);

                // Establish data channel connection - must be defined.
                InitializeDataChannel(settings);

                m_rateCalcTimer.Enabled = true;
                m_enabled = true;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Stop();

                // Check for common error when using an IPv4 address on an IPv6 stack
                if (ex.ErrorCode == 10014)
                    OnConnectionException(new InvalidOperationException(string.Format("Bad IP address format in \"{0}\": {1}\r\n\r\nUse a DNS name or an IPv6 formatted IP address (e.g., ::1); otherwise, force IPv4 mode.", m_connectionString, ex.Message), ex), 1);
                else
                    OnConnectionException(new InvalidOperationException(string.Format("{0} in \"{1}\"", ex.Message, m_connectionString), ex), 1);
            }
            catch (Exception ex)
            {
                Stop();
                OnConnectionException(new InvalidOperationException(string.Format("{0} in \"{1}\"", ex.Message, m_connectionString), ex), 1);
            }
        }

        /// <summary>
        /// Initialize frame parser.
        /// </summary>
        /// <param name="settings">Key/value pairs dictionary parsed from connection string.</param>
        private void InitializeFrameParser(Dictionary<string, string> settings)
        {
            string setting;

            // Instantiate protocol specific frame parser
            switch (m_phasorProtocol)
            {
                case PhasorProtocol.IeeeC37_118V1:
                    m_frameParser = new IeeeC37_118.FrameParser(IeeeC37_118.DraftRevision.Draft7);
                    break;
                case PhasorProtocol.IeeeC37_118D6:
                    m_frameParser = new IeeeC37_118.FrameParser(IeeeC37_118.DraftRevision.Draft6);
                    break;
                case PhasorProtocol.Ieee1344:
                    m_frameParser = new Ieee1344.FrameParser();
                    break;
                case PhasorProtocol.BpaPdcStream:
                    m_frameParser = new BpaPdcStream.FrameParser();

                    // Check for BPA PDCstream protocol specific parameters in connection string
                    BpaPdcStream.ConnectionParameters bpaPdcParameters = m_connectionParameters as BpaPdcStream.ConnectionParameters;

                    if (bpaPdcParameters != null)
                    {
                        // INI file name setting is required
                        if (settings.TryGetValue("iniFileName", out setting))
                            bpaPdcParameters.ConfigurationFileName = FilePath.GetAbsolutePath(setting);
                        else if (string.IsNullOrEmpty(bpaPdcParameters.ConfigurationFileName))
                            throw new ArgumentException("BPA PDCstream INI filename setting (e.g., \"inifilename=DEVICE_PDC.ini\") was not found. This setting is required for BPA PDCstream protocol connections - frame parser initialization terminated.");

                        if (settings.TryGetValue("refreshConfigFileOnChange", out setting))
                            bpaPdcParameters.RefreshConfigurationFileOnChange = setting.ParseBoolean();

                        if (settings.TryGetValue("parseWordCountFromByte", out setting))
                            bpaPdcParameters.ParseWordCountFromByte = setting.ParseBoolean();
                    }
                    break;
                case PhasorProtocol.FNet:
                    m_frameParser = new FNet.FrameParser();

                    // Check for F-NET protocol specific parameters in connection string
                    FNet.ConnectionParameters fnetParameters = m_connectionParameters as FNet.ConnectionParameters;

                    if (fnetParameters != null)
                    {
                        if (settings.TryGetValue("timeOffset", out setting))
                            fnetParameters.TimeOffset = long.Parse(setting);

                        if (settings.TryGetValue("stationName", out setting))
                            fnetParameters.StationName = setting;

                        if (settings.TryGetValue("frameRate", out setting))
                            fnetParameters.FrameRate = ushort.Parse(setting);

                        if (settings.TryGetValue("nominalFrequency", out setting))
                            fnetParameters.NominalFrequency = (LineFrequency)int.Parse(setting);
                    }
                    break;
                case PhasorProtocol.SelFastMessage:
                    m_frameParser = new SelFastMessage.FrameParser();

                    // Check for SEL Fast Message protocol specific parameters in connection string
                    SelFastMessage.ConnectionParameters selParameters = m_connectionParameters as SelFastMessage.ConnectionParameters;

                    if (selParameters != null)
                    {
                        if (settings.TryGetValue("messagePeriod", out setting))
                            selParameters.MessagePeriod = (SelFastMessage.MessagePeriod)Enum.Parse(typeof(SelFastMessage.MessagePeriod), setting, true);
                    }
                    break;
                case PhasorProtocol.Macrodyne:
                    m_frameParser = new Macrodyne.FrameParser();
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Phasor protocol \"{0}\" is not recognized, failed to initialize frame parser", m_phasorProtocol));
            }

            // Assign frame parser properties
            m_frameParser.ConnectionParameters = m_connectionParameters;
            m_frameParser.ExecuteParseOnSeparateThread = m_executeParseOnSeparateThread;

            // Setup event handlers
            m_frameParser.ReceivedCommandFrame += m_frameParser_ReceivedCommandFrame;
            m_frameParser.ReceivedConfigurationFrame += m_frameParser_ReceivedConfigurationFrame;
            m_frameParser.ReceivedDataFrame += m_frameParser_ReceivedDataFrame;
            m_frameParser.ReceivedHeaderFrame += m_frameParser_ReceivedHeaderFrame;
            m_frameParser.ReceivedUndeterminedFrame += m_frameParser_ReceivedUndeterminedFrame;
            m_frameParser.ReceivedFrameBufferImage += m_frameParser_ReceivedFrameBufferImage;
            m_frameParser.ConfigurationChanged += m_frameParser_ConfigurationChanged;
            m_frameParser.ParsingException += m_frameParser_ParsingException;

            // Start parsing engine
            m_frameParser.Start();
        }

        /// <summary>
        /// Initialize command channel.
        /// </summary>
        /// <param name="connectionString">Command channel connection string.</param>
        private void InitializeCommandChannel(string connectionString)
        {
            // Parse command channel connection settings
            TransportProtocol transportProtocol;
            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
            string setting;

            // Verify user did not attempt to setup command channel as a TCP server
            if (settings.ContainsKey("islistener") && settings["islistener"].ParseBoolean())
                throw new ArgumentException("Command channel cannot be setup as a TCP server.");

            // Determine what transport protocol user selected
            if (settings.TryGetValue("transportProtocol", out setting) || settings.TryGetValue("protocol", out setting))
            {
                transportProtocol = (TransportProtocol)Enum.Parse(typeof(TransportProtocol), setting, true);

                // The communications engine only recognizes the transport protocol key as "protocol"
                connectionString = connectionString.ReplaceCaseInsensitive("transportProtocol", "protocol");
            }
            else
                throw new ArgumentException("No transport protocol was specified for command channel. For example: \"transportProtocol=Tcp\".");

            // Validate command channel transport protocol selection
            if (transportProtocol != TransportProtocol.Tcp && transportProtocol != TransportProtocol.Serial && transportProtocol != TransportProtocol.File)
                throw new ArgumentException("Command channel transport protocol can only be defined as TCP, Serial or File");

            // Instantiate command channel based on defined transport layer
            m_commandChannel = ClientBase.Create(connectionString);

            // Setup event handlers
            m_commandChannel.ConnectionEstablished += m_commandChannel_ConnectionEstablished;
            m_commandChannel.ConnectionAttempt += m_commandChannel_ConnectionAttempt;
            m_commandChannel.ConnectionException += m_commandChannel_ConnectionException;
            m_commandChannel.ConnectionTerminated += m_commandChannel_ConnectionTerminated;

            // Attempt connection to device over command channel
            m_commandChannel.ReceiveDataHandler = Write;
            m_commandChannel.ReceiveBufferSize = m_bufferSize;
            m_commandChannel.MaxConnectionAttempts = m_maximumConnectionAttempts;
            m_commandChannel.Handshake = false;
            m_commandChannel.ConnectAsync();
        }

        /// <summary>
        /// Initialize data channel.
        /// </summary>
        /// <param name="settings">Key/value pairs dictionary parsed from connection string.</param>
        private void InitializeDataChannel(Dictionary<string, string> settings)
        {
            string setting;

            // Instantiate selected transport layer
            switch (m_transportProtocol)
            {
                case TransportProtocol.Tcp:
                    // The TCP transport may be set up as a server or as a client, we distinguish
                    // this simply by deriving the value of an added key/value pair in the
                    // connection string called "IsListener"
                    if (settings.TryGetValue("islistener", out setting))
                    {
                        if (setting.ParseBoolean())
                            m_serverBasedDataChannel = new TcpServer();
                        else
                            m_dataChannel = new TcpClient();
                    }
                    else
                    {
                        // If the key doesn't exist, we assume it's a client connection
                        m_dataChannel = new TcpClient();
                    }
                    break;
                case TransportProtocol.Udp:
                    m_dataChannel = new UdpClient();
                    break;
                case TransportProtocol.Serial:
                    m_dataChannel = new SerialClient();
                    break;
                case TransportProtocol.File:
                    // For file based playback, we allow the option of auto-repeat
                    FileClient fileClient = new FileClient();

                    fileClient.FileOpenMode = FileMode.Open;
                    fileClient.FileAccessMode = FileAccess.Read;
                    fileClient.FileShareMode = FileShare.Read;
                    fileClient.AutoRepeat = m_autoRepeatCapturedPlayback;

                    m_dataChannel = fileClient;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Transport protocol \"{0}\" is not recognized, failed to initialize data channel", m_transportProtocol));
            }

            // Handle primary data connection, this *must* be defined...
            if (m_dataChannel != null)
            {
                // Setup event handlers
                m_dataChannel.ConnectionEstablished += m_dataChannel_ConnectionEstablished;
                m_dataChannel.ConnectionAttempt += m_dataChannel_ConnectionAttempt;
                m_dataChannel.ConnectionException += m_dataChannel_ConnectionException;
                m_dataChannel.ConnectionTerminated += m_dataChannel_ConnectionTerminated;

                // Attempt connection to device
                m_dataChannel.ReceiveDataHandler = Write;
                m_dataChannel.ReceiveBufferSize = m_bufferSize;
                m_dataChannel.ConnectionString = m_connectionString;
                m_dataChannel.MaxConnectionAttempts = m_maximumConnectionAttempts;
                m_dataChannel.Handshake = false;
                m_dataChannel.ConnectAsync();
            }
            else if (m_serverBasedDataChannel != null)
            {
                // Setup event handlers
                m_serverBasedDataChannel.ClientConnected += m_serverBasedDataChannel_ClientConnected;
                m_serverBasedDataChannel.ClientDisconnected += m_serverBasedDataChannel_ClientDisconnected;
                m_serverBasedDataChannel.ServerStarted += m_serverBasedDataChannel_ServerStarted;
                m_serverBasedDataChannel.ServerStopped += m_serverBasedDataChannel_ServerStopped;

                // Listen for device connection
                m_serverBasedDataChannel.ReceiveClientDataHandler = Write;
                m_serverBasedDataChannel.ReceiveBufferSize = m_bufferSize;
                m_serverBasedDataChannel.ConfigurationString = m_connectionString;
                m_serverBasedDataChannel.MaxClientConnections = 1;
                m_serverBasedDataChannel.Handshake = false;
                m_serverBasedDataChannel.Start();
            }
            else
                throw new InvalidOperationException("No data channel was initialized, cannot start frame parser");
        }

        /// <summary>
        /// Stops the <see cref="MultiProtocolFrameParser"/>.
        /// </summary>
        public void Stop()
        {
            m_enabled = false;
            m_rateCalcTimer.Enabled = false;
            m_lastFrameReceivedTime = 0;
            m_configurationFrame = null;

            // Make sure data stream is disabled
            if (!m_skipDisableRealTimeData)
                SendDeviceCommand(DeviceCommand.DisableRealTimeData);

            if (m_dataChannel != null)
            {
                try
                {
                    m_dataChannel.Disconnect();
                }
                catch (Exception ex)
                {
                    OnParsingException(ex, "Failed to properly disconnect data channel: {0}", ex.Message);
                }
                finally
                {
                    m_dataChannel.ReceiveDataHandler = null;
                    m_dataChannel.ConnectionEstablished -= m_dataChannel_ConnectionEstablished;
                    m_dataChannel.ConnectionAttempt -= m_dataChannel_ConnectionAttempt;
                    m_dataChannel.ConnectionException -= m_dataChannel_ConnectionException;
                    m_dataChannel.ConnectionTerminated -= m_dataChannel_ConnectionTerminated;
                    m_dataChannel.Dispose();
                }
            }
            m_dataChannel = null;

            if (m_serverBasedDataChannel != null)
            {
                try
                {
                    m_serverBasedDataChannel.DisconnectAll();
                }
                catch (Exception ex)
                {
                    OnParsingException(ex, "Failed to properly disconnect server based data channel: {0}", ex.Message);
                }
                finally
                {
                    m_serverBasedDataChannel.ReceiveClientDataHandler = null;
                    m_serverBasedDataChannel.ClientConnected -= m_serverBasedDataChannel_ClientConnected;
                    m_serverBasedDataChannel.ClientDisconnected -= m_serverBasedDataChannel_ClientDisconnected;
                    m_serverBasedDataChannel.ServerStarted -= m_serverBasedDataChannel_ServerStarted;
                    m_serverBasedDataChannel.ServerStopped -= m_serverBasedDataChannel_ServerStopped;
                    m_serverBasedDataChannel.Dispose();
                }
            }
            m_serverBasedDataChannel = null;

            if (m_commandChannel != null)
            {
                try
                {
                    m_commandChannel.Disconnect();
                }
                catch (Exception ex)
                {
                    OnParsingException(ex, "Failed to properly disconnect command channel: {0}", ex.Message);
                }
                finally
                {
                    m_commandChannel.ReceiveDataHandler = null;
                    m_commandChannel.ConnectionEstablished -= m_commandChannel_ConnectionEstablished;
                    m_commandChannel.ConnectionAttempt -= m_commandChannel_ConnectionAttempt;
                    m_commandChannel.ConnectionException -= m_commandChannel_ConnectionException;
                    m_commandChannel.ConnectionTerminated -= m_commandChannel_ConnectionTerminated;
                    m_commandChannel.Dispose();
                }
            }
            m_commandChannel = null;

            if (m_frameParser != null)
            {
                try
                {
                    m_frameParser.Stop();
                }
                catch (Exception ex)
                {
                    OnParsingException(ex, "Failed to properly stop frame parser: {0}", ex.Message);
                }
                finally
                {
                    m_frameParser.ReceivedCommandFrame -= m_frameParser_ReceivedCommandFrame;
                    m_frameParser.ReceivedConfigurationFrame -= m_frameParser_ReceivedConfigurationFrame;
                    m_frameParser.ReceivedDataFrame -= m_frameParser_ReceivedDataFrame;
                    m_frameParser.ReceivedHeaderFrame -= m_frameParser_ReceivedHeaderFrame;
                    m_frameParser.ReceivedUndeterminedFrame -= m_frameParser_ReceivedUndeterminedFrame;
                    m_frameParser.ReceivedFrameBufferImage -= m_frameParser_ReceivedFrameBufferImage;
                    m_frameParser.ConfigurationChanged -= m_frameParser_ConfigurationChanged;
                    m_frameParser.ParsingException -= m_frameParser_ParsingException;
                    m_frameParser.Dispose();
                }
            }
            m_frameParser = null;

            #if RawDataCapture
            if (m_rawDataCapture != null)
                m_rawDataCapture.Close();
            m_rawDataCapture = null;
            #endif
        }

        /// <summary>
        /// Sends the specified <see cref="DeviceCommand"/> to the remote device.
        /// </summary>
        /// <param name="command"><see cref="DeviceCommand"/> to send to the remote device.</param>
        /// <remarks>
        /// Command will only be sent if <see cref="DeviceSupportsCommands"/> is <c>true</c> and <see cref="MultiProtocolFrameParser"/>.
        /// </remarks>
        /// <returns>A <see cref="WaitHandle"/>.</returns>
        public WaitHandle SendDeviceCommand(DeviceCommand command)
        {
            WaitHandle handle = null;

            try
            {
                if (m_deviceSupportsCommands && (m_dataChannel != null || m_serverBasedDataChannel != null || m_commandChannel != null))
                {
                    ICommandFrame commandFrame;

                    // Only the IEEE, SEL Fast Message and Macrodyne protocols support commands
                    switch (m_phasorProtocol)
                    {
                        case PhasorProtocols.PhasorProtocol.IeeeC37_118V1:
                        case PhasorProtocols.PhasorProtocol.IeeeC37_118D6:
                            commandFrame = new IeeeC37_118.CommandFrame(m_deviceID, command, 1);
                            break;
                        case PhasorProtocols.PhasorProtocol.Ieee1344:
                            commandFrame = new Ieee1344.CommandFrame(m_deviceID, command);
                            break;
                        case PhasorProtocols.PhasorProtocol.SelFastMessage:
                            // Get defined message period
                            SelFastMessage.MessagePeriod messagePeriod = SelFastMessage.MessagePeriod.DefaultRate;
                            SelFastMessage.ConnectionParameters connectionParameters = m_connectionParameters as SelFastMessage.ConnectionParameters;

                            if (connectionParameters != null)
                                messagePeriod = connectionParameters.MessagePeriod;

                            commandFrame = new SelFastMessage.CommandFrame(command, messagePeriod);
                            break;
                        case PhasorProtocols.PhasorProtocol.Macrodyne:
                            commandFrame = new Macrodyne.CommandFrame(command);
                            break;
                        default:
                            commandFrame = null;
                            break;
                    }

                    if (commandFrame != null)
                    {
                        byte[] buffer = commandFrame.BinaryImage;

                        // Send command over appropriate communications channel - command channel, if defined,
                        // will take precedence over other communications channels for command traffic...
                        if (m_commandChannel != null && m_commandChannel.CurrentState == ClientState.Connected)
                            handle = m_commandChannel.SendAsync(buffer, 0, buffer.Length);
                        else if (m_dataChannel != null && m_dataChannel.CurrentState == ClientState.Connected)
                            handle = m_dataChannel.SendAsync(buffer, 0, buffer.Length);
                        else if (m_serverBasedDataChannel != null && m_serverBasedDataChannel.CurrentState == ServerState.Running)
                        {
                            WaitHandle[] handles = m_serverBasedDataChannel.MulticastAsync(buffer, 0, buffer.Length);

                            if (handles != null && handles.Length > 0)
                                handle = handles[0];
                        }

                        if (SentCommandFrame != null)
                            SentCommandFrame(this, new EventArgs<ICommandFrame>(commandFrame));
                    }
                }
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "Failed to send device command \"{0}\": {1}", command, ex.Message);
            }

            return handle;
        }        

        /// <summary>
        /// Writes data directly to the frame parsing engine buffer.
        /// </summary>
        /// <remarks>
        /// This method allows consumer to "manually send extra data" to the parsing engine to be parsed, if desired.
        /// </remarks>
        /// <param name="buffer">Buffer containing data to be parsed.</param>
        /// <param name="offset">Offset into buffer where data begins.</param>
        /// <param name="count">Length of data in buffer to be parsed.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            // This is the delegate implementation used by the communication source for reception
            // of data directly from the socket (i.e., ReceiveDataHandler) that is used for a
            // speed boost in communications processing...

            #if RawDataCapture
            if (m_rawDataCapture == null)
                m_rawDataCapture = new FileStream(FilePath.GetAbsolutePath("RawData.Capture"), FileMode.Create);
            m_rawDataCapture.Write(buffer, offset, count);
            #endif

            // Pass data from communications client into protocol specific frame parser
            m_frameParser.Write(buffer, offset, count);
            
            m_byteRateTotal += count;

            if (m_initiatingDataStream)
                m_initialBytesReceived += count;
        }

        // Data received from a server will include a client ID - since in our case
        // the server will only host a single device, we ignore this ID
        private void Write(Guid clientID, byte[] buffer, int offset, int count)
        {
            Write(buffer, offset, count);
        }

        /// <summary>
        /// Raises the <see cref="ParsingException"/> event.
        /// </summary>
        /// <param name="ex">Exception to send to <see cref="ParsingException"/> event.</param>
        private void OnParsingException(Exception ex)
        {
            if (ParsingException != null && !(ex is ThreadAbortException))
                ParsingException(this, new EventArgs<Exception>(ex));

            if (DateTime.Now.Ticks - m_lastParsingExceptionTime > m_parsingExceptionWindow)
            {
                // Exception window has passed since last exception, so we reset counters
                m_lastParsingExceptionTime = DateTime.Now.Ticks;
                m_parsingExceptionCount = 0;
            }

            m_parsingExceptionCount++;

            if (m_parsingExceptionCount > m_allowedParsingExceptions)
            {
                try
                {
                    // When the parsing exception threshold has been exceeded, connection is stopped
                    Stop();
                }
                finally
                {
                    // Notify consumer of parsing exception threshold deviation
                    OnExceededParsingExceptionThreshold();
                    m_lastParsingExceptionTime = 0;
                    m_parsingExceptionCount = 0;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ParsingException"/> event.
        /// </summary>
        /// <param name="innerException">Actual exception to send as inner exception to <see cref="ParsingException"/> event.</param>
        /// <param name="message">Message of new exception to send to <see cref="ParsingException"/> event.</param>
        /// <param name="args">Arguments of message of new exception to send to <see cref="ParsingException"/> event.</param>
        private void OnParsingException(Exception innerException, string message, params object[] args)
        {
            if (!(innerException is ThreadAbortException))
                OnParsingException(new Exception(string.Format(message, args), innerException));
        }

        /// <summary>
        /// Raises the <see cref="ExceededParsingExceptionThreshold"/> event.
        /// </summary>
        private void OnExceededParsingExceptionThreshold()
        {
            if (ExceededParsingExceptionThreshold != null)
                ExceededParsingExceptionThreshold(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ConnectionException"/> event.
        /// </summary>
        /// <param name="ex">Exception to raise.</param>
        /// <param name="connectionAttempts">Number of connection attempts to report.</param>
        private void OnConnectionException(Exception ex, int connectionAttempts)
        {
            if (ConnectionException != null && !(ex is ThreadAbortException))
                ConnectionException(this, new EventArgs<Exception, int>(ex, connectionAttempts));
        }

        /// <summary>
        /// Derives a flag based on settings that determines if the current connection supports device commands.
        /// </summary>
        /// <returns>Derived flag that determines if the current connection supports device commands.</returns>
        private bool DeriveCommandSupport()
        {
            // Command support is based on phasor protocol, transport protocol and connection style
            if (IsIEEEProtocol || m_phasorProtocol == PhasorProtocol.SelFastMessage || m_phasorProtocol == PhasorProtocol.Macrodyne)
            {
                // IEEE protocols using TCP or Serial connection support device commands
                if (m_transportProtocol == TransportProtocol.Tcp || m_transportProtocol == TransportProtocol.Serial)
                    return true;

                if (!string.IsNullOrEmpty(m_connectionString))
                {
                    Dictionary<string, string> settings = m_connectionString.ParseKeyValuePairs();

                    // A defined command channel inherently means commands are supported
                    if (settings.ContainsKey("commandchannel"))
                    {
                        return true;
                    }
                    else if (m_transportProtocol == TransportProtocol.Udp)
                    {
                        // IEEE protocols "can" use UDP connection to support devices commands, but only
                        // when remote device acts as a UDP listener (i.e., a "server" connection)
                        return settings.ContainsKey("server");
                    }
                }
            }

            return false;
        }

        // Starts data parsing sequence.
        private void StartDataParsingSequence(object state)
        {
            // This thread pool delegate is used to start streaming data on a remote device.
            int attempts = 0;

            // Some devices will only send a config frame once data streaming has been disabled, so
            // we use this code to disable real-time data and wait for data to stop streaming...
            try
            {
                if (!m_skipDisableRealTimeData)
                {
                    // Make sure data stream is disabled
                    WaitHandle handle = SendDeviceCommand(DeviceCommand.DisableRealTimeData);
                    if (handle != null)
                        handle.WaitOne();

                    // Allow device time to receive and process command before sending another
                    Thread.Sleep(1000);

                    // Wait for real-time data stream to cease for up to two seconds
                    while (m_initialBytesReceived > 0)
                    {
                        m_initialBytesReceived = 0;
                        Thread.Sleep(100);

                        attempts++;
                        if (attempts >= 20)
                            break;
                    }
                }
            }
            finally
            {
                m_initiatingDataStream = false;
            }

            // Request configuration frame once real-time data has been disabled. Note that data stream
            // will be enabled when we receive a configuration frame. 
            switch (m_phasorProtocol)
            {
                case PhasorProtocol.SelFastMessage:
                    // SEL Fast Message doesn't define a binary configuration frame so we skip
                    // requesting one and jump straight to enabling the data stream.
                    SendDeviceCommand(DeviceCommand.EnableRealTimeData);
                    break;
                case PhasorProtocol.Macrodyne:
                    // We collect the station name (i.e. the unit ID) from the Macrodyne
                    // protocol as a header frame before we get the configuration frame
                    SendDeviceCommand(DeviceCommand.SendHeaderFrame);
                    break;
                default:
                    // Otherwise we just rquest the configuration frame
                    SendDeviceCommand(DeviceCommand.SendConfigurationFrame2);
                    break;
            }
        }

        // Calculate frame and data rates
        private void m_rateCalcTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            double time = Ticks.ToSeconds(DateTime.Now.Ticks - m_dataStreamStartTime);

            m_calculatedFrameRate = (double)m_frameRateTotal / time;
            m_calculatedByteRate = (double)m_byteRateTotal / time;

            m_totalMissingFrames += (long)(m_configuredFrameRate * m_rateCalcTimer.Interval * SI.Milli) - m_frameRateTotal;
            m_totalFramesReceived += m_frameRateTotal;
            m_totalBytesReceived += m_byteRateTotal;

            m_frameRateTotal = 0;
            m_byteRateTotal = 0;
            m_dataStreamStartTime = DateTime.Now.Ticks;
        }

        // Handles needed start-up actions once a client is connected
        private void ClientConnectedHandler()
        {
            if (ConnectionEstablished != null)
                ConnectionEstablished(this, EventArgs.Empty);

            // Begin data parsing sequence to handle reception of configuration frame
            if (m_deviceSupportsCommands && m_autoStartDataParsingSequence)
            {
                m_initialBytesReceived = 0;
                m_initiatingDataStream = true;
                ThreadPool.QueueUserWorkItem(StartDataParsingSequence, null);
            }
        }

        private void MaintainCapturedFrameReplayTiming(IFrame sourceFrame)
        {
            if (m_inputTimer == null)
            {
                if (m_lastFrameReceivedTime > 0)
                {
                    // To maintain timing on "frames per second", we wait for defined frame rate interval
                    double sleepTime = (1.0D / m_definedFrameRate) - ((double)(DateTime.UtcNow.Ticks - m_lastFrameReceivedTime) / (double)Ticks.PerSecond);

                    // Thread sleep time is a minimum suggested sleep time depending on system activity, so we target 9/10 of a second
                    // to make this a little more accurate. Since this is just used for replay, getting close is good enough - no need
                    // to incur the overhead of using a PrecisionTimer here unless requested
                    if (sleepTime > 0)
                        Thread.Sleep((int)(sleepTime * 900.0D));
                }

                m_lastFrameReceivedTime = DateTime.UtcNow.Ticks;
            }
            else
            {
                // When high resolution input timing is requested, we only need to wait for the next signal...
                m_frameWaitHandle.WaitOne();
            }
            
            // If injecting a simulated timestamp, use the last received time
            if (m_injectSimulatedTimestamp)
                sourceFrame.Timestamp = m_lastFrameReceivedTime;
        }

        // This timer function is called every millisecond so that frames can be published at the exact desired time 
        void m_inputTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = PrecisionTimer.UtcNow;
            long ticks = now.Ticks;
            int milliseconds = now.Millisecond;
            long baseTicks, ticksBeyondSecond, frameIndex;

            // Baseline timestamp to the top of the second
            baseTicks = ticks - ticks % Ticks.PerSecond;

            // Remove the seconds from ticks
            ticksBeyondSecond = ticks - baseTicks;

            // Calculate a frame index between 0 and m_definedFrameRate - 1, corresponding to ticks
            // rounded down to the nearest frame
            frameIndex = (long)(ticksBeyondSecond / m_ticksPerFrame);

            // See if it is time to publish
            if ((int)(1.0D / m_definedFrameRate * (frameIndex * 1000.0D)) == milliseconds)
            {
                // Baseline timestamp to the top of the millisecond for frame publication
                m_lastFrameReceivedTime = ticks - ticks % Ticks.PerMillisecond;

                // Release wait handle
                m_frameWaitHandle.Set();
            }
        }

        #region [ Data Channel Event Handlers ]

        private void m_dataChannel_ConnectionEstablished(object sender, EventArgs e)
        {
            // Only handle client connection from data channel when command channel is undefined
            if (!(m_commandChannel != null && m_commandChannel.CurrentState == ClientState.Connected))
                ClientConnectedHandler();
        }

        private void m_dataChannel_ConnectionAttempt(object sender, EventArgs e)
        {
            m_connectionAttempts++;

            if (ConnectionAttempt != null)
                ConnectionAttempt(this, EventArgs.Empty);
        }

        private void m_dataChannel_ConnectionException(object sender, EventArgs<Exception> e)
        {
            OnConnectionException(e.Argument, m_connectionAttempts);
        }

        private void m_dataChannel_ConnectionTerminated(object sender, EventArgs e)
        {
            if (ConnectionTerminated != null)
                ConnectionTerminated(this, EventArgs.Empty);
        }

        #endregion

        #region [ Server Based Data Channel Event Handlers ]

        private void m_serverBasedDataChannel_ClientConnected(object sender, EventArgs<Guid> e)
        {
            ClientConnectedHandler();
        }

        private void m_serverBasedDataChannel_ClientDisconnected(object sender, EventArgs<Guid> e)
        {
            if (ConnectionTerminated != null)
                ConnectionTerminated(this, EventArgs.Empty);
        }

        private void m_serverBasedDataChannel_ServerStarted(object sender, EventArgs e)
        {
            if (ServerStarted != null)
                ServerStarted(this, EventArgs.Empty);
        }

        private void m_serverBasedDataChannel_ServerStopped(object sender, EventArgs e)
        {
            if (ServerStopped != null)
                ServerStopped(this, EventArgs.Empty);
        }

        #endregion

        #region [ Command Channel Event Handlers ]

        private void m_commandChannel_ConnectionEstablished(object sender, EventArgs e)
        {
            ClientConnectedHandler();
        }

        private void m_commandChannel_ConnectionAttempt(object sender, EventArgs e)
        {
            m_connectionAttempts++;

            if (ConnectionAttempt != null)
                ConnectionAttempt(this, EventArgs.Empty);
        }

        private void m_commandChannel_ConnectionException(object sender, EventArgs<Exception> e)
        {
            OnConnectionException(e.Argument, m_connectionAttempts);
        }

        private void m_commandChannel_ConnectionTerminated(object sender, EventArgs e)
        {
            if (ConnectionTerminated != null)
                ConnectionTerminated(this, EventArgs.Empty);
        }

        #endregion

        #region [ Frame Parser Event Handlers ]

        private void m_frameParser_ReceivedCommandFrame(object sender, EventArgs<ICommandFrame> e)
        {
            m_frameRateTotal++;

            // We don't stop parsing for exceptions thrown in consumer event handlers
            try
            {
                if (m_transportProtocol == TransportProtocol.File)
                    MaintainCapturedFrameReplayTiming(e.Argument);
                else if (m_injectSimulatedTimestamp)
                    m_lastFrameReceivedTime = PrecisionTimer.UtcNow.Ticks;

                if (ReceivedCommandFrame != null)
                    ReceivedCommandFrame(this, e);
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "MultiProtocolFrameParser \"ReceivedCommandFrame\" consumer event handler exception: {0}", ex.Message);
            }
        }

        private void m_frameParser_ReceivedConfigurationFrame(object sender, EventArgs<IConfigurationFrame> e)
        {
            // We automatically request enabling of real-time data upon reception of config frame if requested. Note that SEL Fast Message will
            // have already been enabled at this point so we don't duplicate request for enabling real-time data stream
            if (m_configurationFrame == null && m_deviceSupportsCommands && m_autoStartDataParsingSequence && m_phasorProtocol != PhasorProtocol.SelFastMessage)
                SendDeviceCommand(DeviceCommand.EnableRealTimeData);

            m_frameRateTotal++;
            m_configurationFrame = e.Argument;

            // We don't stop parsing for exceptions thrown in consumer event handlers
            try
            {
                if (m_transportProtocol == TransportProtocol.File)
                    MaintainCapturedFrameReplayTiming(e.Argument);
                else if (m_injectSimulatedTimestamp)
                    m_lastFrameReceivedTime = PrecisionTimer.UtcNow.Ticks;

                if (ReceivedConfigurationFrame != null)
                    ReceivedConfigurationFrame(this, e);

                if (m_configurationFrame != null)
                    m_configuredFrameRate = m_configurationFrame.FrameRate;
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "MultiProtocolFrameParser \"ReceivedConfigurationFrame\" consumer event handler exception: {0}", ex.Message);
            }
        }

        private void m_frameParser_ReceivedDataFrame(object sender, EventArgs<IDataFrame> e)
        {
            m_frameRateTotal++;

            // We don't stop parsing for exceptions thrown in consumer event handlers
            try
            {
                if (m_transportProtocol == TransportProtocol.File)
                    MaintainCapturedFrameReplayTiming(e.Argument);
                else if (m_injectSimulatedTimestamp)
                    m_lastFrameReceivedTime = PrecisionTimer.UtcNow.Ticks;

                if (ReceivedDataFrame != null)
                    ReceivedDataFrame(this, e);
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "MultiProtocolFrameParser \"ReceivedDataFrame\" consumer event handler exception: {0}", ex.Message);
            }
        }

        private void m_frameParser_ReceivedHeaderFrame(object sender, EventArgs<IHeaderFrame> e)
        {
            // Macrodyne receives header frame which contains station name before configuration frame
            if (m_configurationFrame == null && m_phasorProtocol == PhasorProtocol.Macrodyne)
                SendDeviceCommand(DeviceCommand.SendConfigurationFrame2);

            m_frameRateTotal++;

            // We don't stop parsing for exceptions thrown in consumer event handlers
            try
            {
                if (m_transportProtocol == TransportProtocol.File)
                    MaintainCapturedFrameReplayTiming(e.Argument);
                else if (m_injectSimulatedTimestamp)
                    m_lastFrameReceivedTime = PrecisionTimer.UtcNow.Ticks;

                if (ReceivedHeaderFrame != null)
                    ReceivedHeaderFrame(this, e);
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "MultiProtocolFrameParser \"ReceivedHeaderFrame\" consumer event handler exception: {0}", ex.Message);
            }
        }

        private void m_frameParser_ReceivedUndeterminedFrame(object sender, EventArgs<IChannelFrame> e)
        {
            m_frameRateTotal++;

            // We don't stop parsing for exceptions thrown in consumer event handlers
            try
            {
                if (m_transportProtocol == TransportProtocol.File)
                    MaintainCapturedFrameReplayTiming(e.Argument);
                else if (m_injectSimulatedTimestamp)
                    m_lastFrameReceivedTime = PrecisionTimer.UtcNow.Ticks;

                if (ReceivedUndeterminedFrame != null)
                    ReceivedUndeterminedFrame(this, e);
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "MultiProtocolFrameParser \"ReceivedUndeterminedFrame\" consumer event handler exception: {0}", ex.Message);
            }
        }

        private void m_frameParser_ReceivedFrameBufferImage(object sender, EventArgs<FundamentalFrameType, byte[], int, int> e)
        {
            // We don't stop parsing for exceptions thrown in consumer event handlers
            try
            {
                if (ReceivedFrameBufferImage != null)
                    ReceivedFrameBufferImage(this, e);
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "MultiProtocolFrameParser \"ReceivedFrameBufferImage\" consumer event handler exception: {0}", ex.Message);
            }
        }

        private void m_frameParser_ConfigurationChanged(object sender, EventArgs e)
        {
            // We don't stop parsing for exceptions thrown in consumer event handlers
            try
            {
                if (ConfigurationChanged != null)
                    ConfigurationChanged(this, e);
            }
            catch (Exception ex)
            {
                OnParsingException(ex, "MultiProtocolFrameParser \"ConfigurationChanged\" consumer event handler exception: {0}", ex.Message);
            }
        }

        private void m_frameParser_ParsingException(object sender, EventArgs<Exception> e)
        {
            Exception ex = e.Argument;

            if (ex is CrcException)
                m_totalCrcExceptions++;

            OnParsingException(ex);
        }

        #endregion

        #endregion
    }
}