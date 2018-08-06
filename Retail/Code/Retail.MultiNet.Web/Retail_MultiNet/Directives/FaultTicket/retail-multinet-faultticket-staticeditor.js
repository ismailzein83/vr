'use strict';

app.directive('retailMultinetFaultticketStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailMultinetFaultticketStaticeditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Directives/FaultTicket/Templates/FaultTicketStaticEditor.html"
        };

        function retailMultinetFaultticketStaticeditor(ctrl, $scope, $attrs) {
          

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.codeNumberList = [];

               
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {                   

                };

                api.setData = function (faultTicketObject) {
                    
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }
        return directiveDefinitionObject;
    }]);