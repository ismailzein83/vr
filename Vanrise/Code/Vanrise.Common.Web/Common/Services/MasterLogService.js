app.service('VRCommon_MasterLogService', ['UtilsService', function (UtilsService) {
        var tabsDefinitions = [];
        var tabsPromises = [];
        return ({            
            getTabsDefinition: getTabsDefinition,
            addTabDefinition: addTabDefinition,
            addTabPromise: addTabPromise

        });
        function addTabPromise(promise) {
            tabsPromises.push(promise);
        }
        function addTabDefinition(tab) {
            tabsDefinitions.push(tab);
        }

        function getTabsDefinition() {
            return UtilsService.waitMultiplePromises(tabsPromises).then(function () {
                return tabsDefinitions;
            })
        }

}]);
