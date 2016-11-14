using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class NotificationManager
    {

        #region Public Methods

        public SaleZoneManager _SaleZoneManager;

        public NotificationManager()
        {
            _SaleZoneManager = new SaleZoneManager();
        }

        public void BuildNotifications(INotificationContext context)
        {
            if (context.CustomerIds == null)
                throw new NullReferenceException("CustomerIds");

            IEnumerable<SaleCode> saleCodes = new SaleCodeManager().GetSaleCodesEffectiveAfter(context.SellingNumberPlanId, Vanrise.Common.Utilities.Min(context.EffectiveDate, DateTime.Today));
            IEnumerable<SaleCodeExistingEntity> saleCodesExistingEntities = saleCodes.MapRecords(SaleCodeExistingEntityMapper);

            if (saleCodesExistingEntities == null)
                return;

            Dictionary<string, Dictionary<string, List<SaleCodeExistingEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(saleCodesExistingEntities);
            Dictionary<int, List<ZoneWrapper>> zonesWrapperByCountry = StructureZonesWrapperByCountry(existingSaleCodesByZoneName);
            Dictionary<int, List<SalePLZoneChange>> zoneChangesByCountryId = StructureZoneChangesByCountry(context.ZoneChanges);

            List<RoutingCustomerInfoDetails> routingCustomersInfoDetails = new List<RoutingCustomerInfoDetails>();
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            Dictionary<int, int> carrierAccounts = new Dictionary<int, int>();

            foreach (int customerId in context.CustomerIds)
            {
                CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, context.EffectiveDate, false);
                if (customerSellingProduct == null)
                    continue;

                if (!carrierAccounts.ContainsKey(customerId))
                    carrierAccounts.Add(customerId, customerSellingProduct.SellingProductId);

                routingCustomersInfoDetails.Add(new RoutingCustomerInfoDetails()
                {
                    CustomerId = customerId,
                    SellingProductId = customerSellingProduct.SellingProductId
                });
            }

            SaleRateReadAllNoCache saleRateReadWithNoCache = new SaleRateReadAllNoCache(routingCustomersInfoDetails, context.EffectiveDate, false);
            SaleEntityZoneRateLocator rateLocator = new SaleEntityZoneRateLocator(saleRateReadWithNoCache);

            CustomerZoneManager customerZoneManager = new CustomerZoneManager();

            SalePLZoneNotification salePLZoneNotifications = new SalePLZoneNotification();
           

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            foreach (int customerId in context.CustomerIds)
            {
                CarrierAccount customer = carrierAccountManager.GetCarrierAccount(customerId);
                List<SalePLZoneNotification> customerZoneNotifications = new List<SalePLZoneNotification>();

                IEnumerable<SaleZone> saleZones = customerZoneManager.GetCustomerSaleZones(customerId, context.SellingNumberPlanId, context.EffectiveDate, false);
                if (saleZones == null)
                    continue;

                SalePriceListType customerSalePriceListType = GetSalePriceListType(customer.CustomerSettings.IsAToZ, context.ChangeType);
                IEnumerable<int> customerSoldCountries = saleZones.Select(item => item.CountryId).Distinct();

                foreach (int soldCountryId in customerSoldCountries)
                {
                    List<ZoneWrapper> zonesWrapperForCountry = zonesWrapperByCountry.GetRecord(soldCountryId);
                    IEnumerable<SalePLZoneChange> countryZonesChanges = zoneChangesByCountryId.GetRecord(soldCountryId);

                    if (customer.CustomerSettings.IsAToZ || countryZonesChanges != null)
                    {
                        if (context.ChangeType == SalePLChangeType.CodeAndRate)
                        {
                            CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperForCountry, rateLocator, carrierAccounts.GetRecord(customerId), customerId);
                        }
                        else if (context.ChangeType == SalePLChangeType.Rate)
                        {
                            IEnumerable<SalePLZoneChange> countryZoneChanges = zoneChangesByCountryId.GetRecord(soldCountryId);
                            if (zonesWrapperForCountry != null)
                            {
                                List<ZoneWrapper> zonesWrapperHaveChanges = new List<ZoneWrapper>();
                                foreach (ZoneWrapper zoneWrapper in zonesWrapperForCountry)
                                {
                                    SalePLZoneChange salePLZoneChange = countryZoneChanges.FindRecord(item => item.ZoneName.Equals(zoneWrapper.ZoneName));
                                    if (salePLZoneChange != null && salePLZoneChange.CustomersHavingRateChange.Contains(customerId))
                                    {
                                        zonesWrapperHaveChanges.Add(zoneWrapper);
                                    }
                                }
                                CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperHaveChanges, rateLocator, carrierAccounts.GetRecord(customerId), customerId);
                            }
                        }
                    }

                }

				if (customerZoneNotifications.Count > 0)
					SendPriceList(customer, customerSalePriceListType, context.InitiatorId, customerZoneNotifications);
            }
        }

        #endregion


        #region Private Methods And Classes

        private SalePriceListType GetSalePriceListType(bool isAtoZ, SalePLChangeType changeType)
        {
            if (isAtoZ)
                return SalePriceListType.Full;

            return (changeType == SalePLChangeType.CodeAndRate) ? SalePriceListType.Country : SalePriceListType.RateChange;
        }
        

        private class ZoneWrapper
        {
            public long ZoneId { get; set; }
            public string ZoneName { get; set; }

            private List<CodeObject> _codes = new List<CodeObject>();
            public List<CodeObject> Codes
            {
                get
                {
                    return this._codes;
                }
            }
        }

        private class CodeObject
        {
            public string Code { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }

        }


        private void SendPriceList(CarrierAccount customer, SalePriceListType customerSalePriceListType, int initiatorId, List<SalePLZoneNotification> customerZonesNotifications)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int priceListTemplateId = carrierAccountManager.GetSalePriceListTemplateId(customer.CarrierAccountId);

            SalePriceListTemplateManager salePriceListTemplateManager = new SalePriceListTemplateManager();
            SalePriceListTemplate template = salePriceListTemplateManager.GetSalePriceListTemplate(priceListTemplateId);

            if (template == null)
                throw new DataIntegrityValidationException(string.Format("Customer with Id {0} does not have a Sale Price List Template", customer.CarrierAccountId));

            ISalePriceListTemplateSettingsContext salePLTemplateSettingsContext = new SalePriceListTemplateSettingsContext()
            {
                Zones = customerZonesNotifications
            };

            byte [] salePLTemplateBytes = template.Settings.Execute(salePLTemplateSettingsContext);

            Guid salePLMailTemplateId = carrierAccountManager.GetSalePLMailTemplateId(customer.CarrierAccountId);
            
            UserManager userManager = new UserManager();
            Vanrise.Security.Entities.User initiator = userManager.GetUserbyId(initiatorId);

            this.SendMail(salePLMailTemplateId, salePLTemplateBytes, initiator, customer);

        }

        private void SendMail(Guid salePLMailTemplateId, byte[] salePLTemplateBytes, Vanrise.Security.Entities.User initiator, CarrierAccount customer)
        {
            MemoryStream memoryStream = new MemoryStream(salePLTemplateBytes);
            memoryStream.Position = 0;

            var attachment = new Attachment(memoryStream, "SalePriceList.xlsx");
            attachment.ContentType = new ContentType("application/vnd.ms-excel");
            attachment.TransferEncoding = TransferEncoding.Base64;
            attachment.NameEncoding = Encoding.UTF8;
            attachment.Name = "SalePriceList.xls";

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("Customer", customer);
            objects.Add("User", initiator);

            VRMailManager vrMailManager = new VRMailManager();
            VRMailEvaluatedTemplate evaluatedTemplate = vrMailManager.EvaluateMailTemplate(salePLMailTemplateId, objects);

            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            EmailSettingData emailSettingData = configManager.GetSystemEmail();

            MailMessage objMail = new MailMessage();

			if (evaluatedTemplate.To != null)
			{
				foreach (string toEmail in evaluatedTemplate.To.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
					objMail.To.Add(toEmail);
			}
			if (evaluatedTemplate.CC != null)
			{
				foreach (string ccEmail in evaluatedTemplate.CC.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
					objMail.CC.Add(ccEmail);
			}
            objMail.From = new MailAddress(emailSettingData.SenderEmail);
            objMail.Subject = evaluatedTemplate.Subject;
            objMail.Body = evaluatedTemplate.Body;
            objMail.IsBodyHtml = true;
            objMail.Priority = MailPriority.High;
            objMail.Attachments.Add(attachment);

            SmtpClient client = vrMailManager.GetSMTPClient(emailSettingData);
            client.Send(objMail);
        }

        private void CreateSalePLZoneNotifications(List<SalePLZoneNotification> salePLZoneNotifications, List<ZoneWrapper> zonesWrappers, SaleEntityZoneRateLocator rateLocator, int sellingProductId,
            int customerId)
        {
            if (zonesWrappers != null)
            {
                foreach (ZoneWrapper zoneWrapper in zonesWrappers)
                {
                    SalePLZoneNotification zoneNotification = new SalePLZoneNotification()
                    {
                        ZoneName = zoneWrapper.ZoneName,
                        ZoneId = zoneWrapper.ZoneId
                    };

                    SaleEntityZoneRate saleEntityZoneRate = rateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneWrapper.ZoneId);
                    
                    zoneNotification.Rate = saleEntityZoneRate != null ? SalePLRateNotificationMapper(saleEntityZoneRate) : GetExistingRate();
                    zoneNotification.Codes.AddRange(zoneWrapper.Codes.MapRecords(SalePLCodeNotificationMapper));
                    salePLZoneNotifications.Add(zoneNotification);
                }
            }
        }


        private Dictionary<int, List<ZoneWrapper>> StructureZonesWrapperByCountry(Dictionary<string, Dictionary<string, List<SaleCodeExistingEntity>>> existingSaleCodesByZoneName)
        {
            if (existingSaleCodesByZoneName == null)
                return null;

            Dictionary<int, List<ZoneWrapper>> zonesWrapperByCountry = new Dictionary<int, List<ZoneWrapper>>();
            List<ZoneWrapper> zonesWrapper;
            SaleZoneManager zoneManager = new SaleZoneManager();

            DateTime today = DateTime.Today;
            foreach (KeyValuePair<string, Dictionary<string, List<SaleCodeExistingEntity>>> zoneItem in existingSaleCodesByZoneName)
            {
                ZoneWrapper zoneWrapper = new ZoneWrapper()
                {
                    ZoneName = zoneItem.Key,
                    ZoneId = GetEffectiveZoneId(zoneItem.Value.First().Value, today)
                };

                foreach (KeyValuePair<string, List<SaleCodeExistingEntity>> codeItem in zoneItem.Value)
                {
                    List<SaleCodeExistingEntity> connectedSaleCodes = codeItem.Value.GetConnectedEntities(today);
                    CodeObject codeObject = new CodeObject()
                    {
                        Code = codeItem.Key,
                        BED = connectedSaleCodes.First().BED,
                        EED = connectedSaleCodes.Last().EED
                    };
                    zoneWrapper.Codes.Add(codeObject);
                }

                int countryId = zoneManager.GetSaleZoneCountryId(zoneWrapper.ZoneId);
                if (!zonesWrapperByCountry.TryGetValue(countryId, out zonesWrapper))
                {
                    zonesWrapper = new List<ZoneWrapper>();
                    zonesWrapper.Add(zoneWrapper);
                    zonesWrapperByCountry.Add(countryId, zonesWrapper);
                }
                else
                    zonesWrapper.Add(zoneWrapper);
            }

            return zonesWrapperByCountry;
        }

        private long GetEffectiveZoneId(List<SaleCodeExistingEntity> saleCodes, DateTime effectiveOn)
        {
            long zoneId = saleCodes.First().CodeEntity.ZoneId;
            foreach (SaleCodeExistingEntity saleCode in saleCodes)
            {
                SaleCode codeEntity = saleCode.CodeEntity;
                if (codeEntity.BED <= effectiveOn && codeEntity.EED.VRGreaterThan(effectiveOn))
                {
                    zoneId = codeEntity.ZoneId;
                    break;
                }
            }
            return zoneId;
        }

        private SalePLRateNotification GetExistingRate()
        {
            //TODO: Must Get Existing Rate
            return null;
        }

        private IEnumerable<SalePLZoneNotification> CreateSalePLZoneNotifications(IEnumerable<SaleCodeExistingEntity> saleCodesByCountry)
        {
            Dictionary<string, SalePLZoneNotification> countryZoneNotifications = new Dictionary<string, SalePLZoneNotification>();

            if (saleCodesByCountry != null)
            {
                foreach (SaleCodeExistingEntity saleCode in saleCodesByCountry)
                {
                    string zoneName = saleCode.ZoneName;
                    SalePLZoneNotification zoneNotification = null;
                    if (!countryZoneNotifications.TryGetValue(zoneName, out zoneNotification))
                    {
                        zoneNotification = new SalePLZoneNotification() { ZoneName = zoneName, ZoneId = saleCode.CodeEntity.ZoneId };
                        countryZoneNotifications.Add(zoneName, zoneNotification);
                    }

                    zoneNotification.Codes.Add(this.CodeNotificationMapper(saleCode));
                }
            }

            return countryZoneNotifications.Values;
        }

        private Dictionary<int, List<SalePLZoneChange>> StructureZoneChangesByCountry(IEnumerable<SalePLZoneChange> zoneChanges)
        {
            Dictionary<int, List<SalePLZoneChange>> existingSaleCodesByCountryId = new Dictionary<int, List<SalePLZoneChange>>();
            if (zoneChanges != null)
            {
                List<SalePLZoneChange> zoneChangesList;
                foreach (SalePLZoneChange zoneChange in zoneChanges)
                {
                    if (!existingSaleCodesByCountryId.TryGetValue(zoneChange.CountryId, out zoneChangesList))
                    {
                        zoneChangesList = new List<SalePLZoneChange>();
                        zoneChangesList.Add(zoneChange);
                        existingSaleCodesByCountryId.Add(zoneChange.CountryId, zoneChangesList);
                    }
                    else
                        zoneChangesList.Add(zoneChange);
                }
            }

            return existingSaleCodesByCountryId;
        }


        private Dictionary<string, Dictionary<string, List<SaleCodeExistingEntity>>> StructureExistingSaleCodesByZoneName(IEnumerable<SaleCodeExistingEntity> saleCodesExistingEntities)
        {
            Dictionary<string, Dictionary<string, List<SaleCodeExistingEntity>>> existingSaleCodesByZoneName = new Dictionary<string, Dictionary<string, List<SaleCodeExistingEntity>>>();
            if (saleCodesExistingEntities != null)
            {
                Dictionary<string, List<SaleCodeExistingEntity>> saleCodesByCodeValue;
                List<SaleCodeExistingEntity> saleCodes;
                foreach (SaleCodeExistingEntity saleCodeExistingEntity in saleCodesExistingEntities)
                {
                    if (!existingSaleCodesByZoneName.TryGetValue(saleCodeExistingEntity.ZoneName, out saleCodesByCodeValue))
                    {
                        saleCodesByCodeValue = new Dictionary<string, List<SaleCodeExistingEntity>>();
                        saleCodes = new List<SaleCodeExistingEntity>();
                        saleCodes.Add(saleCodeExistingEntity);
                        saleCodesByCodeValue.Add(saleCodeExistingEntity.CodeEntity.Code, saleCodes);
                        existingSaleCodesByZoneName.Add(saleCodeExistingEntity.ZoneName, saleCodesByCodeValue);
                    }
                    else
                    {
                        if (!saleCodesByCodeValue.TryGetValue(saleCodeExistingEntity.CodeEntity.Code, out saleCodes))
                        {
                            saleCodes = new List<SaleCodeExistingEntity>();
                            saleCodes.Add(saleCodeExistingEntity);
                            saleCodesByCodeValue.Add(saleCodeExistingEntity.CodeEntity.Code, saleCodes);
                        }
                        else
                            saleCodes.Add(saleCodeExistingEntity);
                    }
                }
            }

            return existingSaleCodesByZoneName;
        }

        private Dictionary<int, List<SaleCodeExistingEntity>> StructureExistingSaleCodesByCountry(IEnumerable<SaleCodeExistingEntity> saleCodesExistingEntities)
        {
            Dictionary<int, List<SaleCodeExistingEntity>> existingSaleCodesByCountryId = new Dictionary<int, List<SaleCodeExistingEntity>>();
            if (saleCodesExistingEntities != null)
            {
                List<SaleCodeExistingEntity> saleCodesExistingEntitiesList;
                foreach (SaleCodeExistingEntity saleCodeExistingEntity in saleCodesExistingEntities)
                {
                    if (!existingSaleCodesByCountryId.TryGetValue(saleCodeExistingEntity.CountryId, out saleCodesExistingEntitiesList))
                    {
                        saleCodesExistingEntitiesList = new List<SaleCodeExistingEntity>();
                        saleCodesExistingEntitiesList.Add(saleCodeExistingEntity);
                        existingSaleCodesByCountryId.Add(saleCodeExistingEntity.CountryId, saleCodesExistingEntitiesList);
                    }
                    else
                        saleCodesExistingEntitiesList.Add(saleCodeExistingEntity);
                }
            }

            return existingSaleCodesByCountryId;
        }

        #endregion


        #region Private Mappers

        private SalePLCodeNotification CodeNotificationMapper(SaleCodeExistingEntity saleCode)
        {
            return new SalePLCodeNotification()
            {
                Code = saleCode.CodeEntity.Code,
                BED = saleCode.BED,
                EED = saleCode.EED
            };
        }

        private SalePLCodeNotification SalePLCodeNotificationMapper(CodeObject saleCode)
        {
            return new SalePLCodeNotification()
            {
                Code = saleCode.Code,
                BED = saleCode.BED,
                EED = saleCode.EED
            };
        }

        private SaleCodeExistingEntity SaleCodeExistingEntityMapper(SaleCode saleCode)
        {
            return new SaleCodeExistingEntity(saleCode)
            {
                CountryId = _SaleZoneManager.GetSaleZoneCountryId(saleCode.ZoneId)
            };
        }

        private SalePLRateNotification SalePLRateNotificationMapper(SaleEntityZoneRate saleEntityZoneRate)
        {
            return new SalePLRateNotification()
            {
                Rate = saleEntityZoneRate.Rate.Rate,
                BED = saleEntityZoneRate.Rate.BED,
                EED = saleEntityZoneRate.Rate.EED
            };
        }


        #endregion


    }
}
