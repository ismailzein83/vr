using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcStoreEventsOutputProcessor : IDAProfCalcOutputRecordProcessor
    {
        public void Initialize(IDAProfCalcOutputRecordProcessorIntializeContext context)
        {

        }

        public void ProcessOutputRecords(IDAProfCalcOutputRecordProcessorProcessContext context)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (context.OutputRecords != null)
            {
                foreach (DAProfCalcOutputRecord outputRecord in context.OutputRecords)
                {
                    strBuilder.AppendLine(Vanrise.Common.Serializer.Serialize(outputRecord.Records));
                }
            }
            File.WriteAllText(string.Format("C:\\BCPFiles\\{0}.txt", Guid.NewGuid().ToString()), strBuilder.ToString());
        }

        public void Finalize(IDAProfCalcOutputRecordProcessorFinalizeContext context)
        {

        }
    }
}
