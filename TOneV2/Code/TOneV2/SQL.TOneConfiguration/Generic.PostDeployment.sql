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
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DFA58374-4249-4526-AB1A-7786AAC6B91B','VR_GenericData_GenericBusinessEntity','Business Entity','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html","EnableAdd":false}'),
('A3AD3B1D-B56A-49D0-BC25-0F1EF7DFB03D','VR_GenericData_GenericRule','Generic Rule'				,'VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html","EnableAdd":false}'),
('729BE766-F3D7-4BCC-9678-CCCF57BD4AAD','VR_GenericData_GenericView','Generic Rules View'		,'VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/VR_GenericData/Views/GenericRuleView/GenericRuleViewEditor.html","EnableAdd":true}'),

('6F3FBD7B-275A-4D92-8E06-AD7F7B04C7D6','Generic Business Entity','Generic BE','VR_GenericData_BusinessEntityDefinitionSettingsConfig'					,'{"Editor":"vr-genericdata-genericbusinessentity-editor"}'),

('9CF3C165-1921-4F83-990D-03B82A04AA5A','IfElseStep','IfElseStep','VR_GenericData_DataTransformationStepConfig'											,'{"Editor":"vr-genericdata-datatransformation-ifelsestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ifelsestep-preview"}'),
('1B22813C-F507-4BFB-BEE8-621F9F983ECB','IfStep','IfStep','VR_GenericData_DataTransformationStepConfig'													,'{"Editor":"vr-genericdata-datatransformation-ifstep","StepPreviewUIControl":"vr-genericdata-datatransformation-ifstep-preview"}'),
('9C158FA5-8516-4AF7-AEDD-1BC69D026AFC','AddItemToArrayStep','AddItemToArrayStep','VR_GenericData_DataTransformationStepConfig'							,'{"Editor":"vr-genericdata-datatransformation-additemtoarraystep","StepPreviewUIControl":"vr-genericdata-datatransformation-additemtoarraystep-preview"}'),
('D7CE9698-2721-467E-9820-1ED44B446D0D','ForLoopStep','ForLoopStep','VR_GenericData_DataTransformationStepConfig'										,'{"Editor":"vr-genericdata-datatransformation-forloopstep","StepPreviewUIControl":"vr-genericdata-datatransformation-forloopstep-preview"}'),
('3FC597F0-F5CE-4B49-9AEA-24795C81FE80','TariffRule','TariffRule','VR_GenericData_DataTransformationStepConfig'											,'{"Editor":"vr-genericdata-datatransformation-tariffrulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-tariffrulestep-preview"}'),
('A61D1F7A-A437-4966-B101-5178A0C07880','RateTypeRuleStep','RateTypeRuleStep','VR_GenericData_DataTransformationStepConfig'								,'{"Editor":"vr-genericdata-datatransformation-ratetyperulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ratetyperulestep-preview"}'),
('5D6BBC8B-A602-4A94-AE85-8A602CA26805','ExtraChargeRuleStep','ExtraChargeRuleStep','VR_GenericData_DataTransformationStepConfig'						,'{"Editor":"vr-genericdata-datatransformation-extrachargerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-extrachargerulestep-preview"}'),
('3FB4B968-B4B1-4072-908F-E6F19EB87BE0','RateValueRuleStep','RateValueRuleStep','VR_GenericData_DataTransformationStepConfig'							,'{"Editor":"vr-genericdata-datatransformation-ratevaluerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ratevaluerulestep-preview"}'),
('59B40C67-527F-40F8-BFE7-CE19D2101802','AssignValueruleStep','AssignValueruleStep','VR_GenericData_DataTransformationStepConfig'						,'{"Editor":"vr-genericdata-datatransformation-assignvaluerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-assignvaluerulestep-preview"}'),
('60435713-B35E-4E38-A538-9A479061472E','Execute Transformation','Execute Transformation','VR_GenericData_DataTransformationStepConfig'					,'{"Editor":"vr-genericdata-datatransformation-executetransformationstep","StepPreviewUIControl":"vr-genericdata-datatransformation-executetransformationstep-preview"}'),
('CD71025C-136F-4157-8922-A56294030E46','NormalizationRuleStep','NormalizationRuleStep','VR_GenericData_DataTransformationStepConfig'					,'{"Editor":"vr-genericdata-datatransformation-normalizationrulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-normalizationrulestep-preview"}'),
('F02AC0B5-79A4-4343-8081-85CD3787B88C','InitializeRecordStep','InitializeRecordStep','VR_GenericData_DataTransformationStepConfig'						,'{"Editor":"vr-genericdata-datatransformation-initializerecordstep","StepPreviewUIControl":"vr-genericdata-datatransformation-initializerecordstep-preview"}'),
('00E8E50C-017E-44ED-96A9-6D4291A9C4B6','AssignFieldStep','AssignFieldStep','VR_GenericData_DataTransformationStepConfig'								,'{"Editor":"vr-genericdata-datatransformation-assignfieldstep","StepPreviewUIControl":"vr-genericdata-datatransformation-assignfieldstep-preview"}'),
('FB8AB50E-77EB-40DD-B246-40E3B4E23539','CloneDataRecordStep','CloneDataRecordStep','VR_GenericData_DataTransformationStepConfig'						,'{"Editor":"vr-genericdata-datatransformation-clonedatarecordstep","StepPreviewUIControl":"vr-genericdata-datatransformation-clonedatarecordstep-preview"}'),
('CB929C72-D08A-410A-B9E1-2B5D29D417FA','StopIterationStep','StopIterationStep','VR_GenericData_DataTransformationStepConfig'							,'{"Editor":"vr-genericdata-datatransformation-stopiterationstep","StepPreviewUIControl":"vr-genericdata-datatransformation-stopiterationstep-preview"}'),
('CE33AAB3-2C0A-4F8C-8FBD-FF6B91677C8F','ExecuteExpressionStep','Execute Expression','VR_GenericData_DataTransformationStepConfig'						,'{"Editor":"vr-genericdata-datatransformation-executeexpressionstep","StepPreviewUIControl":"vr-genericdata-datatransformation-executeexpressionstep-preview"}'),
('C01EEEAF-7D51-4FCE-8842-5DB1A8D1B39A','LoadBEByIDStep','LoadBEByIDStep','VR_GenericData_DataTransformationStepConfig'									,'{"Editor":"vr-genericdata-loadbebyidstep","StepPreviewUIControl":"vr-genericdata-loadbebyidstep-preview"}'),
('43932599-404B-4F18-9F5B-963915BF16DC','BELookupRuleStep','BELookupRuleStep','VR_GenericData_DataTransformationStepConfig'								,'{"Editor":"vr-genericdata-belookuprulestep","StepPreviewUIControl":"vr-genericdata-belookuprulestep-preview"}'),
('073E5902-D609-440A-B849-DF0A5E20F8F4','MasterPlanSaleCodeMatchStep','MasterPlanSaleCodeMatchStep','VR_GenericData_DataTransformationStepConfig'		,'{"Editor":"vr-np-datatransformation-masterplansalecodematch","StepPreviewUIControl":"vr-np-datatransformation-masterplansalecodematch-preview"}'),

('C47176FB-32EB-430C-B92D-D34DFADCDDF9','Time Interval','Time Interval','VR_GenericData_SummaryBatchIntervalSettings'											,'{"Editor":"vr-genericdata-summarytransformation-timeinterval-selector"}'),
('2AEEC2DE-EC44-4698-AAEF-8E9DBF669D1E','SQL','SQL','VR_GenericData_DataStoreConfig'																			,'{"Editor":"vr-genericdata-datastoresetting-sql","DataRecordSettingsEditor":"vr-genericdata-datarecordstoragesettings-sql"}'),

