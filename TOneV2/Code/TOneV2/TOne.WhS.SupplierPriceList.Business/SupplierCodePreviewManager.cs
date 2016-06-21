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
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCodePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CodePreviewDetail> GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierCodePreviewRequestHandler());
        }


        #region Private Classes

        private class SupplierCodePreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, CodePreview, CodePreviewDetail>
        {
            public override CodePreviewDetail EntityDetailMapper(CodePreview entity)
            {
                SupplierCodePreviewManager manager = new SupplierCodePreviewManager();
                return manager.CodePreviewDetailMapper(entity);
            }

            public override IEnumerable<CodePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierCodePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodePreviewDataManager>();
                return dataManager.GetFilteredCodePreview(input.Query);
            }
        }

        #endregion


        #region Private Mappers
        private CodePreviewDetail CodePreviewDetailMapper(CodePreview codePreview)
        {
            CodePreviewDetail codePreviewDetail = new CodePreviewDetail();
            codePreviewDetail.Entity = codePreview;
            codePreviewDetail.ChangeTypeDecription = Utilities.GetEnumAttribute<CodeChangeType, DescriptionAttribute>(codePreview.ChangeType).Description;
            return codePreviewDetail;
        }

        #endregion
    }
}
