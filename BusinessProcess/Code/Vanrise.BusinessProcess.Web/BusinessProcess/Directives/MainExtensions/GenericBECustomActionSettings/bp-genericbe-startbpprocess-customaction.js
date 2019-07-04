//"use strict";

//app.directive("bpGenericbeStartbpprocessCustomaction", ["UtilsService", "VRUIUtilsService", 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_VRWorkflowAPIService', 'VRNotificationService', 'VRWorkflowArgumentDirectionEnum',
//    function (UtilsService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_VRWorkflowAPIService, VRNotificationService, VRWorkflowArgumentDirectionEnum) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new StartBPProcessCustomActionCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/GenericBECustomActionSettings/Templates/StartBPProcessCustomActionTemplate.html"
//        };

//        function StartBPProcessCustomActionCtor($scope, ctrl) {
//            this.initializeController = initializeController;

//            function initializeController() {
//                $scope.scopeModel = {};
          
//                defineAPI();
//            }

//            function defineAPI() {
//            }

//        return directiveDefinitionObject;
//    }]);