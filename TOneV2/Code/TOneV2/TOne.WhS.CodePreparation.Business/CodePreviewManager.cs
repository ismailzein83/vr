using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CodePreviewDetail> GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CodePreviewRequestHandler());
        }


        #region Private Classes

        private class CodePreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, CodePreview, CodePreviewDetail>
        {
            public override CodePreviewDetail EntityDetailMapper(CodePreview entity)
            {
                CodePreviewManager manager = new CodePreviewManager();
                return manager.CodePreviewDetailMapper(entity);
            }

            public override IEnumerable<CodePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISaleCodePreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleCodePreviewDataManager>();
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
