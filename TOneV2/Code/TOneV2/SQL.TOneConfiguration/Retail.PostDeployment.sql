/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					 
--------------------------------------------------------------------------------------
*/

--[common].[extensionconfiguration]-------------------------------------------------------------------
delete from [common].[extensionconfiguration] where ID in ('5D41FC92-ECCB-48CD-A32E-4DDE13E757ED', '02f5eb86-af71-4d35-9e5a-4739a43f2438','fe81b4b3-d90c-47de-b40c-3b4379e880a4','A4011953-B2CC-4C91-89CB-3ADFF84D94D1','26EF5E51-C9C9-4D87-97B8-4506F42892AC','44EBA251-6032-4F0F-A254-4CCF0ED62DB2','D64C95FC-5E4B-46B7-95CD-77082F91B07F', '077ECA6F-3CB8-42C3-95F4-9C563EB2BDCB', '80B1AFC2-3222-41D5-84B6-7004838BFBA9')
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1DD9CB13-CCBB-47EF-8514-6CCA50AEF298','VR_Common_VRObjectType_RetailAccount','Retail Account','VR_Common_ObjectType'																						,'{"Editor":"retail-be-retailaccount-objecttype", "PropertyEvaluatorExtensionType": "Retail_BE_RetailAccountObjectType_PropertyEvaluator"}'),

('2750F03C-713E-477A-B892-F8E9037AD5C5','User Convertor','User Convertor','VR_BEBridge_BEConvertor'																											,'{"Editor":"retail-be-user-convertor-editor"}'),

('6BA989F8-71D6-42DC-80F0-5128EBB8FFD2','ChargingPolicyVoice','Voice','Retail_BE_ChargingPolicyDefinition'																									,'{"Editor":"retail-voice-chargingpolicydefinition"}'),
('4E993EFD-6AFE-4C3A-ACA2-83CD5C8FFC35','ChargingPolicyData','Data','Retail_BE_ChargingPolicyDefinition'																									,'{"Editor":"retail-data-chargingpolicydefinition"}'),

('3900317C-B982-4D8B-BD0D-01215AC1F3D9','Personal Info','Personal Info','Retail_BE_AccountPartDefinition'																									,'{"DefinitionEditor":"retail-be-accounttype-part-definition-personalinfo" , "RuntimeEditor":"retail-be-accounttype-part-runtime-personalinfo"}'),
('747D0C68-A508-4AA3-8D02-0D3CDFE72149','Residential Profile','Residential Profile','Retail_BE_AccountPartDefinition'																						,'{"DefinitionEditor":"retail-be-accounttype-part-definition-residentialprofile" , "RuntimeEditor":"retail-be-accounttype-part-runtime-residentialprofile"}'),

('C07BB017-0A93-455C-96C3-1AFF84FFA7A6','CRM Account Convertor','CRM Account Convertor','VR_BEBridge_BEConvertor'																							,'{"Editor":"retail-be-crmaccount-convertor-editor"}'),
('81E81C54-61C5-4D09-BA9F-1F3A10D16509','Block Teles Switch','Block Teles Switch','Retail_BE_ProvisionerDefinition'																							,'{"DefinitionEditor":"retail-be-provisioner-definitionsettings-blocktelesswitch", "RuntimeEditor":"retail-be-provisioner-runtimesettings-blocktelesswitch"}'),
('E879D6D5-6CBE-4391-B1E0-29FD7C378F65','SingleDurationTariff','Single Duration Tariff','Retail_BE_ChargingPolicyPart_DurationTariff'																		,'{"DefinitionEditor":"retail-be-chargingpolicypart-durationtarrifs-single","RuntimeEditor":"retail-be-chargingpolicypart-durationtariff-single-runtimeeditor"}'),
('BA425FA1-13CA-4F44-883A-2A12B7E3F988','Financial','Financial','Retail_BE_AccountPartDefinition'																											,'{"DefinitionEditor":"retail-be-accounttype-part-definition-financial", "RuntimeEditor":"retail-be-accounttype-part-runtime-financial"}'),
('387AA4E2-9782-4805-88A4-30F2C9EA297B','Activate Teles Switch','Activate Teles Switch','Retail_BE_ProvisionerDefinition'																					,'{"DefinitionEditor":"retail-be-provisioner-definitionsettings-activatetelesswitch", "RuntimeEditor":"retail-be-provisioner-runtimesettings-activatetelesswitch"}'),
('A8E093F4-F9C3-420B-99B7-32EEA2C1DF78','BalanceAlertAccountAction','Account Action','VR_AccountBalance_BalanceAlert_VRAction'																				,'{"Editor":"retail-be-action-balancealertaccount"}'),
('3E20362A-340D-493D-BB25-3DE674CDCD1D','Generic','Generic','Retail_BE_AccountPartDefinition'																												,'{"DefinitionEditor":"retail-be-accounttype-part-definition-generic", "RuntimeEditor":"retail-be-accounttype-part-runtime-generic"}'),
('1AFF2BF7-1F15-4E0B-ACCF-457EDF36A342','Company Profile','Company Profile','Retail_BE_AccountPartDefinition'																								,'{"DefinitionEditor":"retail-be-accounttype-part-definition-companyprofile", "RuntimeEditor":"retail-be-accounttype-part-runtime-companyprofile"}'),
('91E3809B-244F-4EE8-8487-567E10E2FBC7','Monthly Recurring Period','Monthly','Retail_BE_RecurringPeriod'																									,'{"Editor":"retail-be-recurringperiod-monthly"}'),
('2B442DB1-B688-47CB-91CF-6D1A8435633D','SingleRateValue','Single Rate Value','Retail_BE_ChargingPolicyPart_RateValue'																						,'{"DefinitionEditor":"retail-be-chargingpolicypart-ratevalues-single","RuntimeEditor":"retail-be-chargingpolicypart-ratevalue-single-runtimeeditor"}'),
('C6AF019E-2CAF-4FAB-9889-74737B13BB0D','WebService','Web Service','Retail_BE_SwitchIntegration'																											,'{"Editor":"retail-be-switchintegrations-webservice"}'),
('A982BC4B-89B9-4A84-ABAE-78B1D1C37941','Activation','Activation','Retail_BE_AccountPartDefinition'																											,'{"DefinitionEditor":"retail-be-accounttype-part-definition-activation", "RuntimeEditor":"retail-be-accounttype-part-runtime-activation"}'),

