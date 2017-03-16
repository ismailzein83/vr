(function (appControllers) {

    'use stict';

    ServiceTypeService.$inject = ['VRModalService', 'VRNotificationService', 'VRUIUtilsService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericRuleAPIService', 'VRCommon_ObjectTrackingService'];

    function ServiceTypeService(VRModalService, VRNotificationService, VRUIUtilsService, VR_GenericData_GenericRule, VR_GenericData_GenericRuleAPIService, VRCommon_ObjectTrackingService) {

        function editServiceType(serviceTypeId, parentServiceTypeId, onServiceTypeUpdated) {
            var parameters = {
                serviceTypeId: serviceTypeId,
                parentServiceTypeId: parentServiceTypeId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onServiceTypeUpdated = onServiceTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ServiceType/ServiceTypeEditor.html', parameters, settings);
        };

        function editPartType(partEntity, onPartTypeUpdated, context) {
            var parameters = {
                partEntity: partEntity,
                context: context
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPartTypeUpdated = onPartTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ServiceType/ChargingPolicyPartEditor.html', parameters, settings);
        };

        function addPartType(onPartTypeAdded, context) {
            var parameters = {
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPartTypeAdded = onPartTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ServiceType/ChargingPolicyPartEditor.html', parameters, settings);
        };

        function defineServiceTypeRuleTabsAndMenuActions(dataItem, gridAPI, criteriaFieldValues) {
            if (dataItem.RuleDefinitions == null) {
              
                return;
            }

            var drillDownTabs = [];
            var menuActions = [];

            for (var i = 0; i < dataItem.RuleDefinitions.length; i++) {
                var ruleDefinition = dataItem.RuleDefinitions[i];
                addDrillDownTab(ruleDefinition);
                addMenuAction(ruleDefinition, i);
            }
            addDrillDownHistoryTab();
            setDrillDownTabs();
            setMenuActions();
            function addDrillDownHistoryTab()
            {
                var drillDownDefinition = {};

                drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
                drillDownDefinition.directive = "vr-common-objecttracking-grid";


                drillDownDefinition.loadDirective = function (directiveAPI,chargingPolicyItem) {
                   
                    chargingPolicyItem.objectTrackingGridAPI = directiveAPI;
                    var query = {
                        ObjectId: chargingPolicyItem.Entity.ChargingPolicyId,
                        EntityUniqueName: getEntityUniqueName(chargingPolicyItem.AccountBEDefinitionId),

                    };
                    return chargingPolicyItem.objectTrackingGridAPI.load(query);
                };
                drillDownTabs.push(drillDownDefinition);
            }

            function getEntityUniqueName(accountBEDefinitionId) {
                return "Retail_BusinessEntity_ChargingPolicy_" + accountBEDefinitionId;
            }
            function addDrillDownTab(ruleDefinition) {
                var drillDownTab = {};

                drillDownTab.title = ruleDefinition.Title;
                drillDownTab.directive = 'vr-genericdata-genericrule-grid';

                drillDownTab.loadDirective = function (directiveAPI, dataItem) {
                    dataItem['ruleGrid' + ruleDefinition.RuleDefinitionId + 'API'] = directiveAPI;
                    return directiveAPI.loadGrid(buildRuleGridQuery(ruleDefinition));
                };

                function buildRuleGridQuery() {
                    var ruleGridQuery = {};

                    ruleGridQuery.RuleDefinitionId = ruleDefinition.RuleDefinitionId;

                    ruleGridQuery.CriteriaFieldValues = {};
                    for (var key in criteriaFieldValues)
                        ruleGridQuery.CriteriaFieldValues[key] = buildBusinessEntityFieldTypeFilter(criteriaFieldValues[key]);

                    ruleGridQuery.accessibility = buildAccessibilityObj();
                    ruleGridQuery.criteriaFieldsToHide = ['ServiceType', 'ChargingPolicy', 'Package'];

                    function buildBusinessEntityFieldTypeFilter(filterValue) {
                        return {
                            $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.Filters.BusinessEntityFieldTypeFilter,Vanrise.GenericData.MainExtensions',
                            BusinessEntityIds: [filterValue]
                        };
                    }

                    return ruleGridQuery;
                }

                drillDownTabs.push(drillDownTab);
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI, undefined);
                drillDownManager.setDrillDownExtensionObject(dataItem);
            }
           
            function addMenuAction(ruleDefinition, ruleDefinitionIndex)
            {
                var menuAction = {};
                menuAction.name = 'New ' + ruleDefinition.Title;
                menuAction.clicked = function (dataItem) {
                    dataItem.drillDownExtensionObject.drillDownDirectiveTabs[ruleDefinitionIndex].setTabSelected(dataItem);

                    var onGenericRuleAdded = function (addedGenericRule) {
                        dataItem['ruleGrid' + ruleDefinition.RuleDefinitionId + 'API'].onGenericRuleAdded(addedGenericRule);
                    };

                    var preDefinedData = {};
                    preDefinedData.criteriaFieldsValues = {};
                    for (var key in criteriaFieldValues)
                        preDefinedData.criteriaFieldsValues[key] = { Values: [criteriaFieldValues[key]] };

                    var accessibility = buildAccessibilityObj();

                    VR_GenericData_GenericRule.addGenericRule(ruleDefinition.RuleDefinitionId, onGenericRuleAdded, preDefinedData, accessibility);
                };
                menuAction.haspermission = function () {
                    return VR_GenericData_GenericRuleAPIService.DoesUserHaveAddAccess(ruleDefinition.RuleDefinitionId);
                };
                menuActions.push(menuAction);
            }
            function setMenuActions() {
                dataItem.menuActions = [];
                for (var i = 0; i < menuActions.length; i++) {
                    dataItem.menuActions.push(menuActions[i]);
                }
            }

            function buildAccessibilityObj() {
                return {
                    criteriaAccessibility: {
                        'ServiceType': { notAccessible: true },
                        'ChargingPolicy': { notAccessible: true },
                        'Package': { notAccessible: true }
                    },
                    settingNotAccessible: false
                };
            }
        }
      
        return {
            editServiceType: editServiceType,
            addPartType: addPartType,
            editPartType: editPartType,
            defineServiceTypeRuleTabsAndMenuActions: defineServiceTypeRuleTabsAndMenuActions
            
        };
    }

    appControllers.service('Retail_BE_ServiceTypeService', ServiceTypeService);

})(appControllers);