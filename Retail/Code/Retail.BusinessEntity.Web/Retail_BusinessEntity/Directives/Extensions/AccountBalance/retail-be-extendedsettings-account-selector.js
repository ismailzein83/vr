﻿'use strict';
app.directive('retailBeExtendedsettingsAccountSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new accountsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'accountCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {
            return '<retail-be-account-selector isrequired="accountCtrl.isrequired" normal-col-num="{{accountCtrl.normalColNum}}"  on-ready="accountCtrl.onDirectiveReady" ></retail-be-account-selector>';
        }

        function accountsCtor(ctrl, $scope, attrs) {

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                ctrl.onDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    directiveReadyPromiseDeferred.resolve();
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }
                    var promises = [];

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload;
                        if (selectedIds != undefined) {
                            selectorPayload = {
                                selectedIds: selectedIds
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, selectorPayload, directiveLoadPromiseDeferred);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = directiveReadyAPI.getSelectedIds();
                    return {
                        selectedIds: data != undefined ? data.selectedIds : undefined,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
           
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);