('F3CEE2A7-1D63-4804-B9C0-9ABA4F43F480','ChargingPolicyItem','Charging Policy','Retail_BE_ServicePackageItem'																								,'{"Editor":"retail-be-package-packageitem-chargingpolicy"}'),
('E548DC54-6664-45E6-B5CF-9B84D046D782','VolumeItem','Volume','Retail_BE_ServicePackageItem'																												,'{"Editor":"retail-be-package-packageitem-volume"}'),
('B2255268-649B-4648-8584-C00AB98E56DE','Regular','Regular','Retail_BE_ActionBPDefinition', '{"Editor":"retail-be-actionbpdefinition-definitionsettings-regular"}'),
('B2DF0389-BF8F-4289-887B-C7E4D472227A','Reactivate Teles Switch','Reactivate Teles Switch','Retail_BE_ProvisionerDefinition'																				,'{"DefinitionEditor":"retail-be-provisioner-definitionsettings-reactivatetelesswitch", "RuntimeEditor":"retail-be-provisioner-runtimesettings-reactivatetelesswitch"}'),
('D6D3FBCC-FB40-44E4-9FA1-DD7F1DBB6751','Weekly Recurring Period','Weekly','Retail_BE_RecurringPeriod'																										,'{"Editor":"retail-be-recurringperiod-weekly"}'),
('A7F4E906-CFB0-48FB-843A-F25DBACB934E','RadiusSQL','Radius SQL','Retail_BE_ProvisionerDefinition'																											,'{"DefinitionEditor":"retail-be-provisioner-definitionsettings-radiussql", "RuntimeEditor":"retail-be-provisioner-runtimesettings-radiussql"}'),

('E5EBA8E1-B0DC-4977-B770-2B9F62DCBC17','DurationTariff','Duration Tariff','Retail_BE_ChargingPolicyPartType'							,'{"PartTypeExtensionName":"Retail_BE_ChargingPolicyPart_DurationTariff"}'),
('7E54E33D-EF53-4DA9-9786-DC6721BEF618','RateValue','Rate Value','Retail_BE_ChargingPolicyPartType'										,'{"PartTypeExtensionName":"Retail_BE_ChargingPolicyPart_RateValue"}'),

('497557D1-399E-4AF5-BA10-A03338D1CAF4','FixedBalanceAlertThreshold','Fixed','Retail_BE_AccountBalance_AlertThreshold_PostPaidAccount'											,'{"Editor":"retail-be-balancealertrule-threshold-fixed"}'),
('30B37A0A-63D8-4323-899B-3A2782FC5A05','PercentageBalanceAlertThreshold','Percentage','Retail_BE_AccountBalance_AlertThreshold_PostPaidAccount'								,'{"Editor":"retail-be-balancealertrule-threshold-percentage"}'),

('FB2D7DC4-AF79-4068-8452-1058AF7544D7','PriceVoiceEventStep','PriceVoiceEventStep','VR_GenericData_DataTransformationStepConfig'							,'{"Editor":"retail-voice-pricevoiceeventstep","StepPreviewUIControl":"retail-voice-pricevoiceeventstep-preview"}'),
('7F43E2A1-2F27-4AB2-B7A4-C74A9F6B704D','InternationalIdentificationStep','InternationalIdentificationStep','VR_GenericData_DataTransformationStepConfig'	,'{"Editor":"retail-voice-internationalidentificationstep","StepPreviewUIControl":"retail-voice-internationalidentificationstep-preview"}'),
('7AD561D3-0650-4345-8FFD-D51A10C656BE','AccountIdentificationStep','AccountIdentificationStep','VR_GenericData_DataTransformationStepConfig'				,'{"Editor":"retail-voice-accountidentificationstep","StepPreviewUIControl":"retail-voice-accountidentificationstep-preview"}'),
('60BE8BF8-692D-4111-8038-63F7BCB62DAE','RetrieveFinancialInfoStep','RetrieveFinancialInfoStep','VR_GenericData_DataTransformationStepConfig'				,'{"Editor":"retail-be-retrievefinancialinfostep","StepPreviewUIControl":"retail-be-retrievefinancialinfostep-preview"}'),

('2FF81206-1E07-4E66-9E35-7F53BF049AB3','Retail_Voice_VoiceServiceType','Voice','Retail_BE_ServiceTypeExtendedSettingsConfig'								,'{"Editor":"retail-voice-voiceservicetype"}'),
('B0B5BC1F-E899-4AE5-AEFB-4FCD5D1BA140','Retail_Voice_StandardPolicyEvaluator','Standard','Retail_Voice_VoiceChargingPolicyEvaluatorConfig'					,'{"Editor":"retail-voice-standardpolicyevaluator"}'),
('1A73D2E9-1419-4B41-AD2B-6AB04930466B','DIDAccountIdentification','DID Account Identification','Retail_Voice_AccountIdentification'						,'{"Editor":"retail-voice-didaccountidentification"}'),
('6F57934D-DC86-473E-A8E5-5B24289D2086','DIDInternationalIdentification','DID International Identification','Retail_Voice_InternationalIdentification'		,'{"Editor":"retail-voice-didinternationalidentification"}'),
('D65AC3F8-3E92-4B48-AE0B-1F25C588916D','RuleInternationalIdentification','Rule International Identification','Retail_Voice_InternationalIdentification'	,'{"Editor":"retail-voice-ruleinternationalidentification"}'),

('BA8A44F4-506F-4B5D-8784-7765FB170E94','Retail_BE_RetailAccountPropertyEvaluator','Account Attribute','Retail_BE_RetailAccountObjectType_PropertyEvaluator'	,'{"Editor":"retail-be-retailaccount-propertyevaluator"}'),

