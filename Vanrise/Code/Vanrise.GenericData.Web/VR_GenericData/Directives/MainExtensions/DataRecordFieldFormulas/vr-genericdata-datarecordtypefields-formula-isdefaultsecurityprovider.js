﻿'use strict';

app.directive('vrGenericdataDatarecordtypefieldsFormulaIsdefaultsecurityprovider', ['UtilsService', 
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new IsDefaultSecurityProvider(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/IsDefaultSecurityProviderFieldFormulaTemplate.html';
            }
        };

        function IsDefaultSecurityProvider(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {

                        if (payload.formula != undefined) {
                            $scope.scopeModel.fieldName = payload.formula.FieldName;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.IsDefaultSecurityProviderFieldFormula, Vanrise.GenericData.MainExtensions",
                        FieldName: $scope.scopeModel.fieldName,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);