(function (app) {

    'use strict';

    RetailAccountPropertyEvaluator.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService'];

    function RetailAccountPropertyEvaluator(UtilsService, VRUIUtilsService, Retail_BE_AccountTypeAPIService) {
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
                var ctor = new RetailAccountPropertyEvaluatorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/RetailAccountPropertyEvaluatorTemplate.html"
        };

        function RetailAccountPropertyEvaluatorCtor($scope, ctrl, $attrs) {
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
                    
                    if (payload != undefined && payload.objectPropertyEvaluator != undefined) {
                        genericFieldDefinition = payload.objectPropertyEvaluator.GenericFieldDefinition;
                    }

                    var selectorLoadPromise = getSelectorLoadPromise();
                    promises.push(selectorLoadPromise);


                    function getSelectorLoadPromise() {

                        return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitions().then(function (response) {
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

                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.VRObjectTypes.RetailAccountPropertyEvaluator, Retail.BusinessEntity.MainExtensions",
                        GenericFieldDefinition: $scope.scopeModel.selectedGenericFieldDefinition
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('retailBeRetailaccountPropertyevaluator', RetailAccountPropertyEvaluator);

})(app);
