(function (app) {

    'use strict';

    VRPropertySelector.$inject = ['VR_Sec_UserProfilePropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function VRPropertySelector(VR_Sec_UserProfilePropertyEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var propertySelector = new PropertySelector($scope, ctrl, $attrs);
                propertySelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Security/Directives/Extensions/User/Templates/UserProfilePropertySelectorTemplate.html'
        };

        function PropertySelector($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var property; // property = {valueObjectName:valueObjectName, valuePropertyName:valuePropertyName}

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.selectedvalues;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var userProfileProperty;
                    if (payload != undefined && payload.userProfileProperty != undefined)
                        userProfileProperty = payload.userProfileProperty;

                    $scope.scopeModel.datasource = UtilsService.getArrayEnum(VR_Sec_UserProfilePropertyEnum); //UtilsService.getEnum(VR_Sec_UserProfilePropertyEnum, propertyFilter, valueFilter)

                    console.log($scope.scopeModel.datasource);

                    if(userProfileProperty) {
                        $scope.scopeModel.selectedvalues = UtilsService.getEnum(VR_Sec_UserProfilePropertyEnum, 'value', userProfileProperty)
                    }

                    //Loading ObjectSelector
                    //if (payload != undefined && payload.objects != undefined) {
                    //    for (var key in payload.objects)
                    //        $scope.scopeModel.objects.push(payload.objects[key]);

                    //    if (property != undefined) {
                    //        $scope.scopeModel.selectedObject = UtilsService.getItemByVal($scope.scopeModel.objects, property.objectName, 'ObjectName');

                    //        //In Case we have deleted the object
                    //        if ($scope.scopeModel.selectedObject == undefined) {
                    //            property = undefined;
                    //        }
                    //    }
                    //}

                };

                api.getData = function () {

                    var data;
                    if ($scope.scopeModel.selectedvalues != undefined) {
                        data = { userProfileProperty: $scope.scopeModel.selectedvalues };
                    }

                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrSecUserprofilepropertySelector', VRPropertySelector);

})(app);
