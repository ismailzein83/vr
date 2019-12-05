(function (app) {

    'use strict';

    OpenBPTaskGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OpenBPTaskGenericBEDefinitionActionDirective(UtilsService, VRUIUtilsService) {
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
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/OpenBPTaskActionTemplate.html"
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
                    var promises = [];
                    var settings;
                    var taskIdFieldName;

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
                            context.showSecurityGridCallBack(false);

                        settings = payload.settings;
                        if (settings != undefined)
                            taskIdFieldName = settings.TaskIdFieldName;
                    }

                    function loadDataRecordFieldsSelector() {
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var dataRecordTypeFieldsPayload = {
                                dataRecordTypeId: context != undefined ? context.getDataRecordTypeId() : undefined,
                                selectedIds: taskIdFieldName
                            };

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: [loadDataRecordFieldsSelector()] });
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.OpenBPTaskGenericBEAction, Vanrise.BusinessProcess.MainExtensions",
                        TaskIdFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('bpInstanceOpenbptaskAction', OpenBPTaskGenericBEDefinitionActionDirective);

})(app);