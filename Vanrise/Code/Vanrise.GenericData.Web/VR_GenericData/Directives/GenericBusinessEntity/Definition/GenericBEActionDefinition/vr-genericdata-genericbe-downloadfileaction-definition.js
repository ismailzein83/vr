(function (app) {

    'use strict';

    DownloadFileGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DownloadFileGenericBEDefinitionActionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DownloadFileGenericBEDefinitionActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/DownloadFileGenericBEDefinitionActionTemplate.html'
        };

        function DownloadFileGenericBEDefinitionActionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();

                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var context = payload.context;
                    if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
                        context.showSecurityGridCallBack(false);
                    var promises = [];
                    promises.push(loadDataRecordFieldsSelector());

                    $scope.scopeModel.openNewWindow = payload != undefined && payload.settings != undefined ? payload.settings.OpenNewWindow : undefined;
                    function loadDataRecordFieldsSelector() {
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var dataRecordTypeFieldsPayload = {
                                dataRecordTypeId: context.getDataRecordTypeId(),
                            };
                            if (payload.settings != undefined && payload.settings.FileIdFieldName != undefined) {
                                dataRecordTypeFieldsPayload.selectedIds = payload.settings.FileIdFieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEActions.DownloadFileGenericBEAction, Vanrise.GenericData.MainExtensions",
                        FileIdFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                        OpenNewWindow: $scope.scopeModel.openNewWindow
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeDownloadfileactionDefinition', DownloadFileGenericBEDefinitionActionDirective);

})(app);