('FC76233F-5F8F-4B5E-BF10-1E77EA24FD35','VR_RateValueRule','Rate Value Rule','VR_GenericData_GenericRuleTypeConfig'												,'{"Editor":"vr-genericdata-genericruledefinitionsettings-ratevalue","RuntimeEditor":"vr-rules-pricingrulesettings-ratevalue", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.RateValueRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.RateValueRuleManager, Vanrise.GenericData.Pricing"}'),
('7FD67C90-3FF6-4E5D-9052-BB0A87AB371E','VR_ExtraChargeRule','Extra Charge Rule','VR_GenericData_GenericRuleTypeConfig'											,'{"Editor":"vr-genericdata-genericruledefinitionsettings-extracharge","RuntimeEditor":"vr-rules-pricingrulesettings-extracharge", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.ExtraChargeRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.ExtraChargeRuleManager, Vanrise.GenericData.Pricing"}'),
('AE91755C-F573-4ADD-8DBA-7733193384AF','VR_MappingRule','Mapping Rule','VR_GenericData_GenericRuleTypeConfig'													,'{"Editor":"vr-genericdata-genericruledefinitionsettings-mapping","RuntimeEditor":"vr-genericdata-genericrulesettings-mapping-runtimeeditor","RuleTypeFQTN":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","RuleManagerFQTN":"Vanrise.GenericData.Transformation.MappingRuleManager, Vanrise.GenericData.Transformation","FilterEditor":"vr-genericdata-genericrulesettings-mapping-filtereditor"}'),
('D2BA0E63-A47D-4F41-93C9-2DF105EDC26C','VR_NormalizationRule','Normalization Rule','VR_GenericData_GenericRuleTypeConfig'										,'{"Editor":"vr-genericdata-genericruledefinitionsettings-normalization","RuntimeEditor":"vr-rules-normalizenumbersettings", "RuleTypeFQTN":"Vanrise.GenericData.Normalization.NormalizationRule, Vanrise.GenericData.Normalization", "RuleManagerFQTN":"Vanrise.GenericData.Normalization.NormalizationRuleManager, Vanrise.GenericData.Normalization"}'),
('5969790E-1BD4-45E4-BE39-B8D7FA6A1842','VR_RateTypeRule','Rate Type Rule','VR_GenericData_GenericRuleTypeConfig'												,'{"Editor":"vr-genericdata-genericruledefinitionsettings-ratetype","RuntimeEditor":"vr-rules-pricingrulesettings-ratetype", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.RateTypeRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.RateTypeRuleManager, Vanrise.GenericData.Pricing"}'),
('B2061C48-A2C9-4494-A707-0E84A195B5E5','VR_TariffRule','Tariff Rule','VR_GenericData_GenericRuleTypeConfig'													,'{"Editor":"vr-genericdata-genericruledefinitionsettings-tariff","RuntimeEditor":"vr-rules-pricingrulesettings-tariff", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.TariffRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.TariffRuleManager, Vanrise.GenericData.Pricing"}'),
('EBB2BF06-4FD8-4942-8DF4-8892A22AA6FD','VR_BalanceAlert','Balance Action Rule','VR_GenericData_GenericRuleTypeConfig'											,'{"Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-accountbalance-balancealertrulesettings", "RuleTypeFQTN":"Vanrise.AccountBalance.Entities.BalanceAlertRule, Vanrise.AccountBalance.Entities", "RuleManagerFQTN":" Vanrise.AccountBalance.Business.BalanceAlertRuleManager, Vanrise.AccountBalance.Business"}'),
('14950E0B-749A-44D6-9464-F8B1A41D1EDF','VR_TaxRule','Tax Rule','VR_GenericData_GenericRuleTypeConfig'															,'{"Editor":"vr-genericdata-genericruledefinitionsettings-tax","RuntimeEditor":"vr-rules-pricingrulesettings-tax", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.TaxRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.TaxRuleManager, Vanrise.GenericData.Pricing"}'),

('B8712417-83AB-4D4B-9EE1-109D20CEB909','DateTime','DateTime','VR_GenericData_DataRecordFieldType'																,'{"Editor":"vr-genericdata-fieldtype-datetime","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","RuleFilterEditor":"vr-genericdata-fieldtype-datetime-rulefiltereditor"}'),
('3F29315E-B542-43B8-9618-7DE216CD9653','Text','Text','VR_GenericData_DataRecordFieldType'																		,'{"Editor":"vr-genericdata-fieldtype-text","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-text-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-text-rulefiltereditor"}'),
('75AEF329-27BD-4108-B617-F5CC05FF2AA3','Number','Number','VR_GenericData_DataRecordFieldType'																	,'{"Editor":"vr-genericdata-fieldtype-number","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-number-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-number-rulefiltereditor"}'),
('EABC41A9-E332-4120-AC85-F0B7E53C0D0D','Choices','Choices','VR_GenericData_DataRecordFieldType'																,'{"Editor":"vr-genericdata-fieldtype-choices","RuntimeEditor":"vr-genericdata-fieldtype-choices-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-choices-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-choices-rulefiltereditor"}'),
('A77FAD19-D044-40D8-9D04-6362B79B177B','Boolean','Boolean','VR_GenericData_DataRecordFieldType'																,'{"Editor":"vr-genericdata-fieldtype-boolean","RuleFilterEditor":"vr-genericdata-fieldtype-boolean-rulefiltereditor","RuntimeEditor":"vr-genericdata-fieldtype-boolean-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-boolean-filtereditor"}'),
('2E16C3D4-837B-4433-B80E-7C02F6D71467','Business Entity','Business Entity','VR_GenericData_DataRecordFieldType'												,'{"Editor":"vr-genericdata-fieldtype-businessentity","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-businessentity-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-businessentity-rulefiltereditor"}'),
('82E04A93-5A3F-4C32-80A4-08E1DAFCF305','Array','Array','VR_GenericData_DataRecordFieldType'																	,'{"Editor":"vr-genericdata-fieldtype-array","RuntimeEditor":"vr-genericdata-fieldtype-array-runtimeeditor", "FilterEditor": "vr-genericdata-fieldtype-array-filtereditor"}'),
('414C5C8D-48AD-4343-ABDA-4CD34D570C53','Dictionary','Dictionary','VR_GenericData_DataRecordFieldType'															,'{"Editor":"vr-genericdata-fieldtype-dictionary","RuntimeEditor":"vr-genericdata-fieldtype-dictionary-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-dictionary-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-dictionary-rulefiltereditor"}'),

('BBC57155-0412-4371-83E5-1917A8BEA468','VR_GenericData_VRObjectTypes_DataRecordType','Data Record','VR_Common_ObjectType'													,'{"Editor":"vr-genericdata-datarecordobjecttype", "PropertyEvaluatorExtensionType": "VR_GenericData_DataRecordObjectType_PropertyEvaluator"}'),
('F663BF74-99DB-4746-8CBC-E74198E1786C','VR_GenericData_VRObjectTypes_DataRecordField','Field','VR_GenericData_DataRecordObjectType_PropertyEvaluator'						,'{"Editor":"vr-genericdata-datarecordobjectfield"}'),
('FE0EE225-6893-410F-8095-1834DB99D7B7','VR_GenericData_VRObjectTypes_DataRecordFieldComparison','Field Comparison','VR_GenericData_DataRecordObjectType_PropertyEvaluator'	,'{"Editor":"vr-genericdata-datarecordobjectfieldcomparison"}'),

