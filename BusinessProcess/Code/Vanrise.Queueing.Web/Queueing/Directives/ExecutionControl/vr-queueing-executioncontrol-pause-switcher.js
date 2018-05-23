'use strict';
app.directive('vrQueueingExecutioncontrolPauseSwitcher', ['VR_Queueing_ExecutionControlDataAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Queueing_ExecutionControlDataAPIService, UtilsService, VRUIUtilsService) {



        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {               
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new executionControlCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,           
            template: function (element, attrs) {
                return getExecutionControl(attrs);
            }

        };


        function getExecutionControl(attrs) {
            return ' <vr-button type="Pause" data-onclick="ctrl.pauseExecution" vr-disabled="!ctrl.isPaused"></vr-button>'
                    + ' <vr-button type="Resume" data-onclick="ctrl.resumeExecution" vr-disabled="ctrl.isPaused"></vr-button>';
        }

        function executionControlCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            ctrl.isPaused = false;

            ctrl.pauseExecution = function () {
                return VR_Queueing_ExecutionControlDataAPIService.UpdateExecutionPaused(true).then(function (response) {
                    ctrl.isPaused = !response;
                });
            };

            ctrl.resumeExecution = function () {
                return VR_Queueing_ExecutionControlDataAPIService.UpdateExecutionPaused(false).then(function (response) {
                    ctrl.isPaused = !response;
                });
            };
            function initializeController() {
                VR_Queueing_ExecutionControlDataAPIService.IsExecutionPaused().then(function (response) {
                    $scope.isPaused = response;
                });
            }
          
        }

        return directiveDefinitionObject;
    }]);