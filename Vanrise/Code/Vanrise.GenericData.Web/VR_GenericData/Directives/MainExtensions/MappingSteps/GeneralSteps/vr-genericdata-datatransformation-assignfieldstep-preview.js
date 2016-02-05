﻿'use strict';
app.directive('vrGenericdataDatatransformationAssignfieldstepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new AssignFieldStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/Templates/AssignFieldStepPreviewTemplate.html';
            }

        };

        function AssignFieldStepCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        if (payload.stepDetails != undefined)
                        {
                            stepObj.stepDetails = payload.stepDetails;
                            stepObj.configId = payload.stepDetails.ConfigId;
                            ctrl.target = payload.stepDetails.Target;
                            ctrl.source = payload.stepDetails.Source;
                        }
                       checkValidation();
                    }
                 
                }

                api.applyChanges = function (changes) {
                    ctrl.target = changes.Target;
                    ctrl.source = changes.Source;
                    stepObj.stepDetails = changes;
                }

                api.checkValidation = function ()
                {
                  return  checkValidation();
                }

                api.getData = function () {
                    stepObj.stepDetails.ConfigId = stepObj.configId;
                    return stepObj.stepDetails      
                }
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.target == undefined || ctrl.source == undefined) {
                    return "Error";
                }
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);