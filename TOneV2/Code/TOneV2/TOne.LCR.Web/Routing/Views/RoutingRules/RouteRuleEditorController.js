RouteRuleEditorController.$inject = ['$scope', 'RoutingRulesAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'RoutingRulesTemplatesEnum', 'CodeSetTypeEnum', 'CustomerSetsEnum', 'UtilsService', 'DaysOfWeekEnum'];
function RouteRuleEditorController($scope, RoutingRulesAPIService, VRModalService, VRNotificationService, VRNavigationService, RoutingRulesTemplatesEnum, CodeSetTypeEnum, CustomerSetsEnum, UtilsService, DaysOfWeekEnum) {
    var editMode;
    defineScope();
    $scope.RouteRuleId;
    loadParameters();

    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.ruleId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.ruleId = parameters.ruleId;

        if ($scope.ruleId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.routeRule;
        $scope.subViewCodeSetConnector = {};
        $scope.subViewActionDataConnector = {};
        $scope.subViewCustomerSetConnector = {};
        $scope.subViewTimeSettingConnector = {};

        $scope.BEDDate = '';
        $scope.EEDDate = '';

        $scope.ruleTypes = [];
        $scope.customerSets = [];
        $scope.codeSets = [];

        $scope.selectedRuleType = '';
        $scope.selectedCustomerSet = '';
        $scope.selectedCodeSet = '';

        $scope.saveRouteRule = function () {
            if (editMode) {
                return updateRouteRule();
            }
            else {
                return insertRouteRule();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        loadRuleTypes();
        loadCodeSets();
        loadCustomerSets();
        if (editMode) {
            $scope.isGettingData = true;
            getRouteRule().finally(function () {
                $scope.isGettingData = false;
            })
        }
    }

    function getRouteRule() {
        return RoutingRulesAPIService.getRouteRuleDetails($scope.ruleId)
           .then(function (response) {
               fillScopeFromRouteRuleObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function buildRouteRuleObjFromScope() {
        var settings = $scope.subViewTimeSettingConnector.getData();
        var routeRuleObject = {
            CodeSet: $scope.subViewCodeSetConnector.getData(),
            CarrierAccountSet: $scope.subViewCustomerSetConnector.getData(),
            ActionData: $scope.subViewActionDataConnector.getData(),
            Type: "RouteRule",
            BeginEffectiveDate: settings.BeginEffectiveDate,
            EndEffectiveDate: settings.EndEffectiveDate,
            Reason: $scope.Reason,
            RouteRuleId: ($scope.RouteRuleId != null) ? $scope.RouteRuleId : 0,
            TimeExecutionSetting: settings
        };

        return routeRuleObject;
    }

    function fillScopeFromRouteRuleObj(routeRuleObject) {
        $scope.RouteRuleId = routeRuleObject.RouteRuleId;
        $scope.routeRule = routeRuleObject;

        $scope.selectedRuleType = UtilsService.getItemByVal($scope.ruleTypes, routeRuleObject.ActionData.$type, 'objectType');
        $scope.Reason = routeRuleObject.Reason;
        $scope.selectedCustomerSet = $scope.customerSets[0]; //UtilsService.getItemByVal($scope.customerSets, routeRuleObject.CarrierAccountSet.$type, 'objectType');
        $scope.selectedCodeSet = UtilsService.getItemByVal($scope.codeSets, routeRuleObject.CodeSet.$type, 'objectType');
        $scope.subViewActionDataConnector.data = routeRuleObject.ActionData;
        $scope.BEDDate = routeRuleObject.BeginEffectiveDate;
        $scope.EEDDate = routeRuleObject.EndEffectiveDate;
        $scope.subViewCodeSetConnector.data = routeRuleObject.CodeSet;
        $scope.subViewCustomerSetConnector.data = routeRuleObject.CarrierAccountSet;
        $scope.subViewTimeSettingConnector.setData(routeRuleObject.TimeExecutionSetting);
    }

    function insertRouteRule() {
        $scope.issaving = true;
        var routeRuleObject = buildRouteRuleObjFromScope();
        return RoutingRulesAPIService.InsertRouteRule(routeRuleObject).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("RouteRule", response)) {
                if ($scope.onRouteRuleAdded != undefined)
                    $scope.onRouteRuleAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });

    }

    function updateRouteRule() {
        var routeRuleObject = buildRouteRuleObjFromScope();
        return RoutingRulesAPIService.UpdateRouteRule(routeRuleObject).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("RouteRule", response)) {
                if ($scope.onRouteRuleUpdated != undefined)
                    $scope.onRouteRuleUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }

    function loadRuleTypes() {
        for (var prop in RoutingRulesTemplatesEnum) {
            $scope.ruleTypes.push(RoutingRulesTemplatesEnum[prop]);
        }
    }

    function loadCodeSets() {
        for (var prop in CodeSetTypeEnum) {
            $scope.codeSets.push(CodeSetTypeEnum[prop]);
        }
    }

    function loadCustomerSets() {
        for (var prop in CustomerSetsEnum) {
            $scope.customerSets.push(CustomerSetsEnum[prop]);
        }
    }

}
appControllers.controller('RoutingRules_RouteRuleEditorController', RouteRuleEditorController);