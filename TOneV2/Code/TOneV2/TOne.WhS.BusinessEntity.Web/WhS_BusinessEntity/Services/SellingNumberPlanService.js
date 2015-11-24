
app.service('WhS_BE_SellingNumberPlanService', ['VRModalService', 'UtilsService',
    function (VRModalService, UtilsService) {
        
        var drillDownDefinitions = [];
        return ({
            editSellingNumberPlan: editSellingNumberPlan,
            addSellingNumberPlan: addSellingNumberPlan,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        });

        function editSellingNumberPlan(SellingNumberPlanId, onSellingNumberPlanUpdated) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForUpdateEditor("Selling Number Plan");
                modalScope.onSellingNumberPlanUpdated = onSellingNumberPlanUpdated;
            };
            var parameters = {
                SellingNumberPlanId: SellingNumberPlanId
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanEditor.html', parameters, settings);
        }

        function addSellingNumberPlan(onSellingNumberPlanAdded) {
            var settings = {

            };
            
            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Selling Number Plan");
                modalScope.onSellingNumberPlanAdded = onSellingNumberPlanAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanEditor.html', parameters, settings);
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }]);
