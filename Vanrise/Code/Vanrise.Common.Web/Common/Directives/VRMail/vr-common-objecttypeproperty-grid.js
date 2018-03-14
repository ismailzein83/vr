'use strict';

app.directive('vrCommonObjecttypepropertyGrid', ['VRCommon_VRObjectTypeDefinitionAPIService', 'VRNotificationService', function (VRCommon_VRObjectTypeDefinitionAPIService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var objectTypePropertyGrid = new ObjectTypePropertyGrid($scope, ctrl, $attrs);
            objectTypePropertyGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Common/Directives/VRMail/Templates/VRObjectTypePropertyGridTemplate.html'
    };

    function ObjectTypePropertyGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;
        var objectVariable;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.objectTypeProperties = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.copyValueExpression = function (dataItem, coldef, event) {
              
                var copyElement = document.createElement("textarea");
                copyElement.style.position = 'fixed';
                copyElement.style.opacity = '0';
                copyElement.textContent = dataItem.ValueExpression;
                var body = document.getElementsByTagName('body')[0];
                body.appendChild(copyElement);
                copyElement.select();
                document.execCommand('copy');
                body.removeChild(copyElement);
                var self = angular.element(event.currentTarget);
                var divParent = $(self).parent();

                var selfOffset = $(divParent).offset();
                var eltop = selfOffset.top - $(window).scrollTop();
                var elleft = selfOffset.left - $(window).scrollLeft();
                setTimeout(function () {
                    $(divParent).append('<span id="quickMsg"  class="alert alert-info vr-quickmsg">Expression Copied</span>');
                    $("#quickMsg").css({ position: 'fixed', top: eltop, left: elleft });
                    $(divParent).find("#quickMsg").fadeIn(300);
                }, 100);
               
                setTimeout(function () {
                    $(divParent).find("#quickMsg").fadeOut(2000, function () {
                        $(divParent).find("#quickMsg").remove();
                    });
                },100)
            };
            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined)
                    objectVariable = payload.objectVariable;

                return VRCommon_VRObjectTypeDefinitionAPIService.GetVRObjectTypeDefinition(objectVariable.VRObjectTypeDefinitionId).then(function (response) {

                    var objectTypeDefinition = response;
                    var properties = objectTypeDefinition.Settings.Properties;
                    var property;

                    for (var key in properties) 
                        if (key != "$type") {
                            property = properties[key];
                            extendVariableObject(property);
                            $scope.scopeModel.objectTypeProperties.push(property);
                        }          
                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function extendVariableObject(property) {

            property.ValueExpression = "@Model.GetVal(\"" + objectVariable.ObjectName + "\",\"" + property.Name + "\")";
            property.title = "Click To Copy Expression";
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions = [];
        }
    }
}]);
