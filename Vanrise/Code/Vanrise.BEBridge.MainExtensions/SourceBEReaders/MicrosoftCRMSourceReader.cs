using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Vanrise.BEBridge.MainExtensions.SourceBEReaders
{
    public class MicrosoftCRMSourceReader : SourceBEReader
    {
        DateTime? _modifiedDate;
        public CRMSourceReaderSettings Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            CRMSourceReaderState state = context.ReaderState as CRMSourceReaderState;
            if (state == null)
                state = new CRMSourceReaderState();
            _modifiedDate = state.LastRetrievedTime;

            string apiLink = string.Format("{0}{1}?$orderby=modifiedon asc{2}{3}", Setting.BaseAddress, Setting.EntityName, BuildSelectFields(), BuildFilter());

            while (!string.IsNullOrEmpty(apiLink))
            {
                var webRequest = WebRequest.Create(apiLink);
                webRequest.Headers.Add("Prefer", string.Format("odata.maxpagesize={0}", Setting.TopRecords));
                webRequest.Credentials = new NetworkCredential(Setting.UserName, Setting.Password);
                var response = webRequest.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    context.OnSourceBEBatchRetrieved(GetSourceBatch(stream, ref apiLink), null);
                }
                state.LastRetrievedTime = _modifiedDate;
                context.ReaderState = state;
            }

        }

        private SourceBEBatch GetSourceBatch(Stream stream, ref string apiLink)
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            byte[] bytes = ms.ToArray();
            ms.Close();
            var jsonContent = System.Text.Encoding.Default.GetString(bytes).Replace("@odata.", "");

            dynamic jsonObject = Serializer.Deserialize(jsonContent);
            apiLink = jsonObject.nextLink;
            List<dynamic> data = Serializer.Deserialize<List<dynamic>>(jsonObject.value.ToString());
            if (data.Count > 0)
            {
                DateTime modifiedDate = data.Max(d => d.modifiedon).Value;
                _modifiedDate = modifiedDate;
            }
            return new CRMSourceBatch
            {
                EntityList = data,
                EntityName = Setting.EntityName
            };
        }

        string BuildSelectFields()
        {
            if (Setting.Fields.Count > 0)
            {
                StringBuilder selectQuery = new StringBuilder("&$select=");
                foreach (var field in Setting.Fields)
                    selectQuery.AppendFormat("{0},", field);
                return selectQuery.ToString().TrimEnd(',');
            }
            return string.Empty;
        }
        string BuildFilter()
        {
            StringBuilder filter = new StringBuilder();
            if (_modifiedDate.HasValue)
            {
                filter.AppendFormat("&$filter=modifiedon gt {0}", _modifiedDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                return filter.ToString();
            }

            return string.Empty;
        }
    }

    public class CRMSourceReaderSettings
    {
        public string EntityName { get; set; }
        public List<string> Fields { get; set; }
        public string BaseAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int TopRecords { get; set; }
    }

    public class CRMSourceReaderState
    {
        public DateTime? LastRetrievedTime { get; set; }
    }
}