('CD147065-88F3-4337-A625-8578708C4A53','Account Synchronizer','Account Synchronizer','VR_BEBridge_BESynchronizer'														,'{"Editor":"retail-be-account-synchronizer-editor"}'),
('BE74A60E-D312-4B4F-BD76-5B7BE81ABE62','Send Email', 'Send Email','VR_AccountBalance_BalanceAlert_VRAction'															,'{"Editor":"retail-be-accountaction-email"}'),

('A9475C63-ECA4-4C01-B9FF-11DF8AA4C157','Financial Transactions','Financial Transactions','Retail_BE_AccountViewDefinitionConfig'										,'{"Editor":"retail-be-accountviewdefinitionsettings-financialtransactions"}'),
('A8098DDE-51C2-4922-B346-32AFF202A4C1','Identification Rules','Identification Rules','Retail_BE_AccountViewDefinitionConfig'											,'{"Editor":"retail-be-accountviewdefinitionsettings-identificationrules"}'),
('71AB18ED-F2AC-4E71-B4E4-4826D092A201','Services','Services','Retail_BE_AccountViewDefinitionConfig'																	,'{"Editor":"retail-be-accountviewdefinitionsettings-services"}'),
('9A5B27E1-4928-4B71-B548-71C2F89444A5','SubAccounts','Sub Accounts','Retail_BE_AccountViewDefinitionConfig'															,'{"Editor":"retail-be-accountviewdefinitionsettings-subaccounts"}'),
('30064FB0-193D-4C41-A4B9-BFB7E236656B','AccountInfo','Account Info','Retail_BE_AccountViewDefinitionConfig'															,'{"Editor":"retail-be-accountviewdefinitionsettings-accountinfo"}'),
('0FF1E64B-15D4-45B8-B616-DDC9B0B78F74','Actions','Actions','Retail_BE_AccountViewDefinitionConfig'																		,'{"Editor":"retail-be-accountviewdefinitionsettings-actions"}'),
('BB2CBAE6-05A1-4132-A2E0-F6C761B273DA','Packages','Packages','Retail_BE_AccountViewDefinitionConfig'																	,'{"Editor":"retail-be-accountviewdefinitionsettings-packages"}'),
('871CEED6-F7E0-4D4F-9A30-8F2869B6E0EE','Child BE Relation','Child BE Relation','Retail_BE_AccountViewDefinitionConfig'													,'{"Editor":"retail-be-accountviewdefinitionsettings-childberelation"}'),
('DAB350C7-1451-42B2-9E04-215E252433E0','Portal Account','Portal Account','Retail_BE_AccountViewDefinitionConfig'														,'{"Editor":"retail-be-accountviewdefinitionsettings-portalaccount"}'),

('1819FC7B-B159-49CD-B678-261B3D0F41D5','Open360DegreeAccount','Open 360 Degree Account','Retail_BE_AccountActionDefinitionConfig'										,'{"Editor":"retail-be-accountactiondefinitionsettings-open360degreeaccount"}'),
('2504A630-D16B-43DC-8505-F85E3DFD0568','EditAccount','Edit Account','Retail_BE_AccountActionDefinitionConfig'															,'{"Editor":"retail-be-accountactiondefinitionsettings-editaccount"}'),
('fa494842-57c6-4972-a85f-3d9aa11c695d','ChangeStatusAction','Change Status','Retail_BE_AccountActionDefinitionConfig'													,'{"Editor":"retail-be-accountactiondefinitionsettings-changestatus"}'),
('17817576-4DE9-4C00-9BEF-0505007B4F53','ExecuteBusinessProcess','Execute Business Process','Retail_BE_AccountActionDefinitionConfig'									,'{"Editor":"retail-be-accountactiondefinitionsettings-bpaccount"}'),
('E7B4E2D1-1D81-4D11-9E8D-417BDD7A3C6E','ExportRatesAction','Export Rates','Retail_BE_AccountActionDefinitionConfig'													,'{"Editor":"retail-be-accountactiondefinitionsettings-exportrates"}'),

('385AB73F-D18D-4A1B-8552-FC4E6AC487DE','SubAccountCondition','Can Have Sub Accounts','Retail_BE_AccountConditionConfig'												,'{"Editor":"retail-be-accountcondition-subaccount"}'),
('EE17B999-5473-467F-A9BF-623EEF6CD409','FinancialAccountCondition','Is Financial','Retail_BE_AccountConditionConfig'													,'{"Editor":"retail-be-accountcondition-financialaccount"}'),
('1B1AF5DD-52EB-42C7-97EF-8CE824BB7D03','FilterGroupAccountCondition','Expression','Retail_BE_AccountConditionConfig'													,'{"Editor":"retail-be-accountcondition-filtergroup"}'),
('20aba92b-ecb3-497e-b136-59e4c71bd3b7','AssignableToPackage','Assignable To Package','Retail_BE_AccountConditionConfig'												,'{"Editor":"retail-be-accountcondition-assignabletopackage"}'),

('F123F0C6-42CF-4B6F-B3EB-F1AD5E0E8931','Assign Product and Packages Handler','Assign Product and Packages Handler','Retail_BE_AccountSynchronizerInsertHandlerConfig'	,'{"Editor":"retail-be-accountsynchronizerhandler-assignproductandpackages"}'),
('1CBD8BEF-14D6-4D47-BB9B-264ECC0E92B4','Portal Account Handler','Portal Account Handler','Retail_BE_AccountSynchronizerInsertHandlerConfig'							,'{"Editor":"retail-be-accountsynchronizerhandler-portalaccount"}'),

('8FEF1186-28BF-47FF-9C9B-3A2873F48F15','Fixed Charge','Fixed Charge','Retail_BE_AccountRecurringChargeEvaluator'														,'{"Editor":"retail-be-accountchargeevaluator-fixedcharge"}'),
('70D4A6AD-10CC-4F0B-8364-7D8EF3C044C4','Account Business Entity','Account BE','VR_GenericData_BusinessEntityDefinitionSettingsConfig'									,'{"Editor":"retail-be-accountbedefinitions-editor"}'),

