using System;
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
        public void Insert(int priceListId, IEnumerable<CodePreview> codePreviewList)
        {
            ISupplierCodePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodePreviewDataManager>();
            dataManager.Insert(priceListId, codePreviewList);
        }
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

        private CodePreviewDetail CodePreviewDetailMapper(CodePreview codePreview){
            CodePreviewDetail codePreviewDetail = new CodePreviewDetail();

            codePreviewDetail.Entity = codePreview;
            var changeTypeAttribute = Utilities.GetEnumAttribute<CodeChangeType, DescriptionAttribute>(codePreview.ChangeType);

            if (changeTypeAttribute!= null)
                codePreviewDetail.ChangeTypeDecription = changeTypeAttribute.Description;
            else
                codePreviewDetail.ChangeTypeDecription = codePreview.ChangeType.ToString();
            return codePreviewDetail;
        }
    }
}
