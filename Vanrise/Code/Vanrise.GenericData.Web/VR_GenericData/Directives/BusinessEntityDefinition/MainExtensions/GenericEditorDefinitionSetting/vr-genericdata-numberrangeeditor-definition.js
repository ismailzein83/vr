//(function (app) {

//    'use strict';

//    NumberRangeGenericBEDefinitionViewDirective.$inject = ['UtilsService', 'VRNotificationService'];

//    function NumberRangeGenericBEDefinitionViewDirective(UtilsService, VRNotificationService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                normalColNum: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new NumberRangeGenericBEDefinitionViewCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericBENumberRangeEditorDefinitionTemplate.html'
//        };

//        function NumberRangeGenericBEDefinitionViewCtor($scope, ctrl) {
//            this.initializeController = initializeController;
//            function initializeController() {
//                $scope.scopeModel = {};
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var settings;
//                    $scope.scopeModel.rangeVariableName = "GeneratedRangeInfo";

//                    if (payload != undefined)
//                        settings = payload.settings;

//                    if (settings != undefined)
//                        $scope.scopeModel.rangeVariableName = settings.RangeVariableName;

//                };


//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericEditorDefinitionSetting.NumberRangeGenericEditorDefinition, Vanrise.GenericData.MainExtensions",
//                        RangeVariableName: $scope.scopeModel.rangeVariableName
//                    };
//                };
//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataNumberrangeeditorDefinition', NumberRangeGenericBEDefinitionViewDirective);

//})(app);