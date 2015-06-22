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
        $scope.BED = "";
        $scope.EED = "";
        $scope.step = 1;
        $scope.setStep = function (step) {
            $scope.step = step;
        }

        $scope.routeRule;
        $scope.subViewConnector = {};

        //$scope.BEDDate = "";
        //$scope.EEDDate = "";

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
        if (editMode) {
            $scope.isGettingData = true;
            getRouteRule().finally(function () {
                $scope.isGettingData = false;
            })
        }
        loadRuleTypes();
        loadCodeSets();
        loadCustomerSets();
    }

    function getRouteRule() {
        return RoutingRulesAPIService.getRouteRuleDetails($scope.ruleId)
           .then(function (response) {
               fillScopeFromRouteRuleObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }

    function buildRouteRuleObjFromScope() {
        var routeRuleObject = {
            CodeSet: $scope.subViewConnector.getCodeSet(),
            CarrierAccountSet: $scope.subViewConnector.getCarrierAccountSet(),
            ActionData: $scope.subViewConnector.getActionData(),
            Type: "RouteRule",
            BeginEffectiveDate: $scope.BED,
            EndEffectiveDate: $scope.EED,
            Reason: $scope.Reason,
            RouteRuleId: ($scope.RouteRuleId != null) ? $scope.RouteRuleId : 0,
            TimeExecutionSetting: $scope.subViewConnector.getTimeSettings()
        };

        return routeRuleObject;
    }

    function fillScopeFromRouteRuleObj(routeRuleObject) {
        $scope.RouteRuleId = routeRuleObject.RouteRuleId;
        $scope.routeRule = routeRuleObject;
        
        $scope.selectedRuleType = UtilsService.getItemByVal($scope.ruleTypes, routeRuleObject.ActionData.$type, 'objectType');
        $scope.BED = routeRuleObject.TimeExecutionSetting != null && routeRuleObject.TimeExecutionSetting != undefined ? routeRuleObject.TimeExecutionSetting.BeginEffectiveDate : routeRuleObject.BeginEffectiveDate;
        $scope.EED = routeRuleObject.TimeExecutionSetting != null && routeRuleObject.TimeExecutionSetting != undefined ? routeRuleObject.TimeExecutionSetting.EndEffectiveDate : routeRuleObject.EndEffectiveDate;
        $scope.subViewConnector.load();
        $scope.Reason = routeRuleObject.Reason;
        $scope.selectedCustomerSet = $scope.customerSets[0]; //UtilsService.getItemByVal($scope.customerSets, routeRuleObject.CarrierAccountSet.$type, 'objectType');
        $scope.selectedCodeSet = null;
        $scope.selectedCodeSet = UtilsService.getItemByVal($scope.codeSets, routeRuleObject.CodeSet.$type, 'objectType');
        $scope.currentTimeSetting = routeRuleObject.TimeExecutionSetting;

    }

    function insertRouteRule() {
        $scope.issaving = true;
        var routeRuleObject = buildRouteRuleObjFromScope();
        return RoutingRulesAPIService.InsertRouteRule(routeRuleObject)
        .then(function (response) {
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
        RoutingRulesAPIService.UpdateRouteRule(routeRuleObject)
        .then(function (response) {
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