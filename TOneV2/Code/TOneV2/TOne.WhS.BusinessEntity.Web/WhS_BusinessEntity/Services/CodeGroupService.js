
app.service('WhS_BE__CodeGroupService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_CountryService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_CountryService) {

        return ({
            addCodeGroup: addCodeGroup,
            editCodeGroup: editCodeGroup
        });
        
        function addDrillDownToCountry() {
            var drillDownItem = {};
            
            drillDownItem.title = "Code Groups";
            drillDownItem.directive = "vr-whs-be-codegroup-grid";
            drillDownItem.countryMenuActions = [{
                name: "New Code Group",
                clicked: function (countryItem) {
                    if (drillDownItem.setSelected != undefined)
                        drillDownItem.setSelected();
                    var query = {
                        CountriesIds: [countryItem.CountryId]
                    }                    
                    var onCodeGroupAdded = function (codeGroupObj) {
                        if (countryItem.extensionObject.codeGroupGridAPI != undefined) {
                            countryItem.extensionObject.codeGroupGridAPI.onCodeGroupAdded(codeGroupObj);
                        }
                    };
                    addCodeGroup(onCodeGroupAdded, countryItem.CountryId);
                }
            }];
            drillDownItem.onDirectiveReady = function (directiveAPI, countryItem) {
                if (countryItem.extensionObject == undefined)
                    countryItem.extensionObject = {};
                var query = {
                    CountriesIds: [countryItem.CountryId],
                };
                countryItem.extensionObject.codeGroupGridAPI = directiveAPI;
                countryItem.extensionObject.codeGroupGridAPI.loadGrid(query);
                countryItem.extensionObject.onCodeGroupGridReady = undefined;
            };

            VRCommon_CountryService.addDrillDownEntity(drillDownItem);
        }

        addDrillDownToCountry();

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

    }]);
