(function () {
    'use strict';

    angular.module('OutOfRangeApp')
    .config(['$routeProvider', '$locationProvider', '$localStorageProvider', function ($routeProvider, $locationProvider, $localStorageProvider) {
        $routeProvider
            .when('/', {
                templateUrl: '/templates/front-page.html',
                controller: 'HomeCtrl'
            })
            .when('/register', {
                templateUrl: '/templates/Auth/register.html',
                controller: 'RegisterCtrl'
            })
            .when('/login', {
                templateUrl: '/templates/Auth/login.html',
                controller: 'LoginCtrl',
                redirectTo: function (params, path, search) {

                    if ($localStorageProvider.get('authData'))
                        return params.redirectUrl ? decodeURIComponent(params.redirectUrl) : '/';
                }
            })
            .when('/logout', {
                controller: 'LogoutCtrl',
                template: ''
            })
            .otherwise({
                templateUrl: '/templates/404.html'
            });
        $locationProvider.html5Mode(true);
    }]);
})();