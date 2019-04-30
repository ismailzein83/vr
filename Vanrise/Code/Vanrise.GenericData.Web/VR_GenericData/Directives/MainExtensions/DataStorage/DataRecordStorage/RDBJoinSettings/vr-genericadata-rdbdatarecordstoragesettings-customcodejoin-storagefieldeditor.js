//'use strict';

//app.directive('vrGenericadataRdbdatarecordstoragesettingsCustomcodejoinStoragefieldeditor', ['UtilsService', 'VRUIUtilsService',
//    function (UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new CustomCodeJoinStorageFieldEditorCtol(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBJoinSettings/Templates/CustomCodeJoinStorageFieldEditorTemplate.html"
//        };

//        function CustomCodeJoinStorageFieldEditorCtol(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var context;

//            function initializeController() {
//                $scope.scopeModel = {};
               
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        $scope.scopeModel.sourceStorageFieldName = payload.sourceStorageFieldName;
//                        context = payload.context;
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            return {
//                                promises: []
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return $scope.scopeModel.sourceStorageFieldName;
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;
//    }]);