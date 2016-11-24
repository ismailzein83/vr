(function (appControllers) {

    'use strict';

    CodeGroupService.$inject = ['VRCommon_CountryService', 'VRModalService'];

    function CodeGroupService(VRCommon_CountryService, VRModalService) {
        return ({
            addCodeGroup: addCodeGroup,
            editCodeGroup: editCodeGroup,
            registerDrillDownToCountry: registerDrillDownToCountry,
            uploadCodeGroup: uploadCodeGroup
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

            VRModalService.showModal('/Client/Modules/VR_NumberingPlan/Views/CodeGroup/CodeGroupEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_NumberingPlan/Views/CodeGroup/CodeGroupEditor.html', parameters, settings);
        }

        function registerDrillDownToCountry() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Code Groups";
            drillDownDefinition.directive = "vr-np-codegroup-grid";
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

        function uploadCodeGroup(onCodeGroupUploaded) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodeGroupUploaded = onCodeGroupUploaded;
            };

            VRModalService.showModal('/Client/Modules/VR_NumberingPlan/Views/CodeGroup/CodeGroupUploadEditor.html', null, settings);
        }
    }

    appControllers.service('Vr_NP_CodeGroupService', CodeGroupService);

})(appControllers);