('44F7D357-CD66-4397-A159-7A597A8C1164','ProductDefinition','Product Definition','VR_Common_VRComponentType'												,'{"Editor":"retail-be-productdefinition-settings"}'),
('CE9260A7-732F-4573-BEF8-9A3F8FC7BCC6','PackageDefinition','Package Definition','VR_Common_VRComponentType'												,'{"Editor":"retail-be-packagedefinition-settings"}'),

('76A889A4-9F93-4327-91C4-EE2F1EF2026E','Pricing','Pricing','Retail_BE_PackageDefinition'																				,'{"DefinitionEditor":"retail-be-packagedefinition-extendedsettings-pricing"}'),
('E326482A-9AB5-4715-848F-11CAF4940040','InvoiceRecurCharge','Invoice RecurCharge','Retail_BE_PackageDefinition'														,'{"DefinitionEditor":"retail-be-packagedefinition-extendedsettings-recurcharge"}'),

('82B558C6-CEF2-4318-8819-A8495097E770','VR_GenericData_BEParentChildRelationDefinition','Parent-Child Relation Definition','VR_Common_VRComponentType'		,'{"Editor":"vr-genericdata-beparentchildrelationdefinition-settings"}'),

('AC102D41-B0DB-4E02-A26B-DB8D6BFE47F3','Postpaid','Postpaid','Retail_BE_ProductDefinition'		,'{"DefinitionEditor":"retail-be-productdefinition-extendedsettings-postpaid"}'),
('360ADAB8-1516-4A3D-BDB7-0655C6A0965B','Prepaid','Prepaid','Retail_BE_ProductDefinition'		,'{"DefinitionEditor":"retail-be-productdefinition-extendedsettings-prepaid"}'),

('45C757B5-B2FE-44E5-943E-A9770A384AE9','Single Rule','Single Rule','Retail_BE_AccountRecurringChargeRuleSetSettings'			,'{"Editor":"retail-be-recurringchargerulesetsettings-singlerule"}'),
('1F5BF4F6-A2C5-408B-9E68-C6B1A32E6EF3','Apply First','Apply First','Retail_BE_AccountRecurringChargeRuleSetSettings'			,'{"Editor":"retail-be-recurringchargerulesetsettings-applyfirst"}'),

('FEB17AE0-6CD4-467B-A7FA-4FEE9D8538EC','Account Balance','Account Balance','VR_AccountBalance_AccountTypeExtendedSettingsConfig','{"Editor":"retail-be-extendedsettings-accountbalance"}'),

('70CF19AC-B860-4010-A544-B6D41F94F505','Retail_BE_AccountBEDefinition','Account BE','VR_Security_ViewTypeConfig'				,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"retail-be-accountbedefinition-vieweditor"}'),

('EB85EE78-78CE-437D-B13E-18DD15EABE54','Retail_BE_Visibility','Retail Business Entity','VRCommon_ModuleVisibility'																,'{"Editor":"retail-be-visibilityaccountdefinitions-management"}'),
('1bdacfe6-f050-4187-9e96-9647049605d3','RetailBE_VRNotification_VRAction_BalanceRetailAccountEMail','Balance Retail Account Email ','VR_Notification_VRActionDefinition'		,'{"Editor":"retail-be-actiondefinition-sendemailsettings"}'),
('820856b8-c29d-43d9-9950-18ae7af22bb9','RetailBE_VRNotification_VRAction_BalanceRetailAccountAction','Balance Retail Account Action','VR_Notification_VRActionDefinition'		,'{"Editor":"retail-be-actiondefinition-balancealertsettings"}'),
('fb232763-6ac1-49b5-a410-fa792980055c','RetailBE_AccountView_AccountHistory','Account History','Retail_BE_AccountViewDefinitionConfig'											,'{"Editor":"retail-be-accountviewdefinitionsettings-acounthistory"}'),
('7B651637-EEE9-4804-91E4-51ECC82D8DD0','ChargeableEntity','Chargeable Entity','VRCommon_GenericLKUPDefinition'																	,'{"DefinitionEditor":"retail-be-chargeableentitydefinitionsettings"}'),