('AAF0C72D-1C8F-47DA-AC11-DD7819B93351','Execute Highest Level','Execute Highest Level','VR_GenericData_DataRecordAlertRuleConfig'													,'{"Editor":"vr-genericdata-datarecordalertrule-recordfilter-management"}'),
('434F7B1E-8B93-4144-9150-E24BF3EB4EFB','VR_Notification_VRAlertRuleTypeSettings_DataRecordActionRuleTypeSettings','Data Record Alert','VR_Notification_VRAlertRuleTypeSettings'	,'{"Editor":"vr-genericdata-datarecordalertruletype-settings"}'),
('8DC29F02-7197-4C60-8E21-CBDE0C2AE87B','BEDefinitionOverriddenConfiguration','BEDefinition Overridden Configuration','VRCommon_OverriddenConfiguration','{"Editor":"vr-genericdata-overriddenconfiguration-bedefinition"}'),
('B2903DAC-1980-4C21-82FB-8DCC72E5068D','GenericRuleDefinitionOverriddenConfiguration','Generic Rule Definition Overridden Configuration','VRCommon_OverriddenConfiguration','{"Editor":"vr-genericdata-overriddenconfiguration-genericruledefinition"}'),
('B99B2B0A-9A80-49FC-B68F-C946E1628595','VR_GenericData_GenericBEView','Generic BE','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-genericdata-genericbusinessentity-vieweditor"}'),

('3B904E8C-2AC0-43DB-A4EF-425869D40544','DataRecordSendEmail','Data Record Send Email','VR_Notification_VRActionDefinition'												,'{"Editor":"vr-genericdata-datarecord-vractiondefinition-extendedsettings-sendemail"}'),
('E64C51A2-08E0-4B7D-96F0-9FF1848A72FA','VR_GenericData_DataRecordNotificationTypeSettings','Data Record Notification','VR_Notification_VRNotificationTypeSettings'		,'{"Editor" : "vr-genericdata-datarecordnotificationtype-settings"}'),

('458EBD88-4537-4B57-B2AB-14AFAB2EF9B6','VR_Generic_DataRecordFieldFormula_NullToBoolean','Null To Boolean','VR_Generic_DataRecordFieldFormula'					,'{"Editor":"vr-genericdata-datarecordtypefields-formula-nulltoboolean"}'),
('FC4B69F0-D577-4319-8D10-ED8F95E07441','VR_Generic_DataRecordFieldFormula_ParentBusinessEntity','Parent Business Entity','VR_Generic_DataRecordFieldFormula'	,'{"Editor":"vr-genericdata-datarecordtypefields-formula-parentbusinessentity"}'),
('9A9E268C-0DC3-4488-8F11-CBEFF8D70E1D','VR_Generic_DataRecordFieldFormula_DateTimeRound','Date Time Round','VR_Generic_DataRecordFieldFormula'					,'{"Editor":"vr-genericdata-datarecordtypefields-formula-datetimeround"}'),
('5B8471BA-3D9A-412D-A158-27BDEC9C4094','VR_Generic_DataRecordFieldFormula_SingleMathOperation','Single Math Operation','VR_Generic_DataRecordFieldFormula'		,'{"Editor":"vr-genericdata-datarecordtypefields-formula-singlemathoperation"}'),
('93DD84F4-A750-4577-8173-36E7A653B920','VR_Generic_DataRecordFieldFormula_SwitchCase','Switch Case','VR_Generic_DataRecordFieldFormula'						,'{"Editor":"vr-genericdata-datarecordtypefields-formula-switchcase"}'),
('F5B55B95-503D-4B7B-A1BB-7A7BCB52C5DF','VR_Generic_DataRecordFieldFormula_CompareToConstant','Compare To Constant','VR_Generic_DataRecordFieldFormula'			,'{"Editor":"vr-genericdata-datarecordtypefields-formula-comparetoconstant"}'),

('c21fe60a-1ac2-4424-9b27-2a6eb237ab12','Send_Email_After_Save','Send Email','VR_GenericData_GenericBEOnAfterSaveHandlerSettings','{"Editor":"vr-genericdata-genericbe-aftersavehandler-sendemail"}'),																	
('3cc1f256-2775-4a08-adf4-b74df4df3f03','Conditional_After_Save','Conditional After Save','VR_GenericData_GenericBEOnAfterSaveHandlerSettings','{"Editor":"vr-genericdata-genericbe-aftersavehandler-conditional"}'),																		
('293b2fab-6abe-4be7-ad58-7d9fa0ba9524','Edit_Generic','Edit Generic','VR_GenericData_GenericBEActionDefinitionSettings','{"Editor":"vr-genericdata-genericbe-editgenericaction-definition"}'),																		
('6ffb05b7-497b-4303-9cf0-61ca0f965d7d','Time_Filter','Time Filter','VR_GenericData_GenericBEFilterDefinitionSettings','{"Editor":"vr-genericdata-genericbe-filterdefinition-timefilter"}'),		
('fddfa89b-b871-4864-9bf3-590262ec68f2','Static_Filter','Static Filter','VR_GenericData_GenericBEFilterDefinitionSettings','{"Editor":"vr-genericdata-genericbe-filterdefinition-staticfilter"}'),													
('6d005236-ece6-43a1-b8ea-281bc0e7643e','Generic_Filter','Generic Filter','VR_GenericData_GenericBEFilterDefinitionSettings','{"Editor":"vr-genericdata-genericbe-filterdefinition-genericfilter"}'),	
('3c9143c3-0361-4d29-8d94-7c7c87f36b0e','Group_Filter','Group Filter','VR_GenericData_GenericBEFilterDefinitionSettings','{"Editor":"vr-genericdata-genericbe-filterdefinition-filtergroup"}'),
('8082ffc6-6781-4501-aa31-02493d74cb8f','Basic_Advance','Basic Advance','VR_GenericData_GenericBEFilterDefinitionSettings','{"Editor":"vr-genericdata-genericbe-filterdefinition-basicadvanced"}'),

('ad2d93e0-0c06-4ebe-b7a9-bf380c256eee','Tabs_Container','Tabs Container','VR_GenericData_GenericBEEditorDefinitionSettings'	,'{"Editor":"vr-genericdata-tabscontainereditor-definition","ValidContainers":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.ContainerType, Vanrise.GenericData.Entities]], mscorlib","$values":[0,1,3]}}'),	
('5be30b11-8ee3-47eb-8269-41bdafe077e1','Generic_Editor','Generic Editor','VR_GenericData_GenericBEEditorDefinitionSettings'	,'{"Editor":"vr-genericdata-genericeditorsetting-definition","ValidContainers":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.ContainerType, Vanrise.GenericData.Entities]], mscorlib","$values":[0,1,3]}}'),			
('ec8b54d7-28ac-474f-b40a-d7ac02d89630','Static_Editor','Static Editor','VR_GenericData_GenericBEEditorDefinitionSettings'		,'{"Editor":"vr-genericdata-staticeditor-definition"}'),					
('8d9fb17a-9df2-4a59-95b1-2f3402a1ead3','Call_Rest_API','Call Rest API','VR_GenericData_GenericBEEditorDefinitionSettings'		,'{"Editor":"vr-genericdata-callrestapieditorsetting-definition","ValidContainers":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.ContainerType, Vanrise.GenericData.Entities]], mscorlib","$values":[2]}}'),
('f290120f-e657-439f-9897-3d1ab8c6e107','Generic_Fields','Generic Fields','VR_GenericData_GenericBEEditorDefinitionSettings'	,'{"Editor":"vr-genericdata-genericfieldseditorsetting-definition","ValidContainers":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.ContainerType, Vanrise.GenericData.Entities]], mscorlib","$values":[2]}}'),
('747f6659-2541-4008-a9cf-56a604e3f63c','Rows_Container','Rows Container','VR_GenericData_GenericBEEditorDefinitionSettings'	,'{"Editor":"vr-genericdata-rowscontainereditor-definition","ValidContainers":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.ContainerType, Vanrise.GenericData.Entities]], mscorlib","$values":[0,1,3]}}'),

