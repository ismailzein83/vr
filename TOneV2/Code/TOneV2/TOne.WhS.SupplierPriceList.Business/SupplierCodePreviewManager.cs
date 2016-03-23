﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCodePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CodePreviewDetail> GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ISupplierCodePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodePreviewDataManager>();
            BigResult<CodePreview> codesPreview = dataManager.GetCodePreviewFilteredFromTemp(input);
            BigResult<CodePreviewDetail> codePreviewDetailResult = new BigResult<CodePreviewDetail>()
            {
                ResultKey = codesPreview.ResultKey,
                TotalCount = codesPreview.TotalCount,
                Data = codesPreview.Data.MapRecords(CodePreviewDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, codePreviewDetailResult);
        }

        private CodePreviewDetail CodePreviewDetailMapper(CodePreview codePreview)
        {
            CodePreviewDetail codePreviewDetail = new CodePreviewDetail();
            codePreviewDetail.Entity = codePreview;
            codePreviewDetail.ChangeTypeDecription = Utilities.GetEnumAttribute<CodeChangeType, DescriptionAttribute>(codePreview.ChangeType).Description;
            return codePreviewDetail;
        }
    }
}
