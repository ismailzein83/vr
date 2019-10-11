(function (app) {

    'use strict';

    CustomObjectRecordFilterDefinition.$inject = ['UtilsService', 'VRUIUtilsService'];

    function CustomObjectRecordFilterDefinition(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomObjectRecordFilterDefinitionDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/CustomObject/Definition/Templates/RecordFilterDefinitionTemplate.html'
        };

        function CustomObjectRecordFilterDefinitionDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.normalColNum = ctrl.normalColNum; console.log(ctrl.normalColNum)
                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    function loadDataRecordTypeSelector() {
                        var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {

                            var directivePayload;
                            if (settings != undefined) {
                                directivePayload = {
                                    selectedIds: settings.DataRecordTypeID
                                };
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                        return loadDataRecordTypeSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: [loadDataRecordTypeSelector()] });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.CustomObject.RecordFilterCustomObjectTypeSettings, Vanrise.GenericData.MainExtensions",
                        DataRecordTypeID: dataRecordTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataCustomobjectRecordfilterDefinition', CustomObjectRecordFilterDefinition);
})(app);