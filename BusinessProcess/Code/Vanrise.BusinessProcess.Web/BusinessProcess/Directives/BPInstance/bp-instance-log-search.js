"use strict";

app.directive("bpInstanceLogSearch", ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPInstanceService', 'VRValidationService', 'VRNotificationService', 'VRDateTimeService', 'UISettingsService',
function (UtilsService, VRUIUtilsService, BusinessProcess_BPInstanceService, VRValidationService, VRNotificationService, VRDateTimeService, UISettingsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new LogSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/BPInstanceSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {


        var gridAPI;
        var filter = {};


        var bpDefinitionDirectiveApi;
        var bpDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var bpInstanceStatusDirectiveApi;
        var bpInstanceStatusReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();


        this.initializeController = initializeController;
        function initializeController() {

            defineScope();

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());
            function getDirectiveAPI() {

                var directiveAPI = {};
                directiveAPI.load = function () {
                    return load();
                };
                return directiveAPI;
            }

        }

        function defineScope() {
            $scope.showGrid = false;
            $scope.onBPDefinitionDirectiveReady = function (api) {
                bpDefinitionDirectiveApi = api;
                bpDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.onBPInstanceStatusDirectiveReady = function (api) {
                bpInstanceStatusDirectiveApi = api;
                bpInstanceStatusReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.searchClicked = function () {
                $scope.showGrid = true;
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
            var fromDate = VRDateTimeService.getNowDateTime();
            fromDate.setHours(0, 0, 0, 0);
            $scope.fromDate = fromDate;

            $scope.top = 100;
            $scope.maxNumberOfRecords = UISettingsService.getMaxSearchRecordCount();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };
            $scope.checkMaxNumberResords = function () {
                if ($scope.top <= $scope.maxNumberOfRecords || $scope.maxNumberOfRecords == undefined) {
                    return null;
                }
                else {
                    return "Max top value can be entered is: " + $scope.maxNumberOfRecords;
                }
            };
        }

        function load() {
            return loadAllControls();
        }

        function getFilterObject() {
            filter = {
                DefinitionsId: bpDefinitionDirectiveApi.getSelectedIds(),
                InstanceStatus: bpInstanceStatusDirectiveApi.getSelectedIds(),
                DateFrom: $scope.fromDate,
                DateTo: $scope.toDate,
                Top: $scope.top
            };
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadBPDefinitions, loadBPInstanceStatus])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = 'Business Process';
        }

        function loadBPDefinitions() {
            var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
            bpDefinitionReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, undefined, loadBPDefinitionsPromiseDeferred);
            });

            return loadBPDefinitionsPromiseDeferred.promise;
        }

        function loadBPInstanceStatus() {
            var loadBPInstanceStatusPromiseDeferred = UtilsService.createPromiseDeferred();
            bpInstanceStatusReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(bpInstanceStatusDirectiveApi, undefined, loadBPInstanceStatusPromiseDeferred);
            });

            return loadBPInstanceStatusPromiseDeferred.promise;
        }


    }

    return directiveDefinitionObject;

}]);