('77f7dcb8-e42f-4ec3-8f46-0d655fd519b0','History_Grid_View','History Grid View','VR_GenericData_GenericBEViewDefinitionSettings','{"Editor":"vr-genericdata-genericbe-historygridview-definition"}'),
('C094566B-B9AE-4D49-9810-A7238503FC1C','Condition_Group','	Condition Group','VR_GenericData_GenericBESaveConditionSettings','{"Editor":"vr-genericdata-genericbe-savecondition-conditiongroup"}'),
('D89CB0E3-BD36-409A-965B-3E6AFBCAD4C9','Filter_Group_Condition','Filter Group Condition','VR_GenericData_GenericBEConditionSettings','{"Editor":"vr-genericdata-genericbe-filtergroupcondition"}'),	
('39D842AB-7AFB-46EC-B43E-E7F19493CB86','Serial_Number','Serial Number','VR_GenericData_GenericBEOnBeforeInsertHandlerSettings','{"Editor":"vr-genericdata-genericbe-beforeinserthandler-serialnumber"}'),
('E1C87C72-9481-4707-A73A-C511E0F35AEA','Generic_Field','Generic Field','VR_GenericData_GenericBESerialNumberPartSettings','{"Editor":"vr-genericdata-genericbe-serialnumber-genericfield"}'),
('277816C4-DE23-48E2-971F-46BD0436C1C6','Date_Time','Date Time','VR_GenericData_GenericBESerialNumberPartSettings','{"Editor":"vr-genericdata-genericbe-serialnumber-datetime"}'),
('16197ED3-F3EF-4C50-9106-61A576789726','Sequence','Sequence','VR_GenericData_GenericBESerialNumberPartSettings','{"Editor":"vr-genericdata-genericbe-serialnumber-sequence"}')	,

('28411d23-ea66-47ac-a323-106be0b9da7e','CustomObject','Custom Object','VR_GenericData_DataRecordFieldType','{"Editor":"vr-genericdata-fieldtype-customobject","RuntimeEditor":"","FilterEditor":"vr-genericdata-fieldtype-customobject-filtereditor","RuleFilterEditor":""}')	,
('a80260fd-4492-45c9-8e60-41d91dcd4e9e','Attachment','Attachment','VR_GenericData_DataRecordFieldType','{"Editor":"vr-genericdata-fieldtype-attachment","RuntimeEditor":"vr-genericdata-fieldtype-attachment-runtimeeditor","FilterEditor":"","RuleFilterEditor":""}')	,
('ebd22f77-6275-4194-8710-7bf3063dcb68','Guid','Guid','VR_GenericData_DataRecordFieldType','{"Editor":"vr-genericdata-fieldtype-guid","RuntimeEditor":"vr-genericdata-fieldtype-guid-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-guid-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-guid-rulefiltereditor"}'),
('aa969bad-225d-4d83-b76c-68bfdbc0f045','DataRecordRuleEvaluatorDefinition','Data Record Rule Evaluator','VR_Common_VRComponentType','{"Editor":"vr-genericdata-datarecordruleevaluatordefinition-settings"}'),
('C9FA9112-6D68-490C-BCBF-BD41A912A865','Business Object','Business Object','VR_GenericData_DataStoreConfig','{"Editor":"vr-genericdata-datastoresetting-businessobject","DataRecordSettingsEditor":"vr-genericdata-datarecordstoragesettings-businessobject"}'),
('E2AE62B2-68D9-46C0-9071-58212741CD23','LimitResult_Filter','Limit Result Filter','VR_GenericData_GenericBEFilterDefinitionSettings','{"Editor":"vr-genericdata-genericbe-filterdefinition-limitresultfilter","RuntimeDirective":"vr-genericdata-genericbe-filterruntime-limitresult"}'),
('99E22964-F94E-4BCD-8383-22A613E5AE7F','LookUp Business Entity','LookUp BE','VR_GenericData_BusinessEntityDefinitionSettingsConfig','{"Editor":"vr-genericdata-lookupbusinessentity-editor"}'),
('442BCFAD-1407-4158-82E7-E1B7A0AB458B','VR_GenericData_LKUPBEExtendedSettings_VRActionAuditLKUP','Action Audit LookUp','VR_GenericData_LKUPBEExtendedSettings','{"Editor":"vr-genericdata-lookupbeextendedsettings-actionaudit"}'),
('919798ED-8B0D-40CD-A011-D90C3B691C88','CDRCorrelationDefinitionSettings','CDR Correlation','VR_Common_VRComponentType','{"Editor":"vr-genericdata-cdrcorrelationdefinition-settings"}'),
('6BC1FB84-F28D-476A-81FA-A11FC4E5CC06','Delete_Generic','Delete Generic','VR_GenericData_GenericBEActionDefinitionSettings','{"Editor":"vr-genericdata-genericbe-deletegenericaction-definition"}'),

('6B1A593A-E5E5-4CA4-834A-9A20A1FD16BA','VR_GenericData_GenericRuleDefinitionCriteria','Rule Tree Criteria','VR_GenericData_CriteriaDefinition','{"GridEditor":"vr-genericdata-genericrule-grid-ruletreecriteria","SearchEditor":"vr-genericdata-genericrulesearch-ruletreecriteria","DefinitionEditor":"vr-genericdata-genericruledefinition-ruletreecriteria"}'),

('25127A16-D2EB-47A1-97C0-FFA45C2C31A0','DataRecordTypeCompositeRecordConditionDefinition','Data Record Type','VR_GenericData_CompositeRecordConditionDefinitionSettingConfig','{"Editor":"vr-genericdata-datarecordtype-compositerecordconditiondefinition"}'),
('F16BBC6F-F471-4601-B5C9-7C95B88B3ECB','RecordFilterCompositeCondition','Record Filter','VR_GenericData_CompositeRecordConditionConfig','{"Editor":"vr-genericdata-recordfiltercompositecondition"}'),
('9ADCEA46-87C2-4747-8B88-20796AA99CA0','CompositeRecordConditionGroup','Condition Group','VR_GenericData_CompositeRecordConditionConfig','{"Editor":"vr-genericdata-compositerecordconditiongroup"}'),
('F8DD5B05-7F69-4F16-BFC6-D65EF9B65BF8','RDB','RDB','VR_GenericData_DataStoreConfig','{"Editor":"vr-genericdata-datastoresetting-rdb","DataRecordSettingsEditor":"vr-genericdata-datarecordstoragesettings-rdb"}'),
('6623B11A-333D-487C-8906-B07F1784F944','RDB Record Storage','RDB Record Storage','VR_Analytic_AnalyticDataProviderTableConfig','{"Editor":"vr-rdb-analytic-dataprovider-table"}'),
('77EEF180-EED0-46A8-BBAB-F19BFDB43F60','GenericBE_Grid_View','GenericBE Grid View','VR_GenericData_GenericBEViewDefinitionSettings','{"Editor":"vr-genericdata-genericbe-genericbegridview-definition"}'),
('34FB0901-6270-4EE2-901E-78F6A2C40E4F','FilterGroupGenericBEGridCondition','Filter Group','VR_GenericData_GenericBEGridCondition','{"Editor":"vr-genericdata-genericbe-gridviewcondition-filtergroup"}'),
('8C0574CD-8862-4F29-828A-CE398635F2F7','Data Record Type List','Data Record Type List','VR_GenericData_DataRecordFieldType','{"Editor":"vr-genericdata-fieldtype-datarecordtypelist","RuntimeEditor":"vr-genericdata-fieldtype-datarecordtypelist-runtimeeditor","FilterEditor":"","RuleFilterEditor":""}'),
('661E02F1-7A44-4D56-A73F-7912EF3017B1','Grid_View','Grid View','VR_GenericData_ListRecordRuntimeViewType','{"Editor":"vr-genericdata-fieldtype-datarecordtypelist-gridview-definition"}'),
('03925E9D-6A0F-4D4F-A4A4-36F5757D71EB','Grid_Editor_View','Grid Editor View','VR_GenericData_ListRecordRuntimeViewType','{"Editor":"vr-genericdata-fieldtype-datarecordtypelist-grideditorview-definition"}'),
('6A8E0D5E-318C-4C6E-A99B-991EAE562B7C','Field_View','Field View','VR_GenericData_ListRecordRuntimeViewType','{"Editor":"vr-genericdata-fieldtype-datarecordtypelist-fieldview-definition"}'),
('F0FCE857-0119-4895-9459-648E14EFA60A','Comment View','Comment','VR_GenericData_GenericBEViewDefinitionSettings','{"Editor":"vr-common-comment-genericbeview-definition"}'),