('52525041-7A8B-4AE1-9599-A3F34A87CB38','Retail Account Balance','Retail Account Balance','VR_AccountBalance_NotificationTypeExtendedSettingsConfig'							,'{"Editor":"retail-be-accountbalancenotificationtype-settings"}'),
('F21A72DC-48BF-43F4-A2A7-97E72F75B391','Operator Setting','Operator Setting','Retail_BE_AccountPartDefinition'																	,'{"DefinitionEditor":"retail-be-accounttype-part-definition-operator","RuntimeEditor":"retail-be-accounttype-part-runtime-operator"}'),
('D879B75A-7F13-4543-8EDE-961327CB3E33','Inv To Acc Relation','Inv To Acc Relation','VR_InvToAccBalanceRelation_RelationDefinitionExtendedSettings','{"Editor":"retail-invtoaccbalancerelation-definition-accountextendedsettings"}'),
('B78610BA-4CA2-4E60-8143-73CEF6E99D14','Retail_BE_PackageExtendedSettingsConfig_PricingPackageSettings','Pricing Package','Retail_BE_PackageExtendedSettingsConfig','{"Editor":"retail-be-pricingpackagesettings-management"}'),
('6A33AFF5-C8D0-41BA-906D-3F9CBB8A7D3E','Financial Account Balance','Financial Account Balance','VR_AccountBalance_AccountTypeExtendedSettingsConfig','{"Editor":"retail-be-extendedsettings-financialaccountbalance"}'),
('3ED8A0C3-99E7-486A-A560-5789BA1DEAEE','Financial Account Definition','Financial Account Definition','VR_Common_VRComponentType','{"Editor":"retail-be-financialaccountdefinition-settings"}'),
('1F7F8131-E49E-4A1D-802A-0432BA92EBAB','PeriodicRecurringCharge','Periodic Recurring Charge','Retail_BE_RecurringChargeEvaluator','{"DefinitionEditor":"retail-be-packagedefinition-recurcharge-evaluator-periodic"}'),
('2C11E2C0-D54B-41DF-95FA-1FBCFD5C93B0','OneTimeRecurringCharge','One Time Recurring Charge','Retail_BE_RecurringChargeEvaluator','{"DefinitionEditor":"retail-be-packagedefinition-recurcharge-evaluator-onetime"}'),
('006BA22A-439D-4BF5-B8C2-0254C2F6B40C','RetailBE_AccountView_FinancialAccount','Financial Account','Retail_BE_AccountViewDefinitionConfig','{"Editor":"retail-be-accountviewdefinitionsettings-financialaccount"}'),
('FEE1242D-8664-4C64-B203-BAE3290DCF3F','ConditionGroup','Condition Group','Retail_BE_AccountConditionConfig','{"Editor":"retail-be-accountcondition-conditiongroup"}'),
('0953E01C-8F4E-4C01-A714-FE55F62882A8','RetailBE_FinancialAccount_PostpaidFinancialAccount','Postpaid','Retail_BE_FinancialAccountDefinition','{"Editor":"retail-be-financialaccountdefinition-postpaid"}'),
('4F2A2B2F-CAA6-423A-A08F-39DE8587E3BA','Service Type','Service Type','VRCommon_OverriddenConfiguration','{"Editor":"retail-be-overriddenconfiguration-servicetype"}'),
('22C9E36D-D328-4220-83E8-E45AD1B005D8','AccountType','Account Type','VRCommon_OverriddenConfiguration','{"Editor":"retail-be-overriddenconfiguration-accounttype"}'),
('63E3987B-302B-42BA-8E61-A7762FA7BFD3','DefaultFinancialAccountLocator','Default Locator','Retail_BE_FinancialAccountLocator','{"Editor":"retail-be-accountbedefinition-financialaccountlocator-default"}'),
('C01FA066-28C8-4225-9F59-39F5EECF86ED','AccountPartDefinitionOverriddenConfiguration','Account Part Definition','VRCommon_OverriddenConfiguration','{"Editor":"retail-overriddenconfiguration-accountpartdefinition"}'),
('9b59a5e8-923a-4e9e-8338-9549150ec88c','Account City','Account City','VR_InvoiceType_SerialNumberParts','{"Editor":"retail-invoicetype-serialnumber-accountcity"}'),

('3A88F4C6-5CCE-4A31-A74E-E83BF73A6892','Retail_BE_DataRecordFieldFormula_ZoneEvaluator','Zone Evaluator','VR_Generic_DataRecordFieldFormula','{"Editor":"retail-be-datarecordtypefields-formula-zoneevaluator"}'),
('B3D9B0A4-B751-4544-8A7A-6764687059ED','Retail_BE_DataRecordFieldFormula_ParentAccount','Retail Parent Account','VR_Generic_DataRecordFieldFormula','{"Editor":"retail-be-datarecordtypefields-formula-parentaccount"}'),
('fb3b7f00-0d58-4a11-9be8-dcd9a9212c58','ChangeStatus','Change Status','Retail_BE_AccountProvisionerPostAction','{"Editor" : "retail-be-actionbpdefinition-definitionpostaction-changestatus"}'),
('889c2da2-5bba-4316-a245-521e85e3fbe8','RevertStatus','Revert Status','Retail_BE_AccountProvisionerPostAction','{"Editor" : "retail-be-actionbpdefinition-definitionpostaction-revertstatus"}'),
('95183b89-056a-410a-b920-91ec6a134f82','Portal Account Extra Field','Portal Account Extra Field','Retail_BE_AccountExtraFieldDefinitionConfig','{"Editor":"retail-be-accountextrafield-portalaccount"}'),

