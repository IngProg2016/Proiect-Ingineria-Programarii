(function () {
    'use strict';

    angular.module('OutOfRangeApp')
    .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider
            .when('/register', {
                templateUrl: '/templates/Auth/register.html',
                controller: 'RegisterCtrl'
            });
        $locationProvider.html5Mode(true);
    }]);
})();