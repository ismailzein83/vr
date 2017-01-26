(function (app) {

    'use strict';

    VisibilityAccountDefinitionsDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_VisibilityAccountDefinitionService', 'Retail_BE_AccountTypeAPIService'];

    function VisibilityAccountDefinitionsDirective(UtilsService, VRNotificationService, Retail_BE_VisibilityAccountDefinitionService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityAccountDefinitionsCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/Templates/VisibilityAccountDefinitionsTemplate.html'
        };

        function VisibilityAccountDefinitionsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountFields;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.visibilityAccountDefinitions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddVisibilityAccountDefinition = function () {
                    var onVisibilityAccountDefinitionAdded = function (addedVisibilityAccountDefinition) {
                        //extendVisibilityAccountDefinitionObj(addedVisibilityAccountDefinition);
                        $scope.scopeModel.visibilityAccountDefinitions.push({ Entity: addedVisibilityAccountDefinition });
                    };

                    Retail_BE_VisibilityAccountDefinitionService.addVisibilityAccountDefinition(onVisibilityAccountDefinitionAdded);
                };
                $scope.scopeModel.onDeleteVisibilityAccountDefinitions = function (visibilityAccountDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.visibilityAccountDefinitions, visibilityAccountDefinition.Entity.Title, 'Entity.Title');
                            $scope.scopeModel.visibilityAccountDefinitions.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    console.log(payload);

                    var accountDefinitions;

                    if (payload != undefined && payload.vrModuleVisibility != undefined) {
                        accountDefinitions = payload.vrModuleVisibility.AccountDefinitions;
                    }

                    if (accountDefinitions != undefined) {
                        for (var index in accountDefinitions) {
                            if (index != "$type") {
                                var visibilityAccountDefinition = accountDefinitions[index];
                                //extendVRModuleVisibility(visibilityAccountDefinition);
                                $scope.scopeModel.visibilityAccountDefinitions.push({ Entity: visibilityAccountDefinition });
                            }
                        }
                    }
                };

                api.getData = function () {

                    var visibilityAccountDefinitions;
                    if ($scope.scopeModel.visibilityAccountDefinitions.length > 0) {
                        visibilityAccountDefinitions = {};
                        for (var i = 0; i < $scope.scopeModel.visibilityAccountDefinitions.length; i++) {
                            var visibilityAccountDefinition = $scope.scopeModel.visibilityAccountDefinitions[i].Entity;
                            visibilityAccountDefinitions[visibilityAccountDefinition.AccountBEDefinitionId] = visibilityAccountDefinition;
                        }
                    }

                    return {
                        $type: "Retail.BusinessEntity.Entities.VRRetailBEVisibility, Retail.BusinessEntity.Entities",
                        AccountDefinitions: visibilityAccountDefinitions
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editVisibilityAccountDefinitionDefinition
                }];
            }
            function editVisibilityAccountDefinitionDefinition(visibilityAccountDefinition) {
                var onVisibilityAccountDefinitionUpdated = function (updatedVisibilityAccountDefinition) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.visibilityAccountDefinitions, visibilityAccountDefinition.Entity.Title, 'Entity.Title');
                    //extendVisibilityAccountDefinitionObj(updatedVisibilityAccountDefinition);
                    $scope.scopeModel.visibilityAccountDefinitions[index] = { Entity: updatedVisibilityAccountDefinition };
                };

                Retail_BE_VisibilityAccountDefinitionService.editVisibilityAccountDefinition(visibilityAccountDefinition.Entity, onVisibilityAccountDefinitionUpdated);
            }

            //function extendVisibilityAccountDefinitionObj(visibilityAccountDefinition) {
            //    if (accountFields == undefined)
            //        return;

            //    for (var index = 0; index < accountFields.length; index++) {
            //        var currentAccountField = accountFields[index];
            //        if (visibilityAccountDefinition.FieldName == currentAccountField.Name) {
            //            visibilityAccountDefinition.FieldTitle = currentAccountField.Title;
            //            return;
            //        }
            //    }
            //}
        }
    }

    app.directive('retailBeVisibilityAccountdefinitions', VisibilityAccountDefinitionsDirective);

})(app);