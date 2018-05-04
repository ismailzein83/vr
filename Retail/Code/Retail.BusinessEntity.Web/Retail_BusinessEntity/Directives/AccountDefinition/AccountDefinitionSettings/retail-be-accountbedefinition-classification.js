(function (app) {

    'use strict';

    retailBeAccountBeDefinitionClassificationDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function retailBeAccountBeDefinitionClassificationDirective(UtilsService, VRUIUtilsService) {
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
                var ctor = new AccountClassificationsDirective($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountDefinitionSettings/Templates/AccountBEClassificationTemplate.html'

        };

        function AccountClassificationsDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                ctrl.onAddClassification = function () {
                    var dataItem;
                    dataItem = {
                        id: ctrl.datasource.length + 1,
                        title: "",
                        name: ""
                    };
                    ctrl.datasource.push(dataItem);
                };

                ctrl.isNameValid = function () {
                    
                   for (var x = 0; x < ctrl.datasource.length; x++) {
                        var currentItem = ctrl.datasource[x];
                        for (var y = x + 1; y < ctrl.datasource.length; y++) {
                            var dataItem = ctrl.datasource[y];
                            if (dataItem.name === currentItem.name)
                                return 'This name already exists';
                        }
                    }
                    return null;
                };

                ctrl.removeRow = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                    ctrl.datasource.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    
                    var promises = [];

                    if (payload != undefined && payload.classifications != undefined) {

                         for (var i = 0; i < payload.classifications.length; i++) {
                            var item = {
                                payload: payload.classifications[i]
                            };
                            addItemToGrid(item);
                        }
                    }

                    function addItemToGrid(item) {
                        
                        var dataItem = {
                            id: ctrl.datasource.length + 1,
                            title: item.payload.Title,
                            name: item.payload.Name
                        };
                        
                        ctrl.datasource.push(dataItem);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var classifications = [];
                    angular.forEach(ctrl.datasource, function (item) {
                        var dataItem = {
                            Title: item.title,
                            Name: item.name
                        };
                        classifications.push(dataItem);
                    });

                    return classifications;
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

    }

    app.directive('retailBeAccountbedefinitionClassification', retailBeAccountBeDefinitionClassificationDirective);

})(app);