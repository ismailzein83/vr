(function (app) {

    'use strict';

    GenericBESubviewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService'];

    function GenericBESubviewDirective(UtilsService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBESubviewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEViewDefinition/Templates/GenericBESubviewTemplate.html'
        };

        function GenericBESubviewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var businessEntityDefinitionId;
            var genericBEGridView;
            var fieldValues;
            var genericBusinessEntityId;
            var gridAPI;
            var isObjectValue = false;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };


            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    $scope.scopeModel.isGridLoading = true;
                    if (payload != undefined) {
                        genericBEGridView = payload.genericBEGridView;
                        fieldValues = payload.parentBEEntity != undefined ? payload.parentBEEntity.FieldValues : undefined;

                        genericBusinessEntityId = payload.genericBusinessEntityId;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;

                        if (genericBEGridView != undefined && genericBEGridView.Settings != undefined) {
                            payload = {};

                            if (!canMap()) {
                                promises.push(getGenericBusinessEntity());
                            }
                        }

                    }
                    var rootPromiseNode = {
                        promises: promises,
                        getChildNode: function () {
                            payload = buildPayload();
                            return {
                                promises: [gridAPI.load(payload)]
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });

                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildPayload() {
                return {
                    businessEntityDefinitionId: genericBEGridView.Settings.GenericBEDefinitionId,
                    fieldValues: buildFieldsMappingfields(),
                    filterValues: buildFiltersMappingfields()
                };
            }

            function getGenericBusinessEntity() {
                return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(businessEntityDefinitionId, genericBusinessEntityId).then(function (response) {
                    if (response != undefined) {
                        fieldValues = response.FieldValues;
                        isObjectValue = true;
                    }
                });
            }

            function canMap() {
                var canMap = true;
                for (var i = 0; i < genericBEGridView.Settings.Mappings.length; i++) {
                    var currentMapping = genericBEGridView.Settings.Mappings[i];
                    var parentFieldValue = fieldValues[currentMapping.ParentColumnName];
                    if (parentFieldValue == undefined) {
                        canMap = false;
                        break;
                    }
                }
                return canMap;
            }

            function buildFieldsMappingfields() {

                var fields = {};
                for (var i = 0; i < genericBEGridView.Settings.Mappings.length; i++) {
                    var currentMapping = genericBEGridView.Settings.Mappings[i];
                    var childName = currentMapping.SubviewColumnName;
                    var parentFieldValue = fieldValues[currentMapping.ParentColumnName];
                    if (parentFieldValue != undefined && childName != undefined) {
                        if (!isObjectValue && parentFieldValue.Value != undefined) {
                            fields[childName] = fillMappingFields(parentFieldValue.Value);
                        } else {
                            fields[childName] = fillMappingFields(parentFieldValue);
                        }
                    }
                }

                return fields;
            }

            function buildFiltersMappingfields() {
                var fields = {};
                for (var i = 0; i < genericBEGridView.Settings.Mappings.length; i++) {
                    var currentMapping = genericBEGridView.Settings.Mappings[i];
                    //  if (!currentMapping.ExcludeFromFilter) {
                    var childName = currentMapping.SubviewColumnName;
                    var parentFieldValue = fieldValues[currentMapping.ParentColumnName];
                    if (parentFieldValue != undefined && childName != undefined) {
                        if (!isObjectValue && parentFieldValue.Value != undefined) {
                            fields[childName] = fillMappingFields(parentFieldValue.Value);
                        } else {
                            fields[childName] = fillMappingFields(parentFieldValue);
                        }
                    }
                    // }
                }
                return fields;
            }

            function fillMappingFields(value) {
                return {
                    value: value,
                    isHidden: true,//currentMapping.IsVisibile?false:true,
                    isDisabled: true// currentMapping.IsEnabled?false:true
                };
            }
        }
    }

    app.directive('vrGenericdataGenericbeGenericbegridviewRuntime', GenericBESubviewDirective);

})(app);