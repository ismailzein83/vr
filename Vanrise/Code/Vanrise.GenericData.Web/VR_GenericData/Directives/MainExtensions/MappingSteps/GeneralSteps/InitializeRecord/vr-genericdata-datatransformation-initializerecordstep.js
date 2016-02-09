'use strict';
app.directive('vrGenericdataDatatransformationInitializerecordstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new InitializeRecordStepCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/InitializeRecord/Templates/InitializeRecordStepTemplate.html';
            }

        };

        function InitializeRecordStepCtor(ctrl, $scope) {
            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                    {
                        if(payload.context != undefined)
                            ctrl.records = payload.context.getAllRecordNames();
                        if (payload.stepDetails != undefined)
                            ctrl.selectedRecordName = UtilsService.getItemByVal(ctrl.records, payload.stepDetails.RecordName, "Name");
                    }
                   

                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.InitializeRecordStep, Vanrise.GenericData.Transformation.MainExtensions",
                        RecordName: ctrl.selectedRecordName !=undefined? ctrl.selectedRecordName.Name:undefined,
                        
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);