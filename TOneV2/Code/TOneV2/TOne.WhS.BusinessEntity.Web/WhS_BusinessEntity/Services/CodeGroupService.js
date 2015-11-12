
app.service('WhS_BE_CodeGroupService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_CountryService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_CountryService) {
        
        return ({
            addCodeGroup: addCodeGroup,
            editCodeGroup: editCodeGroup,
            registerDrillDownToCountry: registerDrillDownToCountry
        });
        
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
                    }                    
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