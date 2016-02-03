(function (app) {

    'use strict';

    BusinessEntityDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function BusinessEntityDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var businessEntity = new BusinessEntity(ctrl, $scope);
                businessEntity.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFields/Templates/BusinessEntityDirectiveTemplate.html';
            }
        };

        function BusinessEntity(ctrl, $scope) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;

                    if (payload != undefined) {
                        selectedId = payload.BusinessEntityDefinitionId;
                    }

                    return loadSelector();

                    function loadSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = { selectedIds: selectedId };
                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                        return selectorLoadDeferred.promise;
                    }
                }

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions',
                        BusinessEntityDefinitionId: selectorAPI.getSelectedIds()
                    };
                }

                return api;
            }
        }
    }

    app.directive('vrGenericdataDatarecordfieldtypeBusinessentity', BusinessEntityDirective);

})(app);