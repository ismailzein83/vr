'use strict';

app.directive('vrCommonCompanycontactSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/GeneralTechnicalSettings/Templates/CompanyContactSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

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
                $scope.scopeModel.validateCompanyContacts = function () {
                    //if ($scope.scopeModel.contactTypes.length == 0)
                    //    return "You must add at least add one contact type.";
                    if (!validateNameIdentity())
                        return "Contact type name should be identical.";
                    return null;
                };
                $scope.scopeModel.removeContactType = function (contactTypeObj) {
                    $scope.scopeModel.contactTypes.splice($scope.scopeModel.contactTypes.indexOf(contactTypeObj), 1);
                };

                function validateNameIdentity() {
                    var contacts = $scope.scopeModel.contactTypes;
                    var contactlength = contacts.length;
                    for (var i = 0; i < contactlength ; i++) {
                        var currentContact = contacts[i];
                        for (var j = 0 ; j < contactlength ; j++) {
                            if (j != i && currentContact.Name == contacts[j].Name)
                                return false;
                        }
                    }
                    return true;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.contactTypes.length = 0;
                        if (payload.data != undefined && payload.data.ContactTypes != undefined) {
                            for (var i = 0; i < payload.data.ContactTypes.length; i++) {
                                var currentContactType = payload.data.ContactTypes[i];
                                currentContactType.ContactId = counter + 1;
                                counter++;
                                $scope.scopeModel.contactTypes.push(currentContactType);
                            }
                        }
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
                        ContactTypes: contactTypes.length > 0 ? contactTypes : undefined
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);