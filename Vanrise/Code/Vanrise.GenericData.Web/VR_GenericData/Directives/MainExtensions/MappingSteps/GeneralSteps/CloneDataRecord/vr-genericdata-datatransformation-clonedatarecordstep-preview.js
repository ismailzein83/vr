'use strict';

app.directive('vrGenericdataDatatransformationClonedatarecordstepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new CloneDataRecordStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/CloneDataRecord/Templates/CloneDataRecordStepPreviewTemplate.html';
            }

        };

        function CloneDataRecordStepCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            ctrl.source = payload.stepDetails.SourceRecordName;
                            ctrl.target = payload.stepDetails.TargetRecordName;
                            stepObj.stepDetails = payload.stepDetails;
                        }
                        checkValidation();
                    }
                };

                api.applyChanges = function (changes) {
                    ctrl.source = changes.SourceRecordName;
                    ctrl.target = changes.TargetRecordName;
                    stepObj.stepDetails = changes;
                };

                api.checkValidation = function () {
                    return checkValidation();
                };

                api.getData = function () {
                    return stepObj.stepDetails
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.source == undefined) {
                    return "Missing source mapping.";
                }
                if (ctrl.target == undefined) {
                    return "Missing target mapping.";
                }
                return null;
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);