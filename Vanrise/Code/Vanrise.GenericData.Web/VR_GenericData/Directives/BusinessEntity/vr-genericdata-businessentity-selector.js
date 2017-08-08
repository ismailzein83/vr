(function (app) {

    'use strict';

    BusinessEntitySelectorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function BusinessEntitySelectorDirective(UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '=',
                onselectionchanged: '=',
                ismultipleselection: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BusinessEntitySelectorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlBE",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            var showaddbutton = "";
            if (attrs.showaddbutton != undefined) {
                showaddbutton = "showaddbutton";
            }

            return '<span vr-loader="scopeModel.isLoadingDirective">' +
                        '<vr-directivewrapper directive="scopeModel.Editor" on-ready="scopeModel.onDirectiveReady" onselectionchanged="ctrlBE.onselectionchanged" ' + showaddbutton +
                            ' customvalidate="{{ctrlBE.customvalidate}}" normal-col-num="{{ctrlBE.normalColNum}}" isrequired="{{ctrlBE.isrequired}}" ' + multipleselection + '>' +
                        '</vr-directivewrapper>' +
                   '</span>';
        }

        function BusinessEntitySelectorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var businessEntityDefinitionId;
            var businessEntityDefinitionEntity, filter, beRuntimeSelectorFilter;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = { };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                            businessEntityDefinitionId: businessEntityDefinitionId,
                            filter: filter,
                            beRuntimeSelectorFilter: beRuntimeSelectorFilter
                        };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = { };

                api.load = function (payload) {

                    var promises = [];

                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        filter = payload.filter;
                        beRuntimeSelectorFilter = payload.beRuntimeSelectorFilter;
                    }

                    var businessEntityDefinitionPromise = getBusinessEntityDefinition(businessEntityDefinitionId);
                    promises.push(businessEntityDefinitionPromise);

                    if (selectedIds != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getBusinessEntityDefinition(businessEntityDefinitionId) {
                        return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(businessEntityDefinitionId).then(function(response) {
                            businessEntityDefinitionEntity = response;

                            if (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings != undefined)
                                $scope.scopeModel.Editor = businessEntityDefinitionEntity.Settings.SelectorUIControl;
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function() {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                    businessEntityDefinitionId: businessEntityDefinitionId,
                                    selectedIds: selectedIds,
                                    filter: filter,
                                    beRuntimeSelectorFilter: beRuntimeSelectorFilter
                                };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return directiveAPI.getSelectedIds();
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataBusinessentitySelector', BusinessEntitySelectorDirective);

}) (app);