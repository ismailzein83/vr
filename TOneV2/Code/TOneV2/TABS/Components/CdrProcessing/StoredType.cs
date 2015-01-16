
namespace TABS.Components.CdrProcessing
{
    public enum StoredType : byte
    {
        /// <summary>
        /// 4 byte or less integral types
        /// </summary>
        Integer,
        /// <summary>
        /// more than 4 byte types
        /// </summary>
        Number,
        /// <summary>
        /// Date and Time
        /// </summary>
        DateTime,
        /// <summary>
        /// Text
        /// </summary>
        Text
    }
}