('043E058A-0E0E-40E2-82AB-FE04C896E615','VR_GenericData_ParentDataRecordType','Parent Data Record Type','VR_GenericData_DataRecordTypeExtraField','{"Editor":"vr-genericdata-datarecordtypeextrafields-parentrecordtype"}'),
('466EECEE-F3AC-4880-8818-8FEDD8D92BA8','VR_GenericData_MultipleParentDataRecordType','Multiple Parent Data Record Type','VR_GenericData_DataRecordTypeExtraField','{"Editor":"vr-genericdata-datarecordtypextrafields-multipleparentrecordtype"}'),
('DEC7A24F-6439-4DEB-A912-1AA3B0290F21','RDBDataRecordStorageExpressionFieldFromJoin','From Join','VR_GenericData_RDBDataRecordStorageExpressionFieldSettingsConfig','{"Editor":"vr-genericdata-rdbrecordstoragesettings-expressionfield-fromjoin"}'),
('FF293CA2-522F-47F1-808A-6178DF488E11','RDBDataRecordStorageCustomCodeJoin','Custom Code','VR_GenericData_RDBDataRecordStorageJoinSettingsConfig','{"Editor":"vr-genericadata-rdbdatarecordstoragesettings-customcodejoin"}'),
('2E000BBE-3842-49D5-BCAE-C9B20D778F18','RDBDataRecordStorageCustomCodeExpressionField','Custom Code','VR_GenericData_RDBDataRecordStorageExpressionFieldSettingsConfig','{"Editor":"vr-genericdata-rdbrecordstoragesettings-expressionfield-customcode"}'),
('E5FA86E6-FA20-4301-9FFE-E20A57BEC459','RDBDataRecordStorageJoinOtherRecordStorage','Other Record Storage','VR_GenericData_RDBDataRecordStorageJoinSettingsConfig','{"Editor":"vr-genericadata-rdbdatarecordstoragesettings-joinotherrecordstorage"}'),
('1D2F3E14-AD64-41C6-AE5F-475D1CB75D2D','RDBDataRecordStorageSettingsFilterGroup','Filter Group','VR_GenericData_RDBDataRecordStorageSettingsFilterConfig','{"Editor":"vr-genericdata-rdbrecordstoragesettings-filter-filtergroup"}'),
('CBAFD46F-676F-4CC1-ADA4-4DF7ED762C95','SendEmail_Generic','Send Email Generic','VR_GenericData_GenericBEActionDefinitionSettings','{"Editor":"vr-genericdata-genericbe-sendemailaction-definition"}'),
('5F841B8D-D330-496A-BC62-0720352B25B7','DataRecordStorageFilterGroupPermanentFilter','Filter Group','VR_GenericData_PermanentFilterSettings','{"Editor": "vr-genericdata-datarecordstorage-permanentfilter-recordfilter" }'),
('177D3C5C-4A3D-4B3E-B63C-60B1021C0C40','IsEmpty_Filter','Empty Filter','VR_GenericData_GenericBEFilterDefinitionSettings','{"Editor":"vr-genericdata-genericbe-filterdefinition-isemptyfilter"}'),
('080E340A-32FC-454A-A681-6FF5281A31E2','Fixed_Values','Fixed Values','VR_GenericData_GenericBEOnBeforeInsertHandlerSettings','{"Editor":"vr-genericdata-genericbe-beforeinserthandler-fixedvalues"}'),
('A4C1333D-886B-4C94-BEFF-E1A32ADD69BE','GenericEditorSubView','Generic Editor Sub View','VR_GenericData_GenericBEViewDefinitionSettings','{"Editor":"vr-genericdata-genericbe-genericeditorsubview-definition"}'),
('8ef5476f-37eb-436f-8558-e3181ab78c82','Download File','Download File','VR_GenericData_GenericBEActionDefinitionSettings','{"Editor":"vr-genericdata-genericbe-downloadfileaction-definition"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\	
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[extensionconfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------
end


--[sec].[Module]---------------------------301 to 400---------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','Generic Data',null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,2,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------3001 to 4000-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1E9577A9-EF49-475B-B32C-585A26063B04','Data Record Types','Data Record Types','#/view/VR_GenericData/Views/GenericDataRecord/DataRecordTypeManagement'																,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataRecordType/GetFilteredDataRecordTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
('A3219A0D-87BA-4CA9-A9A1-BB1FB2BD732F','Generic Rule Definitions','Generic Rule Definitions','#/view/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionManagement'										,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/GenericRuleDefinition/GetFilteredGenericRuleDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3),
('F9137B44-C362-4823-B2A3-6EF9E5C40430','Data Transformation Definitions','Data Transformation Definitions','#/view/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionManagement'			,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataTransformationDefinition/GetFilteredDataTransformationDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',4),
('651DD50E-9CBB-49F5-8CA4-F3FDDBCB9C24','Data Stores','Data Stores','#/view/VR_GenericData/Views/DataStore/DataStoreManagement'																							,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataStore/GetFilteredDataStores',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),
('47318833-AFE8-414F-A61D-F3A81727FD5B','Data Record Storages','Data Record Storages','#/view/VR_GenericData/Views/DataRecordStorage/DataRecordStorageManagement'														,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataRecordStorage/GetFilteredDataRecordStorages',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',6),
('0CDB7EDF-3907-4EEC-B781-CF1E19A44C92','Business Entity Definitions','Business Entity Definitions','#/view/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEDefinitionManagement'						,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/BusinessEntityDefinition/GetFilteredBusinessEntityDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',7),
('CA6F551E-8275-49EF-98EB-11DF3EA6BE98','Summary Transformation Definition','Summary Transformation Definition','#/view/VR_GenericData/Views/SummaryTransformationDefinition/SummaryTransformationDefinitionManagement'	,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/SummaryTransformationDefinition/GetFilteredSummaryTransformationDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',8),
('4564B29F-FCBB-4E49-BB2E-C84076BE8EF1','BE Lookup Rule Definition','BE Lookup Rule Definition','#/view/VR_GenericData/Views/BELookupRuleDefinition/BELookupRuleDefinitionManagement'									,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/BELookupRuleDefinition/GetFilteredBELookupRuleDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',9),
('044F280C-F049-4A4E-A8FC-530AA313FDFF','Data Record Field Choice','Data Record Field Choice','#/view/VR_GenericData/Views/DataRecordFieldChoice/DataRecordFieldChoiceManagement'										,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataRecordFieldChoice/GetFilteredDataRecordFieldChoices',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('340746E4-A063-4E7E-BB0B-E30E0A126E64','Generic LKUP','Generic LKUP','#/view/Common/Views/GenericLKUP/GenericLKUPManagement'																							,'4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VRCommon/GenericLKUP/GetFilteredGenericLKUPItems',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',20)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]----------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_GenericData/BusinessEntityDefinition/GetFilteredBusinessEntityDefinitions','VR_SystemConfiguration: View'),
('VR_GenericData/BusinessEntityDefinition/GetBusinessEntityDefinition',null),
('VR_GenericData/BusinessEntityDefinition/GetBusinessEntityDefinitionsInfo',null),
('VR_GenericData/BusinessEntityDefinition/UpdateBusinessEntityDefinition','VR_SystemConfiguration: Edit'),
('VR_GenericData/BusinessEntityDefinition/AddBusinessEntityDefinition','VR_SystemConfiguration: Add'),
('VR_GenericData/BusinessEntityDefinition/GetGenericBEDefinitionView',null),
('VR_GenericData/DataRecordFieldTypeConfig/GetDataRecordFieldTypes',null),
('VR_GenericData/DataRecordFieldTypeConfig/GetDataRecordFieldTypeConfig',null),
('VR_GenericData/DataRecordStorage/GetFilteredDataRecordStorages','VR_SystemConfiguration: View'),
('VR_GenericData/DataRecordStorage/GetDataRecordsStorageInfo',null),
('VR_GenericData/DataRecordStorage/GetDataRecordStorage','VR_SystemConfiguration: View'),
('VR_GenericData/DataRecordStorage/AddDataRecordStorage','VR_SystemConfiguration: Add'),
('VR_GenericData/DataRecordStorage/UpdateDataRecordStorage','VR_SystemConfiguration: Edit'),
('VR_GenericData/DataRecordType/GetDataRecordType',null),
('VR_GenericData/DataRecordType/GetFilteredDataRecordTypes','VR_SystemConfiguration: View'),
('VR_GenericData/DataRecordType/UpdateDataRecordType','VR_SystemConfiguration: Edit'),
('VR_GenericData/DataRecordType/AddDataRecordType','VR_SystemConfiguration: Add'),
('VR_GenericData/DataRecordType/GetDataRecordFieldTypeTemplates',null),
('VR_GenericData/DataRecordType/GetDataRecordTypeInfo',null),
('VR_GenericData/DataStoreConfig/GetDataStoreConfigs',null),
('VR_GenericData/DataStoreConfig/GetDataStoreConfig',null),
('VR_GenericData/DataStore/GetFilteredDataStores','VR_SystemConfiguration: View'),
('VR_GenericData/DataStore/GetDataStoresInfo',null),
('VR_GenericData/DataStore/GetDataStore','VR_SystemConfiguration: View'),
('VR_GenericData/DataStore/AddDataStore','VR_SystemConfiguration: Add'),
('VR_GenericData/DataStore/UpdateDataStore','VR_SystemConfiguration: Edit'),
('VR_GenericData/DataTransformationDefinition/GetDataTransformationDefinition','VR_SystemConfiguration: View'),
('VR_GenericData/DataTransformationDefinition/GetDataTransformationDefinitionRecords',null),
('VR_GenericData/DataTransformationDefinition/GetFilteredDataTransformationDefinitions','VR_SystemConfiguration: View'),
('VR_GenericData/DataTransformationDefinition/UpdateDataTransformationDefinition','VR_SystemConfiguration: Edit'),
('VR_GenericData/DataTransformationDefinition/AddDataTransformationDefinition','VR_SystemConfiguration: Add'),
('VR_GenericData/DataTransformationDefinition/GetDataTransformationDefinitions',null),
('VR_GenericData/DataTransformationDefinition/TryCompileSteps','VR_SystemConfiguration: Edit'),
('VR_GenericData/DataTransformationDefinition/ExportCompilationResult','VR_SystemConfiguration: Edit'),
('VR_GenericData/DataTransformationStepConfig/GetDataTransformationSteps',null),
('VR_GenericData/ExpressionBuilderConfig/GetExpressionBuilderTemplates',null),
('VR_GenericData/ExtensibleBEItem/GetExtensibleBEItem','VR_SystemConfiguration: View'),
('VR_GenericData/ExtensibleBEItem/UpdateExtensibleBEItem','VR_SystemConfiguration: Edit'),
('VR_GenericData/ExtensibleBEItem/AddExtensibleBEItem','VR_SystemConfiguration: Add'),
('VR_GenericData/ExtensibleBEItem/GetFilteredExtensibleBEItems','VR_SystemConfiguration: View'),
('VR_GenericData/GenericBusinessEntity/GetFilteredGenericBusinessEntities',null),
('VR_GenericData/GenericBusinessEntity/GetGenericBusinessEntity',null),
('VR_GenericData/GenericBusinessEntity/AddGenericBusinessEntity',null),
('VR_GenericData/GenericBusinessEntity/UpdateGenericBusinessEntity',null),
('VR_GenericData/GenericBusinessEntity/GetGenericBusinessEntityInfo',null),
('VR_GenericData/GenericBusinessEntity/GetBusinessEntityTitle',null),
('VR_GenericData/GenericBusinessEntity/DeleteGenericBusinessEntity',null),
('VR_GenericData/GenericRule/GetFilteredGenericRules',null),
('VR_GenericData/GenericRule/GetGenericRule',null),
('VR_GenericData/GenericRule/AddGenericRule',null),
('VR_GenericData/GenericRule/UpdateGenericRule',null),
('VR_GenericData/GenericRuleDefinition/GetFilteredGenericRuleDefinitions','VR_SystemConfiguration: View '),
('VR_GenericData/GenericRuleDefinition/GetGenericRuleDefinition',null),
('VR_GenericData/GenericRuleDefinition/AddGenericRuleDefinition','VR_SystemConfiguration: Add'),
('VR_GenericData/GenericRuleDefinition/UpdateGenericRuleDefinition','VR_SystemConfiguration: Edit'),
('VR_GenericData/GenericRuleDefinition/GetGenericRuleDefinitionsInfo',null),
('VR_GenericData/GenericRuleDefinition/GetGenericRuleDefinitionView',null),
('VR_GenericData/GenericRuleTypeConfig/GetGenericRuleTypes',null),
('VR_GenericData/GenericRuleTypeConfig/GetGenericRuleTypeByName',null),
('VR_GenericData/GenericRuleTypeConfig/GetGenericRuleTypeById',null),
('VR_GenericData/GenericUIRuntime/GetExtensibleBEItemRuntime',null),
('VR_GenericData/GenericUIRuntime/GetGenericManagementRuntime',null),
('VR_GenericData/GenericUIRuntime/GetGenericEditorRuntime',null),
('VR_GenericData/GenericUIRuntime/GetDataRecordTypesInfo',null),
('VR_GenericData/SummaryTransformationDefinition/GetSummaryTransformationDefinition','VR_SystemConfiguration: View'),
('VR_GenericData/SummaryTransformationDefinition/GetFilteredSummaryTransformationDefinitions','VR_SystemConfiguration: View'),
('VR_GenericData/SummaryTransformationDefinition/AddSummaryTransformationDefinition','VR_SystemConfiguration: Add'),
('VR_GenericData/SummaryTransformationDefinition/UpdateSummaryTransformationDefinition','VR_SystemConfiguration: Edit'),
('VR_GenericData/SummaryTransformationDefinition/GetSummaryTransformationDefinitionInfo',null),
('VR_GenericData/SummaryTransformationDefinition/GetSummaryBatchIntervalSourceTemplates',null),

('VR_GenericData/BELookupRuleDefinition/GetFilteredBELookupRuleDefinitions','VR_SystemConfiguration: View'),
('VR_GenericData/BELookupRuleDefinition/GetBELookupRuleDefinitionsInfo','VR_SystemConfiguration: View'),
('VR_GenericData/BELookupRuleDefinition/GetBELookupRuleDefinition','VR_SystemConfiguration: View'),
('VR_GenericData/BELookupRuleDefinition/AddBELookupRuleDefinition','VR_SystemConfiguration: Add'),
('VR_GenericData/BELookupRuleDefinition/UpdateBELookupRuleDefinition','VR_SystemConfiguration: Edit'),

('VR_GenericData/DataRecordFieldChoice/GetFilteredDataRecordFieldChoices','VR_SystemConfiguration: View'),
('VR_GenericData/DataRecordFieldChoice/GetDataRecordFieldChoicesInfo','VR_SystemConfiguration: View'),
('VR_GenericData/DataRecordFieldChoice/GetDataRecordFieldChoice','VR_SystemConfiguration: View'),
('VR_GenericData/DataRecordFieldChoice/AddDataRecordFieldChoice','VR_SystemConfiguration: Add'),
('VR_GenericData/DataRecordFieldChoice/UpdateDataRecordFieldChoice','VR_SystemConfiguration: Edit'),

('VRCommon/GenericLKUP/GetFilteredGenericLKUPItems','VR_SystemConfiguration: View'),
('VRCommon/GenericLKUP/AddGenericLKUPItem','VR_SystemConfiguration: Add'),
('VRCommon/GenericLKUP/UpdateGenericLKUPItem','VR_SystemConfiguration: Edit'),
('VRCommon/GenericLKUP/GetGenericLKUPItem',null),
('VRCommon/GenericLKUP/GetGenericLKUPDefinitionExtendedSetings',null),
('VRCommon/GenericLKUP/GetGenericLKUPItemsInfo',null)
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
--------------------------------------------------------------------------------------------------------------
end



--[genericdata].[ExpressionBuilderConfig]-----------------------------------------------------------
begin
set nocount on;
set identity_insert [genericdata].[ExpressionBuilderConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'RecordValue','{"ExpressionBuilderConfigId":"1","Name":"RecordValue","Title":"Record Value","Editor":"vr-genericdata-expressionbuilder-recordvalue"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[ExpressionBuilderConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [genericdata].[ExpressionBuilderConfig] off;
----------------------------------------------------------------------------------------------------
end

--[queue].[QueueActivatorConfig]----------------1 to 500--------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('f6ced9a6-86c4-4fb9-b706-6e91d8f02908','Store Batch Queue Activator','{ "QueueActivatorConfigId": "f6ced9a6-86c4-4fb9-b706-6e91d8f02908" , "Name": "Store Batch Queue Activator" ,"Title" : "Store Batch Queue Activator"									, "Editor" :"vr-genericdata-queueactivator-storebatch"}'),
('65db9db0-61ca-47da-af9e-7e9e0adb11d9','Transform Batch Queue Activator','{ "QueueActivatorConfigId": "65db9db0-61ca-47da-af9e-7e9e0adb11d9" , "Name": "Transform  Batch Queue Activator" ,"Title" : "Transform  Batch Queue Activator"					, "Editor" :"vr-genericdata-queueactivator-transformbatch"}'),
('3c45f696-1d3d-4ba4-8530-ad24368d55b9','Custom Activator','{ "QueueActivatorConfigId": "3c45f696-1d3d-4ba4-8530-ad24368d55b9" , "Name": "Custom Activator" ,"Title" : "Custom Activator"																	, "Editor" :"vr-queueing-queueactivator-customactivator"}'),
('d5580f72-b018-48ad-a0a8-0f754cc2478a','Update Summary Queue Activator','{ "QueueActivatorConfigId": "d5580f72-b018-48ad-a0a8-0f754cc2478a" , "Name": "Update Summary Queue Activator" ,"Title" : "Update Summary Queue Activator"							, "Editor" :"vr-genericdata-queueactivator-updatesummary"}'),
('bc4a5cee-e7c6-491d-9ced-9ee2bc11113f','Generate Summary Queue Activator','{ "QueueActivatorConfigId": "bc4a5cee-e7c6-491d-9ced-9ee2bc11113f" , "Name": "Generate Summary Queue Activator" ,"Title" : "Generate Summary Queue Activator"					, "Editor" :"vr-genericdata-queueactivator-generatesummary"}'),
('b3c5d97f-1cb8-405b-a2ed-9837f677ddf5','Update Account Balances Queue Activator','{"QueueActivatorConfigId": "b3c5d97f-1cb8-405b-a2ed-9837f677ddf5" ,"Name": "Update Account Balances Queue Activator","Title": "Update Account Balances Queue Activator"  ,"Editor" :"vr-accountbalance-queueactivator-updateaccountbalances"}'),
('3c4f6dfd-95e5-41aa-9e1a-53c011cec3c1','Check Action Rules Queue Activator','{"QueueActivatorConfigId": "3c4f6dfd-95e5-41aa-9e1a-53c011cec3c1" ,"Name": "Check Alert Rules Queue Activator","Title": "Check Action Rules Queue Activator"					,"Editor":"vr-genericdata-queueactivator-datarecordcheckalertrules"}'),
('2f2fa33f-4c73-40dc-92e5-341e17c2183b','Distribute Batch Queue Activator','{"QueueActivatorConfigId": "2f2fa33f-4c73-40dc-92e5-341e17c2183b" ,"Name": "Distribute Batch Queue Activator","Title": "Distribute Batch Queue Activator", "Editor":"vr-genericdata-queueactivator-distributebatch"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[queue].[QueueActivatorConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
----------------------------------------------------------------------------------------------------
end

--[bp].[BPDefinition]-------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('94fbbe2f-ecea-4aea-8de6-2efa7cc962e5','Vanrise.GenericData.Notification.Arguments.DataRecordRuleEvaluatorProcessInput','Data Record Rule Evaluator','Vanrise.GenericData.Notification.BP.DataRecordRuleEvaluatorProcess, Vanrise.GenericData.Notification.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-genericdata-datarecordruleevaluator-manual","ScheduledExecEditor":"vr-genericdata-datarecordruleevaluator-schedualed","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.GenericData.Notification.RecordRuleEvaluatorDefinitionBPExtentedSettings, Vanrise.GenericData.Notification"}}'),
('92082EE6-B150-46DE-937B-1CF56B5BA453','Vanrise.GenericData.BP.Arguments.CDRCorrelationProcessInput','CDR Correlation','Vanrise.GenericData.BP.CDRCorrelationProcess,Vanrise.GenericData.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-genericdata-cdrcorrelation-process","ScheduledExecEditor":"vr-genericdata-cdrcorrelation-task","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.GenericData.MainExtensions.CDRCorrelationBPDefinitionSettings, Vanrise.GenericData.MainExtensions"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}}},"BusinessRuleSetSupported":false}')
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

--[genericdata].[DataStore]-------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('d76e65b5-21be-4c0b-92af-d38feab10c68','BusinessObjects','{"$type":"Vanrise.GenericData.Business.BusinessObjectDataStoreSettings, Vanrise.GenericData.Business","ConfigId":"c9fa9112-6d68-490c-bcbf-bd41a912a865","IsRemoteDataStore":false}'),
('608A5CC4-A933-4BF3-83A7-3797EDD0BB41','Configuration Store','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage","ConfigId":"2aeec2de-ec44-4698-aaef-8e9dbf669d1e","ConnectionStringName":"ConfigurationDBConnString","IsRemoteDataStore":false}'),
('E3D48EB5-BC66-4D21-B7FE-A9AA0E7E85D3','RDB Configuration Store','{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataStoreSettings, Vanrise.GenericData.RDBDataStorage","ConfigId":"f8dd5b05-7f69-4f16-bfc6-d65ef9b65bf8","ConnectionStringName":"ConfigurationDBConnString","ConnectionStringAppSettingName":"ConfigurationConnectionStringKey","IsRemoteDataStore":false}'),
('ff5bf627-0d23-4d5e-9c2c-6cc2a41e4a61','Common Entities Store','{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataStoreSettings, Vanrise.GenericData.RDBDataStorage","ConfigId":"f8dd5b05-7f69-4f16-bfc6-d65ef9b65bf8","ConnectionStringName":"CommonEntitiesConnString","ConnectionStringAppSettingName":"CommonEntitiesConnStringKey","IsRemoteDataStore":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[genericdata].[DataStore] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DF55359B-CDE4-4A8F-866C-DDAEEC613D98','VR_GenericData_GenericRuleDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_GenericRuleDefinition_ViewHistoryItem"}'),
('3D801CAF-5426-44AF-87DA-E2361515210D','VR_GenericData_BusinessEntityDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_BusinessEntityDefinition_ViewHistoryItem"}'),
('AB285647-868E-4686-B5F2-FE8B427C1C6E','VR_GenericData_BELookupRuleDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_BELookupRuleDefinition_ViewHistoryItem"}'),
('02CF382D-C4C2-4102-B815-50D2D0464AE3','VR_GenericData_SummaryTransformationDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_SummaryTransformationDefinition_ViewHistoryItem"}'),
('E4195EAE-03A8-4C12-8290-6D6B139F511B','VR_GenericData_DataRecordFieldChoice','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_DataRecordFieldChoice_ViewHistoryItem"}'),
('691CCE99-44CD-42DC-A1B4-91FF66416933','VR_GenericData_DataTransformationDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_DataTransformationDefinition_ViewHistoryItem"}'),
('BF9C4083-292B-4754-A4FB-09D7B9063F66','VR_GenericData_DataRecordStorage','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_DataRecordStorage_ViewHistoryItem"}'),
('B12050CB-2A8C-4653-8B78-36A33ED4F712','VR_GenericData_DataStore','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_DataStore_ViewHistoryItem"}'),
('25A80EC2-C744-4447-9741-BFC94CE3DCC8','VR_GenericData_DataRecordType','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_GenericData_DataRecordType_ViewHistoryItem"}'),

('04F699F9-41DB-4CBD-A983-5318AE0A28F2','VR_Common_GenericLKUPItem','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_GenericLKUPItem_ViewHistoryItem"}')
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

---update QueueInstance with new structure for QueueActivatorConfig 2017-09-20 to be removed after updating related project
CREATE TABLE #QueueActivatorConfig_temp(
	[ID] [int] NULL,
	[NewID] [uniqueidentifier] NULL,
	[Name] [varchar](255) NULL)

insert into #QueueActivatorConfig_temp
Values	(1,	'F6CED9A6-86C4-4FB9-B706-6E91D8F02908',	'Store Batch Queue Activator')
		, (2,	'65DB9DB0-61CA-47DA-AF9E-7E9E0ADB11D9',	'Transform Batch Queue Activator')
		, (3,	'3C45F696-1D3D-4BA4-8530-AD24368D55B9',	'Custom Activator')
		, (4,	'D5580F72-B018-48AD-A0A8-0F754CC2478A',	'Update Summary Queue Activator')
		, (5,	'BC4A5CEE-E7C6-491D-9CED-9EE2BC11113F',	'Generate Summary Queue Activator')
		, (6,	'B3C5D97F-1CB8-405B-A2ED-9837F677DDF5',	'Update Account Balances Queue Activator')
		,(7,	'38ac5083-f3f4-48b5-a89b-528dd155a254','Update WhS Balances Queue Activator')
		, (8,	'3C4F6DFD-95E5-41AA-9E1A-53C011CEC3C1',	'Check Action Rules Queue Activator')
		, (9,	'2F2FA33F-4C73-40DC-92E5-341E17C2183B',	'Distribute Batch Queue Activator')
		,(10,	'1cc49e27-3e66-4160-b55f-beab092a1be1','Evaluate Deal Queue Activator')
		,(501,	'6309e517-3006-47bd-8eb4-8741feac673b','Mediation Staging Records Queue Activator')

       UPDATE  q
       SET    Settings = 
			REPLACE(	q.Settings,
						SUBSTRING ( q.Settings ,CHARINDEX('"ConfigId":',q.Settings) , len(q.Settings)-1-CHARINDEX('"ConfigId":',q.Settings)),
						(select '"ConfigId":"'+cast([NewID] as varchar(100))+'"' from #QueueActivatorConfig_temp where ID=SUBSTRING ( q.Settings ,CHARINDEX('"ConfigId":',q.Settings)+11 , len(q.Settings)-1-CHARINDEX('"ConfigId":',q.Settings)-11))
						)
from	[queue].QueueInstance q
where	ISNUMERIC(SUBSTRING ( q.Settings ,CHARINDEX('"ConfigId":',q.Settings)+11 , 1))=1

drop table #QueueActivatorConfig_temp


--[genericdata].[DataRecordType]-------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[DevProjectID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings],[CreatedTime],[LastModifiedTime])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('3E50B634-706A-4245-B274-30CFE03F30F6',null,'BaseLKUPGuidId',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ID","Title":"ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldGuidType, Vanrise.GenericData.MainExtensions","ConfigId":"ebd22f77-6275-4194-8710-7bf3063dcb68","RuntimeEditor":"vr-genericdata-fieldtype-guid-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-guid-viewereditor","IsNullable":false,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Name","Title":"Name","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false}]}','{"$type":"Vanrise.GenericData.MainExtensions.ParentDataRecordTypeExtraFields, Vanrise.GenericData.MainExtensions","ConfigId":"043e058a-0e0e-40e2-82ab-fe04c896e615","DataRecordTypeId":"1a069c4a-c56c-4d2c-8c21-ff2436db7a83"}','{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities"}','2019-05-17 15:27:05.230','2019-05-17 15:27:05.230'),('157157B3-4AA0-4F96-9217-D4614ABA9BBD',null,'BaseLKUPIntId',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ID","Title":"ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Name","Title":"Name","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false}]}','{"$type":"Vanrise.GenericData.MainExtensions.ParentDataRecordTypeExtraFields, Vanrise.GenericData.MainExtensions","ConfigId":"043e058a-0e0e-40e2-82ab-fe04c896e615","DataRecordTypeId":"1a069c4a-c56c-4d2c-8c21-ff2436db7a83"}','{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities"}','2019-05-17 15:27:47.803','2019-05-17 15:27:47.803'),('1A069C4A-C56C-4D2C-8C21-FF2436DB7A83',null,'BaseBE',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CreatedTime","Title":"Created Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DataType":0,"IsNullable":false,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CreatedBy","Title":"Created By","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"217a8f71-1dd6-4613-8ae2-540a510f5ff5","DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastModifiedTime","Title":"Last Modified Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DataType":0,"IsNullable":false,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastModifiedBy","Title":"Last Modified By","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"217a8f71-1dd6-4613-8ae2-540a510f5ff5","DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false}]}',null,'{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities"}','2019-05-15 14:48:50.010','2019-05-15 14:48:50.010')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[DevProjectID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings],[CreatedTime],[LastModifiedTime]))merge	[genericdata].[DataRecordType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[DevProjectID] = s.[DevProjectID],[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings],[CreatedTime] = s.[CreatedTime],[LastModifiedTime] = s.[LastModifiedTime]when not matched by target then	insert([ID],[DevProjectID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings],[CreatedTime],[LastModifiedTime])	values(s.[ID],s.[DevProjectID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings],s.[CreatedTime],s.[LastModifiedTime]);