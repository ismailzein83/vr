using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

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
        DAProfCalcExecInput DAProfCalcExecInput { get; }

        List<DAProfCalcOutputRecord> OutputRecords { get; }

        Action<LogEntryType, string> LogMessage { get; }
    }

    public interface IDAProfCalcOutputRecordProcessorFinalizeContext
    {
        List<dynamic> FinalResultItems { set; }
    }
}