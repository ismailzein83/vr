//(function (app) {

//    'use strict';

//    DataRecordStorageRDBJoins.$inject = ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordStorageService'];

//    function DataRecordStorageRDBJoins(VRNotificationService, VRUIUtilsService, UtilsService, VR_GenericData_DataRecordStorageService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var dataRecordStorageRDBJoins = new DataRecordStorageRDBJoinsController($scope, ctrl, $attrs);
//                dataRecordStorageRDBJoins.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/DataRecordStorageRDBJoinsTemplate.html'
//        };

//        function DataRecordStorageRDBJoinsController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var mainDataRecordTypeId;

//            var joinGridAPI;
//            var joinGripReadyDeferred = UtilsService.createPromiseDeferred();
           
//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.joins = [];

//                $scope.scopeModel.onJoinGridReady = function (api) {
//                    joinGridAPI = api;
//                    joinGripReadyDeferred.resolve();
//                    defineAPI();
//                };

//                $scope.scopeModel.addJoin = function () {
//                    var onJoinAdded = function (addedJoinObj) {
//                        $scope.scopeModel.joins.push(addedJoinObj);
//                        console.log($scope.scopeModel.joins);
//                    };
//                    var context = buildContext();
//                    VR_GenericData_DataRecordStorageService.addRDBJoinDataRecordStorage(context, onJoinAdded);
//                };

//                $scope.scopeModel.removeJoinColumn = function (dataItem) {
//                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.joins, dataItem.RDBRecordStorageJoinName, 'RDBRecordStorageJoinName');
//                    if (index > -1) {
//                        $scope.scopeModel.joins.splice(index, 1);
//                    }
//                };

//                defineMenuActions();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    $scope.isLoading = true;
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        var joinsList = payload.joinsList;
//                        mainDataRecordTypeId = payload.dataRecordTypeId;

//                        if (joinsList != undefined) {
//                            for (var i = 0; i < joinsList.length; i++) {
//                                var join = joinsList[i];
//                                $scope.scopeModel.joins.push(join);
//                            }
//                        }
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
//                        $scope.isLoading = false;
//                    });
//                };

//                api.getData = function () {
//                    return $scope.scopeModel.joins;
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function defineMenuActions() {
//                $scope.scopeModel.menuActions = [{
//                    name: 'Edit',
//                    clicked: editRDBJoinDataRecordStorage,
//                }];
//            }

//            function editRDBJoinDataRecordStorage(joinEntity) {
//                var context = buildContext();

//                var onJoinUpdated = function (updatedJoinObj) {
//                    console.log(updatedJoinObj);
//                };

//                VR_GenericData_DataRecordStorageService.editRDBJoinDataRecordStorage(joinEntity, context, onJoinUpdated);
//            }

//            function buildContext() {
//                var context = {
//                    getJoinsList: function () {
//                        return $scope.scopeModel.joins;
//                    },
//                    getMainDataRecordTypeId: function () {
//                        return mainDataRecordTypeId;
//                    }
//                };
//                return context;
//            }
//        }
//    }

//    app.directive('vrGenericdataDatarecordstorageRdbJoins', DataRecordStorageRDBJoins);

//})(app);