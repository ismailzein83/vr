'use strict';

app.directive('vrCommonCommentGenericbeviewRuntime', ['VRCommon_VRCommentAPIService', 'UtilsService', 'VRUIUtilsService',

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
            templateUrl: '/Client/Modules/Common/Directives/VRComment/Templates/CommentGenericBEViewRuntime.html'
        };

        function Comment(ctrl, $scope, attrs) {

            var vrCommentGridAPI;
            var vrCommentGridReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel.onVRCommentReady = function (api) {
                    vrCommentGridAPI = api;
                    vrCommentGridReadyDeferred.resolve();
                };

                defineApi();
            }

            function loadVRComment(commentPayload) {
                var vrCommentLoadDeferred = UtilsService.createPromiseDeferred();
                vrCommentGridReadyDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(vrCommentGridAPI, commentPayload, vrCommentLoadDeferred);
                });
                return vrCommentLoadDeferred.promise;
            }
            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    var commentPayload = {};
                    
                    if (payload != undefined) {
                        var genericBEGridView = payload.genericBEGridView;
                        commentPayload = {
                            objectId: payload.genericBusinessEntityId,
                            definitionId: (genericBEGridView != undefined && genericBEGridView.Settings != undefined) ? genericBEGridView.Settings.CommentBEDefinitionId : undefined
                        };
                    }
                    var rootPromiseNode = {
                        promises: [loadVRComment(commentPayload)]
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        };
    }]);
