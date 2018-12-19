(function (app) {

    'use strict';

    DataRecordTypeCompositeRecordConditionDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DataRecordTypeCompositeRecordConditionDefinitionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordTypeCompositeRecordConditionDefinitionDirective($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/CompositeRecordCondition/Definition/Templates/DataRecordTypeCompositeRecordConditionDefinitionTemplate.html'
        };

        function DataRecordTypeCompositeRecordConditionDefinitionDirective($scope, ctrl) {
            this.initializeController = initializeController;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadDataRecordTypeSelectorPromise = loadDataRecordTypeSelector();
                    promises.push(loadDataRecordTypeSelectorPromise);

                    function loadDataRecordTypeSelector() {
                        var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorReadyDeferred.promise.then(function () {

                            var dataRecordTypeSelectorPayload;
                            if (payload != undefined) {
                                dataRecordTypeSelectorPayload = { selectedIds: payload.DataRecordTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                        return loadDataRecordTypeSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.CompositeRecordCondition.Definition.DataRecordTypeCompositeRecordConditionDefinition, Vanrise.GenericData.MainExtensions",
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordtypeCompositerecordconditiondefinition', DataRecordTypeCompositeRecordConditionDefinitionDirective);
})(app);