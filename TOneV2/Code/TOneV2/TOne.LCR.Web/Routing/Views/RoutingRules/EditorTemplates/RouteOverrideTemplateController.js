appControllers.controller('RoutingRules_RouteOverrideTemplateController',
    function RouteOverrideController($scope, $http, CarrierAccountAPIService, RoutingRulesTemplatesEnum, UtilsService, CarrierTypeEnum) {

        defineScope();
        load();
        function defineScope() {

            $scope.suppliers = [];
            $scope.selectedSuppliers = [];

            $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };

            $scope.subViewActionDataConnector.getData = function () {
                return getActionData()

            };

            $scope.subViewActionDataConnector.setData = function (data) {
                $scope.subViewActionDataConnector.data = data;
                loadForm();
            }

            $scope.selectSupplier = function ($event, s) {
                $event.preventDefault();
                $event.stopPropagation();
                var index = null;
                try {
                    var index = UtilsService.getItemIndexByVal($scope.selectedSuppliers, s.CarrierAccountID, 'CarrierAccountID');
                }
                catch (e) {

                }
                if (index >= 0) {
                    $scope.selectedSuppliers.splice(index, 1);
                }
                else
                    $scope.selectedSuppliers.push(s);
            };
        }

        function load() {
            CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
                $scope.suppliers = response;
                if ($scope.subViewActionDataConnector.data != undefined) {
                    loadForm();
                }
            })
        }

        function loadForm() {
            if ($scope.subViewActionDataConnector.data == undefined)
                return;

            if ($scope.suppliers == undefined || $scope.suppliers.length == 0)
                return;
            var data = $scope.subViewActionDataConnector.data;
            var supplierOptions = [];
            $.each(data.Options, function (i, value) {
                $scope.selectedSuppliers.length = 0;
                var existobj = UtilsService.getItemByVal($scope.suppliers, value.SupplierId, 'CarrierAccountID')
                if (existobj != null) {
                    supplierOptions.push({
                        CarrierAccountID: value.SupplierId,
                        Name: existobj.Name,
                        AllowLoss: data.Options[i].AllowLoss,
                        Percentage: data.Options[i].Percentage
                    });
                }
            });
            $scope.selectedSuppliers = supplierOptions;
        }

        function getActionData() {
            return {
                $type: RoutingRulesTemplatesEnum.OverrideTemplate.objectType,
                Options: fillOptionsData()
            }

        }

        function fillOptionsData() {
            var tab = [];
            $.each($scope.selectedSuppliers, function (i, value) {
                tab[i] = {
                    SupplierId: value.CarrierAccountID,
                    Percentage: value.Percentage,
                    AllowLoss: value.AllowLoss
                }

            });
            return tab;
        }

    });