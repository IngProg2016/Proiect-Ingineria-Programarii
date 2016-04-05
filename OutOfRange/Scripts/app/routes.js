(function () {
    'use strict';

    angular.module('OutOfRangeApp')
    .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                templateUrl: '/templates/front-page.html',
                controller: 'MainCtrl'
            })
            .when('/register', {
                templateUrl: '/templates/Auth/register.html',
                controller: 'RegisterCtrl'
            })
            .when('/login', {
                templateUrl: '/templates/Auth/login.html',
                controller: 'LoginCtrl'
            })
            .otherwise({
                templateUrl: '/templates/404.html'
            });
        $locationProvider.html5Mode(true);
    }]);
})();