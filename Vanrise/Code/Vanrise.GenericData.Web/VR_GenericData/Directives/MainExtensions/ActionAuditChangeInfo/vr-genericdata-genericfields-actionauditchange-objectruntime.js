'use strict';
app.directive('vrGenericdataGenericfieldsActionauditchangeObjectruntime', ['UtilsService', 'VRUIUtilsService','VR_GenericData_BusinessEntityDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new genericFieldsActionAuditChangeInfoObjectRuntimeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/ActionAuditChangeInfo/Templates/GenericFieldsActionAuditChangeInfoObjectRuntime.html';
            }
        };

        function genericFieldsActionAuditChangeInfoObjectRuntimeCtor(ctrl, $scope) {

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };

                UtilsService.setContextReadOnly($scope);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var changeInfoDefinition;
                    var historyId;
                    var loggableEntityUniqueName;
                    var promises = [];

                    if (payload != undefined) {
                        loggableEntityUniqueName = payload.loggableEntityUniqueName;
                        changeInfoDefinition = payload.changeInfoDefinition;
                        historyId = payload.historyId;
                       

                        if (changeInfoDefinition.BusinessEntityDefinitionId != undefined) {
                            var promiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(promiseDeferred.promise);

                            getBusinessEntityDefinitionViewEditor().then(function () {
                                if( $scope.scopeModel.viewEditor != undefined)
                                {
                                    var  loadDirectiveDeferred = UtilsService.createPromiseDeferred();

                                    directiveReadyDeferred.promise.then(function () {
                                        var directivePayload = {
                                            businessEntityDefinitionId: changeInfoDefinition.BusinessEntityDefinitionId,
                                            historyId: historyId
                                        };
                                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectiveDeferred);
                                    });

                                    loadDirectiveDeferred.promise.then(function () {
                                        promiseDeferred.resolve();
                                    }).catch(function (error) {
                                        promiseDeferred.reject(error);
                                    });

                                }else
                                {
                                    promiseDeferred.resolve();
                                }

                            }).catch(function (error) {
                                promiseDeferred.reject(error);
                            });

                            function getBusinessEntityDefinitionViewEditor() {
                                return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinitionViewEditor(changeInfoDefinition.BusinessEntityDefinitionId).then(function (response) {
                                    $scope.scopeModel.viewEditor = response;
                                });
                            }
                        }

                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);