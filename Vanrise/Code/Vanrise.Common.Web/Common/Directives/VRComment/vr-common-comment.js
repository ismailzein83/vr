'use strict';

app.directive('vrCommonComment', ['VRCommon_VRCommentAPIService', 'UtilsService', 'VRUIUtilsService',

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
            templateUrl: '/Client/Modules/Common/Directives/VRComment/Templates/CommentTemplate.html'
        };

        function Comment(ctrl, $scope, attrs) {
            var gridApi;
            var commentApi;
            var definitionId;
            var objectId ;

            this.initializeController = initializeController;


            function initializeController() {
                
                
                $scope.scopeModel = {};
                $scope.vRComments = [];
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.newComment;


                $scope.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                };

                $scope.scopeModel.onCommentReady = function (api) {
                    commentApi = api;
                };
                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (query) {
                        if (query != undefined) {
                           definitionId = query.definitionId;
                           objectId = query.objectId;
                        }
                        return gridApi.retrieveData(query);
                    };
                    directiveApi.onVRCommentAdded = function (comment) {
                        gridApi.itemAdded(comment);
                    };
                    return directiveApi;
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { 
                    return VRCommon_VRCommentAPIService.GetFilteredVRComments(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                $scope.scopeModel.addcomment = function () {

                    $scope.scopeModel.isLoading = true;
                    var commenttoadd = { DefinitionId: definitionId, ObjectId: objectId, Content: $scope.scopeModel.newComment };

                    return VRCommon_VRCommentAPIService.AddVRComment(commenttoadd).then(function (response) {
                        if (response != null) {
                            $scope.vRComments.unshift(response.InsertedObject);
                        }
                        $scope.scopeModel.newComment = undefined;
                        $scope.scopeModel.isLoading = false;
                        commentApi.focusComponent();
                    });

                };               
            }

           
           
        }

    }]);