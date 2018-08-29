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

            var gridAPI;

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
                    $scope.scopeModel.isGridLoading = true;
                    if (payload != undefined) {
                        fieldValues = payload.parentBEEntity != undefined ? payload.parentBEEntity.FieldValues : undefined;
                        genericBEGridView = payload.genericBEGridView;
                        if (genericBEGridView != undefined && genericBEGridView.Settings != undefined) {
                            businessEntityDefinitionId = genericBEGridView.Settings.GenericBEDefinitionId;
                            payload = {
                                businessEntityDefinitionId: businessEntityDefinitionId,
                                fieldValues: buildMappingfields()
                            };
                        }


                    }


                    return gridAPI.load(payload).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });


                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function buildMappingfields() {

                var fields = {};
                for (var i = 0; i < genericBEGridView.Settings.Mappings.length; i++) {
                    var currentMapping = genericBEGridView.Settings.Mappings[i];
                    var childName = currentMapping.SubviewColumnName;

                    var parentFieldValue = fieldValues[currentMapping.ParentColumnName];
                    if (parentFieldValue != undefined && parentFieldValue.Value != undefined && childName!=undefined) {
                        fields[childName] = parentFieldValue.Value;
                    }
                }

                return fields;
            }
        }
    }

    app.directive('vrGenericdataGenericbeGenericbegridviewRuntime', GenericBESubviewDirective);

})(app);