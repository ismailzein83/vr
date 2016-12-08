using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IDAProfCalcOutputRecordProcessor
    {
        void Initialize(IDAProfCalcOutputRecordProcessorIntializeContext context);

        void ProcessOutputRecords(IDAProfCalcOutputRecordProcessorProcessContext context);

        void Finalize(IDAProfCalcOutputRecordProcessorFinalizeContext context);
    }

    public interface IDAProfCalcOutputRecordProcessorIntializeContext
    {

    }

    public interface IDAProfCalcOutputRecordProcessorProcessContext
    {
        List<DAProfCalcOutputRecord> OutputRecords { get; }
    }

    public interface IDAProfCalcOutputRecordProcessorFinalizeContext
    {
        List<dynamic> FinalResultItems { set; }
    }
}
