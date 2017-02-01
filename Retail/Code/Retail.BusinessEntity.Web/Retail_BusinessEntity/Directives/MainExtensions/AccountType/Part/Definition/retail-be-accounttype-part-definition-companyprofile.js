'use strict';

app.directive('retailBeAccounttypePartDefinitionCompanyprofile', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeCompanyProfilePartDefinition = new AccountTypeCompanyProfilePartDefinition($scope, ctrl, $attrs);
            accountTypeCompanyProfilePartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeCompanyProfilePartDefinitionTemplate.html'
    };

    function AccountTypeCompanyProfilePartDefinition($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var counter = 0;
        var gridAPI;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.contactTypes = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.addContactType = function () {
                var dataItem = {
                    ContactId: counter + 1,
                    Name: undefined,
                    Title: undefined
                };
                counter++;
                $scope.scopeModel.contactTypes.push(dataItem);
            };

            $scope.scopeModel.removeContactType = function (contactTypeObj) {
                $scope.scopeModel.contactTypes.splice($scope.scopeModel.contactTypes.indexOf(contactTypeObj), 1);
            };

            $scope.scopeModel.validateContactTypes = function () {
                for (var i = 0; i < $scope.scopeModel.contactTypes.length; i++) {
                    var contactType = $scope.scopeModel.contactTypes[i];
                    for (var j = 0; j < $scope.scopeModel.contactTypes.length; j++) {
                        var currentContactType = $scope.scopeModel.contactTypes[j];
                        if (contactType.Name == currentContactType.Name && contactType.ContactId != currentContactType.ContactId)
                            return "Contact types with same name exist.";
                    }
                }
                if ($scope.scopeModel.contactTypes.length > 0)
                    return null;

                return "One contact type at least should be added.";

            };
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.scopeModel.contactTypes.length = 0;
                    if (payload.partDefinitionSettings != undefined && payload.partDefinitionSettings.ContactTypes) {
                        for (var i = 0; i < payload.partDefinitionSettings.ContactTypes.length; i++) {
                            var currentContactType = payload.partDefinitionSettings.ContactTypes[i];
                            currentContactType.ContactId = counter + 1;
                            counter++;
                            $scope.scopeModel.contactTypes.push(currentContactType);
                        }
                    }
                    $scope.scopeModel.includeArabicName = payload.partDefinitionSettings != undefined && payload.partDefinitionSettings.IncludeArabicName || false;
                }
            };

            api.getData = function () {
                var contactTypes = [];
                for (var i = 0; i < $scope.scopeModel.contactTypes.length; i++) {
                    var contactType = $scope.scopeModel.contactTypes[i];
                    contactTypes.push({
                        Name: contactType.Name,
                        Title: contactType.Title,
                        });
                }
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfileDefinition,Retail.BusinessEntity.MainExtensions',
                    ContactTypes: contactTypes,
                    IncludeArabicName: $scope.scopeModel.includeArabicName
                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
}]);