('b573969d-05a2-4c92-a856-1f846557520c','Topup','Top Up','Retail_BE_OperatorDeclarationServiceConfig','{"Editor":"retail-be-operatordeclarationservice-topup"}'),
('696fcb6c-ee81-4c34-a390-6793ecc7252d','Voice','Voice','Retail_BE_OperatorDeclarationServiceConfig','{"Editor":"retail-be-operatordeclarationservice-postpaidcdr"}'),
('0f35bd74-81d4-4cf3-950d-98de8cdad7d9','SMS','SMS','Retail_BE_OperatorDeclarationServiceConfig','{"Editor":"retail-be-operatordeclarationservice-postpaidsms"}'),
('81A1FFFA-8AE6-41F0-A3E2-ED6457F72FDB','Retail_BE_AccountBulkActionsSettingConfig_SendEmail','Send Rates','Retail_BE_AccountBulkActionSettingsConfig','{"Editor":"retail-be-accountbulkactionsettings-sendrates"}'),
('6388F486-9BB7-47D2-B16C-31C9FDCE9A8D','Taxes','Taxes','Retail_BE_AccountPartDefinition'		,'{"DefinitionEditor":"retail-be-accounttype-part-definition-taxes","RuntimeEditor":"retail-be-accounttype-part-runtime-taxes"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[ExtensionConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------

END


--[sec].[Module]------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E7855563-9173-47F0-A8E7-4C47CD2A1F42','Voice Entites'			,null,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03',null,3,1),
('E70468BE-4793-466B-9B83-BAF2535D64D2','Network Elements'		,null,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03',null,4,0),

('AD9EEB65-70A3-4F57-B261-79F40D541E23','Business CRM'			,null,null,'/images/menu-icons/plug.png',20,1),
('66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B','Pricing Management'	,null,null,'/images/menu-icons/Sale Area.png',30,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
----------------------------------------------------------------------------------------------------
END

GO
--delete useless views from TOne product such 'My Scheduler Service', 'Organizational Charts'
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308',--'My Scheduler Service'
										'DCF8CA21-852C-41B9-9101-6990E545509D',--'Organizational Charts'
										'2D39B12D-8FBF-4D4E-B2A5-5E3FE57580DF',--'Locked Sessions'
										'0F111ADC-B7F6-46A4-81BC-72FFDEB305EB'--,'Time Zone'
										)
--[sec].[View]--------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9F5B379C-1576-4078-9999-3218B329FEAC','Packages','Packages','#/view/Retail_BusinessEntity/Views/Package/PackageManagement'									,'66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B',null,null,null,'{"$type":"Retail.BusinessEntity.Business.PackageViewSettings, Retail.BusinessEntity.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',4,0),

('E68EFF9C-879E-4A6C-B412-8E225A966571','Charging Policies','Charging Policies','#/view/Retail_BusinessEntity/Views/ChargingPolicy/ChargingPolicyManagement'	,'66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B','Retail_BE/ChargingPolicy/GetFilteredChargingPolicies',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2,0),
('D3799B4D-5B86-4665-BF03-94AFF7F00E21','Product Families','Product Families','#/view/Retail_BusinessEntity/Views/ProductFamily/ProductFamilyManagement'				,'66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B',null,null,null,'{"$type":"Retail.BusinessEntity.Business.ProductViewSettings, Retail.BusinessEntity.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2,0),

('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','System Logs','System Logs',null,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Common.Business.MasterLogViewSettings, Vanrise.Common.Business","Items":[{"PermissionName":"VRCommon_System_Log: View General Logs","Directive":"vr-log-entry-search","Title":"General"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-log-search","Title":"Data Source"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-importedbatch-search","Title":"Imported Batch"},{"PermissionName":"VR_Integration_DataProcesses: View Logs", "Directive":"bp-instance-log-search","Title":"Business Process"},{"PermissionName":"VRCommon_System_Log: View Action Audit","Directive":"vr-common-actionaudit-search","Title":"Action Audit"}]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15,0),

('D4385711-5F67-4EC4-BBEC-B5B1BF767188','Account Parts','Account Parts','#/view/Retail_BusinessEntity/Views/AccountPartDefinition/AccountPartDefinitionManagement'	,'A459D3D0-35AE-4B0E-B267-54436FDA729A','Retail_BE/AccountPartDefinition/GetFilteredAccountPartDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5,0),
('296B2785-172C-4332-9846-D32FE3166C62','Service Types','Service Type','#/view/Retail_BusinessEntity/Views/ServiceType/ServiceTypeManagement'			,'A459D3D0-35AE-4B0E-B267-54436FDA729A','Retail_BE/ServiceType/GetFilteredServiceTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3,0),
('9C3BE71A-81D8-4A02-A1F7-FBBE6536BBBB','Account Types','Account Types','#/view/Retail_BusinessEntity/Views/AccountType/AccountTypeManagement'						,'A459D3D0-35AE-4B0E-B267-54436FDA729A','Retail_BE/AccountType/GetFilteredAccountTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2,0)

--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank],[IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);
----------------------------------------------------------------------------------------------------
END


--[sec].[BusinessEntityModule]------------------------201 to 300----------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FC455C73-490D-48F2-A2B8-C2385137DE0F','Business CRM'			,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),
('783139D6-4BC9-4EC5-8191-A809937E590D','Pricing'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([ID],[Name],[ParentId],[BreakInheritance])
	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end


--[sec].[BusinessEntity]-------------------5401 to 5700-------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0B08F2E1-6518-46FF-B669-0256CF9BB7D4','Retail_BE_Subscriber','Subscriber'							,'FC455C73-490D-48F2-A2B8-C2385137DE0F',0,'["View","Add","Edit","Provision","Block","Unblock"]'),
('A2DE8C67-B8AE-4520-BB48-448EE98B7251','Retail_BE_Dealer','Dealer'									,'FC455C73-490D-48F2-A2B8-C2385137DE0F',0,'["View","Add","Edit"]'),
('0E89CCF7-9240-4D1B-8A01-F91957ECA321','Retail_BE_Operator','Operator'								,'FC455C73-490D-48F2-A2B8-C2385137DE0F',0,'["View","Add","Edit"]'),

('1F99405A-9FDD-4E73-BDC7-EEE33FD01D7C','Retail_BE_Product','Product'								,'783139D6-4BC9-4EC5-8191-A809937E590D',0,'["View","Add","Edit"]'),
('BE4E62AA-CCE6-48AD-94ED-87D54A605D35','Retail_BE_Package','Package'								,'783139D6-4BC9-4EC5-8191-A809937E590D',0,'["View","Add","Edit","View Assigned Subscribers","Assign Subscribers"]'),
('2045E39B-B9AA-4720-AD1E-7DEF11985335','Retail_BE_ChargingPolicy','Charging Policy'				,'783139D6-4BC9-4EC5-8191-A809937E590D',0,'["View","Add","Edit"]'),
('32E40D20-A8B9-4286-AADE-F4E229D054A9','Retail_BE_CreditClasses','Credit Classes'					,'783139D6-4BC9-4EC5-8191-A809937E590D',0,'["View","Add","Edit"]'),

('B5634DD0-11C7-4074-B902-8A7C0B68E5AC','Retail_CDRRules','CDR Rules'								,'B6B8F582-4759-43FB-9220-AA7662C366EA',0,'["View","Add","Edit"]')

--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
END

--[sec].[SystemAction]------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('Retail_BE/Package/GetFilteredPackages',null),
('Retail_BE/Package/GetPackage',null),
('Retail_BE/Package/GetPackagesInfo',null),
('Retail_BE/Package/AddPackage',null),
('Retail_BE/Package/UpdatePackage',null),

('Retail_BE/Account/GetFilteredAccounts',null),
('Retail_BE/Account/GetAccount',null),
('Retail_BE/Account/GetAccountName',null),
('Retail_BE/Account/AddAccount',null),
('Retail_BE/Account/UpdateAccount',null),

('Retail_BE/AccountPackage/GetFilteredAccountPackages',null),
('Retail_BE/AccountPackage/GetAccountPackage',null),
('Retail_BE/AccountPackage/AddAccountPackage',null),

('Retail_BE/ChargingPolicy/GetFilteredChargingPolicies','Retail_BE_ChargingPolicy: View'),
('Retail_BE/ChargingPolicy/GetChargingPolicy',null),
('Retail_BE/ChargingPolicy/AddChargingPolicy','Retail_BE_ChargingPolicy: Add'),
('Retail_BE/ChargingPolicy/UpdateChargingPolicy','Retail_BE_ChargingPolicy: Edit'),
('Retail_BE/AccountType/GetFilteredAccountTypes','VR_SystemConfiguration: View'),
('Retail_BE/AccountType/GetAccountType',null),
('Retail_BE/AccountType/GetAccountTypesInfo',null),
('Retail_BE/AccountType/GetAccountTypePartDefinitionExtensionConfigs',null),
('Retail_BE/AccountType/AddAccountType','VR_SystemConfiguration: Add'),
('Retail_BE/AccountType/UpdateAccountType','VR_SystemConfiguration: Edit'),
('Retail_BE/ServiceType/GetFilteredServiceTypes','VR_SystemConfiguration: View'),
('Retail_BE/ServiceType/GetServiceTypesInfo',null),
('Retail_BE/ServiceType/GetServiceType',null),
('Retail_BE/ServiceType/GetServiceTypeChargingPolicyDefinitionSettings',null),
('Retail_BE/ServiceType/UpdateServiceType','VR_SystemConfiguration: Edit'),
('Retail_BE/StatusDefinition/GetFilteredStatusDefinitions','VR_SystemConfiguration: View'),
('Retail_BE/StatusDefinition/AddStatusDefinition','VR_SystemConfiguration: Add'),
('Retail_BE/StatusDefinition/UpdateStatusDefinition','VR_SystemConfiguration: Edit'),
('Retail_BE/StatusDefinition/GetStatusDefinition',null),
('Retail_BE/StatusDefinition/GetStatusDefinitionsInfo',null),

('Retail_BE/AccountPartDefinition/GetFilteredAccountPartDefinitions','VR_SystemConfiguration: View'),
('Retail_BE/AccountPartDefinition/GetAccountPartDefinition',null),
('Retail_BE/AccountPartDefinition/GetAccountPartDefinitionsInfo',null),
('Retail_BE/AccountPartDefinition/GetAccountPartDefinitionExtensionConfigs',null),
('Retail_BE/AccountPartDefinition/AddAccountPartDefinition','VR_SystemConfiguration: Add'),
('Retail_BE/AccountPartDefinition/UpdateAccountPartDefinition','VR_SystemConfiguration: Edit'),

('Retail_BE/ActionDefinition/AddActionDefinition','VR_SystemConfiguration: Add'),
('Retail_BE/ActionDefinition/UpdateActionDefinition','VR_SystemConfiguration: Edit'),
('Retail_BE/ActionDefinition/GetFilteredActionDefinitions','VR_SystemConfiguration: View'),
('Retail_BE/ActionDefinition/GetActionDefinition',null),
('Retail_BE/ActionDefinition/GetActionBPDefinitionExtensionConfigs',null),
('Retail_BE/ActionDefinition/GetProvisionerDefinitionExtensionConfigs',null),
('Retail_BE/ActionDefinition/GetActionDefinitionsInfo',null),

('Retail_BE/CreditClass/GetFilteredCreditClasses','Retail_BE_CreditClasses:  View'),
('Retail_BE/CreditClass/GetCreditClass',null),
('Retail_BE/CreditClass/AddCreditClass','Retail_BE_CreditClasses:  Add'),
('Retail_BE/CreditClass/UpdateCreditClass','Retail_BE_CreditClasses:  Edit'),
('Retail_BE/CreditClass/GetCreditClassesInfo',null),

('Retail_BE/Switch/GetFilteredSwitches',null),
('Retail_BE/Switch/GetSwitchSettingsTemplateConfigs',null),
('Retail_BE/Switch/AddSwitch',null),
('Retail_BE/Switch/UpdateSwitch',null),
('Retail_BE/Switch/GetSwitch',null),

('Retail_BE/ProductFamily/GetFilteredProductFamilies',null),
('Retail_BE/ProductFamily/GetProductFamilyEditorRuntime',null),
('Retail_BE/ProductFamily/AddProductFamily',null),
('Retail_BE/ProductFamily/UpdateProductFamily',null),
('Retail_BE/ProductFamily/GetProductFamiliesInfo',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
		[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);
----------------------------------------------------------------------------------------------------
END

--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D0EE9BF8-385E-48EF-B989-A87666A00072','Retail_BE_ChargingPolicy','Charging Policy'									,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-chargingpolicy-selector","ManagerFQTN":"Retail.BusinessEntity.Business.ChargingPolicyManager, Retail.BusinessEntity.Business","IdType":"System.Int32"}'),
('1BC07506-D535-4FF8-AC61-C8FDAAF37038','Retail_BE_AccountType','Account Type'											,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-accounttype-selector","GroupSelectorUIControl":"","ManagerFQTN":"Retail.BusinessEntity.Business.AccountTypeManager, Retail.BusinessEntity.Business","IdType":"System.Guid"}'),
('C0C76DB1-4876-4E0D-9B59-CA89120E6BE9','Retail_BE_Package','Package'													,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-package-selector","ManagerFQTN":"Retail.BusinessEntity.Business.PackageManager, Retail.BusinessEntity.Business","IdType":"System.Int32"}'),
('BFAD446F-7129-45B1-94BF-FEBD290F394D','Retail_BE_ServiceType','Service Type'											,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-servicetype-selector","ManagerFQTN":"Retail.BusinessEntity.Business.ServiceTypeManager, Retail.BusinessEntity.Business","IdType":"System.Guid"}'),
('41767702-B520-4811-96BE-103F96B81177','Retail_BusinessEntity_Product','Product'										,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-product-selector","GroupSelectorUIControl":"","ManagerFQTN":"Retail.BusinessEntity.Business.ProductManager,Retail.BusinessEntity.Business","IdType":"System.Int32"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[bp].[BPDefinition]----------------------5001 to 6000---------------------------------------------
BEGIN 
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('049A1012-A30B-457D-96AE-51E1CA388191','Retail_BE_ActionBPInputArgument_17817576-4de9-4c00-9bef-0505007b4f53','Account Action','Retail.BusinessEntity.MainActionBPs.RegularActionBP,Retail.BusinessEntity.MainActionBPs','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"ExtendedSettings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.BPAccountActionBPDefinitionSettings, Retail.BusinessEntity.MainExtensions"}}'),
('21AACDC8-4DA8-4A84-974A-54CFED0B1EFB','Retail.BusinessEntity.BP.Arguments.AccountRecurringChargeEvaluatorProcessInput','Account Recurring Charge Evaluator','Retail.BusinessEntity.BP.AccountRecurringChargeEvaluatorProcess,Retail.BusinessEntity.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"retail-be-recurringcharge-process","ScheduledExecEditor":"retail-be-recurringcharge-task","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Retail.BusinessEntity.Business.AccountRecurringChargeEvaluatorBPDefinitionSettings, Retail.BusinessEntity.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}'),
('4d3cf378-c8ee-4e13-860c-643fa4775220','Retail.BusinessEntity.BP.Arguments.AccountBulkActionProcessInput','Account Bulk Action','Retail.BusinessEntity.BP.AccountBulkActionProcess,Retail.BusinessEntity.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END

Delete from [common].[Setting] where ID = '4047054E-1CF4-4BE6-A005-6D4706757AD3'--,'Session Lock'
--[common].[Setting]---------------------------201 to 300-------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Retail","VersionNumber":"version #VersionNumber# ~ #VersionDate#"}}',1),
('554149f4-6361-4e32-8b3c-dc2a30084c77','Invoice Settings','Retail_BE_RetailInvoiceSettings','Business Entities','{"Editor":"retail-be-retailinvoicesettings"}',null,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
END

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A392AE14-74F2-4DD8-B7F6-E6728C9559A0','VR_Common_ComponentType_44f7d357-cd66-4397-a159-7a597a8c1164','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_ComponentType__ViewHistoryItem"}'),
('316AC1CB-AFD0-47B6-B3A0-0A6B7FF44DF6','VR_Common_ComponentType_ce9260a7-732f-4573-bef8-9a3f8fc7bcc6','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_ComponentType__ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);

--Delete from [runtime].[SchedulerTaskActionType] where Id='0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9' --Exchange Rate



--[runtime].[RuntimeNodeConfiguration]--------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A0B4A18A-C0C2-4ABE-AF6A-C7839BD6BD36','Default','{"$type":"Vanrise.Runtime.Entities.RuntimeNodeConfigurationSettings, Vanrise.Runtime.Entities","Processes":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities]], mscorlib","2b7dfb5b-529f-4298-a3b1-0ad5f7f7122b":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Scheduler Service Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":1,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","8382d9ca-05fc-485c-807c-3f6f5f617fd5":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Scheduler Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Runtime.SchedulerService, Vanrise.Runtime","ServiceTypeUniqueName":"Vanrise.Runtime.SchedulerService, Vanrise.Runtime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}},"a40db74d-f99c-4a2d-a571-cb626506a3d8":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Regulator Service Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":1,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","8554cd70-01c8-4b7d-bbb8-37c976b8da25":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Regulator Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.BusinessProcess.BPRegulatorRuntimeService, Vanrise.BusinessProcess","ServiceTypeUniqueName":"Vanrise.BusinessProcess.BPRegulatorRuntimeService, Vanrise.BusinessProcess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}},"93546ace-d83a-4b17-966f-281a0b5ef1a3":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Services Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":3,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","bfc6d07d-1355-4c34-ac8e-d86bad74b413":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.BusinessProcess.BusinessProcessService, Vanrise.BusinessProcess","ServiceTypeUniqueName":"VR_BusinessProcess_BusinessProcessService","Interval":"00:00:01"}}}}}},"4fe96f64-1c25-4c65-9a18-2f9e0384547d":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Queue Regulator Service Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":1,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","f4ddab13-00c9-4a3c-962d-e7b298f94471":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Queue Regulator Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Queueing.QueueRegulatorRuntimeService, Vanrise.Queueing","serviceInstanceIndex":0,"ServiceTypeUniqueName":"Vanrise.Queueing.QueueRegulatorRuntimeService, Vanrise.Queueing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}},"1c395b1d-9ec6-4b87-b8e7-35370c09bc26":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Queue Activation Services Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":3,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","ebf8dd5d-5082-46b9-8ca7-77f1af6d8268":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Queue Activation Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Queueing.QueueActivationRuntimeService, Vanrise.Queueing","ServiceTypeUniqueName":"VR_Queueing_QueueActivationRuntimeService","Interval":"00:00:01"}}}}}},"45efd1ba-d606-41a7-a43d-2e9b1f8de040":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Summary Queue Activation Services Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":3,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","555ce82f-f932-4494-89a3-1838e5cbe8a0":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Summary Queue Activation Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Queueing.SummaryQueueActivationRuntimeService, Vanrise.Queueing","ServiceTypeUniqueName":"VR_Queueing_SummaryQueueActivationRuntimeService","Interval":"00:00:01"}}}}}},"49fa4851-39c4-45e9-8f01-12b66a15d96a":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Data Source Services Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":3,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","650f9943-4fe6-4087-bb67-eafb0d19ac02":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Data Source Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Integration.Business.DataSourceRuntimeService, Vanrise.Integration.Business","ServiceTypeUniqueName":"Vanrise.Integration.Business.DataSourceRuntimeService, Vanrise.Integration.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}},"0057b8ad-9a79-41ba-826a-4a27aa76e6c0":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Big Data Services Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":2,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","4f90e325-5ddd-4544-98f9-fa5d55a0c594":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Big Data Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Common.Business.BigDataRuntimeService, Vanrise.Common.Business","ServiceTypeUniqueName":"Vanrise.Common.Business.BigDataRuntimeService, Vanrise.Common.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}}}}',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNodeConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);


--[runtime].[RuntimeNode]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('CA664C3B-2E31-4542-BEEB-89E7367E090C','A0B4A18A-C0C2-4ABE-AF6A-C7839BD6BD36','Node 1',null,null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNode] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[RuntimeNodeConfigurationID] = s.[RuntimeNodeConfigurationID],[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[RuntimeNodeConfigurationID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);
