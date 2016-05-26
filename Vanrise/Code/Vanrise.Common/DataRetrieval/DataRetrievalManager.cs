﻿using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class DataRetrievalManager
    {
        #region Singleton Instance

        static DataRetrievalManager _instance = new DataRetrievalManager();

        public static DataRetrievalManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private DataRetrievalManager()
        {

        }

        ExcelManager _excelManager = new ExcelManager();

        #endregion

        public IDataRetrievalResult<T> ProcessResult<T>(DataRetrievalInput dataRetrievalInput, BigResult<T> result)
        {
            return ProcessResult(dataRetrievalInput, result, null);
        }

        public IDataRetrievalResult<T> ProcessResult<T>(DataRetrievalInput dataRetrievalInput, BigResult<T> result, ResultProcessingHandler<T> handler)
        {
            if (result == null)
                return null;
            switch (dataRetrievalInput.DataRetrievalResultType)
            {
                case DataRetrievalResultType.Excel: return _excelManager.ExportExcel(result, handler != null ? handler.ExportExcelHandler : null);
                case DataRetrievalResultType.Normal: return result;
            }
            return null;
        }

       
    }
}
