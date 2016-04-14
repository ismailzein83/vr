app.service('VRCommon_MasterLogService',function () {
        var tabsDefinitions = [];
        return ({            
            getTabsDefinition: getTabsDefinition,
            addTabDefinition: addTabDefinition

        });
        function addTabDefinition(tab) {
            tabsDefinitions.push(tab);
        }

        function getTabsDefinition() {
            return tabsDefinitions;
        }

});
