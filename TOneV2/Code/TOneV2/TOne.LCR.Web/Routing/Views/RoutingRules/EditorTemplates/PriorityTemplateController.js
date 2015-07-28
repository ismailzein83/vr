appControllers.controller('RoutingRules_PriorityTemplateController',
    function PriorityController($scope, $http, CarrierAccountAPIService, RoutingRulesTemplatesEnum, UtilsService, CarrierTypeEnum) {

        defineScope();
        load();
        function defineScope() {


            $scope.suppliers = [];
            $scope.selectedSuppliers = [];
            $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };

            $scope.subViewActionDataConnector.getData = function () {
                return getActionData();
            }

            $scope.selectSupplier = function ($event, s) {
                $event.preventDefault();
                $event.stopPropagation();

                var index = UtilsService.getItemIndexByVal($scope.selectedSuppliers, s.CarrierAccountID, 'CarrierAccountID');

                if (index >= 0) {
                    $scope.selectedSuppliers.splice(index, 1);
                }
                else {
                    $scope.selectedSuppliers.push(s);
                }

            };

            $scope.fillOptionsData = function () {
                var tab = [];
                $.each($scope.selectedSuppliers, function (i, value) {
                    tab[i] = {
                        SupplierId: value.CarrierAccountID,
                        Priority: ($scope.selectedSuppliers.length) - i,
                        Force: (value.Force == true) ? true : false,
                        Percentage: value.Percentage
                    }

                });
                return tab;
            }

            $scope.subViewActionDataConnector.setData = function (data) {
                $scope.subViewActionDataConnector.data = data;
                loadForm();
            }
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
            var supplierOptopns = [];
            $.each(data.Options, function (i, value) {
                var existobj = UtilsService.getItemByVal($scope.suppliers, value.SupplierId, 'CarrierAccountID')
                if (existobj != null) {
                    supplierOptopns.push({
                        CarrierAccountID: value.SupplierId,
                        Name: existobj.Name,
                        Force: value.Force,
                        Percentage: data.Options[i].Percentage,
                        Priority: value.Priority
                    })
                }

            });
            $scope.selectedSuppliers = supplierOptopns;
        }

        function getActionData() {
            return {
                $type: RoutingRulesTemplatesEnum.PriorityTemplate.objectType,
                Options: $scope.fillOptionsData()
            }
        }

    });