"use strict";

app.directive("qmClitesterTestcall", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'Qm_CliTester_TestCallAPIService', function (UtilsService, VRUIUtilsService, VRNotificationService , Qm_CliTester_TestCallAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            
            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/QM_CliTester/Directives/MainExtensions/ITest/Templates/TestCallTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        
        var countryDirectiveAPI;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var zoneDirectiveAPI;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var profileDirectiveAPI;
        var profileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sourceTypeDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        function initializeController() {
            $scope.countries = [];
            
            $scope.zones = [];

            $scope.suppliers = [];

            $scope.selectedSupplier = [];

            $scope.onProfileDirectiveReady = function (api) {
                profileDirectiveAPI = api;
                profileReadyPromiseDeferred.resolve();
            }

            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            }

            $scope.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            }

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveAPI = api;
                countryReadyPromiseDeferred.resolve();
            }

            $scope.onCountrySelectItem = function (selectedItem) {
                if (selectedItem != undefined) {
                    var setLoader = function (value) { $scope.isLoadingZoneSelector = value };
                    var payload = {
                        filter: {
                            CountryId: selectedItem.CountryId
                        }
                    }
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader);
                }
            }

            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTypeDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            }

            $scope.scopeModal = {
                emails: []
            };

            $scope.scopeModal.disabledemail = true;
            $scope.scopeModal.onEmailValueChange = function (value) {
                $scope.scopeModal.disabledemail = (value == undefined);
            }

            $scope.addEmailOption = function () {
                var email = $scope.scopeModal.emailvalue;
                $scope.scopeModal.emails.push({
                    email: email
                });
                $scope.scopeModal.emailvalue = undefined;
                $scope.scopeModal.disabledemail = true;
            };


            defineAPI();
        }

        function defineAPI() {
           
            
            var api = {};
            api.getData = function () {
                return {
                    $type: "QM.CLITester.Business.TestCallTaskActionArgument, QM.CLITester.Business",
                    AddTestCallInput: buildTestCallObjFromScope()
                };
                
            };

            function buildTestCallObjFromScope() {
                var listEmailsObj = "";
               
                for (var i = 0; i < $scope.scopeModal.emails.length; i++) {
                    listEmailsObj = listEmailsObj + $scope.scopeModal.emails[i].email + ";";
                }

                var obj = {
                    SupplierID: UtilsService.getPropValuesFromArray($scope.selectedSupplier, "SupplierId"),
                    CountryID: $scope.selectedCountry.CountryId,
                    ZoneID: $scope.selectedZone.ZoneId,
                    ProfileID: $scope.selectedProfile.ProfileId,
                    ListEmails: listEmailsObj
                };
                return obj;
            }

            api.load = function (payload) {
                var promises = [];

                var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                profileReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.AddTestCallInput)
                            directivePayload = {
                                selectedIds: payload.data.AddTestCallInput.ProfileID
                            }
                        VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
                    });

                promises.push(profileLoadPromiseDeferred.promise);

                var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.AddTestCallInput)
                            directivePayload = {
                                selectedIds: payload.data.AddTestCallInput.SupplierID
                            }
                        VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                    });

                promises.push(supplierLoadPromiseDeferred.promise);

                var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                countryReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.AddTestCallInput)
                            directivePayload = {
                                selectedIds: payload.data.AddTestCallInput.CountryID
                            }
                        VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, directivePayload, countryLoadPromiseDeferred);
                    });

                promises.push(countryLoadPromiseDeferred.promise);

                if(payload != undefined && payload.data != undefined && payload.data.AddTestCallInput && payload.data.AddTestCallInput.CountryID != undefined)
                {
                     var zoneLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                zoneReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                            directivePayload = {
                                filter: {
                                    CountryId: payload.data.CountryID
                                }
                            }
                            if (payload.data.AddTestCallInput.ZoneID != undefined)
                                directivePayload.selectedIds = payload.data.AddTestCallInput.ZoneID;

                            VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, directivePayload, zoneLoadPromiseDeferred);

                    });
                      promises.push(zoneLoadPromiseDeferred.promise);
                }

                if (payload != undefined && payload.data != undefined && payload.data.AddTestCallInput && payload.data.AddTestCallInput.ListEmails != undefined) {
                    var listEmails = payload.data.AddTestCallInput.ListEmails.split(";");

                    for (var i = 0; i < listEmails.length; i++) {
                        if (i != listEmails.length - 1) {
                            $scope.scopeModal.emails.push({
                                email: listEmails[i]
                            });
                        }
                    }
                }

                return UtilsService.waitMultiplePromises(promises);
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
