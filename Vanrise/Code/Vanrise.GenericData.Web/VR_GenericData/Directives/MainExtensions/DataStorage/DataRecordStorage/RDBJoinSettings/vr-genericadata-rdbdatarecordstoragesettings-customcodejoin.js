//'use strict';

//app.directive('vrGenericadataRdbdatarecordstoragesettingsCustomcodejoin', ['UtilsService', 'VRUIUtilsService',
//    function (UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                ismultipleselection: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new CustomCodeJoinCtol(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBJoinSettings/Templates/CustomCodeJoinTemplate.html"
//        };

//        function CustomCodeJoinCtol(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var context;
//            var joinSettings;

//            var dependentJoinsAPI;
//            var dependentJoinsReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};
//                ctrl.datasource = [];
//                ctrl.selectedvalues = [];

//                $scope.scopeModel.onDependentJoinsSelectorReady = function (api) {
//                    dependentJoinsAPI = api;
//                    dependentJoinsReadyDeferred.resolve();
//                };

//                UtilsService.waitMultiplePromises([dependentJoinsReadyDeferred.promise]).then(function () {
//                    defineAPI();
//                });
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    dependentJoinsAPI.clearDataSource();
//                    var initialPromises = [];
                    
//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            if (payload != undefined) {
//                                context = payload.context;
//                                var joinsList = context.getJoinsList();
//                                joinSettings = payload.joinSettings;
                               
//                                if (joinsList != undefined) {
//                                    ctrl.datasource =  joinsList;
//                                }

//                                if (joinSettings != undefined) {
//                                    $scope.scopeModel.customCode = joinSettings.CustomCode;
//                                    VRUIUtilsService.setSelectedValues(joinSettings.DependentJoins, 'RDBRecordStorageJoinName', attrs, ctrl);
//                                }
//                            }

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.RDBDataStorage.MainExtensions.Joins.RDBDataRecordStorageCustomCodeJoin, Vanrise.GenericData.RDBDataStorage",
//                        CustomCode: $scope.scopeModel.customCode,
//                        DependentJoins: getDependentJoinslist() 
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function getDependentJoinslist() {
//                var length = ctrl.selectedvalues.length;
//                var dependenJoins = [];
//                if (length > 0) {
//                    for (var i = 0; i < length; i++) {
//                        var dependentJoin = ctrl.selectedvalues[i];
//                        dependenJoins.push(dependentJoin.RDBRecordStorageJoinName);
//                    }
//                }
//                return dependenJoins;
//            }
//        }

//        return directiveDefinitionObject;
//    }]);