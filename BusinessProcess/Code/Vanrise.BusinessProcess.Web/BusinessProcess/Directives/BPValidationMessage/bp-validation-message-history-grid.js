'use strict';

app.directive('businessprocessBpValidationMessageHistory', ['BusinessProcess_BPValidationMessageAPIService', 'VRNotificationService', 'UtilsService', 'BPActionSeverityEnum', 'BusinessProcess_BPValidationMessageService', function (BusinessProcess_BPValidationMessageAPIService, VRNotificationService, UtilsService, BPActionSeverityEnum, BusinessProcess_BPValidationMessageService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var bpValidationMessageGrid = new BPValidationMessageGrid($scope, ctrl, $attrs);
            bpValidationMessageGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/BusinessProcess/Directives/BPValidationMessage/Templates/BPValidationMessageHistoryGridTemplate.html'
    };
    function BPValidationMessageGrid($scope, ctrl) {

        this.initializeController = initializeController;

        var bpInstanceId;
        var hasWarningMessages;
        var requireWarningConfirmation;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();
        var gridFilter = {};

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.bpValidationMessages = [];

            $scope.scopeModel.actionSeverities = UtilsService.getArrayEnum(BPActionSeverityEnum);
            $scope.scopeModel.selectedActionSeverities = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_BPValidationMessageAPIService.GetFilteredBPValidationMessage(dataRetrievalInput)
                    .then(function (response) {
                        if (response != undefined) {
                            if (hasWarningMessages === undefined) {
                                hasWarningMessages = response.HasWarningMessages;
                                if (hasWarningMessages === true && requireWarningConfirmation === true)
                                    $scope.scopeModel.showWarningConfirmationSection = true;
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            $scope.scopeModel.getSeverityColor = function (dataItem, colDef) {
                return BusinessProcess_BPValidationMessageService.getSeverityColor(dataItem.Entity.Severity);
            };

            $scope.scopeModel.search = function () {
                setGridFilter();
                return gridAPI.retrieveData(gridFilter);
            };

            $scope.scopeModel.validateWarningConfirmationValue = function () {
                return ($scope.scopeModel.warningConfirmationValue === true) ? null : 'You must confirm the warnings to continue';
            };

            UtilsService.waitMultiplePromises([gridReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.load = function (query) {
                bpInstanceId = query.BPInstanceID;
                requireWarningConfirmation = query.requireWarningConfirmation;
                setGridFilter();
                return gridAPI.retrieveData(gridFilter);
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        function setGridFilter() {
            gridFilter = {
                ProcessInstanceId: bpInstanceId,
                Severities: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedActionSeverities, 'value')
            };
        }
    }
}]);