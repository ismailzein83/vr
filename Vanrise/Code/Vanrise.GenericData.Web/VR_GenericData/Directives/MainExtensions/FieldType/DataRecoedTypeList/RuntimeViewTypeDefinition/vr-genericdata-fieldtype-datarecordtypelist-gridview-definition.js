'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGridviewDefinition', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new gridViewTypeListTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeDefinition/Templates/FieldViewTypeDefinitionTemplate.html';
            }
        };

        function gridViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};

            function initializeController() {

                defineAPI();

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.GridViewListRecordRuntimeViewType, Vanrise.GenericData.MainExtensions",
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);