(function (app) {

    'use strict';

    AccountGenericFieldDefinitionSelector.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService'];

    function AccountGenericFieldDefinitionSelector(UtilsService, VRUIUtilsService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AccountGenericFieldDefinitionSelectorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountGenericFieldDefinitionSelectorTemplate.html"
        };

        function AccountGenericFieldDefinitionSelectorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.genericFieldDefinitions = [];

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var genericFieldDefinition;

                    if (payload != undefined) {
                        genericFieldDefinition = payload.genericFieldDefinition;
                    }

                    var selectorLoadPromise = getSelectorLoadPromise();
                    promises.push(selectorLoadPromise);


                    function getSelectorLoadPromise() {

                        return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.genericFieldDefinitions.push(response[i]);
                                }
                                if (genericFieldDefinition != undefined) {
                                    $scope.scopeModel.selectedGenericFieldDefinition =
                                        UtilsService.getItemByVal($scope.scopeModel.genericFieldDefinitions, genericFieldDefinition.Name, 'Name');
                                }
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    //var data = {
                    //    genericFieldDefinition: $scope.scopeModel.selectedGenericFieldDefinition
                    //};

                    var obj = $scope.scopeModel.selectedGenericFieldDefinition;
                    console.log(obj);
                    //return $scope.scopeModel.selectedGenericFieldDefinition;
                    if (obj != undefined)
                        return obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('retailBeAccountGenericfielddefinitionSelector', AccountGenericFieldDefinitionSelector);

})(app);
