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
        #region Fields

        private SaleZoneManager _saleZoneManager = new SaleZoneManager();

        #endregion

        #region Public Methods

        public void BuildNotifications(INotificationContext context)
        {
            if (context.CustomerIds == null)
                throw new NullReferenceException("CustomerIds");

            IEnumerable<SaleCode> saleCodes = new SaleCodeManager().GetSaleCodesEffectiveAfter(context.SellingNumberPlanId, Vanrise.Common.Utilities.Min(context.EffectiveDate, DateTime.Today));
            IEnumerable<ExistingSaleCodeEntity> saleCodesExistingEntities = saleCodes.MapRecords(SaleCodeExistingEntityMapper);

            if (saleCodesExistingEntities == null)
                return;

            SalePriceListManager salePriceListManager = new SalePriceListManager();
            IEnumerable<SalePriceList> salePriceLists = salePriceListManager.GetCustomerSalePriceListsByProcessInstanceId(context.ProcessInstanceId);
            Dictionary<int, SalePriceList> salePriceListsByCustomer = StructureSalePriceListsByCustomer(salePriceLists);

            Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(saleCodesExistingEntities);
            Dictionary<int, List<ExistingSaleZone>> zonesWrapperByCountry = StructureZonesWrapperByCountry(existingSaleCodesByZoneName);
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

            SaleRateReadAllNoCache saleRateReadWithNoCache = new SaleRateReadAllNoCache(routingCustomersInfoDetails, context.EffectiveDate, true);
            SaleEntityZoneRateLocator rateLocator = new SaleEntityZoneRateLocator(saleRateReadWithNoCache);

            SalePLZoneNotification salePLZoneNotifications = new SalePLZoneNotification();
            var customerCountryManager = new CustomerCountryManager();

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            foreach (int customerId in context.CustomerIds)
            {
                CarrierAccount customer = carrierAccountManager.GetCarrierAccount(customerId);

                List<SalePLZoneNotification> customerZoneNotifications = new List<SalePLZoneNotification>();

                int assignedSellingProductId = carrierAccounts.GetRecord(customerId);

                SalePriceListType customerSalePriceListType = GetSalePriceListType(customer.CustomerSettings.IsAToZ, context.ChangeType);
                IEnumerable<int> customerSoldCountries = customerCountryManager.GetCustomerCountryIds(customerId, context.EffectiveDate, false);

                if (customerSoldCountries != null)
                {

                    foreach (int soldCountryId in customerSoldCountries)
                    {
                        List<ExistingSaleZone> zonesWrapperForCountry = zonesWrapperByCountry.GetRecord(soldCountryId);
                        IEnumerable<SalePLZoneChange> countryZonesChanges = zoneChangesByCountryId.GetRecord(soldCountryId);

                        if (customer.CustomerSettings.IsAToZ)
                        {
                            CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperForCountry, rateLocator, assignedSellingProductId, customerId);
                        }
                        else if (countryZonesChanges != null)
                        {
                            if (context.ChangeType == SalePLChangeType.CodeAndRate)
                            {
                                CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperForCountry, rateLocator, assignedSellingProductId, customerId);
                            }
                            else if (context.ChangeType == SalePLChangeType.Rate)
                            {
                                if (zonesWrapperForCountry != null)
                                {
                                    List<ExistingSaleZone> zonesWrapperHaveChanges = new List<ExistingSaleZone>();
                                    foreach (ExistingSaleZone zoneWrapper in zonesWrapperForCountry)
                                    {
                                        SalePLZoneChange salePLZoneChange = countryZonesChanges.FindRecord(item => item.ZoneName.Equals(zoneWrapper.ZoneName));
                                        if (salePLZoneChange != null && salePLZoneChange.CustomersHavingRateChange.Contains(customerId))
                                        {
                                            zonesWrapperHaveChanges.Add(zoneWrapper);
                                        }
                                    }
                                    CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperHaveChanges, rateLocator, assignedSellingProductId, customerId);
                                }
                            }
                        }
                    }
                }

                if (customerZoneNotifications.Count > 0)
                {
                    SendPriceList(customer, customerSalePriceListType, context.InitiatorId, customerZoneNotifications, salePriceListsByCustomer, context.ProcessInstanceId);
                }

            }
        }



        #endregion

        #region Private Methods

        private SalePriceListType GetSalePriceListType(bool isAtoZ, SalePLChangeType changeType)
        {
            if (isAtoZ)
                return SalePriceListType.Full;

            return (changeType == SalePLChangeType.CodeAndRate) ? SalePriceListType.Country : SalePriceListType.RateChange;
        }

        private void SendPriceList(CarrierAccount customer, SalePriceListType customerSalePriceListType, int initiatorId, List<SalePLZoneNotification> customerZonesNotifications, Dictionary<int, SalePriceList> salePriceListsByCustomer, long processInstanceId)
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

            byte[] salePLTemplateBytes = template.Settings.Execute(salePLTemplateSettingsContext);

            Guid salePLMailTemplateId = carrierAccountManager.GetSalePLMailTemplateId(customer.CarrierAccountId);

            UserManager userManager = new UserManager();
            Vanrise.Security.Entities.User initiator = userManager.GetUserbyId(initiatorId);

            string customerName = carrierAccountManager.GetCarrierAccountName(customer.CarrierAccountId);
            string fileName = string.Concat("Pricelist_", customerName, "_", DateTime.Today, ".xls");

            VRFile file = new VRFile()
            {
                Content = salePLTemplateBytes,
                Name = fileName,
                ModuleName = "WhS_BE_SalePriceList",
                Extension = "xls",
                CreatedTime = DateTime.Today,
            };

            VRFileManager fileManager = new VRFileManager();
            long fileId = fileManager.AddFile(file);

            SalePriceList salePriceList;
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            if (salePriceListsByCustomer.TryGetValue(customer.CarrierAccountId, out salePriceList))
            {
                salePriceList.FileId = fileId;
                salePriceList.PriceListType = customerSalePriceListType;
                salePriceListManager.UpdateSalePriceList(salePriceList);
            }
            else
            {
                int salePriceListId = (int)salePriceListManager.ReserveIdRange(1);

                salePriceList = new SalePriceList()
                {
                    OwnerId = customer.CarrierAccountId,
                    OwnerType = SalePriceListOwnerType.Customer,
                    PriceListType = customerSalePriceListType,
                    FileId = fileId,
                    PriceListId = salePriceListId,
                    ProcessInstanceId = processInstanceId,
                    EffectiveOn = DateTime.Today,
                    CurrencyId = customer.CarrierAccountSettings.CurrencyId
                };
                salePriceListManager.AddSalePriceList(salePriceList);
            }

            this.SendMail(salePLMailTemplateId, salePLTemplateBytes, initiator, customer, salePriceList);

        }

        private void SendMail(Guid salePLMailTemplateId, byte[] salePLTemplateBytes, Vanrise.Security.Entities.User initiator, CarrierAccount customer, SalePriceList salePriceList)
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
            objects.Add("Sale Pricelist", salePriceList);

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

        private void CreateSalePLZoneNotifications(List<SalePLZoneNotification> salePLZoneNotifications, List<ExistingSaleZone> zonesWrappers, SaleEntityZoneRateLocator rateLocator, int sellingProductId, int customerId)
        {
            if (zonesWrappers != null)
            {
                foreach (ExistingSaleZone zoneWrapper in zonesWrappers)
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

        private Dictionary<int, SalePriceList> StructureSalePriceListsByCustomer(IEnumerable<SalePriceList> salePriceLists)
        {
            Dictionary<int, SalePriceList> salePriceListsByCustomer = new Dictionary<int, SalePriceList>();

            if (salePriceLists != null)
            {
                SalePriceList salePriceListsTemp;
                foreach (SalePriceList salePriceList in salePriceLists)
                {
                    if (!salePriceListsByCustomer.TryGetValue(salePriceList.OwnerId, out salePriceListsTemp))
                    {
                        salePriceListsTemp = new SalePriceList();
                        salePriceListsByCustomer.Add(salePriceList.OwnerId, salePriceListsTemp);
                    }

                }
            }

            return salePriceListsByCustomer;
        }

        private Dictionary<int, List<ExistingSaleZone>> StructureZonesWrapperByCountry(Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName)
        {
            if (existingSaleCodesByZoneName == null)
                return null;

            Dictionary<int, List<ExistingSaleZone>> zonesWrapperByCountry = new Dictionary<int, List<ExistingSaleZone>>();
            List<ExistingSaleZone> zonesWrapper;
            SaleZoneManager zoneManager = new SaleZoneManager();

            DateTime today = DateTime.Today;
            foreach (KeyValuePair<string, Dictionary<string, List<ExistingSaleCodeEntity>>> zoneItem in existingSaleCodesByZoneName)
            {

                List<ExistingSaleCode> codes = new List<ExistingSaleCode>();
                long zoneId = zoneItem.Value.First().Value.First().CodeEntity.ZoneId;
                DateTime maxCodeBED = DateTime.MinValue;
                foreach (KeyValuePair<string, List<ExistingSaleCodeEntity>> codeItem in zoneItem.Value)
                {
                    IEnumerable<ExistingSaleCodeEntity> connectedSaleCodes = codeItem.Value.OrderBy(itm => itm.BED).ToList().GetLastConnectedEntities();

                    ExistingSaleCode codeObject = new ExistingSaleCode()
                    {
                        Code = codeItem.Key,
                        BED = connectedSaleCodes.First().BED,
                        EED = connectedSaleCodes.Last().EED
                    };

                    if (maxCodeBED < codeObject.BED)
                    {
                        maxCodeBED = codeObject.BED;
                        zoneId = connectedSaleCodes.First().CodeEntity.ZoneId;
                    }

                    codes.Add(codeObject);
                }

                ExistingSaleZone zoneWrapper = new ExistingSaleZone()
                {
                    ZoneName = zoneItem.Key,
                    ZoneId = zoneId,
                    Codes = codes
                };

                int countryId = zoneManager.GetSaleZoneCountryId(zoneWrapper.ZoneId);
                if (!zonesWrapperByCountry.TryGetValue(countryId, out zonesWrapper))
                {
                    zonesWrapper = new List<ExistingSaleZone>();
                    zonesWrapper.Add(zoneWrapper);
                    zonesWrapperByCountry.Add(countryId, zonesWrapper);
                }
                else
                    zonesWrapper.Add(zoneWrapper);
            }

            return zonesWrapperByCountry;
        }

        private long GetEffectiveZoneId(List<ExistingSaleCodeEntity> saleCodes, DateTime effectiveOn)
        {
            long zoneId = saleCodes.First().CodeEntity.ZoneId;
            foreach (ExistingSaleCodeEntity saleCode in saleCodes)
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

        private IEnumerable<SalePLZoneNotification> CreateSalePLZoneNotifications(IEnumerable<ExistingSaleCodeEntity> saleCodesByCountry)
        {
            Dictionary<string, SalePLZoneNotification> countryZoneNotifications = new Dictionary<string, SalePLZoneNotification>();

            if (saleCodesByCountry != null)
            {
                foreach (ExistingSaleCodeEntity saleCode in saleCodesByCountry)
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

        private Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> StructureExistingSaleCodesByZoneName(IEnumerable<ExistingSaleCodeEntity> saleCodesExistingEntities)
        {
            Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = new Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>>();
            if (saleCodesExistingEntities != null)
            {
                Dictionary<string, List<ExistingSaleCodeEntity>> saleCodesByCodeValue;
                List<ExistingSaleCodeEntity> saleCodes;
                foreach (ExistingSaleCodeEntity saleCodeExistingEntity in saleCodesExistingEntities)
                {
                    if (!existingSaleCodesByZoneName.TryGetValue(saleCodeExistingEntity.ZoneName, out saleCodesByCodeValue))
                    {
                        saleCodesByCodeValue = new Dictionary<string, List<ExistingSaleCodeEntity>>();
                        saleCodes = new List<ExistingSaleCodeEntity>();
                        saleCodes.Add(saleCodeExistingEntity);
                        saleCodesByCodeValue.Add(saleCodeExistingEntity.CodeEntity.Code, saleCodes);
                        existingSaleCodesByZoneName.Add(saleCodeExistingEntity.ZoneName, saleCodesByCodeValue);
                    }
                    else
                    {
                        if (!saleCodesByCodeValue.TryGetValue(saleCodeExistingEntity.CodeEntity.Code, out saleCodes))
                        {
                            saleCodes = new List<ExistingSaleCodeEntity>();
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

        private Dictionary<int, List<ExistingSaleCodeEntity>> StructureExistingSaleCodesByCountry(IEnumerable<ExistingSaleCodeEntity> saleCodesExistingEntities)
        {
            Dictionary<int, List<ExistingSaleCodeEntity>> existingSaleCodesByCountryId = new Dictionary<int, List<ExistingSaleCodeEntity>>();
            if (saleCodesExistingEntities != null)
            {
                List<ExistingSaleCodeEntity> saleCodesExistingEntitiesList;
                foreach (ExistingSaleCodeEntity saleCodeExistingEntity in saleCodesExistingEntities)
                {
                    if (!existingSaleCodesByCountryId.TryGetValue(saleCodeExistingEntity.CountryId, out saleCodesExistingEntitiesList))
                    {
                        saleCodesExistingEntitiesList = new List<ExistingSaleCodeEntity>();
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

        #region Private Classes

        private class ExistingSaleZone
        {
            public long ZoneId { get; set; }
            public string ZoneName { get; set; }

            public List<ExistingSaleCode> Codes { get; set; }
        }

        private class ExistingSaleCode
        {
            public string Code { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }

        }

        #endregion

        #region Mappers

        private SalePLCodeNotification CodeNotificationMapper(ExistingSaleCodeEntity saleCode)
        {
            return new SalePLCodeNotification()
            {
                Code = saleCode.CodeEntity.Code,
                BED = saleCode.BED,
                EED = saleCode.EED
            };
        }

        private SalePLCodeNotification SalePLCodeNotificationMapper(ExistingSaleCode saleCode)
        {
            return new SalePLCodeNotification()
            {
                Code = saleCode.Code,
                BED = saleCode.BED,
                EED = saleCode.EED
            };
        }

        private ExistingSaleCodeEntity SaleCodeExistingEntityMapper(SaleCode saleCode)
        {
            return new ExistingSaleCodeEntity(saleCode)
            {
                CountryId = _saleZoneManager.GetSaleZoneCountryId(saleCode.ZoneId),
                ZoneName = _saleZoneManager.GetSaleZoneName(saleCode.ZoneId)
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
