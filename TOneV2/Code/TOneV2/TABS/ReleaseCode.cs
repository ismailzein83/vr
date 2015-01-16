using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    /// <summary>
    /// Represents an International Release Code (ITU, IETF, SIP)
    /// </summary>
    public class ReleaseCode : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(ReleaseCode).FullName);
        }

        internal static Dictionary<int, ReleaseCode> _All;

        /// <summary>
        /// All the International Release Codes
        /// </summary>
        public static Dictionary<int, ReleaseCode> All
        {
            get
            {
                lock (typeof(ReleaseCode))
                {
                    if (_All == null)
                    {
                        _All = ObjectAssembler.GetList<ReleaseCode>().ToDictionary(rc => rc.Code);

                        // Let's Insert the Default
                        if (_All.Count == 0)
                        {
                            Exception ex;
                            ObjectAssembler.SaveXorUpdate(Default, out ex, true);
                            _All = Default.ToDictionary(rc => rc.Code);
                        }
                    }
                }
                return _All;
            }
        }

        public virtual int Code { get; set; }
        public virtual string Description { get; set; }

        public override string Identifier
        {
            get { return string.Concat("ReleaseCode:", Code); }
        }

        public override string ToString()
        {
            return string.Format("({0:000}) - {1}", Code, Description);
        }

        public static ReleaseCode[] Default = new ReleaseCode[]
        {
            new ReleaseCode { Code = 1, Description = "Unallocated (unassigned) number." },
            new ReleaseCode { Code = 2, Description = "No route to specified transit network (national use)." },
            new ReleaseCode { Code = 3, Description = "No route to destination." },
            new ReleaseCode { Code = 4, Description = "Send special information tone." },
            new ReleaseCode { Code = 5, Description = "Misdialed trunk prefix (national use)." },
            new ReleaseCode { Code = 6, Description = "Channel Unacceptable." },
            new ReleaseCode { Code = 7, Description = "Call awarded and being delivered in an Established channel." },
            new ReleaseCode { Code = 8, Description = "Pre-Emption." },
            new ReleaseCode { Code = 0, Description = "Pre-Emption – Circuit reserved for reuse." },
            new ReleaseCode { Code = 16, Description = "Normal Call Clearing." },
            new ReleaseCode { Code = 17, Description = "User Busy." },
            new ReleaseCode { Code = 18, Description = "No User Responding." },
            new ReleaseCode { Code = 19, Description = "No Answer from User (User Alerted)." },
            new ReleaseCode { Code = 20, Description = "Subscriber Absent." },
            new ReleaseCode { Code = 21, Description = "Call Rejected." },
            new ReleaseCode { Code = 22, Description = "Number Changed." },
            new ReleaseCode { Code = 26, Description = "Non-Selected User Clearing." },
            new ReleaseCode { Code = 27, Description = "Destination Out-of-Order." },
            new ReleaseCode { Code = 28, Description = "Invalid Number Format (address incomplete)." },
            new ReleaseCode { Code = 29, Description = "Facility Rejected." },
            new ReleaseCode { Code = 30, Description = "Response to STATUS ENQUIRY." },
            new ReleaseCode { Code = 31, Description = "Normal, Unspecified." },
            new ReleaseCode { Code = 34, Description = "No Circuit/Channel Available." },
            new ReleaseCode { Code = 38, Description = "Network Out-of-Order." },
            new ReleaseCode { Code = 39, Description = "Permanent Frame Mode Connection Out-of-Service." },
            new ReleaseCode { Code = 40, Description = "Permanent Frame Mode Connection Operational." },
            new ReleaseCode { Code = 41, Description = "Temporary Failure." },
            new ReleaseCode { Code = 42, Description = "Switching Equipment Congestion" },
            new ReleaseCode { Code = 43, Description = "Access Information Discarded" },
            new ReleaseCode { Code = 44, Description = "Requested Circuit/Channel not Available" },
            new ReleaseCode { Code = 47, Description = "Resource Unavailable, Unspecified" },
            new ReleaseCode { Code = 49, Description = "Quality of Service Not Available" },
            new ReleaseCode { Code = 50, Description = "Requested Facility Not Subscribed" },
            new ReleaseCode { Code = 53, Description = "Outgoing Calls Barred Within Closed User Group (CUG)." },
            new ReleaseCode { Code = 55, Description = "Incoming Calls Barred within CUG" },
            new ReleaseCode { Code = 57, Description = "Bearer Capability Not Authorized" },
            new ReleaseCode { Code = 58, Description = "Bearer Capability Not Presently Available" },
            new ReleaseCode { Code = 62, Description = "Inconsistency in Designated Outgoing Access Information and Subscriber Class" },
            new ReleaseCode { Code = 63, Description = "Service or Option Not Available, Unspecified." },
            new ReleaseCode { Code = 65, Description = "Bearer Capability Not Implemented" },
            new ReleaseCode { Code = 66, Description = "Channel Type Not Implemented" },
            new ReleaseCode { Code = 69, Description = "Requested Facility Not Implemented" },
            new ReleaseCode { Code = 70, Description = "Only Restricted Digital Information Bearer Capability is Available (national use)." },
            new ReleaseCode { Code = 79, Description = "Service or Option Not Implemented, Unspecified." },
            new ReleaseCode { Code = 81, Description = "Invalid Call Reference Value." },
            new ReleaseCode { Code = 82, Description = "Identified Channel Does Not Exist." },
            new ReleaseCode { Code = 83, Description = "A Suspended Call Exists, but This Call Identity Does Not." },
            new ReleaseCode { Code = 84, Description = "Call Identity in Use." },
            new ReleaseCode { Code = 85, Description = "No Call Suspended." },
            new ReleaseCode { Code = 86, Description = "Call Having the Requested Call Identity Has Been Cleared." },
            new ReleaseCode { Code = 87, Description = "User Not Member of CUG." },
            new ReleaseCode { Code = 88, Description = "Incompatible Destination." },
            new ReleaseCode { Code = 90, Description = "Non-Existent CUG." },
            new ReleaseCode { Code = 91, Description = "Invalid Transit Network Selection (national use)." },
            new ReleaseCode { Code = 95, Description = "Invalid Message, Unspecified." },
            new ReleaseCode { Code = 96, Description = "Mandatory Information Element is Missing." },
            new ReleaseCode { Code = 97, Description = "Message Type Non-Existent or Not Implemented." },
            new ReleaseCode { Code = 98, Description = "Message Type Not Implemented." },
            new ReleaseCode { Code = 99, Description = "An Information Element or Parameter Does Not Exist or is Not Implemented." },
            new ReleaseCode { Code = 100, Description = "Invalid Information Element Contents." },
            new ReleaseCode { Code = 101, Description = "The Message is Not Compatible with the Call State." },
            new ReleaseCode { Code = 102, Description = "Recovery on Timer Expired." },
            new ReleaseCode { Code = 103, Description = "Parameter Non-Existent or Not Implemented – Passed On (national use)." },
            new ReleaseCode { Code = 110, Description = "Message with Unrecognized Parameter Discarded." },
            new ReleaseCode { Code = 111, Description = "Protocol Error, Unspecified." },
            new ReleaseCode { Code = 127, Description = "Interworking, Unspecified" },
            new ReleaseCode { Code = 400, Description = "Bad Request" },
            new ReleaseCode { Code = 401, Description = "Unauthorized" },
            new ReleaseCode { Code = 402, Description = "Payment Required" },
            new ReleaseCode { Code = 403, Description = "Forbidden" },
            new ReleaseCode { Code = 404, Description = "Not Found" },
            new ReleaseCode { Code = 405, Description = "Method Not Allowed" },
            new ReleaseCode { Code = 406, Description = "Not Acceptable" },
            new ReleaseCode { Code = 407, Description = "Proxy Authentication Required" },
            new ReleaseCode { Code = 408, Description = "Request Timeout" },
            new ReleaseCode { Code = 409, Description = "Conflict" },
            new ReleaseCode { Code = 410, Description = "Gone" },
            new ReleaseCode { Code = 412, Description = "Conditional Request Failed" },
            new ReleaseCode { Code = 413, Description = "Request Entity Too Large" },
            new ReleaseCode { Code = 414, Description = "Request-URI Too Long" },
            new ReleaseCode { Code = 415, Description = "Unsupported Media Type" },
            new ReleaseCode { Code = 416, Description = "Unsupported URI Scheme" },
            new ReleaseCode { Code = 417, Description = "Unknown Resource-Priority" },
            new ReleaseCode { Code = 420, Description = "Bad Extension" },
            new ReleaseCode { Code = 421, Description = "Extension Required" },
            new ReleaseCode { Code = 422, Description = "Session Interval Too Small" },
            new ReleaseCode { Code = 423, Description = "Interval Too Brief" },
            new ReleaseCode { Code = 424, Description = "Bad Location Information" },
            new ReleaseCode { Code = 428, Description = "Use Identity Header" },
            new ReleaseCode { Code = 429, Description = "Provide Referrer Identity" },
            new ReleaseCode { Code = 433, Description = "Anonymity Disallowed" },
            new ReleaseCode { Code = 436, Description = "Bad Identity-Info" },
            new ReleaseCode { Code = 437, Description = "Unsupported Certificate" },
            new ReleaseCode { Code = 438, Description = "Invalid Identity Header" },
            new ReleaseCode { Code = 480, Description = "Temporarily Unavailable" },
            new ReleaseCode { Code = 481, Description = "Call/Transaction Does Not Exist" },
            new ReleaseCode { Code = 482, Description = "Loop Detected" },
            new ReleaseCode { Code = 483, Description = "Too Many Hops" },
            new ReleaseCode { Code = 484, Description = "Address Incomplete" },
            new ReleaseCode { Code = 485, Description = "Ambiguous" },
            new ReleaseCode { Code = 486, Description = "Busy Here" },
            new ReleaseCode { Code = 487, Description = "Request Terminated" },
            new ReleaseCode { Code = 488, Description = "Not Acceptable Here" },
            new ReleaseCode { Code = 489, Description = "Bad Event" },
            new ReleaseCode { Code = 491, Description = "Request Pending" },
            new ReleaseCode { Code = 493, Description = "Undecipherable" },
            new ReleaseCode { Code = 494, Description = "Security Agreement Required" },
            new ReleaseCode { Code = 500, Description = "Server Internal Error" },
            new ReleaseCode { Code = 501, Description = "Not Implemented" },
            new ReleaseCode { Code = 502, Description = "Bad Gateway" },
            new ReleaseCode { Code = 503, Description = "Service Unavailable" },
            new ReleaseCode { Code = 504, Description = "Server Time-out" },
            new ReleaseCode { Code = 505, Description = "Version Not Supported" },
            new ReleaseCode { Code = 513, Description = "Message Too Large" },
            new ReleaseCode { Code = 580, Description = "Precondition Failure" },
            new ReleaseCode { Code = 600, Description = "Busy Everywhere" },
            new ReleaseCode { Code = 603, Description = "Decline" },
            new ReleaseCode { Code = 604, Description = "Does Not Exist Anywhere" },
            new ReleaseCode { Code = 606, Description = "Not Acceptable" }
        };
    }
}
