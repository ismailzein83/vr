'use strict';

restrict: 'E',
app.directive('mediationOutputhandlerOthermediationEditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var editor = new OtherEditor($scope, ctrl, $attrs);
            editor.initializeController();
        },
        controllerAs: 'otherCtrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Mediation_Generic/Directives/MainExtensions/OutPutHandler/Templates/OutPutHandlerOtherMediationEditorTemplate.html'
    };

    function OtherEditor($scope, ctrl, $attrs) {
     
        var mediationSelectorAPI;
        var mediationSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        $scope.scopeModel.onMediationDefinitionSelectorReady = function (api) {
            console.log("in");
            mediationSelectorAPI = api;
            mediationSelectorReadyDeferred.resolve();
        };
        this.initializeController = initializeController;
        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var mediationSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                mediationSelectorReadyDeferred.promise.then(function () {
                    var slectorPayload = {
                            selectedIds: payload!=undefined && payload.data!=undefined && payload.data.MediationDefinitionId || undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(mediationSelectorAPI, slectorPayload, mediationSelectorLoadDeferred);
                });

                return mediationSelectorLoadDeferred.promise;
            };

            api.getData = function () {

                return {
                    $type: 'Mediation.Generic.MainExtensions.MediationOutputHandlers.ExecuteOtherMediationOutputHandler, Mediation.Generic.MainExtensions',
                    MediationDefinitionId: mediationSelectorAPI.getSelectedIds()

                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
}]);