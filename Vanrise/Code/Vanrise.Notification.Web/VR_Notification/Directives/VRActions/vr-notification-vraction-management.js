'use strict';
app.directive('vrNotificationVractionManagement', ['UtilsService','VR_Notification_VRActionService',
function (UtilsService, VR_Notification_VRActionService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new VRActionsManagement(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Notification/Directives/VRActions/Templates/VRActionsManagementTemplate.html"

    };


    function VRActionsManagement(ctrl, $scope, $attrs) {

        function initializeController() {

            ctrl.datasource = [];

            ctrl.isValid = function () {

                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should Select at least one action.";
            }

            ctrl.addVRAction = function () {
                var onVRActionAdded = function(action)
                {
                    ctrl.datasource.push({ Entity: action });
                }
                VR_Notification_VRActionService.addVRAction(onVRActionAdded);

            };

            ctrl.removeVRAction = function (dataItem) {
                var index = ctrl.datasource.indexOf(dataItem);
                ctrl.datasource.splice(index, 1);
            };

            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var data;
                if(ctrl.datasource.length > 0)
                {
                    data = [];
                    for(var i=0;i<ctrl.datasource.length;i++)
                    {
                        data.push(ctrl.datasource[i].Entity);
                    }

                }
                return data;
            }

            api.load = function (payload) {

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);