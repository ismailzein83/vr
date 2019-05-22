'use strict';

app.directive('vrCommonCommentGenericbeviewDefinition', ['VRCommon_VRCommentAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VRCommon_VRCommentAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var comment = new Comment(ctrl, $scope, $attrs);
                comment.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRComment/Templates/CommentGenericBEViewDefinition.html'
        };

        function Comment(ctrl, $scope, attrs) {
            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel = {};
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                defineApi();
            }
            function defineApi() {
                var api = {};

                api.load = function (payload) {
                   
                    var rootPromiseNode = {
                        promises: [loadBusinessEntityDefinitionSelector()]
                    };
                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                selectedIds: (payload != undefined && payload.parameterEntity != undefined) ? payload.parameterEntity.CommentBEDefinitionId : undefined,
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.CommentBEDefinitionFilter, Vanrise.Common.Business"
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };
                api.getData = function () {

                    return {
                        $type: "Vanrise.Common.MainExtensions.VRCommentGenericBEDefinitionView, Vanrise.Common.MainExtensions",
                        CommentBEDefinitionId: beDefinitionSelectorApi.getSelectedIds()
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }]);