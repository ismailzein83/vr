var RouteManagerController = function ($scope, RoutingAPIService, CarriersService, ZonesService) {

    var pageSize = 40;
    var page = 0;
    var pageUp = 0;
    var last = false;

    defineScopeObjects();
    defineScopeMethods();
    load();


    function defineScopeObjects() {
        $scope.numberoflines = 10;
        $scope.isloadingdata = false;


        $scope.optionsZonesFilter = {
            selectedvalues: [],
            datasource: []
        };

        $scope.optionsCustomersF = {
            selectedvalues: [],
            datasource: []
        };


    }
    function defineScopeMethods() {
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
    }

    function load() {
        $('.action-bar-ddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });
        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('.action-bar-ddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        var dropdownHidingTimeoutHandleropt;
        $('#optionddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#optionddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });

        $('.option-filter').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandleropt);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });

        $('.option-filter').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandleropt = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });



        //$scope.getDatalist(page, pageSize);

        CarriersService.getCustomers().then(function (response) {
            $scope.optionsCustomersF.datasource = response;

        })
    }


}

RouteManagerController.$inject = ['$scope', 'RoutingAPIService', 'CarriersService', 'ZonesService'];

appControllers.controller('RouteManagerController', RouteManagerController)

