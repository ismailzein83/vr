﻿/*
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
('17817576-4DE9-4C00-9BEF-0505007B4F53','ExecuteBusinessProcess','Execute Business Process','Retail_BE_AccountActionDefinitionConfig'									,'{"Editor":"retail-be-accountactiondefinitionsettings-bpaccount"}'),

('385AB73F-D18D-4A1B-8552-FC4E6AC487DE','SubAccountCondition','Can Have Sub Accounts','Retail_BE_AccountConditionConfig'												,'{"Editor":"retail-be-accountcondition-subaccount"}'),
('EE17B999-5473-467F-A9BF-623EEF6CD409','FinancialAccountCondition','Is Financial','Retail_BE_AccountConditionConfig'													,'{"Editor":"retail-be-accountcondition-financialaccount"}'),
('1B1AF5DD-52EB-42C7-97EF-8CE824BB7D03','FilterGroupAccountCondition','Expression','Retail_BE_AccountConditionConfig'													,'{"Editor":"retail-be-accountcondition-filtergroup"}'),

('F123F0C6-42CF-4B6F-B3EB-F1AD5E0E8931','Assign Product and Packages Handler','Assign Product and Packages Handler','Retail_BE_AccountSynchronizerInsertHandlerConfig'	,'{"Editor":"retail-be-accountsynchronizerhandler-assignproductandpackages"}'),
('1CBD8BEF-14D6-4D47-BB9B-264ECC0E92B4','Portal Account Handler','Portal Account Handler','Retail_BE_AccountSynchronizerInsertHandlerConfig'							,'{"Editor":"retail-be-accountsynchronizerhandler-portalaccount"}'),

('8FEF1186-28BF-47FF-9C9B-3A2873F48F15','Fixed Charge','Fixed Charge','Retail_BE_AccountRecurringChargeEvaluator'														,'{"Editor":"retail-be-accountchargeevaluator-fixedcharge"}'),
('70D4A6AD-10CC-4F0B-8364-7D8EF3C044C4','Account Business Entity','Account BE','VR_GenericData_BusinessEntityDefinitionSettingsConfig'									,'{"Editor":"retail-be-accountbedefinitions-editor"}'),

('44F7D357-CD66-4397-A159-7A597A8C1164','ProductDefinition','Product Definition','VR_Common_VRComponentType'												,'{"Editor":"retail-be-productdefinition-settings"}'),
('82B558C6-CEF2-4318-8819-A8495097E770','VR_GenericData_BEParentChildRelationDefinition','Parent-Child Relation Definition','VR_Common_VRComponentType'		,'{"Editor":"vr-genericdata-beparentchildrelationdefinition-settings"}'),

('AC102D41-B0DB-4E02-A26B-DB8D6BFE47F3','Postpaid','Postpaid','Retail_BE_ProductDefinition'		,'{"DefinitionEditor":"retail-be-productdefinition-extendedsettings-postpaid"}'),
('360ADAB8-1516-4A3D-BDB7-0655C6A0965B','Prepaid','Prepaid','Retail_BE_ProductDefinition'		,'{"DefinitionEditor":"retail-be-productdefinition-extendedsettings-prepaid"}'),

('45C757B5-B2FE-44E5-943E-A9770A384AE9','Single Rule','Single Rule','Retail_BE_AccountRecurringChargeRuleSetSettings'			,'{"Editor":"retail-be-recurringchargerulesetsettings-singlerule"}'),
('1F5BF4F6-A2C5-408B-9E68-C6B1A32E6EF3','Apply First','Apply First','Retail_BE_AccountRecurringChargeRuleSetSettings'			,'{"Editor":"retail-be-recurringchargerulesetsettings-applyfirst"}'),

('FEB17AE0-6CD4-467B-A7FA-4FEE9D8538EC','Account Balance','Account Balance','VR_AccountBalance_AccountTypeExtendedSettingsConfig','{"Editor":"retail-be-extendedsettings-accountbalance"}'),

('70CF19AC-B860-4010-A544-B6D41F94F505','Retail_BE_AccountBEDefinition','Account BE','VR_Security_ViewTypeConfig'				,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"retail-be-accountbedefinition-vieweditor"}'),

('EB85EE78-78CE-437D-B13E-18DD15EABE54','Retail_BE_Visibility','Retail Business Entity','VRCommon_ModuleVisibility'							,'{"Editor":"retail-be-visibilityaccountdefinitions-management"}')
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
('EB303A61-929A-4D33-BF50-18F40308BC86','Reports & Dashboards'	,null,null,'/images/menu-icons/busines intel.png',10,1),-- since there is no BI Module here
('1C7569FA-43C9-4853-AE4C-1152746A34FD','Rules'					,null,null,'/images/menu-icons/other.png',7,0),
('E7855563-9173-47F0-A8E7-4C47CD2A1F42','Voice Entites'			,null,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03',null,3,1),
('AD9EEB65-70A3-4F57-B261-79F40D541E23','Business CRM'			,null,null,'/images/menu-icons/plug.png',6,1),
('66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B','Pricing Management'	,null,null,'/images/menu-icons/Sale Area.png',8,1),
('E70468BE-4793-466B-9B83-BAF2535D64D2','Network Elements'		,null,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03',null,4,0),
('6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C','Billing'				,null,null,'/images/menu-icons/billing.png',9,0)
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
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308','DCF8CA21-852C-41B9-9101-6990E545509D')
--[sec].[View]--------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9F5B379C-1576-4078-9999-3218B329FEAC','Packages','Packages Management','#/view/Retail_BusinessEntity/Views/Package/PackageManagement'									,'66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B',null,null,null,'{"$type":"Retail.BusinessEntity.Business.PackageViewSettings, Retail.BusinessEntity.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,4,null),

('E68EFF9C-879E-4A6C-B412-8E225A966571','Charging Policies','Charging Policies Management','#/view/Retail_BusinessEntity/Views/ChargingPolicy/ChargingPolicyManagement'	,'66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B','Retail_BE/ChargingPolicy/GetFilteredChargingPolicies',null,null,null,'372ed3cb-4b7b-4464-9abf-59cd7b08bd23',0,2,null),
('D3799B4D-5B86-4665-BF03-94AFF7F00E21','Product Families','Product Families','#/view/Retail_BusinessEntity/Views/ProductFamily/ProductFamilyManagement'				,'66F2DD29-5EAF-4AEE-97C7-A5FD9CCAD47B',null,null,null,'{"$type":"Retail.BusinessEntity.Business.ProductViewSettings, Retail.BusinessEntity.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,2,null),

('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','Event Logs','Event Logs',null,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Common.Business.MasterLogViewSettings, Vanrise.Common.Business","Items":[{"PermissionName":"VRCommon_System_Log: View","Directive":"vr-log-entry-search","Title":"General"},{"PermissionName":"VR_Integration_DataSource: Log","Directive":"vr-integration-log-search","Title":"Data Source"},{"PermissionName":"VR_Integration_DataSource: ImportedBatch","Directive":"vr-integration-importedbatch-search","Title":"Imported Batch"},{"PermissionName":"BusinessProcess_BP_BPInstance_Log: Log","Directive":"bp-instance-log-search","Title":"Business Process"},{"PermissionName":"VRCommon_UserActionAudit: View","Directive":"vr-useractionaudit-search","Title":"User Action Audit"}]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5,null),

('D4385711-5F67-4EC4-BBEC-B5B1BF767188','Account Parts','Account Parts','#/view/Retail_BusinessEntity/Views/AccountPartDefinition/AccountPartDefinitionManagement'	,'A459D3D0-35AE-4B0E-B267-54436FDA729A','Retail_BE/AccountPartDefinition/GetFilteredAccountPartDefinitions',null,null,null,'372ed3cb-4b7b-4464-9abf-59cd7b08bd23',0,5,null),
('296B2785-172C-4332-9846-D32FE3166C62','Service Types','Service Type Management','#/view/Retail_BusinessEntity/Views/ServiceType/ServiceTypeManagement'			,'A459D3D0-35AE-4B0E-B267-54436FDA729A','Retail_BE/ServiceType/GetFilteredServiceTypes',null,null,null,'372ed3cb-4b7b-4464-9abf-59cd7b08bd23',0,3,null),
('9C3BE71A-81D8-4A02-A1F7-FBBE6536BBBB','Account Types','Account Types','#/view/Retail_BusinessEntity/Views/AccountType/AccountTypeManagement'						,'A459D3D0-35AE-4B0E-B267-54436FDA729A','Retail_BE/AccountType/GetFilteredAccountTypes',null,null,null,'372ed3cb-4b7b-4464-9abf-59cd7b08bd23',0,2,null),

--should be remove to common in a later stage
('2CF7E0BE-1396-4305-AA27-11070ACFC18F','Application Visibilities','Application Visibilities','#/view/Common/Views/VRApplicationVisibility/VRApplicationVisibilityManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRApplicationVisibility/GetFilteredVRApplicationVisibilities',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',null,26,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank],[IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank],s.[IsDeleted]);
----------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntityModule]--------------------1701 to 1800--------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('520558FA-CF2F-440B-9B58-09C23B6A2E9B',null,'Billing'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',1,0),
('16419FE1-ED56-49BA-B609-284A5E21FC07',null,'Traffic'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',1,0),
('FC73B0DB-502B-4739-AB3B-AE680F0DAD58',null,'Entities Definition'	,'7913ACD9-38C5-43B3-9612-BEFF66606F22',-1,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])
	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
----------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntity]-------------------5401 to 5700-------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0B08F2E1-6518-46FF-B669-0256CF9BB7D4',null,'Retail_BE_Subscriber','Subscriber'						,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit"]'),
('A2DE8C67-B8AE-4520-BB48-448EE98B7251',null,'Retail_BE_Dealer','Dealer'								,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit"]'),
('0E89CCF7-9240-4D1B-8A01-F91957ECA321',null,'Retail_BE_Operator','Operator'							,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit"]'),

--('4218FCD0-466A-47A9-80DC-3BA20BCF56C4',5405,'Retail_BE_AccountPackage','Account Package'				,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit"]'),
('2045E39B-B9AA-4720-AD1E-7DEF11985335',5406,'Retail_BE_ChargingPolicy','Charging Policy'				,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit"]'),
('BE4E62AA-CCE6-48AD-94ED-87D54A605D35',5403,'Retail_BE_Package','Retail_BE_Package'					,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit","View Assigned Subscribers","Assign Subscribers"]'),

('3ABA6D5B-03AC-4EE5-A3EC-AAC6D9C55621',null,'Retail_BE_Switches','Switches'							,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit"]'),

('1B380EE1-16E1-4A9B-9165-55252E333E9E',5408,'Retail_BE_ServiceType','Service Type'						,'FC73B0DB-502B-4739-AB3B-AE680F0DAD58',null,0,'["View","Edit"]'),
('A8B32E85-079D-4376-BA28-67468B27BDC8',5410,'Retail_BE_AccountPartDefinition','Account Part Definition','FC73B0DB-502B-4739-AB3B-AE680F0DAD58',null,0,'["View","Add","Edit"]'),
('45729623-D9B0-401C-8F23-7656ED03AB61',5407,'Retail_BE_AccountType','Account Type'						,'FC73B0DB-502B-4739-AB3B-AE680F0DAD58',null,0,'["View","Add","Edit"]'),
('5790CCC4-5B3F-4AD3-8EB5-BBC7EE977D25',5411,'Retail_BE_ActionDefinition','Action Definition'			,'FC73B0DB-502B-4739-AB3B-AE680F0DAD58',null,0,'["View","Add","Edit"]'),
('503E011C-3311-43C2-92D7-EA515D790479',5409,'Retail_BE_StatusDefinition','Status Definition'			,'FC73B0DB-502B-4739-AB3B-AE680F0DAD58',null,0,'["View","Add","Edit"]'),

('32E40D20-A8B9-4286-AADE-F4E229D054A9',null,'Retail_BE_CreditClasses','Credit Classes'					,'520558FA-CF2F-440B-9B58-09C23B6A2E9B',null,0,'["View","Add","Edit"]'),
('AB794846-853C-4402-A8E4-6F5C3A75F5F2',3304,'BillingStatistic','Billing Statistic'						,'520558FA-CF2F-440B-9B58-09C23B6A2E9B',1205,0,'["View"]'),
('9AABB14D-B3B2-4CA2-93F9-9FFAE606336B',3310,'BillingCDR','Billing CDR'									,'520558FA-CF2F-440B-9B58-09C23B6A2E9B',1205,0,'["View"]'),

('A611A651-B60B-483D-BC83-1C2B667A120A',3309,'TrafficStatistic','Traffic Statistic'						,'16419FE1-ED56-49BA-B609-284A5E21FC07',1206,0,'["View"]'),
('CF09B199-2DA3-4CA2-B243-E83A1338FB27',3311,'RawCDR','Raw CDR'											,'16419FE1-ED56-49BA-B609-284A5E21FC07',1206,0,'["View"]'),
('583CC2A2-1BBC-44D7-A7AF-923996CD72A9',3312,'FailedCDR','Failed CDR'									,'16419FE1-ED56-49BA-B609-284A5E21FC07',1206,0,'["View"]'),
('73F17C8C-EC3E-46B5-8B53-83CCE74F5C89',3313,'InvalidCDR','Invalid CDR'									,'16419FE1-ED56-49BA-B609-284A5E21FC07',1206,0,'["View"]'),

('0B0572AC-ABFB-4267-8F51-D2480851D14A',5401,'BusinessProcess_BP_Account_Action','Account Action'		,'04493174-83F0-44D6-BBE4-DBEB8B57875A',null,0,'["View", "StartInstance", "ScheduleTask"]'),
('FDB996F9-9157-4778-897E-3F2DED954297',5402,'BusinessProcess_BP_Source_BE_Sync','Source BE Sync'		,'04493174-83F0-44D6-BBE4-DBEB8B57875A',null,0,'["View", "StartInstance", "ScheduleTask"]'),
('08FB93FA-0719-4385-AD9E-0513E3966B26',5701,'BusinessProcess_BP_Account_Balance','Account Balance'		,'692D0589-D764-4DF5-857B-52A98D89FFD6',1801,0,'["View", "StartInstance", "ScheduleTask"]'),

('E4186240-7525-4B39-9B4D-48CCDE5F2590',null,'Retail_Pricing_Rule','Pricing'							,'9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF',null,0,'["View","Add","Edit"]'),
('B5634DD0-11C7-4074-B902-8A7C0B68E5AC',null,'Retail_Identification_Rule','Identification'				,'9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF',null,0,'["View","Add","Edit"]'),

('C0C98E3B-12C8-4805-93D4-F4289C21C6B8',null,'VRCommon_ApplicationVisibility','Application Visibility'	,'7913ACD9-38C5-43B3-9612-BEFF66606F22',null,0,'["View","Add","Edit"]'),
('1F99405A-9FDD-4E73-BDC7-EEE33FD01D7C',null,'Retail_BE_Product','Product'					,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',null,0,'["View","Add","Edit"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
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
('Retail_BE/AccountType/GetFilteredAccountTypes','Retail_BE_AccountType: View'),
('Retail_BE/AccountType/GetAccountType',null),
('Retail_BE/AccountType/GetAccountTypesInfo',null),
('Retail_BE/AccountType/GetAccountTypePartDefinitionExtensionConfigs',null),
('Retail_BE/AccountType/AddAccountType','Retail_BE_AccountType: Add'),
('Retail_BE/AccountType/UpdateAccountType','Retail_BE_AccountType: Edit'),
('Retail_BE/ServiceType/GetFilteredServiceTypes','Retail_BE_ServiceType: View'),
('Retail_BE/ServiceType/GetServiceTypesInfo',null),
('Retail_BE/ServiceType/GetServiceType',null),
('Retail_BE/ServiceType/GetServiceTypeChargingPolicyDefinitionSettings',null),
('Retail_BE/ServiceType/UpdateServiceType','Retail_BE_ServiceType: Edit'),
('Retail_BE/StatusDefinition/GetFilteredStatusDefinitions','Retail_BE_StatusDefinition: View'),
('Retail_BE/StatusDefinition/AddStatusDefinition','Retail_BE_StatusDefinition: Add'),
('Retail_BE/StatusDefinition/UpdateStatusDefinition','Retail_BE_StatusDefinition: Edit'),
('Retail_BE/StatusDefinition/GetStatusDefinition',null),
('Retail_BE/StatusDefinition/GetStatusDefinitionsInfo',null),

('Retail_BE/AccountPartDefinition/GetFilteredAccountPartDefinitions','Retail_BE_AccountPartDefinition: View'),
('Retail_BE/AccountPartDefinition/GetAccountPartDefinition',null),
('Retail_BE/AccountPartDefinition/GetAccountPartDefinitionsInfo',null),
('Retail_BE/AccountPartDefinition/GetAccountPartDefinitionExtensionConfigs',null),
('Retail_BE/AccountPartDefinition/AddAccountPartDefinition','Retail_BE_AccountPartDefinition: Add'),
('Retail_BE/AccountPartDefinition/UpdateAccountPartDefinition','Retail_BE_AccountPartDefinition: Edit'),

('Retail_BE/ActionDefinition/AddActionDefinition','Retail_BE_ActionDefinition: Add'),
('Retail_BE/ActionDefinition/UpdateActionDefinition','Retail_BE_ActionDefinition: Edit'),
('Retail_BE/ActionDefinition/GetFilteredActionDefinitions','Retail_BE_ActionDefinition: View'),
('Retail_BE/ActionDefinition/GetActionDefinition',null),
('Retail_BE/ActionDefinition/GetActionBPDefinitionExtensionConfigs',null),
('Retail_BE/ActionDefinition/GetProvisionerDefinitionExtensionConfigs',null),
('Retail_BE/ActionDefinition/GetActionDefinitionsInfo',null),

('Retail_BE/CreditClass/GetFilteredCreditClasses','Retail_BE_CreditClasses: View'),
('Retail_BE/CreditClass/GetCreditClass',null),
('Retail_BE/CreditClass/AddCreditClass','Retail_BE_CreditClasses: Add'),
('Retail_BE/CreditClass/UpdateCreditClass','Retail_BE_CreditClasses: Edit'),
('Retail_BE/CreditClass/GetCreditClassesInfo',null),

('Retail_BE/Switch/GetFilteredSwitches','Retail_BE_Switches: View'),
('Retail_BE/Switch/GetSwitchSettingsTemplateConfigs',null),
('Retail_BE/Switch/AddSwitch','Retail_BE_Switches: Add'),
('Retail_BE/Switch/UpdateSwitch','Retail_BE_Switches: Edit'),
('Retail_BE/Switch/GetSwitch',null),

('VRCommon/VRApplicationVisibility/GetFilteredVRApplicationVisibilities','VRCommon_ApplicationVisibility: View'),('VRCommon/VRApplicationVisibility/GetVRApplicationVisibilityEditorRuntime',null),('VRCommon/VRApplicationVisibility/AddVRApplicationVisibility','VRCommon_ApplicationVisibility: Add'),('VRCommon/VRApplicationVisibility/UpdateVRApplicationVisibility','VRCommon_ApplicationVisibility: Edit'),('VRCommon/VRApplicationVisibility/GetVRApplicationVisibiltiesInfo',null),('VRCommon/VRApplicationVisibility/GetVRModuleVisibilityExtensionConfigs',null),('VRCommon/VRApplicationVisibility/GetVRApplicationVisibilitiesInfo',null),

('Retail_BE/ProductFamily/GetFilteredProductFamilies',null),('Retail_BE/ProductFamily/GetProductFamilyEditorRuntime',null),('Retail_BE/ProductFamily/AddProductFamily',null),('Retail_BE/ProductFamily/UpdateProductFamily',null),('Retail_BE/ProductFamily/GetProductFamiliesInfo',null)
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

--[genericdata].[DataRecordType]--------------------------------------------------------------------
Delete From [genericdata].[DataRecordType] Where [ID] in ('548613D7-17EE-41FD-A3C3-0D82357EE5E7','A2913B86-CA10-422F-A8BE-30D9CC2AD115','7F3EEA5C-6480-4FDE-AE26-448EA83035DE','4F694A53-D78A-4BAA-8C60-47DC8B01D607','976B6B04-BD15-4200-BB18-4859D8F2F33B','92010E40-1EF1-41C0-9DBF-533883F7D5F9','C4FAD850-CADE-4AA3-88FF-4FC028833904','7FF7337E-FC95-4E8D-BEBD-564BE6DF0395','D221198E-3CC7-4102-BC63-56944DCDAD35','8EE3A64D-6CC7-4C2A-9F41-5AC71E1272ED','4A7F1894-6B33-466B-80D1-74CE46631ABC','C8074CA2-B6F8-4865-B356-74DBAB201220','D0D7E57B-798E-4946-80AF-774BC65923C4','0AB64C3F-A35F-4E8B-9B0F-8CB0440C3554','B7688C91-8B68-4070-8505-9C5DC4B6C656','4ACBF7A3-E000-47F0-AAFC-AD7A9C3EF421','387BCE79-42C8-4EF2-9830-D6E97FA0FCED')


--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldID],[Name],[Title],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D0EE9BF8-385E-48EF-B989-A87666A00072',-2002,'Retail_BE_ChargingPolicy','Charging Policy'									,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-chargingpolicy-selector","ManagerFQTN":"Retail.BusinessEntity.Business.ChargingPolicyManager, Retail.BusinessEntity.Business","IdType":"System.Int32"}'),('1BC07506-D535-4FF8-AC61-C8FDAAF37038',null,'Retail_BE_AccountType','Account Type'											,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-accounttype-selector","GroupSelectorUIControl":"","ManagerFQTN":"Retail.BusinessEntity.Business.AccountTypeManager, Retail.BusinessEntity.Business","IdType":"System.Guid"}'),('C0C76DB1-4876-4E0D-9B59-CA89120E6BE9',-2003,'Retail_BE_Package','Package'													,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-package-selector","ManagerFQTN":"Retail.BusinessEntity.Business.PackageManager, Retail.BusinessEntity.Business","IdType":"System.Int32"}'),('BFAD446F-7129-45B1-94BF-FEBD290F394D',-2004,'Retail_BE_ServiceType','Service Type'										,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-servicetype-selector","ManagerFQTN":"Retail.BusinessEntity.Business.ServiceTypeManager, Retail.BusinessEntity.Business","IdType":"System.Guid"}'),('41767702-B520-4811-96BE-103F96B81177',null,'Retail_BusinessEntity_Product','Product'										,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"retail-be-product-selector","GroupSelectorUIControl":"","ManagerFQTN":"Retail.BusinessEntity.Business.ProductManager,Retail.BusinessEntity.Business","IdType":"System.Int32"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[Title],[Settings]))merge	[genericdata].[BusinessEntityDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]when not matched by target then	insert([ID],[OldID],[Name],[Title],[Settings])	values(s.[ID],s.[OldID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
END



--[Retail_BE].[AccountPartDefinition]---------------------------------------------------------------
Delete From [Retail_BE].[AccountPartDefinition] Where [ID] in ('A4198D5C-B09F-455B-AAC6-0BF9C2572527','A153EA40-E2B8-4D50-A569-B117F64BB2EC')
BEGIN
set nocount on;
;with cte_data([ID],[OldID],[Title],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('05FECF19-6413-402F-BD65-64B0EEF1FB52',22,'Residential Profile','Residential Profile','{"$type":"Retail.BusinessEntity.Entities.AccountPartDefinition, Retail.BusinessEntity.Entities","AccountPartDefinitionId":"05fecf19-6413-402f-bd65-64b0eef1fb52","Name":"Residential Profile","Title":"Residential Profile","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartResidentialProfileDefinition, Retail.BusinessEntity.MainExtensions","ConfigId":"747d0c68-a508-4aa3-8d02-0d3cdfe72149"}}'),
('82228BE2-E633-4EF8-B383-9894F28C8CB0',14,'Financial','Financial','{"$type":"Retail.BusinessEntity.Entities.AccountPartDefinition, Retail.BusinessEntity.Entities","AccountPartDefinitionId":"82228be2-e633-4ef8-b383-9894f28c8cb0","Name":"Financial","Title":"Financial","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancialDefinition, Retail.BusinessEntity.MainExtensions","ConfigId":"ba425fa1-13ca-4f44-883a-2a12b7e3f988"}}'),
('A46ABF18-DEEC-4F78-9178-F433790B5AEB',27,'Personal Info','Personal Info','{"$type":"Retail.BusinessEntity.Entities.AccountPartDefinition, Retail.BusinessEntity.Entities","AccountPartDefinitionId":"a46abf18-deec-4f78-9178-f433790b5aeb","Name":"Personal Info","Title":"Personal Info","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartPersonalInfoDefinition, Retail.BusinessEntity.MainExtensions","ConfigId":"3900317c-b982-4d8b-bd0d-01215ac1f3d9"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Title],[Name],[Details]))
merge	[Retail_BE].[AccountPartDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Title] = s.[Title],[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[OldID],[Title],[Name],[Details])
	values(s.[ID],s.[OldID],s.[Title],s.[Name],s.[Details]);
----------------------------------------------------------------------------------------------------
END

--[common].[StyleDefinition]------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('61A682F3-E00C-4B31-B2F5-26DD5F5E4C2F','Red','{"$type":"Vanrise.Entities.StyleDefinitionSettings, Vanrise.Entities","StyleFormatingSettings":{"$type":"Vanrise.Common.MainExtensions.StyleFormatingSettings.CSSClass, Vanrise.Common.MainExtensions","ClassName":"label label-danger","ConfigId":"1dd9cb13-ccbb-47ef-8514-6cca50aef298"}}'),
('FAC30BBC-68B1-4E8E-B5DE-93015285C012','Green','{"$type":"Vanrise.Entities.StyleDefinitionSettings, Vanrise.Entities","StyleFormatingSettings":{"$type":"Vanrise.Common.MainExtensions.StyleFormatingSettings.CSSClass, Vanrise.Common.MainExtensions","ClassName":"label label-success","ConfigId":"1dd9cb13-ccbb-47ef-8514-6cca50aef298"}}'),
('1E644B07-528A-47B5-A40A-A9E8A0FC868A','Blue','{"$type":"Vanrise.Entities.StyleDefinitionSettings, Vanrise.Entities","StyleFormatingSettings":{"$type":"Vanrise.Common.MainExtensions.StyleFormatingSettings.CSSClass, Vanrise.Common.MainExtensions","ClassName":"label label-primary","ConfigId":"1dd9cb13-ccbb-47ef-8514-6cca50aef298"}}'),
('F3FEE864-02EF-4C0B-A68D-D9AEB5BAC07E','gray','{"$type":"Vanrise.Entities.StyleDefinitionSettings, Vanrise.Entities","StyleFormatingSettings":{"$type":"Vanrise.Common.MainExtensions.StyleFormatingSettings.CSSClass, Vanrise.Common.MainExtensions","ClassName":"gray","ConfigId":"1dd9cb13-ccbb-47ef-8514-6cca50aef298"}}'),
('A6F96839-2922-4CEE-B0F6-F026F8BD8C11','Yellow','{"$type":"Vanrise.Entities.StyleDefinitionSettings, Vanrise.Entities","StyleFormatingSettings":{"$type":"Vanrise.Common.MainExtensions.StyleFormatingSettings.CSSClass, Vanrise.Common.MainExtensions","ClassName":"label label-warning","ConfigId":"1dd9cb13-ccbb-47ef-8514-6cca50aef298"}}'),
('58D24325-7136-40AB-8057-FC1B64311C40','white','{"$type":"Vanrise.Entities.StyleDefinitionSettings, Vanrise.Entities","StyleFormatingSettings":{"$type":"Vanrise.Common.MainExtensions.StyleFormatingSettings.CSSClass, Vanrise.Common.MainExtensions","ClassName":"white","ConfigId":"1dd9cb13-ccbb-47ef-8514-6cca50aef298"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[StyleDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
END


--[bp].[BPDefinition]----------------------5001 to 6000---------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[OldID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('049A1012-A30B-457D-96AE-51E1CA388191',5001,'Retail_BE_ActionBPInputArgument_17817576-4de9-4c00-9bef-0505007b4f53','Account Action','Retail.BusinessEntity.MainActionBPs.RegularActionBP,Retail.BusinessEntity.MainActionBPs','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0B0572AC-ABFB-4267-8F51-D2480851D14A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0B0572AC-ABFB-4267-8F51-D2480851D14A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0B0572AC-ABFB-4267-8F51-D2480851D14A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('DD5BEB84-AAA3-4777-AF2F-A20BACBA5C07',5020,'Vanrise.BEBridge.BP.Arguments.SourceBESyncProcessInput','Source BE Sync','Vanrise.BEBridge.BP.BEBridgeProcess,Vanrise.BEBridge.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-bebridge-process-manual","ScheduledExecEditor":"vr-bebridge-process-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FDB996F9-9157-4778-897E-3F2DED954297","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FDB996F9-9157-4778-897E-3F2DED954297","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FDB996F9-9157-4778-897E-3F2DED954297","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[OldID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[OldID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END

Delete from [common].[Setting] where [ID] in ('682c68ef-5687-47f1-957c-0150a6132f7e','547C43CD-58D2-45A3-BF87-F97E93C1DB9A')--old Product Info GUID
--[common].[Setting]---------------------------201 to 300-------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C',108,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Retail","VersionNumber":"version 0.9"}}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[OldID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
END



Delete from [runtime].[SchedulerTaskActionType] where Id='0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9' --Exchange Rate
