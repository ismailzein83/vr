using System;
using Vanrise.GenericData.Entities;
using Retail.RA.Entities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace Retail.RA.Business
{
    public class IntlOperatorDeclarationServicesCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("DEDB89C0-370F-4DF8-BE63-EE60C73436F6"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as IntlOperatorDeclarationServices;
            if (valueObject != null)
            {
                var services = valueObject.Services as List<IntlOperatorDeclarationService>;
                if(services == null)
                    services = Utilities.ConvertJsonToList<IntlOperatorDeclarationService>(valueObject.Services);
                if (services != null)
                {
                    long voiceInNumberOfCalls = 0;
                    long voiceOutNumberOfCalls = 0;

                    Decimal voiceInDuration = 0;
                    Decimal voiceOutDuration = 0;

                    Decimal voiceInRevenue = 0;
                    Decimal voiceOutRevenue = 0;

                    long numberOfSmsIn = 0;
                    long numberOfSmsOut = 0;

                    Decimal smsInRevenue = 0;
                    Decimal smsOutRevenue = 0;

                    foreach (var service in services)
                    {
                        if (service.Settings == null)
                            throw new NullReferenceException("service.Settings");
                        var serviceType = service.Settings.GetServiceType();
                        if (serviceType == ServiceType.Voice)
                        {
                            var voice = service.Settings as IntlVoice;
                            if (voice != null)
                            {
                                var trafficDirection = voice.GetTrafficDirection();
                                if(trafficDirection == TrafficDirection.IN)
                                {
                                    voiceInNumberOfCalls += voice.DeclaredNumberOfCalls;
                                    voiceInDuration += voice.DeclaredDuration;
                                    voiceInRevenue += voice.DeclaredRevenue;
                                }
                                else if(trafficDirection == TrafficDirection.OUT)
                                {
                                    voiceOutNumberOfCalls += voice.DeclaredNumberOfCalls;
                                    voiceOutDuration += voice.DeclaredDuration;
                                    voiceOutRevenue += voice.DeclaredRevenue;
                                }
                            }
                        }
                        else if (serviceType == ServiceType.SMS)
                        {
                            var sms = service.Settings as IntlSMS;
                            if (sms != null)
                            {
                                var trafficDirection = sms.GetTrafficDirection();
                                if (trafficDirection == TrafficDirection.IN)
                                {
                                    numberOfSmsIn += sms.NumberOfSMSs;
                                    smsInRevenue += sms.Revenue;
                                }
                                else if (trafficDirection == TrafficDirection.OUT)
                                {
                                    numberOfSmsOut += sms.NumberOfSMSs;
                                    smsOutRevenue += sms.Revenue;
                                }
                            }
                        }
                    }
                    string voiceInDescription = null;
                    string voiceOutDescription = null;
                    string smsInDescription = null;
                    string smsOutDescription = null;
                    List<string> descriptions = new List<string>();

                    if(voiceInNumberOfCalls != 0 || voiceInDuration != 0 || voiceInRevenue != 0)
                    {
                        voiceInDescription = string.Format("Voice(IN)-Number Of Calls: {0},Duration: {1} , Revenue: {2}", voiceInNumberOfCalls, voiceInDuration,voiceInRevenue);
                        descriptions.Add(voiceInDescription);
                    }  
                    if(voiceOutNumberOfCalls != 0 || voiceOutDuration != 0 || voiceOutRevenue != 0)
                    {
                        voiceOutDescription = string.Format("Voice(Out)-Number Of Calls: {0},Duration: {1} , Revenue: {2}", voiceOutNumberOfCalls, voiceOutDuration, voiceOutRevenue);
                        descriptions.Add(voiceOutDescription);
                    }
                    if(numberOfSmsIn != 0 || smsInRevenue!= 0)
                    {
                        smsInDescription = string.Format("SMS(IN)-Number Of SMS: {0} , Revenue: {1}", numberOfSmsIn, smsInRevenue);
                        descriptions.Add(smsInDescription);
                    }
                    if(numberOfSmsOut != 0 || smsOutRevenue != 0)
                    {
                        smsOutDescription = string.Format("SMS(Out)-Number Of SMS: {0} , Revenue: {1}", numberOfSmsOut, smsOutRevenue);
                        descriptions.Add(smsOutDescription);
                    }
                    if(descriptions.Count() > 0)
                    {
                        var description = string.Join(" / ", descriptions);
                        return description;
                    }
                }
            }
            return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(IntlOperatorDeclarationServices);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as IntlOperatorDeclarationServices;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Operator Declaration Services";
        }
    }
}
