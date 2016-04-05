(function () {
    'use strict';

    angular.module('OutOfRangeApp', ['ngRoute', 'ngResource', 'ngStorage'])
    .run(['$rootScope', '$location', '$routeParams', 'authService', function ($rootScope, $location, $routeParams, authService) {
        $rootScope.$on('$routeChangeStart', function (event, next) {
            var isAuthentificated = authService.getAuthentificationInfo().isAuth;

            if (next.requiresLogin && !isAuthentificated) {
                var oldUrl = $location.path()
                $location.path('/login').search('returnUrl', encodeURIComponent(oldUrl));
            }

            if (next.guestOnly && isAuthentificated) {
                var returnUrl = $routeParams.returnUrl ? decodeURIComponent($routeParams.returnUrl) : '/';
                $location.path(returnUrl).search('returnUrl', null);
            }
        });
    }]);
})();