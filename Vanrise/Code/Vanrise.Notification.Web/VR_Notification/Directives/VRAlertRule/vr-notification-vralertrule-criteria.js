
'use strict';

app.directive('vrNotificationVralertruleCriteria', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var dataAnalysisItemDefinitionSettings = new DataAnalysisItemDefinitionSettings($scope, ctrl, $attrs);
            dataAnalysisItemDefinitionSettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRule/Templates/VRAlertRuleCriteriaTemplate.html'
    };

    function DataAnalysisItemDefinitionSettings($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                console.log(payload);

                var promises = [];
                var dataAnalysisDefinitionId;
                var criteriaEditor;
                var criteria;


                if (payload != undefined) {
                    dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                    criteriaEditor = payload.criteriaEditor;
                    criteria = payload.criteria;
                }

                if (criteriaEditor != undefined)
                    $scope.scopeModel.directiveEditor = criteriaEditor;


                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                directiveReadyDeferred.promise.then(function () {

                    var directivePayload = {};
                    directivePayload.dataAnalysisDefinitionId = dataAnalysisDefinitionId;
                    if (criteria != undefined)
                        directivePayload.criteria = criteria;

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            };

            api.getData = function () {
                return directiveAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);

