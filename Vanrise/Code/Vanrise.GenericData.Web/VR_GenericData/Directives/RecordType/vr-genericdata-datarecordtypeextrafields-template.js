'use strict';

app.directive('vrGenericdataDatarecordtypeextrafieldsTemplate', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new extraFieldCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'extraFieldCtrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_GenericData/Directives/RecordType/Templates/DataRecordTypeExtraFieldsTemplate.html'
    };


    function extraFieldCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        $scope.scopeModel = {};

        var dataRecordTypeExtraFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var dataRecordTypeExtraFieldsSelectorDirectiveApi;

        var directiveReadyPromiseDeferred;
        var directiveApi;

        function initializeController() {

            $scope.scopeModel.onDataRecordTypeExtraFieldsSelectorReady = function (api) {
                dataRecordTypeExtraFieldsSelectorDirectiveApi = api;
                dataRecordTypeExtraFieldsSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveApi = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveApi, undefined, setLoader, directiveReadyPromiseDeferred);
            };
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var loadDataRecordTypeExtraFieldsPromiseDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeExtraFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                    var dataRecordTypeExtraFieldsPayload;
                    if (payload != undefined) {
                        
                        dataRecordTypeExtraFieldsPayload = { selectedIds: payload.ConfigId };
                    }
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeExtraFieldsSelectorDirectiveApi, dataRecordTypeExtraFieldsPayload, loadDataRecordTypeExtraFieldsPromiseDeferred);
                });
                promises.push(loadDataRecordTypeExtraFieldsPromiseDeferred.promise);

                if (payload != undefined) {
                    directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyPromiseDeferred.promise.then(function () {
                        directiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveApi, payload, directiveLoadDeferred);
                    });

                    promises.push(directiveLoadDeferred.promise);
                }

                return UtilsService.waitMultiplePromises(promises);
            };


            api.getData = function () {
                return directiveApi.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };
    }

    return directiveDefinitionObject;
}]);