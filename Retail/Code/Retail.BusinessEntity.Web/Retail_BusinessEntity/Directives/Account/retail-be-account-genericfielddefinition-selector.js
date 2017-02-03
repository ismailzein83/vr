(function (app) {

    'use strict';

    AccountGenericFieldDefinitionSelector.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService'];

    function AccountGenericFieldDefinitionSelector(UtilsService, VRUIUtilsService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountGenericFieldDefinitionSelectorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function AccountGenericFieldDefinitionSelectorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = { };
                ctrl.genericFieldDefinitions =[];

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = { };

                api.load = function (payload) {

                    var promises = [];

                    var accountBEDefinitionId;
                    var genericFieldDefinition;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        genericFieldDefinition = payload.genericFieldDefinition;
                    }

                    var selectorLoadPromise = getSelectorLoadPromise();
                    promises.push(selectorLoadPromise);


                    function getSelectorLoadPromise() {

                        return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo(accountBEDefinitionId).then(function(response) {
                            if(response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.genericFieldDefinitions.push(response[i]);
                                }
                                if (genericFieldDefinition != undefined) {
                                    ctrl.selectedGenericFieldDefinition =
                                        UtilsService.getItemByVal(ctrl.genericFieldDefinitions, genericFieldDefinition.Name, 'Name');
                                }
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = ctrl.selectedGenericFieldDefinition;
                    if (obj == undefined)
                        return;

                    return {
                            Name: obj.Name,
                            Title: obj.Title,
                            FieldType: obj.FieldType
                        };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Account Field";

            if (attrs.ismultipleselection != undefined) {
                label = "Account Fields";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' 
                      + '<vr-select ' + multipleselection + ' datatextfield="Title" datavaluefield="Name" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.genericFieldDefinitions" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedGenericFieldDefinition" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + ' hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' 
                       + '</vr-select>'
                   + '</vr-columns>';
        }
    }

    app.directive('retailBeAccountGenericfielddefinitionSelector', AccountGenericFieldDefinitionSelector);

}) (app);
