(function (appControllers) {

    'use strict';

    CodeGroupService.$inject = ['VRCommon_CountryService', 'VRModalService', 'VRCommon_ObjectTrackingService', 'WhS_BE_CodeGroupAPIService'];

    function CodeGroupService(VRCommon_CountryService, VRModalService, VRCommon_ObjectTrackingService, WhS_BE_CodeGroupAPIService) {
        var drillDownDefinitions = [];
        return ({
            addCodeGroup: addCodeGroup,
            editCodeGroup: editCodeGroup,
            registerDrillDownToCountry: registerDrillDownToCountry,
            uploadCodeGroup: uploadCodeGroup,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToCodeGroupe: registerObjectTrackingDrillDownToCodeGroupe
        });

        function addCodeGroup(onCodeGroupAdded, countryId) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodeGroupAdded = onCodeGroupAdded;
            };
            var parameters;
            if (countryId != undefined) {
                parameters = {
                    CountryId: countryId
                };
            }

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CodeGroup/CodeGroupEditor.html', parameters, settings);
        }

        function editCodeGroup(codeGroupId, onCodeGroupUpdated, disableCountry) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodeGroupUpdated = onCodeGroupUpdated;
            };
            var parameters = {
                CodeGroupId: codeGroupId,
                disableCountry: disableCountry != undefined
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CodeGroup/CodeGroupEditor.html', parameters, settings);
        }

        function registerDrillDownToCountry() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Code Groups";
            drillDownDefinition.directive = "vr-whs-be-codegroup-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "New Code Group",
                clicked: function (countryItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(countryItem);
                    var query = {
                        CountriesIds: [countryItem.Entity.CountryId]
                    };
                    var onCodeGroupAdded = function (codeGroupObj) {
                        if (countryItem.codeGroupGridAPI != undefined) {
                            countryItem.codeGroupGridAPI.onCodeGroupAdded(codeGroupObj);
                        }
                    };
                    addCodeGroup(onCodeGroupAdded, countryItem.Entity.CountryId);
                },
                haspermission: function () {
                    return WhS_BE_CodeGroupAPIService.HasAddCodeGroupPermission();
                }

            }];

            drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {
                countryItem.codeGroupGridAPI = directiveAPI;
                var query = {
                    CountriesIds: [countryItem.Entity.CountryId],
                };
                return countryItem.codeGroupGridAPI.loadGrid(query);
            };

            VRCommon_CountryService.addDrillDownDefinition(drillDownDefinition);
        }
        function getEntityUniqueName() {
            return "WhS_BusinessEntity_CodeGroup";
        }

        function registerObjectTrackingDrillDownToCodeGroupe() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, CodeGroupItem) {
                CodeGroupItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: CodeGroupItem.Entity.CodeGroupId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return CodeGroupItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
        function uploadCodeGroup(onCodeGroupUploaded) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodeGroupUploaded = onCodeGroupUploaded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CodeGroup/CodeGroupUploadEditor.html', null, settings);
        }
    }

    appControllers.service('WhS_BE_CodeGroupService', CodeGroupService);

})(appControllers);
