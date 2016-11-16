﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    class ExistingSaleCodeEntity : IExistingEntity
    {
        #region Public Methods

        public ExistingSaleCodeEntity(BusinessEntity.Entities.SaleCode saleCode)
        {
            this.CodeEntity = saleCode;
        }

        public BusinessEntity.Entities.SaleCode CodeEntity { get; set; }

        public string ZoneName { get; set; }

        public int CountryId { get; set; }

        #endregion

        #region IExistingEntity Members

        public IChangedEntity ChangedEntity
        {
            get { return null; }
        }

        public DateTime? OriginalEED
        {
            get { return this.EED; }
        }

        public bool IsSameEntity(IExistingEntity nextEntity)
        {
            ExistingSaleCodeEntity nextExistingCode = nextEntity as ExistingSaleCodeEntity;
            return this.ZoneName.Equals(nextExistingCode.ZoneName, StringComparison.InvariantCultureIgnoreCase);
        }

        public DateTime BED
        {
            get { return this.CodeEntity.BED; }
        }

        public DateTime? EED
        {
            get { return this.CodeEntity.EED; }
        }

        #endregion
    }
}
